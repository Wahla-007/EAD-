using Microsoft.EntityFrameworkCore;
using AttendanceManagementSystem.Data.Entities;

namespace AttendanceManagementSystem.Data
{
    public class AttendanceManagementDbContext : DbContext
    {
        public AttendanceManagementDbContext(DbContextOptions<AttendanceManagementDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }
        public DbSet<StudentCourseRegistration> StudentCourseRegistrations { get; set; }
        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<LmsRegistration> LmsRegistrations { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique indexes
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.RollNumber)
                .IsUnique();

            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.EmployeeId)
                .IsUnique();

            modelBuilder.Entity<Course>()
                .HasIndex(c => c.CourseCode)
                .IsUnique();

            modelBuilder.Entity<Department>()
                .HasIndex(d => d.DepartmentCode)
                .IsUnique();

            // Configure cascade delete behavior
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.User)
                .WithOne(u => u.Teacher)
                .HasForeignKey<Teacher>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}