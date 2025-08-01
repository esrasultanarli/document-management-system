using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Moq;
using DocumentManagementSystem.Data;
using DocumentManagementSystem.Models;
using DocumentManagementSystem.Services;
using DocumentManagementSystem.ViewModels;
using Xunit;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Tests
{
    public class DocumentServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;
        private readonly Mock<ILogger<DocumentService>> _mockLogger;
        private readonly DocumentService _service;

        public DocumentServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockLogger = new Mock<ILogger<DocumentService>>();

            _mockEnvironment.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockGeminiService = new Mock<IGeminiService>();
            _service = new DocumentService(_context, _mockEnvironment.Object, _mockLogger.Object, mockServiceProvider.Object, mockGeminiService.Object);
        }

        [Fact]
        public async Task UploadDocumentAsync_ValidDocument_ShouldCreateDocument()
        {
            // Arrange
            var model = new DocumentUploadViewModel
            {
                Title = "Test Document",
                File = CreateMockFile("test.txt", "Test content")
            };

            // Act
            var result = await _service.UploadDocumentAsync(model, "testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Document", result.Title);
            Assert.Equal("testuser", result.UploadedBy);
            Assert.Equal("txt", result.FileType);
            Assert.Equal("Pending", result.ProcessingStatus);
        }

        [Fact]
        public async Task UploadDocumentAsync_InvalidFileType_ShouldThrowException()
        {
            // Arrange
            var model = new DocumentUploadViewModel
            {
                Title = "Test Document",
                File = CreateMockFile("test.exe", "Test content")
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.UploadDocumentAsync(model, "testuser"));
        }

        [Fact]
        public async Task GetDocumentByIdAsync_ExistingDocument_ShouldReturnDocument()
        {
            // Arrange
            var document = new Document
            {
                Title = "Test Document",
                FileName = "test.txt",
                FilePath = "/uploads/test.txt",
                FileType = "txt",
                UploadedBy = "testuser"
            };
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetDocumentByIdAsync(document.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Document", result.Title);
        }

        [Fact]
        public async Task GetDocumentByIdAsync_NonExistingDocument_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetDocumentByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllDocumentsAsync_ShouldReturnAllDocuments()
        {
            // Arrange
            var documents = new List<Document>
            {
                new Document { Title = "Doc 1", FileName = "doc1.txt", FilePath = "/uploads/doc1.txt", FileType = "txt", UploadedBy = "user1" },
                new Document { Title = "Doc 2", FileName = "doc2.pdf", FilePath = "/uploads/doc2.pdf", FileType = "pdf", UploadedBy = "user2" }
            };
            _context.Documents.AddRange(documents);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllDocumentsAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SearchDocumentsAsync_WithSearchTerm_ShouldReturnMatchingDocuments()
        {
            // Arrange
            var documents = new List<Document>
            {
                new Document { Title = "AI Document", FileName = "ai.txt", FilePath = "/uploads/ai.txt", FileType = "txt", UploadedBy = "user1", Content = "This is about artificial intelligence" },
                new Document { Title = "ML Document", FileName = "ml.txt", FilePath = "/uploads/ml.txt", FileType = "txt", UploadedBy = "user2", Content = "This is about machine learning" },
                new Document { Title = "Database Document", FileName = "db.txt", FilePath = "/uploads/db.txt", FileType = "txt", UploadedBy = "user3", Content = "This is about databases" }
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
        public async Task UpdateDocumentAsync_ExistingDocument_ShouldUpdateTitle()
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
                Title = "New Title"
            };

            // Act
            var result = await _service.UpdateDocumentAsync(document.Id, model);

            // Assert
            Assert.Equal("New Title", result.Title);
        }

        [Fact]
        public async Task DeleteDocumentAsync_ExistingDocument_ShouldReturnTrue()
        {
            // Arrange
            var document = new Document
            {
                Title = "Test Document",
                FileName = "test.txt",
                FilePath = "/uploads/test.txt",
                FileType = "txt",
                UploadedBy = "testuser"
            };
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteDocumentAsync(document.Id, "testuser");

            // Assert
            Assert.True(result);
            Assert.Empty(_context.Documents);
        }

        [Fact]
        public async Task DeleteDocumentAsync_UnauthorizedUser_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var document = new Document
            {
                Title = "Test Document",
                FileName = "test.txt",
                FilePath = "/uploads/test.txt",
                FileType = "txt",
                UploadedBy = "user1"
            };
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.DeleteDocumentAsync(document.Id, "user2"));
        }

        [Fact]
        public async Task DeleteDocumentAsync_NonExistingDocument_ShouldReturnFalse()
        {
            // Act
            var result = await _service.DeleteDocumentAsync(999, "testuser");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExtractTextFromFileAsync_TxtFile_ShouldReturnContent()
        {
            // Arrange
            var file = CreateMockFile("test.txt", "This is test content");

            // Act
            var result = await _service.ExtractTextFromFileAsync(file);

            // Assert
            Assert.Equal("This is test content", result);
        }

        [Fact]
        public async Task GenerateSummaryAsync_ValidContent_ShouldReturnSummary()
        {
            // Arrange
            var content = "This is a test document. It contains multiple sentences. The summary should extract important information.";

            // Act
            var result = await _service.GenerateSummaryAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Length < content.Length);
        }

        [Fact]
        public async Task ExtractKeywordsAsync_ValidContent_ShouldReturnKeywords()
        {
            // Arrange
            var content = "This document discusses artificial intelligence and machine learning technologies.";

            // Act
            var result = await _service.ExtractKeywordsAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("artificial", result);
            Assert.Contains("intelligence", result);
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
            await File.WriteAllTextAsync(tempPath, "Test content");

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