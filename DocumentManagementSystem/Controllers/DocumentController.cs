using Microsoft.AspNetCore.Mvc;
using DocumentManagementSystem.Services;
using DocumentManagementSystem.ViewModels;
using DocumentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DocumentManagementSystem.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(IDocumentService documentService, ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        // Ana sayfa - Doküman listesi
        public async Task<IActionResult> Index()
        {
            try
            {
                var documents = await _documentService.GetAllDocumentsAsync();
                return View(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Doküman listesi alınırken hata oluştu");
                TempData["Error"] = "Dokümanlar yüklenirken bir hata oluştu.";
                return View(new List<Document>());
            }
        }

        // Doküman yükleme sayfası
        public IActionResult Upload()
        {
            return View();
        }

        // Doküman yükleme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(DocumentUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Auth");
                }

                var document = await _documentService.UploadDocumentAsync(model, username);

                TempData["Success"] = $"'{document.Title}' başarıyla yüklendi ve işleniyor.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Doküman yükleme hatası");
                ModelState.AddModelError("", "Doküman yüklenirken bir hata oluştu.");
                return View(model);
            }
        }

        // Doküman detay sayfası
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var document = await _documentService.GetDocumentByIdAsync(id);
                if (document == null)
                {
                    TempData["Error"] = "Doküman bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                return View(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Doküman detayları alınırken hata oluştu: {DocumentId}", id);
                TempData["Error"] = "Doküman detayları yüklenirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Doküman düzenleme sayfası
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var document = await _documentService.GetDocumentByIdAsync(id);
                if (document == null)
                {
                    TempData["Error"] = "Doküman bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                // Kullanıcının sadece kendi yüklediği dokümanları düzenleyebilmesi
                var currentUser = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
                if (document.UploadedBy != currentUser)
                {
                    _logger.LogWarning("Yetkisiz doküman düzenleme denemesi: {DocumentId}, User: {Username}, Owner: {Owner}", 
                        id, currentUser, document.UploadedBy);
                    TempData["Error"] = "Bu dokümanı sadece yükleyen kişi düzenleyebilir.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                var viewModel = new DocumentViewModel
                {
                    Id = document.Id,
                    Title = document.Title,
                    FileName = document.FileName,
                    FileType = document.FileType,
                    FileSize = document.FileSize,
                    Content = document.Content,
                    Summary = document.Summary,
                    Keywords = document.Keywords,
                    UploadDate = document.UploadDate,
                    UploadedBy = document.UploadedBy,
                    IsProcessed = document.IsProcessed,
                    ProcessingStatus = document.ProcessingStatus
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Doküman düzenleme sayfası yüklenirken hata oluştu: {DocumentId}", id);
                TempData["Error"] = "Doküman düzenleme sayfası yüklenirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Doküman düzenleme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocumentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kullanıcının sadece kendi yüklediği dokümanları düzenleyebilmesi
                var document = await _documentService.GetDocumentByIdAsync(id);
                if (document == null)
                {
                    TempData["Error"] = "Doküman bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                var currentUser = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
                if (document.UploadedBy != currentUser)
                {
                    _logger.LogWarning("Yetkisiz doküman düzenleme denemesi: {DocumentId}, User: {Username}, Owner: {Owner}", 
                        id, currentUser, document.UploadedBy);
                    TempData["Error"] = "Bu dokümanı sadece yükleyen kişi düzenleyebilir.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                var updatedDocument = await _documentService.UpdateDocumentAsync(id, model);
                TempData["Success"] = $"'{updatedDocument.Title}' başarıyla güncellendi.";
                return RedirectToAction(nameof(Details), new { id = updatedDocument.Id });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Doküman güncelleme hatası: {DocumentId}", id);
                ModelState.AddModelError("", "Doküman güncellenirken bir hata oluştu.");
                return View(model);
            }
        }

        // Doküman silme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login", "Auth");
                }

                var success = await _documentService.DeleteDocumentAsync(id, username);
                if (success)
                {
                    TempData["Success"] = "Doküman başarıyla silindi.";
                }
                else
                {
                    TempData["Error"] = "Doküman bulunamadı.";
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Yetkisiz dosya silme denemesi: {DocumentId}, User: {Username}", id, User.FindFirst(ClaimTypes.Name)?.Value);
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Doküman silme hatası: {DocumentId}", id);
                TempData["Error"] = "Doküman silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Arama sayfası
        public IActionResult Search()
        {
            return View();
        }

        // Arama işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(DocumentSearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var results = await _documentService.SearchDocumentsAsync(model);
                ViewBag.SearchTerm = model.SearchTerm;
                return View("SearchResults", results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Arama hatası");
                TempData["Error"] = "Arama yapılırken bir hata oluştu.";
                return View(model);
            }
        }

        // Arama sonuçları sayfası
        public async Task<IActionResult> SearchResults(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return RedirectToAction(nameof(Search));
            }

            try
            {
                var model = new DocumentSearchViewModel
                {
                    SearchTerm = searchTerm,
                    Page = 1,
                    PageSize = 10
                };

                var results = await _documentService.SearchDocumentsAsync(model);
                ViewBag.SearchTerm = searchTerm;
                return View(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Arama sonuçları alınırken hata oluştu");
                TempData["Error"] = "Arama sonuçları yüklenirken bir hata oluştu.";
                return RedirectToAction(nameof(Search));
            }
        }

        // Doküman yeniden işleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reprocess(int id)
        {
            try
            {
                var success = await _documentService.ProcessDocumentAsync(id);
                if (success)
                {
                    TempData["Success"] = "Doküman yeniden işleniyor.";
                }
                else
                {
                    TempData["Error"] = "Doküman işlenirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Doküman yeniden işleme hatası: {DocumentId}", id);
                TempData["Error"] = "Doküman yeniden işlenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}