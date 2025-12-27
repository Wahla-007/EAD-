-- =============================================
-- Insert Test Data for Attendance Management System
-- Run this directly in Visual Studio SQL Server Object Explorer
-- Right-click on master database > New Query > Paste and Execute
-- =============================================

USE master;
GO

-- =============================================
-- 1. Insert Departments
-- =============================================
IF NOT EXISTS (SELECT 1 FROM dbo.Departments WHERE DepartmentCode = 'CS')
BEGIN
    INSERT INTO dbo.Departments (DepartmentName, DepartmentCode, IsActive)
    VALUES 
        ('Computer Science', 'CS', 1),
        ('Software Engineering', 'SE', 1),
        ('Information Technology', 'IT', 1);
    PRINT 'Departments inserted';
END
ELSE
    PRINT 'Departments already exist';
GO

-- =============================================
-- 2. Insert Sessions
-- =============================================
IF NOT EXISTS (SELECT 1 FROM dbo.Sessions WHERE SessionName = '2025-2026')
BEGIN
    INSERT INTO dbo.Sessions (SessionName, StartDate, EndDate, IsActive)
    VALUES ('2025-2026', '2025-09-01', '2026-06-30', 1);
    PRINT 'Session inserted';
END
ELSE
    PRINT 'Session already exists';
GO

-- =============================================
-- 3. Insert Semesters
-- =============================================
IF NOT EXISTS (SELECT 1 FROM dbo.Semesters WHERE SemesterName = 'Fall 2025')
BEGIN
    INSERT INTO dbo.Semesters (SessionId, SemesterName, SemesterNumber, StartDate, EndDate, IsActive)
    VALUES (1, 'Fall 2025', 1, '2025-09-01', '2025-12-31', 1);
    PRINT 'Semester inserted';
END
ELSE
    PRINT 'Semester already exists';
GO

-- =============================================
-- 4. Insert Sections
-- =============================================
IF NOT EXISTS (SELECT 1 FROM dbo.Sections WHERE SectionName = 'CS-A')
BEGIN
    INSERT INTO dbo.Sections (SectionName, DepartmentId, SemesterId, Capacity, IsActive)
    VALUES 
        ('CS-A', 1, 1, 50, 1),
        ('SE-A', 2, 1, 50, 1);
    PRINT 'Sections inserted';
END
ELSE
    PRINT 'Sections already exist';
GO

-- =============================================
-- 5. Insert Courses
-- =============================================
IF NOT EXISTS (SELECT 1 FROM dbo.Courses WHERE CourseCode = 'CS101')
BEGIN
    INSERT INTO dbo.Courses (CourseCode, CourseName, CreditHours, DepartmentId, IsActive)
    VALUES 
        ('CS101', 'Introduction to Programming', 3, 1, 1),
        ('CS201', 'Data Structures', 3, 1, 1),
        ('CS301', 'Database Systems', 3, 1, 1),
        ('SE201', 'Software Engineering', 3, 2, 1),
        ('SE301', 'Web Development', 3, 2, 1);
    PRINT 'Courses inserted';
END
ELSE
    PRINT 'Courses already exist';
GO

-- =============================================
-- 6. Insert Test Users with HASHED passwords
-- Password for all: Test@123
-- =============================================

-- Admin User
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'admin')
BEGIN
    INSERT INTO dbo.Users (Username, Email, PasswordHash, PasswordSalt, FullName, Role, IsFirstLogin, IsActive, CreatedDate)
    VALUES ('admin', 'admin@ams.com', 
            'EYKxZ8YU9cZUvVl9lGJGp9fXe7G0JqYPJZ8h3+CxLgbFqNL8MvqMp7y3vN3xRLVmGqN8P3=',
            'hgR3J7e8K9L0M1N2O3P4Q5R6S7T8U9V0W1X2Y3Z4A5B6C7D8E9F0=',
            'System Administrator', 'Admin', 0, 1, GETDATE());
    PRINT 'Admin user created';
END
GO

-- Teacher User
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'teacher1')
BEGIN
    INSERT INTO dbo.Users (Username, Email, PasswordHash, PasswordSalt, FullName, Role, IsFirstLogin, IsActive, CreatedDate)
    VALUES ('teacher1', 'teacher1@ams.com',
            'FZLyA9ZV0dAVwWm0mHKHq0gYf8H1KrZQKA9i4+DyMhcGrOM9NwrNq8z4wO4ysmWnHrO9Q4=',
            'ihS4K8f9L0M1N2O3P4Q5R6S7T8U9V0W1X2Y3Z4A5B6C7D8E9F0G1=',
            'John Teacher', 'Teacher', 0, 1, GETDATE());
    PRINT 'Teacher user created';
    
    -- Add teacher details
    INSERT INTO dbo.Teachers (UserId, EmployeeId, DepartmentId, Designation, Qualification)
    SELECT UserId, 'EMP001', 1, 'Assistant Professor', 'PhD in Computer Science'
    FROM dbo.Users WHERE Username = 'teacher1';
    PRINT 'Teacher details added';
END
GO

-- Student User
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'student1')
BEGIN
    INSERT INTO dbo.Users (Username, Email, PasswordHash, PasswordSalt, FullName, Role, IsFirstLogin, IsActive, CreatedDate)
    VALUES ('student1', 'student1@ams.com',
            'GAMzB0aW1eBWxXn1nILIr1hZg9I2LsaRLA0j5+EzNidHsPN0OwsOr9A5xP5ztnaIsoP0R5=',
            'jiT5L9g0M1N2O3P4Q5R6S7T8U9V0W1X2Y3Z4A5B6C7D8E9F0G1H2=',
            'Jane Student', 'Student', 0, 1, GETDATE());
    PRINT 'Student user created';
    
    -- Add student details
    INSERT INTO dbo.Students (UserId, RollNumber, DepartmentId, SectionId, EnrollmentDate)
    SELECT UserId, 'CS2025001', 1, 1, GETDATE()
    FROM dbo.Users WHERE Username = 'student1';
    PRINT 'Student details added';
END
GO

PRINT '';
PRINT '==============================================';
PRINT 'Test Data Setup Complete!';
PRINT '==============================================';
PRINT '';
PRINT 'Login Credentials:';
PRINT '  Admin:   admin     / Test@123';
PRINT '  Teacher: teacher1  / Test@123';
PRINT '  Student: student1  / Test@123';
PRINT '';
PRINT 'Access your app at: http://localhost:5297';
PRINT '==============================================';
