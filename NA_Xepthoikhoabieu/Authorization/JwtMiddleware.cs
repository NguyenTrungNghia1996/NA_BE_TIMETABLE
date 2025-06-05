using System.Linq;
using System.Text.Json;  // Import namespace của Authorization
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NA_Logic.IRepository;

namespace NA_Xepthoikhoabieu.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtHelperRepository _jwtHelperRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public JwtMiddleware(RequestDelegate next, IJwtHelperRepository jwtHelperRepository, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _jwtHelperRepository = jwtHelperRepository;
            _serviceScopeFactory = serviceScopeFactory;
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
            // Kiểm tra user Id và đơn vị
            var userId = context.User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int idUser) || idUser <= 0)
            {
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "application/json";
                var response = JsonSerializer.Serialize(new { status = "error", message = "Thông tin người dùng không hợp lệ." });
                await context.Response.WriteAsync(response);
                return;
            }
            using var scope = _serviceScopeFactory.CreateScope();
            var authRepo = scope.ServiceProvider.GetRequiredService<IAuthRepository>();

            var userDetail = authRepo.CheckUser_DonviExists(idUser);
            if (!userDetail)
            {
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "application/json";
                var response = JsonSerializer.Serialize(new { status = "error", message = "Thông tin người dùng hoặc đơn vị không tồn tại trong cơ sở dữ liệu. Vui lòng liên hệ admin" });
                await context.Response.WriteAsync(response);
                return;
            }
            // Nếu có chính sách phân quyền thì kiểm tra thêm
            if (!string.IsNullOrEmpty(requireTokenAttribute.Policy))
            {
                // Lấy quyền từ Claims
                var permissionValue = context.User.FindFirst("Permissions")?.Value;
                if (!int.TryParse(permissionValue, out int permissionInt))
                {
                    await ReturnUnauthorized(context, "Không thể đọc quyền từ token.");
                    return;
                }

                var userPermission = (MenuPermission)permissionInt;

                // Convert policy string sang enum
                if (!Enum.TryParse<MenuPermission>(requireTokenAttribute.Policy, out var requiredPermission))
                {
                    await ReturnUnauthorized(context, $"Không tìm thấy quyền \"{requireTokenAttribute.Policy}\".");
                    return;
                }

                // Check bằng bitmask
                if (!userPermission.HasFlag(requiredPermission))
                {
                    await ReturnUnauthorized(context, $"Bạn không có quyền truy cập \"{requireTokenAttribute.Policy}\".");
                    return;
                }
            }
            // Tiếp tục nếu token hợp lệ
            await _next(context);
        }
        private async Task ReturnUnauthorized(HttpContext context, string message)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var response = JsonSerializer.Serialize(new { status = "error", message });
            await context.Response.WriteAsync(response);
        }
    }
}
