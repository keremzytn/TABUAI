using AutoMapper;
using MediatR;
using TabuAI.Application.Common.DTOs;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Game.Commands;

public class StartGameCommandHandler : IRequestHandler<StartGameCommand, GameSessionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StartGameCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GameSessionDto> Handle(StartGameCommand request, CancellationToken cancellationToken)
    {
        // Get a random word based on criteria
        var wordsQuery = await _unitOfWork.Words.FindAsync(w => w.IsActive);
        var words = wordsQuery.ToList();

        if (!string.IsNullOrEmpty(request.Category))
        {
            words = words.Where(w => w.Category.Equals(request.Category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (request.Difficulty.HasValue)
        {
            words = words.Where(w => (int)w.Difficulty == request.Difficulty.Value).ToList();
        }

        if (!words.Any())
        {
            throw new InvalidOperationException("No words found matching the criteria");
        }

        var random = new Random();
        var selectedWord = words[random.Next(words.Count)];

        // Create game session
        var gameSession = new GameSession
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            WordId = selectedWord.Id,
            Mode = Enum.Parse<GameMode>(request.GameMode),
            StartedAt = DateTime.UtcNow,
            Status = GameStatus.InProgress
        };

        await _unitOfWork.GameSessions.AddAsync(gameSession);
        await _unitOfWork.SaveChangesAsync();

        // Load the word relationship
        gameSession.Word = selectedWord;

        return _mapper.Map<GameSessionDto>(gameSession);
    }
}