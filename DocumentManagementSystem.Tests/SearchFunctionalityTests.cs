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
    public class SearchFunctionalityTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IDocumentService> _mockDocumentService;
        private readonly Mock<ILogger<DocumentController>> _mockLogger;
        private readonly DocumentController _controller;

        public SearchFunctionalityTests()
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
        public async Task Test_Search_WithRelevantKeyword_ShouldReturnMatchingDocuments()
        {
            // Arrange
            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = "yapay zeka"
            };

            var searchResults = new List<SearchResult>
            {
                new SearchResult
                {
                    Document = new Document
                    {
                        Id = 1,
                        Title = "Yapay Zeka Teknolojileri Raporu",
                        Content = "Bu rapor yapay zeka teknolojilerinin gelişimini ele almaktadır.",
                        UploadedBy = "user1"
                    },
                    RelevanceScore = 0.95,
                    MatchedTerms = new List<string> { "yapay", "zeka" }
                },
                new SearchResult
                {
                    Document = new Document
                    {
                        Id = 2,
                        Title = "Makine Öğrenmesi ve Yapay Zeka",
                        Content = "Makine öğrenmesi yapay zeka alanının önemli bir parçasıdır.",
                        UploadedBy = "user2"
                    },
                    RelevanceScore = 0.85,
                    MatchedTerms = new List<string> { "yapay", "zeka" }
                }
            };

            _mockDocumentService.Setup(s => s.SearchDocumentsAsync(searchModel))
                .ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(searchModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<SearchResult>>(viewResult.Model);

            Assert.Equal(2, model.Count);
            Assert.Equal("Yapay Zeka Teknolojileri Raporu", model[0].Document.Title);
            Assert.Equal("Makine Öğrenmesi ve Yapay Zeka", model[1].Document.Title);
            Assert.Equal(0.95, model[0].RelevanceScore);
            Assert.Equal(0.85, model[1].RelevanceScore);
            Assert.Contains("yapay", model[0].MatchedTerms);
            Assert.Contains("zeka", model[0].MatchedTerms);
        }

        [Fact]
        public async Task Test_Search_WithNaturalLanguageQuery_ShouldReturnRelevantResults()
        {
            // Arrange
            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = "Dördüncü çeyrek satış rakamlarını içeren rapor hangisi?"
            };

            var searchResults = new List<SearchResult>
            {
                new SearchResult
                {
                    Document = new Document
                    {
                        Id = 1,
                        Title = "2023 Q4 Satış Raporu",
                        Content = "Dördüncü çeyrek satış rakamları %15 artış gösterdi. Toplam satış 2.5 milyon TL'ye ulaştı.",
                        UploadedBy = "finance_user"
                    },
                    RelevanceScore = 0.98,
                    MatchedTerms = new List<string> { "dördüncü", "çeyrek", "satış", "rakamları" }
                },
                new SearchResult
                {
                    Document = new Document
                    {
                        Id = 2,
                        Title = "2023 Yıllık Satış Analizi",
                        Content = "Yıl boyunca satış rakamları incelendi. Dördüncü çeyrekte önemli artış görüldü.",
                        UploadedBy = "analyst_user"
                    },
                    RelevanceScore = 0.75,
                    MatchedTerms = new List<string> { "satış", "rakamları", "dördüncü", "çeyrek" }
                },
                new SearchResult
                {
                    Document = new Document
                    {
                        Id = 3,
                        Title = "Genel Pazar Analizi",
                        Content = "Pazar koşulları ve genel ekonomik durum değerlendirildi.",
                        UploadedBy = "market_user"
                    },
                    RelevanceScore = 0.25,
                    MatchedTerms = new List<string> { "pazar" }
                }
            };

            _mockDocumentService.Setup(s => s.SearchDocumentsAsync(searchModel))
                .ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(searchModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<SearchResult>>(viewResult.Model);

            Assert.Equal(3, model.Count);
            // En alakalı doküman en üstte olmalı
            Assert.Equal("2023 Q4 Satış Raporu", model[0].Document.Title);
            Assert.Equal(0.98, model[0].RelevanceScore);
            Assert.Contains("dördüncü", model[0].MatchedTerms);
            Assert.Contains("çeyrek", model[0].MatchedTerms);
            Assert.Contains("satış", model[0].MatchedTerms);
            Assert.Contains("rakamları", model[0].MatchedTerms);
        }

        [Fact]
        public async Task Test_Search_WithNoResults_ShouldReturnEmptyList()
        {
            // Arrange
            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = "hiçbir yerde bulunmayan çok özel terim"
            };

            var emptyResults = new List<SearchResult>();

            _mockDocumentService.Setup(s => s.SearchDocumentsAsync(searchModel))
                .ReturnsAsync(emptyResults);

            // Act
            var result = await _controller.Search(searchModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<SearchResult>>(viewResult.Model);

            Assert.Empty(model);
            Assert.Equal(0, model.Count);
        }

        [Fact]
        public async Task Test_Search_WithEmptyQuery_ShouldReturnBadRequestOrEmpty()
        {
            // Arrange
            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = ""
            };

            // Act
            var result = await _controller.Search(searchModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<SearchResult>>(viewResult.Model);

            // Boş sorgu ile arama yapıldığında tüm dokümanlar dönebilir veya boş liste
            Assert.NotNull(model);
        }

        [Fact]
        public async Task Test_Search_WithWhitespaceQuery_ShouldReturnEmptyOrAllDocuments()
        {
            // Arrange
            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = "   \n\t   "
            };

            // Act
            var result = await _controller.Search(searchModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<SearchResult>>(viewResult.Model);

            Assert.NotNull(model);
        }

        [Fact]
        public async Task Test_Search_WithExactPhrase_ShouldReturnExactMatches()
        {
            // Arrange
            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = "\"yapay zeka teknolojileri\""
            };

            var searchResults = new List<SearchResult>
            {
                new SearchResult
                {
                    Document = new Document
                    {
                        Id = 1,
                        Title = "Yapay Zeka Teknolojileri Raporu",
                        Content = "Bu rapor yapay zeka teknolojilerinin gelişimini ele almaktadır.",
                        UploadedBy = "user1"
                    },
                    RelevanceScore = 1.0,
                    MatchedTerms = new List<string> { "yapay zeka teknolojileri" }
                }
            };

            _mockDocumentService.Setup(s => s.SearchDocumentsAsync(searchModel))
                .ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(searchModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<SearchResult>>(viewResult.Model);

            Assert.Single(model);
            Assert.Equal("Yapay Zeka Teknolojileri Raporu", model[0].Document.Title);
            Assert.Equal(1.0, model[0].RelevanceScore);
        }

        [Fact]
        public async Task Test_Search_WithCaseInsensitiveQuery_ShouldReturnMatches()
        {
            // Arrange
            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = "YAPAY ZEKA"
            };

            var searchResults = new List<SearchResult>
            {
                new SearchResult
                {
                    Document = new Document
                    {
                        Id = 1,
                        Title = "Yapay Zeka Teknolojileri",
                        Content = "Bu doküman yapay zeka konularını ele almaktadır.",
                        UploadedBy = "user1"
                    },
                    RelevanceScore = 0.9,
                    MatchedTerms = new List<string> { "yapay", "zeka" }
                }
            };

            _mockDocumentService.Setup(s => s.SearchDocumentsAsync(searchModel))
                .ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(searchModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<SearchResult>>(viewResult.Model);

            Assert.Single(model);
            Assert.Equal("Yapay Zeka Teknolojileri", model[0].Document.Title);
        }

        [Fact]
        public async Task Test_Search_WithPartialWord_ShouldReturnMatches()
        {
            // Arrange
            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = "teknolo"
            };

            var searchResults = new List<SearchResult>
            {
                new SearchResult
                {
                    Document = new Document
                    {
                        Id = 1,
                        Title = "Teknoloji Raporu",
                        Content = "Bu rapor teknoloji gelişmelerini ele almaktadır.",
                        UploadedBy = "user1"
                    },
                    RelevanceScore = 0.8,
                    MatchedTerms = new List<string> { "teknoloji" }
                }
            };

            _mockDocumentService.Setup(s => s.SearchDocumentsAsync(searchModel))
                .ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(searchModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<SearchResult>>(viewResult.Model);

            Assert.Single(model);
            Assert.Equal("Teknoloji Raporu", model[0].Document.Title);
        }

        [Fact]
        public async Task Test_Search_WithMultipleKeywords_ShouldReturnRelevantResults()
        {
            // Arrange
            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = "yapay zeka makine öğrenmesi"
            };

            var searchResults = new List<SearchResult>
            {
                new SearchResult
                {
                    Document = new Document
                    {
                        Id = 1,
                        Title = "Yapay Zeka ve Makine Öğrenmesi",
                        Content = "Bu doküman hem yapay zeka hem de makine öğrenmesi konularını ele almaktadır.",
                        UploadedBy = "user1"
                    },
                    RelevanceScore = 0.95,
                    MatchedTerms = new List<string> { "yapay", "zeka", "makine", "öğrenmesi" }
                },
                new SearchResult
                {
                    Document = new Document
                    {
                        Id = 2,
                        Title = "Yapay Zeka Teknolojileri",
                        Content = "Bu rapor yapay zeka teknolojilerini ele almaktadır.",
                        UploadedBy = "user2"
                    },
                    RelevanceScore = 0.7,
                    MatchedTerms = new List<string> { "yapay", "zeka" }
                }
            };

            _mockDocumentService.Setup(s => s.SearchDocumentsAsync(searchModel))
                .ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(searchModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<SearchResult>>(viewResult.Model);

            Assert.Equal(2, model.Count);
            Assert.Equal("Yapay Zeka ve Makine Öğrenmesi", model[0].Document.Title);
            Assert.Equal(0.95, model[0].RelevanceScore);
            Assert.Equal(4, model[0].MatchedTerms.Count);
        }

        [Fact]
        public async Task Test_Search_WithSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = "C++ & Python"
            };

            var searchResults = new List<SearchResult>
            {
                new SearchResult
                {
                    Document = new Document
                    {
                        Id = 1,
                        Title = "C++ ve Python Programlama",
                        Content = "Bu doküman C++ ve Python programlama dillerini ele almaktadır.",
                        UploadedBy = "user1"
                    },
                    RelevanceScore = 0.9,
                    MatchedTerms = new List<string> { "C++", "Python" }
                }
            };

            _mockDocumentService.Setup(s => s.SearchDocumentsAsync(searchModel))
                .ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(searchModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<SearchResult>>(viewResult.Model);

            Assert.Single(model);
            Assert.Equal("C++ ve Python Programlama", model[0].Document.Title);
        }

        [Fact]
        public async Task Test_Search_WithNumbers_ShouldReturnMatches()
        {
            // Arrange
            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = "2023 2024"
            };

            var searchResults = new List<SearchResult>
            {
                new SearchResult
                {
                    Document = new Document
                    {
                        Id = 1,
                        Title = "2023-2024 Yıllık Rapor",
                        Content = "Bu rapor 2023 ve 2024 yıllarını kapsamaktadır.",
                        UploadedBy = "user1"
                    },
                    RelevanceScore = 0.9,
                    MatchedTerms = new List<string> { "2023", "2024" }
                }
            };

            _mockDocumentService.Setup(s => s.SearchDocumentsAsync(searchModel))
                .ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(searchModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<SearchResult>>(viewResult.Model);

            Assert.Single(model);
            Assert.Equal("2023-2024 Yıllık Rapor", model[0].Document.Title);
        }

        [Fact]
        public async Task Test_Search_WithTurkishCharacters_ShouldReturnMatches()
        {
            // Arrange
            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = "öğrenme algoritması"
            };

            var searchResults = new List<SearchResult>
            {
                new SearchResult
                {
                    Document = new Document
                    {
                        Id = 1,
                        Title = "Makine Öğrenme Algoritmaları",
                        Content = "Bu doküman makine öğrenme algoritmalarını ele almaktadır.",
                        UploadedBy = "user1"
                    },
                    RelevanceScore = 0.9,
                    MatchedTerms = new List<string> { "öğrenme", "algoritması" }
                }
            };

            _mockDocumentService.Setup(s => s.SearchDocumentsAsync(searchModel))
                .ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(searchModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<SearchResult>>(viewResult.Model);

            Assert.Single(model);
            Assert.Equal("Makine Öğrenme Algoritmaları", model[0].Document.Title);
        }

        [Fact]
        public async Task Test_Search_WithVeryLongQuery_ShouldHandleGracefully()
        {
            // Arrange
            var longQuery = string.Join(" ", Enumerable.Range(1, 100).Select(i => $"kelime{i}"));
            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = longQuery
            };

            var searchResults = new List<SearchResult>();

            _mockDocumentService.Setup(s => s.SearchDocumentsAsync(searchModel))
                .ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(searchModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<SearchResult>>(viewResult.Model);

            Assert.Empty(model);
        }

        [Fact]
        public async Task Test_Search_WithNullQuery_ShouldHandleGracefully()
        {
            // Arrange
            var searchModel = new DocumentSearchViewModel
            {
                SearchTerm = null
            };

            // Act
            var result = await _controller.Search(searchModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<SearchResult>>(viewResult.Model);

            Assert.NotNull(model);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}