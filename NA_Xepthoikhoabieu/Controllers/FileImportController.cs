using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Logic.IRepository;

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
        [HttpPost("convert")]
        public IActionResult ConvertExcelToJson(IFormFile file)
        {
            try
            {
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

