using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TabuAI.Application.Common.DTOs;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;
using System.Security.Claims;

namespace TabuAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EconomyController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public EconomyController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());

    [HttpGet("balance")]
    public async Task<ActionResult<CoinBalanceDto>> GetBalance()
    {
        var userId = GetUserId();
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return NotFound();

        var transactions = (await _unitOfWork.CoinTransactions
            .FindAsync(t => t.UserId == userId))
            .OrderByDescending(t => t.CreatedAt)
            .Take(20)
            .Select(t => new CoinTransactionDto
            {
                Amount = t.Amount,
                Type = t.Type.ToString(),
                Description = t.Description,
                CreatedAt = t.CreatedAt
            })
            .ToList();

        return Ok(new CoinBalanceDto
        {
            Balance = user.PromptCoins,
            CurrentStreak = user.CurrentStreak,
            BestStreak = user.BestStreak,
            StreakMultiplier = GetStreakMultiplier(user.CurrentStreak),
            RecentTransactions = transactions
        });
    }

    [HttpPost("purchase-hint")]
    public async Task<ActionResult<PurchaseHintResult>> PurchaseHint([FromBody] PurchaseHintRequest request)
    {
        var userId = GetUserId();
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return NotFound();

        const int hintCost = 50;
        if (user.PromptCoins < hintCost)
        {
            return Ok(new PurchaseHintResult
            {
                Success = false,
                ErrorMessage = "Yeterli PromptCoin yok!",
                RemainingCoins = user.PromptCoins
            });
        }

        if (!Guid.TryParse(request.GameSessionId, out var sessionId))
            return BadRequest("Geçersiz oturum ID");

        var session = await _unitOfWork.GameSessions.GetByIdAsync(sessionId);
        if (session == null) return NotFound("Oyun oturumu bulunamadı");

        var word = await _unitOfWork.Words.GetByIdAsync(session.WordId);
        if (word == null) return NotFound("Kelime bulunamadı");

        var hint = GenerateHint(word);

        user.PromptCoins -= hintCost;
        await _unitOfWork.Users.UpdateAsync(user);

        await _unitOfWork.CoinTransactions.AddAsync(new CoinTransaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = -hintCost,
            Type = CoinTransactionType.HintPurchase,
            Description = $"İpucu satın alındı: {word.TargetWord}",
            CreatedAt = DateTime.UtcNow
        });

        await _unitOfWork.SaveChangesAsync();

        return Ok(new PurchaseHintResult
        {
            Success = true,
            Hint = hint,
            RemainingCoins = user.PromptCoins
        });
    }

    [HttpGet("shop")]
    public ActionResult<List<ShopItemDto>> GetShopItems()
    {
        var items = new List<ShopItemDto>
        {
            new() { Id = "hint", Name = "İpucu", Description = "Kelime hakkında bir ipucu al", Price = 50, Category = "Gameplay", Emoji = "💡" },
            new() { Id = "extra_attempt", Name = "Ekstra Deneme", Description = "+1 deneme hakkı", Price = 100, Category = "Gameplay", Emoji = "🔄" },
            new() { Id = "reveal_category", Name = "Kategori Göster", Description = "Kelimenin kategorisini göster", Price = 30, Category = "Gameplay", Emoji = "📂" },
            new() { Id = "streak_shield", Name = "Seri Kalkanı", Description = "Bir gün kaçırsan serin bozulmasın", Price = 200, Category = "Streak", Emoji = "🛡️" },
            new() { Id = "double_coins", Name = "2x Coin Boost", Description = "Sonraki 5 oyunda 2x coin kazan", Price = 300, Category = "Boost", Emoji = "⚡" }
        };

        return Ok(items);
    }

    private static string GenerateHint(Word word)
    {
        var hints = new List<string>
        {
            $"Kelimenin ilk harfi: {word.TargetWord[0]}",
            $"Kelime {word.TargetWord.Length} harften oluşuyor",
            $"Kategorisi: {word.Category}",
            $"Kelimede '{word.TargetWord[word.TargetWord.Length / 2]}' harfi var"
        };
        return hints[Random.Shared.Next(hints.Count)];
    }

    private static double GetStreakMultiplier(int streak)
    {
        return streak switch
        {
            >= 30 => 3.0,
            >= 14 => 2.5,
            >= 7 => 2.0,
            >= 5 => 1.5,
            >= 3 => 1.25,
            _ => 1.0
        };
    }
}
