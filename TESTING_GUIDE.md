# QUICK TESTING GUIDE

## 🚀 Application is Running at: http://localhost:5297

## TEST CREDENTIALS
- **Admin**: admin / Test@123
- **Teacher**: teacher1 / Test@123
- **Student**: student1 / Test@123

---

## ✅ ADMIN TESTS (3 Features)

### Test 1: Assign Student to Section
1. Login as **admin/Test@123**
2. Click **"Students"** in sidebar
3. Click **"Assign to Section"** button (top right)
4. Click **"Assign Section"** button for student1
5. Select a section from dropdown (CS-A or SE-A)
6. Click **"Assign"**
7. ✅ Success message appears, page refreshes, student now shows assigned section

### Test 2: Assign Teacher to Course
1. Still logged in as admin
2. Click **"Teachers"** in sidebar
3. Click **"Assign to Course"** button (top right)
4. Click **"Assign Course"** button for teacher1
5. Select course (e.g., CS101 - Programming Fundamentals)
6. Select section (e.g., CS-A)
7. Click **"Assign"**
8. ✅ Success message, teacher now has 1 course assigned

### Test 3: Create LMS Registration
1. Click **"LMS Registrations"** in sidebar
2. Click **"Create Registration"** button
3. Select user from dropdown
4. Enter LMS Username: "student1_lms"
5. Enter LMS Password: "lms123"
6. Click **"Create"**
7. ✅ New registration appears in table

---

## ✅ TEACHER TESTS (2 Features)

### Test 4: Mark Attendance
1. Logout → Login as **teacher1/Test@123**
2. Click **"My Courses"** in sidebar
3. Click **"Mark Attendance"** button on any course card
4. Course is auto-selected (or select from dropdown)
5. Date is today (or change it)
6. Click **"Load"** button
7. ✅ List of students appears
8. Click **"Present"** for each student (or click "Mark All Present")
9. Click **"Submit Attendance"**
10. ✅ Success message appears

### Test 5: View Attendance
1. Click **"View Attendance"** link (top right of Mark Attendance page)
2. Select course from dropdown
3. Select date (use same date you just marked attendance)
4. Click **"View"**
5. ✅ See statistics: Total Students, Present, Absent, Attendance %
6. ✅ See detailed table with all students and their attendance

---

## ✅ STUDENT TESTS (3 Features)

### Test 6: Register for Course
1. Logout → Login as **student1/Test@123**
2. Click **"Register Courses"** in sidebar
3. ✅ See available courses for your section
4. Click **"Register"** button on any course
5. ✅ Success message appears
6. ✅ Button changes or course marked as registered

### Test 7: View My Courses
1. Click **"My Courses"** in sidebar
2. ✅ See all enrolled courses with teacher names
3. ✅ Each course shows credit hours, section, semester

### Test 8: View My Attendance
1. Click **"My Attendance"** in sidebar
2. ✅ See attendance percentage for each enrolled course
3. ✅ Color-coded cards: Green (≥75%), Pink (60-74%), Red (<60%)
4. ✅ Each card shows course name and percentage

---

## ✅ ADDITIONAL FEATURES TO SHOW

### Schedule/Timetable
1. As Student, click **"Schedule"** in sidebar
2. ✅ Shows weekly timetable if admin has set it up
3. ✅ Empty state message if no timetable

### Change Password
1. Any user, click **"Change Password"** in sidebar
2. Enter old password: Test@123
3. Enter new password: Test@456
4. Confirm new password: Test@456
5. Click **"Change Password"**
6. ✅ Automatically logs out
7. ✅ Can login with new password

### Dashboards
- ✅ **Admin Dashboard**: Purple theme with stats cards
- ✅ **Teacher Dashboard**: Green theme with stats cards
- ✅ **Student Dashboard**: Pink theme with stats cards

---

## 🎯 EVALUATION CHECKLIST

| Feature | Status | Evidence |
|---------|--------|----------|
| Login/Logout | ✅ | Works for all 3 roles |
| Admin - Assign Students to Sections | ✅ | Working modal + database update |
| Admin - Assign Teachers to Courses | ✅ | Working modal + database update |
| Admin - LMS Registrations | ✅ | Working modal + database update |
| Teacher - Mark Attendance | ✅ | Full form with student list |
| Teacher - View Attendance | ✅ | Statistics + detailed table |
| Student - Register Courses | ✅ | Available courses + register button |
| Student - View Courses | ✅ | Enrolled courses display |
| Student - View Attendance | ✅ | Percentage per course |
| Change Password | ✅ | Works with validation |
| Role-based Dashboards | ✅ | 3 different themed dashboards |
| Beautiful UI | ✅ | Modern gradient design |
| Database Integration | ✅ | LocalDB with EF Core |
| JWT Authentication | ✅ | HttpOnly cookies |
| Service Layer | ✅ | Proper architecture |

---

## 🚨 TROUBLESHOOTING

### If application is not running:
```bash
cd c:\Users\hp\source\repos\EAD_project
dotnet run
```

### If database errors occur:
1. Open SQL Server Object Explorer in Visual Studio
2. Connect to (localdb)\MSSQLLocalDB
3. Verify tables exist in "master" database

### If login fails:
- Verify using correct credentials: admin/Test@123 (case-sensitive password)
- Check if User table has records

---

## 💡 DEMO TIP

**Suggested order for live demonstration:**
1. Login page → Show authentication
2. Admin dashboard → Assign student to section
3. Admin → Assign teacher to course  
4. Logout → Teacher login
5. Teacher → Mark attendance (show the interactive form)
6. Teacher → View attendance (show statistics)
7. Logout → Student login
8. Student → Register for course
9. Student → View my courses
10. Student → View my attendance

**Total demo time: 5-7 minutes**

---

## ✨ KEY SELLING POINTS

1. **Fully Functional** - Not just UI mockups, actual working CRUD operations
2. **Database Integrated** - Real data persistence with SQL Server
3. **Professional Architecture** - Service layer, repository pattern, dependency injection
4. **Security** - JWT authentication, password hashing, role-based authorization
5. **Modern UI** - Beautiful gradients, responsive design, interactive modals
6. **Production-Ready** - No placeholders in core features, proper error handling

---

**YOU ARE READY FOR EVALUATION! 🎉**