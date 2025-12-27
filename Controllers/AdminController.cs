using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendanceManagementSystem.Services.Interfaces;
using AttendanceManagementSystem.Models.ViewModels;
using System.Security.Claims;
using AttendanceManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using AttendanceManagementSystem.Data.Entities;

namespace AttendanceManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IAuthService _authService;
        private readonly IStudentService _studentService;
        private readonly AttendanceManagementDbContext _context;

        public AdminController(IAdminService adminService, IAuthService authService, 
            IStudentService studentService, AttendanceManagementDbContext context)
        {
            _adminService = adminService;
            _authService = authService;
            _studentService = studentService;
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var username = User.FindFirstValue(ClaimTypes.Name);

            var viewModel = new DashboardViewModel
            {
                UserId = userId,
                Username = username ?? "Admin",
                Role = "Admin",
                FullName = User.FindFirstValue(ClaimTypes.GivenName) ?? "Administrator"
            };

            return View(viewModel);
        }

        public IActionResult Users()
        {
            return View();
        }

        // Students Management
        public async Task<IActionResult> Students()
        {
            var students = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Department)
                .Include(s => s.Section)
                    .ThenInclude(sec => sec.Semester)
                        .ThenInclude(sem => sem.Session)
                .OrderBy(s => s.RollNumber)
                .ToListAsync();
            
            return View("ManageStudents", students);
        }

        // API endpoint to get sections for dropdown
        [HttpGet]
        public async Task<IActionResult> GetSections()
        {
            var sections = await _context.Sections
                .Include(s => s.Semester)
                    .ThenInclude(sem => sem.Session)
                .Select(s => new {
                    s.SectionId,
                    SectionName = s.SectionName,
                    SessionName = s.Semester.Session.SessionName
                })
                .ToListAsync();
            
            return Json(sections);
        }

        // Assign student to section
        [HttpPost]
        public async Task<IActionResult> AssignStudentToSection(int studentId, int sectionId)
        {
            try
            {
                await _adminService.AssignStudentToSectionAsync(studentId, sectionId);
                TempData["Success"] = "Student assigned to section successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Students");
        }

        // Create new student
        [HttpPost]
        public async Task<IActionResult> CreateStudent(string fullName, string email, string rollNumber, int departmentId, string password)
        {
            try
            {
                await _adminService.CreateStudentAsync(fullName, email, rollNumber, departmentId, password);
                TempData["Success"] = "Student created successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Students");
        }

        // Teachers Management
        public async Task<IActionResult> Teachers()
        {
            var teachers = await _context.Teachers
                .Include(t => t.User)
                .Include(t => t.Department)
                .Include(t => t.CourseAssignments)
                    .ThenInclude(ca => ca.Course)
                .OrderBy(t => t.EmployeeId)
                .ToListAsync();
            
            return View("ManageTeachers", teachers);
        }

        // Get available courses for assignment
        [HttpGet]
        public async Task<IActionResult> GetCoursesForAssignment()
        {
            var courses = await _context.Courses
                .Select(c => new {
                    c.CourseId,
                    c.CourseCode,
                    CourseName = c.CourseName
                })
                .ToListAsync();
            
            return Json(courses);
        }

        // Get sections with semester info
        [HttpGet]
        public async Task<IActionResult> GetSectionsWithSemester()
        {
            var sections = await _context.Sections
                .Include(s => s.Semester)
                    .ThenInclude(sem => sem.Session)
                .Include(s => s.Department)
                .Select(s => new {
                    s.SectionId,
                    SectionName = s.SectionName,
                    SessionName = s.Semester.Session.SessionName,
                    SemesterName = s.Semester.SemesterName,
                    s.SemesterId
                })
                .ToListAsync();
            
            return Json(sections);
        }

        // Assign teacher to course
        [HttpPost]
        public async Task<IActionResult> AssignTeacherToCourse(int teacherId, int courseId, int sectionId, int semesterId)
        {
            try
            {
                var assignment = new CourseAssignment
                {
                    TeacherId = teacherId,
                    CourseId = courseId,
                    SectionId = sectionId,
                    SemesterId = semesterId,
                    IsActive = true
                };
                
                await _adminService.CreateCourseAssignmentAsync(assignment);
                TempData["Success"] = "Teacher assigned to course successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Teachers");
        }

        // Create new teacher
        [HttpPost]
        public async Task<IActionResult> CreateTeacher(string fullName, string email, string employeeId, int departmentId, string password)
        {
            try
            {
                await _adminService.CreateTeacherAsync(fullName, email, employeeId, departmentId, password);
                TempData["Success"] = "Teacher created successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Teachers");
        }

        // Get departments for dropdown
        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _context.Departments
                .Select(d => new {
                    d.DepartmentId,
                    d.DepartmentName
                })
                .ToListAsync();
            return Json(departments);
        }

        // Course Assignments (Assign courses to students)
        public async Task<IActionResult> CourseAssignments()
        {
            var assignments = await _context.CourseAssignments
                .Include(ca => ca.Course)
                .Include(ca => ca.Teacher)
                    .ThenInclude(t => t.User)
                .Include(ca => ca.Section)
                .Include(ca => ca.Semester)
                .Where(ca => ca.IsActive)
                .OrderBy(ca => ca.Section.SectionName)
                .ThenBy(ca => ca.Course.CourseCode)
                .ToListAsync();
            
            return View(assignments);
        }

        // LMS Registration Management
        public async Task<IActionResult> LMSRegistrations()
        {
            var registrations = await _context.LmsRegistrations
                .Include(l => l.User)
                .OrderByDescending(l => l.RegistrationDate)
                .ToListAsync();
            
            return View(registrations);
        }

        // Create LMS Registration
        [HttpPost]
        public async Task<IActionResult> CreateLMSRegistration(int userId, string lmsUsername, string lmsPassword)
        {
            try
            {
                var registration = new LmsRegistration
                {
                    UserId = userId,
                    LMSUsername = lmsUsername,
                    LMSPassword = lmsPassword,
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                };
                
                await _adminService.CreateLMSRegistrationAsync(registration);
                TempData["Success"] = "LMS registration created successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("LMSRegistrations");
        }

        // Get all users for LMS registration
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .Select(u => new {
                    u.UserId,
                    u.FullName,
                    u.Email,
                    u.Role
                })
                .ToListAsync();
            
            return Json(users);
        }

        public async Task<IActionResult> Departments()
        {
            var departments = await _context.Departments
                .OrderBy(d => d.DepartmentCode)
                .ToListAsync();
            return View(departments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment(string departmentCode, string departmentName)
        {
            try
            {
                var dept = new Department
                {
                    DepartmentCode = departmentCode,
                    DepartmentName = departmentName,
                    IsActive = true
                };
                _context.Departments.Add(dept);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Department created successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Departments");
        }

        public async Task<IActionResult> Courses()
        {
            var courses = await _context.Courses
                .Include(c => c.Department)
                .OrderBy(c => c.CourseCode)
                .ToListAsync();
            return View(courses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse(string courseCode, string courseName, int creditHours, int departmentId)
        {
            try
            {
                var course = new Course
                {
                    CourseCode = courseCode,
                    CourseName = courseName,
                    CreditHours = creditHours,
                    DepartmentId = departmentId,
                    IsActive = true
                };
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Course created successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Courses");
        }

        public IActionResult Sections()
        {
            return View();
        }

        public async Task<IActionResult> Attendance()
        {
            // Overall attendance statistics
            var attendanceData = await _context.Attendances
                .Include(a => a.Registration)
                    .ThenInclude(r => r.Student)
                    .ThenInclude(s => s.User)
                .Include(a => a.Registration)
                    .ThenInclude(r => r.Assignment)
                    .ThenInclude(a => a.Course)
                .OrderByDescending(a => a.AttendanceDate)
                .Take(100)
                .ToListAsync();

            return View(attendanceData);
        }

        public IActionResult Reports()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAdminReports()
        {
            var totalStudents = await _context.Students.CountAsync();
            var totalTeachers = await _context.Teachers.CountAsync();
            var totalCourses = await _context.Courses.Where(c => c.IsActive).CountAsync();
            var totalDepartments = await _context.Departments.Where(d => d.IsActive).CountAsync();

            var totalAttendanceRecords = await _context.Attendances.CountAsync();
            var presentCount = await _context.Attendances.Where(a => a.Status == "Present").CountAsync();
            var avgAttendance = totalAttendanceRecords > 0 ? (presentCount * 100.0 / totalAttendanceRecords) : 0;

            var activeStudents = await _context.Students.Include(s => s.User).Where(s => s.User.IsActive).CountAsync();
            var activeTeachers = await _context.Teachers.Include(t => t.User).Where(t => t.User.IsActive).CountAsync();

            return Json(new
            {
                totalStudents,
                totalTeachers,
                totalCourses,
                totalDepartments,
                avgAttendance,
                activeStudents,
                activeTeachers
            });
        }

        public IActionResult Settings()
        {
            return View();
        }
    }
}
