using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Data.Entities
{
    [Table("CourseAssignments")]
    public class CourseAssignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required]
        public int SemesterId { get; set; }

        public int? TeacherId { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }

        [ForeignKey("SectionId")]
        public virtual Section? Section { get; set; }

        [ForeignKey("SemesterId")]
        public virtual Semester? Semester { get; set; }

        [ForeignKey("TeacherId")]
        public virtual Teacher? Teacher { get; set; }

        public virtual ICollection<StudentCourseRegistration> StudentCourseRegistrations { get; set; } = new List<StudentCourseRegistration>();
        public virtual ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
    }
}