# =============================================
# Quick Test User Creation Script
# =============================================

Write-Host "=== Generating Test User SQL Statements ===" -ForegroundColor Cyan
Write-Host ""

function New-PasswordHash {
    param([string]$password)
    
    $hmac = [System.Security.Cryptography.HMACSHA512]::new()
    $salt = [Convert]::ToBase64String($hmac.Key)
    $hash = [Convert]::ToBase64String($hmac.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($password)))
    $hmac.Dispose()
    
    return @{
        Hash = $hash
        Salt = $salt
    }
}

# Generate credentials
$adminCreds = New-PasswordHash -password "Admin@123"
$teacherCreds = New-PasswordHash -password "Teacher@123"
$studentCreds = New-PasswordHash -password "Student@123"

# Create SQL file
$sqlContent = @"
-- =============================================
-- INSERT TEST USERS
-- Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
-- =============================================

USE AttendanceManagementDB;
GO

-- Admin User
INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, FullName, Role, IsFirstLogin, IsActive)
VALUES ('admin', 'admin@ams.com', '$($adminCreds.Hash)', '$($adminCreds.Salt)', 'System Administrator', 'Admin', 0, 1);

-- Teacher User
INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, FullName, Role, IsFirstLogin, IsActive)
VALUES ('teacher1', 'teacher1@ams.com', '$($teacherCreds.Hash)', '$($teacherCreds.Salt)', 'John Teacher', 'Teacher', 0, 1);

-- Student User
INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, FullName, Role, IsFirstLogin, IsActive)
VALUES ('student1', 'student1@ams.com', '$($studentCreds.Hash)', '$($studentCreds.Salt)', 'Jane Student', 'Student', 0, 1);

-- Add Teacher record
INSERT INTO Teachers (UserId, EmployeeId, DepartmentId, Specialization, JoiningDate)
SELECT UserId, 'EMP001', 1, 'Computer Science', GETDATE()
FROM Users WHERE Username = 'teacher1';

-- Add Student record
INSERT INTO Students (UserId, RollNumber, DepartmentId, SectionId, EnrollmentDate)
SELECT UserId, 'CS2025001', 1, 1, GETDATE()
FROM Users WHERE Username = 'student1';

PRINT 'Test users created successfully!';
GO
"@

# Save to file
$sqlContent | Out-File -FilePath "InsertTestUsers.sql" -Encoding UTF8

Write-Host "✓ SQL file created: InsertTestUsers.sql" -ForegroundColor Green
Write-Host ""
Write-Host "Test Credentials:" -ForegroundColor Yellow
Write-Host "  Admin    : admin     / Admin@123" -ForegroundColor White
Write-Host "  Teacher  : teacher1  / Teacher@123" -ForegroundColor White
Write-Host "  Student  : student1  / Student@123" -ForegroundColor White
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "1. Run: DatabaseSetup.sql in SQL Server" -ForegroundColor White
Write-Host "2. Run: TestUsersSetup.sql in SQL Server" -ForegroundColor White
Write-Host "3. Run: InsertTestUsers.sql in SQL Server" -ForegroundColor White
Write-Host "4. Login to http://localhost:5297" -ForegroundColor White
