using DocumentManagementSystem.Models;
using DocumentManagementSystem.ViewModels;

namespace DocumentManagementSystem.Services
{
    public interface IDocumentService
    {
        Task<Document> UploadDocumentAsync(DocumentUploadViewModel model, string uploadedBy);
        Task<Document?> GetDocumentByIdAsync(int id);
        Task<List<Document>> GetAllDocumentsAsync();
        Task<List<SearchResult>> SearchDocumentsAsync(DocumentSearchViewModel model);
        Task<Document> UpdateDocumentAsync(int id, DocumentViewModel model);
        Task<bool> DeleteDocumentAsync(int id, string currentUser);
        Task<string> ExtractTextFromFileAsync(IFormFile file);
        Task<string> GenerateSummaryAsync(string content);
        Task<string> ExtractKeywordsAsync(string content);
        Task<bool> ProcessDocumentAsync(int documentId);
    }
}