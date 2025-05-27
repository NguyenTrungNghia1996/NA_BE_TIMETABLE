using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using NA_Logic.IRepository;  // Import namespace của Authorization

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
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Token không tồn tại. Vui lòng đăng nhập.");
                return;
            }

            // Kiểm tra tính hợp lệ của token
            if (!_jwtHelperRepository.IsValidToken(token))
            {
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Token không hợp lệ. Vui lòng đăng nhập lại.");
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
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Token không hợp lệ. Vui lòng đăng nhập lại.");
                return;
            }

            // Tiếp tục nếu token hợp lệ
            await _next(context);
        }
    }
}
