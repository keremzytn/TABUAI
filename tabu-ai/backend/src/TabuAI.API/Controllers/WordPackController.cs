using Microsoft.AspNetCore.Mvc;
using TabuAI.Application.Common.DTOs;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WordPackController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public WordPackController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<List<WordPackDto>>> GetPublicPacks([FromQuery] string language = "tr")
    {
        try
        {
            var packs = await _unitOfWork.WordPacks.FindAsync(p => p.IsPublic && p.IsApproved && p.Language == language);
            var result = new List<WordPackDto>();
            foreach (var pack in packs.OrderByDescending(p => p.LikeCount))
            {
                var words = await _unitOfWork.Words.FindAsync(w => w.WordPackId == pack.Id && w.IsActive);
                var creator = await _unitOfWork.Users.GetByIdAsync(pack.CreatedByUserId);
                result.Add(new WordPackDto
                {
                    Id = pack.Id,
                    Name = pack.Name,
                    Description = pack.Description,
                    Language = pack.Language,
                    CreatedByUsername = creator?.DisplayName ?? creator?.Username ?? "Bilinmeyen",
                    CreatedByUserId = pack.CreatedByUserId,
                    IsPublic = pack.IsPublic,
                    IsApproved = pack.IsApproved,
                    PlayCount = pack.PlayCount,
                    LikeCount = pack.LikeCount,
                    WordCount = words.Count(),
                    CreatedAt = pack.CreatedAt
                });
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Paketler yüklenemedi", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WordPackDto>> GetPack(Guid id)
    {
        try
        {
            var pack = await _unitOfWork.WordPacks.GetByIdAsync(id);
            if (pack == null) return NotFound(new { message = "Paket bulunamadı" });

            var words = await _unitOfWork.Words.FindAsync(w => w.WordPackId == pack.Id && w.IsActive);
            var creator = await _unitOfWork.Users.GetByIdAsync(pack.CreatedByUserId);

            return Ok(new WordPackDto
            {
                Id = pack.Id,
                Name = pack.Name,
                Description = pack.Description,
                Language = pack.Language,
                CreatedByUsername = creator?.DisplayName ?? creator?.Username ?? "Bilinmeyen",
                CreatedByUserId = pack.CreatedByUserId,
                IsPublic = pack.IsPublic,
                IsApproved = pack.IsApproved,
                PlayCount = pack.PlayCount,
                LikeCount = pack.LikeCount,
                WordCount = words.Count(),
                CreatedAt = pack.CreatedAt,
                Words = words.Select(w => new WordDto
                {
                    Id = w.Id,
                    TargetWord = w.TargetWord,
                    TabuWords = w.TabuWords,
                    Category = w.Category,
                    Difficulty = (int)w.Difficulty,
                    Language = w.Language
                }).ToList()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Paket yüklenemedi", error = ex.Message });
        }
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<WordPackDto>>> GetMyPacks([FromQuery] string userId)
    {
        try
        {
            if (!Guid.TryParse(userId, out var parsedUserId))
                return BadRequest(new { message = "Geçersiz kullanıcı ID" });

            var packs = await _unitOfWork.WordPacks.FindAsync(p => p.CreatedByUserId == parsedUserId);
            var result = new List<WordPackDto>();
            foreach (var pack in packs.OrderByDescending(p => p.CreatedAt))
            {
                var wordCount = await _unitOfWork.Words.CountAsync(w => w.WordPackId == pack.Id);
                result.Add(new WordPackDto
                {
                    Id = pack.Id,
                    Name = pack.Name,
                    Description = pack.Description,
                    Language = pack.Language,
                    CreatedByUserId = pack.CreatedByUserId,
                    IsPublic = pack.IsPublic,
                    IsApproved = pack.IsApproved,
                    PlayCount = pack.PlayCount,
                    LikeCount = pack.LikeCount,
                    WordCount = wordCount,
                    CreatedAt = pack.CreatedAt
                });
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Paketler yüklenemedi", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<WordPackDto>> CreatePack([FromBody] CreateWordPackRequest request, [FromQuery] string userId)
    {
        try
        {
            if (!Guid.TryParse(userId, out var parsedUserId))
                return BadRequest(new { message = "Geçersiz kullanıcı ID" });

            if (request.Words.Count < 3)
                return BadRequest(new { message = "En az 3 kelime gerekli" });

            if (request.Words.Count > 50)
                return BadRequest(new { message = "En fazla 50 kelime eklenebilir" });

            var pack = new WordPack
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Language = request.Language,
                CreatedByUserId = parsedUserId,
                IsPublic = request.IsPublic,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.WordPacks.AddAsync(pack);

            foreach (var wordReq in request.Words)
            {
                var word = new Word
                {
                    Id = Guid.NewGuid(),
                    TargetWord = wordReq.TargetWord,
                    TabuWords = wordReq.TabuWords,
                    Category = wordReq.Category,
                    Difficulty = (DifficultyLevel)wordReq.Difficulty,
                    Language = request.Language,
                    CreatedByUserId = parsedUserId,
                    WordPackId = pack.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Words.AddAsync(word);
            }

            await _unitOfWork.SaveChangesAsync();

            return Ok(new WordPackDto
            {
                Id = pack.Id,
                Name = pack.Name,
                Description = pack.Description,
                Language = pack.Language,
                CreatedByUserId = parsedUserId,
                IsPublic = pack.IsPublic,
                WordCount = request.Words.Count,
                CreatedAt = pack.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Paket oluşturulamadı", error = ex.Message });
        }
    }

    [HttpPost("{id}/like")]
    public async Task<ActionResult> LikePack(Guid id)
    {
        try
        {
            var pack = await _unitOfWork.WordPacks.GetByIdAsync(id);
            if (pack == null) return NotFound();

            pack.LikeCount++;
            await _unitOfWork.WordPacks.UpdateAsync(pack);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { likeCount = pack.LikeCount });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Beğeni eklenemedi", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePack(Guid id, [FromQuery] string userId)
    {
        try
        {
            if (!Guid.TryParse(userId, out var parsedUserId))
                return BadRequest(new { message = "Geçersiz kullanıcı ID" });

            var pack = await _unitOfWork.WordPacks.GetByIdAsync(id);
            if (pack == null) return NotFound();
            if (pack.CreatedByUserId != parsedUserId)
                return Forbid();

            var words = await _unitOfWork.Words.FindAsync(w => w.WordPackId == id);
            foreach (var word in words)
            {
                await _unitOfWork.Words.DeleteAsync(word);
            }

            await _unitOfWork.WordPacks.DeleteAsync(pack);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Paket silinemedi", error = ex.Message });
        }
    }
}
