using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using DocumentManagementSystem.Controllers;
using DocumentManagementSystem.Data;
using DocumentManagementSystem.Models;
using DocumentManagementSystem.Services;
using DocumentManagementSystem.ViewModels;
using Xunit;

namespace DocumentManagementSystem.Tests
{
    public class DocumentRetrievalTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IDocumentService> _mockDocumentService;
        private readonly Mock<ILogger<DocumentController>> _mockLogger;
        private readonly DocumentController _controller;

        public DocumentRetrievalTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockDocumentService = new Mock<IDocumentService>();
            _mockLogger = new Mock<ILogger<DocumentController>>();
            _controller = new DocumentController(_mockDocumentService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Test_GetDocument_WithValidId_ShouldReturnFile()
        {
            // Arrange
            var documentId = 1;
            var document = new Document
            {
                Id = documentId,
                Title = "Test Document",
                FileName = "test.pdf",
                FilePath = "/uploads/test.pdf",
                FileType = "pdf",
                FileSize = 1024,
                Content = "This is test content",
                UploadedBy = "testuser",
                UploadDate = DateTime.Now,
                IsProcessed = true,
                ProcessingStatus = "Completed"
            };

            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            // Act
            var result = await _controller.Details(documentId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Document>(viewResult.Model);

            Assert.Equal(documentId, model.Id);
            Assert.Equal("Test Document", model.Title);
            Assert.Equal("test.pdf", model.FileName);
            Assert.Equal("pdf", model.FileType);
            Assert.Equal(1024, model.FileSize);
            Assert.Equal("This is test content", model.Content);
            Assert.Equal("testuser", model.UploadedBy);
            Assert.True(model.IsProcessed);
            Assert.Equal("Completed", model.ProcessingStatus);
        }

        [Fact]
        public async Task Test_GetDocument_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var invalidDocumentId = 999;
            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(invalidDocumentId))
                .ReturnsAsync((Document?)null);

            // Act
            var result = await _controller.Details(invalidDocumentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Test_GetDocument_WithZeroId_ShouldReturnNotFound()
        {
            // Arrange
            var zeroDocumentId = 0;
            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(zeroDocumentId))
                .ReturnsAsync((Document?)null);

            // Act
            var result = await _controller.Details(zeroDocumentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Test_GetDocument_WithNegativeId_ShouldReturnNotFound()
        {
            // Arrange
            var negativeDocumentId = -1;
            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(negativeDocumentId))
                .ReturnsAsync((Document?)null);

            // Act
            var result = await _controller.Details(negativeDocumentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Test_GetDocument_WithValidId_ShouldReturnCorrectFileType()
        {
            // Arrange
            var documentId = 1;
            var document = new Document
            {
                Id = documentId,
                Title = "PDF Document",
                FileName = "document.pdf",
                FilePath = "/uploads/document.pdf",
                FileType = "pdf",
                FileSize = 2048,
                UploadedBy = "testuser"
            };

            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            // Act
            var result = await _controller.Details(documentId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Document>(viewResult.Model);

            Assert.Equal("pdf", model.FileType);
            Assert.Equal("document.pdf", model.FileName);
            Assert.Equal("/uploads/document.pdf", model.FilePath);
        }

        [Fact]
        public async Task Test_GetDocument_WithValidId_ShouldReturnCorrectFileSize()
        {
            // Arrange
            var documentId = 1;
            var document = new Document
            {
                Id = documentId,
                Title = "Large Document",
                FileName = "large.pdf",
                FilePath = "/uploads/large.pdf",
                FileType = "pdf",
                FileSize = 1048576, // 1MB
                UploadedBy = "testuser"
            };

            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            // Act
            var result = await _controller.Details(documentId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Document>(viewResult.Model);

            Assert.Equal(1048576, model.FileSize);
        }

        [Fact]
        public async Task Test_GetDocument_WithValidId_ShouldReturnCorrectUploadInfo()
        {
            // Arrange
            var documentId = 1;
            var uploadDate = DateTime.Now.AddDays(-5);
            var document = new Document
            {
                Id = documentId,
                Title = "Test Document",
                FileName = "test.txt",
                FilePath = "/uploads/test.txt",
                FileType = "txt",
                FileSize = 512,
                UploadedBy = "admin",
                UploadDate = uploadDate,
                LastModified = uploadDate
            };

            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            // Act
            var result = await _controller.Details(documentId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Document>(viewResult.Model);

            Assert.Equal("admin", model.UploadedBy);
            Assert.Equal(uploadDate.Date, model.UploadDate.Date);
            Assert.Equal(uploadDate.Date, model.LastModified.Date);
        }

        [Fact]
        public async Task Test_GetDocument_WithValidId_ShouldReturnProcessingStatus()
        {
            // Arrange
            var documentId = 1;
            var document = new Document
            {
                Id = documentId,
                Title = "Processing Document",
                FileName = "processing.txt",
                FilePath = "/uploads/processing.txt",
                FileType = "txt",
                FileSize = 256,
                UploadedBy = "testuser",
                ProcessingStatus = "Processing",
                IsProcessed = false
            };

            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            // Act
            var result = await _controller.Details(documentId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Document>(viewResult.Model);

            Assert.Equal("Processing", model.ProcessingStatus);
            Assert.False(model.IsProcessed);
        }

        [Fact]
        public async Task Test_GetDocument_WithValidId_ShouldReturnCompletedStatus()
        {
            // Arrange
            var documentId = 1;
            var document = new Document
            {
                Id = documentId,
                Title = "Completed Document",
                FileName = "completed.txt",
                FilePath = "/uploads/completed.txt",
                FileType = "txt",
                FileSize = 256,
                UploadedBy = "testuser",
                ProcessingStatus = "Completed",
                IsProcessed = true,
                Content = "Processed content",
                Summary = "Document summary",
                Keywords = "keyword1, keyword2"
            };

            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            // Act
            var result = await _controller.Details(documentId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Document>(viewResult.Model);

            Assert.Equal("Completed", model.ProcessingStatus);
            Assert.True(model.IsProcessed);
            Assert.Equal("Processed content", model.Content);
            Assert.Equal("Document summary", model.Summary);
            Assert.Equal("keyword1, keyword2", model.Keywords);
        }

        [Fact]
        public async Task Test_GetDocument_WithValidId_ShouldReturnErrorStatus()
        {
            // Arrange
            var documentId = 1;
            var document = new Document
            {
                Id = documentId,
                Title = "Error Document",
                FileName = "error.txt",
                FilePath = "/uploads/error.txt",
                FileType = "txt",
                FileSize = 256,
                UploadedBy = "testuser",
                ProcessingStatus = "Error",
                IsProcessed = false
            };

            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            // Act
            var result = await _controller.Details(documentId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Document>(viewResult.Model);

            Assert.Equal("Error", model.ProcessingStatus);
            Assert.False(model.IsProcessed);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}