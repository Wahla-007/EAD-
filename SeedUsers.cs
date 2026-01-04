// Comprehensive seed script with Pakistani names, correct usernames, and full data population
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AttendanceManagementSystem.Data;
using AttendanceManagementSystem.Data.Entities;

public static class SeedUsers
{
    private static Random _rng = new Random();

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
        var seDept = context.Departments.FirstOrDefault(d => d.DepartmentCode == "SE");

        // ========================================
        // 3. SEED SECTIONS
        // ========================================
        if (!context.Sections.Any() && activeSemester != null && csDept != null)
        {
            var semesterId = activeSemester.SemesterId;
            var csId = csDept.DepartmentId;
            
            context.Sections.AddRange(
                new Section { SectionName = "CS-A", SemesterId = semesterId, DepartmentId = csId, Capacity = 50, IsActive = true },
                new Section { SectionName = "CS-B", SemesterId = semesterId, DepartmentId = csId, Capacity = 50, IsActive = true }
            );
            context.SaveChanges();
        }
        
        var sectionA = context.Sections.FirstOrDefault(s => s.SectionName == "CS-A");

        // ========================================
        // 4. SEED COURSES (More Courses)
        // ========================================
        if (!context.Courses.Any() && csDept != null && seDept != null)
        {
             var csId = csDept.DepartmentId;
             var seId = seDept.DepartmentId;
             
             context.Courses.AddRange(
                new Course { CourseCode = "CS101", CourseName = "Intro to Computing", CreditHours = 3, DepartmentId = csId, IsActive = true },
                new Course { CourseCode = "CS201", CourseName = "Programming Fundamentals", CreditHours = 3, DepartmentId = csId, IsActive = true },
                new Course { CourseCode = "CS301", CourseName = "Data Structures", CreditHours = 3, DepartmentId = csId, IsActive = true },
                new Course { CourseCode = "SE101", CourseName = "Software Engineering I", CreditHours = 3, DepartmentId = seId, IsActive = true },
                new Course { CourseCode = "SE201", CourseName = "Software Architecture", CreditHours = 3, DepartmentId = seId, IsActive = true },
                new Course { CourseCode = "CS501", CourseName = "Web Development", CreditHours = 3, DepartmentId = csId, IsActive = true }
            );
            context.SaveChanges();
        }

        // ========================================
        // 5. SEED ADMIN USER
        // ========================================
        EnsureUser(context, "admin", "admin@ams.com", "System Admin", "Admin", null);

        // ========================================
        // 6. SEED TEACHERS (Named Usernames)
        // ========================================
        var teachers = new[] {
            ("muhammad.ahmed", "Muhammad Ahmed", "ahmed@ams.com", "EMP-001"),
            ("fatima.bibi", "Fatima Bibi", "fatima@ams.com", "EMP-002"),
            ("ali.hassan", "Ali Hassan", "ali@ams.com", "EMP-003"),
            ("zainab.khan", "Zainab Khan", "zainab@ams.com", "EMP-004"),
            ("usman.gondal", "Usman Gondal", "usman@ams.com", "EMP-005")
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
                    Qualification = "PhD / MS"
                });
                context.SaveChanges();
            }
        }

        // ========================================
        // 7. SEED STUDENTS (Named Usernames)
        // ========================================
        var students = new[] {
            ("bilal.raza", "Bilal Raza", "bilal@ams.com", "F24-001"),
            ("sara.malik", "Sara Malik", "sara@ams.com", "F24-002"),
            ("hamza.shah", "Hamza Shah", "hamza@ams.com", "F24-003"),
            ("ayesha.sids", "Ayesha Siddiqui", "ayesha@ams.com", "F24-004"),
            ("omar.farooq", "Omar Farooq", "omar@ams.com", "F24-005")
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
                    EnrollmentDate = DateTime.UtcNow.AddMonths(-4)
                });
                context.SaveChanges();
            }
        }

        // ========================================
        // 8. ASSIGN COURSES TO TEACHERS (Unique Assignments)
        // ========================================
        if (activeSemester != null && sectionA != null)
        {
            // Get Teachers
            var t1 = GetTeacher(context, "muhammad.ahmed");
            var t2 = GetTeacher(context, "fatima.bibi");
            var t3 = GetTeacher(context, "ali.hassan");
            var t4 = GetTeacher(context, "zainab.khan");
            var t5 = GetTeacher(context, "usman.gondal");

            // Get Courses
            var c1 = context.Courses.FirstOrDefault(c => c.CourseCode == "CS101");
            var c2 = context.Courses.FirstOrDefault(c => c.CourseCode == "CS201");
            var c3 = context.Courses.FirstOrDefault(c => c.CourseCode == "CS301");
            var c4 = context.Courses.FirstOrDefault(c => c.CourseCode == "SE101");
            var c5 = context.Courses.FirstOrDefault(c => c.CourseCode == "SE201");
            var c6 = context.Courses.FirstOrDefault(c => c.CourseCode == "CS501");

            // Assign
            if (t1 != null && c1 != null) EnsureAssignment(context, t1.TeacherId, c1.CourseId, sectionA.SectionId, activeSemester.SemesterId); // Ahmed: CS101
            if (t1 != null && c2 != null) EnsureAssignment(context, t1.TeacherId, c2.CourseId, sectionA.SectionId, activeSemester.SemesterId); // Ahmed: CS201
            
            if (t2 != null && c3 != null) EnsureAssignment(context, t2.TeacherId, c3.CourseId, sectionA.SectionId, activeSemester.SemesterId); // Fatima: CS301
            
            if (t3 != null && c4 != null) EnsureAssignment(context, t3.TeacherId, c4.CourseId, sectionA.SectionId, activeSemester.SemesterId); // Ali: SE101
            
            if (t4 != null && c5 != null) EnsureAssignment(context, t4.TeacherId, c5.CourseId, sectionA.SectionId, activeSemester.SemesterId); // Zainab: SE201
            
            if (t5 != null && c6 != null) EnsureAssignment(context, t5.TeacherId, c6.CourseId, sectionA.SectionId, activeSemester.SemesterId); // Usman: Web Dev
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
                        RegistrationDate = DateTime.UtcNow.AddMonths(-4),
                        Status = "Registered"
                    });
                }
            }
        }
        context.SaveChanges();

        // ========================================
        // 10. CREATE TIMETABLES (For All Assignments)
        // ========================================
        if (!context.Timetables.Any())
        {
            var days = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
            int dayIndex = 0;
            
            foreach (var assignment in assignments)
            {
                // Assign 2 classes per week per assignment
                var day1 = days[dayIndex % 5];
                var day2 = days[(dayIndex + 2) % 5];
                
                context.Timetables.Add(new Timetable { AssignmentId = assignment.AssignmentId, DayOfWeek = (int)day1, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 30, 0), RoomNumber = "Lab-" + (dayIndex + 1), IsActive = true });
                context.Timetables.Add(new Timetable { AssignmentId = assignment.AssignmentId, DayOfWeek = (int)day2, StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(12, 30, 0), RoomNumber = "Lab-" + (dayIndex + 1), IsActive = true });
                
                dayIndex++;
            }
            context.SaveChanges();
        }

        // ========================================
        // 11. MARK RANDOM ATTENDANCE (For Stats)
        // ========================================
        var allRegistrations = context.StudentCourseRegistrations.Include(r => r.Assignment).ThenInclude(a => a.Teacher).ToList();
        
        if (allRegistrations.Any() && !context.Attendances.Any())
        {
            // Generate for last 5 days (excluding weekend)
            for (int i = 0; i < 5; i++)
            {
                var date = DateTime.Today.AddDays(-i);
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) continue;

                foreach (var reg in allRegistrations)
                {
                    // 80% chance Present, 10% Absent, 10% Leave
                    var roll = _rng.Next(100);
                    string status = "Present";
                    if (roll > 80) status = "Absent";
                    if (roll > 90) status = "Leave";

                    // Ensure marked by the teacher of that course
                    var markerId = reg.Assignment.Teacher!.UserId;

                    context.Attendances.Add(new Attendance
                    {
                        RegistrationId = reg.RegistrationId,
                        AttendanceDate = date,
                        Status = status,
                        MarkedBy = markerId,
                        MarkedDate = DateTime.UtcNow,
                        Remarks = status == "Absent" ? "Auto-marked" : null
                    });
                }
            }
            context.SaveChanges();
        }
        
        Console.WriteLine("DB Seeded Successfully with Rich Data!");
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
