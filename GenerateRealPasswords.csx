// Run this to generate real password hashes
// In terminal: dotnet script GenerateRealPasswords.csx

using System;
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("=== Generating Real Password Hashes ===\n");

GenerateHash("admin", "Test@123");
GenerateHash("teacher1", "Test@123");
GenerateHash("student1", "Test@123");

void GenerateHash(string username, string password)
{
    using var hmac = new HMACSHA512();
    var salt = Convert.ToBase64String(hmac.Key);
    var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
    
    Console.WriteLine($"-- {username}");
    Console.WriteLine($"UPDATE Users SET PasswordHash = '{hash}', PasswordSalt = '{salt}' WHERE Username = '{username}';");
    Console.WriteLine();
}
