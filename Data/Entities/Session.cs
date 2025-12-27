using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Data.Entities
{
    [Table("Sessions")]
    public class Session
    {
        [Key]
        public int SessionId { get; set; }

        [Required]
        [StringLength(50)]
        public string SessionName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Semester> Semesters { get; set; } = new List<Semester>();
    }
}