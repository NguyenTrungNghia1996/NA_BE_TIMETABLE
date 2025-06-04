using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [ApiController]
    [Route("api/donvi")]
    public class DM_DonviController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_DonviRepository _donvi;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        public DM_DonviController(IMapper mapper, IDM_DonviRepository donvi, IClaimHelperRepository claimHelperRepository, IAuthRepository auth)
        {
            _mapper = mapper;
            _donvi = donvi;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _donvi.GetList_Paging(PageIndex, PageSize, search, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            var listDto = _mapper.Map<List<DM_Donvi_List_Dto>>(list);
            return ApiResult.Success(new
            {
                items = listDto,
                totalrecord = totalrecord
            },
            "Thành công");
        }

    }
}