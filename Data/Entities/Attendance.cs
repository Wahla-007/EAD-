using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Data.Entities
{
    [Table("Attendance")]
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [Required]
        public int RegistrationId { get; set; }

        [Required]
        public DateTime AttendanceDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Absent";

        [Required]
        public int MarkedBy { get; set; }

        public DateTime MarkedDate { get; set; } = DateTime.Now;

        public string? Remarks { get; set; }

        // Navigation properties
        [ForeignKey("RegistrationId")]
        public virtual StudentCourseRegistration? Registration { get; set; }

        [ForeignKey("MarkedBy")]
        public virtual User? MarkedByUser { get; set; }
    }
}