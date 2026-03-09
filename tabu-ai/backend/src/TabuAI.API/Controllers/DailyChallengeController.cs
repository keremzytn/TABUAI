using Microsoft.AspNetCore.Mvc;
using TabuAI.Application.Common.DTOs;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.API.Controllers;

[ApiController]
[Route("api/daily-challenge")]
[Produces("application/json")]
public class DailyChallengeController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public DailyChallengeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("today")]
    public async Task<ActionResult<DailyChallengeDto>> GetTodaysChallenge([FromQuery] string language = "tr", [FromQuery] string? userId = null)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var challenge = (await _unitOfWork.DailyChallenges.FindAsync(
                d => d.ChallengeDate == today && d.Language == language && d.IsActive))
                .FirstOrDefault();

            if (challenge == null)
            {
                challenge = await CreateDailyChallenge(today, language);
                if (challenge == null)
                    return NotFound(new { message = "Günün kelimesi oluşturulamadı. Yeterli kelime yok." });
            }

            var word = await _unitOfWork.Words.GetByIdAsync(challenge.WordId);
            if (word == null) return NotFound(new { message = "Kelime bulunamadı" });

            var totalPlayers = await _unitOfWork.DailyChallengeEntries.CountAsync(e => e.DailyChallengeId == challenge.Id);

            bool alreadyPlayed = false;
            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var parsedUserId))
            {
                alreadyPlayed = await _unitOfWork.DailyChallengeEntries.ExistsAsync(
                    e => e.DailyChallengeId == challenge.Id && e.UserId == parsedUserId);
            }

            return Ok(new DailyChallengeDto
            {
                Id = challenge.Id,
                ChallengeDate = challenge.ChallengeDate,
                Language = challenge.Language,
                Word = new WordDto
                {
                    Id = word.Id,
                    TargetWord = word.TargetWord,
                    TabuWords = word.TabuWords,
                    Category = word.Category,
                    Difficulty = (int)word.Difficulty,
                    Language = word.Language
                },
                AlreadyPlayed = alreadyPlayed,
                TotalPlayers = totalPlayers
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Günün meydan okuması yüklenemedi", error = ex.Message });
        }
    }

    [HttpPost("complete")]
    public async Task<ActionResult<DailyChallengeResultDto>> CompleteChallenge([FromBody] CompleteDailyChallengeRequest request)
    {
        try
        {
            if (!Guid.TryParse(request.UserId, out var parsedUserId))
                return BadRequest(new { message = "Geçersiz kullanıcı ID" });

            if (!Guid.TryParse(request.DailyChallengeId, out var parsedChallengeId))
                return BadRequest(new { message = "Geçersiz challenge ID" });

            if (!Guid.TryParse(request.GameSessionId, out var parsedGameSessionId))
                return BadRequest(new { message = "Geçersiz game session ID" });

            var alreadyPlayed = await _unitOfWork.DailyChallengeEntries.ExistsAsync(
                e => e.DailyChallengeId == parsedChallengeId && e.UserId == parsedUserId);
            if (alreadyPlayed)
                return BadRequest(new { message = "Bu günün meydan okumasını zaten tamamladınız" });

            var gameSession = await _unitOfWork.GameSessions.GetByIdAsync(parsedGameSessionId);
            if (gameSession == null)
                return NotFound(new { message = "Oyun oturumu bulunamadı" });

            var entry = new DailyChallengeEntry
            {
                Id = Guid.NewGuid(),
                DailyChallengeId = parsedChallengeId,
                UserId = parsedUserId,
                GameSessionId = parsedGameSessionId,
                Score = gameSession.Score,
                AttemptsUsed = gameSession.AttemptNumber,
                TimeTaken = gameSession.TimeSpent,
                IsCompleted = gameSession.Status == GameStatus.Completed,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.DailyChallengeEntries.AddAsync(entry);

            // Daily Challenge bonus coin
            if (entry.IsCompleted)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(parsedUserId);
                if (user != null)
                {
                    int bonus = 25;
                    user.PromptCoins += bonus;
                    await _unitOfWork.Users.UpdateAsync(user);
                    await _unitOfWork.CoinTransactions.AddAsync(new CoinTransaction
                    {
                        Id = Guid.NewGuid(),
                        UserId = parsedUserId,
                        Amount = bonus,
                        Type = CoinTransactionType.DailyChallengeBonus,
                        Description = $"Günün meydan okuması tamamlandı! (+{bonus} coin)",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var allEntries = (await _unitOfWork.DailyChallengeEntries.FindAsync(
                e => e.DailyChallengeId == parsedChallengeId))
                .OrderByDescending(e => e.Score)
                .ThenBy(e => e.AttemptsUsed)
                .ThenBy(e => e.TimeTaken)
                .ToList();

            var rank = allEntries.FindIndex(e => e.UserId == parsedUserId) + 1;

            var topPlayers = new List<DailyChallengeLeaderboardDto>();
            int rankCounter = 1;
            foreach (var e in allEntries.Take(10))
            {
                var user = await _unitOfWork.Users.GetByIdAsync(e.UserId);
                topPlayers.Add(new DailyChallengeLeaderboardDto
                {
                    UserId = e.UserId,
                    DisplayName = user?.DisplayName ?? user?.Username ?? "Anonim",
                    Score = e.Score,
                    AttemptsUsed = e.AttemptsUsed,
                    TimeTaken = e.TimeTaken,
                    Rank = rankCounter++
                });
            }

            return Ok(new DailyChallengeResultDto
            {
                Score = entry.Score,
                Rank = rank,
                TotalPlayers = allEntries.Count,
                TopPlayers = topPlayers
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Challenge tamamlanamadı", error = ex.Message });
        }
    }

    [HttpGet("leaderboard")]
    public async Task<ActionResult<List<DailyChallengeLeaderboardDto>>> GetLeaderboard([FromQuery] string? dailyChallengeId = null, [FromQuery] string language = "tr")
    {
        try
        {
            Guid challengeId;
            if (!string.IsNullOrEmpty(dailyChallengeId) && Guid.TryParse(dailyChallengeId, out var parsed))
            {
                challengeId = parsed;
            }
            else
            {
                var today = DateTime.UtcNow.Date;
                var challenge = (await _unitOfWork.DailyChallenges.FindAsync(
                    d => d.ChallengeDate == today && d.Language == language && d.IsActive))
                    .FirstOrDefault();
                if (challenge == null) return Ok(new List<DailyChallengeLeaderboardDto>());
                challengeId = challenge.Id;
            }

            var entries = (await _unitOfWork.DailyChallengeEntries.FindAsync(e => e.DailyChallengeId == challengeId))
                .OrderByDescending(e => e.Score)
                .ThenBy(e => e.AttemptsUsed)
                .ThenBy(e => e.TimeTaken)
                .Take(50)
                .ToList();

            var result = new List<DailyChallengeLeaderboardDto>();
            int rank = 1;
            foreach (var entry in entries)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(entry.UserId);
                result.Add(new DailyChallengeLeaderboardDto
                {
                    UserId = entry.UserId,
                    DisplayName = user?.DisplayName ?? user?.Username ?? "Anonim",
                    Score = entry.Score,
                    AttemptsUsed = entry.AttemptsUsed,
                    TimeTaken = entry.TimeTaken,
                    Rank = rank++
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Liderlik tablosu yüklenemedi", error = ex.Message });
        }
    }

    private async Task<DailyChallenge?> CreateDailyChallenge(DateTime date, string language)
    {
        var words = (await _unitOfWork.Words.FindAsync(w => w.IsActive && w.Language == language && w.WordPackId == null)).ToList();
        if (!words.Any()) return null;

        var random = new Random();
        var selectedWord = words[random.Next(words.Count)];

        var challenge = new DailyChallenge
        {
            Id = Guid.NewGuid(),
            ChallengeDate = date,
            WordId = selectedWord.Id,
            Language = language,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.DailyChallenges.AddAsync(challenge);
        await _unitOfWork.SaveChangesAsync();
        return challenge;
    }
}
