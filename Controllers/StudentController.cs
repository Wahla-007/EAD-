using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendanceManagementSystem.Services.Interfaces;
using AttendanceManagementSystem.Models.ViewModels;
using System.Security.Claims;
using AttendanceManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace AttendanceManagementSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IAuthService _authService;
        private readonly AttendanceManagementDbContext _context;

        public StudentController(IStudentService studentService, IAuthService authService,
            AttendanceManagementDbContext context)
        {
            _studentService = studentService;
            _authService = authService;
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var username = User.FindFirstValue(ClaimTypes.Name);

            var student = await _studentService.GetStudentByUserIdAsync(userId);
            
            // Get real stats
            int enrolledCoursesCount = 0;
            decimal attendancePercentage = 0;
            int classesPerWeek = 0;
            string academicStatus = "N/A";
            
            if (student != null)
            {
                // Count enrolled courses
                enrolledCoursesCount = await _context.StudentCourseRegistrations
                    .Where(r => r.StudentId == student.StudentId && r.Status == "Registered")
                    .CountAsync();
                
                // Get attendance stats
                var attendanceStats = await _studentService.GetAttendanceStatsAsync(student.StudentId);
                if (attendanceStats != null && attendanceStats.Any())
                {
                    attendancePercentage = attendanceStats.Values.Average();
                }
                
                // Get classes per week if student has a section
                if (student.SectionId.HasValue)
                {
                    classesPerWeek = await _context.Timetables
                        .Where(t => t.Assignment.SectionId == student.SectionId && t.IsActive)
                        .CountAsync();
                }
                
                // Determine academic status based on attendance
                if (attendancePercentage >= 90) academicStatus = "A+";
                else if (attendancePercentage >= 80) academicStatus = "A";
                else if (attendancePercentage >= 70) academicStatus = "B";
                else if (attendancePercentage >= 60) academicStatus = "C";
                else if (attendancePercentage > 0) academicStatus = "D";
                else academicStatus = "N/A";
            }

            var viewModel = new DashboardViewModel
            {
                UserId = userId,
                Username = username ?? "Student",
                Role = "Student",
                FullName = student?.User?.FullName ?? "Student",
                EnrolledCoursesCount = enrolledCoursesCount,
                AttendancePercentage = Math.Round(attendancePercentage, 0),
                ClassesPerWeek = classesPerWeek,
                AcademicStatus = academicStatus
            };

            return View(viewModel);
        }

        public async Task<IActionResult> MyCourses()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var student = await _studentService.GetStudentByUserIdAsync(userId);
            
            if (student == null)
                return RedirectToAction("Dashboard");

            var courses = await _studentService.GetStudentCoursesAsync(student.StudentId);
            return View(courses);
        }

        // Register for Courses
        public async Task<IActionResult> RegisterCourses()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var student = await _studentService.GetStudentByUserIdAsync(userId);
            
            if (student == null || student.SectionId == null)
            {
                TempData["Error"] = "You must be assigned to a section before registering for courses.";
                return RedirectToAction("Dashboard");
            }

            // Get available courses for student's section
            var availableCourses = await _context.CourseAssignments
                .Include(ca => ca.Course)
                .Include(ca => ca.Teacher)
                    .ThenInclude(t => t.User)
                .Include(ca => ca.Section)
                .Include(ca => ca.Semester)
                .Where(ca => ca.SectionId == student.SectionId && ca.IsActive)
                .ToListAsync();

            // Get already registered courses
            var registeredCourseIds = await _context.StudentCourseRegistrations
                .Where(r => r.StudentId == student.StudentId && r.Status == "Registered")
                .Select(r => r.AssignmentId)
                .ToListAsync();

            ViewBag.RegisteredCourseIds = registeredCourseIds;
            return View(availableCourses);
        }

        // Register for a course
        [HttpPost]
        public async Task<IActionResult> RegisterForCourse(int assignmentId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                var student = await _studentService.GetStudentByUserIdAsync(userId);
                
                if (student == null)
                    return RedirectToAction("Dashboard");

                await _studentService.RegisterForCourseAsync(student.StudentId, assignmentId);
                TempData["Success"] = "Successfully registered for the course!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("RegisterCourses");
        }

        public async Task<IActionResult> Attendance()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var student = await _studentService.GetStudentByUserIdAsync(userId);
            
            if (student == null)
                return RedirectToAction("Dashboard");

            var stats = await _studentService.GetAttendanceStatsAsync(student.StudentId);
            return View(stats);
        }

        // View Timetable
        public async Task<IActionResult> Schedule()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var student = await _studentService.GetStudentByUserIdAsync(userId);
            
            if (student == null || student.SectionId == null)
            {
                TempData["Error"] = "You must be assigned to a section to view timetable.";
                return View(new List<Data.Entities.Timetable>());
            }

            // Get timetable for student's registered courses
            var timetable = await _context.Timetables
                .Include(t => t.Assignment)
                    .ThenInclude(a => a.Course)
                .Include(t => t.Assignment)
                    .ThenInclude(a => a.Teacher)
                        .ThenInclude(teacher => teacher.User)
                .Where(t => t.Assignment.SectionId == student.SectionId && t.IsActive)
                .OrderBy(t => t.DayOfWeek)
                .ThenBy(t => t.StartTime)
                .ToListAsync();

            return View(timetable);
        }

        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Department)
                .Include(s => s.Section)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
                return NotFound();

            return View(student);
        }
    }
}
