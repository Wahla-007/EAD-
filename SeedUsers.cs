// Comprehensive seed script with Pakistani names and full data
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AttendanceManagementSystem.Data;
using AttendanceManagementSystem.Data.Entities;

public static class SeedUsers
{
    public static void SeedTestUsers(AttendanceManagementDbContext context)
    {
        // ========================================
        // 1. SEED DEPARTMENTS
        // ========================================
        if (!context.Departments.Any())
        {
            context.Departments.AddRange(
                new Department { DepartmentName = "Computer Science", DepartmentCode = "CS", IsActive = true },
                new Department { DepartmentName = "Software Engineering", DepartmentCode = "SE", IsActive = true },
                new Department { DepartmentName = "Information Technology", DepartmentCode = "IT", IsActive = true }
            );
            context.SaveChanges();
        }

        // ========================================
        // 2. SEED SESSIONS & SEMESTERS
        // ========================================
        if (!context.Sessions.Any())
        {
            context.Sessions.Add(new Session 
            { 
                SessionName = "2024-2025", 
                StartDate = new DateTime(2024, 9, 1), 
                EndDate = new DateTime(2025, 8, 31), 
                IsActive = true 
            });
            context.SaveChanges();
        }

        var activeSession = context.Sessions.FirstOrDefault(s => s.IsActive);
        
        if (!context.Semesters.Any() && activeSession != null)
        {
            context.Semesters.AddRange(
                new Semester { SemesterName = "Fall 2024", SessionId = activeSession.SessionId, StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2025, 1, 15), IsActive = true },
                new Semester { SemesterName = "Spring 2025", SessionId = activeSession.SessionId, StartDate = new DateTime(2025, 2, 1), EndDate = new DateTime(2025, 6, 15), IsActive = false }
            );
            context.SaveChanges();
        }

        var activeSemester = context.Semesters.FirstOrDefault(s => s.IsActive);
        var csDept = context.Departments.FirstOrDefault(d => d.DepartmentCode == "CS");

        // ========================================
        // 3. SEED SECTIONS
        // ========================================
        if (!context.Sections.Any() && activeSemester != null && csDept != null)
        {
            var semesterId = activeSemester.SemesterId;
            var deptId = csDept.DepartmentId;
            
            context.Sections.AddRange(
                new Section { SectionName = "CS-A", SemesterId = semesterId, DepartmentId = deptId, Capacity = 50, IsActive = true },
                new Section { SectionName = "CS-B", SemesterId = semesterId, DepartmentId = deptId, Capacity = 50, IsActive = true },
                new Section { SectionName = "SE-A", SemesterId = semesterId, DepartmentId = deptId, Capacity = 50, IsActive = true }
            );
            context.SaveChanges();
        }

        var sectionA = context.Sections.FirstOrDefault(s => s.SectionName == "CS-A");

        // ========================================
        // 4. SEED COURSES
        // ========================================
        if (!context.Courses.Any() && csDept != null)
        {
             var deptId = csDept.DepartmentId;
             context.Courses.AddRange(
                new Course { CourseCode = "CS101", CourseName = "Intro to Programming", CreditHours = 3, DepartmentId = deptId, IsActive = true },
                new Course { CourseCode = "CS201", CourseName = "Data Structures", CreditHours = 3, DepartmentId = deptId, IsActive = true },
                new Course { CourseCode = "CS301", CourseName = "Database Systems", CreditHours = 3, DepartmentId = deptId, IsActive = true },
                new Course { CourseCode = "CS401", CourseName = "Software Engineering", CreditHours = 3, DepartmentId = deptId, IsActive = true },
                new Course { CourseCode = "CS501", CourseName = "Web Development", CreditHours = 3, DepartmentId = deptId, IsActive = true }
            );
            context.SaveChanges();
        }

        // ========================================
        // 5. SEED ADMIN USER
        // ========================================
        EnsureUser(context, "admin", "admin@ams.com", "System Admin", "Admin", null);

        // ========================================
        // 6. SEED TEACHERS
        // ========================================
        var teachers = new[] {
            ("teacher1", "Muhammad Ahmed", "teacher1@ams.com", "EMP001"),
            ("teacher2", "Fatima Bibi", "teacher2@ams.com", "EMP002"),
            ("teacher3", "Ali Hassan", "teacher3@ams.com", "EMP003"),
            ("teacher4", "Zainab Khan", "teacher4@ams.com", "EMP004"),
            ("teacher5", "Usman Gondal", "teacher5@ams.com", "EMP005")
        };

        foreach (var (username, fullName, email, empId) in teachers)
        {
            var user = EnsureUser(context, username, email, fullName, "Teacher", null);
            
            if (csDept != null && !context.Teachers.Any(t => t.UserId == user.UserId))
            {
                context.Teachers.Add(new Teacher
                {
                    UserId = user.UserId,
                    EmployeeId = empId,
                    DepartmentId = csDept.DepartmentId,
                    Designation = "Lecturer",
                    Qualification = "MS CS"
                });
                context.SaveChanges();
            }
        }

        // ========================================
        // 7. SEED STUDENTS
        // ========================================
        var students = new[] {
            ("student1", "Bilal Raza", "student1@ams.com", "CS-001"),
            ("student2", "Sara Malik", "student2@ams.com", "CS-002"),
            ("student3", "Hamza Shah", "student3@ams.com", "CS-003"),
            ("student4", "Ayesha Siddiqui", "student4@ams.com", "CS-004"),
            ("student5", "Omar Farooq", "student5@ams.com", "CS-005")
        };

        foreach (var (username, fullName, email, rollNo) in students)
        {
            var user = EnsureUser(context, username, email, fullName, "Student", null);
            
            if (csDept != null && sectionA != null && !context.Students.Any(s => s.UserId == user.UserId))
            {
                context.Students.Add(new Student
                {
                    UserId = user.UserId,
                    RollNumber = rollNo,
                    DepartmentId = csDept.DepartmentId,
                    SectionId = sectionA.SectionId,
                    EnrollmentDate = DateTime.UtcNow
                });
                context.SaveChanges();
            }
        }

        // ========================================
        // 8. ASSIGN COURSES TO TEACHERS
        // ========================================
        if (activeSemester != null && sectionA != null)
        {
            var teacher1 = GetTeacher(context, "teacher1");
            var teacher2 = GetTeacher(context, "teacher2");
            var cs101 = context.Courses.FirstOrDefault(c => c.CourseCode == "CS101");
            var cs201 = context.Courses.FirstOrDefault(c => c.CourseCode == "CS201");
            var cs301 = context.Courses.FirstOrDefault(c => c.CourseCode == "CS301");

            // Teacher 1 teaches CS101 and CS201
            if (teacher1 != null && cs101 != null) EnsureAssignment(context, teacher1.TeacherId, cs101.CourseId, sectionA.SectionId, activeSemester.SemesterId);
            if (teacher1 != null && cs201 != null) EnsureAssignment(context, teacher1.TeacherId, cs201.CourseId, sectionA.SectionId, activeSemester.SemesterId);
            
            // Teacher 2 teaches CS301
            if (teacher2 != null && cs301 != null) EnsureAssignment(context, teacher2.TeacherId, cs301.CourseId, sectionA.SectionId, activeSemester.SemesterId);
        }

        // ========================================
        // 9. REGISTER STUDENTS TO COURSES
        // ========================================
        var allStudents = context.Students.ToList();
        var assignments = context.CourseAssignments.Where(a => a.IsActive).ToList();

        foreach (var assignment in assignments)
        {
            foreach (var student in allStudents)
            {
                if (!context.StudentCourseRegistrations.Any(r => r.StudentId == student.StudentId && r.AssignmentId == assignment.AssignmentId))
                {
                    context.StudentCourseRegistrations.Add(new StudentCourseRegistration
                    {
                        StudentId = student.StudentId,
                        AssignmentId = assignment.AssignmentId,
                        RegistrationDate = DateTime.UtcNow,
                        Status = "Registered"
                    });
                }
            }
        }
        context.SaveChanges();

        // ========================================
        // 10. CREATE TIMETABLE (For Teacher Dashboard Stats)
        // ========================================
        var assignment1 = context.CourseAssignments.FirstOrDefault();
        if (assignment1 != null && !context.Timetables.Any())
        {
            context.Timetables.AddRange(
                new Timetable { AssignmentId = assignment1.AssignmentId, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 30, 0), RoomNumber = "Lab-1", IsActive = true },
                new Timetable { AssignmentId = assignment1.AssignmentId, DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 30, 0), RoomNumber = "Lab-1", IsActive = true }
            );
            context.SaveChanges();
        }

        // ========================================
        // 11. MARK SOME ATTENDANCE (For Stats)
        // ========================================
        var registrations = context.StudentCourseRegistrations.Include(r => r.Assignment).Include(r => r.Student).ThenInclude(s => s.User).Take(5).ToList();
        var teacherUser = context.Users.FirstOrDefault(u => u.Username == "teacher1");

        if (registrations.Any() && teacherUser != null && !context.Attendances.Any())
        {
            var date = DateTime.Today.AddDays(-1);
            foreach (var reg in registrations)
            {
                context.Attendances.Add(new Attendance
                {
                    RegistrationId = reg.RegistrationId,
                    AttendanceDate = date,
                    Status = "Present",
                    MarkedBy = teacherUser.UserId,
                    MarkedDate = DateTime.UtcNow
                });
            }
            context.SaveChanges();
        }
        
        Console.WriteLine("DB Seeded Successfully!");
    }

    private static User EnsureUser(AttendanceManagementDbContext context, string username, string email, string fullName, string role, string? profileImage)
    {
        var user = context.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            var (hash, salt) = HashPassword("Test@123");
            user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                FullName = fullName,
                Role = role,
                IsFirstLogin = false,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                ProfileImage = profileImage
            };
            context.Users.Add(user);
            context.SaveChanges();
        }
        return user;
    }

    private static Teacher? GetTeacher(AttendanceManagementDbContext context, string username)
    {
        return context.Teachers.Include(t => t.User).FirstOrDefault(t => t.User.Username == username);
    }

    private static void EnsureAssignment(AttendanceManagementDbContext context, int teacherId, int courseId, int sectionId, int semesterId)
    {
        if (!context.CourseAssignments.Any(ca => ca.TeacherId == teacherId && ca.CourseId == courseId && ca.SectionId == sectionId))
        {
            context.CourseAssignments.Add(new CourseAssignment
            {
                TeacherId = teacherId,
                CourseId = courseId,
                SectionId = sectionId,
                SemesterId = semesterId,
                IsActive = true
            });
            context.SaveChanges();
        }
    }

    private static (string hash, string salt) HashPassword(string password)
    {
        using var hmac = new HMACSHA512();
        var salt = Convert.ToBase64String(hmac.Key);
        var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        return (hash, salt);
    }
}
