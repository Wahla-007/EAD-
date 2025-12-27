using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Data.Entities
{
    [Table("StudentCourseRegistrations")]
    public class StudentCourseRegistration
    {
        [Key]
        public int RegistrationId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Registered";

        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        [ForeignKey("AssignmentId")]
        public virtual CourseAssignment? Assignment { get; set; }

        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}