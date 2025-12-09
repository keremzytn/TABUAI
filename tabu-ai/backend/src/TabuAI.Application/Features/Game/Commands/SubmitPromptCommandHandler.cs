using MediatR;
using TabuAI.Application.Common.DTOs;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Game.Commands;

public class SubmitPromptCommandHandler : IRequestHandler<SubmitPromptCommand, GameResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiService _aiService;

    public SubmitPromptCommandHandler(IUnitOfWork unitOfWork, IAiService aiService)
    {
        _unitOfWork = unitOfWork;
        _aiService = aiService;
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
        }
        else
        {
            gameSession.AttemptNumber++;
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

        return new GameResultDto
        {
            IsCorrect = aiResult.IsCorrect,
            AiGuess = aiResult.GuessedWord,
            AiFeedback = promptAnalysis.Feedback,
            Score = score,
            PromptQuality = promptAnalysis.PromptQuality,
            Suggestions = suggestions,
            GameCompleted = aiResult.IsCorrect
        };
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