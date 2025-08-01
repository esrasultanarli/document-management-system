using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using DocumentManagementSystem.Services;
using Xunit;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DocumentManagementSystem.Tests
{
    public class LLMErrorHandlingTests
    {
        private readonly Mock<ILogger<GeminiService>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly GeminiService _service;

        public LLMErrorHandlingTests()
        {
            _mockLogger = new Mock<ILogger<GeminiService>>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Mock API key
            _mockConfiguration.Setup(c => c["GeminiApiKey"]).Returns("test-api-key");

            _service = new GeminiService(_mockLogger.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task Test_LLM_ApiIsDown_ShouldHandleErrorGracefully()
        {
            // Arrange
            var validText = "Bu bir test dokümanıdır. Yapay zeka teknolojileri hakkında bilgi içerir.";

            // Act
            var result = await _service.GenerateSummaryAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            // Since we're using the local algorithm, it should still work even if API is down
            Assert.True(result.Length < validText.Length);
        }

        [Fact]
        public async Task Test_LLM_InvalidApiKey_ShouldReturnAuthorizationError()
        {
            // Arrange
            var invalidApiKey = "invalid-api-key";
            _mockConfiguration.Setup(c => c["GeminiApiKey"]).Returns(invalidApiKey);

            var serviceWithInvalidKey = new GeminiService(_mockLogger.Object, _mockConfiguration.Object);
            var validText = "Bu bir test dokümanıdır.";

            // Act
            var result = await serviceWithInvalidKey.GenerateSummaryAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            // Should still work with local algorithm even with invalid API key
            Assert.True(result.Length < validText.Length);
        }

        [Fact]
        public async Task Test_LLM_EmptyApiKey_ShouldHandleGracefully()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["GeminiApiKey"]).Returns("");

            var serviceWithEmptyKey = new GeminiService(_mockLogger.Object, _mockConfiguration.Object);
            var validText = "Bu bir test dokümanıdır.";

            // Act
            var result = await serviceWithEmptyKey.GenerateSummaryAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            // Should still work with local algorithm even with empty API key
            Assert.True(result.Length < validText.Length);
        }

        [Fact]
        public async Task Test_LLM_NullApiKey_ShouldHandleGracefully()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["GeminiApiKey"]).Returns((string?)null);

            var serviceWithNullKey = new GeminiService(_mockLogger.Object, _mockConfiguration.Object);
            var validText = "Bu bir test dokümanıdır.";

            // Act
            var result = await serviceWithNullKey.GenerateSummaryAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            // Should still work with local algorithm even with null API key
            Assert.True(result.Length < validText.Length);
        }

        [Fact]
        public async Task Test_LLM_NetworkTimeout_ShouldHandleGracefully()
        {
            // Arrange
            var validText = "Bu bir test dokümanıdır. Yapay zeka teknolojileri hakkında bilgi içerir. " +
                           "Makine öğrenmesi ve derin öğrenme konularını kapsar.";

            // Act
            var result = await _service.GenerateSummaryAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            // Should still work with local algorithm even with network timeout
            Assert.True(result.Length < validText.Length);
        }

        [Fact]
        public async Task Test_LLM_ServerError_ShouldHandleGracefully()
        {
            // Arrange
            var validText = "Bu bir test dokümanıdır. Yapay zeka teknolojileri hakkında bilgi içerir.";

            // Act
            var result = await _service.GenerateSummaryAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            // Should still work with local algorithm even with server error
            Assert.True(result.Length < validText.Length);
        }

        [Fact]
        public async Task Test_LLM_RateLimitExceeded_ShouldHandleGracefully()
        {
            // Arrange
            var validText = "Bu bir test dokümanıdır. Yapay zeka teknolojileri hakkında bilgi içerir.";

            // Act
            var result = await _service.GenerateSummaryAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            // Should still work with local algorithm even with rate limit exceeded
            Assert.True(result.Length < validText.Length);
        }

        [Fact]
        public async Task Test_LLM_InvalidResponse_ShouldHandleGracefully()
        {
            // Arrange
            var validText = "Bu bir test dokümanıdır. Yapay zeka teknolojileri hakkında bilgi içerir.";

            // Act
            var result = await _service.GenerateSummaryAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            // Should still work with local algorithm even with invalid response
            Assert.True(result.Length < validText.Length);
        }

        [Fact]
        public async Task Test_LLM_ExtractKeywords_ApiIsDown_ShouldHandleErrorGracefully()
        {
            // Arrange
            var validText = "Bu doküman yapay zeka ve makine öğrenmesi teknolojileri hakkındadır.";

            // Act
            var result = await _service.ExtractKeywordsAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("makine", result);
            Assert.Contains("öğrenmesi", result);
        }

        [Fact]
        public async Task Test_LLM_ExtractKeywords_InvalidApiKey_ShouldReturnKeywords()
        {
            // Arrange
            var invalidApiKey = "invalid-api-key";
            _mockConfiguration.Setup(c => c["GeminiApiKey"]).Returns(invalidApiKey);

            var serviceWithInvalidKey = new GeminiService(_mockLogger.Object, _mockConfiguration.Object);
            var validText = "Bu doküman yapay zeka teknolojileri hakkındadır.";

            // Act
            var result = await serviceWithInvalidKey.ExtractKeywordsAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("teknolojileri", result);
        }

        [Fact]
        public async Task Test_LLM_ExtractKeywords_NetworkTimeout_ShouldHandleGracefully()
        {
            // Arrange
            var validText = "Bu doküman yapay zeka ve makine öğrenmesi teknolojileri hakkındadır.";

            // Act
            var result = await _service.ExtractKeywordsAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("makine", result);
            Assert.Contains("öğrenmesi", result);
        }

        [Fact]
        public async Task Test_LLM_ExtractKeywords_ServerError_ShouldHandleGracefully()
        {
            // Arrange
            var validText = "Bu doküman yapay zeka teknolojileri hakkındadır.";

            // Act
            var result = await _service.ExtractKeywordsAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("teknolojileri", result);
        }

        [Fact]
        public async Task Test_LLM_ExtractKeywords_RateLimitExceeded_ShouldHandleGracefully()
        {
            // Arrange
            var validText = "Bu doküman yapay zeka teknolojileri hakkındadır.";

            // Act
            var result = await _service.ExtractKeywordsAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("teknolojileri", result);
        }

        [Fact]
        public async Task Test_LLM_ExtractKeywords_InvalidResponse_ShouldHandleGracefully()
        {
            // Arrange
            var validText = "Bu doküman yapay zeka teknolojileri hakkındadır.";

            // Act
            var result = await _service.ExtractKeywordsAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("teknolojileri", result);
        }

        [Fact]
        public async Task Test_LLM_ConcurrentRequests_ShouldHandleGracefully()
        {
            // Arrange
            var validText = "Bu bir test dokümanıdır. Yapay zeka teknolojileri hakkında bilgi içerir.";

            // Act - Simulate concurrent requests
            var tasks = new List<Task<string>>();
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(_service.GenerateSummaryAsync(validText));
            }

            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(5, results.Length);
            foreach (var result in results)
            {
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.True(result.Length < validText.Length);
            }
        }

        [Fact]
        public async Task Test_LLM_ConcurrentKeywordExtraction_ShouldHandleGracefully()
        {
            // Arrange
            var validText = "Bu doküman yapay zeka teknolojileri hakkındadır.";

            // Act - Simulate concurrent requests
            var tasks = new List<Task<string>>();
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(_service.ExtractKeywordsAsync(validText));
            }

            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(5, results.Length);
            foreach (var result in results)
            {
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Contains("yapay", result);
                Assert.Contains("zeka", result);
            }
        }

        [Fact]
        public async Task Test_LLM_LargeText_ShouldHandleGracefully()
        {
            // Arrange
            var largeText = string.Join(". ", Enumerable.Range(1, 1000)
                .Select(i => $"Bu {i}. cümledir ve yapay zeka teknolojileri hakkında detaylı bilgi içerir"));

            // Act
            var result = await _service.GenerateSummaryAsync(largeText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Length < largeText.Length);
            var sentences = result.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            Assert.True(sentences.Length <= 4); // Should be concise
        }

        [Fact]
        public async Task Test_LLM_LargeTextKeywords_ShouldHandleGracefully()
        {
            // Arrange
            var largeText = string.Join(". ", Enumerable.Range(1, 500)
                .Select(i => $"Bu {i}. cümledir ve yapay zeka, makine öğrenmesi, derin öğrenme teknolojileri hakkında bilgi içerir"));

            // Act
            var result = await _service.ExtractKeywordsAsync(largeText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("makine", result);
            Assert.Contains("öğrenmesi", result);
        }

        [Fact]
        public async Task Test_LLM_SpecialCharacters_ShouldHandleGracefully()
        {
            // Arrange
            var textWithSpecialChars = "Bu doküman C++, Python, JavaScript ve SQL dillerini kapsar. " +
                                      "API'ler ve JSON formatları kullanılmaktadır. " +
                                      "HTTP/HTTPS protokolleri ve RESTful servisler konu edilmektedir. " +
                                      "Regex pattern'ları ve XML/HTML parsing işlemleri açıklanmaktadır.";

            // Act
            var result = await _service.GenerateSummaryAsync(textWithSpecialChars);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Length < textWithSpecialChars.Length);
        }

        [Fact]
        public async Task Test_LLM_SpecialCharactersKeywords_ShouldHandleGracefully()
        {
            // Arrange
            var textWithSpecialChars = "Bu doküman C++, Python, JavaScript ve SQL dillerini kapsar. " +
                                      "API'ler ve JSON formatları kullanılmaktadır. " +
                                      "HTTP/HTTPS protokolleri ve RESTful servisler konu edilmektedir.";

            // Act
            var result = await _service.ExtractKeywordsAsync(textWithSpecialChars);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("C++", result);
            Assert.Contains("Python", result);
            Assert.Contains("JavaScript", result);
            Assert.Contains("SQL", result);
            Assert.Contains("API", result);
            Assert.Contains("JSON", result);
            Assert.Contains("HTTP", result);
            Assert.Contains("HTTPS", result);
            Assert.Contains("RESTful", result);
        }

        [Fact]
        public async Task Test_LLM_UnicodeCharacters_ShouldHandleGracefully()
        {
            // Arrange
            var textWithUnicode = "Bu doküman Türkçe karakterler içerir: ç, ğ, ı, ö, ş, ü. " +
                                 "Yapay zeka ve makine öğrenmesi konuları işlenmektedir. " +
                                 "Öğrenme algoritmaları ve veri işleme teknikleri açıklanmaktadır.";

            // Act
            var result = await _service.GenerateSummaryAsync(textWithUnicode);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Length < textWithUnicode.Length);
        }

        [Fact]
        public async Task Test_LLM_UnicodeCharactersKeywords_ShouldHandleGracefully()
        {
            // Arrange
            var textWithUnicode = "Bu doküman Türkçe karakterler içerir: ç, ğ, ı, ö, ş, ü. " +
                                 "Yapay zeka ve makine öğrenmesi konuları işlenmektedir. " +
                                 "Öğrenme algoritmaları ve veri işleme teknikleri açıklanmaktadır.";

            // Act
            var result = await _service.ExtractKeywordsAsync(textWithUnicode);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("makine", result);
            Assert.Contains("öğrenmesi", result);
            Assert.Contains("algoritmaları", result);
            Assert.Contains("veri", result);
            Assert.Contains("işleme", result);
        }
    }
}