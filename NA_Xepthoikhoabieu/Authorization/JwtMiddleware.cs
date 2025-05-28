using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using NA_Logic.IRepository;
using System.Text.Json;  // Import namespace của Authorization

namespace NA_Xepthoikhoabieu.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtHelperRepository _jwtHelperRepository;

        public JwtMiddleware(RequestDelegate next, IJwtHelperRepository jwtHelperRepository)
        {
            _next = next;
            _jwtHelperRepository = jwtHelperRepository;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            // Kiểm tra nếu action/controller có attribute [RequireToken]
            var requireTokenAttribute = endpoint?.Metadata.GetMetadata<RequireTokenAttribute>();

            // Nếu không có [RequireToken], bỏ qua middleware này
            if (requireTokenAttribute == null)
            {
                await _next(context);
                return;
            }
            // Nếu có [RequireToken], kiểm tra token

            // Lấy token từ header
            var token = _jwtHelperRepository.GetTokenFromRequest(context.Request);

            // Nếu không có token
            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "application/json";
                var response = JsonSerializer.Serialize(new { status = "error", message = "Token không tồn tại. Vui lòng đăng nhập." });
                await context.Response.WriteAsync(response);
                return;
            }

            // Kiểm tra tính hợp lệ của token
            if (!_jwtHelperRepository.IsValidToken(token))
            {
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "application/json";
                var response = JsonSerializer.Serialize(new { status = "error", message = "Token không tồn tại. Vui lòng đăng nhập." });
                await context.Response.WriteAsync(response);
                return;
            }

            // Gán giá trị claims sau khi xác thực thành công để lấy userId
            var principal = _jwtHelperRepository.GetPrincipalFromToken(token);
            if (principal != null)
            {
                context.User = principal;
            }
            else
            {
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "application/json";
                var response = JsonSerializer.Serialize(new { status = "error", message = "Token không tồn tại. Vui lòng đăng nhập." });
                await context.Response.WriteAsync(response);
                return;
            }

            // Tiếp tục nếu token hợp lệ
            await _next(context);
        }
    }
}
