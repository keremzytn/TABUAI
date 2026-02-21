using Microsoft.EntityFrameworkCore.Storage;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;
using TabuAI.Infrastructure.Data;

namespace TabuAI.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly TabuAIDbContext _context;
    private IDbContextTransaction? _transaction;
    
    private IRepository<User>? _users;
    private IRepository<Word>? _words;
    private IRepository<GameSession>? _gameSessions;
    private IRepository<GameAttempt>? _gameAttempts;
    private IRepository<Badge>? _badges;
    private IRepository<UserBadge>? _userBadges;
    private IRepository<UserStatistic>? _userStatistics;

    public UnitOfWork(TabuAIDbContext context)
    {
        _context = context;
    }

    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Word> Words => _words ??= new Repository<Word>(_context);
    public IRepository<GameSession> GameSessions => _gameSessions ??= new Repository<GameSession>(_context);
    public IRepository<GameAttempt> GameAttempts => _gameAttempts ??= new Repository<GameAttempt>(_context);
    public IRepository<Badge> Badges => _badges ??= new Repository<Badge>(_context);
    public IRepository<UserBadge> UserBadges => _userBadges ??= new Repository<UserBadge>(_context);
    public IRepository<UserStatistic> UserStatistics => _userStatistics ??= new Repository<UserStatistic>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}