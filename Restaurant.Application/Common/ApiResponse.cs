using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T> { Success = true, Data = data, Message = message };
    }

    public static ApiResponse<T> FailResponse(string message)
    {
        return new ApiResponse<T> { Success = false, Message = message };
    }

    public static ApiResponse<T> FailResponse(IDictionary<string, string[]> errors)
    {
        return new ApiResponse<T> { Success = false, Message = "Doğrulama xətaları.", Errors = errors };
    }
}

