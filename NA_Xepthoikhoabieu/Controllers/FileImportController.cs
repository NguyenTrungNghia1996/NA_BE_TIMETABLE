using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Logic.IRepository;
using NA_Xepthoikhoabieu.Helpers;

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
        public IActionResult Import(IFormFile file)
        {
            try
            {
                int idDonvi = _claimHelperRepository.GetIdDonvi(User);
                if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
                bool result = false;
                using (var stream = file.OpenReadStream())
                {
                    result = _file.ImportExcelToDb(stream, idDonvi);
                }
                if (!result)
                    return ApiResult.BadRequest("Import thất bại");
                return ApiResult.Success("Import thành công");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpPost("exceltojson")]
        [RequireToken]
        public IActionResult ConvertExcelToJson(IFormFile file)
        {
            try
            {
                int idDonvi = _claimHelperRepository.GetIdDonvi(User);
                if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
                string jsonString;
                using (var stream = file.OpenReadStream())
                {
                    jsonString = _file.ConvertExcelToJson(stream);
                }
                return Ok(jsonString);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}

