using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Logic.IRepository;
using NA_Xepthoikhoabieu.Helpers;
using Newtonsoft.Json;

namespace NA_Xepthoikhoabieu.Controllers
{
    [ApiController]
    [Route("api/file")]
    public class FileImportController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IFileImportRepository _file;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;


        public FileImportController(IMapper mapper, IFileImportRepository file, IClaimHelperRepository claimHelperRepository, IAuthRepository auth)
        {
            _mapper = mapper;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _file = file;
        }
        [HttpPost("import")]
        [RequireToken]
        public IActionResult ConvertExcelToJson(IFormFile file)
        {
            try
            {
                int idDonvi = _claimHelperRepository.GetIdDonvi(User);
                if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
                if (file == null || file.Length == 0)
                    return ApiResult.BadRequest("File không được để trống");

                if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
                    return ApiResult.BadRequest("Chỉ chấp nhận file Excel (.xlsx, .xls)");

                string jsonString;
                using (var stream = file.OpenReadStream())
                {
                    jsonString = _file.ConvertExcelToJson(stream);
                }

                // Check nếu convert thành công
                if (string.IsNullOrEmpty(jsonString))
                    return ApiResult.BadRequest("Convert Excel thất bại - dữ liệu trống");

                return Ok(jsonString);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}

