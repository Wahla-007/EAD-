using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Data.Entities
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public int LogId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [StringLength(50)]
        public string? TableName { get; set; }

        public int? RecordId { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        [StringLength(50)]
        public string? IPAddress { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}