using MediatR;
using TabuAI.Application.Features.Users.DTOs;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Users.Queries;

public class GetStyleAnalysisQueryHandler : IRequestHandler<GetStyleAnalysisQuery, StyleAnalysisDto>
{
    private readonly IUnitOfWork _unitOfWork;

    private static readonly HashSet<string> TurkishAdjectives = new(StringComparer.OrdinalIgnoreCase)
    {
        "büyük", "küçük", "güzel", "çirkin", "hızlı", "yavaş", "eski", "yeni", "uzun", "kısa",
        "sıcak", "soğuk", "karanlık", "aydınlık", "yumuşak", "sert", "tatlı", "acı", "geniş", "dar",
        "derin", "sığ", "kalın", "ince", "ağır", "hafif", "temiz", "kirli", "zengin", "fakir",
        "mutlu", "üzgün", "korkak", "cesur", "önemli", "basit", "kolay", "zor", "meşhur", "sessiz"
    };

    private static readonly HashSet<string> TurkishVerbs = new(StringComparer.OrdinalIgnoreCase)
    {
        "yapmak", "etmek", "olmak", "gitmek", "gelmek", "almak", "vermek", "demek", "bilmek", "görmek",
        "istemek", "bulmak", "söylemek", "çalışmak", "başlamak", "düşünmek", "beklemek", "kalmak", "anlamak", "yazmak",
        "kullanmak", "açmak", "kapamak", "koymak", "tutmak", "taşımak", "çıkmak", "girmek", "oturmak", "kalkmak",
        "yapan", "eden", "olan", "giden", "gelen", "alan", "veren", "diyen", "bilen", "gören",
        "kullanan", "açan", "yapılan", "edilen", "kullanılan", "oluşan"
    };

    public GetStyleAnalysisQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StyleAnalysisDto> Handle(GetStyleAnalysisQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _unitOfWork.GameSessions.FindAsync(s => s.UserId == request.UserId);
        var sessionList = sessions.ToList();

        if (sessionList.Count == 0)
        {
            return new StyleAnalysisDto
            {
                PlayerTitle = "Yeni Başlayan",
                TitleEmoji = "\U0001F331",
                Description = "Henüz yeterli oyun verisi yok. Oynamaya devam et!"
            };
        }

        var sessionIds = sessionList.Select(s => s.Id).ToList();
        var attempts = await _unitOfWork.GameAttempts.FindAsync(a => sessionIds.Contains(a.GameSessionId));
        var attemptsList = attempts.ToList();

        var wordIds = sessionList.Select(s => s.WordId).Distinct().ToList();
        var words = await _unitOfWork.Words.FindAsync(w => wordIds.Contains(w.Id));
        var wordsList = words.ToList();

        var allPrompts = attemptsList.Select(a => a.UserPrompt).Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
        var allWordsInPrompts = allPrompts
            .SelectMany(p => p.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Select(w => w.ToLowerInvariant().Trim())
            .Where(w => w.Length > 1)
            .ToList();

        var totalWords = allWordsInPrompts.Count;
        var uniqueWords = allWordsInPrompts.Distinct().Count();
        var avgLength = allPrompts.Count > 0 ? allPrompts.Average(p => p.Length) : 0;

        int adjCount = allWordsInPrompts.Count(w => TurkishAdjectives.Contains(w));
        int verbCount = allWordsInPrompts.Count(w => TurkishVerbs.Contains(w));

        double adjRatio = totalWords > 0 ? (double)adjCount / totalWords * 100 : 0;
        double verbRatio = totalWords > 0 ? (double)verbCount / totalWords * 100 : 0;
        double uniqueRatio = totalWords > 0 ? (double)uniqueWords / totalWords * 100 : 0;

        string dominantStyle;
        if (adjRatio > verbRatio && adjRatio > 15) dominantStyle = "Tanımlayıcı";
        else if (verbRatio > adjRatio && verbRatio > 15) dominantStyle = "Aksiyoncu";
        else if (uniqueRatio > 70) dominantStyle = "Yaratıcı";
        else if (avgLength > 100) dominantStyle = "Detaycı";
        else if (avgLength < 30) dominantStyle = "Minimalist";
        else dominantStyle = "Dengeli";

        var (title, emoji, desc) = dominantStyle switch
        {
            "Tanımlayıcı" => ("Tanımlama Ustası", "\U0001F3A8", "Sıfatları ustaca kullanarak AI'ı yönlendiriyorsun!"),
            "Aksiyoncu" => ("Aksiyon Kahramanı", "\u26A1", "Fiillerle dolu promptların AI'ı harekete geçiriyor!"),
            "Yaratıcı" => ("Kelime Sihirbazı", "\U0001F9D9", "Zengin kelime dağarcığınla fark yaratıyorsun!"),
            "Detaycı" => ("Titiz Anlatıcı", "\U0001F50D", "Detaylı açıklamalarınla hiçbir şeyi kaçırmıyorsun!"),
            "Minimalist" => ("Az Laf Ustası", "\U0001F3AF", "Kısa ve öz promptlarınla hedefe gidiyorsun!"),
            _ => ("Dengeli Oyuncu", "\u2696\uFE0F", "Hem tanımlama hem aksiyon dengesini kuruyorsun!")
        };

        var winRate = sessionList.Count > 0 ? (double)sessionList.Count(s => s.IsCorrectGuess) / sessionList.Count * 100 : 0;
        var avgScore = sessionList.Count > 0 ? sessionList.Average(s => s.Score) : 0;

        var traits = new List<StyleTraitDto>
        {
            new() { Name = "Kelime Zenginliği", Emoji = "\U0001F4DA", Value = Math.Min(100, uniqueRatio), Description = $"{uniqueWords} benzersiz kelime kullandın" },
            new() { Name = "Sıfat Kullanımı", Emoji = "\U0001F3A8", Value = Math.Min(100, adjRatio * 3), Description = $"Promptlarının %{adjRatio:F1}'i sıfat" },
            new() { Name = "Fiil Gücü", Emoji = "\U0001F4AA", Value = Math.Min(100, verbRatio * 3), Description = $"Promptlarının %{verbRatio:F1}'i fiil" },
            new() { Name = "Başarı Oranı", Emoji = "\U0001F3AF", Value = winRate, Description = $"%{winRate:F0} doğru tahmin" },
            new() { Name = "Ortalama Skor", Emoji = "\u2B50", Value = Math.Min(100, avgScore / 2), Description = $"Ortalama {avgScore:F0} puan" }
        };

        var categoryPerf = sessionList
            .GroupBy(s => wordsList.FirstOrDefault(w => w.Id == s.WordId)?.Category ?? "Bilinmeyen")
            .Select(g => new CategoryPerformanceDto
            {
                Category = g.Key,
                GamesPlayed = g.Count(),
                SuccessRate = g.Count() > 0 ? (double)g.Count(s => s.IsCorrectGuess) / g.Count() * 100 : 0,
                AverageScore = g.Average(s => s.Score)
            })
            .OrderByDescending(c => c.GamesPlayed)
            .Take(5)
            .ToList();

        return new StyleAnalysisDto
        {
            PlayerTitle = title,
            TitleEmoji = emoji,
            Description = desc,
            Traits = traits,
            PromptStyle = new PromptStyleDto
            {
                AverageLength = avgLength,
                AdjectiveRatio = adjRatio,
                VerbRatio = verbRatio,
                UniqueWordRatio = uniqueRatio,
                DominantStyle = dominantStyle
            },
            TopCategories = categoryPerf
        };
    }
}
