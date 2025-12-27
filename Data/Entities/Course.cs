using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Data.Entities
{
    [Table("Courses")]
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [StringLength(20)]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        public int CreditHours { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }
        public virtual ICollection<CourseAssignment> CourseAssignments { get; set; } = new List<CourseAssignment>();
    }
}