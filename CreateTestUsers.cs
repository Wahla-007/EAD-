// =============================================
// C# Script to Generate Test Users
// Run this as a console app or use it in a test method
// =============================================

using System;
using System.Security.Cryptography;
using System.Text;

public class TestUserGenerator
{
    public static void Main()
    {
        Console.WriteLine("=== Test User Generator ===\n");
        
        // Generate hash for Admin
        GenerateUserCredentials("admin", "Admin@123", "Admin");
        
        // Generate hash for Teacher
        GenerateUserCredentials("teacher1", "Teacher@123", "Teacher");
        
        // Generate hash for Student
        GenerateUserCredentials("student1", "Student@123", "Student");
    }
    
    private static void GenerateUserCredentials(string username, string password, string role)
    {
        var (hash, salt) = HashPassword(password);
        
        Console.WriteLine($"\n--- {role} User ---");
        Console.WriteLine($"Username: {username}");
        Console.WriteLine($"Password: {password}");
        Console.WriteLine($"\nSQL Insert Statement:");
        Console.WriteLine($"INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, FullName, Role, IsFirstLogin, IsActive)");
        Console.WriteLine($"VALUES ('{username}', '{username}@ams.com', '{hash}', '{salt}', '{role} User', '{role}', 0, 1);");
        Console.WriteLine();
    }
    
    private static (string hash, string salt) HashPassword(string password)
    {
        using var hmac = new HMACSHA512();
        var salt = Convert.ToBase64String(hmac.Key);
        var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        return (hash, salt);
    }
}

/* 
===========================================
QUICK SETUP INSTRUCTIONS:
===========================================

1. Open SQL Server Management Studio
2. Run DatabaseSetup.sql to create the database
3. Run TestUsersSetup.sql to create departments, courses, etc.
4. Run this C# code to generate the INSERT statements
5. Copy and paste the generated INSERT statements into SQL Server
6. Test login with:
   - Admin: admin / Admin@123
   - Teacher: teacher1 / Teacher@123
   - Student: student1 / Student@123

===========================================
*/
