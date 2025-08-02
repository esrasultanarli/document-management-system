using System.Text;
using System.Text.Json;

namespace DocumentManagementSystem.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly ILogger<GeminiService> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";

        public GeminiService(ILogger<GeminiService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _apiKey = configuration["GeminiApiKey"] ?? throw new InvalidOperationException("GeminiApiKey configuration is missing. Please set it in appsettings.json or environment variables.");
        }

        public async Task<string> GenerateSummaryAsync(string content)
        {
            try
            {
                _logger.LogInformation("GenerateSummaryAsync başlatıldı. API Key: {ApiKey}", _apiKey.Substring(0, 10) + "...");

                if (string.IsNullOrWhiteSpace(content))
                    return "İçerik bulunamadı.";

                // Akıllı özet algoritması - metadata kısmını atlar
                var cleanContent = RemoveMetadataSection(content);
                var sentences = ExtractSentences(cleanContent);

                if (sentences.Count == 0)
                {
                    _logger.LogWarning("Temizlenmiş içerikte cümle bulunamadı, orijinal içerik kullanılıyor");
                    sentences = ExtractSentences(content);
                }

                // Cümleleri skorla
                var scoredSentences = ScoreSentences(sentences);

                // En yüksek skorlu cümleleri seç (3-5 cümle)
                var topSentences = scoredSentences
                    .OrderByDescending(s => s.Score)
                    .Take(4)
                    .OrderBy(s => s.OriginalIndex) // Orijinal sırayı koru
                    .Select(s => s.Sentence)
                    .ToList();

                var summary = string.Join(". ", topSentences);
                if (!summary.EndsWith(".") && !summary.EndsWith("!") && !summary.EndsWith("?"))
                    summary += ".";

                _logger.LogInformation("Akıllı özet algoritması kullanılarak özet oluşturuldu. {SentenceCount} cümle seçildi", topSentences.Count);
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Özet oluşturulurken hata oluştu");
                return "Özet oluşturulurken hata oluştu.";
            }
        }

        private string RemoveMetadataSection(string content)
        {
            // Metadata göstergeleri
            var metadataKeywords = new[]
            {
                "abstract", "özet", "summary", "abstract:", "özet:", "summary:",
                "introduction", "giriş", "introduction:", "giriş:",
                "author", "yazar", "authors", "yazarlar",
                "published", "yayınlanmış", "publication", "yayın",
                "journal", "dergi", "conference", "konferans",
                "university", "üniversite", "department", "bölüm",
                "doi:", "doi", "issn:", "issn", "isbn:", "isbn",
                "received", "alınan", "accepted", "kabul edilen",
                "keywords:", "anahtar kelimeler:", "keywords", "anahtar kelimeler"
            };

            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var contentLines = new List<string>();
            var foundContent = false;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmedLine))
                    continue;

                // Metadata kontrolü
                var isMetadata = metadataKeywords.Any(keyword =>
                    trimmedLine.ToLowerInvariant().Contains(keyword.ToLowerInvariant()));

                // Eğer metadata bulunduysa ve henüz içerik başlamadıysa, bu satırı atla
                if (isMetadata && !foundContent)
                    continue;

                // İçerik başladı, artık tüm satırları al
                foundContent = true;
                contentLines.Add(trimmedLine);
            }

            return string.Join("\n", contentLines);
        }

        private List<string> ExtractSentences(string content)
        {
            // Cümle ayırma - daha gelişmiş
            var sentences = new List<string>();
            var currentSentence = new System.Text.StringBuilder();
            var inQuotes = false;
            var quoteCount = 0;

            for (int i = 0; i < content.Length; i++)
            {
                var c = content[i];
                currentSentence.Append(c);

                if (c == '"' || c == '"' || c == '"' || c == '"')
                {
                    inQuotes = !inQuotes;
                    quoteCount++;
                }

                // Cümle sonu kontrolü
                if ((c == '.' || c == '!' || c == '?') && !inQuotes)
                {
                    var sentence = currentSentence.ToString().Trim();
                    if (sentence.Length > 20) // Çok kısa cümleleri atla
                    {
                        sentences.Add(sentence);
                    }
                    currentSentence.Clear();
                }
            }

            // Son cümleyi ekle
            var lastSentence = currentSentence.ToString().Trim();
            if (lastSentence.Length > 20)
            {
                sentences.Add(lastSentence);
            }

            return sentences;
        }

        private List<SentenceScore> ScoreSentences(List<string> sentences)
        {
            var scoredSentences = new List<SentenceScore>();
            var allWords = sentences.SelectMany(s =>
                s.Split(new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ':', ';', '(', ')', '[', ']', '"', '"' },
                StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 2)
                .Select(w => w.ToLowerInvariant()))
                .ToList();

            // Kelime frekansı
            var wordFrequency = allWords
                .GroupBy(w => w)
                .ToDictionary(g => g.Key, g => g.Count());

            // Stop words
            var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ve", "veya", "ile", "için", "bu", "bir", "da", "de", "mi", "mu", "mü",
                "the", "and", "or", "for", "this", "that", "with", "in", "on", "at", "to", "of", "a", "an",
                "is", "are", "was", "were", "be", "been", "have", "has", "had", "do", "does", "did",
                "will", "would", "could", "should", "may", "might", "can", "must", "shall"
            };

            for (int i = 0; i < sentences.Count; i++)
            {
                var sentence = sentences[i];
                var words = sentence.Split(new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ':', ';', '(', ')', '[', ']', '"', '"' },
                    StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 2)
                    .Select(w => w.ToLowerInvariant())
                    .ToList();

                var score = 0.0;

                // Kelime frekansı skoru
                foreach (var word in words)
                {
                    if (!stopWords.Contains(word) && wordFrequency.ContainsKey(word))
                    {
                        score += wordFrequency[word];
                    }
                }

                // Cümle uzunluğu bonusu (çok kısa veya çok uzun cümleleri cezalandır)
                var wordCount = words.Count;
                if (wordCount >= 8 && wordCount <= 25)
                    score *= 1.2;
                else if (wordCount < 5 || wordCount > 40)
                    score *= 0.5;

                // Pozisyon bonusu (makalenin ortasındaki cümleler daha önemli)
                var positionRatio = (double)i / sentences.Count;
                if (positionRatio >= 0.2 && positionRatio <= 0.8)
                    score *= 1.1;

                // Özel kelime bonusu
                var importantWords = new[] { "sonuç", "bulgu", "araştırma", "çalışma", "yöntem", "veri", "analiz",
                    "result", "finding", "research", "study", "method", "data", "analysis", "conclusion", "summary" };

                foreach (var importantWord in importantWords)
                {
                    if (sentence.ToLowerInvariant().Contains(importantWord))
                    {
                        score *= 1.3;
                        break;
                    }
                }

                scoredSentences.Add(new SentenceScore
                {
                    Sentence = sentence,
                    Score = score,
                    OriginalIndex = i
                });
            }

            return scoredSentences;
        }

        private class SentenceScore
        {
            public string Sentence { get; set; } = "";
            public double Score { get; set; }
            public int OriginalIndex { get; set; }
        }

        public async Task<string> ExtractKeywordsAsync(string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                    return "İçerik bulunamadı.";

                // Geçici olarak basit anahtar kelime çıkarma algoritması kullanıyoruz
                // API key sorunu çözülene kadar bu kullanılacak
                var words = content.Split(new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

                // Stop words'leri filtrele
                var stopWords = new[] { "ve", "veya", "ile", "için", "bu", "bir", "da", "de", "mi", "mu", "mü", "the", "and", "or", "for", "this", "that", "with", "in", "on", "at", "to", "of", "a", "an", "is", "are", "was", "were", "be", "been", "have", "has", "had", "do", "does", "did", "will", "would", "could", "should" };

                var keywords = words
                    .Where(w => w.Length > 3 && !stopWords.Contains(w.ToLowerInvariant()))
                    .GroupBy(w => w.ToLowerInvariant())
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .Select(g => g.Key)
                    .ToList();

                var result = string.Join(", ", keywords);

                _logger.LogInformation("Basit anahtar kelime algoritması kullanılarak anahtar kelimeler çıkarıldı");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Anahtar kelimeler çıkarılırken hata oluştu");
                return "Anahtar kelimeler çıkarılırken hata oluştu.";
            }
        }
    }

    // Gemini API Response Models
    public class GeminiResponse
    {
        public Candidate[]? candidates { get; set; }
    }

    public class Candidate
    {
        public Content? content { get; set; }
    }

    public class Content
    {
        public Part[]? parts { get; set; }
    }

    public class Part
    {
        public string? text { get; set; }
    }
}