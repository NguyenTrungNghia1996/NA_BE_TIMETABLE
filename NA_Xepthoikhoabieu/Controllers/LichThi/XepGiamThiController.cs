using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.IRepository.Auth;
using NA_Logic.IRepository.LichOnTap;
using NA_Logic.IRepository.LichThi;
using NA_Logic.IRepository.XepGiamThi;
using NA_Logic.IRepository.XepThoiKhoaBieu;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/xeplichthi")]
    [ApiController]
    public class XepGiamThiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_LichthiRepository _lichthi;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IXepGiamThiRepository _xep;
        private readonly IDM_GiamthiRepository _giamthi;
        public XepGiamThiController(IMapper mapper, IDM_LichthiRepository lichthi, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                    IXepGiamThiRepository xep, IDM_GiamthiRepository giamthi)
        {
            _mapper = mapper;
            _lichthi = lichthi;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _xep = xep;
            _giamthi = giamthi;
        }

        [HttpPost]
        [RequireToken]
        public IActionResult XepTuDong([FromQuery] int IdLich)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            bool checkLich = _lichthi.CheckId(IdLich, idDonvi);
            if (!checkLich)
                return ApiResult.NotFound("Id lịch không hợp lệ");

            var xep = _xep.XepGiamThi(IdLich);
            if (!xep)
                return ApiResult.BadRequest($"Xếp lịch thi thất bại");

            return ApiResult.Success("Thành công");
        }
        [HttpPost("le")]
        [RequireToken]
        public IActionResult XepLẻ([FromQuery] int IdLich, [FromQuery] int idGiamThi)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            bool checkLich = _lichthi.CheckId(IdLich, idDonvi);
            if (!checkLich)
                return ApiResult.NotFound("Id lịch không hợp lệ");
            bool checkGiamThi = _giamthi.CheckId(IdLich, idDonvi);
            if (!checkGiamThi)
                return ApiResult.NotFound("Id giám thị không hợp lệ");

            var xep = _xep.XepGiamThi(IdLich);
            if (!xep)
                return ApiResult.BadRequest($"Xếp lịch thi thất bại");

            return ApiResult.Success("Thành công");
        }
        [HttpPost("huyketqua")]
        [RequireToken]
        public IActionResult HuyKetQua([FromQuery] int IdLich)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            bool checkLich = _lichthi.CheckId(IdLich, idDonvi);
            if (!checkLich)
                return ApiResult.NotFound("Id lịch không hợp lệ");

            var xep = _xep.HuyKetQua(IdLich);
            if (!xep)
                return ApiResult.BadRequest($"Xếp lịch thi thất bại");

            return ApiResult.Success("Thành công");
        }
    }
}
