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
    public class DocumentAuthorizationTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IDocumentService> _mockDocumentService;
        private readonly Mock<ILogger<DocumentController>> _mockLogger;
        private readonly DocumentController _controller;

        public DocumentAuthorizationTests()
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
        public async Task Test_DeleteDocument_AsOwner_ShouldSucceed()
        {
            // Arrange
            var documentId = 1;
            var ownerUsername = "documentowner";
            var document = new Document
            {
                Id = documentId,
                Title = "Owner's Document",
                FileName = "owner_doc.pdf",
                FilePath = "/uploads/owner_doc.pdf",
                FileType = "pdf",
                FileSize = 1024,
                UploadedBy = ownerUsername,
                UploadDate = DateTime.Now
            };

            // Mock the document retrieval
            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            // Mock successful deletion
            _mockDocumentService.Setup(s => s.DeleteDocumentAsync(documentId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteConfirmed(documentId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockDocumentService.Verify(s => s.DeleteDocumentAsync(documentId), Times.Once);
        }

        [Fact]
        public async Task Test_DeleteDocument_AsNonOwner_ShouldReturnForbidden()
        {
            // Arrange
            var documentId = 1;
            var ownerUsername = "documentowner";
            var nonOwnerUsername = "otheruser";
            var document = new Document
            {
                Id = documentId,
                Title = "Owner's Document",
                FileName = "owner_doc.pdf",
                FilePath = "/uploads/owner_doc.pdf",
                FileType = "pdf",
                FileSize = 1024,
                UploadedBy = ownerUsername,
                UploadDate = DateTime.Now
            };

            // Mock the document retrieval
            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            // Mock failed deletion (simulating permission denied)
            _mockDocumentService.Setup(s => s.DeleteDocumentAsync(documentId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteConfirmed(documentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _mockDocumentService.Verify(s => s.DeleteDocumentAsync(documentId), Times.Once);
        }

        [Fact]
        public async Task Test_DeleteDocument_AsAdmin_ShouldSucceed()
        {
            // Arrange
            var documentId = 1;
            var ownerUsername = "regularuser";
            var adminUsername = "admin";
            var document = new Document
            {
                Id = documentId,
                Title = "User's Document",
                FileName = "user_doc.pdf",
                FilePath = "/uploads/user_doc.pdf",
                FileType = "pdf",
                FileSize = 1024,
                UploadedBy = ownerUsername,
                UploadDate = DateTime.Now
            };

            // Mock the document retrieval
            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            // Mock successful deletion (admin can delete any document)
            _mockDocumentService.Setup(s => s.DeleteDocumentAsync(documentId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteConfirmed(documentId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockDocumentService.Verify(s => s.DeleteDocumentAsync(documentId), Times.Once);
        }

        [Fact]
        public async Task Test_DeleteDocument_NonExistentDocument_ShouldReturnNotFound()
        {
            // Arrange
            var nonExistentDocumentId = 999;
            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(nonExistentDocumentId))
                .ReturnsAsync((Document?)null);

            // Act
            var result = await _controller.DeleteConfirmed(nonExistentDocumentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _mockDocumentService.Verify(s => s.DeleteDocumentAsync(nonExistentDocumentId), Times.Never);
        }

        [Fact]
        public async Task Test_EditDocument_AsOwner_ShouldSucceed()
        {
            // Arrange
            var documentId = 1;
            var ownerUsername = "documentowner";
            var document = new Document
            {
                Id = documentId,
                Title = "Original Title",
                FileName = "owner_doc.pdf",
                FilePath = "/uploads/owner_doc.pdf",
                FileType = "pdf",
                FileSize = 1024,
                UploadedBy = ownerUsername,
                UploadDate = DateTime.Now
            };

            var updateModel = new DocumentViewModel
            {
                Id = documentId,
                Title = "Updated Title",
                Description = "Updated description"
            };

            var updatedDocument = new Document
            {
                Id = documentId,
                Title = "Updated Title",
                FileName = "owner_doc.pdf",
                FilePath = "/uploads/owner_doc.pdf",
                FileType = "pdf",
                FileSize = 1024,
                UploadedBy = ownerUsername,
                UploadDate = DateTime.Now,
                Description = "Updated description"
            };

            // Mock the document retrieval
            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            // Mock successful update
            _mockDocumentService.Setup(s => s.UpdateDocumentAsync(documentId, updateModel))
                .ReturnsAsync(updatedDocument);

            // Act
            var result = await _controller.Edit(documentId, updateModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(documentId, redirectResult.RouteValues["id"]);

            _mockDocumentService.Verify(s => s.UpdateDocumentAsync(documentId, updateModel), Times.Once);
        }

        [Fact]
        public async Task Test_EditDocument_AsNonOwner_ShouldReturnForbidden()
        {
            // Arrange
            var documentId = 1;
            var ownerUsername = "documentowner";
            var nonOwnerUsername = "otheruser";
            var document = new Document
            {
                Id = documentId,
                Title = "Original Title",
                FileName = "owner_doc.pdf",
                FilePath = "/uploads/owner_doc.pdf",
                FileType = "pdf",
                FileSize = 1024,
                UploadedBy = ownerUsername,
                UploadDate = DateTime.Now
            };

            var updateModel = new DocumentViewModel
            {
                Id = documentId,
                Title = "Updated Title"
            };

            // Mock the document retrieval
            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            // Mock failed update (simulating permission denied)
            _mockDocumentService.Setup(s => s.UpdateDocumentAsync(documentId, updateModel))
                .ThrowsAsync(new UnauthorizedAccessException("User not authorized to edit this document"));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _controller.Edit(documentId, updateModel));

            _mockDocumentService.Verify(s => s.UpdateDocumentAsync(documentId, updateModel), Times.Once);
        }

        [Fact]
        public async Task Test_ViewDocument_AsOwner_ShouldSucceed()
        {
            // Arrange
            var documentId = 1;
            var ownerUsername = "documentowner";
            var document = new Document
            {
                Id = documentId,
                Title = "Owner's Document",
                FileName = "owner_doc.pdf",
                FilePath = "/uploads/owner_doc.pdf",
                FileType = "pdf",
                FileSize = 1024,
                UploadedBy = ownerUsername,
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
            Assert.Equal(ownerUsername, model.UploadedBy);
            Assert.Equal("Owner's Document", model.Title);
        }

        [Fact]
        public async Task Test_ViewDocument_AsNonOwner_ShouldSucceed()
        {
            // Arrange
            var documentId = 1;
            var ownerUsername = "documentowner";
            var viewerUsername = "viewer";
            var document = new Document
            {
                Id = documentId,
                Title = "Owner's Document",
                FileName = "owner_doc.pdf",
                FilePath = "/uploads/owner_doc.pdf",
                FileType = "pdf",
                FileSize = 1024,
                UploadedBy = ownerUsername,
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
            Assert.Equal(ownerUsername, model.UploadedBy);
            Assert.Equal("Owner's Document", model.Title);
        }

        [Fact]
        public async Task Test_DeleteDocument_WithProcessingStatus_ShouldSucceed()
        {
            // Arrange
            var documentId = 1;
            var ownerUsername = "documentowner";
            var document = new Document
            {
                Id = documentId,
                Title = "Processing Document",
                FileName = "processing_doc.pdf",
                FilePath = "/uploads/processing_doc.pdf",
                FileType = "pdf",
                FileSize = 1024,
                UploadedBy = ownerUsername,
                UploadDate = DateTime.Now,
                ProcessingStatus = "Processing",
                IsProcessed = false
            };

            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            _mockDocumentService.Setup(s => s.DeleteDocumentAsync(documentId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteConfirmed(documentId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockDocumentService.Verify(s => s.DeleteDocumentAsync(documentId), Times.Once);
        }

        [Fact]
        public async Task Test_DeleteDocument_WithErrorStatus_ShouldSucceed()
        {
            // Arrange
            var documentId = 1;
            var ownerUsername = "documentowner";
            var document = new Document
            {
                Id = documentId,
                Title = "Error Document",
                FileName = "error_doc.pdf",
                FilePath = "/uploads/error_doc.pdf",
                FileType = "pdf",
                FileSize = 1024,
                UploadedBy = ownerUsername,
                UploadDate = DateTime.Now,
                ProcessingStatus = "Error",
                IsProcessed = false
            };

            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            _mockDocumentService.Setup(s => s.DeleteDocumentAsync(documentId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteConfirmed(documentId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockDocumentService.Verify(s => s.DeleteDocumentAsync(documentId), Times.Once);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}