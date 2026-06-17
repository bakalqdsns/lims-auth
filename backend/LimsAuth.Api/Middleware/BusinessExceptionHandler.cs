using Microsoft.AspNetCore.Diagnostics;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Middleware;

/// <summary>
/// 全局业务异常处理
/// - ArgumentException -> 400
/// - DbUpdateException (FOREIGN KEY / UNIQUE 等约束违反) -> 400(友好消息)
/// - 其他 -> 透传给默认 500 处理
/// </summary>
public class BusinessExceptionHandler : IExceptionHandler
{
    private readonly ILogger<BusinessExceptionHandler> _logger;

    public BusinessExceptionHandler(ILogger<BusinessExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ArgumentException argEx)
        {
            _logger.LogWarning("Bad request: {Message}", argEx.Message);
            var problem = new ApiResponse<object>
            {
                Code = 400,
                Message = argEx.Message,
                Data = null
            };
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }

        if (exception is Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
        {
            // 提取 SQLite 约束错误的简短描述
            var msg = dbEx.InnerException?.Message ?? dbEx.Message;
            _logger.LogWarning("DB constraint violation: {Message}", msg);
            var problem = new ApiResponse<object>
            {
                Code = 400,
                Message = "数据约束失败: " + ExtractFriendlyMessage(msg),
                Data = null
            };
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }

        return false;
    }

    private static string ExtractFriendlyMessage(string raw)
    {
        // 截取 SQLite 错误的关键片段
        if (raw.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase))
            return "外键约束失败:引用的记录不存在";
        if (raw.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
            return "唯一约束失败:字段值已存在";
        if (raw.Contains("NOT NULL", StringComparison.OrdinalIgnoreCase))
            return "必填字段缺失";
        // 默认取第一行
        var firstLine = raw.Split('\n').FirstOrDefault() ?? raw;
        return firstLine.Length > 200 ? firstLine.Substring(0, 200) : firstLine;
    }
}
