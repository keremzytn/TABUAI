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
            HasStreakShield = user.HasStreakShield,
            DoubleCoinGamesLeft = user.DoubleCoinGamesLeft,
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
    public async Task<ActionResult<List<ShopItemDto>>> GetShopItems()
    {
        var userId = GetUserId();
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        
        var items = new List<ShopItemDto>
        {
            new() { Id = "hint", Name = "İpucu", Description = "Kelime hakkında bir ipucu al", Price = 50, Category = "Gameplay", Emoji = "💡" },
            new() { Id = "extra_attempt", Name = "Ekstra Deneme", Description = "+1 deneme hakkı", Price = 100, Category = "Gameplay", Emoji = "🔄" },
            new() { Id = "reveal_category", Name = "Kategori Göster", Description = "Kelimenin kategorisini göster", Price = 30, Category = "Gameplay", Emoji = "📂" },
            new() { Id = "streak_shield", Name = "Seri Kalkanı", Description = "Bir gün kaçırsan serin bozulmasın", Price = 200, Category = "Streak", Emoji = "🛡️", IsOwned = user?.HasStreakShield ?? false },
            new() { Id = "double_coins", Name = "2x Coin Boost", Description = "Sonraki 5 oyunda 2x coin kazan", Price = 300, Category = "Boost", Emoji = "⚡", IsOwned = (user?.DoubleCoinGamesLeft ?? 0) > 0 },
            // Kozmetikler
            new() { Id = "avatar_flame", Name = "Ateş Avatarı", Description = "Profiline ateş avatarı", Price = 150, Category = "Avatar", Emoji = "🔥" },
            new() { Id = "avatar_star", Name = "Yıldız Avatarı", Description = "Profiline yıldız avatarı", Price = 150, Category = "Avatar", Emoji = "⭐" },
            new() { Id = "avatar_diamond", Name = "Elmas Avatarı", Description = "Profiline elmas avatarı", Price = 500, Category = "Avatar", Emoji = "💎" },
            new() { Id = "avatar_crown", Name = "Taç Avatarı", Description = "Profiline taç avatarı", Price = 400, Category = "Avatar", Emoji = "👑" },
            new() { Id = "card_neon", Name = "Neon Kart", Description = "Neon efektli oyun kartı", Price = 200, Category = "CardDesign", Emoji = "💜" },
            new() { Id = "card_gold", Name = "Altın Kart", Description = "Altın efektli oyun kartı", Price = 350, Category = "CardDesign", Emoji = "✨" },
            new() { Id = "card_galaxy", Name = "Galaksi Kart", Description = "Galaksi temalı oyun kartı", Price = 450, Category = "CardDesign", Emoji = "🌌" },
        };

        if (user != null)
        {
            var purchases = (await _unitOfWork.ShopPurchases.FindAsync(p => p.UserId == userId && p.IsActive)).ToList();
            foreach (var item in items.Where(i => i.Category is "Avatar" or "CardDesign"))
            {
                item.IsOwned = purchases.Any(p => p.ItemId == item.Id);
            }
        }

        return Ok(items);
    }

    [HttpPost("purchase")]
    public async Task<ActionResult<PurchaseResult>> PurchaseItem([FromBody] PurchaseItemRequest request)
    {
        var userId = GetUserId();
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return NotFound();

        var shopItems = new Dictionary<string, (string Name, int Price, string Category)>
        {
            ["streak_shield"] = ("Seri Kalkanı", 200, "Streak"),
            ["double_coins"] = ("2x Coin Boost", 300, "Boost"),
            ["avatar_flame"] = ("Ateş Avatarı", 150, "Avatar"),
            ["avatar_star"] = ("Yıldız Avatarı", 150, "Avatar"),
            ["avatar_diamond"] = ("Elmas Avatarı", 500, "Avatar"),
            ["avatar_crown"] = ("Taç Avatarı", 400, "Avatar"),
            ["card_neon"] = ("Neon Kart", 200, "CardDesign"),
            ["card_gold"] = ("Altın Kart", 350, "CardDesign"),
            ["card_galaxy"] = ("Galaksi Kart", 450, "CardDesign"),
        };

        if (!shopItems.TryGetValue(request.ItemId, out var item))
            return BadRequest(new PurchaseResult { Success = false, ErrorMessage = "Geçersiz ürün" });

        if (user.PromptCoins < item.Price)
            return Ok(new PurchaseResult { Success = false, ErrorMessage = "Yeterli PromptCoin yok!", RemainingCoins = user.PromptCoins });

        if (item.Category is "Avatar" or "CardDesign")
        {
            var alreadyOwned = await _unitOfWork.ShopPurchases.ExistsAsync(
                p => p.UserId == userId && p.ItemId == request.ItemId && p.IsActive);
            if (alreadyOwned)
                return Ok(new PurchaseResult { Success = false, ErrorMessage = "Bu ürüne zaten sahipsin!", RemainingCoins = user.PromptCoins });
        }

        user.PromptCoins -= item.Price;

        var transactionType = item.Category switch
        {
            "Avatar" => CoinTransactionType.AvatarPurchase,
            "CardDesign" => CoinTransactionType.CardDesignPurchase,
            _ => CoinTransactionType.ShopPurchase
        };

        await _unitOfWork.CoinTransactions.AddAsync(new CoinTransaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = -item.Price,
            Type = transactionType,
            Description = $"{item.Name} satın alındı",
            CreatedAt = DateTime.UtcNow
        });

        switch (request.ItemId)
        {
            case "streak_shield":
                user.HasStreakShield = true;
                break;
            case "double_coins":
                user.DoubleCoinGamesLeft += 5;
                break;
            default:
                await _unitOfWork.ShopPurchases.AddAsync(new ShopPurchase
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ItemId = request.ItemId,
                    ItemName = item.Name,
                    Price = item.Price,
                    IsActive = true,
                    PurchasedAt = DateTime.UtcNow
                });
                break;
        }

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new PurchaseResult
        {
            Success = true,
            RemainingCoins = user.PromptCoins,
            Message = $"{item.Name} başarıyla satın alındı!"
        });
    }

    [HttpPost("equip")]
    public async Task<ActionResult> EquipItem([FromBody] EquipItemRequest request)
    {
        var userId = GetUserId();
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return NotFound();

        if (request.Type == "avatar")
        {
            if (request.ItemId == null)
            {
                user.SelectedAvatar = null;
            }
            else
            {
                var owned = await _unitOfWork.ShopPurchases.ExistsAsync(
                    p => p.UserId == userId && p.ItemId == request.ItemId && p.IsActive);
                if (!owned) return BadRequest(new { message = "Bu ürüne sahip değilsin" });
                user.SelectedAvatar = request.ItemId;
            }
        }
        else if (request.Type == "card")
        {
            if (request.ItemId == null)
            {
                user.SelectedCardDesign = null;
            }
            else
            {
                var owned = await _unitOfWork.ShopPurchases.ExistsAsync(
                    p => p.UserId == userId && p.ItemId == request.ItemId && p.IsActive);
                if (!owned) return BadRequest(new { message = "Bu ürüne sahip değilsin" });
                user.SelectedCardDesign = request.ItemId;
            }
        }

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return Ok(new { message = "Ürün seçildi!", avatar = user.SelectedAvatar, cardDesign = user.SelectedCardDesign });
    }

    [HttpGet("inventory")]
    public async Task<ActionResult> GetInventory()
    {
        var userId = GetUserId();
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return NotFound();

        var purchases = (await _unitOfWork.ShopPurchases.FindAsync(p => p.UserId == userId && p.IsActive))
            .Select(p => new { p.ItemId, p.ItemName, p.PurchasedAt })
            .ToList();

        return Ok(new
        {
            selectedAvatar = user.SelectedAvatar,
            selectedCardDesign = user.SelectedCardDesign,
            hasStreakShield = user.HasStreakShield,
            doubleCoinGamesLeft = user.DoubleCoinGamesLeft,
            ownedItems = purchases
        });
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
