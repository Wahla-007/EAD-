using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Data.Entities
{
    [Table("Departments")]
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string DepartmentCode { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
        public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
        public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
    }
}