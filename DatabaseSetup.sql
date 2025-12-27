-- =============================================
-- Attendance Management System Database
-- =============================================

USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'AttendanceManagementDB')
BEGIN
    ALTER DATABASE AttendanceManagementDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE AttendanceManagementDB;
END
GO

-- Create Database
CREATE DATABASE AttendanceManagementDB;
GO

USE AttendanceManagementDB;
GO

-- =============================================
-- TABLES
-- =============================================

-- Users Table (Base for all roles)
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    PasswordSalt NVARCHAR(MAX) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin', 'Teacher', 'Student')),
    IsFirstLogin BIT DEFAULT 1,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETDATE(),
    LastLoginDate DATETIME2 NULL,
    ProfileImage NVARCHAR(MAX) NULL
);

-- Departments Table
CREATE TABLE Departments (
    DepartmentId INT PRIMARY KEY IDENTITY(1,1),
    DepartmentName NVARCHAR(100) NOT NULL,
    DepartmentCode NVARCHAR(20) UNIQUE NOT NULL,
    IsActive BIT DEFAULT 1
);

-- Sessions Table (Academic Years)
CREATE TABLE Sessions (
    SessionId INT PRIMARY KEY IDENTITY(1,1),
    SessionName NVARCHAR(50) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    IsActive BIT DEFAULT 1
);

-- Semesters Table
CREATE TABLE Semesters (
    SemesterId INT PRIMARY KEY IDENTITY(1,1),
    SessionId INT FOREIGN KEY REFERENCES Sessions(SessionId),
    SemesterName NVARCHAR(20) NOT NULL,
    SemesterNumber INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    IsActive BIT DEFAULT 1
);

-- Sections Table
CREATE TABLE Sections (
    SectionId INT PRIMARY KEY IDENTITY(1,1),
    SectionName NVARCHAR(50) NOT NULL,
    DepartmentId INT FOREIGN KEY REFERENCES Departments(DepartmentId),
    SemesterId INT FOREIGN KEY REFERENCES Semesters(SemesterId),
    Capacity INT NOT NULL DEFAULT 50,
    IsActive BIT DEFAULT 1
);

-- Courses Table
CREATE TABLE Courses (
    CourseId INT PRIMARY KEY IDENTITY(1,1),
    CourseCode NVARCHAR(20) UNIQUE NOT NULL,
    CourseName NVARCHAR(100) NOT NULL,
    CreditHours INT NOT NULL,
    DepartmentId INT FOREIGN KEY REFERENCES Departments(DepartmentId),
    IsActive BIT DEFAULT 1
);

-- Student Details
CREATE TABLE Students (
    StudentId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId) ON DELETE CASCADE,
    RollNumber NVARCHAR(50) UNIQUE NOT NULL,
    DepartmentId INT FOREIGN KEY REFERENCES Departments(DepartmentId),
    SectionId INT FOREIGN KEY REFERENCES Sections(SectionId) NULL,
    EnrollmentDate DATE DEFAULT GETDATE(),
    CGPA DECIMAL(3,2) NULL
);

-- Teacher Details
CREATE TABLE Teachers (
    TeacherId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId) ON DELETE CASCADE,
    EmployeeId NVARCHAR(50) UNIQUE NOT NULL,
    DepartmentId INT FOREIGN KEY REFERENCES Departments(DepartmentId),
    Designation NVARCHAR(50) NULL,
    Qualification NVARCHAR(100) NULL
);

-- Course Assignments
CREATE TABLE CourseAssignments (
    AssignmentId INT PRIMARY KEY IDENTITY(1,1),
    CourseId INT FOREIGN KEY REFERENCES Courses(CourseId),
    SectionId INT FOREIGN KEY REFERENCES Sections(SectionId),
    SemesterId INT FOREIGN KEY REFERENCES Semesters(SemesterId),
    TeacherId INT FOREIGN KEY REFERENCES Teachers(TeacherId) NULL,
    IsActive BIT DEFAULT 1,
    CONSTRAINT UK_CourseAssignment UNIQUE(CourseId, SectionId, SemesterId)
);

-- Student Course Registrations
CREATE TABLE StudentCourseRegistrations (
    RegistrationId INT PRIMARY KEY IDENTITY(1,1),
    StudentId INT FOREIGN KEY REFERENCES Students(StudentId),
    AssignmentId INT FOREIGN KEY REFERENCES CourseAssignments(AssignmentId),
    RegistrationDate DATETIME2 DEFAULT GETDATE(),
    Status NVARCHAR(20) DEFAULT 'Registered' CHECK (Status IN ('Registered', 'Dropped', 'Completed')),
    CONSTRAINT UK_StudentCourseReg UNIQUE(StudentId, AssignmentId)
);

-- Timetable Table
CREATE TABLE Timetable (
    TimetableId INT PRIMARY KEY IDENTITY(1,1),
    AssignmentId INT FOREIGN KEY REFERENCES CourseAssignments(AssignmentId),
    DayOfWeek INT NOT NULL CHECK (DayOfWeek BETWEEN 1 AND 7),
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    RoomNumber NVARCHAR(50) NULL,
    IsActive BIT DEFAULT 1
);

-- Attendance Table
CREATE TABLE Attendance (
    AttendanceId INT PRIMARY KEY IDENTITY(1,1),
    RegistrationId INT FOREIGN KEY REFERENCES StudentCourseRegistrations(RegistrationId),
    AttendanceDate DATE NOT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Present', 'Absent', 'Late', 'Leave')),
    MarkedBy INT FOREIGN KEY REFERENCES Users(UserId),
    MarkedDate DATETIME2 DEFAULT GETDATE(),
    Remarks NVARCHAR(MAX) NULL,
    CONSTRAINT UK_Attendance UNIQUE(RegistrationId, AttendanceDate)
);

-- LMS Registration Table
CREATE TABLE LMSRegistrations (
    LMSRegistrationId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    LMSUsername NVARCHAR(100) NOT NULL,
    LMSPassword NVARCHAR(MAX) NOT NULL,
    RegistrationDate DATETIME2 DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);

-- Refresh Tokens for JWT
CREATE TABLE RefreshTokens (
    TokenId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    Token NVARCHAR(MAX) NOT NULL,
    ExpiryDate DATETIME2 NOT NULL,
    IsRevoked BIT DEFAULT 0,
    CreatedDate DATETIME2 DEFAULT GETDATE()
);

-- Notifications Table
CREATE TABLE Notifications (
    NotificationId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    IsRead BIT DEFAULT 0,
    CreatedDate DATETIME2 DEFAULT GETDATE(),
    NotificationType NVARCHAR(50) NULL
);

-- Audit Log
CREATE TABLE AuditLogs (
    LogId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId) NULL,
    Action NVARCHAR(100) NOT NULL,
    TableName NVARCHAR(50) NULL,
    RecordId INT NULL,
    OldValues NVARCHAR(MAX) NULL,
    NewValues NVARCHAR(MAX) NULL,
    IPAddress NVARCHAR(50) NULL,
    CreatedDate DATETIME2 DEFAULT GETDATE()
);

GO

-- =============================================
-- INDEXES
-- =============================================

CREATE INDEX IX_Users_Username ON Users(Username);
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Students_RollNumber ON Students(RollNumber);
CREATE INDEX IX_Teachers_EmployeeId ON Teachers(EmployeeId);
CREATE INDEX IX_Attendance_Date ON Attendance(AttendanceDate);
CREATE INDEX IX_Attendance_RegistrationId ON Attendance(RegistrationId);
CREATE INDEX IX_CourseAssignments_SectionId ON CourseAssignments(SectionId);
CREATE INDEX IX_Timetable_DayOfWeek ON Timetable(DayOfWeek);

GO

-- =============================================
-- SEED DATA
-- =============================================

-- Insert Admin User (Password: Admin@123)
-- Note: You'll need to generate proper hash/salt in your application
INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, FullName, Role, IsFirstLogin, CreatedDate) 
VALUES 
('admin', 'admin@university.edu', 
 'AQAAAAIAAYagAAAAEKpxqZtUYHrYiZQOXmN+xQVG8V0L9FqrJwX3pK5mN2B8D4C6E8F0H2J4L6N8P0R2T4V6', 
 'randomsalt123', 'System Administrator', 'Admin', 0, GETDATE());

-- Insert Sample Departments
INSERT INTO Departments (DepartmentName, DepartmentCode, IsActive) 
VALUES 
('Computer Science', 'CS', 1),
('Electrical Engineering', 'EE', 1),
('Mechanical Engineering', 'ME', 1),
('Civil Engineering', 'CE', 1);

-- Insert Sample Session
INSERT INTO Sessions (SessionName, StartDate, EndDate, IsActive) 
VALUES ('2024-2025', '2024-09-01', '2025-08-31', 1);

-- Insert Sample Semesters
INSERT INTO Semesters (SessionId, SemesterName, SemesterNumber, StartDate, EndDate, IsActive) 
VALUES 
(1, 'Fall 2024', 1, '2024-09-01', '2025-01-15', 1),
(1, 'Spring 2025', 2, '2025-01-16', '2025-06-15', 1);

-- Insert Sample Sections
INSERT INTO Sections (SectionName, DepartmentId, SemesterId, Capacity, IsActive) 
VALUES 
('CS-A', 1, 1, 50, 1),
('CS-B', 1, 1, 50, 1),
('EE-A', 2, 1, 45, 1);

-- Insert Sample Courses
INSERT INTO Courses (CourseCode, CourseName, CreditHours, DepartmentId, IsActive) 
VALUES 
('CS101', 'Introduction to Programming', 3, 1, 1),
('CS102', 'Data Structures', 3, 1, 1),
('CS201', 'Database Systems', 3, 1, 1),
('EE101', 'Circuit Analysis', 3, 2, 1),
('EE102', 'Digital Logic Design', 3, 2, 1);

GO

PRINT 'Database setup completed successfully!';
GO