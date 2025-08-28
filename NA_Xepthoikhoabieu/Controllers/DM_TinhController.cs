using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/tinh")]
    [ApiController]
    public class DM_TinhController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_TinhRepository _tinh;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        public DM_TinhController(IMapper mapper, IDM_TinhRepository tinh, IClaimHelperRepository claimHelperRepository, IAuthRepository auth)
        {
            _mapper = mapper;
            _tinh = tinh;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
        }
        [HttpGet]
        public IActionResult GetList()
        {
            // Lấy danh sách dữ liệu
            var list = _tinh.GetList();
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            return ApiResult.Success(new
            {
                items = list,
            },
            "Thành công");
        }
    }
}
