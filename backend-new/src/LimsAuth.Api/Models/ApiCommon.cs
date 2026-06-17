namespace LimsAuth.Api.Models;

public class ApiResponse
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ApiResponse Success(string message = "success")
    {
        return new ApiResponse { Code = 200, Message = message };
    }

    public static ApiResponse Error(int code, string message)
    {
        return new ApiResponse { Code = code, Message = message };
    }
}

public class ApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> Success(T data, string message = "success")
    {
        return new ApiResponse<T> { Code = 200, Message = message, Data = data };
    }

    public static ApiResponse<T> Success(string message = "success")
    {
        return new ApiResponse<T> { Code = 200, Message = message, Data = default };
    }

    public static ApiResponse<T> Error(int code, string message)
    {
        return new ApiResponse<T> { Code = code, Message = message, Data = default };
    }
}

public class PagedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => Total > 0 ? (int)Math.Ceiling((double)Total / PageSize) : 0;
}

public class PagedQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class ToggleStatusRequest
{
    public bool IsActive { get; set; }
}
