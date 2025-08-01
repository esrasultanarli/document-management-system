using System.ComponentModel.DataAnnotations;

namespace DocumentManagementSystem.ViewModels
{
    public class DocumentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık gereklidir")]
        [StringLength(255, ErrorMessage = "Başlık en fazla 255 karakter olabilir")]
        public string Title { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public bool IsProcessed { get; set; }
        public string ProcessingStatus { get; set; } = string.Empty;
    }

    public class DocumentUploadViewModel
    {
        [Required(ErrorMessage = "Lütfen bir dosya seçin")]
        public IFormFile? File { get; set; }

        [Required(ErrorMessage = "Başlık gereklidir")]
        [StringLength(255, ErrorMessage = "Başlık en fazla 255 karakter olabilir")]
        public string Title { get; set; } = string.Empty;
    }

    public class DocumentSearchViewModel
    {
        [Required(ErrorMessage = "Arama terimi gereklidir")]
        public string SearchTerm { get; set; } = string.Empty;

        public string? FileType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? UploadedBy { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}