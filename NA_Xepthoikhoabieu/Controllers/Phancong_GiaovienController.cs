using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Xepthoikhoabieu.Helpers;
using System.Composition;

namespace NA_Xepthoikhoabieu.Controllers
{
    [ApiController]
    [Route("api/phanconggv")]
    public class Phancong_GiaovienController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IPhancong_GiaovienRepository _pcgv;
        private readonly IDM_LophocRepository _lop;
        private readonly IDM_MonhocRepository _mon;
        private readonly IDM_GiaovienRepository _giaovien;
        private readonly IDM_PhonghocRepository _phong;
        public Phancong_GiaovienController(IMapper mapper, IClaimHelperRepository claimHelperRepository, IDM_MonhocRepository mon, IDM_LophocRepository lop, IPhancong_GiaovienRepository pcgv,
            IDM_PhonghocRepository phong, IDM_GiaovienRepository giaovien)
        {
            _mapper = mapper;
            _claimHelperRepository = claimHelperRepository;
            _lop = lop;
            _mon = mon;
            _pcgv = pcgv;
            _phong = phong;
            _giaovien = giaovien;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList([FromQuery] int idgv, [FromQuery] int type = 1)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            bool check_gv = _giaovien.CheckId(idgv, idDonvi);
            if (!check_gv)
            {
                return ApiResult.BadRequest($"Id_giao_vien: {idgv} không hợp lệ");
            }
            if(type > 2 || type<= 0)
            {
                return ApiResult.BadRequest($"Type: {type} không hợp lệ");

            }
            // Lấy danh sách dữ liệu
            var list = _pcgv.GetList_Paging(idgv, idDonvi, type);
            if (list == null)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            return ApiResult.Success(list, "Thành công");
        }
        [HttpGet("lop")]
        [RequireToken]
        public IActionResult GetListLop([FromQuery] int idgv, [FromQuery] int idmon)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            bool check_gv = _giaovien.CheckId(idgv, idDonvi);
            if (!check_gv)
            {
                return ApiResult.BadRequest($"Id_giao_vien: {idgv} không hợp lệ");
            }
            bool check_mon = _mon.CheckId(idmon, idDonvi);
            if (!check_mon)
            {
                return ApiResult.BadRequest($"Id_mon: {idmon} không hợp lệ");
            }
            // Lấy danh sách dữ liệu
            var list = _pcgv.GetList_Lop_ByGvAndMon(idgv, idDonvi, idmon);
            if (list == null)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            return ApiResult.Success(list, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Update([FromBody] List<PhancongGVDto> phancongList)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }

            // Validate
            foreach (var phancong in phancongList)
            {
                bool check_gv = _giaovien.CheckId(phancong.Id_giao_vien, idDonvi);
                if (!check_gv)
                {
                    return ApiResult.BadRequest($"Id_giao_vien {phancong.Id_giao_vien} không hợp lệ");
                }

                bool check_mon = _mon.CheckId(phancong.Id_mon, idDonvi);
                if (!check_mon)
                {
                    return ApiResult.BadRequest($"Id_mon {phancong.Id_mon} không hợp lệ");
                }

                bool check_lop = _lop.CheckIds(phancong.Id_lop, idDonvi);
                if (!check_lop)
                {
                    return ApiResult.BadRequest($"Id_lop: {string.Join(", ", phancong.Id_lop)} không hợp lệ");
                }
            }

            bool result = _pcgv.Add(phancongList);
            if (!result)
            {
                return ApiResult.BadRequest("Cập nhật thông tin phân công không thành công");
            }

            return ApiResult.Success($"Phân công chuyên môn thành công");
        }
        [HttpGet("export")]
        [RequireToken]
        public IActionResult Export()
        {
            try
            {
                int idDonvi = _claimHelperRepository.GetIdDonvi(User);
                var excelBytes = _pcgv.Export(idDonvi);

                if (excelBytes == null)
                    return NotFound("Không có dữ liệu thời khóa biểu");

                var fileName = $"PhanCongGiaoVien_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                //header
                Response.Headers.Append("Content-Disposition", $"attachment; filename={fileName}; filename*=UTF-8''{Uri.EscapeDataString(fileName)}");
                Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition, Content-Length");
                return File(excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
    }
}
