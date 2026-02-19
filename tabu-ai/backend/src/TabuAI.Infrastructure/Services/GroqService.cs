using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
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

    public GroqService(IConfiguration configuration, ILogger<GroqService> logger)
    {
        var apiKey = configuration["Groq:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "Groq API key is not configured");
        var baseUrl = configuration["Groq:BaseUrl"] ?? "https://api.groq.com/openai/v1";
        _model = configuration["Groq:Model"] ?? "llama-3.1-8b-instant";

        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri(baseUrl)
        };

        _groqClient = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        _logger = logger;
    }

    public async Task<AiGuessResult> GuessWordAsync(string prompt, string targetWord, List<string> tabuWords)
    {
        try
        {
            var systemMessage = $@"Sen bir TABU oyunu asistanısın. Kullanıcı sana bir tanımlama gönderecek ve sen bu tanımlamaya göre kelimeyi tahmin etmelisin.
                
Kurallar:
1. Sadece TEK KELİME ile cevap ver
2. Cevabını JSON formatında ver: {{""word"": ""tahmin_ettiğin_kelime"", ""confidence"": 0.95}}
3. Confidence 0.0 ile 1.0 arasında olmalı
4. Türkçe kelimelerle cevap ver
5. En muhtemel kelimeyi tahmin et

Tabu kelimeler (bunlar ipucu değil, sadece bilgi amaçlı): {string.Join(", ", tabuWords)}";

            var chatClient = _groqClient.GetChatClient(_model);
            var response = await chatClient.CompleteChatAsync(
                new ChatMessage[]
                {
                    new SystemChatMessage(systemMessage),
                    new UserChatMessage(prompt)
                });

            var content = response.Value.Content[0].Text;
            _logger.LogInformation("AI Response: {Response}", content);

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
                    var isCorrect = string.Equals(jsonResponse.Word.Trim(), targetWord.Trim(), StringComparison.OrdinalIgnoreCase);
                    
                    return new AiGuessResult
                    {
                        GuessedWord = jsonResponse.Word,
                        IsCorrect = isCorrect,
                        Confidence = jsonResponse.Confidence,
                        Reasoning = $"AI tahmini: {jsonResponse.Word} (Güven: {jsonResponse.Confidence:P0})"
                    };
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse AI guess JSON response. Using fallback.");
                // Fallback logic
                var cleanedResponse = content.Trim().Trim('"');
                if (cleanedResponse.Length > 50) cleanedResponse = cleanedResponse[..50]; // Prevention for DB limits
                
                return new AiGuessResult
                {
                    GuessedWord = cleanedResponse,
                    IsCorrect = false,
                    Confidence = 0.1,
                    Reasoning = "AI anlamsız giriş tespit etti veya format hatası oluştu"
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
            _logger.LogError(ex, "Error occurred while getting AI guess");
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
            // Check for tabu words first
            var detectedTabuWords = new List<string>();
            var promptLower = prompt.ToLowerInvariant();
            
            foreach (var tabuWord in tabuWords)
            {
                if (promptLower.Contains(tabuWord.ToLowerInvariant()))
                {
                    detectedTabuWords.Add(tabuWord);
                }
            }

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

            var systemMessage = $@"Sen bir prompt kalitesi değerlendirme uzmanısın. Kullanıcının verdiği tanımlamayı analiz et.

Hedef kelime: {targetWord}
Tabu kelimeler: {string.Join(", ", tabuWords)}

Değerlendirme kriterleri:
1. Açıklık ve netlik
2. Yaratıcılık
3. Tabu kelimelerden kaçınma
4. Hedef kelimeye yönlendirme

Cevabını JSON formatında ver:
{{
    ""quality"": 1-5, // 1: Çok kötü, 5: Mükemmel
    ""feedback"": ""Genel değerlendirme"",
    ""strengths"": [""Güçlü yön 1"", ""Güçlü yön 2""],
    ""weaknesses"": [""Zayıf yön 1"", ""Zayıf yön 2""]
}}";

            var chatClient = _groqClient.GetChatClient(_model);
            var response = await chatClient.CompleteChatAsync(
                new ChatMessage[]
                {
                    new SystemChatMessage(systemMessage),
                    new UserChatMessage($"Analiz edilecek prompt: {prompt}")
                });

            var content = response.Value.Content[0].Text;
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
            var systemMessage = $@"Sen bir prompt iyileştirme danışmanısın. Kullanıcının promptunu analiz et ve iyileştirme önerileri ver.

Hedef kelime: {targetWord}
Tabu kelimeler: {string.Join(", ", tabuWords)}
Tahmin doğru muydu: {(wasCorrect ? "Evet" : "Hayır")}

3-5 adet pratik iyileştirme önerisi ver. Öneriler kısa ve uygulanabilir olmalı.

Cevabını JSON formatında ver:
{{
    ""suggestions"": [""Öneri 1"", ""Öneri 2"", ""Öneri 3""]
}}";

            var chatClient = _groqClient.GetChatClient(_model);
            var response = await chatClient.CompleteChatAsync(
                new ChatMessage[]
                {
                    new SystemChatMessage(systemMessage),
                    new UserChatMessage($"İyileştirilecek prompt: {prompt}")
                });

            var content = response.Value.Content[0].Text;
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

    private string ExtractJson(string content)
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

    private class AiResponse
    {
        public string Word { get; set; } = string.Empty;
        public double Confidence { get; set; }
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
}