using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NA_Xepthoikhoabieu.Helpers
{
    public static class ModelStateExtensions
    {
        //Ghép tất cả lỗi của ModelState thành chuỗi string
        public static string GetErrorsAsString(this ModelStateDictionary modelState)
        {
            return string.Join("; ", modelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
        }
    }
}
