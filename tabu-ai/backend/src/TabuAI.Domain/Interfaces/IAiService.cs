namespace TabuAI.Domain.Interfaces;

public interface IAiService
{
    Task<AiGuessResult> GuessWordAsync(string prompt, string targetWord, List<string> tabuWords);
    Task<PromptAnalysisResult> AnalyzePromptAsync(string prompt, string targetWord, List<string> tabuWords);
    Task<List<string>> GenerateImprovementSuggestionsAsync(string prompt, string targetWord, List<string> tabuWords, bool wasCorrect);
}

public class AiGuessResult
{
    public string GuessedWord { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public double Confidence { get; set; }
    public string Reasoning { get; set; } = string.Empty;
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