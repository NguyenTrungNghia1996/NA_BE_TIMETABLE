using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace NA_Xepthoikhoabieu.Helpers
{
    public static class ClaimHelper
    {
        public static int GetIdDonvi(ClaimsPrincipal user)
        {
            var claimValue = user.FindFirst("Id_Donvi")?.Value;
            return int.TryParse(claimValue, out int idDonvi) ? idDonvi : 0;
        }

        public static IActionResult CheckIdDonvi(ClaimsPrincipal user)
        {
            int idDonvi = GetIdDonvi(user);
            if (idDonvi <= 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }
            return null;
        }
    }
}
