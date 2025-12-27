using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Data.Entities
{
    [Table("Timetable")]
    public class Timetable
    {
        [Key]
        public int TimetableId { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        [Required]
        [Range(1, 7)]
        public int DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [StringLength(50)]
        public string? RoomNumber { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("AssignmentId")]
        public virtual CourseAssignment? Assignment { get; set; }
    }
}