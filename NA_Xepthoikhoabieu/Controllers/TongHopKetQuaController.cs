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
        private readonly IDM_KhoilopRepository _khoi;
        private readonly IDM_Tohopmon_OntapRepository _tohopmon;
        private readonly IClaimHelperRepository _claimHelperRepository;
        public TongHopKetQuaController(IMapper mapper, IDM_LopontapRepository lopon, IKetqua_BaikiemtraRepository ketqua, IClaimHelperRepository claimHelperRepository, IDM_KhoilopRepository khoi, IDM_Tohopmon_OntapRepository tohopmon)
        {
            _mapper = mapper;
            _lopon = lopon;
            _ketqua = ketqua;
            _claimHelperRepository = claimHelperRepository;
            _khoi = khoi;
            _tohopmon = tohopmon;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetKetQuaHocSinh([FromQuery] int Id_lop_on)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            bool checklop = _lopon.CheckId(Id_lop_on, idDonvi);
            if (Id_lop_on > 0 && !checklop)
                return ApiResult.BadRequest("Id lớp ôn không hợp lệ");
            var list = _ketqua.GetKetQuaHocSinh(Id_lop_on, idDonvi);
            if (list == null)
                return ApiResult.Ok();
            return ApiResult.Success(new
            {
                items = list
            },
            "Thành công");
        }
        [HttpGet("tohopmon")]
        [RequireToken]
        public IActionResult GetKetQuaHocSinh_ToHopMon([FromQuery] int Id_to_hop, [FromQuery] int Id_khoi)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            bool checkkhoi = _khoi.CheckKhoilopByDonvi(Id_khoi, idDonvi);
            if (Id_khoi > 0 && !checkkhoi)
                return ApiResult.BadRequest("Id khối lớp không hợp lệ");
            else if (Id_khoi <= 0 || Id_khoi == null)
                return ApiResult.BadRequest("Vui lòng chọn khối lớp");

            bool checktohop = _tohopmon.CheckId(Id_to_hop, idDonvi);
            if (Id_to_hop > 0 && !checkkhoi)
                return ApiResult.BadRequest("Id tổ hợp môn không hợp lệ");
            else if (Id_to_hop <= 0 || Id_to_hop == null)
                return ApiResult.BadRequest("Vui lòng chọn tổ hợp môn");

                var list = _ketqua.GetKetQuaHocSinh_ToHopMon(Id_to_hop, Id_khoi, idDonvi);
            if (list == null)
                return ApiResult.Ok();
            return ApiResult.Success(new
            {
                items = list
            },
            "Thành công");
        }
    }
}
