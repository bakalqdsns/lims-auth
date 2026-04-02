using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LimsAuth.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace LimsAuth.Api.Services;

/// <summary>
/// JWT 服务
/// </summary>
public class JwtService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtService> _logger;

    public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// 生成 JWT Token
    /// </summary>
    public string GenerateToken(User user, List<string> roles, List<string> permissions)
    {
        var secretKey = _configuration["Jwt:SecretKey"] ?? "your-super-secret-key-min-32-chars-long!!";
        var issuer = _configuration["Jwt:Issuer"] ?? "LimsAuth";
        var audience = _configuration["Jwt:Audience"] ?? "LimsClient";
        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 构建 Claims
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.Username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new("fullName", user.FullName ?? user.Username)
        };

        // 添加角色声明
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // 添加权限声明
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 验证 Token
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        var secretKey = _configuration["Jwt:SecretKey"] ?? "your-super-secret-key-min-32-chars-long!!";
        var issuer = _configuration["Jwt:Issuer"] ?? "LimsAuth";
        var audience = _configuration["Jwt:Audience"] ?? "LimsClient";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Token 验证失败: {Message}", ex.Message);
            return null;
        }
    }
}
