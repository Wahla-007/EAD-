using AttendanceManagementSystem.Data.Entities;

namespace AttendanceManagementSystem.Services.Interfaces
{
    public interface IStudentService
    {
        Task<Student?> GetStudentByIdAsync(int studentId);
        Task<Student?> GetStudentByUserIdAsync(int userId);
        Task<List<Student>> GetAllStudentsAsync();
        Task<List<StudentCourseRegistration>> GetStudentCoursesAsync(int studentId);
        Task<bool> RegisterForCourseAsync(int studentId, int assignmentId);
        Task<Dictionary<string, decimal>> GetAttendanceStatsAsync(int studentId);
    }
}