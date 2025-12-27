using AttendanceManagementSystem.Data.Entities;

namespace AttendanceManagementSystem.Services.Interfaces
{
    public interface ITeacherService
    {
        Task<Teacher?> GetTeacherByIdAsync(int teacherId);
        Task<Teacher?> GetTeacherByUserIdAsync(int userId);
        Task<List<CourseAssignment>> GetTeacherCoursesAsync(int teacherId);
        Task<List<Student>> GetCourseStudentsAsync(int assignmentId);
        Task<bool> MarkAttendanceAsync(int registrationId, DateTime date, string status, int markedBy, string? remarks = null);
        Task<List<Attendance>> GetCourseAttendanceAsync(int assignmentId, DateTime date);
    }
}