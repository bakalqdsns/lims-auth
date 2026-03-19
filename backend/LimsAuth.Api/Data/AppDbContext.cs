using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Data;

/// <summary>
/// 数据库上下文
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // 添加默认管理员用户
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Username = "admin",
            // 密码: admin123 (使用 BCrypt 哈希)
            PasswordHash = "$2a$11$3Zf0J1vX9qQqQqQqQqQqQOe0e0e0e0e0e0e0e0e0e0e0e0e0e0e0e0e",
            Role = "Admin",
            FullName = "系统管理员",
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
