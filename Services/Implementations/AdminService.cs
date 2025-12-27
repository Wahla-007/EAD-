using Microsoft.EntityFrameworkCore;
using AttendanceManagementSystem.Data;
using AttendanceManagementSystem.Data.Entities;
using AttendanceManagementSystem.Services.Interfaces;
using AttendanceManagementSystem.Helpers;

namespace AttendanceManagementSystem.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly AttendanceManagementDbContext _context;

        public AdminService(AttendanceManagementDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateUserAsync(User user, string password, string roleSpecificId)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == user.Username || u.Email == user.Email);

            if (existingUser != null)
                throw new Exception("Username or email already exists");

            var (hash, salt) = PasswordHasher.HashPassword(password);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.IsFirstLogin = true;
            user.IsActive = true;
            user.CreatedDate = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create role-specific record
            if (user.Role == "Student")
            {
                var student = new Student
                {
                    UserId = user.UserId,
                    RollNumber = roleSpecificId,
                    DepartmentId = 1, // Default department
                    EnrollmentDate = DateTime.UtcNow
                };
                _context.Students.Add(student);
            }
            else if (user.Role == "Teacher")
            {
                var teacher = new Teacher
                {
                    UserId = user.UserId,
                    EmployeeId = roleSpecificId,
                    DepartmentId = 1 // Default department
                };
                _context.Teachers.Add(teacher);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignStudentToSectionAsync(int studentId, int sectionId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
                throw new Exception("Student not found");

            student.SectionId = sectionId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignTeacherToCourseAsync(int teacherId, int courseId, int sectionId, int semesterId)
        {
            var assignment = await _context.CourseAssignments
                .FirstOrDefaultAsync(ca => ca.CourseId == courseId && ca.SectionId == sectionId && ca.SemesterId == semesterId);

            if (assignment != null)
            {
                assignment.TeacherId = teacherId;
            }
            else
            {
                assignment = new CourseAssignment
                {
                    CourseId = courseId,
                    SectionId = sectionId,
                    SemesterId = semesterId,
                    TeacherId = teacherId,
                    IsActive = true
                };
                _context.CourseAssignments.Add(assignment);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateCourseAssignmentAsync(CourseAssignment assignment)
        {
            _context.CourseAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .OrderByDescending(u => u.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> DeactivateUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateLMSRegistrationAsync(LmsRegistration registration)
        {
            _context.LmsRegistrations.Add(registration);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateTeacherAsync(string fullName, string email, string employeeId, int departmentId, string password)
        {
            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
                throw new Exception("User with this email already exists");

            var existingTeacher = await _context.Teachers.FirstOrDefaultAsync(t => t.EmployeeId == employeeId);
            if (existingTeacher != null)
                throw new Exception("Teacher with this Employee ID already exists");

            // Create user
            var user = new User
            {
                Username = email.Split('@')[0],
                Email = email,
                FullName = fullName,
                Role = "Teacher",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            // Hash password
            var (hash, salt) = PasswordHasher.HashPassword(password);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create teacher
            var teacher = new Teacher
            {
                UserId = user.UserId,
                EmployeeId = employeeId,
                DepartmentId = departmentId
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CreateStudentAsync(string fullName, string email, string rollNumber, int departmentId, string password)
        {
            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
                throw new Exception("User with this email already exists");

            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.RollNumber == rollNumber);
            if (existingStudent != null)
                throw new Exception("Student with this Roll Number already exists");

            // Create user
            var user = new User
            {
                Username = email.Split('@')[0],
                Email = email,
                FullName = fullName,
                Role = "Student",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            // Hash password
            var (hash, salt) = PasswordHasher.HashPassword(password);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create student
            var student = new Student
            {
                UserId = user.UserId,
                RollNumber = rollNumber,
                DepartmentId = departmentId,
                EnrollmentDate = DateTime.UtcNow
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}