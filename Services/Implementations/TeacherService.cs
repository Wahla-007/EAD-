using Microsoft.EntityFrameworkCore;
using AttendanceManagementSystem.Data;
using AttendanceManagementSystem.Data.Entities;
using AttendanceManagementSystem.Services.Interfaces;

namespace AttendanceManagementSystem.Services.Implementations
{
    public class TeacherService : ITeacherService
    {
        private readonly AttendanceManagementDbContext _context;

        public TeacherService(AttendanceManagementDbContext context)
        {
            _context = context;
        }

        public async Task<Teacher?> GetTeacherByIdAsync(int teacherId)
        {
            return await _context.Teachers
                .Include(t => t.User)
                .Include(t => t.Department)
                .FirstOrDefaultAsync(t => t.TeacherId == teacherId);
        }

        public async Task<Teacher?> GetTeacherByUserIdAsync(int userId)
        {
            return await _context.Teachers
                .Include(t => t.User)
                .Include(t => t.Department)
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<List<CourseAssignment>> GetTeacherCoursesAsync(int teacherId)
        {
            return await _context.CourseAssignments
                .Include(ca => ca.Course)
                .Include(ca => ca.Section)
                .Include(ca => ca.Semester)
                .Where(ca => ca.TeacherId == teacherId && ca.IsActive)
                .ToListAsync();
        }

        public async Task<List<Student>> GetCourseStudentsAsync(int assignmentId)
        {
            return await _context.StudentCourseRegistrations
                .Include(r => r.Student)
                    .ThenInclude(s => s!.User)
                .Where(r => r.AssignmentId == assignmentId && r.Status == "Registered")
                .Select(r => r.Student!)
                .ToListAsync();
        }

        public async Task<bool> MarkAttendanceAsync(int registrationId, DateTime date, string status, int markedBy, string? remarks = null)
        {
            var existingAttendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.RegistrationId == registrationId && a.AttendanceDate.Date == date.Date);

            if (existingAttendance != null)
            {
                existingAttendance.Status = status;
                existingAttendance.MarkedBy = markedBy;
                existingAttendance.MarkedDate = DateTime.UtcNow;
                existingAttendance.Remarks = remarks;
            }
            else
            {
                var attendance = new Attendance
                {
                    RegistrationId = registrationId,
                    AttendanceDate = date.Date,
                    Status = status,
                    MarkedBy = markedBy,
                    MarkedDate = DateTime.UtcNow,
                    Remarks = remarks
                };

                _context.Attendances.Add(attendance);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Attendance>> GetCourseAttendanceAsync(int assignmentId, DateTime date)
        {
            return await _context.Attendances
                .Include(a => a.Registration)
                    .ThenInclude(r => r!.Student)
                        .ThenInclude(s => s!.User)
                .Where(a => a.Registration!.AssignmentId == assignmentId && a.AttendanceDate.Date == date.Date)
                .ToListAsync();
        }
    }
}