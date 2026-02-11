using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/tonghop/ketqua")]
    [ApiController]
    public class TongHopKetQuaController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_LopontapRepository _lopon;
        private readonly IKetqua_BaikiemtraRepository _ketqua;
        private readonly IClaimHelperRepository _claimHelperRepository;
        public TongHopKetQuaController(IMapper mapper, IDM_LopontapRepository lopon, IKetqua_BaikiemtraRepository ketqua, IClaimHelperRepository claimHelperRepository)
        {
            _mapper = mapper;
            _lopon = lopon;
            _ketqua = ketqua;
            _claimHelperRepository = claimHelperRepository;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int Id_lop_on)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var list = _ketqua.GetKetQuaHocSinh(Id_lop_on, idDonvi);
            if (list == null)
                return ApiResult.Ok();
            return ApiResult.Success(new
            {
                items = list,
            },
            "Thành công");
        }
    }
}
