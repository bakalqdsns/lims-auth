using System.Security.Claims;
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
    private readonly UserService _userService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext dbContext, JwtService jwtService, UserService userService, ILogger<AuthService> logger)
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new LoginResponse
            {
                Code = 400,
                Message = "用户名和密码不能为空"
            };
        }

        // 查找用户
        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

        if (user == null)
        {
            return new LoginResponse
            {
                Code = 401,
                Message = "用户名或密码错误"
            };
        }

        // 验证密码
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
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

        // 获取用户角色和权限
        var roles = await _userService.GetUserRolesAsync(user.Id);
        var permissions = await _userService.GetUserPermissionsAsync(user.Id);

        // 生成 JWT Token
        var token = _jwtService.GenerateToken(user, roles, permissions);

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
                    Email = user.Email,
                    Phone = user.Phone,
                    FullName = user.FullName,
                    AvatarUrl = user.AvatarUrl,
                    IsActive = user.IsActive,
                    Roles = roles,
                    Permissions = permissions
                }
            }
        };
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    public async Task<ApiResponse<UserInfo>> GetCurrentUserAsync(Guid userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

        if (user == null)
        {
            return ApiResponse<UserInfo>.Error(404, "用户不存在");
        }

        var roles = await _userService.GetUserRolesAsync(user.Id);
        var permissions = await _userService.GetUserPermissionsAsync(user.Id);

        return ApiResponse<UserInfo>.Success(new UserInfo
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Phone = user.Phone,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
            Roles = roles,
            Permissions = permissions
        });
    }

    /// <summary>
    /// 刷新 Token
    /// </summary>
    public async Task<ApiResponse<LoginData>> RefreshTokenAsync(Guid userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

        if (user == null)
        {
            return ApiResponse<LoginData>.Error(404, "用户不存在");
        }

        var roles = await _userService.GetUserRolesAsync(user.Id);
        var permissions = await _userService.GetUserPermissionsAsync(user.Id);

        var token = _jwtService.GenerateToken(user, roles, permissions);

        return ApiResponse<LoginData>.Success(new LoginData
        {
            Token = token,
            TokenType = "Bearer",
            ExpiresIn = 3600,
            User = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                IsActive = user.IsActive,
                Roles = roles,
                Permissions = permissions
            }
        });
    }
}
