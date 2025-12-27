using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Data.Entities
{
    [Table("Sections")]
    public class Section
    {
        [Key]
        public int SectionId { get; set; }

        [Required]
        [StringLength(50)]
        public string SectionName { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int SemesterId { get; set; }

        [Required]
        public int Capacity { get; set; } = 50;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        [ForeignKey("SemesterId")]
        public virtual Semester? Semester { get; set; }

        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
        public virtual ICollection<CourseAssignment> CourseAssignments { get; set; } = new List<CourseAssignment>();
    }
}