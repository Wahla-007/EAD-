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

// Configure Data Protection - Use ephemeral provider for stateless deployment
// This is fine because we use JWT for authentication, not sessions
builder.Services.AddDataProtection()
    .SetApplicationName("AttendanceManagementSystem")
    .UseEphemeralDataProtectionProvider();

// Configure session state with memory cache for temporary UI state
// JWT cookies handle authentication
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.Name = ".AttendanceSystem.Session";
});

builder.Services.AddHttpContextAccessor();

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
// 🪐 Antigravity: Launches text in UTF-8 space!
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Type", "text/html; charset=utf-8");
    await next();
});

// Prevent browser caching of authenticated pages (fixes back button after logout)
app.Use(async (context, next) =>
{
    // Add no-cache headers for HTML pages (not static files)
    if (!context.Request.Path.StartsWithSegments("/css") &&
        !context.Request.Path.StartsWithSegments("/js") &&
        !context.Request.Path.StartsWithSegments("/lib") &&
        !context.Request.Path.StartsWithSegments("/images") &&
        !context.Request.Path.Value?.Contains(".") == true)
    {
        context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        context.Response.Headers["Pragma"] = "no-cache";
        context.Response.Headers["Expires"] = "0";
    }
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add middleware to clear invalid cookies BEFORE session middleware
app.Use(async (context, next) =>
{
    // Wrap session processing in try-catch when session cookies are present
    // to handle cryptographic exceptions from invalid/old cookies
    if (context.Request.Cookies.ContainsKey(".AttendanceSystem.Session") || 
        context.Request.Cookies.ContainsKey(".AspNetCore.Session"))
    {
        try
        {
            await next();
        }
        catch (System.Security.Cryptography.CryptographicException)
        {
            // Clear invalid cookies
            context.Response.Cookies.Delete(".AttendanceSystem.Session");
            context.Response.Cookies.Delete(".AspNetCore.Session");
            context.Response.Cookies.Delete("AccessToken");
            
            // Redirect to login
            if (!context.Response.HasStarted)
            {
                context.Response.Redirect("/Login");
            }
        }
    }
    else
    {
        await next();
    }
});

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
