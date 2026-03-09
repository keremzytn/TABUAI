using AutoMapper;
using MediatR;
using TabuAI.Application.Common.DTOs;
using TabuAI.Application.Common.Services;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Game.Commands;

public class SubmitPromptCommandHandler : IRequestHandler<SubmitPromptCommand, GameResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiService _aiService;
    private readonly IBadgeService _badgeService;
    private readonly IMapper _mapper;

    public SubmitPromptCommandHandler(IUnitOfWork unitOfWork, IAiService aiService, IBadgeService badgeService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _aiService = aiService;
        _badgeService = badgeService;
        _mapper = mapper;
    }

    public async Task<GameResultDto> Handle(SubmitPromptCommand request, CancellationToken cancellationToken)
    {
        // Parse GameSessionId
        if (!Guid.TryParse(request.GameSessionId, out var gameSessionId))
        {
            throw new ArgumentException("Invalid game session ID format");
        }

        // Get game session with word and attempts
        var gameSession = await _unitOfWork.GameSessions.GetByIdAsync(gameSessionId);
        if (gameSession == null)
        {
            throw new InvalidOperationException("Game session not found");
        }

        if (gameSession.Status != GameStatus.InProgress)
        {
            throw new InvalidOperationException("Game session is already completed");
        }

        // Get attempts count
        var currentAttempts = await _unitOfWork.GameAttempts.FindAsync(a => a.GameSessionId == gameSessionId);
        int attemptCount = currentAttempts.Count();

        if (attemptCount >= 3)
        {
            throw new InvalidOperationException("Maximum number of attempts (3) has been reached for this word.");
        }

        var word = await _unitOfWork.Words.GetByIdAsync(gameSession.WordId);
        if (word == null)
        {
            throw new InvalidOperationException("Word not found");
        }

        // Check for tabu words in prompt
        var promptAnalysis = await _aiService.AnalyzePromptAsync(
            request.Prompt, 
            word.TargetWord, 
            word.TabuWords
        );

        if (promptAnalysis.ContainsTabuWords)
        {
            // Even if tabu, it's an attempt? 
            // Usually tabu words don't count towards the 3 tries, the user just gets warned.
            // But let's see. The user said 3 tries. 
            // I'll count it as a try but mark it as failed. 
            // Or maybe tabu shouldn't count? Let's not count tabu as a "try" towards the 3 limit, 
            // but return failure immediately for UI to show warning.
            
            return new GameResultDto
            {
                IsCorrect = false,
                AiGuess = "",
                AiFeedback = $"Tabu kelimeler kullandınız: {string.Join(", ", promptAnalysis.DetectedTabuWords)}",
                Score = 0,
                PromptQuality = 1,
                Suggestions = new List<string> { "Tabu kelimeleri kullanmadan tekrar deneyin." },
                GameCompleted = false
            };
        }

        // Premium model costs coins
        if (request.AiModel == "premium")
        {
            var premiumUser = await _unitOfWork.Users.GetByIdAsync(gameSession.UserId);
            if (premiumUser != null && premiumUser.PromptCoins >= 25)
            {
                premiumUser.PromptCoins -= 25;
                await _unitOfWork.Users.UpdateAsync(premiumUser);
                await _unitOfWork.CoinTransactions.AddAsync(new CoinTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = premiumUser.Id,
                    Amount = -25,
                    Type = CoinTransactionType.HintPurchase,
                    Description = "Premium AI modeli kullanıldı",
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                request.AiModel = null;
            }
        }

        // Get AI guess (with persona if specified)
        var aiResult = request.AiModel == "premium"
            ? await _aiService.GuessWordWithModelAsync(request.Prompt, word.TargetWord, word.TabuWords, request.Persona, null)
            : await _aiService.GuessWordAsync(request.Prompt, word.TargetWord, word.TabuWords, request.Persona);
        
        // Calculate attempt number (1-based)
        int currentAttemptNumber = attemptCount + 1;

        // Calculate score
        int score = CalculateScore(aiResult.IsCorrect, promptAnalysis.PromptQuality, currentAttemptNumber);

        // Generate suggestions
        var suggestions = await _aiService.GenerateImprovementSuggestionsAsync(
            request.Prompt, 
            word.TargetWord, 
            word.TabuWords, 
            aiResult.IsCorrect
        );

        // Create new attempt
        var attempt = new GameAttempt
        {
            Id = Guid.NewGuid(),
            GameSessionId = gameSession.Id,
            AttemptNumber = currentAttemptNumber,
            UserPrompt = request.Prompt,
            AiGuess = aiResult.GuessedWord,
            IsCorrect = aiResult.IsCorrect,
            Score = score,
            AiFeedback = promptAnalysis.Feedback,
            PromptQuality = (PromptQuality)promptAnalysis.PromptQuality,
            Suggestions = suggestions,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.GameAttempts.AddAsync(attempt);

        // Update game session summary fields (for legacy support and quick access)
        gameSession.UserPrompt = request.Prompt;
        gameSession.AiResponse = aiResult.GuessedWord;
        gameSession.IsCorrectGuess = aiResult.IsCorrect;
        gameSession.Score += score;
        gameSession.AttemptNumber = currentAttemptNumber;
        
        int coinsEarned = 0;
        int newStreak = 0;
        int totalCoins = 0;

        if (aiResult.IsCorrect)
        {
            gameSession.CompletedAt = DateTime.UtcNow;
            gameSession.Status = GameStatus.Completed;
            gameSession.TimeSpent = gameSession.CompletedAt.Value - gameSession.StartedAt;

            var user = await _unitOfWork.Users.GetByIdAsync(gameSession.UserId);
            if (user != null)
            {
                user.TotalScore += score;
                user.GamesPlayed++;
                user.GamesWon++;

                await UpdateStreakAsync(user);
                newStreak = user.CurrentStreak;

                double streakMultiplier = GetStreakMultiplier(user.CurrentStreak);

                int baseCoins = 10 + (score / 20);
                double coinMultiplier = streakMultiplier;
                if (user.DoubleCoinGamesLeft > 0)
                {
                    coinMultiplier *= 2;
                    user.DoubleCoinGamesLeft--;
                }
                coinsEarned = (int)(baseCoins * coinMultiplier);
                user.PromptCoins += coinsEarned;

                string boostInfo = coinMultiplier > streakMultiplier ? ", 2x boost aktif" : "";
                await _unitOfWork.CoinTransactions.AddAsync(new CoinTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Amount = coinsEarned,
                    Type = CoinTransactionType.GameWin,
                    Description = $"Oyun kazanıldı! (+{coinsEarned} coin, {streakMultiplier}x streak{boostInfo})",
                    CreatedAt = DateTime.UtcNow
                });

                await CheckStreakMilestonesAsync(user);

                totalCoins = user.PromptCoins;
                await _unitOfWork.Users.UpdateAsync(user);
                await UpdateDetailedStatsAsync(user);
            }
        }
        else if (currentAttemptNumber >= 3)
        {
            gameSession.CompletedAt = DateTime.UtcNow;
            gameSession.Status = GameStatus.Failed;
            gameSession.TimeSpent = gameSession.CompletedAt.Value - gameSession.StartedAt;

            var user = await _unitOfWork.Users.GetByIdAsync(gameSession.UserId);
            if (user != null)
            {
                user.GamesPlayed++;
                await UpdateStreakAsync(user);
                await _unitOfWork.Users.UpdateAsync(user);
                await UpdateDetailedStatsAsync(user);
            }
        }

        await _unitOfWork.GameSessions.UpdateAsync(gameSession);
        await _unitOfWork.SaveChangesAsync();

        // Check for badges and level up
        if (aiResult.IsCorrect)
        {
            try
            {
                await _badgeService.UpdateUserLevelAsync(gameSession.UserId);
                await _badgeService.CheckAndAwardBadgesAsync(gameSession.UserId);
            }
            catch
            {
                // Badge check should not fail the game result
            }
        }

        // Get all attempts for history
        var allAttempts = (await _unitOfWork.GameAttempts.FindAsync(a => a.GameSessionId == gameSessionId))
            .OrderBy(a => a.AttemptNumber)
            .ToList();

        var gameCompleted = aiResult.IsCorrect || currentAttemptNumber >= 3;

        // Prompt Koçu analizi (sadece oyun bittiğinde)
        PromptCoachDto? promptCoach = null;
        if (gameCompleted)
        {
            try
            {
                var attemptInfos = allAttempts.Select(a => new Domain.Interfaces.PromptAttemptInfo
                {
                    UserPrompt = a.UserPrompt,
                    AiGuess = a.AiGuess,
                    IsCorrect = a.IsCorrect,
                    PromptQuality = (int)(a.PromptQuality ?? 0)
                }).ToList();

                var coachResult = await _aiService.GeneratePromptCoachAnalysisAsync(
                    word.TargetWord, word.TabuWords, attemptInfos);

                promptCoach = new PromptCoachDto
                {
                    OverallAnalysis = coachResult.OverallAnalysis,
                    BestPrompt = coachResult.BestPrompt,
                    IdealPromptExample = coachResult.IdealPromptExample,
                    TipsForNextTime = coachResult.TipsForNextTime,
                    PromptEngineeringScore = coachResult.PromptEngineeringScore
                };
            }
            catch
            {
                // Coach failure should not break the game result
            }
        }

        return new GameResultDto
        {
            IsCorrect = aiResult.IsCorrect,
            AiGuess = aiResult.GuessedWord,
            AiFeedback = promptAnalysis.Feedback,
            Score = score,
            PromptQuality = promptAnalysis.PromptQuality,
            Suggestions = suggestions,
            GameCompleted = gameCompleted,
            History = _mapper.Map<List<GameAttemptDto>>(allAttempts),
            AiReaction = aiResult.Reaction,
            PromptCoach = promptCoach,
            CoinsEarned = coinsEarned,
            NewStreak = newStreak,
            TotalCoins = totalCoins
        };
    }

    private async Task UpdateDetailedStatsAsync(User user)
    {
        // 1. Update Success Rate
        var successRateStat = (await _unitOfWork.UserStatistics
            .FindAsync(s => s.UserId == user.Id && s.Type == StatisticType.SuccessRate))
            .FirstOrDefault();

        if (successRateStat == null)
        {
            await _unitOfWork.UserStatistics.AddAsync(new UserStatistic
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                MetricName = "Başarı Oranı",
                Value = user.WinRate,
                Type = StatisticType.SuccessRate,
                RecordedAt = DateTime.UtcNow
            });
        }
        else
        {
            successRateStat.Value = user.WinRate;
            successRateStat.RecordedAt = DateTime.UtcNow;
            await _unitOfWork.UserStatistics.UpdateAsync(successRateStat);
        }

        // 2. Update Total Games
        var gamesPlayedStat = (await _unitOfWork.UserStatistics
            .FindAsync(s => s.UserId == user.Id && s.Type == StatisticType.TotalScore)) 
            .FirstOrDefault(s => s.MetricName == "Oyun Sayısı");

        if (gamesPlayedStat == null)
        {
            await _unitOfWork.UserStatistics.AddAsync(new UserStatistic
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                MetricName = "Oyun Sayısı",
                Value = user.GamesPlayed,
                Type = StatisticType.TotalScore,
                RecordedAt = DateTime.UtcNow
            });
        }
        else
        {
            gamesPlayedStat.Value = user.GamesPlayed;
            gamesPlayedStat.RecordedAt = DateTime.UtcNow;
            await _unitOfWork.UserStatistics.UpdateAsync(gamesPlayedStat);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task UpdateStreakAsync(User user)
    {
        var now = DateTime.UtcNow;
        
        if (user.LastPlayedAt.HasValue && user.LastPlayedAt.Value.Date == now.Date)
            return;

        int daysSinceLastPlay = user.LastPlayedAt.HasValue
            ? (now.Date - user.LastPlayedAt.Value.Date).Days
            : int.MaxValue;

        if (daysSinceLastPlay == 1)
        {
            user.CurrentStreak++;
        }
        else if (daysSinceLastPlay > 1)
        {
            if (daysSinceLastPlay == 2 && user.HasStreakShield)
            {
                user.CurrentStreak++;
                user.HasStreakShield = false;
                await _unitOfWork.CoinTransactions.AddAsync(new CoinTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Amount = 0,
                    Type = CoinTransactionType.StreakShieldUsed,
                    Description = $"Seri Kalkanı kullanıldı! Serin korundu ({user.CurrentStreak} gün)",
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                user.CurrentStreak = 1;
                user.HasStreakShield = false;
            }
        }

        if (user.CurrentStreak > user.BestStreak)
            user.BestStreak = user.CurrentStreak;
        user.LastPlayedAt = now;
    }

    private async Task CheckStreakMilestonesAsync(User user)
    {
        if (user.CurrentStreak <= 0) return;
        
        int[] milestones = { 5, 10, 15, 20, 25, 30, 50, 100 };
        if (!milestones.Contains(user.CurrentStreak)) return;

        int streakBonus = user.CurrentStreak * 10;
        user.PromptCoins += streakBonus;
        await _unitOfWork.CoinTransactions.AddAsync(new CoinTransaction
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Amount = streakBonus,
            Type = CoinTransactionType.StreakBonus,
            Description = $"{user.CurrentStreak} günlük seri bonusu!",
            CreatedAt = DateTime.UtcNow
        });

        string? rewardDescription = user.CurrentStreak switch
        {
            5 => "Ateş Kartı tasarımı açıldı!",
            10 => "Yıldız Avatarı açıldı!",
            15 => "Neon Kartı tasarımı açıldı!",
            30 => "Efsane Avatarı açıldı!",
            50 => "Altın Kart tasarımı açıldı!",
            100 => "Elmas Avatarı açıldı!",
            _ => null
        };

        if (rewardDescription != null)
        {
            await _unitOfWork.CoinTransactions.AddAsync(new CoinTransaction
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Amount = 0,
                Type = CoinTransactionType.StreakMilestoneReward,
                Description = $"{user.CurrentStreak} gün seri ödülü: {rewardDescription}",
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private static double GetStreakMultiplier(int streak) => streak switch
    {
        >= 30 => 3.0,
        >= 14 => 2.5,
        >= 7 => 2.0,
        >= 5 => 1.5,
        >= 3 => 1.25,
        _ => 1.0
    };

    private static int CalculateScore(bool isCorrect, int promptQuality, int attemptNumber)
    {
        if (!isCorrect) return 0;

        int baseScore = 100;
        int qualityBonus = promptQuality * 20;
        int attemptPenalty = (attemptNumber - 1) * 20;

        return Math.Max(10, baseScore + qualityBonus - attemptPenalty);
    }
}