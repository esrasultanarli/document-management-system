using System.ComponentModel.DataAnnotations;

namespace DocumentManagementSystem.Models
{
    public class Document
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        public string FileType { get; set; } = string.Empty; // pdf, txt, docx

        public long FileSize { get; set; }

        public string Content { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public string Keywords { get; set; } = string.Empty;

        public DateTime UploadDate { get; set; } = DateTime.Now;

        public DateTime LastModified { get; set; } = DateTime.Now;

        public string UploadedBy { get; set; } = string.Empty;

        public bool IsProcessed { get; set; } = false;

        public string ProcessingStatus { get; set; } = "Pending"; // Pending, Processing, Completed, Failed
    }
}