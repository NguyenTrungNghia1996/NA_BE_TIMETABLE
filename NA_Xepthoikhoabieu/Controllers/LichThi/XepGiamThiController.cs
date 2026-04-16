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
        private readonly IDM_PhongthiRepository _phong;
        private readonly IPhongthi_ThisinhRepository _phongThisinh;
        public XepGiamThiController(IMapper mapper, IDM_LichthiRepository lichthi, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                    IXepGiamThiRepository xep, IDM_GiamthiRepository giamthi, IDM_PhongthiRepository phong, IPhongthi_ThisinhRepository phongThisinh)
        {
            _mapper = mapper;
            _lichthi = lichthi;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _xep = xep;
            _giamthi = giamthi;
            _phong = phong;
            _phongThisinh = phongThisinh;
        }

        [HttpPost]
        [RequireToken]
        public IActionResult XepTuDong([FromQuery] int IdLich)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var checkLich = _lichthi.GetDetailById(IdLich, idDonvi);
            if (checkLich == null)
                return ApiResult.NotFound("Id lịch không hợp lệ");
            bool checkPhong = _phongThisinh.CheckPhongCoThiSinh(checkLich.Id_diem_thi);
            if (checkPhong == false)
                return ApiResult.BadRequest("Chưa xếp thí sinh vào phòng thi, vui lòng xếp phòng trước");
            var xep = _xep.XepGiamThi(IdLich);
            if (!xep)
                return ApiResult.BadRequest($"Xếp lịch thi thất bại");

            return ApiResult.Success("Thành công");
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetKetQua([FromQuery] int IdLich)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            bool checkLich = _lichthi.CheckId(IdLich, idDonvi);
            if (!checkLich)
                return ApiResult.NotFound("Id lịch không hợp lệ");

            var xep = _xep.GetChiTietLichCoiThi(IdLich);
            if (xep==null)
                return ApiResult.BadRequest($"Thất bại");

            return ApiResult.Success(new {data = xep} ,"Thành công");
        }
        [HttpGet("phongcho")]
        [RequireToken]
        public IActionResult GetPhongCho([FromQuery] int IdLich)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            bool checkLich = _lichthi.CheckId(IdLich, idDonvi);
            if (!checkLich)
                return ApiResult.NotFound("Id lịch không hợp lệ");

            var xep = _xep.GetListPhongCho(IdLich);
            if (xep==null)
                return ApiResult.BadRequest($"Thất bại");

            return ApiResult.Success(new {data = xep} ,"Thành công");
        }
        [HttpGet("chuaxep")]
        [RequireToken]
        public IActionResult GetChuaXep([FromQuery] int IdLich)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            bool checkLich = _lichthi.CheckId(IdLich, idDonvi);
            if (!checkLich)
                return ApiResult.NotFound("Id lịch không hợp lệ");

            var xep = _xep.GetListChuaXep(IdLich);
            if (xep==null)
                return ApiResult.BadRequest($"Thất bại");

            return ApiResult.Success(new {data = xep} ,"Thành công");
        }
        [HttpPost("le")]
        [RequireToken]
        public IActionResult XepLe([FromQuery] int IdLich, [FromQuery] int idGiamThi)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var checkLich = _lichthi.GetDetailById(IdLich, idDonvi);
            if (checkLich == null) 
                return ApiResult.NotFound("Id lịch không hợp lệ");
            bool checkPhong = _phongThisinh.CheckPhongCoThiSinh(checkLich.Id_diem_thi);
            if (checkPhong == false)
                return ApiResult.BadRequest("Chưa xếp thí sinh vào phòng thi, vui lòng xếp phòng trước");
            bool checkGiamThi = _giamthi.CheckId(idGiamThi, checkLich.Id_diem_thi);
            if (!checkGiamThi)
                return ApiResult.NotFound("Id giám sát không hợp lệ");

            (bool result, string mess) = _xep.XepMotGiamThi(IdLich, idGiamThi);
            if (!result)
                return ApiResult.BadRequest($"Xếp lịch thi thất bại");

            return ApiResult.Success(mess);
        }
        [HttpDelete("huyketqua")]
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
                return ApiResult.BadRequest($"Thất bại");

            return ApiResult.Success("Thành công");
        }
        [HttpDelete("huyketqua/giamsat")]
        [RequireToken]
        public IActionResult HuyKetQuaGiamSat([FromQuery] int IdLich, [FromQuery] int IdGiamSat)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var checkLich = _lichthi.GetDetailById(IdLich, idDonvi);
            bool checkGiamThi = _giamthi.CheckId(IdGiamSat, checkLich.Id_diem_thi);
            if (!checkGiamThi)
                return ApiResult.NotFound("Id giám sát không hợp lệ");

            var xep = _xep.HuyKetQuaGiamSat(IdGiamSat);
            if (!xep)
                return ApiResult.BadRequest($"Thất bại");

            return ApiResult.Success("Thành công");
        }
        [HttpDelete("huyketqua/phong")]
        [RequireToken]
        public IActionResult HuyKetQuaPhong([FromQuery] int IdLich, [FromQuery] int IdPhong)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var checkLich = _lichthi.GetDetailById(IdLich, idDonvi);
            bool checkPhong = _phong.CheckIdByDiemThi(IdPhong, checkLich.Id_diem_thi);
            if (!checkPhong)
                return ApiResult.NotFound("Id phòng không hợp lệ");

            var xep = _xep.HuyKetQuaPhong(IdPhong);
            if (!xep)
                return ApiResult.BadRequest($"Thất bại");

            return ApiResult.Success("Thành công");
        }
    }
}
