using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using DocumentManagementSystem.Services;
using Xunit;

namespace DocumentManagementSystem.Tests
{
    public class GeminiServiceTests
    {
        private readonly Mock<ILogger<GeminiService>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly GeminiService _service;

        public GeminiServiceTests()
        {
            _mockLogger = new Mock<ILogger<GeminiService>>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Mock API key
            _mockConfiguration.Setup(c => c["GeminiApiKey"]).Returns("test-api-key");

            _service = new GeminiService(_mockLogger.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task GenerateSummaryAsync_ValidContent_ShouldReturnSummary()
        {
            // Arrange
            var content = "Bu bir test dokümanıdır. Yapay zeka teknolojileri hakkında bilgi içerir. " +
                         "Makine öğrenmesi ve derin öğrenme konularını kapsar. " +
                         "Bu teknolojiler günümüzde çok önemlidir. " +
                         "Gelecekte daha da gelişeceklerdir.";

            // Act
            var result = await _service.GenerateSummaryAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Length < content.Length);
            Assert.Contains(".", result);
        }

        [Fact]
        public async Task GenerateSummaryAsync_EmptyContent_ShouldReturnDefaultMessage()
        {
            // Arrange
            var content = "";

            // Act
            var result = await _service.GenerateSummaryAsync(content);

            // Assert
            Assert.Equal("İçerik bulunamadı.", result);
        }

        [Fact]
        public async Task GenerateSummaryAsync_WhitespaceContent_ShouldReturnDefaultMessage()
        {
            // Arrange
            var content = "   \n\t   ";

            // Act
            var result = await _service.GenerateSummaryAsync(content);

            // Assert
            Assert.Equal("İçerik bulunamadı.", result);
        }

        [Fact]
        public async Task GenerateSummaryAsync_ShortContent_ShouldReturnSummary()
        {
            // Arrange
            var content = "Kısa bir test içeriği.";

            // Act
            var result = await _service.GenerateSummaryAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GenerateSummaryAsync_ContentWithMetadata_ShouldRemoveMetadata()
        {
            // Arrange
            var content = "Abstract: Bu bir özettir.\n" +
                         "Introduction: Bu bir giriştir.\n" +
                         "Author: Test Yazar\n" +
                         "Bu gerçek içeriktir. Bu cümle özetlenmelidir. " +
                         "Bu da ikinci cümledir. Bu da üçüncü cümledir.";

            // Act
            var result = await _service.GenerateSummaryAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.DoesNotContain("Abstract:", result);
            Assert.DoesNotContain("Introduction:", result);
            Assert.DoesNotContain("Author:", result);
        }

        [Fact]
        public async Task GenerateSummaryAsync_ContentWithMultipleSentences_ShouldSelectTopSentences()
        {
            // Arrange
            var content = "Birinci cümle. " +
                         "İkinci cümle yapay zeka hakkında. " +
                         "Üçüncü cümle. " +
                         "Dördüncü cümle makine öğrenmesi hakkında. " +
                         "Beşinci cümle. " +
                         "Altıncı cümle derin öğrenme hakkında.";

            // Act
            var result = await _service.GenerateSummaryAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            var sentences = result.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            Assert.True(sentences.Length <= 4); // Should select top 4 sentences
        }

        [Fact]
        public async Task ExtractKeywordsAsync_ValidContent_ShouldReturnKeywords()
        {
            // Arrange
            var content = "Bu doküman yapay zeka ve makine öğrenmesi teknolojileri hakkındadır. " +
                         "Derin öğrenme algoritmaları ve neural network'ler konu edilmektedir.";

            // Act
            var result = await _service.ExtractKeywordsAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("makine", result);
            Assert.Contains("öğrenmesi", result);
        }

        [Fact]
        public async Task ExtractKeywordsAsync_EmptyContent_ShouldReturnDefaultMessage()
        {
            // Arrange
            var content = "";

            // Act
            var result = await _service.ExtractKeywordsAsync(content);

            // Assert
            Assert.Equal("İçerik bulunamadı.", result);
        }

        [Fact]
        public async Task ExtractKeywordsAsync_ShortContent_ShouldReturnKeywords()
        {
            // Arrange
            var content = "Yapay zeka teknolojisi.";

            // Act
            var result = await _service.ExtractKeywordsAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task ExtractKeywordsAsync_TechnicalContent_ShouldExtractTechnicalTerms()
        {
            // Arrange
            var content = "Bu araştırma convolutional neural networks (CNN) ve " +
                         "recurrent neural networks (RNN) kullanarak " +
                         "natural language processing (NLP) görevlerini gerçekleştirmektedir. " +
                         "Transfer learning ve fine-tuning teknikleri uygulanmıştır.";

            // Act
            var result = await _service.ExtractKeywordsAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("neural", result);
            Assert.Contains("networks", result);
            Assert.Contains("processing", result);
        }

        [Fact]
        public async Task ExtractKeywordsAsync_ContentWithSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var content = "Bu doküman C++, Python, JavaScript ve SQL dillerini kapsar. " +
                         "API'ler ve JSON formatları kullanılmaktadır. " +
                         "HTTP/HTTPS protokolleri ve RESTful servisler konu edilmektedir.";

            // Act
            var result = await _service.ExtractKeywordsAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GenerateSummaryAsync_LongContent_ShouldReturnConciseSummary()
        {
            // Arrange
            var content = string.Join(". ", Enumerable.Range(1, 20)
                .Select(i => $"Bu {i}. cümledir ve yapay zeka teknolojileri hakkında bilgi içerir"));

            // Act
            var result = await _service.GenerateSummaryAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Length < content.Length);
            var resultSentences = result.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            Assert.True(resultSentences.Length <= 4);
        }

        [Fact]
        public async Task GenerateSummaryAsync_ContentWithNumbers_ShouldHandleCorrectly()
        {
            // Arrange
            var content = "2023 yılında yapay zeka teknolojileri %25 büyüme göstermiştir. " +
                         "2024 yılında bu büyüme %30'a çıkması beklenmektedir. " +
                         "Toplam pazar büyüklüğü 1.5 milyar dolar olmuştur.";

            // Act
            var result = await _service.GenerateSummaryAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task ExtractKeywordsAsync_ContentWithAcronyms_ShouldExtractAcronyms()
        {
            // Arrange
            var content = "Bu doküman AI (Artificial Intelligence), ML (Machine Learning), " +
                         "NLP (Natural Language Processing) ve DL (Deep Learning) konularını kapsar. " +
                         "API ve SDK kullanımları da açıklanmaktadır.";

            // Act
            var result = await _service.ExtractKeywordsAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("AI", result);
            Assert.Contains("ML", result);
            Assert.Contains("NLP", result);
        }

        [Fact]
        public async Task GenerateSummaryAsync_ContentWithQuotes_ShouldHandleCorrectly()
        {
            // Arrange
            var content = "Einstein şöyle demiştir: \"Hayal gücü bilgiden daha önemlidir.\" " +
                         "Bu söz yapay zeka araştırmalarında da geçerlidir. " +
                         "Yaratıcılık ve inovasyon çok önemlidir.";

            // Act
            var result = await _service.GenerateSummaryAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task ExtractKeywordsAsync_ContentWithTurkishCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var content = "Bu doküman Türkçe karakterler içerir: ç, ğ, ı, ö, ş, ü. " +
                         "Yapay zeka ve makine öğrenmesi konuları işlenmektedir. " +
                         "Öğrenme algoritmaları ve veri işleme teknikleri açıklanmaktadır.";

            // Act
            var result = await _service.ExtractKeywordsAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
            Assert.Contains("öğrenme", result);
        }

        [Fact]
        public async Task GenerateSummaryAsync_ContentWithBulletPoints_ShouldHandleCorrectly()
        {
            // Arrange
            var content = "Yapay zeka teknolojileri:\n" +
                         "• Makine öğrenmesi\n" +
                         "• Derin öğrenme\n" +
                         "• Doğal dil işleme\n" +
                         "Bu teknolojiler günümüzde çok önemlidir. " +
                         "Gelecekte daha da gelişeceklerdir.";

            // Act
            var result = await _service.GenerateSummaryAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task ExtractKeywordsAsync_ContentWithMixedLanguages_ShouldHandleCorrectly()
        {
            // Arrange
            var content = "Bu doküman hem Türkçe hem İngilizce terimler içerir. " +
                         "Machine learning ve deep learning konuları işlenmektedir. " +
                         "Yapay zeka ve artificial intelligence aynı anlama gelir. " +
                         "Veri bilimi ve data science eş anlamlıdır.";

            // Act
            var result = await _service.ExtractKeywordsAsync(content);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("machine", result);
            Assert.Contains("learning", result);
            Assert.Contains("yapay", result);
            Assert.Contains("zeka", result);
        }
    }
}