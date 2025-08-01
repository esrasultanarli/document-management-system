namespace DocumentManagementSystem.Models
{
    public class SearchResult
    {
        public int DocumentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public double RelevanceScore { get; set; }
        public List<string> MatchedTerms { get; set; } = new List<string>();
        public string HighlightedContent { get; set; } = string.Empty;
    }
}