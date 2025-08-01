using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using DocumentManagementSystem.Services;
using Xunit;
using System.Net.Http;
using System.Net;

namespace DocumentManagementSystem.Tests
{
    public class LLMIntegrationTests
    {
        private readonly Mock<ILogger<GeminiService>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly GeminiService _service;

        public LLMIntegrationTests()
        {
            _mockLogger = new Mock<ILogger<GeminiService>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockHttpClient = new Mock<HttpClient>();

            // Mock API key
            _mockConfiguration.Setup(c => c["GeminiApiKey"]).Returns("test-api-key");

            _service = new GeminiService(_mockLogger.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task Test_Summarize_WithValidText_ShouldReturnSummary()
        {
            // Arrange
            var validText = "Bu bir test dokümanıdır. Yapay zeka teknolojileri hakkında bilgi içerir. " +
                           "Makine öğrenmesi ve derin öğrenme konularını kapsar. " +
                           "Bu teknolojiler günümüzde çok önemlidir. " +
                           "Gelecekte daha da gelişeceklerdir.";

            // Act
            var result = await _service.GenerateSummaryAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Length < validText.Length);
            Assert.Contains(".", result);
            Assert.DoesNotContain("Bu bir test dokümanıdır", result); // Should be summarized, not exact copy
        }

        [Fact]
        public async Task Test_ExtractKeywords_WithValidText_ShouldReturnKeywords()
        {
            // Arrange
            var validText = "Bu doküman yapay zeka ve makine öğrenmesi teknolojileri hakkındadır. " +
                           "Derin öğrenme algoritmaları ve neural network'ler konu edilmektedir. " +
                           "Doğal dil işleme ve computer vision konuları da ele alınmaktadır.";

            // Act
            var result = await _service.ExtractKeywordsAsync(validText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("makine", result);
            Assert.Contains("öğrenmesi", result);
            Assert.Contains("derin", result);
            Assert.Contains("neural", result);
            Assert.Contains("network", result);
        }

        [Fact]
        public async Task Test_Summarize_WithEmptyText_ShouldReturnEmptyOrError()
        {
            // Arrange
            var emptyText = "";

            // Act
            var result = await _service.GenerateSummaryAsync(emptyText);

            // Assert
            Assert.Equal("İçerik bulunamadı.", result);
        }

        [Fact]
        public async Task Test_Summarize_WithWhitespaceText_ShouldReturnEmptyOrError()
        {
            // Arrange
            var whitespaceText = "   \n\t   ";

            // Act
            var result = await _service.GenerateSummaryAsync(whitespaceText);

            // Assert
            Assert.Equal("İçerik bulunamadı.", result);
        }

        [Fact]
        public async Task Test_ExtractKeywords_WithEmptyText_ShouldReturnEmptyOrError()
        {
            // Arrange
            var emptyText = "";

            // Act
            var result = await _service.ExtractKeywordsAsync(emptyText);

            // Assert
            Assert.Equal("İçerik bulunamadı.", result);
        }

        [Fact]
        public async Task Test_ExtractKeywords_WithWhitespaceText_ShouldReturnEmptyOrError()
        {
            // Arrange
            var whitespaceText = "   \n\t   ";

            // Act
            var result = await _service.ExtractKeywordsAsync(whitespaceText);

            // Assert
            Assert.Equal("İçerik bulunamadı.", result);
        }

        [Fact]
        public async Task Test_Summarize_WithShortText_ShouldReturnSummary()
        {
            // Arrange
            var shortText = "Kısa bir test içeriği.";

            // Act
            var result = await _service.GenerateSummaryAsync(shortText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Length <= shortText.Length);
        }

        [Fact]
        public async Task Test_ExtractKeywords_WithShortText_ShouldReturnKeywords()
        {
            // Arrange
            var shortText = "Yapay zeka teknolojisi.";

            // Act
            var result = await _service.ExtractKeywordsAsync(shortText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
        }

        [Fact]
        public async Task Test_Summarize_WithTechnicalText_ShouldReturnTechnicalSummary()
        {
            // Arrange
            var technicalText = "Bu araştırma convolutional neural networks (CNN) ve " +
                               "recurrent neural networks (RNN) kullanarak " +
                               "natural language processing (NLP) görevlerini gerçekleştirmektedir. " +
                               "Transfer learning ve fine-tuning teknikleri uygulanmıştır. " +
                               "Sonuçlar %85 doğruluk oranı göstermektedir.";

            // Act
            var result = await _service.GenerateSummaryAsync(technicalText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Length < technicalText.Length);
            Assert.Contains(".", result);
        }

        [Fact]
        public async Task Test_ExtractKeywords_WithTechnicalText_ShouldReturnTechnicalKeywords()
        {
            // Arrange
            var technicalText = "Bu araştırma convolutional neural networks (CNN) ve " +
                               "recurrent neural networks (RNN) kullanarak " +
                               "natural language processing (NLP) görevlerini gerçekleştirmektedir. " +
                               "Transfer learning ve fine-tuning teknikleri uygulanmıştır.";

            // Act
            var result = await _service.ExtractKeywordsAsync(technicalText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("neural", result);
            Assert.Contains("networks", result);
            Assert.Contains("CNN", result);
            Assert.Contains("RNN", result);
            Assert.Contains("NLP", result);
            Assert.Contains("processing", result);
        }

        [Fact]
        public async Task Test_Summarize_WithMixedLanguageText_ShouldReturnSummary()
        {
            // Arrange
            var mixedLanguageText = "Bu doküman hem Türkçe hem İngilizce terimler içerir. " +
                                   "Machine learning ve deep learning konuları işlenmektedir. " +
                                   "Yapay zeka ve artificial intelligence aynı anlama gelir. " +
                                   "Veri bilimi ve data science eş anlamlıdır.";

            // Act
            var result = await _service.GenerateSummaryAsync(mixedLanguageText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Length < mixedLanguageText.Length);
        }

        [Fact]
        public async Task Test_ExtractKeywords_WithMixedLanguageText_ShouldReturnMixedKeywords()
        {
            // Arrange
            var mixedLanguageText = "Bu doküman hem Türkçe hem İngilizce terimler içerir. " +
                                   "Machine learning ve deep learning konuları işlenmektedir. " +
                                   "Yapay zeka ve artificial intelligence aynı anlama gelir. " +
                                   "Veri bilimi ve data science eş anlamlıdır.";

            // Act
            var result = await _service.ExtractKeywordsAsync(mixedLanguageText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("machine", result);
            Assert.Contains("learning", result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("artificial", result);
            Assert.Contains("intelligence", result);
        }

        [Fact]
        public async Task Test_Summarize_WithNumbersAndSpecialCharacters_ShouldReturnSummary()
        {
            // Arrange
            var textWithNumbers = "2023 yılında yapay zeka teknolojileri %25 büyüme göstermiştir. " +
                                 "2024 yılında bu büyüme %30'a çıkması beklenmektedir. " +
                                 "Toplam pazar büyüklüğü 1.5 milyar dolar olmuştur.";

            // Act
            var result = await _service.GenerateSummaryAsync(textWithNumbers);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Length < textWithNumbers.Length);
        }

        [Fact]
        public async Task Test_ExtractKeywords_WithNumbersAndSpecialCharacters_ShouldReturnKeywords()
        {
            // Arrange
            var textWithNumbers = "2023 yılında yapay zeka teknolojileri %25 büyüme göstermiştir. " +
                                 "2024 yılında bu büyüme %30'a çıkması beklenmektedir. " +
                                 "Toplam pazar büyüklüğü 1.5 milyar dolar olmuştur.";

            // Act
            var result = await _service.ExtractKeywordsAsync(textWithNumbers);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("teknolojileri", result);
            Assert.Contains("büyüme", result);
        }

        [Fact]
        public async Task Test_Summarize_WithLongText_ShouldReturnConciseSummary()
        {
            // Arrange
            var longText = string.Join(". ", Enumerable.Range(1, 50)
                .Select(i => $"Bu {i}. cümledir ve yapay zeka teknolojileri hakkında detaylı bilgi içerir"));

            // Act
            var result = await _service.GenerateSummaryAsync(longText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Length < longText.Length);
            var sentences = result.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            Assert.True(sentences.Length <= 4); // Should be concise
        }

        [Fact]
        public async Task Test_ExtractKeywords_WithLongText_ShouldReturnRelevantKeywords()
        {
            // Arrange
            var longText = string.Join(". ", Enumerable.Range(1, 30)
                .Select(i => $"Bu {i}. cümledir ve yapay zeka, makine öğrenmesi, derin öğrenme teknolojileri hakkında bilgi içerir"));

            // Act
            var result = await _service.ExtractKeywordsAsync(longText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("makine", result);
            Assert.Contains("öğrenmesi", result);
            Assert.Contains("derin", result);
        }

        [Fact]
        public async Task Test_Summarize_WithMetadata_ShouldRemoveMetadata()
        {
            // Arrange
            var textWithMetadata = "Abstract: Bu bir özettir.\n" +
                                  "Introduction: Bu bir giriştir.\n" +
                                  "Author: Test Yazar\n" +
                                  "Bu gerçek içeriktir. Bu cümle özetlenmelidir. " +
                                  "Bu da ikinci cümledir. Bu da üçüncü cümledir.";

            // Act
            var result = await _service.GenerateSummaryAsync(textWithMetadata);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.DoesNotContain("Abstract:", result);
            Assert.DoesNotContain("Introduction:", result);
            Assert.DoesNotContain("Author:", result);
        }

        [Fact]
        public async Task Test_ExtractKeywords_WithMetadata_ShouldExtractContentKeywords()
        {
            // Arrange
            var textWithMetadata = "Abstract: Bu bir özettir.\n" +
                                  "Introduction: Bu bir giriştir.\n" +
                                  "Author: Test Yazar\n" +
                                  "Bu gerçek içeriktir. Yapay zeka teknolojileri hakkında bilgi içerir. " +
                                  "Makine öğrenmesi konuları ele alınmaktadır.";

            // Act
            var result = await _service.ExtractKeywordsAsync(textWithMetadata);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("makine", result);
            Assert.Contains("öğrenmesi", result);
        }

        [Fact]
        public async Task Test_Summarize_WithQuotes_ShouldHandleCorrectly()
        {
            // Arrange
            var textWithQuotes = "Einstein şöyle demiştir: \"Hayal gücü bilgiden daha önemlidir.\" " +
                                "Bu söz yapay zeka araştırmalarında da geçerlidir. " +
                                "Yaratıcılık ve inovasyon çok önemlidir.";

            // Act
            var result = await _service.GenerateSummaryAsync(textWithQuotes);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Length < textWithQuotes.Length);
        }

        [Fact]
        public async Task Test_ExtractKeywords_WithQuotes_ShouldExtractKeywords()
        {
            // Arrange
            var textWithQuotes = "Einstein şöyle demiştir: \"Hayal gücü bilgiden daha önemlidir.\" " +
                                "Bu söz yapay zeka araştırmalarında da geçerlidir. " +
                                "Yaratıcılık ve inovasyon çok önemlidir.";

            // Act
            var result = await _service.ExtractKeywordsAsync(textWithQuotes);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("yaratıcılık", result);
            Assert.Contains("inovasyon", result);
        }

        [Fact]
        public async Task Test_Summarize_WithBulletPoints_ShouldHandleCorrectly()
        {
            // Arrange
            var textWithBullets = "Yapay zeka teknolojileri:\n" +
                                 "• Makine öğrenmesi\n" +
                                 "• Derin öğrenme\n" +
                                 "• Doğal dil işleme\n" +
                                 "Bu teknolojiler günümüzde çok önemlidir. " +
                                 "Gelecekte daha da gelişeceklerdir.";

            // Act
            var result = await _service.GenerateSummaryAsync(textWithBullets);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Length < textWithBullets.Length);
        }

        [Fact]
        public async Task Test_ExtractKeywords_WithBulletPoints_ShouldExtractKeywords()
        {
            // Arrange
            var textWithBullets = "Yapay zeka teknolojileri:\n" +
                                 "• Makine öğrenmesi\n" +
                                 "• Derin öğrenme\n" +
                                 "• Doğal dil işleme\n" +
                                 "Bu teknolojiler günümüzde çok önemlidir.";

            // Act
            var result = await _service.ExtractKeywordsAsync(textWithBullets);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("makine", result);
            Assert.Contains("öğrenmesi", result);
            Assert.Contains("derin", result);
            Assert.Contains("doğal", result);
            Assert.Contains("dil", result);
            Assert.Contains("işleme", result);
        }
    }
}