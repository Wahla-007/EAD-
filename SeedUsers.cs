// Quick script to seed test users with proper password hashes
// Run in Program.cs startup or as a separate tool

using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AttendanceManagementSystem.Data;
using AttendanceManagementSystem.Data.Entities;

public static class SeedUsers
{
    public static void SeedTestUsers(AttendanceManagementDbContext context)
    {
        // Check if admin already exists with correct format
        var adminUser = context.Users.FirstOrDefault(u => u.Username == "admin");
        
        if (adminUser == null)
        {
            // Create Admin
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
        }
        else
        {
            // Update existing admin password
            var (adminHash, adminSalt) = HashPassword("Test@123");
            adminUser.PasswordHash = adminHash;
            adminUser.PasswordSalt = adminSalt;
        }

        // Check/Create Teacher
        var teacherUser = context.Users.FirstOrDefault(u => u.Username == "teacher1");
        if (teacherUser == null)
        {
            var (hash, salt) = HashPassword("Test@123");
            context.Users.Add(new User
            {
                Username = "teacher1",
                Email = "teacher1@ams.com",
                PasswordHash = hash,
                PasswordSalt = salt,
                FullName = "Ahmed Khan",
                Role = "Teacher",
                IsFirstLogin = false,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            });
        }
        else
        {
            var (hash, salt) = HashPassword("Test@123");
            teacherUser.PasswordHash = hash;
            teacherUser.PasswordSalt = salt;
        }

        // Add more teachers
        var teacherNames = new[] {
            ("teacher2", "Muhammad Ali", "teacher2@ams.com"),
            ("teacher3", "Fatima Hassan", "teacher3@ams.com"),
            ("teacher4", "Zainab Ahmed", "teacher4@ams.com"),
            ("teacher5", "Usman Malik", "teacher5@ams.com")
        };

        foreach (var (username, fullName, email) in teacherNames)
        {
            var existingTeacher = context.Users.FirstOrDefault(u => u.Username == username);
            if (existingTeacher == null)
            {
                var (hash, salt) = HashPassword("Test@123");
                context.Users.Add(new User
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
                });
            }
            else
            {
                var (hash, salt) = HashPassword("Test@123");
                existingTeacher.PasswordHash = hash;
                existingTeacher.PasswordSalt = salt;
            }
        }

        // Check/Create Student
        var studentUser = context.Users.FirstOrDefault(u => u.Username == "student1");
        if (studentUser == null)
        {
            var (hash, salt) = HashPassword("Test@123");
            context.Users.Add(new User
            {
                Username = "student1",
                Email = "student1@ams.com",
                PasswordHash = hash,
                PasswordSalt = salt,
                FullName = "Bilal Ahmed",
                Role = "Student",
                IsFirstLogin = false,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            });
        }
        else
        {
            var (hash, salt) = HashPassword("Test@123");
            studentUser.PasswordHash = hash;
            studentUser.PasswordSalt = salt;
        }

        // Add more students
        var studentNames = new[] {
            ("student2", "Sara Khan", "student2@ams.com"),
            ("student3", "Ali Raza", "student3@ams.com"),
            ("student4", "Ayesha Malik", "student4@ams.com"),
            ("student5", "Hassan Ali", "student5@ams.com")
        };

        foreach (var (username, fullName, email) in studentNames)
        {
            var existingStudent = context.Users.FirstOrDefault(u => u.Username == username);
            if (existingStudent == null)
            {
                var (hash, salt) = HashPassword("Test@123");
                context.Users.Add(new User
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
                });
            }
            else
            {
                var (hash, salt) = HashPassword("Test@123");
                existingStudent.PasswordHash = hash;
                existingStudent.PasswordSalt = salt;
            }
        }

        // Update all Pakistani teachers and students to have working passwords
        var allTeachers = context.Users.Where(u => u.Role == "Teacher").ToList();
        foreach (var teacher in allTeachers)
        {
            var (hash, salt) = HashPassword("Test@123");
            teacher.PasswordHash = hash;
            teacher.PasswordSalt = salt;
        }

        var allStudents = context.Users.Where(u => u.Role == "Student").ToList();
        foreach (var student in allStudents)
        {
            var (hash, salt) = HashPassword("Test@123");
            student.PasswordHash = hash;
            student.PasswordSalt = salt;
        }

        context.SaveChanges();
        Console.WriteLine("Test users seeded/updated successfully! All teachers and students can login with password: Test@123");
    }

    private static (string hash, string salt) HashPassword(string password)
    {
        using var hmac = new HMACSHA512();
        var salt = Convert.ToBase64String(hmac.Key);
        var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        return (hash, salt);
    }
}
