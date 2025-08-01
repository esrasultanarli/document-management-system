namespace DocumentManagementSystem.Services
{
    public interface IGeminiService
    {
        Task<string> GenerateSummaryAsync(string content);
        Task<string> ExtractKeywordsAsync(string content);
    }
} 