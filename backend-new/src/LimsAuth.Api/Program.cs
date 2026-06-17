using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using LimsAuth.Api.Authorization;
using LimsAuth.Api.Data;
using LimsAuth.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:SecretKey"] ?? "LimsAuth_SuperSecretKey_2026_MinLength32Chars!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "LimsAuth.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "LimsAuth.Client";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

// Permission policies (Authorization)
builder.Services.AddPermissionAuthorization();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IInstitutionService, InstitutionService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IMajorService, MajorService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<ITeachingTaskService, TeachingTaskService>();
builder.Services.AddScoped<IBuildingService, BuildingService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IExperimentItemService, ExperimentItemService>();
builder.Services.AddScoped<IBookingApplyService, BookingApplyService>();
builder.Services.AddScoped<IUsageRegisterService, UsageRegisterService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<ILoanApplyService, LoanApplyService>();
builder.Services.AddScoped<IConsumableService, ConsumableService>();
builder.Services.AddScoped<IInboundService, InboundService>();
builder.Services.AddScoped<IOutboundService, OutboundService>();
builder.Services.AddScoped<IStockLogService, StockLogService>();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = builder.Configuration["Swagger:Title"] ?? "LIMS API",
        Version = builder.Configuration["Swagger:Version"] ?? "v1",
        Description = builder.Configuration["Swagger:Description"]
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Auto migrate database and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    await SeedData.SeedAsync(db);
}

// Permission policies registration
using (var scope = app.Services.CreateScope())
{
    var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
    await permissionService.RegisterPermissionPoliciesAsync(app.Services);
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LIMS API V1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

var urls = builder.Configuration["Urls"] ?? "http://0.0.0.0:5047";
app.Urls.Add(urls);

app.Run();
