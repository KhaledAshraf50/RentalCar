using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class ContactMessage
    {
        [Key]
        public int MessageId { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
    }
}
