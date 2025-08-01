using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Moq;
using DocumentManagementSystem.Data;
using DocumentManagementSystem.Models;
using DocumentManagementSystem.Services;
using DocumentManagementSystem.ViewModels;
using Xunit;

namespace DocumentManagementSystem.Tests
{
    public class DocumentServiceAdvancedTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;
        private readonly Mock<ILogger<DocumentService>> _mockLogger;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IGeminiService> _mockGeminiService;
        private readonly DocumentService _service;

        public DocumentServiceAdvancedTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockLogger = new Mock<ILogger<DocumentService>>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockGeminiService = new Mock<IGeminiService>();

            _mockEnvironment.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            _service = new DocumentService(_context, _mockEnvironment.Object, _mockLogger.Object,
                _mockServiceProvider.Object, _mockGeminiService.Object);
        }

        [Fact]
        public async Task UploadDocumentAsync_NullFile_ShouldThrowArgumentException()
        {
            // Arrange
            var model = new DocumentUploadViewModel
            {
                Title = "Test Document",
                File = null
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.UploadDocumentAsync(model, "testuser"));
        }

        [Fact]
        public async Task UploadDocumentAsync_UnsupportedFileType_ShouldThrowArgumentException()
        {
            // Arrange
            var model = new DocumentUploadViewModel
            {
                Title = "Test Document",
                File = CreateMockFile("test.xyz", "Test content")
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.UploadDocumentAsync(model, "testuser"));
        }

        [Fact]
        public async Task UploadDocumentAsync_PdfFile_ShouldCreateDocumentWithPdfType()
        {
            // Arrange
            var model = new DocumentUploadViewModel
            {
                Title = "Test PDF",
                File = CreateMockFile("test.pdf", "PDF content")
            };

            // Act
            var result = await _service.UploadDocumentAsync(model, "testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test PDF", result.Title);
            Assert.Equal("pdf", result.FileType);
            Assert.Equal("Processing", result.ProcessingStatus);
        }

        [Fact]
        public async Task UploadDocumentAsync_DocxFile_ShouldCreateDocumentWithDocxType()
        {
            // Arrange
            var model = new DocumentUploadViewModel
            {
                Title = "Test DOCX",
                File = CreateMockFile("test.docx", "DOCX content")
            };

            // Act
            var result = await _service.UploadDocumentAsync(model, "testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test DOCX", result.Title);
            Assert.Equal("docx", result.FileType);
            Assert.Equal("Processing", result.ProcessingStatus);
        }

        [Fact]
        public async Task SearchDocumentsAsync_EmptySearchTerm_ShouldReturnAllDocuments()
        {
            // Arrange
            var documents = new List<Document>
            {
                new Document { Title = "Doc 1", Content = "Content 1", UploadedBy = "user1" },
                new Document { Title = "Doc 2", Content = "Content 2", UploadedBy = "user2" },
                new Document { Title = "Doc 3", Content = "Content 3", UploadedBy = "user3" }
            };
            _context.Documents.AddRange(documents);
            await _context.SaveChangesAsync();

            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = ""
            };

            // Act
            var result = await _service.SearchDocumentsAsync(searchModel);

            // Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task SearchDocumentsAsync_CaseInsensitiveSearch_ShouldFindMatches()
        {
            // Arrange
            var documents = new List<Document>
            {
                new Document { Title = "AI Document", Content = "This is about ARTIFICIAL INTELLIGENCE", UploadedBy = "user1" },
                new Document { Title = "ML Document", Content = "This is about machine learning", UploadedBy = "user2" }
            };
            _context.Documents.AddRange(documents);
            await _context.SaveChangesAsync();

            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = "artificial intelligence"
            };

            // Act
            var result = await _service.SearchDocumentsAsync(searchModel);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, r => r.Title == "AI Document");
        }

        [Fact]
        public async Task SearchDocumentsAsync_PartialWordSearch_ShouldFindMatches()
        {
            // Arrange
            var documents = new List<Document>
            {
                new Document { Title = "AI Document", Content = "This is about artificial intelligence", UploadedBy = "user1" },
                new Document { Title = "ML Document", Content = "This is about machine learning", UploadedBy = "user2" },
                new Document { Title = "DB Document", Content = "This is about database", UploadedBy = "user3" }
            };
            _context.Documents.AddRange(documents);
            await _context.SaveChangesAsync();

            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = "intelligence"
            };

            // Act
            var result = await _service.SearchDocumentsAsync(searchModel);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, r => r.Title == "AI Document");
        }

        [Fact]
        public async Task SearchDocumentsAsync_MultipleWordSearch_ShouldFindMatches()
        {
            // Arrange
            var documents = new List<Document>
            {
                new Document { Title = "AI Document", Content = "This is about artificial intelligence and machine learning", UploadedBy = "user1" },
                new Document { Title = "ML Document", Content = "This is about machine learning", UploadedBy = "user2" },
                new Document { Title = "DB Document", Content = "This is about database", UploadedBy = "user3" }
            };
            _context.Documents.AddRange(documents);
            await _context.SaveChangesAsync();

            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = "artificial intelligence"
            };

            // Act
            var result = await _service.SearchDocumentsAsync(searchModel);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, r => r.Title == "AI Document");
        }

        [Fact]
        public async Task UpdateDocumentAsync_NonExistentDocument_ShouldThrowException()
        {
            // Arrange
            var model = new DocumentViewModel
            {
                Id = 999,
                Title = "New Title"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.UpdateDocumentAsync(999, model));
        }

        [Fact]
        public async Task UpdateDocumentAsync_ValidUpdate_ShouldUpdateDocument()
        {
            // Arrange
            var document = new Document
            {
                Title = "Old Title",
                FileName = "test.txt",
                FilePath = "/uploads/test.txt",
                FileType = "txt",
                UploadedBy = "testuser"
            };
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            var model = new DocumentViewModel
            {
                Id = document.Id,
                Title = "New Title",
                Description = "New Description"
            };

            // Act
            var result = await _service.UpdateDocumentAsync(document.Id, model);

            // Assert
            Assert.Equal("New Title", result.Title);
            Assert.Equal("New Description", result.Description);
            Assert.Equal(document.Id, result.Id);
        }

        [Fact]
        public async Task DeleteDocumentAsync_NonExistentDocument_ShouldReturnFalse()
        {
            // Act
            var result = await _service.DeleteDocumentAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExtractTextFromFileAsync_TxtFile_ShouldReturnContent()
        {
            // Arrange
            var file = CreateMockFile("test.txt", "This is test content for text file");

            // Act
            var result = await _service.ExtractTextFromFileAsync(file);

            // Assert
            Assert.Equal("This is test content for text file", result);
        }

        [Fact]
        public async Task ExtractTextFromFileAsync_EmptyFile_ShouldReturnEmptyString()
        {
            // Arrange
            var file = CreateMockFile("test.txt", "");

            // Act
            var result = await _service.ExtractTextFromFileAsync(file);

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public async Task ExtractTextFromFileAsync_LargeFile_ShouldHandleCorrectly()
        {
            // Arrange
            var largeContent = string.Join("\n", Enumerable.Range(1, 1000).Select(i => $"Line {i}: This is test content"));
            var file = CreateMockFile("test.txt", largeContent);

            // Act
            var result = await _service.ExtractTextFromFileAsync(file);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("Line 1:", result);
            Assert.Contains("Line 1000:", result);
        }

        [Fact]
        public async Task GenerateSummaryAsync_ValidContent_ShouldReturnSummary()
        {
            // Arrange
            var content = "This is a test document. It contains multiple sentences. " +
                         "The summary should extract important information. " +
                         "This is another sentence for testing purposes.";

            _mockGeminiService.Setup(g => g.GenerateSummaryAsync(content))
                .ReturnsAsync("This is a test document. The summary should extract important information.");

            // Act
            var result = await _service.GenerateSummaryAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            _mockGeminiService.Verify(g => g.GenerateSummaryAsync(content), Times.Once);
        }

        [Fact]
        public async Task ExtractKeywordsAsync_ValidContent_ShouldReturnKeywords()
        {
            // Arrange
            var content = "This document discusses artificial intelligence and machine learning technologies.";

            _mockGeminiService.Setup(g => g.ExtractKeywordsAsync(content))
                .ReturnsAsync("artificial, intelligence, machine, learning, technologies");

            // Act
            var result = await _service.ExtractKeywordsAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            _mockGeminiService.Verify(g => g.ExtractKeywordsAsync(content), Times.Once);
        }

        [Fact]
        public async Task ProcessDocumentAsync_ValidDocument_ShouldProcessSuccessfully()
        {
            // Arrange
            var document = new Document
            {
                Title = "Test Document",
                FileName = "test.txt",
                FilePath = "/uploads/test.txt",
                FileType = "txt",
                UploadedBy = "testuser",
                ProcessingStatus = "Pending"
            };
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            // Create a temporary file
            var tempPath = Path.Combine(Path.GetTempPath(), "test.txt");
            await File.WriteAllTextAsync(tempPath, "Test content for processing");

            _mockGeminiService.Setup(g => g.GenerateSummaryAsync(It.IsAny<string>()))
                .ReturnsAsync("Test content for processing.");
            _mockGeminiService.Setup(g => g.ExtractKeywordsAsync(It.IsAny<string>()))
                .ReturnsAsync("test, content, processing");

            // Act
            var result = await _service.ProcessDocumentAsync(document.Id);

            // Assert
            Assert.True(result);

            var updatedDocument = await _context.Documents.FindAsync(document.Id);
            Assert.Equal("Completed", updatedDocument.ProcessingStatus);
            Assert.True(updatedDocument.IsProcessed);
            Assert.NotEmpty(updatedDocument.Content);
            Assert.NotEmpty(updatedDocument.Summary);
            Assert.NotEmpty(updatedDocument.Keywords);

            // Cleanup
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }

        [Fact]
        public async Task ProcessDocumentAsync_NonExistentDocument_ShouldReturnFalse()
        {
            // Act
            var result = await _service.ProcessDocumentAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ProcessDocumentAsync_AlreadyProcessedDocument_ShouldReturnTrue()
        {
            // Arrange
            var document = new Document
            {
                Title = "Test Document",
                FileName = "test.txt",
                FilePath = "/uploads/test.txt",
                FileType = "txt",
                UploadedBy = "testuser",
                ProcessingStatus = "Completed",
                IsProcessed = true
            };
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ProcessDocumentAsync(document.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetAllDocumentsAsync_EmptyDatabase_ShouldReturnEmptyList()
        {
            // Act
            var result = await _service.GetAllDocumentsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllDocumentsAsync_MultipleDocuments_ShouldReturnOrderedList()
        {
            // Arrange
            var documents = new List<Document>
            {
                new Document { Title = "Doc 1", UploadDate = DateTime.Now.AddDays(-2), UploadedBy = "user1" },
                new Document { Title = "Doc 2", UploadDate = DateTime.Now.AddDays(-1), UploadedBy = "user2" },
                new Document { Title = "Doc 3", UploadDate = DateTime.Now, UploadedBy = "user3" }
            };
            _context.Documents.AddRange(documents);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllDocumentsAsync();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("Doc 3", result[0].Title); // Most recent first
            Assert.Equal("Doc 2", result[1].Title);
            Assert.Equal("Doc 1", result[2].Title);
        }

        private IFormFile CreateMockFile(string fileName, string content)
        {
            var mockFile = new Mock<IFormFile>();
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(content.Length);
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);

            return mockFile.Object;
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}