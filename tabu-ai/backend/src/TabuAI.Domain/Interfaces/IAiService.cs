namespace TabuAI.Domain.Interfaces;

public interface IAiService
{
    Task<AiGuessResult> GuessWordAsync(string prompt, string targetWord, List<string> tabuWords, string? persona = null);
    Task<AiGuessResult> GuessWordWithModelAsync(string prompt, string targetWord, List<string> tabuWords, string? persona = null, string? modelOverride = null);
    Task<PromptAnalysisResult> AnalyzePromptAsync(string prompt, string targetWord, List<string> tabuWords);
    Task<List<string>> GenerateImprovementSuggestionsAsync(string prompt, string targetWord, List<string> tabuWords, bool wasCorrect);
    Task<PromptCoachResult> GeneratePromptCoachAnalysisAsync(string targetWord, List<string> tabuWords, List<PromptAttemptInfo> attempts);
}

public enum AiPersona
{
    Default,
    Sarcastic,
    Childish,
    Meticulous,
    Dramatic,
    Philosopher
}

public class PromptAttemptInfo
{
    public string UserPrompt { get; set; } = string.Empty;
    public string AiGuess { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int PromptQuality { get; set; }
}

public class PromptCoachResult
{
    public string OverallAnalysis { get; set; } = string.Empty;
    public string BestPrompt { get; set; } = string.Empty;
    public string IdealPromptExample { get; set; } = string.Empty;
    public List<string> TipsForNextTime { get; set; } = new();
    public int PromptEngineeringScore { get; set; } // 1-10
}

public class AiGuessResult
{
    public string GuessedWord { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public double Confidence { get; set; }
    public string Reasoning { get; set; } = string.Empty;
    public string Reaction { get; set; } = string.Empty;
}

public class PromptAnalysisResult
{
    public int PromptQuality { get; set; } // 1-5 scale
    public bool ContainsTabuWords { get; set; }
    public List<string> DetectedTabuWords { get; set; } = new();
    public string Feedback { get; set; } = string.Empty;
    public List<string> Strengths { get; set; } = new();
    public List<string> Weaknesses { get; set; } = new();
}