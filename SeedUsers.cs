// Comprehensive seed script with all data
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
                new Department { DepartmentName = "Computer Science", DepartmentCode = "CS" },
                new Department { DepartmentName = "Software Engineering", DepartmentCode = "SE" },
                new Department { DepartmentName = "Information Technology", DepartmentCode = "IT" },
                new Department { DepartmentName = "Electrical Engineering", DepartmentCode = "EE" }
            );
            context.SaveChanges();
        }

        // ========================================
        // 2. SEED SESSIONS
        // ========================================
        if (!context.Sessions.Any())
        {
            context.Sessions.AddRange(
                new Session { SessionName = "2023-2024", StartDate = new DateTime(2023, 9, 1), EndDate = new DateTime(2024, 8, 31), IsActive = false },
                new Session { SessionName = "2024-2025", StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2025, 8, 31), IsActive = true }
            );
            context.SaveChanges();
        }

        var activeSession = context.Sessions.FirstOrDefault(s => s.IsActive);
        var csDept = context.Departments.FirstOrDefault(d => d.DepartmentCode == "CS");

        // ========================================
        // 3. SEED SEMESTERS
        // ========================================
        if (!context.Semesters.Any() && activeSession != null)
        {
            context.Semesters.AddRange(
                new Semester { SemesterName = "Fall 2024", SessionId = activeSession.SessionId, StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2025, 1, 15), IsActive = true },
                new Semester { SemesterName = "Spring 2025", SessionId = activeSession.SessionId, StartDate = new DateTime(2025, 2, 1), EndDate = new DateTime(2025, 6, 15), IsActive = false }
            );
            context.SaveChanges();
        }

        var activeSemester = context.Semesters.FirstOrDefault(s => s.IsActive);

        // ========================================
        // 4. SEED SECTIONS
        // ========================================
        if (!context.Sections.Any() && activeSemester != null)
        {
            context.Sections.AddRange(
                new Section { SectionName = "CS-A", SemesterId = activeSemester.SemesterId },
                new Section { SectionName = "CS-B", SemesterId = activeSemester.SemesterId },
                new Section { SectionName = "SE-A", SemesterId = activeSemester.SemesterId }
            );
            context.SaveChanges();
        }

        var sectionA = context.Sections.FirstOrDefault(s => s.SectionName == "CS-A");

        // ========================================
        // 5. SEED COURSES
        // ========================================
        if (!context.Courses.Any() && csDept != null)
        {
            context.Courses.AddRange(
                new Course { CourseCode = "CS101", CourseName = "Introduction to Programming", CreditHours = 3, DepartmentId = csDept.DepartmentId },
                new Course { CourseCode = "CS201", CourseName = "Data Structures", CreditHours = 3, DepartmentId = csDept.DepartmentId },
                new Course { CourseCode = "CS301", CourseName = "Database Systems", CreditHours = 3, DepartmentId = csDept.DepartmentId },
                new Course { CourseCode = "CS401", CourseName = "Software Engineering", CreditHours = 3, DepartmentId = csDept.DepartmentId },
                new Course { CourseCode = "CS501", CourseName = "Web Development", CreditHours = 3, DepartmentId = csDept.DepartmentId }
            );
            context.SaveChanges();
        }

        // ========================================
        // 6. SEED ADMIN USER
        // ========================================
        var adminUser = context.Users.FirstOrDefault(u => u.Username == "admin");
        if (adminUser == null)
        {
            var (adminHash, adminSalt) = HashPassword("Test@123");
            context.Users.Add(new User
            {
                Username = "admin",
                Email = "admin@ams.com",
                PasswordHash = adminHash,
                PasswordSalt = adminSalt,
                FullName = "System Administrator",
                Role = "Admin",
                IsFirstLogin = false,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            });
            context.SaveChanges();
        }
        else
        {
            var (adminHash, adminSalt) = HashPassword("Test@123");
            adminUser.PasswordHash = adminHash;
            adminUser.PasswordSalt = adminSalt;
            context.SaveChanges();
        }

        // ========================================
        // 7. SEED TEACHERS WITH TEACHER RECORDS
        // ========================================
        var teacherData = new[] {
            ("teacher1", "Ahmed Khan", "teacher1@ams.com", "EMP001"),
            ("teacher2", "Muhammad Ali", "teacher2@ams.com", "EMP002"),
            ("teacher3", "Fatima Hassan", "teacher3@ams.com", "EMP003"),
            ("teacher4", "Zainab Ahmed", "teacher4@ams.com", "EMP004"),
            ("teacher5", "Usman Malik", "teacher5@ams.com", "EMP005")
        };

        foreach (var (username, fullName, email, empId) in teacherData)
        {
            var existingUser = context.Users.FirstOrDefault(u => u.Username == username);
            if (existingUser == null)
            {
                var (hash, salt) = HashPassword("Test@123");
                var newUser = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    FullName = fullName,
                    Role = "Teacher",
                    IsFirstLogin = false,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };
                context.Users.Add(newUser);
                context.SaveChanges();

                // Create Teacher record
                if (csDept != null && !context.Teachers.Any(t => t.EmployeeId == empId))
                {
                    context.Teachers.Add(new Teacher
                    {
                        UserId = newUser.UserId,
                        EmployeeId = empId,
                        DepartmentId = csDept.DepartmentId,
                        Designation = "Lecturer",
                        Qualification = "MS Computer Science"
                    });
                    context.SaveChanges();
                }
            }
            else
            {
                var (hash, salt) = HashPassword("Test@123");
                existingUser.PasswordHash = hash;
                existingUser.PasswordSalt = salt;
                context.SaveChanges();
            }
        }

        // ========================================
        // 8. SEED STUDENTS WITH STUDENT RECORDS
        // ========================================
        var studentData = new[] {
            ("student1", "Bilal Ahmed", "student1@ams.com", "FA24-CS-001"),
            ("student2", "Sara Khan", "student2@ams.com", "FA24-CS-002"),
            ("student3", "Ali Raza", "student3@ams.com", "FA24-CS-003"),
            ("student4", "Ayesha Malik", "student4@ams.com", "FA24-CS-004"),
            ("student5", "Hassan Ali", "student5@ams.com", "FA24-CS-005")
        };

        foreach (var (username, fullName, email, rollNo) in studentData)
        {
            var existingUser = context.Users.FirstOrDefault(u => u.Username == username);
            if (existingUser == null)
            {
                var (hash, salt) = HashPassword("Test@123");
                var newUser = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    FullName = fullName,
                    Role = "Student",
                    IsFirstLogin = false,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };
                context.Users.Add(newUser);
                context.SaveChanges();

                // Create Student record
                if (csDept != null && sectionA != null && !context.Students.Any(s => s.RollNumber == rollNo))
                {
                    context.Students.Add(new Student
                    {
                        UserId = newUser.UserId,
                        RollNumber = rollNo,
                        DepartmentId = csDept.DepartmentId,
                        SectionId = sectionA.SectionId,
                        EnrollmentDate = DateTime.UtcNow
                    });
                    context.SaveChanges();
                }
            }
            else
            {
                var (hash, salt) = HashPassword("Test@123");
                existingUser.PasswordHash = hash;
                existingUser.PasswordSalt = salt;
                context.SaveChanges();
            }
        }

        // ========================================
        // 9. SEED COURSE ASSIGNMENTS (Teacher -> Course -> Section)
        // ========================================
        var teacher1 = context.Teachers.Include(t => t.User).FirstOrDefault(t => t.User.Username == "teacher1");
        var course1 = context.Courses.FirstOrDefault(c => c.CourseCode == "CS101");
        var course2 = context.Courses.FirstOrDefault(c => c.CourseCode == "CS201");

        if (teacher1 != null && course1 != null && sectionA != null && activeSemester != null)
        {
            if (!context.CourseAssignments.Any(ca => ca.TeacherId == teacher1.TeacherId && ca.CourseId == course1.CourseId))
            {
                context.CourseAssignments.Add(new CourseAssignment
                {
                    TeacherId = teacher1.TeacherId,
                    CourseId = course1.CourseId,
                    SectionId = sectionA.SectionId,
                    SemesterId = activeSemester.SemesterId,
                    IsActive = true
                });
                context.SaveChanges();
            }

            if (course2 != null && !context.CourseAssignments.Any(ca => ca.TeacherId == teacher1.TeacherId && ca.CourseId == course2.CourseId))
            {
                context.CourseAssignments.Add(new CourseAssignment
                {
                    TeacherId = teacher1.TeacherId,
                    CourseId = course2.CourseId,
                    SectionId = sectionA.SectionId,
                    SemesterId = activeSemester.SemesterId,
                    IsActive = true
                });
                context.SaveChanges();
            }
        }

        // ========================================
        // 10. SEED STUDENT COURSE REGISTRATIONS
        // ========================================
        var assignment1 = context.CourseAssignments.FirstOrDefault(ca => ca.Course.CourseCode == "CS101");
        var students = context.Students.Include(s => s.User).ToList();

        if (assignment1 != null)
        {
            foreach (var student in students)
            {
                if (!context.StudentCourseRegistrations.Any(r => r.StudentId == student.StudentId && r.AssignmentId == assignment1.AssignmentId))
                {
                    context.StudentCourseRegistrations.Add(new StudentCourseRegistration
                    {
                        StudentId = student.StudentId,
                        AssignmentId = assignment1.AssignmentId,
                        RegistrationDate = DateTime.UtcNow,
                        Status = "Registered"
                    });
                }
            }
            context.SaveChanges();
        }

        Console.WriteLine("==============================================");
        Console.WriteLine("DATABASE SEEDED SUCCESSFULLY!");
        Console.WriteLine("==============================================");
        Console.WriteLine("Login Credentials (Password: Test@123):");
        Console.WriteLine("- Admin: admin");
        Console.WriteLine("- Teachers: teacher1, teacher2, teacher3, teacher4, teacher5");
        Console.WriteLine("- Students: student1, student2, student3, student4, student5");
        Console.WriteLine("==============================================");
    }

    private static (string hash, string salt) HashPassword(string password)
    {
        using var hmac = new HMACSHA512();
        var salt = Convert.ToBase64String(hmac.Key);
        var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        return (hash, salt);
    }
}
