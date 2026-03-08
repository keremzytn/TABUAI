using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TabuAI.API.Hubs;
using TabuAI.Application.Common.DTOs;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class VersusController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<GameHub> _hubContext;
    private readonly ILogger<VersusController> _logger;

    public VersusController(IUnitOfWork unitOfWork, IHubContext<GameHub> hubContext, ILogger<VersusController> logger)
    {
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Create a private room for a versus game
    /// </summary>
    [HttpPost("create-room")]
    [ProducesResponseType(typeof(CreateRoomResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CreateRoomResponse>> CreateRoom([FromBody] CreateRoomRequest request)
    {
        try
        {
            if (!Guid.TryParse(request.UserId, out var userId))
                return BadRequest(new { message = "Geçersiz kullanıcı ID" });

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return NotFound(new { message = "Kullanıcı bulunamadı" });

            // Pick a random word
            var wordsQuery = await _unitOfWork.Words.FindAsync(w => w.IsActive);
            var words = wordsQuery.ToList();

            if (!string.IsNullOrEmpty(request.Category))
                words = words.Where(w => w.Category.Equals(request.Category, StringComparison.OrdinalIgnoreCase)).ToList();

            if (request.Difficulty.HasValue)
                words = words.Where(w => (int)w.Difficulty == request.Difficulty.Value).ToList();

            if (!words.Any())
                return BadRequest(new { message = "Uygun kelime bulunamadı" });

            var random = new Random();
            var selectedWord = words[random.Next(words.Count)];

            var roomCode = GenerateRoomCode();

            var versusGame = new VersusGame
            {
                Id = Guid.NewGuid(),
                WordId = selectedWord.Id,
                Player1Id = userId,
                Status = VersusGameStatus.WaitingForOpponent,
                RoomCode = roomCode,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.VersusGames.AddAsync(versusGame);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new CreateRoomResponse
            {
                VersusGameId = versusGame.Id,
                RoomCode = roomCode
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating versus room");
            return StatusCode(500, new { message = "Oda oluşturulamadı" });
        }
    }

    /// <summary>
    /// Get a versus game by room code
    /// </summary>
    [HttpGet("room/{roomCode}")]
    [ProducesResponseType(typeof(VersusGameDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<VersusGameDto>> GetRoom(string roomCode)
    {
        try
        {
            var games = await _unitOfWork.VersusGames.FindAsync(v => v.RoomCode == roomCode);
            var game = games.FirstOrDefault();

            if (game == null) return NotFound(new { message = "Oda bulunamadı" });

            var player1 = await _unitOfWork.Users.GetByIdAsync(game.Player1Id);
            var player2 = game.Player2Id.HasValue ? await _unitOfWork.Users.GetByIdAsync(game.Player2Id.Value) : null;

            return Ok(new VersusGameDto
            {
                Id = game.Id,
                RoomCode = game.RoomCode,
                Status = game.Status.ToString(),
                WinnerId = game.WinnerId,
                CreatedAt = game.CreatedAt,
                StartedAt = game.StartedAt,
                CompletedAt = game.CompletedAt,
                Player1 = new VersusPlayerDto
                {
                    Id = game.Player1Id,
                    DisplayName = player1?.DisplayName ?? player1?.Username ?? "Oyuncu 1",
                    Score = game.Player1Score,
                    Attempts = game.Player1Attempts,
                    GameSessionId = game.Player1GameSessionId
                },
                Player2 = player2 != null ? new VersusPlayerDto
                {
                    Id = game.Player2Id!.Value,
                    DisplayName = player2.DisplayName ?? player2.Username,
                    Score = game.Player2Score,
                    Attempts = game.Player2Attempts,
                    GameSessionId = game.Player2GameSessionId
                } : null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting room {RoomCode}", roomCode);
            return StatusCode(500, new { message = "Oda bilgileri alınamadı" });
        }
    }

    /// <summary>
    /// Get user's versus game history
    /// </summary>
    [HttpGet("history/{userId}")]
    [ProducesResponseType(typeof(List<VersusGameDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VersusGameDto>>> GetHistory(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var games = await _unitOfWork.VersusGames.FindAsync(
                v => v.Player1Id == userId || v.Player2Id == userId);

            var sortedGames = games
                .Where(v => v.Status == VersusGameStatus.Completed)
                .OrderByDescending(v => v.CompletedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new List<VersusGameDto>();
            foreach (var game in sortedGames)
            {
                var player1 = await _unitOfWork.Users.GetByIdAsync(game.Player1Id);
                var player2 = game.Player2Id.HasValue
                    ? await _unitOfWork.Users.GetByIdAsync(game.Player2Id.Value)
                    : null;

                result.Add(new VersusGameDto
                {
                    Id = game.Id,
                    RoomCode = game.RoomCode,
                    Status = game.Status.ToString(),
                    WinnerId = game.WinnerId,
                    CreatedAt = game.CreatedAt,
                    StartedAt = game.StartedAt,
                    CompletedAt = game.CompletedAt,
                    Player1 = new VersusPlayerDto
                    {
                        Id = game.Player1Id,
                        DisplayName = player1?.DisplayName ?? player1?.Username ?? "Oyuncu 1",
                        Score = game.Player1Score,
                        Attempts = game.Player1Attempts
                    },
                    Player2 = player2 != null ? new VersusPlayerDto
                    {
                        Id = game.Player2Id!.Value,
                        DisplayName = player2.DisplayName ?? player2.Username,
                        Score = game.Player2Score,
                        Attempts = game.Player2Attempts
                    } : null
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting versus history");
            return StatusCode(500, new { message = "Geçmiş alınamadı" });
        }
    }

    /// <summary>
    /// Send a challenge to a friend
    /// </summary>
    [HttpPost("challenge")]
    [ProducesResponseType(typeof(ChallengeDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ChallengeDto>> SendChallenge([FromBody] SendChallengeRequest request)
    {
        try
        {
            if (!Guid.TryParse(request.ChallengerId, out var challengerId))
                return BadRequest(new { message = "Geçersiz challenger ID" });
            if (!Guid.TryParse(request.ChallengedId, out var challengedId))
                return BadRequest(new { message = "Geçersiz challenged ID" });

            var challenger = await _unitOfWork.Users.GetByIdAsync(challengerId);
            var challenged = await _unitOfWork.Users.GetByIdAsync(challengedId);

            if (challenger == null || challenged == null)
                return NotFound(new { message = "Kullanıcı bulunamadı" });

            // Pick word
            Word? selectedWord;
            if (!string.IsNullOrEmpty(request.WordId) && Guid.TryParse(request.WordId, out var wordId))
            {
                selectedWord = await _unitOfWork.Words.GetByIdAsync(wordId);
            }
            else
            {
                var wordsQuery = await _unitOfWork.Words.FindAsync(w => w.IsActive);
                var words = wordsQuery.ToList();

                if (!string.IsNullOrEmpty(request.Category))
                    words = words.Where(w => w.Category.Equals(request.Category, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!words.Any())
                    return BadRequest(new { message = "Uygun kelime bulunamadı" });

                selectedWord = words[new Random().Next(words.Count)];
            }

            if (selectedWord == null)
                return BadRequest(new { message = "Kelime bulunamadı" });

            var challenge = new Challenge
            {
                Id = Guid.NewGuid(),
                ChallengerId = challengerId,
                ChallengedId = challengedId,
                WordId = selectedWord.Id,
                Message = request.Message,
                Status = ChallengeStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            await _unitOfWork.Challenges.AddAsync(challenge);

            // Log activity
            await _unitOfWork.ActivityLogs.AddAsync(new ActivityLog
            {
                Id = Guid.NewGuid(),
                UserId = challengerId,
                Type = ActivityType.ChallengeSent,
                Description = $"{challenger.DisplayName ?? challenger.Username}, {challenged.DisplayName ?? challenged.Username} kullanıcısına meydan okudu!",
                RelatedEntityId = challenge.Id,
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.ActivityLogs.AddAsync(new ActivityLog
            {
                Id = Guid.NewGuid(),
                UserId = challengedId,
                Type = ActivityType.ChallengeReceived,
                Description = $"{challenger.DisplayName ?? challenger.Username} sana meydan okudu!",
                RelatedEntityId = challenge.Id,
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveChangesAsync();

            return Ok(new ChallengeDto
            {
                Id = challenge.Id,
                Challenger = new UserDto { Id = challengerId, Username = challenger.Username, DisplayName = challenger.DisplayName },
                Challenged = new UserDto { Id = challengedId, Username = challenged.Username, DisplayName = challenged.DisplayName },
                Word = new WordDto
                {
                    Id = selectedWord.Id,
                    TargetWord = selectedWord.TargetWord,
                    TabuWords = selectedWord.TabuWords,
                    Category = selectedWord.Category,
                    Difficulty = (int)selectedWord.Difficulty
                },
                Status = challenge.Status.ToString(),
                Message = challenge.Message,
                CreatedAt = challenge.CreatedAt,
                ExpiresAt = challenge.ExpiresAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending challenge");
            return StatusCode(500, new { message = "Meydan okuma gönderilemedi" });
        }
    }

    /// <summary>
    /// Get pending challenges for a user
    /// </summary>
    [HttpGet("challenges/{userId}/pending")]
    [ProducesResponseType(typeof(List<ChallengeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ChallengeDto>>> GetPendingChallenges(Guid userId)
    {
        try
        {
            var challenges = await _unitOfWork.Challenges.FindAsync(
                c => c.ChallengedId == userId && c.Status == ChallengeStatus.Pending && c.ExpiresAt > DateTime.UtcNow);

            var result = new List<ChallengeDto>();
            foreach (var c in challenges.OrderByDescending(c => c.CreatedAt))
            {
                var challenger = await _unitOfWork.Users.GetByIdAsync(c.ChallengerId);
                var word = await _unitOfWork.Words.GetByIdAsync(c.WordId);

                result.Add(new ChallengeDto
                {
                    Id = c.Id,
                    Challenger = new UserDto
                    {
                        Id = c.ChallengerId,
                        Username = challenger?.Username ?? "",
                        DisplayName = challenger?.DisplayName
                    },
                    Challenged = new UserDto { Id = userId },
                    Word = word != null ? new WordDto
                    {
                        Id = word.Id,
                        TargetWord = "???",  // Don't reveal word until accepted
                        TabuWords = new List<string>(),
                        Category = word.Category,
                        Difficulty = (int)word.Difficulty
                    } : new WordDto(),
                    Status = c.Status.ToString(),
                    Message = c.Message,
                    CreatedAt = c.CreatedAt,
                    ExpiresAt = c.ExpiresAt
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending challenges");
            return StatusCode(500, new { message = "Meydan okumalar alınamadı" });
        }
    }

    /// <summary>
    /// Respond to a challenge (accept/reject)
    /// </summary>
    [HttpPut("challenges/{challengeId}/respond")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> RespondToChallenge(Guid challengeId, [FromBody] RespondChallengeRequest request)
    {
        try
        {
            var challenge = await _unitOfWork.Challenges.GetByIdAsync(challengeId);
            if (challenge == null) return NotFound(new { message = "Meydan okuma bulunamadı" });

            if (!Guid.TryParse(request.UserId, out var userId) || challenge.ChallengedId != userId)
                return BadRequest(new { message = "Bu meydan okumayı sadece hedef kullanıcı yanıtlayabilir" });

            if (challenge.Status != ChallengeStatus.Pending)
                return BadRequest(new { message = "Bu meydan okuma zaten yanıtlanmış" });

            if (challenge.ExpiresAt <= DateTime.UtcNow)
            {
                challenge.Status = ChallengeStatus.Expired;
                await _unitOfWork.Challenges.UpdateAsync(challenge);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest(new { message = "Meydan okuma süresi dolmuş" });
            }

            if (request.Accept)
            {
                challenge.Status = ChallengeStatus.Accepted;
                challenge.RespondedAt = DateTime.UtcNow;

                // Create a versus game room
                var roomCode = GenerateRoomCode();
                var versusGame = new VersusGame
                {
                    Id = Guid.NewGuid(),
                    WordId = challenge.WordId,
                    Player1Id = challenge.ChallengerId,
                    Player2Id = challenge.ChallengedId,
                    Status = VersusGameStatus.WaitingForOpponent, // Will become InProgress when both connect via SignalR
                    RoomCode = roomCode,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.VersusGames.AddAsync(versusGame);
                challenge.VersusGameId = versusGame.Id;

                await _unitOfWork.Challenges.UpdateAsync(challenge);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new { message = "Meydan okuma kabul edildi!", roomCode, versusGameId = versusGame.Id });
            }
            else
            {
                challenge.Status = ChallengeStatus.Rejected;
                challenge.RespondedAt = DateTime.UtcNow;
                await _unitOfWork.Challenges.UpdateAsync(challenge);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new { message = "Meydan okuma reddedildi." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error responding to challenge");
            return StatusCode(500, new { message = "Yanıt gönderilemedi" });
        }
    }

    /// <summary>
    /// Get sent challenges for a user
    /// </summary>
    [HttpGet("challenges/{userId}/sent")]
    [ProducesResponseType(typeof(List<ChallengeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ChallengeDto>>> GetSentChallenges(Guid userId)
    {
        try
        {
            var challenges = await _unitOfWork.Challenges.FindAsync(c => c.ChallengerId == userId);
            var result = new List<ChallengeDto>();

            foreach (var c in challenges.OrderByDescending(c => c.CreatedAt).Take(20))
            {
                var challenged = await _unitOfWork.Users.GetByIdAsync(c.ChallengedId);
                var word = await _unitOfWork.Words.GetByIdAsync(c.WordId);

                result.Add(new ChallengeDto
                {
                    Id = c.Id,
                    Challenger = new UserDto { Id = userId },
                    Challenged = new UserDto
                    {
                        Id = c.ChallengedId,
                        Username = challenged?.Username ?? "",
                        DisplayName = challenged?.DisplayName
                    },
                    Word = word != null ? new WordDto
                    {
                        Id = word.Id,
                        TargetWord = word.TargetWord,
                        TabuWords = word.TabuWords,
                        Category = word.Category,
                        Difficulty = (int)word.Difficulty
                    } : new WordDto(),
                    Status = c.Status.ToString(),
                    Message = c.Message,
                    VersusGameId = c.VersusGameId,
                    CreatedAt = c.CreatedAt,
                    ExpiresAt = c.ExpiresAt
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sent challenges");
            return StatusCode(500, new { message = "Gönderilen meydan okumalar alınamadı" });
        }
    }

    private static string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Range(0, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }
}
