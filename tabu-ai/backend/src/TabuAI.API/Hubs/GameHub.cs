using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;
using System.Collections.Concurrent;

namespace TabuAI.API.Hubs;

[Authorize]
public class GameHub : Hub
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiService _aiService;
    private readonly ILogger<GameHub> _logger;

    // In-memory matchmaking queue
    private static readonly ConcurrentQueue<WaitingPlayer> _matchmakingQueue = new();
    private static readonly ConcurrentDictionary<string, string> _userConnections = new();
    private static readonly ConcurrentDictionary<string, bool> _cancelledMatchmaking = new();

    public GameHub(IUnitOfWork unitOfWork, IAiService aiService, ILogger<GameHub> logger)
    {
        _unitOfWork = unitOfWork;
        _aiService = aiService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("UserId")?.Value;
        if (userId != null)
        {
            _userConnections[userId] = Context.ConnectionId;
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst("UserId")?.Value;
        if (userId != null)
        {
            _userConnections.TryRemove(userId, out _);
            _cancelledMatchmaking.TryRemove(userId, out _);

            // If the disconnected user is in an active versus game, award win to opponent
            await HandlePlayerDisconnect(userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private async Task HandlePlayerDisconnect(string userId)
    {
        try
        {
            var parsedUserId = Guid.Parse(userId);
            var activeGames = await _unitOfWork.VersusGames.FindAsync(
                v => v.Status == VersusGameStatus.InProgress &&
                     (v.Player1Id == parsedUserId || v.Player2Id == parsedUserId));

            var activeGame = activeGames.FirstOrDefault();
            if (activeGame == null) return;

            var groupName = $"versus_{activeGame.Id}";

            // Award win to the opponent
            var winnerId = activeGame.Player1Id == parsedUserId
                ? activeGame.Player2Id
                : (Guid?)activeGame.Player1Id;

            activeGame.Status = VersusGameStatus.Completed;
            activeGame.CompletedAt = DateTime.UtcNow;
            activeGame.WinnerId = winnerId;

            await _unitOfWork.VersusGames.UpdateAsync(activeGame);
            await UpdateVersusStats(activeGame);
            await LogVersusActivity(activeGame);
            await _unitOfWork.SaveChangesAsync();

            await Clients.Group(groupName).SendAsync("OpponentDisconnected", new
            {
                disconnectedPlayerId = userId,
                winnerId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling player disconnect for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Player joins the matchmaking queue for a random opponent
    /// </summary>
    public async Task JoinMatchmaking(string? category, int? difficulty)
    {
        var userId = Context.User?.FindFirst("UserId")?.Value;
        if (userId == null) { await Clients.Caller.SendAsync("AuthError", "Oturum süresi dolmuş. Lütfen tekrar giriş yapın."); return; }

        var user = await _unitOfWork.Users.GetByIdAsync(Guid.Parse(userId));
        if (user == null) { await Clients.Caller.SendAsync("AuthError", "Kullanıcı bulunamadı."); return; }

        _cancelledMatchmaking.TryRemove(userId, out _);

        // Check if there's someone already waiting
        var matchmakingTtl = TimeSpan.FromMinutes(5);
        WaitingPlayer? opponent = null;
        while (_matchmakingQueue.TryDequeue(out var candidate))
        {
            // Skip stale, cancelled, disconnected or same user
            if (candidate.UserId == userId) continue;
            if (!_userConnections.ContainsKey(candidate.UserId)) continue;
            if (_cancelledMatchmaking.ContainsKey(candidate.UserId)) continue;
            if (DateTime.UtcNow - candidate.JoinedAt > matchmakingTtl) continue;
            opponent = candidate;
            break;
        }

        if (opponent != null)
        {
            // Match found! Create a versus game
            await CreateVersusMatch(userId, user, opponent);
        }
        else
        {
            // No match, add to queue
            _matchmakingQueue.Enqueue(new WaitingPlayer
            {
                UserId = userId,
                ConnectionId = Context.ConnectionId,
                DisplayName = user.DisplayName ?? user.Username,
                Category = category,
                Difficulty = difficulty,
                JoinedAt = DateTime.UtcNow
            });

            await Clients.Caller.SendAsync("WaitingForOpponent");
        }
    }

    /// <summary>
    /// Cancel matchmaking
    /// </summary>
    public async Task LeaveMatchmaking()
    {
        var userId = Context.User?.FindFirst("UserId")?.Value;
        if (userId != null)
            _cancelledMatchmaking[userId] = true;
        await Clients.Caller.SendAsync("MatchmakingCancelled");
    }

    /// <summary>
    /// Player1 joins the SignalR group for their own room (without starting the game)
    /// </summary>
    public async Task WaitInRoom(string roomCode)
    {
        var userId = Context.User?.FindFirst("UserId")?.Value;
        if (userId == null) { await Clients.Caller.SendAsync("AuthError", "Oturum süresi dolmuş. Lütfen tekrar giriş yapın."); return; }

        try
        {
            var versusGames = await _unitOfWork.VersusGames.FindAsync(
                v => v.RoomCode == roomCode && v.Status == VersusGameStatus.WaitingForOpponent);
            var versusGame = versusGames.FirstOrDefault();

            if (versusGame == null)
            {
                await Clients.Caller.SendAsync("RoomNotFound", "Oda bulunamadı.");
                return;
            }

            if (versusGame.Player1Id.ToString() != userId)
            {
                await Clients.Caller.SendAsync("RoomNotFound", "Bu odanın sahibi değilsiniz.");
                return;
            }

            var groupName = $"versus_{versusGame.Id}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _userConnections[userId] = Context.ConnectionId;

            _logger.LogInformation("Player {UserId} waiting in room {RoomCode} (group: {GroupName})", userId, roomCode, groupName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in WaitInRoom for room {RoomCode}", roomCode);
            await Clients.Caller.SendAsync("RoomNotFound", $"Odada beklenirken hata: {ex.Message}");
        }
    }

    /// <summary>
    /// Join a specific room by code (for friend challenges)
    /// </summary>
    public async Task JoinRoom(string roomCode)
    {
        var userId = Context.User?.FindFirst("UserId")?.Value;
        if (userId == null) { await Clients.Caller.SendAsync("AuthError", "Oturum süresi dolmuş. Lütfen tekrar giriş yapın."); return; }

        try
        {
        var versusGames = await _unitOfWork.VersusGames.FindAsync(
            v => v.RoomCode == roomCode && v.Status == VersusGameStatus.WaitingForOpponent);
        var versusGame = versusGames.FirstOrDefault();

        if (versusGame == null)
        {
            await Clients.Caller.SendAsync("RoomNotFound", "Oda bulunamadı veya süre doldu.");
            return;
        }

        if (versusGame.Player1Id.ToString() == userId)
        {
            await Clients.Caller.SendAsync("RoomNotFound", "Kendi odanıza katılamazsınız.");
            return;
        }

        var parsedUserId = Guid.Parse(userId);
        var player2 = await _unitOfWork.Users.GetByIdAsync(parsedUserId);
        if (player2 == null) { await Clients.Caller.SendAsync("RoomNotFound", "Kullanıcı bulunamadı."); return; }

        // Join the game
        versusGame.Player2Id = parsedUserId;
        versusGame.Status = VersusGameStatus.InProgress;
        versusGame.StartedAt = DateTime.UtcNow;

        // Create game sessions for both players
        var word = await _unitOfWork.Words.GetByIdAsync(versusGame.WordId);
        if (word == null) { await Clients.Caller.SendAsync("RoomNotFound", "Kelime bulunamadı."); return; }

        var p1Session = new GameSession
        {
            Id = Guid.NewGuid(),
            UserId = versusGame.Player1Id,
            WordId = versusGame.WordId,
            Mode = GameMode.Multiplayer,
            StartedAt = DateTime.UtcNow,
            Status = GameStatus.InProgress
        };

        var p2Session = new GameSession
        {
            Id = Guid.NewGuid(),
            UserId = parsedUserId,
            WordId = versusGame.WordId,
            Mode = GameMode.Multiplayer,
            StartedAt = DateTime.UtcNow,
            Status = GameStatus.InProgress
        };

        await _unitOfWork.GameSessions.AddAsync(p1Session);
        await _unitOfWork.GameSessions.AddAsync(p2Session);

        versusGame.Player1GameSessionId = p1Session.Id;
        versusGame.Player2GameSessionId = p2Session.Id;

        await _unitOfWork.VersusGames.UpdateAsync(versusGame);
        await _unitOfWork.SaveChangesAsync();

        // Add both players to the SignalR group
        var groupName = $"versus_{versusGame.Id}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        string? p1Connection = null;
        _userConnections.TryGetValue(versusGame.Player1Id.ToString(), out p1Connection);
        if (p1Connection != null)
        {
            await Groups.AddToGroupAsync(p1Connection, groupName);
        }

        var player1 = await _unitOfWork.Users.GetByIdAsync(versusGame.Player1Id);

        var gameStartedPayload = new
        {
            versusGameId = versusGame.Id,
            roomCode = versusGame.RoomCode,
            word = new
            {
                id = word.Id,
                targetWord = word.TargetWord,
                tabuWords = word.TabuWords,
                category = word.Category,
                difficulty = (int)word.Difficulty
            },
            player1 = new
            {
                id = versusGame.Player1Id,
                displayName = player1?.DisplayName ?? player1?.Username ?? "Oyuncu 1",
                gameSessionId = p1Session.Id
            },
            player2 = new
            {
                id = parsedUserId,
                displayName = player2.DisplayName ?? player2.Username,
                gameSessionId = p2Session.Id
            }
        };

        // Notify via group
        await Clients.Group(groupName).SendAsync("GameStarted", gameStartedPayload);

        // Also notify Player1 directly in case group membership failed
        if (p1Connection != null)
        {
            await Clients.Client(p1Connection).SendAsync("GameStarted", gameStartedPayload);
        }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in JoinRoom for room {RoomCode}", roomCode);
            await Clients.Caller.SendAsync("RoomNotFound", $"Odaya katılırken hata oluştu: {ex.Message}");
        }
    }

    /// <summary>
    /// Submit a prompt in versus mode
    /// </summary>
    public async Task SubmitVersusPrompt(string versusGameId, string gameSessionId, string prompt)
    {
        var userId = Context.User?.FindFirst("UserId")?.Value;
        if (userId == null) { await Clients.Caller.SendAsync("AuthError", "Oturum süresi dolmuş. Lütfen tekrar giriş yapın."); return; }

        var parsedVersusId = Guid.Parse(versusGameId);
        var parsedSessionId = Guid.Parse(gameSessionId);

        var versusGame = await _unitOfWork.VersusGames.GetByIdAsync(parsedVersusId);
        if (versusGame == null || versusGame.Status != VersusGameStatus.InProgress) return;

        var gameSession = await _unitOfWork.GameSessions.GetByIdAsync(parsedSessionId);
        if (gameSession == null || gameSession.Status != GameStatus.InProgress) return;

        var word = await _unitOfWork.Words.GetByIdAsync(versusGame.WordId);
        if (word == null) return;

        // Get current attempts
        var currentAttempts = await _unitOfWork.GameAttempts.FindAsync(a => a.GameSessionId == parsedSessionId);
        int attemptCount = currentAttempts.Count();

        if (attemptCount >= 3) return;

        // Check for tabu words
        var promptAnalysis = await _aiService.AnalyzePromptAsync(prompt, word.TargetWord, word.TabuWords);
        var groupName = $"versus_{versusGame.Id}";

        if (promptAnalysis.ContainsTabuWords)
        {
            await Clients.Caller.SendAsync("TabuWordDetected", new
            {
                detectedWords = promptAnalysis.DetectedTabuWords
            });
            return;
        }

        // Get AI guess
        var aiResult = await _aiService.GuessWordAsync(prompt, word.TargetWord, word.TabuWords);
        int currentAttemptNumber = attemptCount + 1;
        int score = CalculateScore(aiResult.IsCorrect, promptAnalysis.PromptQuality, currentAttemptNumber);

        // Create attempt
        var attempt = new GameAttempt
        {
            Id = Guid.NewGuid(),
            GameSessionId = parsedSessionId,
            AttemptNumber = currentAttemptNumber,
            UserPrompt = prompt,
            AiGuess = aiResult.GuessedWord,
            IsCorrect = aiResult.IsCorrect,
            Score = score,
            AiFeedback = promptAnalysis.Feedback,
            PromptQuality = (PromptQuality)promptAnalysis.PromptQuality,
            Suggestions = new List<string>(),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.GameAttempts.AddAsync(attempt);

        // Update game session
        gameSession.UserPrompt = prompt;
        gameSession.AiResponse = aiResult.GuessedWord;
        gameSession.IsCorrectGuess = aiResult.IsCorrect;
        gameSession.Score += score;
        gameSession.AttemptNumber = currentAttemptNumber;

        bool playerFinished = aiResult.IsCorrect || currentAttemptNumber >= 3;

        if (playerFinished)
        {
            gameSession.CompletedAt = DateTime.UtcNow;
            gameSession.Status = aiResult.IsCorrect ? GameStatus.Completed : GameStatus.Failed;
            gameSession.TimeSpent = gameSession.CompletedAt.Value - gameSession.StartedAt;
        }

        await _unitOfWork.GameSessions.UpdateAsync(gameSession);

        // Update versus game scores
        bool isPlayer1 = versusGame.Player1Id.ToString() == userId;
        if (isPlayer1)
        {
            versusGame.Player1Score = gameSession.Score;
            versusGame.Player1Attempts = currentAttemptNumber;
        }
        else
        {
            versusGame.Player2Score = gameSession.Score;
            versusGame.Player2Attempts = currentAttemptNumber;
        }

        await _unitOfWork.SaveChangesAsync();

        // Notify the group about the attempt
        await Clients.Group(groupName).SendAsync("PlayerAttemptResult", new
        {
            playerId = userId,
            attemptNumber = currentAttemptNumber,
            isCorrect = aiResult.IsCorrect,
            aiGuess = aiResult.GuessedWord,
            score = score,
            totalScore = gameSession.Score,
            playerFinished,
            prompt
        });

        // Check if both players are done
        if (playerFinished)
        {
            await CheckVersusGameCompletion(versusGame, groupName);
        }
    }

    private async Task CreateVersusMatch(string userId, User user, WaitingPlayer opponent)
    {
        var parsedUserId = Guid.Parse(userId);
        var parsedOpponentId = Guid.Parse(opponent.UserId);

        // Pick a random word
        var wordsQuery = await _unitOfWork.Words.FindAsync(w => w.IsActive);
        var words = wordsQuery.ToList();

        if (opponent.Category != null)
            words = words.Where(w => w.Category.Equals(opponent.Category, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!words.Any())
        {
            await Clients.Caller.SendAsync("MatchmakingError", "Uygun kelime bulunamadı.");
            return;
        }

        var random = new Random();
        var selectedWord = words[random.Next(words.Count)];

        var roomCode = GenerateRoomCode();

        var versusGame = new VersusGame
        {
            Id = Guid.NewGuid(),
            WordId = selectedWord.Id,
            Player1Id = parsedOpponentId,
            Player2Id = parsedUserId,
            Status = VersusGameStatus.InProgress,
            RoomCode = roomCode,
            CreatedAt = DateTime.UtcNow,
            StartedAt = DateTime.UtcNow
        };

        // Create game sessions
        var p1Session = new GameSession
        {
            Id = Guid.NewGuid(),
            UserId = parsedOpponentId,
            WordId = selectedWord.Id,
            Mode = GameMode.Multiplayer,
            StartedAt = DateTime.UtcNow,
            Status = GameStatus.InProgress
        };

        var p2Session = new GameSession
        {
            Id = Guid.NewGuid(),
            UserId = parsedUserId,
            WordId = selectedWord.Id,
            Mode = GameMode.Multiplayer,
            StartedAt = DateTime.UtcNow,
            Status = GameStatus.InProgress
        };

        await _unitOfWork.GameSessions.AddAsync(p1Session);
        await _unitOfWork.GameSessions.AddAsync(p2Session);

        versusGame.Player1GameSessionId = p1Session.Id;
        versusGame.Player2GameSessionId = p2Session.Id;

        await _unitOfWork.VersusGames.AddAsync(versusGame);
        await _unitOfWork.SaveChangesAsync();

        // Add both to group
        var groupName = $"versus_{versusGame.Id}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        if (_userConnections.TryGetValue(opponent.UserId, out var opponentConnection))
        {
            await Groups.AddToGroupAsync(opponentConnection, groupName);
        }

        var opponentUser = await _unitOfWork.Users.GetByIdAsync(parsedOpponentId);

        var gameData = new
        {
            versusGameId = versusGame.Id,
            roomCode,
            word = new
            {
                id = selectedWord.Id,
                targetWord = selectedWord.TargetWord,
                tabuWords = selectedWord.TabuWords,
                category = selectedWord.Category,
                difficulty = (int)selectedWord.Difficulty
            },
            player1 = new
            {
                id = parsedOpponentId,
                displayName = opponentUser?.DisplayName ?? opponentUser?.Username ?? "Oyuncu 1",
                gameSessionId = p1Session.Id
            },
            player2 = new
            {
                id = parsedUserId,
                displayName = user.DisplayName ?? user.Username,
                gameSessionId = p2Session.Id
            }
        };

        await Clients.Group(groupName).SendAsync("GameStarted", gameData);
    }

    private async Task CheckVersusGameCompletion(VersusGame versusGame, string groupName)
    {
        // Check if both players' sessions are complete
        GameSession? p1Session = null, p2Session = null;

        if (versusGame.Player1GameSessionId.HasValue)
            p1Session = await _unitOfWork.GameSessions.GetByIdAsync(versusGame.Player1GameSessionId.Value);
        if (versusGame.Player2GameSessionId.HasValue)
            p2Session = await _unitOfWork.GameSessions.GetByIdAsync(versusGame.Player2GameSessionId.Value);

        bool p1Done = p1Session?.Status is GameStatus.Completed or GameStatus.Failed;
        bool p2Done = p2Session?.Status is GameStatus.Completed or GameStatus.Failed;

        if (!p1Done || !p2Done) return;

        // Both done - determine winner
        versusGame.Status = VersusGameStatus.Completed;
        versusGame.CompletedAt = DateTime.UtcNow;

        // Winner logic: higher score wins; if tie, fewer attempts wins; if still tie, faster time wins
        Guid? winnerId = null;
        if (versusGame.Player1Score > versusGame.Player2Score)
            winnerId = versusGame.Player1Id;
        else if (versusGame.Player2Score > versusGame.Player1Score)
            winnerId = versusGame.Player2Id;
        else if (versusGame.Player1Attempts < versusGame.Player2Attempts)
            winnerId = versusGame.Player1Id;
        else if (versusGame.Player2Attempts < versusGame.Player1Attempts)
            winnerId = versusGame.Player2Id;
        else if (p1Session?.TimeSpent < p2Session?.TimeSpent)
            winnerId = versusGame.Player1Id;
        else if (p2Session?.TimeSpent < p1Session?.TimeSpent)
            winnerId = versusGame.Player2Id;
        // else it's a draw, winnerId stays null

        versusGame.WinnerId = winnerId;
        await _unitOfWork.VersusGames.UpdateAsync(versusGame);

        // Update user stats
        await UpdateVersusStats(versusGame);

        // Log activities
        await LogVersusActivity(versusGame);

        await _unitOfWork.SaveChangesAsync();

        // Notify both players
        var player1 = await _unitOfWork.Users.GetByIdAsync(versusGame.Player1Id);
        var player2 = versusGame.Player2Id.HasValue
            ? await _unitOfWork.Users.GetByIdAsync(versusGame.Player2Id.Value)
            : null;

        await Clients.Group(groupName).SendAsync("GameCompleted", new
        {
            versusGameId = versusGame.Id,
            winnerId,
            isDraw = winnerId == null,
            player1 = new
            {
                id = versusGame.Player1Id,
                displayName = player1?.DisplayName ?? player1?.Username,
                score = versusGame.Player1Score,
                attempts = versusGame.Player1Attempts
            },
            player2 = new
            {
                id = versusGame.Player2Id,
                displayName = player2?.DisplayName ?? player2?.Username,
                score = versusGame.Player2Score,
                attempts = versusGame.Player2Attempts
            }
        });
    }

    private async Task UpdateVersusStats(VersusGame versusGame)
    {
        var player1 = await _unitOfWork.Users.GetByIdAsync(versusGame.Player1Id);
        if (player1 != null)
        {
            player1.GamesPlayed++;
            player1.TotalScore += versusGame.Player1Score;
            if (versusGame.WinnerId == player1.Id)
                player1.GamesWon++;
            await _unitOfWork.Users.UpdateAsync(player1);
        }

        if (versusGame.Player2Id.HasValue)
        {
            var player2 = await _unitOfWork.Users.GetByIdAsync(versusGame.Player2Id.Value);
            if (player2 != null)
            {
                player2.GamesPlayed++;
                player2.TotalScore += versusGame.Player2Score;
                if (versusGame.WinnerId == player2.Id)
                    player2.GamesWon++;
                await _unitOfWork.Users.UpdateAsync(player2);
            }
        }
    }

    private async Task LogVersusActivity(VersusGame versusGame)
    {
        var p1Activity = new ActivityLog
        {
            Id = Guid.NewGuid(),
            UserId = versusGame.Player1Id,
            Type = versusGame.WinnerId == versusGame.Player1Id ? ActivityType.VersusWon
                 : versusGame.WinnerId == null ? ActivityType.VersusDraw
                 : ActivityType.VersusLost,
            Description = versusGame.WinnerId == versusGame.Player1Id
                ? $"Düello kazandı! Skor: {versusGame.Player1Score}"
                : versusGame.WinnerId == null
                    ? $"Düello berabere bitti. Skor: {versusGame.Player1Score}"
                    : $"Düelloyu kaybetti. Skor: {versusGame.Player1Score}",
            ScoreEarned = versusGame.Player1Score,
            RelatedEntityId = versusGame.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ActivityLogs.AddAsync(p1Activity);

        if (versusGame.Player2Id.HasValue)
        {
            var p2Activity = new ActivityLog
            {
                Id = Guid.NewGuid(),
                UserId = versusGame.Player2Id.Value,
                Type = versusGame.WinnerId == versusGame.Player2Id ? ActivityType.VersusWon
                     : versusGame.WinnerId == null ? ActivityType.VersusDraw
                     : ActivityType.VersusLost,
                Description = versusGame.WinnerId == versusGame.Player2Id
                    ? $"Düello kazandı! Skor: {versusGame.Player2Score}"
                    : versusGame.WinnerId == null
                        ? $"Düello berabere bitti. Skor: {versusGame.Player2Score}"
                        : $"Düelloyu kaybetti. Skor: {versusGame.Player2Score}",
                ScoreEarned = versusGame.Player2Score,
                RelatedEntityId = versusGame.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ActivityLogs.AddAsync(p2Activity);
        }
    }

    private static int CalculateScore(bool isCorrect, int promptQuality, int attemptNumber)
    {
        if (!isCorrect) return 0;
        int baseScore = 100;
        int qualityBonus = promptQuality * 20;
        int attemptPenalty = (attemptNumber - 1) * 20;
        return Math.Max(10, baseScore + qualityBonus - attemptPenalty);
    }

    private static string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Range(0, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }
}

public class WaitingPlayer
{
    public string UserId { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int? Difficulty { get; set; }
    public DateTime JoinedAt { get; set; }
}
