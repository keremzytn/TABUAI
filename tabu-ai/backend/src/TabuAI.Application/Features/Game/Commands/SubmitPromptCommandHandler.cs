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

        // Get AI guess
        var aiResult = await _aiService.GuessWordAsync(request.Prompt, word.TargetWord, word.TabuWords);
        
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
        
        if (aiResult.IsCorrect)
        {
            gameSession.CompletedAt = DateTime.UtcNow;
            gameSession.Status = GameStatus.Completed;
            gameSession.TimeSpent = gameSession.CompletedAt.Value - gameSession.StartedAt;

            // Update user stats
            var user = await _unitOfWork.Users.GetByIdAsync(gameSession.UserId);
            if (user != null)
            {
                user.TotalScore += score;
                user.GamesPlayed++;
                user.GamesWon++;
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

        return new GameResultDto
        {
            IsCorrect = aiResult.IsCorrect,
            AiGuess = aiResult.GuessedWord,
            AiFeedback = promptAnalysis.Feedback,
            Score = score,
            PromptQuality = promptAnalysis.PromptQuality,
            Suggestions = suggestions,
            GameCompleted = aiResult.IsCorrect || currentAttemptNumber >= 3,
            History = _mapper.Map<List<GameAttemptDto>>(allAttempts)
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

    private static int CalculateScore(bool isCorrect, int promptQuality, int attemptNumber)
    {
        if (!isCorrect) return 0;

        int baseScore = 100;
        int qualityBonus = promptQuality * 20; // 20-100 bonus
        int attemptPenalty = (attemptNumber - 1) * 20; // -20 for each additional attempt

        return Math.Max(10, baseScore + qualityBonus - attemptPenalty);
    }
}