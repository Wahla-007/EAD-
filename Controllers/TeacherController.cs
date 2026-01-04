using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendanceManagementSystem.Services.Interfaces;
using AttendanceManagementSystem.Models.ViewModels;
using System.Security.Claims;
using AttendanceManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace AttendanceManagementSystem.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly IAuthService _authService;
        private readonly AttendanceManagementDbContext _context;

        public TeacherController(ITeacherService teacherService, IAuthService authService, 
            AttendanceManagementDbContext context)
        {
            _teacherService = teacherService;
            _authService = authService;
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var username = User.FindFirstValue(ClaimTypes.Name);

            var teacher = await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get teacher's courses
            var courses = await _context.CourseAssignments
                .Include(ca => ca.Course)
                .Include(ca => ca.Section)
                .Where(ca => ca.TeacherId == teacher.TeacherId && ca.IsActive)
                .ToListAsync();

            var totalCourses = courses.Count;
            
            // Get unique students count
            var totalStudents = await _context.StudentCourseRegistrations
                .Where(r => courses.Select(c => c.AssignmentId).Contains(r.AssignmentId))
                .Select(r => r.StudentId)
                .Distinct()
                .CountAsync();

            // Get classes per week (from timetable)
            var totalClasses = await _context.Timetables
                .Where(t => courses.Select(c => c.AssignmentId).Contains(t.AssignmentId) && t.IsActive)
                .CountAsync();

            var attendanceRecords = await _context.Attendances
                .Where(a => courses.Select(c => c.AssignmentId).Contains(a.Registration.AssignmentId))
                .ToListAsync();

            var avgAttendance = attendanceRecords.Any() 
                ? (decimal)(attendanceRecords.Count(a => a.Status == "Present") * 100.0 / attendanceRecords.Count)
                : 0;

            var viewModel = new DashboardViewModel
            {
                UserId = userId,
                Username = username ?? "Teacher",
                Role = "Teacher",
                FullName = teacher.User?.FullName ?? "Teacher",
                TotalCourses = totalCourses,
                TotalStudents = totalStudents,
                TotalSections = courses.Select(c => c.SectionId).Distinct().Count(), // Reusing TotalSections for Classes/Week or similar
                AttendancePercentage = avgAttendance,
                // Using TotalSections property in VM to store classes count for now, or just pass in ViewBag
            };
            
            ViewBag.WeeklyClasses = totalClasses;

            return View(viewModel);
        }

        public async Task<IActionResult> MyCourses()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var teacher = await _teacherService.GetTeacherByUserIdAsync(userId);
            
            if (teacher == null)
                return RedirectToAction("Dashboard");

            var courses = await _teacherService.GetTeacherCoursesAsync(teacher.TeacherId);
            return View(courses);
        }

        // Mark Attendance Page
        public async Task<IActionResult> MarkAttendance(int? assignmentId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var teacher = await _teacherService.GetTeacherByUserIdAsync(userId);
            
            if (teacher == null)
                return RedirectToAction("Dashboard");

            // Get teacher's courses for dropdown
            var courses = await _teacherService.GetTeacherCoursesAsync(teacher.TeacherId);
            ViewBag.Courses = courses;

            if (assignmentId.HasValue)
            {
                // Get students for selected course
                var students = await _teacherService.GetCourseStudentsAsync(assignmentId.Value);
                ViewBag.Students = students;
                ViewBag.SelectedAssignmentId = assignmentId.Value;
            }

            return View();
        }

        // Submit Attendance
        [HttpPost]
        public async Task<IActionResult> SubmitAttendance(int assignmentId, List<int> studentIds, List<string> statuses, DateTime attendanceDate)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                
                for (int i = 0; i < studentIds.Count; i++)
                {
                    // Find the registration
                    var registration = await _context.StudentCourseRegistrations
                        .FirstOrDefaultAsync(r => r.AssignmentId == assignmentId && r.StudentId == studentIds[i]);
                    
                    if (registration != null)
                    {
                        await _teacherService.MarkAttendanceAsync(
                            registration.RegistrationId,
                            attendanceDate,
                            statuses[i],
                            userId
                        );
                    }
                }
                
                TempData["Success"] = "Attendance marked successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("MarkAttendance", new { assignmentId });
        }

        // View Attendance
        public async Task<IActionResult> ViewAttendance(int? assignmentId, DateTime? date)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var teacher = await _teacherService.GetTeacherByUserIdAsync(userId);
            
            if (teacher == null)
                return RedirectToAction("Dashboard");

            var courses = await _teacherService.GetTeacherCoursesAsync(teacher.TeacherId);
            ViewBag.Courses = courses;

            if (assignmentId.HasValue && date.HasValue)
            {
                var attendance = await _teacherService.GetCourseAttendanceAsync(assignmentId.Value, date.Value);
                ViewBag.AttendanceRecords = attendance;
                ViewBag.SelectedAssignmentId = assignmentId.Value;
                ViewBag.SelectedDate = date.Value;
            }

            return View();
        }

        public IActionResult Attendance()
        {
            return RedirectToAction("MarkAttendance");
        }

        public async Task<IActionResult> Students()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
                return NotFound();

            // Get all student registrations for this teacher's courses
            var studentRegistrations = await _context.StudentCourseRegistrations
                .Include(r => r.Student)
                    .ThenInclude(s => s.User)
                .Include(r => r.Assignment)
                    .ThenInclude(a => a.Course)
                .Include(r => r.Assignment)
                    .ThenInclude(a => a.Section)
                .Where(r => r.Assignment.TeacherId == teacher.TeacherId)
                .OrderBy(r => r.Assignment.Course.CourseCode)
                .ThenBy(r => r.Student.RollNumber)
                .ToListAsync();

            return View(studentRegistrations);
        }

        public async Task<IActionResult> Reports()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
                return NotFound();

            ViewBag.TeacherId = teacher.TeacherId;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTeacherReports(int teacherId)
        {
            // Get teacher's courses
            var courses = await _context.CourseAssignments
                .Include(ca => ca.Course)
                .Include(ca => ca.Section)
                .Where(ca => ca.TeacherId == teacherId && ca.IsActive)
                .ToListAsync();

            var totalCourses = courses.Count;
            var totalStudents = await _context.StudentCourseRegistrations
                .Where(r => courses.Select(c => c.AssignmentId).Contains(r.AssignmentId))
                .Select(r => r.StudentId)
                .Distinct()
                .CountAsync();

            var totalClasses = await _context.Attendances
                .Where(a => courses.Select(c => c.AssignmentId).Contains(a.Registration.AssignmentId))
                .Select(a => a.AttendanceDate.Date)
                .Distinct()
                .CountAsync();

            var attendanceRecords = await _context.Attendances
                .Where(a => courses.Select(c => c.AssignmentId).Contains(a.Registration.AssignmentId))
                .ToListAsync();

            var avgAttendance = attendanceRecords.Any() 
                ? attendanceRecords.Count(a => a.Status == "Present") * 100.0 / attendanceRecords.Count
                : 0;

            var courseReports = new List<object>();
            foreach (var course in courses)
            {
                var studentCount = await _context.StudentCourseRegistrations
                    .Where(r => r.AssignmentId == course.AssignmentId)
                    .CountAsync();

                var classesCount = await _context.Attendances
                    .Where(a => a.Registration.AssignmentId == course.AssignmentId)
                    .Select(a => a.AttendanceDate.Date)
                    .Distinct()
                    .CountAsync();

                var courseAttendance = await _context.Attendances
                    .Where(a => a.Registration.AssignmentId == course.AssignmentId)
                    .ToListAsync();

                var courseAttendancePercentage = courseAttendance.Any()
                    ? courseAttendance.Count(a => a.Status == "Present") * 100.0 / courseAttendance.Count
                    : 0;

                courseReports.Add(new
                {
                    courseCode = course.Course.CourseCode,
                    courseName = course.Course.CourseName,
                    sectionName = course.Section.SectionName,
                    studentCount,
                    classesCount,
                    attendancePercentage = courseAttendancePercentage
                });
            }

            return Json(new
            {
                totalCourses,
                totalStudents,
                totalClasses,
                avgAttendance,
                courses = courseReports
            });
        }

        public async Task<IActionResult> Schedule()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
                return NotFound();

            // Get teacher's timetable
            var timetable = await _context.Timetables
                .Include(t => t.Assignment)
                    .ThenInclude(a => a.Course)
                .Include(t => t.Assignment)
                    .ThenInclude(a => a.Section)
                .Where(t => t.Assignment.TeacherId == teacher.TeacherId && t.IsActive)
                .OrderBy(t => t.DayOfWeek)
                .ThenBy(t => t.StartTime)
                .ToListAsync();

            return View(timetable);
        }
    }
}
