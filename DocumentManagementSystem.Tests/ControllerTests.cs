using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Moq;
using DocumentManagementSystem.Controllers;
using DocumentManagementSystem.Services;
using DocumentManagementSystem.ViewModels;
using DocumentManagementSystem.Models;
using Xunit;

namespace DocumentManagementSystem.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_mockAuthService.Object, _mockLogger.Object);
        }

        [Fact]
        public void Login_Get_ShouldReturnView()
        {
            // Act
            var result = _controller.Login();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public async Task Login_Post_ValidCredentials_ShouldRedirectToHome()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = "testuser",
                Password = "TestPassword123!"
            };

            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                IsActive = true
            };

            _mockAuthService.Setup(s => s.AuthenticateUserAsync(model.Username, model.Password))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Login(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Document", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Login_Post_InvalidCredentials_ShouldReturnViewWithError()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = "testuser",
                Password = "WrongPassword"
            };

            _mockAuthService.Setup(s => s.AuthenticateUserAsync(model.Username, model.Password))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _controller.Login(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey(""));
        }

        [Fact]
        public void Register_Get_ShouldReturnView()
        {
            // Act
            var result = _controller.Register();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public async Task Register_Post_ValidModel_ShouldRedirectToLogin()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Username = "newuser",
                Email = "new@example.com",
                FirstName = "New",
                LastName = "User",
                Password = "TestPassword123!"
            };

            _mockAuthService.Setup(s => s.RegisterUserAsync(model))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Register(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
        }

        [Fact]
        public async Task Register_Post_InvalidModel_ShouldReturnViewWithError()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Username = "existinguser",
                Email = "existing@example.com",
                FirstName = "Existing",
                LastName = "User",
                Password = "TestPassword123!"
            };

            _mockAuthService.Setup(s => s.RegisterUserAsync(model))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Register(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey(""));
        }

        [Fact]
        public void Logout_ShouldRedirectToLogin()
        {
            // Act
            var result = _controller.Logout();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
        }
    }

    public class DocumentControllerTests
    {
        private readonly Mock<IDocumentService> _mockDocumentService;
        private readonly Mock<ILogger<DocumentController>> _mockLogger;
        private readonly DocumentController _controller;

        public DocumentControllerTests()
        {
            _mockDocumentService = new Mock<IDocumentService>();
            _mockLogger = new Mock<ILogger<DocumentController>>();
            _controller = new DocumentController(_mockDocumentService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithDocuments()
        {
            // Arrange
            var documents = new List<Document>
            {
                new Document { Id = 1, Title = "Doc 1", UploadedBy = "user1" },
                new Document { Id = 2, Title = "Doc 2", UploadedBy = "user2" }
            };

            _mockDocumentService.Setup(s => s.GetAllDocumentsAsync())
                .ReturnsAsync(documents);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<Document>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Upload_Get_ShouldReturnView()
        {
            // Act
            var result = _controller.Upload();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public async Task Upload_Post_ValidModel_ShouldRedirectToIndex()
        {
            // Arrange
            var model = new DocumentUploadViewModel
            {
                Title = "Test Document",
                File = CreateMockFile("test.txt", "Test content")
            };

            var document = new Document
            {
                Id = 1,
                Title = "Test Document",
                FileName = "test.txt",
                UploadedBy = "testuser"
            };

            _mockDocumentService.Setup(s => s.UploadDocumentAsync(model, "testuser"))
                .ReturnsAsync(document);

            // Act
            var result = await _controller.Upload(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Upload_Post_InvalidModel_ShouldReturnViewWithError()
        {
            // Arrange
            var model = new DocumentUploadViewModel
            {
                Title = "Test Document",
                File = null
            };

            _controller.ModelState.AddModelError("File", "File is required");

            // Act
            var result = await _controller.Upload(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Details_ExistingDocument_ShouldReturnView()
        {
            // Arrange
            var document = new Document
            {
                Id = 1,
                Title = "Test Document",
                FileName = "test.txt",
                UploadedBy = "testuser"
            };

            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(1))
                .ReturnsAsync(document);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Document>(viewResult.Model);
            Assert.Equal("Test Document", model.Title);
        }

        [Fact]
        public async Task Details_NonExistentDocument_ShouldReturnNotFound()
        {
            // Arrange
            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(999))
                .ReturnsAsync((Document?)null);

            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ExistingDocument_ShouldReturnView()
        {
            // Arrange
            var document = new Document
            {
                Id = 1,
                Title = "Test Document",
                FileName = "test.txt",
                UploadedBy = "testuser"
            };

            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(1))
                .ReturnsAsync(document);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Document>(viewResult.Model);
            Assert.Equal("Test Document", model.Title);
        }

        [Fact]
        public async Task Edit_NonExistentDocument_ShouldReturnNotFound()
        {
            // Arrange
            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(999))
                .ReturnsAsync((Document?)null);

            // Act
            var result = await _controller.Edit(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_ShouldRedirectToDetails()
        {
            // Arrange
            var model = new DocumentViewModel
            {
                Id = 1,
                Title = "Updated Title"
            };

            var document = new Document
            {
                Id = 1,
                Title = "Updated Title",
                FileName = "test.txt",
                UploadedBy = "testuser"
            };

            _mockDocumentService.Setup(s => s.UpdateDocumentAsync(1, model))
                .ReturnsAsync(document);

            // Act
            var result = await _controller.Edit(1, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues["id"]);
        }

        [Fact]
        public async Task Edit_Post_InvalidModel_ShouldReturnView()
        {
            // Arrange
            var model = new DocumentViewModel
            {
                Id = 1,
                Title = ""
            };

            _controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _controller.Edit(1, model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Delete_ExistingDocument_ShouldReturnView()
        {
            // Arrange
            var document = new Document
            {
                Id = 1,
                Title = "Test Document",
                FileName = "test.txt",
                UploadedBy = "testuser"
            };

            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(1))
                .ReturnsAsync(document);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Document>(viewResult.Model);
            Assert.Equal("Test Document", model.Title);
        }

        [Fact]
        public async Task Delete_NonExistentDocument_ShouldReturnNotFound()
        {
            // Arrange
            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(999))
                .ReturnsAsync((Document?)null);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_ExistingDocument_ShouldRedirectToIndex()
        {
            // Arrange
            _mockDocumentService.Setup(s => s.DeleteDocumentAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task DeleteConfirmed_NonExistentDocument_ShouldReturnNotFound()
        {
            // Arrange
            _mockDocumentService.Setup(s => s.DeleteDocumentAsync(999))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteConfirmed(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Search_Get_ShouldReturnView()
        {
            // Act
            var result = _controller.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public async Task Search_Post_ValidModel_ShouldReturnViewWithResults()
        {
            // Arrange
            var model = new DocumentSearchViewModel
            {
                SearchTerm = "artificial intelligence"
            };

            var searchResults = new List<SearchResult>
            {
                new SearchResult
                {
                    Document = new Document { Id = 1, Title = "AI Document" },
                    RelevanceScore = 0.8,
                    MatchedTerms = new List<string> { "artificial", "intelligence" }
                }
            };

            _mockDocumentService.Setup(s => s.SearchDocumentsAsync(model))
                .ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsType<List<SearchResult>>(viewResult.Model);
            Assert.Single(resultModel);
            Assert.Equal("AI Document", resultModel[0].Document.Title);
        }

        [Fact]
        public async Task Search_Post_EmptySearchTerm_ShouldReturnAllDocuments()
        {
            // Arrange
            var model = new DocumentSearchViewModel
            {
                SearchTerm = ""
            };

            var searchResults = new List<SearchResult>
            {
                new SearchResult
                {
                    Document = new Document { Id = 1, Title = "Doc 1" },
                    RelevanceScore = 1.0,
                    MatchedTerms = new List<string>()
                },
                new SearchResult
                {
                    Document = new Document { Id = 2, Title = "Doc 2" },
                    RelevanceScore = 1.0,
                    MatchedTerms = new List<string>()
                }
            };

            _mockDocumentService.Setup(s => s.SearchDocumentsAsync(model))
                .ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsType<List<SearchResult>>(viewResult.Model);
            Assert.Equal(2, resultModel.Count);
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

        [Fact]
        public async Task Test_AccessDocument_UserIsNotOwner_ShouldBeDenied()
        {
            // Arrange
            var documentId = 1;
            var document = new Document
            {
                Id = documentId,
                Title = "Test Document",
                FileName = "test.txt",
                FileType = "txt",
                FileSize = 1024,
                UploadedBy = "otheruser", // Başka kullanıcı
                UploadDate = DateTime.Now,
                LastModified = DateTime.Now,
                ProcessingStatus = "Completed",
                IsProcessed = true
            };

            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            // Act
            var result = await _controller.Edit(documentId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(documentId, redirectResult.RouteValues["id"]);
        }

        [Fact]
        public async Task Test_DeleteDocument_UserIsNotOwner_ShouldBeDenied()
        {
            // Arrange
            var documentId = 1;
            _mockDocumentService.Setup(s => s.DeleteDocumentAsync(documentId, It.IsAny<string>()))
                .ThrowsAsync(new UnauthorizedAccessException("Bu dokümanı silme yetkiniz yok."));

            // Act
            var result = await _controller.Delete(documentId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_UserIsNotOwner_ShouldBeDenied()
        {
            // Arrange
            var documentId = 1;
            var model = new DocumentViewModel
            {
                Id = documentId,
                Title = "Updated Title",
                FileName = "test.txt",
                FileType = "txt",
                FileSize = 1024,
                Content = "Updated content",
                Summary = "Updated summary",
                Keywords = "updated, keywords"
            };

            var document = new Document
            {
                Id = documentId,
                Title = "Original Title",
                FileName = "test.txt",
                FileType = "txt",
                FileSize = 1024,
                UploadedBy = "otheruser", // Başka kullanıcı
                UploadDate = DateTime.Now,
                LastModified = DateTime.Now,
                ProcessingStatus = "Completed",
                IsProcessed = true
            };

            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            // Act
            var result = await _controller.Edit(documentId, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(documentId, redirectResult.RouteValues["id"]);
        }
    }
}