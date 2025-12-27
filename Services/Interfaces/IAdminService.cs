using AttendanceManagementSystem.Data.Entities;

namespace AttendanceManagementSystem.Services.Interfaces
{
    public interface IAdminService
    {
        Task<bool> CreateUserAsync(User user, string password, string roleSpecificId);
        Task<bool> AssignStudentToSectionAsync(int studentId, int sectionId);
        Task<bool> AssignTeacherToCourseAsync(int teacherId, int courseId, int sectionId, int semesterId);
        Task<bool> CreateCourseAssignmentAsync(CourseAssignment assignment);
        Task<List<User>> GetAllUsersAsync();
        Task<bool> DeactivateUserAsync(int userId);
        Task<bool> CreateLMSRegistrationAsync(LmsRegistration registration);
        Task<bool> CreateTeacherAsync(string fullName, string email, string employeeId, int departmentId, string password);
        Task<bool> CreateStudentAsync(string fullName, string email, string rollNumber, int departmentId, string password);
    }
}