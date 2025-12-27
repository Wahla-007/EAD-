using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Data.Entities
{
    [Table("Teachers")]
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [StringLength(50)]
        public string? Designation { get; set; }

        [StringLength(100)]
        public string? Qualification { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        public virtual ICollection<CourseAssignment> CourseAssignments { get; set; } = new List<CourseAssignment>();
    }
}