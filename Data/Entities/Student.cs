using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Data.Entities
{
    [Table("Students")]
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string RollNumber { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        public int? SectionId { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(3,2)")]
        public decimal? CGPA { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        [ForeignKey("SectionId")]
        public virtual Section? Section { get; set; }

        public virtual ICollection<StudentCourseRegistration> StudentCourseRegistrations { get; set; } = new List<StudentCourseRegistration>();
    }
}