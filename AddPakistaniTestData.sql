-- Add more test data with Pakistani names
-- All users will have password: Test@123
USE master;
GO

-- Add more Pakistani teachers
INSERT INTO Users (Username, Email, FullName, PasswordHash, PasswordSalt, Role, IsActive, IsFirstLogin, CreatedDate)
VALUES 
    ('danish', 'danish@nu.edu.pk', 'Danish Ahmed', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Teacher', 1, 0, GETDATE()),
    ('aizaz', 'aizaz@nu.edu.pk', 'Aizaz Akmal', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Teacher', 1, 0, GETDATE()),
    ('umar', 'umar@nu.edu.pk', 'Umar Qasim', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Teacher', 1, 0, GETDATE()),
    ('shehzad', 'shehzad@nu.edu.pk', 'Shehzad Ali', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Teacher', 1, 0, GETDATE()),
    ('quratulain', 'quratulain@nu.edu.pk', 'QuratulAin Fatima', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Teacher', 1, 0, GETDATE()),
    ('shanfa', 'shanfa@nu.edu.pk', 'Shanfa Irum', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Teacher', 1, 0, GETDATE());

-- Get the user IDs for the new teachers
DECLARE @DanishUserId INT = (SELECT UserId FROM Users WHERE Username = 'danish');
DECLARE @AizazUserId INT = (SELECT UserId FROM Users WHERE Username = 'aizaz');
DECLARE @UmarUserId INT = (SELECT UserId FROM Users WHERE Username = 'umar');
DECLARE @ShehzadUserId INT = (SELECT UserId FROM Users WHERE Username = 'shehzad');
DECLARE @QuratUserId INT = (SELECT UserId FROM Users WHERE Username = 'quratulain');
DECLARE @ShanfaUserId INT = (SELECT UserId FROM Users WHERE Username = 'shanfa');

-- Get department IDs
DECLARE @CSDeptId INT = (SELECT DepartmentId FROM Departments WHERE DepartmentCode = 'CS');
DECLARE @SEDeptId INT = (SELECT DepartmentId FROM Departments WHERE DepartmentCode = 'SE');
DECLARE @ITDeptId INT = (SELECT DepartmentId FROM Departments WHERE DepartmentCode = 'IT');

-- Add teacher records
INSERT INTO Teachers (UserId, EmployeeId, DepartmentId)
VALUES 
    (@DanishUserId, 'EMP002', @CSDeptId),
    (@AizazUserId, 'EMP003', @SEDeptId),
    (@UmarUserId, 'EMP004', @CSDeptId),
    (@ShehzadUserId, 'EMP005', @ITDeptId),
    (@QuratUserId, 'EMP006', @SEDeptId),
    (@ShanfaUserId, 'EMP007', @CSDeptId);

-- Add more Pakistani students
INSERT INTO Users (Username, Email, FullName, PasswordHash, PasswordSalt, Role, IsActive, IsFirstLogin, CreatedDate)
VALUES 
    ('sufwan', 'sufwan@nu.edu.pk', 'Sufwan Khan', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Student', 1, 0, GETDATE()),
    ('saif', 'saif@nu.edu.pk', 'Saif Ullah', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Student', 1, 0, GETDATE()),
    ('qasim', 'qasim@nu.edu.pk', 'Qasim Raza', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Student', 1, 0, GETDATE()),
    ('hamza', 'hamza@nu.edu.pk', 'Hamza Naveed', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Student', 1, 0, GETDATE()),
    ('ali', 'ali@nu.edu.pk', 'Ali Hassan', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Student', 1, 0, GETDATE()),
    ('bilal', 'bilal@nu.edu.pk', 'Bilal Ahmed', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Student', 1, 0, GETDATE()),
    ('faizan', 'faizan@nu.edu.pk', 'Faizan Malik', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Student', 1, 0, GETDATE()),
    ('zain', 'zain@nu.edu.pk', 'Zain Abbas', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Student', 1, 0, GETDATE()),
    ('ayesha', 'ayesha@nu.edu.pk', 'Ayesha Siddiqui', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Student', 1, 0, GETDATE()),
    ('fatima', 'fatima@nu.edu.pk', 'Fatima Zahra', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Student', 1, 0, GETDATE()),
    ('maria', 'maria@nu.edu.pk', 'Maria Nawaz', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Student', 1, 0, GETDATE()),
    ('sara', 'sara@nu.edu.pk', 'Sara Khalid', 
     'a48ADSrruPhLa2gpxqPggc2P6B/MbrHIqK1bJQV127d0lj0ysQiZeDVg8NTTnTVvPPXRwsysgff19lgfl1+RCA==', 
     '687y595xFAOCyuTTyZ4UHgIOIcEUYS4j2rlOjbinrUUEb8ZdHpfwkvT1/yqFP7vercx5I1wN2/J0MWbrho+cHCY2DnHOOU/UL40aAKwoYKQ7Eq5+vcIRolmacYvM6PlT1+RPQTHqcsi1MOPBwFjpy9RQlEuNKormu7gIjUe3mg8=', 
     'Student', 1, 0, GETDATE());

-- Get section IDs
DECLARE @CSSection INT = (SELECT SectionId FROM Sections WHERE SectionName = 'CS-A');
DECLARE @SESection INT = (SELECT SectionId FROM Sections WHERE SectionName = 'SE-A');

-- Get user IDs for new students
DECLARE @SufwanId INT = (SELECT UserId FROM Users WHERE Username = 'sufwan');
DECLARE @SaifId INT = (SELECT UserId FROM Users WHERE Username = 'saif');
DECLARE @QasimId INT = (SELECT UserId FROM Users WHERE Username = 'qasim');
DECLARE @HamzaId INT = (SELECT UserId FROM Users WHERE Username = 'hamza');
DECLARE @AliId INT = (SELECT UserId FROM Users WHERE Username = 'ali');
DECLARE @BilalId INT = (SELECT UserId FROM Users WHERE Username = 'bilal');
DECLARE @FaizanId INT = (SELECT UserId FROM Users WHERE Username = 'faizan');
DECLARE @ZainId INT = (SELECT UserId FROM Users WHERE Username = 'zain');
DECLARE @AyeshaId INT = (SELECT UserId FROM Users WHERE Username = 'ayesha');
DECLARE @FatimaId INT = (SELECT UserId FROM Users WHERE Username = 'fatima');
DECLARE @MariaId INT = (SELECT UserId FROM Users WHERE Username = 'maria');
DECLARE @SaraId INT = (SELECT UserId FROM Users WHERE Username = 'sara');

-- Add student records
INSERT INTO Students (UserId, RollNumber, DepartmentId, SectionId, EnrollmentDate)
VALUES 
    (@SufwanId, '21K-3001', @CSDeptId, @CSSection, '2021-09-01'),
    (@SaifId, '21K-3002', @CSDeptId, @CSSection, '2021-09-01'),
    (@QasimId, '21K-3003', @CSDeptId, @CSSection, '2021-09-01'),
    (@HamzaId, '21K-3004', @CSDeptId, @CSSection, '2021-09-01'),
    (@AliId, '21K-3005', @CSDeptId, @CSSection, '2021-09-01'),
    (@BilalId, '21K-4001', @SEDeptId, @SESection, '2021-09-01'),
    (@FaizanId, '21K-4002', @SEDeptId, @SESection, '2021-09-01'),
    (@ZainId, '21K-4003', @SEDeptId, @SESection, '2021-09-01'),
    (@AyeshaId, '21K-3006', @CSDeptId, @CSSection, '2021-09-01'),
    (@FatimaId, '21K-3007', @CSDeptId, @CSSection, '2021-09-01'),
    (@MariaId, '21K-4004', @SEDeptId, @SESection, '2021-09-01'),
    (@SaraId, '21K-4005', @SEDeptId, @SESection, '2021-09-01');

PRINT '========================================';
PRINT 'Pakistani names test data added successfully!';
PRINT '========================================';
PRINT 'Added 6 new teachers:';
PRINT '  - danish / Test@123';
PRINT '  - aizaz / Test@123';
PRINT '  - umar / Test@123';
PRINT '  - shehzad / Test@123';
PRINT '  - quratulain / Test@123';
PRINT '  - shanfa / Test@123';
PRINT '';
PRINT 'Added 12 new students:';
PRINT '  - sufwan / Test@123';
PRINT '  - saif / Test@123';
PRINT '  - qasim / Test@123';
PRINT '  - hamza / Test@123';
PRINT '  - ali / Test@123';
PRINT '  - bilal / Test@123';
PRINT '  - faizan / Test@123';
PRINT '  - zain / Test@123';
PRINT '  - ayesha / Test@123';
PRINT '  - fatima / Test@123';
PRINT '  - maria / Test@123';
PRINT '  - sara / Test@123';
PRINT '========================================';
