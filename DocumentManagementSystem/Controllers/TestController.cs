using Microsoft.AspNetCore.Mvc;
using DocumentManagementSystem.Services;

namespace DocumentManagementSystem.Controllers
{
    public class TestController : Controller
    {
        private readonly IGeminiService _geminiService;
        private readonly ILogger<TestController> _logger;

        public TestController(IGeminiService geminiService, ILogger<TestController> logger)
        {
            _geminiService = geminiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> TestGemini()
        {
            try
            {
                var testContent = "Bu bir test metnidir. Gemini API'nin çalışıp çalışmadığını kontrol etmek için kullanılıyor.";
                
                _logger.LogInformation("Gemini API test başlatılıyor...");
                
                var summary = await _geminiService.GenerateSummaryAsync(testContent);
                var keywords = await _geminiService.ExtractKeywordsAsync(testContent);
                
                var result = new
                {
                    Success = true,
                    Summary = summary,
                    Keywords = keywords,
                    Timestamp = DateTime.Now
                };
                
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini API test hatası");
                return Json(new { Success = false, Error = ex.Message });
            }
        }
    }
} 