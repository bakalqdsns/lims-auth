using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface IAuthService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<UserInfoDto>> GetCurrentUserAsync(Guid userId);
    Task<ApiResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<ApiResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<ApiResponse> RefreshTokenAsync(string token);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _db.SysUsers
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsDeleted == 0);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return ApiResponse<LoginResponse>.Error(401, "用户名或密码错误");
        }

        if (user.Status != 1)
        {
            return ApiResponse<LoginResponse>.Error(403, "账号已被禁用");
        }

        var permissions = await _db.SysUserRoles
            .Where(ur => ur.UserId == user.Id)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync();

        var token = GenerateToken(user, permissions);
        var expirationHours = int.Parse(_config["Jwt:ExpirationHours"] ?? "12");

        // Update login info
        user.LastLoginTime = DateTime.UtcNow;
        user.LoginFailCount = 0;
        await _db.SaveChangesAsync();

        return ApiResponse<LoginResponse>.Success(new LoginResponse
        {
            Token = token,
            ExpiresIn = expirationHours * 3600,
            User = new UserInfoDto
            {
                Id = user.Id,
                Username = user.Username,
                RealName = user.RealName,
                Email = user.Email,
                Mobile = user.Mobile,
                Avatar = user.Avatar,
                Status = user.Status,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
                Permissions = permissions
            }
        });
    }

    public async Task<ApiResponse<UserInfoDto>> GetCurrentUserAsync(Guid userId)
    {
        var user = await _db.SysUsers
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == 0);

        if (user == null)
            return ApiResponse<UserInfoDto>.Error(404, "用户不存在");

        var permissions = await _db.SysUserRoles
            .Where(ur => ur.UserId == user.Id)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync();

        return ApiResponse<UserInfoDto>.Success(new UserInfoDto
        {
            Id = user.Id,
            Username = user.Username,
            RealName = user.RealName,
            Email = user.Email,
            Mobile = user.Mobile,
            Avatar = user.Avatar,
            Status = user.Status,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
            Permissions = permissions
        });
    }

    public async Task<ApiResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _db.SysUsers.FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == 0);
        if (user == null)
            return ApiResponse.Error(404, "用户不存在");

        if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.Password))
            return ApiResponse.Error(400, "原密码错误");

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordUpdateTime = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponse.Success("密码修改成功");
    }

    public async Task<ApiResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _db.SysUsers.FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == 0);
        if (user == null)
            return ApiResponse.Error(404, "用户不存在");

        if (!string.IsNullOrEmpty(request.RealName)) user.RealName = request.RealName;
        if (!string.IsNullOrEmpty(request.Email)) user.Email = request.Email;
        if (!string.IsNullOrEmpty(request.Mobile)) user.Mobile = request.Mobile;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return ApiResponse.Success("资料更新成功");
    }

    public Task<ApiResponse> RefreshTokenAsync(string token)
    {
        return Task.FromResult(ApiResponse.Error(501, "Token刷新功能待实现"));
    }

    private string GenerateToken(SysUser user, List<string> permissions)
    {
        var secretKey = _config["Jwt:SecretKey"] ?? "LimsAuth_SuperSecretKey_2026_MinLength32Chars!";
        var issuer = _config["Jwt:Issuer"] ?? "LimsAuth.Api";
        var audience = _config["Jwt:Audience"] ?? "LimsAuth.Client";
        var expirationHours = int.Parse(_config["Jwt:ExpirationHours"] ?? "12");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.GivenName, user.RealName),
        };

        foreach (var role in user.UserRoles.Select(ur => ur.Role.Name))
            claims.Add(new Claim(ClaimTypes.Role, role));

        foreach (var perm in permissions)
            claims.Add(new Claim("permission", perm));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expirationHours),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
