using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using LimsAuth.Api.Data;
using LimsAuth.Api.Services;
using LimsAuth.Api.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<ExperimentService>();


// Teaching Management Services
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IMajorService, MajorService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<ITeachingTaskService, TeachingTaskService>();
builder.Services.AddScoped<IPeriodTimeService, PeriodTimeService>();
builder.Services.AddScoped<IAcademicCalendarService, AcademicCalendarService>();

// Lab & Equipment Services
builder.Services.AddScoped<ILabService, LabService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();

// Campus & Building Services
builder.Services.AddScoped<ICampusService, CampusService>();
builder.Services.AddScoped<IBuildingService, BuildingService>();

// JWT Authentication
var secretKey = builder.Configuration["Jwt:SecretKey"] ?? "your-super-secret-key-min-32-chars-long!!";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "LimsAuth",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "LimsClient",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization with custom permission handler
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddAuthorization(options =>
{
    // 定义基于权限的策略
    options.AddPolicy("Permission:user:create", policy => policy.RequirePermission("user:create"));
    options.AddPolicy("Permission:user:read", policy => policy.RequirePermission("user:read"));
    options.AddPolicy("Permission:user:update", policy => policy.RequirePermission("user:update"));
    options.AddPolicy("Permission:user:delete", policy => policy.RequirePermission("user:delete"));
    options.AddPolicy("Permission:user:reset_password", policy => policy.RequirePermission("user:reset_password"));

    options.AddPolicy("Permission:role:create", policy => policy.RequirePermission("role:create"));
    options.AddPolicy("Permission:role:read", policy => policy.RequirePermission("role:read"));
    options.AddPolicy("Permission:role:update", policy => policy.RequirePermission("role:update"));
    options.AddPolicy("Permission:role:delete", policy => policy.RequirePermission("role:delete"));
    options.AddPolicy("Permission:role:assign", policy => policy.RequirePermission("role:assign"));

    options.AddPolicy("Permission:permission:read", policy => policy.RequirePermission("permission:read"));
    options.AddPolicy("Permission:permission:assign", policy => policy.RequirePermission("permission:assign"));

    options.AddPolicy("Permission:department:create", policy => policy.RequirePermission("department:create"));
    options.AddPolicy("Permission:department:read", policy => policy.RequirePermission("department:read"));
    options.AddPolicy("Permission:department:update", policy => policy.RequirePermission("department:update"));
    options.AddPolicy("Permission:department:delete", policy => policy.RequirePermission("department:delete"));

    options.AddPolicy("Permission:lab:create", policy => policy.RequirePermission("lab:create"));
    options.AddPolicy("Permission:lab:read", policy => policy.RequirePermission("lab:read"));
    options.AddPolicy("Permission:lab:update", policy => policy.RequirePermission("lab:update"));
    options.AddPolicy("Permission:lab:delete", policy => policy.RequirePermission("lab:delete"));

    options.AddPolicy("Permission:equipment:create", policy => policy.RequirePermission("equipment:create"));
    options.AddPolicy("Permission:equipment:read", policy => policy.RequirePermission("equipment:read"));
    options.AddPolicy("Permission:equipment:update", policy => policy.RequirePermission("equipment:update"));
    options.AddPolicy("Permission:equipment:delete", policy => policy.RequirePermission("equipment:delete"));

    // Course permissions
    options.AddPolicy("Permission:course:create", policy => policy.RequirePermission("course:create"));
    options.AddPolicy("Permission:course:read", policy => policy.RequirePermission("course:read"));
    options.AddPolicy("Permission:course:update", policy => policy.RequirePermission("course:update"));
    options.AddPolicy("Permission:course:delete", policy => policy.RequirePermission("course:delete"));
    options.AddPolicy("Permission:course:schedule", policy => policy.RequirePermission("course:schedule"));

    // Calendar permissions
    options.AddPolicy("Permission:calendar:read", policy => policy.RequirePermission("calendar:read"));
    options.AddPolicy("Permission:calendar:update", policy => policy.RequirePermission("calendar:update"));

    // Period time permissions
    options.AddPolicy("Permission:period_time:create", policy => policy.RequirePermission("period_time:create"));
    options.AddPolicy("Permission:period_time:read", policy => policy.RequirePermission("period_time:read"));
    options.AddPolicy("Permission:period_time:update", policy => policy.RequirePermission("period_time:update"));
    options.AddPolicy("Permission:period_time:delete", policy => policy.RequirePermission("period_time:delete"));

    // Major permissions
    options.AddPolicy("Permission:major:create", policy => policy.RequirePermission("major:create"));
    options.AddPolicy("Permission:major:read", policy => policy.RequirePermission("major:read"));
    options.AddPolicy("Permission:major:update", policy => policy.RequirePermission("major:update"));
    options.AddPolicy("Permission:major:delete", policy => policy.RequirePermission("major:delete"));

    // Class permissions
    options.AddPolicy("Permission:class:create", policy => policy.RequirePermission("class:create"));
    options.AddPolicy("Permission:class:read", policy => policy.RequirePermission("class:read"));
    options.AddPolicy("Permission:class:update", policy => policy.RequirePermission("class:update"));
    options.AddPolicy("Permission:class:delete", policy => policy.RequirePermission("class:delete"));

    // Campus permissions
    options.AddPolicy("Permission:campus:create", policy => policy.RequirePermission("campus:create"));
    options.AddPolicy("Permission:campus:read", policy => policy.RequirePermission("campus:read"));
    options.AddPolicy("Permission:campus:update", policy => policy.RequirePermission("campus:update"));
    options.AddPolicy("Permission:campus:delete", policy => policy.RequirePermission("campus:delete"));

    // Building permissions
    options.AddPolicy("Permission:building:create", policy => policy.RequirePermission("building:create"));
    options.AddPolicy("Permission:building:read", policy => policy.RequirePermission("building:read"));
    options.AddPolicy("Permission:building:update", policy => policy.RequirePermission("building:update"));
    options.AddPolicy("Permission:building:delete", policy => policy.RequirePermission("building:delete"));
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // 确保数据库和表已创建（使用 EnsureCreated 代替 Migrate，适用于 SQLite）
        await dbContext.Database.EnsureCreatedAsync();
        logger.LogInformation("数据库已创建");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "数据库创建失败");
    }
}

app.Run();
