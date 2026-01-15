using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [ApiController]
    [Route("api/lich")]
    public class XepLichOnTapController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IXepLichOnTapRepository _ob;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDanhsach_LichontapRepository _lot;

        public XepLichOnTapController(IMapper mapper, IXepLichOnTapRepository ob, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                      IDanhsach_LichontapRepository lot)
        {
            _mapper = mapper;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _ob = ob;
            _lot = lot;
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create(int idlich)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // mapper data 

            var check_tkb = _lot.CheckId(idlich, idDonvi);
            if (idlich <= 0 || !check_tkb)
                return ApiResult.BadRequest($"Id lịch ôn tập = {idlich} không hợp lệ, vui lòng kiểm tra lại");
            // add 
            bool add = _ob.ProcessThoiKhoaBieu(idlich, idDonvi);
            if (!add)
                return ApiResult.NotFound("Xếp lịch ôn tập không thành công");

            return ApiResult.Success(
            "Xếp lịch ôn tập thành công");
        }
        [HttpGet("lop")]
        [RequireToken]
        public IActionResult tkb_lop(int idLop, int idlich)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var check_tkb = _lot.CheckId(idlich, idDonvi);
            if (idlich <= 0 || !check_tkb)
                return ApiResult.BadRequest($"Id thời khoá biểu = {idlich} không hợp lệ, vui lòng kiểm tra lại");

            // Lấy bản ghi từ db
            var detail = _ob.GetLichByLop(idLop, idlich, idDonvi);
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpGet("giaovien")]
        [RequireToken]
        public IActionResult tkb_giaovien(int idGV, int idlich)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var check_tkb = _lot.CheckId(idlich, idDonvi);
            if (idlich <= 0 || !check_tkb)
                return ApiResult.BadRequest($"Id thời khoá biểu = {idlich} không hợp lệ, vui lòng kiểm tra lại");

            // Lấy bản ghi từ db
            var detail = _ob.GetLichByGiaovien(idGV, idlich, idDonvi);
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpGet("tietchuaxep")]
        [RequireToken]
        public IActionResult tkb_tietchuaxep(int idlich)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var check_tkb = _lot.CheckId(idlich, idDonvi);
            if (idlich <= 0 || !check_tkb)
                return ApiResult.BadRequest($"Id thời khoá biểu = {idlich} không hợp lệ, vui lòng kiểm tra lại");

            // Lấy bản ghi từ db
            var detail = _ob.GetTietChuaXep(idlich);
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpPost("timvitri/lop")]
        [RequireToken]
        public IActionResult timvitri_lop([FromBody] ObjectTietOnTap_theoLopDto tietDachon)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var detail = _ob.TimViTriXepDuoc_byLop(tietDachon, idDonvi);
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpPost("timvitri/chuaxep/lop")]
        [RequireToken]
        public IActionResult timvitri_chuaxep_lop([FromBody] ObjectTietOnTap_theoLopDto tietDachon)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var detail = _ob.TimViTriXepDuoc_TietChuaXep_byLop(tietDachon, idDonvi);
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpPost("timvitri/giaovien")]
        [RequireToken]
        public IActionResult timvitri_giaovien([FromBody] ObjectTietOnTap_theoGVDto tietDachon)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var detail = _ob.TimViTriXepDuoc_byGV(tietDachon, idDonvi);
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpPost("timvitri/chuaxep/giaovien")]
        [RequireToken]
        public IActionResult timvitri_chuaxep_giaovien([FromBody] ObjectTietOnTap_theoGVDto tietDachon)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var detail = _ob.TimViTriXepDuoc_TietChuaXep_byGV(tietDachon, idDonvi);
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpPost("timtiet/lop")]
        [RequireToken]
        public IActionResult timtiet_lop([FromBody] ObjectTietOnTap_theoLopDto tietDachon)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var detail = _ob.TimTietXepDuoc_byLop(tietDachon, idDonvi);
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpPost("timtiet/giaovien")]
        [RequireToken]
        public IActionResult timtiet_giaovien([FromBody] ObjectTietOnTap_theoGVDto tietDachon)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var detail = _ob.TimTietXepDuoc_byGV(tietDachon, idDonvi);
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpPost("update/lop")]
        [RequireToken]
        public IActionResult update_doicho_lop([FromBody] ObjectTietOnTap_theoLopDto tietDaChon)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var (success, detail) = _ob.DoiChoHaiTiet_Lop(tietDaChon, idDonvi);
            if (!success)
            {
                return ApiResult.Success(detail, "Thất bại");
            }
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpPost("update/giaovien")]
        [RequireToken]
        public IActionResult update_doicho_giaovien([FromBody] ObjectTietOnTap_theoGVDto tietDaChon)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // mapper data 

            // add 
            var (success, detail) = _ob.DoiChoHaiTiet_GV(tietDaChon, idDonvi);
            if (!success)
            {
                return ApiResult.Success(detail, "Thất bại");
            }
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpPut("update")]
        [RequireToken]
        public IActionResult Update([FromBody] Object_TietOnTap tiet)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // mapper data 

            //var check_id = _tkb.checkId_chitiet(id, idDonvi);
            //if (id <= 0 || !check_id)
            //    return ApiResult.BadRequest($"Id = {id} không hợp lệ, vui lòng kiểm tra lại");
            // add 
            bool success = _ob.UpdateTietChuaXep(tiet, idDonvi);
            if (!success)
            {
                return ApiResult.BadRequest("Thất bại");
            }
            return ApiResult.Success("Thành công");
        }
    }
}
