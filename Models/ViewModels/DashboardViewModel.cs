namespace AttendanceManagementSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalSections { get; set; }
        public decimal AttendancePercentage { get; set; }
        
        // Student-specific stats
        public int EnrolledCoursesCount { get; set; }
        public int ClassesPerWeek { get; set; }
        public string AcademicStatus { get; set; } = "N/A";
        
        public List<RecentActivity> RecentActivities { get; set; } = new();
        public List<NotificationDto> Notifications { get; set; } = new();
    }

    public class RecentActivity
    {
        public string Activity { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
        public string NotificationType { get; set; } = string.Empty;
    }
}