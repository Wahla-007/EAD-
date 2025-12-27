-- =============================================
-- Test Users Setup Script
-- Run this AFTER running DatabaseSetup.sql
-- =============================================

USE AttendanceManagementDB;
GO

-- Note: These passwords use a simple hash for testing
-- Password for all test users: "Test@123"
-- Hash: "gQNYvPvEm0hG5Z4K9X6/Yw==" (example - you'll need to generate actual hash/salt)

-- =============================================
-- STEP 1: Insert Test Department
-- =============================================
INSERT INTO Departments (DepartmentName, DepartmentCode, IsActive)
VALUES 
    ('Computer Science', 'CS', 1),
    ('Software Engineering', 'SE', 1),
    ('Information Technology', 'IT', 1);

-- =============================================
-- STEP 2: Insert Test Session and Semester
-- =============================================
INSERT INTO Sessions (SessionName, StartDate, EndDate, IsActive)
VALUES ('2025-2026', '2025-09-01', '2026-06-30', 1);

INSERT INTO Semesters (SessionId, SemesterName, SemesterNumber, StartDate, EndDate, IsActive)
VALUES (1, 'Fall 2025', 1, '2025-09-01', '2025-12-31', 1);

-- =============================================
-- STEP 3: Insert Test Section
-- =============================================
INSERT INTO Sections (SectionName, DepartmentId, SemesterId, Capacity, IsActive)
VALUES 
    ('CS-A', 1, 1, 50, 1),
    ('SE-A', 2, 1, 50, 1);

-- =============================================
-- STEP 4: Insert Test Courses
-- =============================================
INSERT INTO Courses (CourseCode, CourseName, CreditHours, DepartmentId, IsActive)
VALUES 
    ('CS101', 'Introduction to Programming', 3, 1, 1),
    ('CS201', 'Data Structures', 3, 1, 1),
    ('CS301', 'Database Systems', 3, 1, 1),
    ('SE201', 'Software Engineering', 3, 2, 1),
    ('SE301', 'Web Development', 3, 2, 1);

-- =============================================
-- STEP 5: Insert Test Users
-- NOTE: You need to generate proper hash/salt using your PasswordHasher
-- For now, I'll show the structure. You'll need to run the C# code to generate proper hashes.
-- =============================================

PRINT '============================================='
PRINT 'IMPORTANT: You need to create users via the application or use the C# PasswordHasher'
PRINT 'The following are placeholder values:'
PRINT '============================================='

-- Admin User (Username: admin, Password: Admin@123)
-- Teacher User (Username: teacher1, Password: Teacher@123)
-- Student User (Username: student1, Password: Student@123)

PRINT 'Database tables are ready!'
PRINT 'Next steps:'
PRINT '1. Use the C# script below to generate hashed passwords'
PRINT '2. Insert users with proper hash/salt values'
GO
