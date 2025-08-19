using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/sotiet")]
    [ApiController]
    public class SoTietDanhMucController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISoTietDanhMucRepository _sotiet;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDanhsach_ThoikhoabieuRepository _tkb;
        public SoTietDanhMucController(IMapper mapper, ISoTietDanhMucRepository sotiet, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, IDanhsach_ThoikhoabieuRepository tkb)
        {
            _mapper = mapper;
            _sotiet = sotiet;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _tkb = tkb;
        }
        [HttpGet("monhoc")]
        [RequireToken]
        public IActionResult GetList_Mon([FromQuery] int idtkb)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var check_tkb = _tkb.CheckId(idtkb, idDonvi);
            if (idtkb <= 0 || !check_tkb)
                return ApiResult.BadRequest($"Id thời khoá biểu = {idtkb} không hợp lệ, vui lòng kiểm tra lại");

            // Lấy bản ghi từ db
            var detail = _sotiet.GetSotiet_Mon(idDonvi,idtkb);
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpGet("lophoc")]
        [RequireToken]
        public IActionResult GetList_Lop([FromQuery] int idtkb)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var check_tkb = _tkb.CheckId(idtkb, idDonvi);
            if (idtkb <= 0 || !check_tkb)
                return ApiResult.BadRequest($"Id thời khoá biểu = {idtkb} không hợp lệ, vui lòng kiểm tra lại");

            // Lấy bản ghi từ db
            var detail = _sotiet.GetSotiet_Lop(idDonvi,idtkb);
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpGet("phonghoc")]
        [RequireToken]
        public IActionResult GetList_Phong([FromQuery] int idtkb)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var check_tkb = _tkb.CheckId(idtkb, idDonvi);
            if (idtkb <= 0 || !check_tkb)
                return ApiResult.BadRequest($"Id thời khoá biểu = {idtkb} không hợp lệ, vui lòng kiểm tra lại");

            // Lấy bản ghi từ db
            var detail = _sotiet.GetSotiet_Phong(idDonvi,idtkb);
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpGet("giaovien")]
        [RequireToken]
        public IActionResult GetList_Giaovien([FromQuery] int idtkb)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var check_tkb = _tkb.CheckId(idtkb, idDonvi);
            if (idtkb <= 0 || !check_tkb)
                return ApiResult.BadRequest($"Id thời khoá biểu = {idtkb} không hợp lệ, vui lòng kiểm tra lại");

            // Lấy bản ghi từ db
            var detail = _sotiet.GetSotiet_Giaovien(idDonvi,idtkb);
            return ApiResult.Success(detail, "Thành công");
        }
    }
}
