using System.ComponentModel.DataAnnotations;

namespace DocumentManagementSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime LastLoginDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public string Role { get; set; } = "User"; // Admin, User

        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}