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

// JSON serialization: ignore circular references + camelCase for incoming requests
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// Services
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<ExperimentService>();
builder.Services.AddSingleton<ExportService>();

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
builder.Services.AddScoped<IEquipmentBorrowService, EquipmentBorrowService>();

// Campus & Building Services
builder.Services.AddScoped<ICampusService, CampusService>();
builder.Services.AddScoped<IBuildingService, BuildingService>();

// 排课预约管理
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ITeachingApplicationService, TeachingApplicationService>();
builder.Services.AddScoped<IUsageRegistrationService, UsageRegistrationService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

// 耗材管理
builder.Services.AddScoped<IConsumableService, ConsumableService>();

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
    options.AddPolicy("Permission:equipment:borrow", policy => policy.RequirePermission("equipment:borrow"));
    options.AddPolicy("Permission:equipment:approve", policy => policy.RequirePermission("equipment:approve"));
    options.AddPolicy("Permission:equipment:statistics", policy => policy.RequirePermission("equipment:statistics"));
    options.AddPolicy("Permission:equipment:export", policy => policy.RequirePermission("equipment:export"));

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

    // 排课管理权限
    options.AddPolicy("Permission:schedule:read", policy => policy.RequirePermission("schedule:read"));
    options.AddPolicy("Permission:schedule:create", policy => policy.RequirePermission("schedule:create"));
    options.AddPolicy("Permission:schedule:update", policy => policy.RequirePermission("schedule:update"));
    options.AddPolicy("Permission:schedule:delete", policy => policy.RequirePermission("schedule:delete"));

    // 预约管理权限
    options.AddPolicy("Permission:reservation:read", policy => policy.RequirePermission("reservation:read"));
    options.AddPolicy("Permission:reservation:create", policy => policy.RequirePermission("reservation:create"));
    options.AddPolicy("Permission:reservation:approve", policy => policy.RequirePermission("reservation:approve"));
    options.AddPolicy("Permission:reservation:cancel", policy => policy.RequirePermission("reservation:cancel"));

    // 授课申请权限
    options.AddPolicy("Permission:teaching_application:read", policy => policy.RequirePermission("teaching_application:read"));
    options.AddPolicy("Permission:teaching_application:create", policy => policy.RequirePermission("teaching_application:create"));
    options.AddPolicy("Permission:teaching_application:approve", policy => policy.RequirePermission("teaching_application:approve"));

    // 使用登记权限
    options.AddPolicy("Permission:usage_registration:read", policy => policy.RequirePermission("usage_registration:read"));
    options.AddPolicy("Permission:usage_registration:create", policy => policy.RequirePermission("usage_registration:create"));

    // 统计报表权限
    options.AddPolicy("Permission:statistics:read", policy => policy.RequirePermission("statistics:read"));
    options.AddPolicy("Permission:statistics:export", policy => policy.RequirePermission("statistics:export"));
    options.AddPolicy("Permission:statistics:dashboard", policy => policy.RequirePermission("statistics:dashboard"));

    // 耗材管理权限
    options.AddPolicy("Permission:consumable:read", policy => policy.RequirePermission("consumable:read"));
    options.AddPolicy("Permission:consumable:create", policy => policy.RequirePermission("consumable:create"));
    options.AddPolicy("Permission:consumable:update", policy => policy.RequirePermission("consumable:update"));
    options.AddPolicy("Permission:consumable:delete", policy => policy.RequirePermission("consumable:delete"));
    options.AddPolicy("Permission:consumable:in", policy => policy.RequirePermission("consumable:in"));
    options.AddPolicy("Permission:consumable:out", policy => policy.RequirePermission("consumable:out"));
    options.AddPolicy("Permission:consumable:approve", policy => policy.RequirePermission("consumable:approve"));
    options.AddPolicy("Permission:consumable:adjust", policy => policy.RequirePermission("consumable:adjust"));
    options.AddPolicy("Permission:consumable:statistics", policy => policy.RequirePermission("consumable:statistics"));
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

        // 兼容性修补：为已有数据库创建新的耗材相关表
        var conn = dbContext.Database.GetDbConnection();
        await conn.OpenAsync();

        var consumableTables = new[]
        {
            (@"
                CREATE TABLE IF NOT EXISTS consumable_categories (
                    id TEXT NOT NULL PRIMARY KEY,
                    name TEXT NOT NULL,
                    remark TEXT,
                    is_active INTEGER NOT NULL DEFAULT 1,
                    created_at TEXT NOT NULL,
                    updated_at TEXT NOT NULL
                )", "consumable_categories"),
            (@"
                CREATE TABLE IF NOT EXISTS consumables (
                    id TEXT NOT NULL PRIMARY KEY,
                    code TEXT NOT NULL,
                    name TEXT NOT NULL,
                    category_id TEXT,
                    specification TEXT,
                    unit TEXT NOT NULL DEFAULT '个',
                    current_stock REAL NOT NULL DEFAULT 0,
                    available_stock REAL NOT NULL DEFAULT 0,
                    locked_stock REAL NOT NULL DEFAULT 0,
                    min_stock REAL NOT NULL DEFAULT 0,
                    location TEXT,
                    supplier TEXT,
                    unit_price REAL,
                    max_single_request REAL NOT NULL DEFAULT 999999,
                    monthly_limit REAL NOT NULL DEFAULT 999999,
                    description TEXT,
                    is_active INTEGER NOT NULL DEFAULT 1,
                    is_deleted INTEGER NOT NULL DEFAULT 0,
                    created_at TEXT NOT NULL,
                    updated_at TEXT NOT NULL,
                    FOREIGN KEY (category_id) REFERENCES consumable_categories(id)
                )", "consumables"),
            (@"
                CREATE TABLE IF NOT EXISTS consumable_in_records (
                    id TEXT NOT NULL PRIMARY KEY,
                    record_no TEXT NOT NULL,
                    consumable_id TEXT NOT NULL,
                    quantity REAL NOT NULL,
                    unit_price REAL,
                    supplier TEXT,
                    in_time TEXT NOT NULL,
                    handler_id TEXT NOT NULL,
                    handler_name TEXT,
                    remark TEXT,
                    status TEXT NOT NULL DEFAULT 'Pending',
                    approved_by TEXT,
                    approver_name TEXT,
                    approved_at TEXT,
                    approval_remark TEXT,
                    is_deleted INTEGER NOT NULL DEFAULT 0,
                    created_at TEXT NOT NULL,
                    FOREIGN KEY (consumable_id) REFERENCES consumables(id)
                )", "consumable_in_records"),
            (@"
                CREATE TABLE IF NOT EXISTS consumable_out_records (
                    id TEXT NOT NULL PRIMARY KEY,
                    record_no TEXT NOT NULL,
                    consumable_id TEXT NOT NULL,
                    quantity REAL NOT NULL,
                    usage_purpose TEXT,
                    usage_lab TEXT,
                    out_time TEXT NOT NULL,
                    applicant_id TEXT NOT NULL,
                    applicant_name TEXT,
                    remark TEXT,
                    status TEXT NOT NULL DEFAULT 'Pending',
                    approved_by TEXT,
                    approver_name TEXT,
                    approved_at TEXT,
                    approval_remark TEXT,
                    is_deleted INTEGER NOT NULL DEFAULT 0,
                    created_at TEXT NOT NULL,
                    FOREIGN KEY (consumable_id) REFERENCES consumables(id)
                )", "consumable_out_records"),
            (@"
                CREATE TABLE IF NOT EXISTS consumable_stock_adjustments (
                    id TEXT NOT NULL PRIMARY KEY,
                    consumable_id TEXT NOT NULL,
                    adjustment_type TEXT NOT NULL,
                    before_quantity REAL NOT NULL,
                    adjustment_quantity REAL NOT NULL,
                    after_quantity REAL NOT NULL,
                    reason TEXT,
                    operator_id TEXT NOT NULL,
                    operator_name TEXT,
                    created_at TEXT NOT NULL,
                    FOREIGN KEY (consumable_id) REFERENCES consumables(id)
                )", "consumable_stock_adjustments"),
            (@"
                CREATE TABLE IF NOT EXISTS consumable_stock_logs (
                    id TEXT NOT NULL PRIMARY KEY,
                    consumable_id TEXT NOT NULL,
                    change_type TEXT NOT NULL,
                    change_quantity REAL NOT NULL,
                    before_stock REAL NOT NULL,
                    after_stock REAL NOT NULL,
                    reference_id TEXT,
                    reference_no TEXT,
                    operator_id TEXT NOT NULL,
                    operator_name TEXT,
                    remark TEXT,
                    created_at TEXT NOT NULL,
                    FOREIGN KEY (consumable_id) REFERENCES consumables(id)
                )", "consumable_stock_logs"),
            (@"
                CREATE TABLE IF NOT EXISTS consumable_notifications (
                    id TEXT NOT NULL PRIMARY KEY,
                    user_id TEXT NOT NULL,
                    type TEXT NOT NULL,
                    title TEXT NOT NULL,
                    content TEXT,
                    related_id TEXT,
                    is_read INTEGER NOT NULL DEFAULT 0,
                    read_at TEXT,
                    created_at TEXT NOT NULL,
                    FOREIGN KEY (user_id) REFERENCES users(id)
                )", "consumable_notifications"),
        };

        foreach (var (sql, tableName) in consumableTables)
        {
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                await cmd.ExecuteNonQueryAsync();
                logger.LogInformation("耗材表 {TableName} 已就绪", tableName);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "耗材表 {TableName} 创建失败", tableName);
            }
        }

        // 兼容性修补：确保 equipments 表有所需的新增列（EnsureCreated 不处理后期新增列）
        var equipmentConn = dbContext.Database.GetDbConnection();
        await equipmentConn.OpenAsync();
        using (var cmd = equipmentConn.CreateCommand())
        {
            cmd.CommandText = "PRAGMA table_info(equipments)";
            var reader = await cmd.ExecuteReaderAsync();
            var existingColumns = new HashSet<string>();
            while (await reader.ReadAsync())
            {
                existingColumns.Add(reader.GetString(1));
            }
            await reader.CloseAsync();

            var alterStatements = new List<string>();

            if (!existingColumns.Contains("total_quantity"))
                alterStatements.Add("ALTER TABLE equipments ADD COLUMN total_quantity INTEGER NOT NULL DEFAULT 1");
            if (!existingColumns.Contains("available_quantity"))
                alterStatements.Add("ALTER TABLE equipments ADD COLUMN available_quantity INTEGER NOT NULL DEFAULT 1");
            if (!existingColumns.Contains("unit"))
                alterStatements.Add("ALTER TABLE equipments ADD COLUMN unit TEXT DEFAULT '台'");
            if (!existingColumns.Contains("brand"))
                alterStatements.Add("ALTER TABLE equipments ADD COLUMN brand TEXT");
            if (!existingColumns.Contains("supplier"))
                alterStatements.Add("ALTER TABLE equipments ADD COLUMN supplier TEXT");
            if (!existingColumns.Contains("updated_at"))
                alterStatements.Add("ALTER TABLE equipments ADD COLUMN updated_at TEXT");

            foreach (var alterSql in alterStatements)
            {
                try
                {
                    using var alterCmd = equipmentConn.CreateCommand();
                    alterCmd.CommandText = alterSql;
                    await alterCmd.ExecuteNonQueryAsync();
                    logger.LogInformation("已执行: {Sql}", alterSql);
                }
                catch (Exception ex)
                {
                    logger.LogWarning("列已存在或跳过: {Sql} — {Msg}", alterSql, ex.Message);
                }
            }
        }

        // 确保 equipment:approve 权限存在（对已有数据库的兼容性处理）
        var deletePermId = Guid.Parse("50000000-0000-0000-0000-000000000004");
        var approvePermId = Guid.Parse("50000000-0000-0000-0000-000000000006");
        var superAdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var labAdminRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var teacherRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        // 确保 equipment:delete 权限及角色分配存在
        if (!await dbContext.Permissions.AnyAsync(p => p.Code == "equipment:delete"))
        {
            dbContext.Permissions.Add(new LimsAuth.Api.Models.Permission
            {
                Id = deletePermId,
                Code = "equipment:delete",
                Name = "删除设备",
                Module = "equipment",
                Description = "删除设备",
                CreatedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();
            logger.LogInformation("已添加 equipment:delete 权限");
        }
        if (!await dbContext.RolePermissions.AnyAsync(rp => rp.RoleId == superAdminRoleId && rp.PermissionId == deletePermId))
        {
            dbContext.RolePermissions.Add(new LimsAuth.Api.Models.RolePermission
            {
                RoleId = superAdminRoleId,
                PermissionId = deletePermId,
                AssignedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();
            logger.LogInformation("已为超级管理员添加 equipment:delete 权限");
        }
        if (!await dbContext.RolePermissions.AnyAsync(rp => rp.RoleId == labAdminRoleId && rp.PermissionId == deletePermId))
        {
            dbContext.RolePermissions.Add(new LimsAuth.Api.Models.RolePermission
            {
                RoleId = labAdminRoleId,
                PermissionId = deletePermId,
                AssignedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();
            logger.LogInformation("已为实验室管理员添加 equipment:delete 权限");
        }

        if (!await dbContext.Permissions.AnyAsync(p => p.Code == "equipment:approve"))
        {
            dbContext.Permissions.Add(new LimsAuth.Api.Models.Permission
            {
                Id = approvePermId,
                Code = "equipment:approve",
                Name = "审批设备",
                Module = "equipment",
                Description = "审批设备借用/归还申请",
                CreatedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();
            logger.LogInformation("已添加 equipment:approve 权限");
        }

        if (!await dbContext.RolePermissions.AnyAsync(rp => rp.RoleId == superAdminRoleId && rp.PermissionId == approvePermId))
        {
            dbContext.RolePermissions.Add(new LimsAuth.Api.Models.RolePermission
            {
                RoleId = superAdminRoleId,
                PermissionId = approvePermId,
                AssignedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();
            logger.LogInformation("已为超级管理员添加 equipment:approve 权限");
        }

        if (!await dbContext.RolePermissions.AnyAsync(rp => rp.RoleId == labAdminRoleId && rp.PermissionId == approvePermId))
        {
            dbContext.RolePermissions.Add(new LimsAuth.Api.Models.RolePermission
            {
                RoleId = labAdminRoleId,
                PermissionId = approvePermId,
                AssignedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();
            logger.LogInformation("已为实验室管理员添加 equipment:approve 权限");
        }

        if (!await dbContext.RolePermissions.AnyAsync(rp => rp.RoleId == teacherRoleId && rp.PermissionId == approvePermId))
        {
            dbContext.RolePermissions.Add(new LimsAuth.Api.Models.RolePermission
            {
                RoleId = teacherRoleId,
                PermissionId = approvePermId,
                AssignedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();
            logger.LogInformation("已为教师添加 equipment:approve 权限");
        }

        // 迁移：确保 equipment_borrow_records 表有 is_deleted 列
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "PRAGMA table_info(equipment_borrow_records)";
            var reader = await cmd.ExecuteReaderAsync();
            var columns = new List<string>();
            while (await reader.ReadAsync())
                columns.Add(reader.GetString(1));
            if (!columns.Contains("is_deleted"))
            {
                await reader.CloseAsync();
                using var cmd2 = conn.CreateCommand();
                cmd2.CommandText = "ALTER TABLE equipment_borrow_records ADD COLUMN is_deleted INTEGER NOT NULL DEFAULT 0";
                await cmd2.ExecuteNonQueryAsync();
                logger.LogInformation("已为借还记录表添加 is_deleted 字段");
            }
        }
        catch { /* 忽略迁移错误，新库会自动创建 */ }

        // 耗材管理：初始化默认分类
        try
        {
            if (!await dbContext.ConsumableCategories.AnyAsync())
            {
                var defaultCategories = new[]
                {
                    new LimsAuth.Api.Models.ConsumableCategory { Id = Guid.NewGuid(), Name = "化学试剂", Remark = "化学实验用试剂" },
                    new LimsAuth.Api.Models.ConsumableCategory { Id = Guid.NewGuid(), Name = "电子元件", Remark = "电子电路相关元件" },
                    new LimsAuth.Api.Models.ConsumableCategory { Id = Guid.NewGuid(), Name = "医疗耗材", Remark = "医疗实验用耗材" },
                    new LimsAuth.Api.Models.ConsumableCategory { Id = Guid.NewGuid(), Name = "办公耗材", Remark = "办公常用耗材" },
                    new LimsAuth.Api.Models.ConsumableCategory { Id = Guid.NewGuid(), Name = "工具类", Remark = "实验室常用工具" }
                };
                dbContext.ConsumableCategories.AddRange(defaultCategories);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("已添加耗材默认分类");
            }
        }
        catch { /* 忽略错误 */ }

        // 耗材管理：初始化权限并分配给角色
        try
        {
            var consumablePerms = new[]
            {
                ("consumable:read", "查看耗材"),
                ("consumable:create", "创建耗材"),
                ("consumable:update", "更新耗材"),
                ("consumable:delete", "删除耗材"),
                ("consumable:in", "耗材入库"),
                ("consumable:out", "耗材出库"),
                ("consumable:approve", "审批耗材申请"),
                ("consumable:adjust", "调整库存"),
                ("consumable:statistics", "耗材统计")
            };

            var labAdminPermIds = new List<Guid>();
            foreach (var (code, name) in consumablePerms)
            {
                if (!await dbContext.Permissions.AnyAsync(p => p.Code == code))
                {
                    var permId = Guid.NewGuid();
                    dbContext.Permissions.Add(new LimsAuth.Api.Models.Permission
                    {
                        Id = permId,
                        Code = code,
                        Name = name,
                        Module = "consumable",
                        Description = name,
                        CreatedAt = DateTime.UtcNow
                    });
                    await dbContext.SaveChangesAsync();
                    logger.LogInformation("已添加权限：{Code}", code);

                    if (!await dbContext.RolePermissions.AnyAsync(rp => rp.RoleId == superAdminRoleId && rp.PermissionId == permId))
                    {
                        dbContext.RolePermissions.Add(new LimsAuth.Api.Models.RolePermission
                        {
                            RoleId = superAdminRoleId,
                            PermissionId = permId,
                            AssignedAt = DateTime.UtcNow
                        });
                        await dbContext.SaveChangesAsync();
                    }

                    if (code != "consumable:delete" && code != "consumable:statistics")
                    {
                        labAdminPermIds.Add(permId);
                    }
                }
            }

            var existingLabAdminPerms = await dbContext.RolePermissions
                .Where(rp => rp.RoleId == labAdminRoleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            foreach (var permId in labAdminPermIds)
            {
                if (!existingLabAdminPerms.Contains(permId))
                {
                    dbContext.RolePermissions.Add(new LimsAuth.Api.Models.RolePermission
                    {
                        RoleId = labAdminRoleId,
                        PermissionId = permId,
                        AssignedAt = DateTime.UtcNow
                    });
                    await dbContext.SaveChangesAsync();
                }
            }

            var userReadPermId = (await dbContext.Permissions.FirstOrDefaultAsync(p => p.Code == "consumable:read"))?.Id;
            var userOutPermId = (await dbContext.Permissions.FirstOrDefaultAsync(p => p.Code == "consumable:out"))?.Id;

            if (userReadPermId.HasValue && !await dbContext.RolePermissions.AnyAsync(rp => rp.RoleId == teacherRoleId && rp.PermissionId == userReadPermId.Value))
            {
                dbContext.RolePermissions.Add(new LimsAuth.Api.Models.RolePermission { RoleId = teacherRoleId, PermissionId = userReadPermId.Value, AssignedAt = DateTime.UtcNow });
                await dbContext.SaveChangesAsync();
            }
            if (userOutPermId.HasValue && !await dbContext.RolePermissions.AnyAsync(rp => rp.RoleId == teacherRoleId && rp.PermissionId == userOutPermId.Value))
            {
                dbContext.RolePermissions.Add(new LimsAuth.Api.Models.RolePermission { RoleId = teacherRoleId, PermissionId = userOutPermId.Value, AssignedAt = DateTime.UtcNow });
                await dbContext.SaveChangesAsync();
            }
            logger.LogInformation("耗材权限初始化完成");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "耗材权限初始化失败");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "数据库创建失败");
    }
}

app.Run();
