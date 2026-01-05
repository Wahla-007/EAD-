using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AttendanceManagementSystem.Data;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Database Context - Using SQLite for cloud deployment
var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "AttendanceDB.db");
builder.Services.AddDbContext<AttendanceManagementDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key not found");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("AccessToken"))
            {
                context.Token = context.Request.Cookies["AccessToken"];
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
    options.AddPolicy("AdminOrTeacher", policy => policy.RequireRole("Admin", "Teacher"));
});

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.Name = ".AttendanceSystem.Session"; // Use a specific name
});

builder.Services.AddHttpContextAccessor();

// Configure Data Protection with a stable key from environment variable
var dataProtectionKey = builder.Configuration["DATA_PROTECTION_KEY"] 
    ?? Environment.GetEnvironmentVariable("DATA_PROTECTION_KEY");

if (!string.IsNullOrEmpty(dataProtectionKey))
{
    // Use the provided key for Data Protection
    var keysDirectory = Path.Combine(Directory.GetCurrentDirectory(), "keys");
    try
    {
        Directory.CreateDirectory(keysDirectory);
    }
    catch (Exception ex)
    {
        // Log the error but continue - the app will fail on first use if keys can't be persisted
        Console.WriteLine($"Warning: Failed to create keys directory at {keysDirectory}: {ex.Message}");
    }
    
    builder.Services.AddDataProtection()
        .SetApplicationName("AttendanceManagementSystem")
        .PersistKeysToFileSystem(new DirectoryInfo(keysDirectory));
}
else
{
    // Fallback: Use ephemeral keys but handle cookie errors gracefully
    builder.Services.AddDataProtection()
        .SetApplicationName("AttendanceManagementSystem")
        .UseEphemeralDataProtectionProvider();
}

// Register Services
builder.Services.AddScoped<AttendanceManagementSystem.Services.Interfaces.IAuthService, AttendanceManagementSystem.Services.Implementations.AuthService>();
builder.Services.AddScoped<AttendanceManagementSystem.Services.Interfaces.IStudentService, AttendanceManagementSystem.Services.Implementations.StudentService>();
builder.Services.AddScoped<AttendanceManagementSystem.Services.Interfaces.ITeacherService, AttendanceManagementSystem.Services.Implementations.TeacherService>();
builder.Services.AddScoped<AttendanceManagementSystem.Services.Interfaces.IAdminService, AttendanceManagementSystem.Services.Implementations.AdminService>();

var app = builder.Build();

// Configure Forwarded Headers for DigitalOcean/Proxy
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

// Seed test users with proper password hashes
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AttendanceManagementDbContext>();

    // Ensure fresh start for demo data
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    SeedUsers.SeedTestUsers(context);
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add middleware to handle invalid session cookies
app.UseMiddleware<AttendanceManagementSystem.Middleware.SessionCookieMiddleware>();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
