# Fix all property names in views and controllers

# Fix Course.Name -> Course.CourseName
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\CourseAssignments.cshtml") -replace 'Course\?\.Name', 'Course?.CourseName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\CourseAssignments.cshtml"
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Teacher\MyCourses.cshtml") -replace 'Course\?\.Name', 'Course?.CourseName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Teacher\MyCourses.cshtml"
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Teacher\MarkAttendance.cshtml") -replace 'Course\?\.Name', 'Course?.CourseName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Teacher\MarkAttendance.cshtml"
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Teacher\ViewAttendance.cshtml") -replace 'Course\?\.Name', 'Course?.CourseName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Teacher\ViewAttendance.cshtml"
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Student\RegisterCourses.cshtml") -replace 'Course\?\.Name', 'Course?.CourseName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Student\RegisterCourses.cshtml"
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Student\Schedule.cshtml") -replace 'Course\?\.Name', 'Course?.CourseName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Student\Schedule.cshtml"

# Fix Section.Name -> Section.SectionName
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\ManageStudents.cshtml") -replace 'Section\?\.Name', 'Section?.SectionName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\ManageStudents.cshtml"
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\CourseAssignments.cshtml") -replace 'Section\?\.Name', 'Section?.SectionName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\CourseAssignments.cshtml"
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Teacher\MyCourses.cshtml") -replace 'Section\?\.Name', 'Section?.SectionName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Teacher\MyCourses.cshtml"
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Teacher\MarkAttendance.cshtml") -replace 'Section\?\.Name', 'Section?.SectionName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Teacher\MarkAttendance.cshtml"
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Teacher\ViewAttendance.cshtml") -replace 'Section\?\.Name', 'Section?.SectionName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Teacher\ViewAttendance.cshtml"
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Student\RegisterCourses.cshtml") -replace 'Section\?\.Name', 'Section?.SectionName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Student\RegisterCourses.cshtml"

# Fix Semester.Name -> Semester.SemesterName
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\CourseAssignments.cshtml") -replace 'Semester\?\.Name', 'Semester?.SemesterName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\CourseAssignments.cshtml"
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Teacher\MyCourses.cshtml") -replace 'Semester\?\.Name', 'Semester?.SemesterName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Teacher\MyCourses.cshtml"
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Student\RegisterCourses.cshtml") -replace 'Semester\?\.Name', 'Semester?.SemesterName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Student\RegisterCourses.cshtml"

# Fix Department.Name -> Department.DepartmentName
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\ManageStudents.cshtml") -replace 'Department\?\.Name', 'Department?.DepartmentName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\ManageStudents.cshtml"
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\ManageTeachers.cshtml") -replace 'Department\?\.Name', 'Department?.DepartmentName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\ManageTeachers.cshtml"

# Fix Section.Session?.Name -> Section.Semester?.Session?.SessionName
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\ManageStudents.cshtml") -replace 'Section\?\.Session\?\.Name', 'Section?.Semester?.Session?.SessionName' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\ManageStudents.cshtml"

# Fix JavaScript property names in ManageStudents.cshtml
$content = Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\ManageStudents.cshtml" -Raw
$content = $content -replace '\$\{section\.name\}', '${section.sectionName}'
$content | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\ManageStudents.cshtml"

# Fix JavaScript property names in ManageTeachers.cshtml
$content = Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\ManageTeachers.cshtml" -Raw
$content = $content -replace '\$\{course\.name\}', '${course.courseName}'
$content = $content -replace '\$\{section\.name\}', '${section.sectionName}'
$content | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\ManageTeachers.cshtml"

# Fix LMSRegistration fields
(Get-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\LMSRegistrations.cshtml") -replace 'registration\.LmsUsername', 'registration.LMSUsername' | Set-Content "c:\Users\hp\source\repos\EAD_project\Views\Admin\LMSRegistrations.cshtml"

Write-Host "All property names fixed successfully!" -ForegroundColor Green