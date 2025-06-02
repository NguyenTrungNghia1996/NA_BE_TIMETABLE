using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.APIResponse;


namespace NA_Xepthoikhoabieu.Helpers
{
    public static class ApiResult
    {
        public static IActionResult Success<T>(T data, string message = "Thành công")
        {
            return new OkObjectResult(new ApiResponse<T>
            {
                Data = data,
                Message = message,
                Status = "success"
            });
        }

        public static IActionResult BadRequest(string message = "Yêu cầu không hợp lệ")
        {
            return new BadRequestObjectResult(new ApiResponse<object>
            {
                Data = new object[] { },
                Message = message,
                Status = "error"
            });
        }

        public static IActionResult NotFound(string message = "Không tìm thấy")
        {
            return new NotFoundObjectResult(new ApiResponse<object>
            {
                Data = new object[] { },
                Message = message,
                Status = "error"
            });
        }

        public static IActionResult Unauthorized(string message = "Chưa xác thực")
        {
            return new UnauthorizedObjectResult(new ApiResponse<object>
            {
                Data = new object[] { },
                Message = message,
                Status = "error"
            });
        }

        public static IActionResult Forbidden(string message = "Không có quyền truy cập")
        {
            return new ObjectResult(new ApiResponse<object>
            {
                Data = new object[] { },
                Message = message,
                Status = "error"
            })
            {
                StatusCode = 403
            };
        }

        public static IActionResult ServerError(string message = "Lỗi máy chủ")
        {
            return new ObjectResult(new ApiResponse<object>
            {
                Data = new object[] { },
                Message = message,
                Status = "error"
            })
            {
                StatusCode = 500
            };
        }
        public static IActionResult Ok(string message = "Thành công")
        {
            return new OkObjectResult(new ApiResponse<object>
            {
                Data = new object[] { },
                Message = message,
                Status = "success"
            });
        }
    }
}
