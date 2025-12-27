using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Data.Entities
{
    [Table("RefreshTokens")]
    public class RefreshToken
    {
        [Key]
        public int TokenId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiryDate { get; set; }

        public bool IsRevoked { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}