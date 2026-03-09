using TabuAI.Domain.Entities;

namespace TabuAI.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Word> Words { get; }
    IRepository<GameSession> GameSessions { get; }
    IRepository<GameAttempt> GameAttempts { get; }
    IRepository<Badge> Badges { get; }
    IRepository<UserBadge> UserBadges { get; }
    IRepository<UserStatistic> UserStatistics { get; }
    IRepository<Friendship> Friendships { get; }
    IRepository<VersusGame> VersusGames { get; }
    IRepository<Challenge> Challenges { get; }
    IRepository<ActivityLog> ActivityLogs { get; }
    IRepository<WordPack> WordPacks { get; }
    IRepository<DailyChallenge> DailyChallenges { get; }
    IRepository<DailyChallengeEntry> DailyChallengeEntries { get; }
    IRepository<CoinTransaction> CoinTransactions { get; }
    IRepository<ShopPurchase> ShopPurchases { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}