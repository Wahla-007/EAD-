using Microsoft.EntityFrameworkCore;
using AttendanceManagementSystem.Data;
using AttendanceManagementSystem.Data.Entities;
using AttendanceManagementSystem.Services.Interfaces;

namespace AttendanceManagementSystem.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly AttendanceManagementDbContext _context;

        public StudentService(AttendanceManagementDbContext context)
        {
            _context = context;
        }

        public async Task<Student?> GetStudentByIdAsync(int studentId)
        {
            return await _context.Students
                .Include(s => s.User)
                .Include(s => s.Department)
                .Include(s => s.Section)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }

        public async Task<Student?> GetStudentByUserIdAsync(int userId)
        {
            return await _context.Students
                .Include(s => s.User)
                .Include(s => s.Department)
                .Include(s => s.Section)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.User)
                .Include(s => s.Department)
                .Include(s => s.Section)
                .Where(s => s.User!.IsActive)
                .ToListAsync();
        }

        public async Task<List<StudentCourseRegistration>> GetStudentCoursesAsync(int studentId)
        {
            return await _context.StudentCourseRegistrations
                .Include(r => r.Assignment)
                    .ThenInclude(a => a!.Course)
                .Include(r => r.Assignment)
                    .ThenInclude(a => a!.Teacher)
                        .ThenInclude(t => t!.User)
                .Include(r => r.Assignment)
                    .ThenInclude(a => a!.Section)
                .Where(r => r.StudentId == studentId && r.Status == "Registered")
                .ToListAsync();
        }

        public async Task<bool> RegisterForCourseAsync(int studentId, int assignmentId)
        {
            var existingRegistration = await _context.StudentCourseRegistrations
                .FirstOrDefaultAsync(r => r.StudentId == studentId && r.AssignmentId == assignmentId);

            if (existingRegistration != null)
                throw new Exception("Already registered for this course");

            var registration = new StudentCourseRegistration
            {
                StudentId = studentId,
                AssignmentId = assignmentId,
                RegistrationDate = DateTime.UtcNow,
                Status = "Registered"
            };

            _context.StudentCourseRegistrations.Add(registration);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<string, decimal>> GetAttendanceStatsAsync(int studentId)
        {
            var registrations = await _context.StudentCourseRegistrations
                .Include(r => r.Attendances)
                .Include(r => r.Assignment)
                    .ThenInclude(a => a!.Course)
                .Where(r => r.StudentId == studentId && r.Status == "Registered")
                .ToListAsync();

            var stats = new Dictionary<string, decimal>();

            foreach (var reg in registrations)
            {
                var totalClasses = reg.Attendances.Count;
                var presentClasses = reg.Attendances.Count(a => a.Status == "Present");
                var percentage = totalClasses > 0 ? (decimal)presentClasses / totalClasses * 100 : 0;

                stats[reg.Assignment?.Course?.CourseName ?? "Unknown"] = Math.Round(percentage, 2);
            }

            return stats;
        }
    }
}