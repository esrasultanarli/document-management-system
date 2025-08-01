using DocumentManagementSystem.Data;
using DocumentManagementSystem.Models;
using DocumentManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using Document = DocumentManagementSystem.Models.Document;

namespace DocumentManagementSystem.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<DocumentService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IGeminiService _geminiService;

        public DocumentService(ApplicationDbContext context, IWebHostEnvironment environment, ILogger<DocumentService> logger, IServiceProvider serviceProvider, IGeminiService geminiService)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _geminiService = geminiService;
        }

        public async Task<Document> UploadDocumentAsync(DocumentUploadViewModel model, string uploadedBy)
        {
            if (model.File == null)
                throw new ArgumentException("Dosya gereklidir");

            // Dosya uzantısını kontrol et
            var allowedExtensions = new[] { ".pdf", ".txt", ".docx" };
            var fileExtension = System.IO.Path.GetExtension(model.File.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Sadece PDF, TXT ve DOCX dosyaları desteklenir");

            // Dosya adını güvenli hale getir
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var uploadsPath = System.IO.Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var filePath = System.IO.Path.Combine(uploadsPath, fileName);

            // Dosyayı kaydet
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await model.File.CopyToAsync(stream);
            }

            // Dokümanı veritabanına kaydet
            var document = new Document
            {
                Title = model.Title,
                FileName = model.File.FileName,
                FilePath = $"/uploads/{fileName}",
                FileType = fileExtension.TrimStart('.'),
                FileSize = model.File.Length,
                UploadedBy = uploadedBy,
                UploadDate = DateTime.Now,
                LastModified = DateTime.Now,
                ProcessingStatus = "Processing"
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            // Arka planda işleme başlat
            _ = Task.Run(async () => await ProcessDocumentAsync(document.Id));

            return document;
        }

        public async Task<Document?> GetDocumentByIdAsync(int id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task<List<Document>> GetAllDocumentsAsync()
        {
            return await _context.Documents
                .OrderByDescending(d => d.UploadDate)
                .ToListAsync();
        }

        public async Task<List<SearchResult>> SearchDocumentsAsync(DocumentSearchViewModel model)
        {
            var query = _context.Documents.AsQueryable();

            // Arama terimini içeren dokümanları filtrele
            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                var searchTerm = model.SearchTerm.ToLowerInvariant();
                query = query.Where(d =>
                    d.Title.ToLower().Contains(searchTerm) ||
                    d.Content.ToLower().Contains(searchTerm) ||
                    d.Summary.ToLower().Contains(searchTerm) ||
                    d.Keywords.ToLower().Contains(searchTerm)
                );
            }

            // Dosya tipine göre filtrele
            if (!string.IsNullOrEmpty(model.FileType))
            {
                query = query.Where(d => d.FileType == model.FileType);
            }

            // Tarih aralığına göre filtrele
            if (model.FromDate.HasValue)
            {
                query = query.Where(d => d.UploadDate >= model.FromDate.Value);
            }

            if (model.ToDate.HasValue)
            {
                query = query.Where(d => d.UploadDate <= model.ToDate.Value);
            }

            // Yükleyene göre filtrele
            if (!string.IsNullOrEmpty(model.UploadedBy))
            {
                query = query.Where(d => d.UploadedBy == model.UploadedBy);
            }

            var documents = await query
                .OrderByDescending(d => d.UploadDate)
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            var results = new List<SearchResult>();

            foreach (var doc in documents)
            {
                var relevanceScore = CalculateRelevanceScore(doc, model.SearchTerm);
                var matchedTerms = FindMatchedTerms(doc, model.SearchTerm);
                var highlightedContent = HighlightSearchTerms(doc.Content, model.SearchTerm);

                results.Add(new SearchResult
                {
                    DocumentId = doc.Id,
                    Title = doc.Title,
                    FileName = doc.FileName,
                    FileType = doc.FileType,
                    Summary = doc.Summary,
                    Keywords = doc.Keywords,
                    UploadDate = doc.UploadDate,
                    UploadedBy = doc.UploadedBy,
                    RelevanceScore = relevanceScore,
                    MatchedTerms = matchedTerms,
                    HighlightedContent = highlightedContent
                });
            }

            return results.OrderByDescending(r => r.RelevanceScore).ToList();
        }

        public async Task<Document> UpdateDocumentAsync(int id, DocumentViewModel model)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
                throw new ArgumentException("Doküman bulunamadı");

            document.Title = model.Title;
            document.LastModified = DateTime.Now;

            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<bool> DeleteDocumentAsync(int id, string currentUser)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
                return false;

            // Kullanıcının sadece kendi yüklediği dosyaları silebilmesini kontrol et
            if (document.UploadedBy != currentUser)
            {
                throw new UnauthorizedAccessException("Bu dosyayı silme yetkiniz bulunmamaktadır. Sadece kendi yüklediğiniz dosyaları silebilirsiniz.");
            }

            // Fiziksel dosyayı sil
            var filePath = System.IO.Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> ExtractTextFromFileAsync(IFormFile file)
        {
            var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

            switch (fileExtension)
            {
                case ".txt":
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        return await reader.ReadToEndAsync();
                    }

                case ".pdf":
                    return await ExtractTextFromPdfAsync(file);

                case ".docx":
                    return await ExtractTextFromDocxAsync(file);

                default:
                    throw new ArgumentException("Desteklenmeyen dosya formatı");
            }
        }

        private async Task<string> ExtractTextFromPdfAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var pdfReader = new PdfReader(stream);
            var text = new StringBuilder();

            for (int i = 1; i <= pdfReader.NumberOfPages; i++)
            {
                text.Append(PdfTextExtractor.GetTextFromPage(pdfReader, i));
            }

            return text.ToString();
        }

        private async Task<string> ExtractTextFromDocxAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var document = WordprocessingDocument.Open(stream, false);
            var body = document.MainDocumentPart?.Document.Body;

            if (body == null)
                return string.Empty;

            return body.InnerText;
        }

        public async Task<string> GenerateSummaryAsync(string content)
        {
            return await _geminiService.GenerateSummaryAsync(content);
        }

        public async Task<string> ExtractKeywordsAsync(string content)
        {
            return await _geminiService.ExtractKeywordsAsync(content);
        }

        public async Task<bool> ProcessDocumentAsync(int documentId)
        {
            try
            {
                // Create a new DbContext for background processing
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var document = await context.Documents.FindAsync(documentId);
                if (document == null)
                    return false;

                document.ProcessingStatus = "Processing";
                await context.SaveChangesAsync();

                // Dosyadan metin çıkar
                var filePath = System.IO.Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('/'));
                if (!File.Exists(filePath))
                {
                    document.ProcessingStatus = "Failed";
                    await context.SaveChangesAsync();
                    return false;
                }

                using var fileStream = File.OpenRead(filePath);
                var file = new FormFile(fileStream, 0, fileStream.Length, document.FileName, document.FileName);
                document.Content = await ExtractTextFromFileAsync(file);

                // Özet ve anahtar kelimeler oluştur
                document.Summary = await _geminiService.GenerateSummaryAsync(document.Content);
                document.Keywords = await _geminiService.ExtractKeywordsAsync(document.Content);

                document.IsProcessed = true;
                document.ProcessingStatus = "Completed";
                document.LastModified = DateTime.Now;

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Doküman işleme hatası: {DocumentId}", documentId);

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var document = await context.Documents.FindAsync(documentId);
                    if (document != null)
                    {
                        document.ProcessingStatus = "Failed";
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception updateEx)
                {
                    _logger.LogError(updateEx, "Doküman durumu güncellenirken hata: {DocumentId}", documentId);
                }

                return false;
            }
        }

        private double CalculateRelevanceScore(Document document, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return 0;

            var term = searchTerm.ToLowerInvariant();
            var score = 0.0;

            // Başlıkta eşleşme
            if (document.Title.ToLowerInvariant().Contains(term))
                score += 10;

            // İçerikte eşleşme
            if (document.Content.ToLowerInvariant().Contains(term))
                score += 5;

            // Özette eşleşme
            if (document.Summary.ToLowerInvariant().Contains(term))
                score += 8;

            // Anahtar kelimelerde eşleşme
            if (document.Keywords.ToLowerInvariant().Contains(term))
                score += 12;

            return score;
        }

        private List<string> FindMatchedTerms(Document document, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return new List<string>();

            var terms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var matchedTerms = new List<string>();

            foreach (var term in terms)
            {
                var lowerTerm = term.ToLowerInvariant();
                if (document.Title.ToLowerInvariant().Contains(lowerTerm) ||
                    document.Content.ToLowerInvariant().Contains(lowerTerm) ||
                    document.Summary.ToLowerInvariant().Contains(lowerTerm) ||
                    document.Keywords.ToLowerInvariant().Contains(lowerTerm))
                {
                    matchedTerms.Add(term);
                }
            }

            return matchedTerms.Distinct().ToList();
        }

        private string HighlightSearchTerms(string content, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm) || string.IsNullOrEmpty(content))
                return content;

            var terms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var highlightedContent = content;

            foreach (var term in terms)
            {
                var pattern = $@"\b{Regex.Escape(term)}\b";
                highlightedContent = Regex.Replace(highlightedContent, pattern, $"<mark>{term}</mark>", RegexOptions.IgnoreCase);
            }

            return highlightedContent;
        }
    }
}