using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Services;

/// <summary>
/// 认证服务
/// </summary>
public class AuthService
{
    private readonly AppDbContext _dbContext;
    private readonly JwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext dbContext, JwtService jwtService, ILogger<AuthService> logger)
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // 查找用户
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

        if (user == null)
        {
            return new LoginResponse
            {
                Code = 401,
                Message = "用户名或密码错误"
            };
        }

        // 验证密码 (简化版使用明文比较，生产环境应使用 BCrypt)
        // 这里为了演示方便，使用简单验证
        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            return new LoginResponse
            {
                Code = 401,
                Message = "用户名或密码错误"
            };
        }

        // 更新最后登录时间
        user.LastLoginAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        // 生成 JWT Token
        var token = _jwtService.GenerateToken(user);

        _logger.LogInformation("用户 {Username} 登录成功", user.Username);

        return new LoginResponse
        {
            Code = 200,
            Message = "登录成功",
            Data = new LoginData
            {
                Token = token,
                TokenType = "Bearer",
                ExpiresIn = 3600, // 1小时
                User = new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    FullName = user.FullName
                }
            }
        };
    }

    public async Task<ApiResponse<UserInfo?>> GetCurrentUserAsync(Guid userId)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

        if (user == null)
        {
            return ApiResponse<UserInfo?>.Error(404, "用户不存在");
        }

        return ApiResponse<UserInfo?>.Success(new UserInfo
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role,
            FullName = user.FullName
        });
    }

    /// <summary>
    /// 初始化种子数据（用于测试）
    /// </summary>
    public async Task SeedDataAsync()
    {
        if (!await _dbContext.Users.AnyAsync())
        {
            var users = new[]
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    PasswordHash = HashPassword("admin123"),
                    Role = "Admin",
                    FullName = "系统管理员",
                    IsActive = true
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "teacher",
                    PasswordHash = HashPassword("teacher123"),
                    Role = "Teacher",
                    FullName = "张老师",
                    IsActive = true
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "student",
                    PasswordHash = HashPassword("student123"),
                    Role = "Student",
                    FullName = "李同学",
                    IsActive = true
                }
            };

            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("种子数据初始化完成");
        }
    }

    private static string HashPassword(string password)
    {
        // 简化版：实际生产环境应使用 BCrypt
        // return BCrypt.HashPassword(password);
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }

    private static bool VerifyPassword(string password, string hash)
    {
        // 简化版：实际生产环境应使用 BCrypt
        // return BCrypt.Verify(password, hash);
        var hashedInput = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        return hashedInput == hash;
    }
}
