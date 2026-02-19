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

    public SubmitPromptCommandHandler(IUnitOfWork unitOfWork, IAiService aiService, IBadgeService badgeService)
    {
        _unitOfWork = unitOfWork;
        _aiService = aiService;
        _badgeService = badgeService;
    }

    public async Task<GameResultDto> Handle(SubmitPromptCommand request, CancellationToken cancellationToken)
    {
        // Parse GameSessionId
        if (!Guid.TryParse(request.GameSessionId, out var gameSessionId))
        {
            throw new ArgumentException("Invalid game session ID format");
        }

        // Get game session with word
        var gameSession = await _unitOfWork.GameSessions.GetByIdAsync(gameSessionId);
        if (gameSession == null)
        {
            throw new InvalidOperationException("Game session not found");
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
            // Return failure result
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
        
        // Calculate score
        int score = CalculateScore(aiResult.IsCorrect, promptAnalysis.PromptQuality, gameSession.AttemptNumber);

        // Update game session
        gameSession.UserPrompt = request.Prompt;
        gameSession.AiResponse = aiResult.GuessedWord;
        gameSession.IsCorrectGuess = aiResult.IsCorrect;
        gameSession.Score += score;
        gameSession.AiFeedback = promptAnalysis.Feedback;
        gameSession.PromptQuality = (PromptQuality)promptAnalysis.PromptQuality;
        
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
        else
        {
            gameSession.AttemptNumber++;

            // If max attempts reached (3), mark as failed
            // Current attempts are counted from 1. After 3 increment, it's finished.
            if (gameSession.AttemptNumber >= 3)
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
        }

        // Generate suggestions
        var suggestions = await _aiService.GenerateImprovementSuggestionsAsync(
            request.Prompt, 
            word.TargetWord, 
            word.TabuWords, 
            aiResult.IsCorrect
        );

        gameSession.Suggestions = suggestions;

        await _unitOfWork.GameSessions.UpdateAsync(gameSession);
        await _unitOfWork.SaveChangesAsync();

        // Check for badges and level up (async, non-blocking)
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

        return new GameResultDto
        {
            IsCorrect = aiResult.IsCorrect,
            AiGuess = aiResult.GuessedWord,
            AiFeedback = promptAnalysis.Feedback,
            Score = score,
            PromptQuality = promptAnalysis.PromptQuality,
            Suggestions = suggestions,
            GameCompleted = aiResult.IsCorrect || gameSession.AttemptNumber > 3
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
            .FindAsync(s => s.UserId == user.Id && s.Type == StatisticType.TotalScore)) // Reusing type mapping for display
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
        int attemptPenalty = (attemptNumber - 1) * 10; // -10 for each additional attempt

        return Math.Max(10, baseScore + qualityBonus - attemptPenalty);
    }
}