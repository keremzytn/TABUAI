using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using Polly;
using Polly.Retry;
using System.ClientModel;
using System.Globalization;
using System.Text.Json;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Infrastructure.Services;

/// <summary>
/// AI Service implementation using Groq API (OpenAI-compatible)
/// </summary>
public class GroqService : IAiService
{
    private readonly OpenAIClient _groqClient;
    private readonly ILogger<GroqService> _logger;
    private readonly string _model;
    private readonly string _premiumModel;
    private readonly ICacheService _cacheService;
    private readonly AsyncRetryPolicy _retryPolicy;

    // Türkçe karakter normalizasyon tablosu
    private static readonly Dictionary<char, char> TurkishCharMap = new()
    {
        { 'ç', 'c' }, { 'Ç', 'C' },
        { 'ğ', 'g' }, { 'Ğ', 'G' },
        { 'ı', 'i' }, { 'I', 'I' },
        { 'İ', 'i' }, { 'i', 'i' },
        { 'ö', 'o' }, { 'Ö', 'O' },
        { 'ş', 's' }, { 'Ş', 'S' },
        { 'ü', 'u' }, { 'Ü', 'U' }
    };

    // Yaygın Türkçe ekler (çekim/hal/iyelik/çoğul)
    private static readonly string[] TurkishSuffixes = new[]
    {
        // Çoğul
        "ler", "lar",
        // Hal ekleri
        "ı", "i", "u", "ü",           // Belirtme
        "ın", "in", "un", "ün",       // Tamlayan
        "a", "e",                       // Yönelme
        "da", "de", "ta", "te",       // Bulunma
        "dan", "den", "tan", "ten",   // Ayrılma
        // İyelik
        "ım", "im", "um", "üm",
        "ın", "in", "un", "ün",
        "ımız", "imiz", "umuz", "ümüz",
        "ınız", "iniz", "unuz", "ünüz",
        "ları", "leri",
        // Sıfat fiil / fiilden isim
        "lık", "lik", "luk", "lük",
        "cı", "ci", "cu", "cü",
        "sız", "siz", "suz", "süz",
        // Birleşik ekler
        "ları", "leri",
        "lardan", "lerden",
        "lara", "lere",
        "ında", "inde", "unda", "ünde",
        "ından", "inden", "undan", "ünden",
        "ına", "ine", "una", "üne",
        "ıyla", "iyle", "uyla", "üyle",
        "yla", "yle",
    };

    public GroqService(IConfiguration configuration, ILogger<GroqService> logger, ICacheService cacheService)
    {
        var apiKey = configuration["Groq:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "Groq API key is not configured");
        var baseUrl = configuration["Groq:BaseUrl"] ?? "https://api.groq.com/openai/v1";
        _model = configuration["Groq:Model"] ?? "llama-3.1-8b-instant";
        _premiumModel = configuration["Groq:PremiumModel"] ?? "llama-3.3-70b-versatile";

        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri(baseUrl)
        };

        _groqClient = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        _logger = logger;
        _cacheService = cacheService;

        // Retry policy: 3 deneme, exponential backoff (1s, 2s, 4s)
        _retryPolicy = Policy
            .Handle<Exception>(ex => ex is not ArgumentException and not InvalidOperationException)
            .WaitAndRetryAsync(
                retryCount: 2,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)),
                onRetry: (exception, timeSpan, retryCount, _) =>
                {
                    _logger.LogWarning(exception,
                        "Groq API çağrısı başarısız oldu. Deneme {RetryCount}, {Delay}s sonra tekrar denenecek.",
                        retryCount, timeSpan.TotalSeconds);
                });
    }

    private static readonly Dictionary<string, string> PersonaPrompts = new()
    {
        { "sarcastic", @"Sen sarkastik ve alaycı bir TABU oyuncususun. Her tahminde iğneleyici ve komik yorumlar yaparsın.
Örnekler: 'Vay canına, bu tanımlama ile bunu mu anlatmaya çalışıyorsun? Neyse, tahminim...', 'Aa ne güzel anlattın, hiç anlamadım ama...'
Tahminden SONRA kısa bir sarkastik yorum ekle." },
        { "childish", @"Sen 7 yaşında heyecanlı bir çocuksun ve TABU oynuyorsun! Her şeye aşırı seviniyorsun.
Örnekler: 'Ohhh ben biliyorum ben biliyorum!!!', 'Yuppiiii bu çok kolaydı!', 'Hımmmm bu zor oldu ama ben çok zekiyim!'
Tahminden SONRA çocuksu bir tepki ekle." },
        { "meticulous", @"Sen aşırı titiz ve detaycı bir profesörsün. Her tanımlamayı akademik bir şekilde analiz edersin.
Örnekler: 'Semantik analiz sonucunda...', 'Verilen parametrelere göre %87.3 olasılıkla...', 'Hmm, bu tanımlama 3 farklı kategoriye uyuyor, ama...'
Tahminden SONRA akademik bir analiz ekle." },
        { "dramatic", @"Sen aşırı dramatik bir tiyatro oyuncususun. Her tahmini büyük bir sahne gibi yaparsın.
Örnekler: 'TANRILAR AŞKINA! Bu kelime... bu kelime...', 'Kalbim duracak gibi... tahminim...', 'Ah! Eureka anı geldi!'
Tahminden SONRA dramatik bir tepki ekle." },
        { "philosopher", @"Sen derin düşünen bir filozofsun. Her kelimeyi varoluşsal bir perspektiften değerlendirirsin.
Örnekler: 'Bu tanımlama beni hayatın anlamı üzerine düşündürdü...', 'Descartes olsaydı şöyle derdi...', 'Kelimenin özü ile tanımı arasındaki boşluk...'
Tahminden SONRA felsefi bir yorum ekle." }
    };

    public Task<AiGuessResult> GuessWordAsync(string prompt, string targetWord, List<string> tabuWords, string? persona = null)
    {
        return GuessWordInternalAsync(prompt, targetWord, tabuWords, persona, _model);
    }

    public Task<AiGuessResult> GuessWordWithModelAsync(string prompt, string targetWord, List<string> tabuWords, string? persona = null, string? modelOverride = null)
    {
        return GuessWordInternalAsync(prompt, targetWord, tabuWords, persona, modelOverride ?? _premiumModel);
    }

    private async Task<AiGuessResult> GuessWordInternalAsync(string prompt, string targetWord, List<string> tabuWords, string? persona, string model)
    {
        try
        {
            var cacheKey = $"ai_guess:{Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes($"{prompt}:{targetWord}:{model}")))}";
            var cached = await _cacheService.GetAsync<AiGuessResult>(cacheKey);
            if (cached != null) return cached;

            var personaExtra = "";
            if (!string.IsNullOrEmpty(persona) && PersonaPrompts.TryGetValue(persona.ToLower(), out var personaPrompt))
            {
                personaExtra = $"\n\nKARAKTER ROLÜN:\n{personaPrompt}";
            }

            var systemMessage = $@"Sen bir TABU oyunu oyuncususun. Karşındaki kişi bir kelimeyi anlatmaya çalışıyor ve sen bu tanımlamadan kelimeyi tahmin etmelisin.

ÖNEMLİ KURALLAR:
1. SADECE kullanıcının verdiği tanımlamayı kullanarak tahmin yap. Başka hiçbir bilgin yok.
2. Tanımlama belirsiz veya yetersizse, yine de en mantıklı tahmini yap ama düşük confidence ver.
3. Tanımlama çok genel veya birden fazla kelimeye uyuyorsa, en yaygın olanı seç ve orta seviye confidence ver.
4. Sadece tanımlamada açıkça belirtilen özelliklere dayanarak tahmin yap. Varsayımda bulunma.
5. Eğer tanımlama çok kısa veya anlamsızsa, genel bir tahmin yap ve çok düşük confidence ver.
6. Sadece TEK KELİME ile cevap ver (birleşik kelimeler kabul edilir, örn: 'buzdolabı').
7. Türkçe kelimelerle cevap ver.
{personaExtra}
Cevabını SADECE şu JSON formatında ver, başka hiçbir şey yazma:
{{""word"": ""tahmin_ettiğin_kelime"", ""confidence"": 0.85, ""reaction"": ""persona tepkin (varsa)""}}

Confidence seviyeleri:
- 0.9-1.0: Tanımlama çok net, kesinlikle bu kelime
- 0.7-0.9: Büyük ihtimalle bu kelime
- 0.5-0.7: Olabilir ama başka seçenekler de var
- 0.3-0.5: Tahmin, belirsiz
- 0.0-0.3: Çok belirsiz, rastgele tahmin";

            string content = null!;
            await _retryPolicy.ExecuteAsync(async () =>
            {
                var chatClient = _groqClient.GetChatClient(model);
                var response = await chatClient.CompleteChatAsync(
                    new ChatMessage[]
                    {
                        new SystemChatMessage(systemMessage),
                        new UserChatMessage(prompt)
                    });
                content = response.Value.Content[0].Text;
            });

            _logger.LogInformation("AI Response (model={Model}): {Response}", model, content);

            // Extract and parse JSON
            var jsonContent = ExtractJson(content);
            try
            {
                var jsonResponse = JsonSerializer.Deserialize<AiResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (jsonResponse?.Word != null)
                {
                    var guessedWord = jsonResponse.Word.Trim();
                    var isCorrect = IsWordMatch(guessedWord, targetWord);
                    var reaction = jsonResponse.Reaction?.Trim();

                    var result = new AiGuessResult
                    {
                        GuessedWord = guessedWord,
                        IsCorrect = isCorrect,
                        Confidence = jsonResponse.Confidence,
                        Reasoning = $"AI tahmini: {guessedWord} (Güven: {jsonResponse.Confidence:P0})",
                        Reaction = reaction ?? ""
                    };

                    await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromHours(1));
                    return result;
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse AI guess JSON response. Using fallback.");
                var cleanedResponse = content.Trim().Trim('"');
                if (cleanedResponse.Length > 50) cleanedResponse = cleanedResponse[..50];

                var isCorrectFallback = IsWordMatch(cleanedResponse, targetWord);
                return new AiGuessResult
                {
                    GuessedWord = cleanedResponse,
                    IsCorrect = isCorrectFallback,
                    Confidence = 0.1,
                    Reasoning = "AI format hatası oluştu ama tahmin değerlendirildi"
                };
            }

            return new AiGuessResult
            {
                GuessedWord = "Bilinmiyor",
                IsCorrect = false,
                Confidence = 0.0,
                Reasoning = "AI yanıt veremedi"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting AI guess (tüm retry denemeleri başarısız)");
            return new AiGuessResult
            {
                GuessedWord = "Hata",
                IsCorrect = false,
                Confidence = 0.0,
                Reasoning = "AI servisi hatası"
            };
        }
    }

    public async Task<PromptAnalysisResult> AnalyzePromptAsync(string prompt, string targetWord, List<string> tabuWords)
    {
        try
        {
            // Türkçe ek/çekim farkındalıklı tabu kelime kontrolü
            var detectedTabuWords = DetectTabuWords(prompt, tabuWords);

            if (detectedTabuWords.Any())
            {
                return new PromptAnalysisResult
                {
                    PromptQuality = 1,
                    ContainsTabuWords = true,
                    DetectedTabuWords = detectedTabuWords,
                    Feedback = $"Tabu kelimeler kullandınız: {string.Join(", ", detectedTabuWords)}",
                    Strengths = new List<string>(),
                    Weaknesses = new List<string> { "Tabu kelimeler kullanıldı" }
                };
            }

            var systemMessage = $@"Sen bir TABU oyunu prompt değerlendirme uzmanısın. Kullanıcının bir kelimeyi anlatmak için yazdığı tanımlamayı analiz et.

Hedef kelime: {targetWord}
Yasaklı (tabu) kelimeler: {string.Join(", ", tabuWords)}

Değerlendirme kriterleri:
1. **Açıklık ve netlik**: Tanımlama hedef kelimeyi ne kadar iyi tarif ediyor? Sadece bu kelimeyi mi işaret ediyor yoksa birçok kelimeye mi uyuyor?
2. **Yaratıcılık**: Tanımlama sıradan mı yoksa akıllıca bir yaklaşım mı kullanıyor? Benzetme, metafor veya farklı bakış açısı kullanılmış mı?
3. **Tabu kelimelerden kaçınma**: Yasaklı kelimelerin kendisi veya türevleri (ekleri, çekimleri) kullanılmış mı?
4. **Hedef kelimeye yönlendirme gücü**: Tanımlama doğrudan hedef kelimeye mi götürüyor yoksa çok genel mi kalıyor?
5. **Kısalık ve öz**: Gereksiz uzun mu yoksa yeterli bilgiyi kısa ve öz mü veriyor?

Puanlama:
- 1: Çok kötü - Tanımlama anlamsız, alakasız veya tabu kelime içeriyor
- 2: Zayıf - Tanımlama çok genel, birçok kelimeye uyabilir
- 3: Orta - Tanımlama makul ama daha iyi olabilir, hedef kelime tahmin edilebilir
- 4: İyi - Tanımlama net ve yaratıcı, hedef kelimeye güçlü şekilde yönlendiriyor
- 5: Mükemmel - Tanımlama kısa, yaratıcı ve hedef kelimeyi neredeyse kesin olarak işaret ediyor

Cevabını SADECE şu JSON formatında ver, başka hiçbir şey yazma:
{{
    ""quality"": 3,
    ""feedback"": ""Genel değerlendirme cümlesi"",
    ""strengths"": [""Güçlü yön 1"", ""Güçlü yön 2""],
    ""weaknesses"": [""Zayıf yön 1"", ""Zayıf yön 2""]
}}";

            string content = null!;
            await _retryPolicy.ExecuteAsync(async () =>
            {
                var chatClient = _groqClient.GetChatClient(_model);
                var response = await chatClient.CompleteChatAsync(
                    new ChatMessage[]
                    {
                        new SystemChatMessage(systemMessage),
                        new UserChatMessage($"Analiz edilecek prompt: {prompt}")
                    });
                content = response.Value.Content[0].Text;
            });

            _logger.LogInformation("Prompt Analysis Response: {Response}", content);

            try
            {
                var jsonContent = ExtractJson(content);
                var analysisResponse = JsonSerializer.Deserialize<PromptAnalysisResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (analysisResponse != null)
                {
                    return new PromptAnalysisResult
                    {
                        PromptQuality = Math.Max(1, Math.Min(5, analysisResponse.Quality)),
                        ContainsTabuWords = false,
                        DetectedTabuWords = new List<string>(),
                        Feedback = analysisResponse.Feedback ?? "Değerlendirme yapıldı",
                        Strengths = analysisResponse.Strengths ?? new List<string>(),
                        Weaknesses = analysisResponse.Weaknesses ?? new List<string>()
                    };
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse prompt analysis JSON response");
            }

            // Fallback to simple analysis
            return new PromptAnalysisResult
            {
                PromptQuality = 3,
                ContainsTabuWords = false,
                DetectedTabuWords = new List<string>(),
                Feedback = "Promptunuz değerlendirildi. Daha açıklayıcı olabilir.",
                Strengths = new List<string> { "Tabu kelimelerden kaçınıldı" },
                Weaknesses = new List<string> { "Daha detaylı açıklama yapılabilir" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while analyzing prompt");
            return new PromptAnalysisResult
            {
                PromptQuality = 1,
                ContainsTabuWords = false,
                DetectedTabuWords = new List<string>(),
                Feedback = "Prompt analizi yapılamadı",
                Strengths = new List<string>(),
                Weaknesses = new List<string> { "Sistem hatası" }
            };
        }
    }

    public async Task<List<string>> GenerateImprovementSuggestionsAsync(string prompt, string targetWord, List<string> tabuWords, bool wasCorrect)
    {
        try
        {
            var systemMessage = $@"Sen bir TABU oyunu koçusun. Kullanıcının bir kelimeyi anlatmak için yazdığı tanımlamayı analiz et ve nasıl daha iyi yazabileceğini öner.

Hedef kelime: {targetWord}
Yasaklı (tabu) kelimeler: {string.Join(", ", tabuWords)}
AI tahmin edebildi mi: {(wasCorrect ? "Evet, doğru tahmin etti" : "Hayır, yanlış tahmin etti")}

{(wasCorrect ? @"Tahmin doğruydu ama yine de daha iyi prompt yazma teknikleri öner:
- Daha kısa ve öz anlatım
- Daha yaratıcı yaklaşımlar
- İlk denemede doğru tahmin ettirecek stratejiler" : @"Tahmin yanlıştı. Şu konularda öneriler ver:
- Tanımlamanın neden yetersiz kaldığını açıkla
- Hedef kelimeye daha iyi nasıl yönlendirileceğini anlat
- Somut alternatif tanımlama örnekleri ver (tabu kelimeler kullanmadan)
- Farklı bakış açıları ve stratejiler öner")}

3-5 adet SOMUT ve UYGULANABILIR öneri ver. Her öneri spesifik olsun, genel tavsiyeler verme.
Mümkünse örnek tanımlama cümleleri de ekle.

Cevabını SADECE şu JSON formatında ver, başka hiçbir şey yazma:
{{
    ""suggestions"": [""Öneri 1"", ""Öneri 2"", ""Öneri 3""]
}}";

            string content = null!;
            await _retryPolicy.ExecuteAsync(async () =>
            {
                var chatClient = _groqClient.GetChatClient(_model);
                var response = await chatClient.CompleteChatAsync(
                    new ChatMessage[]
                    {
                        new SystemChatMessage(systemMessage),
                        new UserChatMessage($"İyileştirilecek prompt: {prompt}")
                    });
                content = response.Value.Content[0].Text;
            });

            _logger.LogInformation("Suggestions Response: {Response}", content);

            try
            {
                var jsonContent = ExtractJson(content);
                var suggestionsResponse = JsonSerializer.Deserialize<SuggestionsResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (suggestionsResponse?.Suggestions != null && suggestionsResponse.Suggestions.Any())
                {
                    return suggestionsResponse.Suggestions.ToList();
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse suggestions JSON response");
            }

            // Fallback suggestions
            return wasCorrect
                ? new List<string>
                {
                    "Tebrikler! Başarılı bir prompt yazdınız.",
                    "Daha yaratıcı tanımlamalar deneyebilirsiniz.",
                    "Farklı açılardan yaklaşmayı deneyin."
                }
                : new List<string>
                {
                    "Daha spesifik detaylar ekleyin.",
                    "Nesnenin kullanım alanlarını belirtin.",
                    "Fiziksel özelliklerini tanımlayın.",
                    "Benzetmeler kullanmayı deneyin."
                };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating suggestions");
            return new List<string>
            {
                "Sistem hatası nedeniyle öneri oluşturulamadı.",
                "Daha sonra tekrar deneyin."
            };
        }
    }

    public async Task<PromptCoachResult> GeneratePromptCoachAnalysisAsync(string targetWord, List<string> tabuWords, List<PromptAttemptInfo> attempts)
    {
        try
        {
            var attemptsText = string.Join("\n", attempts.Select((a, i) => 
                $"Deneme {i + 1}: Prompt=\"{a.UserPrompt}\" → AI Tahmini=\"{a.AiGuess}\" → {(a.IsCorrect ? "DOĞRU" : "YANLIŞ")} (Kalite: {a.PromptQuality}/5)"));

            var systemMessage = $@"Sen bir Prompt Mühendisliği Koçusun. TABU oyununda kullanıcının tüm denemelerini analiz edip ona yapıcı geri bildirim vereceksin.

Hedef kelime: {targetWord}
Yasaklı (tabu) kelimeler: {string.Join(", ", tabuWords)}

Kullanıcının denemeleri:
{attemptsText}

Şu formatta analiz yap:
1. Genel bir değerlendirme (kullanıcının güçlü ve zayıf yönleri)
2. En iyi prompt hangisiydi ve neden
3. Bu kelimeyi anlatmak için İDEAL bir prompt örneği yaz (tabu kelimeler kullanmadan)
4. Gelecek oyunlar için 3-4 somut prompt mühendisliği ipucu

Cevabını SADECE şu JSON formatında ver:
{{
    ""overallAnalysis"": ""Genel değerlendirme..."",
    ""bestPrompt"": ""En iyi prompt ve neden..."",
    ""idealPromptExample"": ""İdeal prompt örneği..."",
    ""tipsForNextTime"": [""İpucu 1"", ""İpucu 2"", ""İpucu 3""],
    ""promptEngineeringScore"": 7
}}

promptEngineeringScore: 1-10 arası (kullanıcının genel prompt yazma becerisi)";

            string content = null!;
            await _retryPolicy.ExecuteAsync(async () =>
            {
                var chatClient = _groqClient.GetChatClient(_model);
                var response = await chatClient.CompleteChatAsync(
                    new ChatMessage[]
                    {
                        new SystemChatMessage(systemMessage),
                        new UserChatMessage("Lütfen denemelerimi analiz et ve bana geri bildirim ver.")
                    });
                content = response.Value.Content[0].Text;
            });

            _logger.LogInformation("Prompt Coach Response: {Response}", content);

            try
            {
                var jsonContent = ExtractJson(content);
                var coachResponse = JsonSerializer.Deserialize<PromptCoachResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (coachResponse != null)
                {
                    return new PromptCoachResult
                    {
                        OverallAnalysis = coachResponse.OverallAnalysis ?? "Analiz yapıldı.",
                        BestPrompt = coachResponse.BestPrompt ?? "",
                        IdealPromptExample = coachResponse.IdealPromptExample ?? "",
                        TipsForNextTime = coachResponse.TipsForNextTime ?? new List<string>(),
                        PromptEngineeringScore = Math.Max(1, Math.Min(10, coachResponse.PromptEngineeringScore))
                    };
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse prompt coach JSON response");
            }

            return new PromptCoachResult
            {
                OverallAnalysis = "Analiz tamamlandı. Daha detaylı açıklamalar deneyin.",
                BestPrompt = "",
                IdealPromptExample = "",
                TipsForNextTime = new List<string> { "Daha spesifik olun", "Metaforlar kullanın", "Kısa ve öz olun" },
                PromptEngineeringScore = 5
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GeneratePromptCoachAnalysisAsync");
            return new PromptCoachResult
            {
                OverallAnalysis = "Koç analizi yapılamadı.",
                TipsForNextTime = new List<string> { "Sistem hatası oluştu." },
                PromptEngineeringScore = 5
            };
        }
    }

    #region Helper Methods

    /// <summary>
    /// Türkçe karakter ve çekim ekleri farkındalıklı kelime eşleştirme.
    /// "Kahve", "kahve", "KAHVE", "kahveler" gibi varyasyonları doğru olarak eşleştirir.
    /// </summary>
    private static bool IsWordMatch(string guessedWord, string targetWord)
    {
        if (string.IsNullOrWhiteSpace(guessedWord) || string.IsNullOrWhiteSpace(targetWord))
            return false;

        var normalizedGuess = NormalizeTurkish(guessedWord.Trim());
        var normalizedTarget = NormalizeTurkish(targetWord.Trim());

        // Tam eşleşme (Türkçe normalize edilmiş)
        if (normalizedGuess == normalizedTarget)
            return true;

        // Tahmin, hedef kelimenin ekli hali mi? (örn: tahmin="kahveler", hedef="kahve")
        if (normalizedGuess.StartsWith(normalizedTarget) && normalizedGuess.Length <= normalizedTarget.Length + 6)
        {
            var suffix = normalizedGuess[normalizedTarget.Length..];
            if (IsTurkishSuffix(suffix))
                return true;
        }

        // Hedef kelime, tahminin ekli hali mi? (örn: tahmin="kahve", hedef="kahveler" - olası değil ama güvenlik)
        if (normalizedTarget.StartsWith(normalizedGuess) && normalizedTarget.Length <= normalizedGuess.Length + 6)
        {
            var suffix = normalizedTarget[normalizedGuess.Length..];
            if (IsTurkishSuffix(suffix))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Türkçe ek/çekim farkındalıklı tabu kelime tespiti.
    /// "çay" tabu kelimesi varken "çayı", "çaydan", "çaylar" gibi kullanımları da yakalar.
    /// </summary>
    private static List<string> DetectTabuWords(string prompt, List<string> tabuWords)
    {
        var detected = new List<string>();
        var promptNormalized = NormalizeTurkish(prompt);
        var promptWords = promptNormalized.Split(new[] { ' ', ',', '.', '!', '?', ';', ':', '\'', '"', '(', ')', '-', '\n', '\r', '\t' },
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var tabuWord in tabuWords)
        {
            var normalizedTabu = NormalizeTurkish(tabuWord);

            foreach (var promptWord in promptWords)
            {
                // Tam eşleşme
                if (promptWord == normalizedTabu)
                {
                    detected.Add(tabuWord);
                    break;
                }

                // Prompt kelimesi tabu kelimeyle başlıyor mu? (ekli kullanım)
                if (promptWord.StartsWith(normalizedTabu) && promptWord.Length <= normalizedTabu.Length + 6)
                {
                    var suffix = promptWord[normalizedTabu.Length..];
                    if (suffix.Length == 0 || IsTurkishSuffix(suffix))
                    {
                        detected.Add(tabuWord);
                        break;
                    }
                }

                // Kısa tabu kelimeleri için (3 harf veya daha az) sadece tam eşleşme veya bilinen ekler
                // Uzun tabu kelimeleri için daha esnek kontrol
                if (normalizedTabu.Length > 3 && promptWord.Contains(normalizedTabu))
                {
                    detected.Add(tabuWord);
                    break;
                }
            }
        }

        return detected;
    }

    /// <summary>
    /// Türkçe karakterleri ASCII karşılıklarına dönüştürür ve küçük harfe çevirir.
    /// "Çay" -> "cay", "Güneş" -> "gunes", "KAHVE" -> "kahve"
    /// </summary>
    private static string NormalizeTurkish(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        // Önce Türkçe kültürüne göre küçük harfe çevir (İ->i, I->ı doğru çalışsın)
        var lower = input.ToLower(new CultureInfo("tr-TR"));

        var chars = new char[lower.Length];
        for (int i = 0; i < lower.Length; i++)
        {
            chars[i] = lower[i] switch
            {
                'ç' => 'c',
                'ğ' => 'g',
                'ı' => 'i',
                'ö' => 'o',
                'ş' => 's',
                'ü' => 'u',
                _ => lower[i]
            };
        }

        return new string(chars);
    }

    /// <summary>
    /// Verilen suffix'in bilinen bir Türkçe ek olup olmadığını kontrol eder.
    /// </summary>
    private static bool IsTurkishSuffix(string suffix)
    {
        if (string.IsNullOrEmpty(suffix)) return true; // Boş suffix = tam eşleşme

        var normalizedSuffix = NormalizeTurkish(suffix);
        return TurkishSuffixes.Any(s => NormalizeTurkish(s) == normalizedSuffix);
    }

    private static string ExtractJson(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return "{}";

        var firstBrace = content.IndexOf('{');
        var lastBrace = content.LastIndexOf('}');

        if (firstBrace != -1 && lastBrace != -1 && lastBrace > firstBrace)
        {
            return content.Substring(firstBrace, lastBrace - firstBrace + 1);
        }

        return content;
    }

    #endregion

    #region Response Models

    private class AiResponse
    {
        public string Word { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string? Reaction { get; set; }
    }

    private class PromptAnalysisResponse
    {
        public int Quality { get; set; }
        public string? Feedback { get; set; }
        public List<string>? Strengths { get; set; }
        public List<string>? Weaknesses { get; set; }
    }

    private class SuggestionsResponse
    {
        public List<string>? Suggestions { get; set; }
    }

    private class PromptCoachResponse
    {
        public string? OverallAnalysis { get; set; }
        public string? BestPrompt { get; set; }
        public string? IdealPromptExample { get; set; }
        public List<string>? TipsForNextTime { get; set; }
        public int PromptEngineeringScore { get; set; }
    }

    #endregion
}
