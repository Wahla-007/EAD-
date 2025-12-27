using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Data.Entities
{
    [Table("LMSRegistrations")]
    public class LmsRegistration
    {
        [Key]
        public int LMSRegistrationId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string LMSUsername { get; set; } = string.Empty;

        [Required]
        public string LMSPassword { get; set; } = string.Empty;

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}