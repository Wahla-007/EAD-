using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Data.Entities
{
    [Table("Semesters")]
    public class Semester
    {
        [Key]
        public int SemesterId { get; set; }

        [Required]
        public int SessionId { get; set; }

        [Required]
        [StringLength(20)]
        public string SemesterName { get; set; } = string.Empty;

        [Required]
        public int SemesterNumber { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("SessionId")]
        public virtual Session? Session { get; set; }
        public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
        public virtual ICollection<CourseAssignment> CourseAssignments { get; set; } = new List<CourseAssignment>();
    }
}