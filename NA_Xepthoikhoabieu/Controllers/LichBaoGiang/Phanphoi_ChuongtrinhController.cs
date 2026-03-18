using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.IRepository.Auth;
using NA_Logic.IRepository.LichBaoGiang;
using NA_Logic.IRepository.XepThoiKhoaBieu;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;
using System.Composition;

namespace NA_Xepthoikhoabieu.Controllers.LichBaoGiang
{
    [Route("api/phanphoi_chuongtrinh")]
    [ApiController]
    public class Phanphoi_ChuongtrinhController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPhanphoi_ChuongtrinhRepository _ppct;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IValidateRepository _validate;
        private readonly IDM_BanhocRepository _banhoc;
        private readonly IDM_KhoilopRepository _khoilop;
        private readonly IDM_NamhocRepository _namhoc;
        private readonly IDM_MonhocRepository _monhoc;
        private readonly IPhanphoi_Chuongtrinh_ChitietRepository _chitiet;
        public Phanphoi_ChuongtrinhController(IMapper mapper, IPhanphoi_ChuongtrinhRepository ppct, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, IValidateRepository validate, 
                                              IDM_BanhocRepository banhoc, IDM_NamhocRepository namhoc, IDM_KhoilopRepository khoilop, IDM_MonhocRepository monhoc, IPhanphoi_Chuongtrinh_ChitietRepository chitiet)
        {
            _mapper = mapper;
            _ppct = ppct;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _validate = validate;
            _monhoc = monhoc;
            _namhoc = namhoc;
            _banhoc = banhoc;
            _khoilop = khoilop;
            _chitiet = chitiet;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] int IdKhoi, [FromQuery] int IdBan, [FromQuery] int IdMon, 
                                            [FromQuery] int IdNam, [FromQuery] string search = "")
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }
            if (IdNam > 0)
            {
                bool checknam = _namhoc.CheckId(IdNam);
                if (!checknam)
                {
                    return ApiResult.BadRequest($"Id năm học = {IdNam} không hợp lệ");
                }
            }
            if (IdNam < 0)
            {
                return ApiResult.BadRequest("Id năm học phải là số dương");
            }
            if (IdKhoi > 0)
            {
                bool checkkhoi = _khoilop.CheckId(IdKhoi);
                if (!checkkhoi)
                {
                    return ApiResult.BadRequest($"Id khối lớp = {IdKhoi} không hợp lệ");
                }
            }
            if (IdKhoi < 0)
            {
                return ApiResult.BadRequest("Id khối phải là số dương");
            }
            if (IdBan > 0)
            {
                bool checkban = _banhoc.CheckId(IdBan, idDonvi);
                if (!checkban)
                {
                    return ApiResult.BadRequest($"Id ban học = {IdBan} không hợp lệ");
                }
            }
            if (IdBan < 0)
            {
                return ApiResult.BadRequest("Id ban học phải là số dương");
            }
            if (IdMon > 0)
            {
                bool checkmon = _monhoc.CheckId(IdMon, idDonvi);
                if (!checkmon)
                {
                    return ApiResult.BadRequest($"Id môn học = {IdMon} không hợp lệ");
                }
            }
            if (IdMon < 0)
            {
                return ApiResult.BadRequest("Id môn học phải là số dương");
            }
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            search = search.Trim();
            var list = _ppct.GetList_Paging(PageIndex, PageSize,IdBan, IdKhoi, IdMon, IdNam, idDonvi, search, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            return ApiResult.Success(new
            {
                items = list,
                totalrecord
            },
            "Thành công");
        }

        [HttpGet("detail")]
        [RequireToken]
        public IActionResult GetDetailByID([FromQuery] int Id)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }
            // Lấy bản ghi từ db
            var detailppct = _ppct.GetDetailById(Id, idDonvi);
            if (detailppct == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            return ApiResult.Success(detailppct, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] Phanphoi_Chuongtrinh ppct)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi==0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }

            ppct.Id = 0;
            ppct.Id_don_vi = idDonvi;
            //check validate
            bool checktrung = _ppct.CheckTrung(ppct);
            if (checktrung)
            {
                return ApiResult.BadRequest("Phân phối chương trình này đã được tạo");
            }
            bool checknam = _namhoc.CheckId(ppct.Id_nam_hoc);
            if (!checknam) {
                return ApiResult.BadRequest($"Id năm học = {ppct.Id_nam_hoc} không hợp lệ");
            }
            bool checkkhoi = _khoilop.CheckId(ppct.Id_khoi);
            if (!checkkhoi) {
                return ApiResult.BadRequest($"Id khối lớp = {ppct.Id_khoi} không hợp lệ");
            }
            bool checkban = _banhoc.CheckId(ppct.Id_ban, idDonvi);
            if (!checkban) {
                return ApiResult.BadRequest($"Id ban học = {ppct.Id_ban} không hợp lệ");
            }
            bool checkmon = _monhoc.CheckId(ppct.Id_mon, idDonvi);
            if (!checkmon) {
                return ApiResult.BadRequest($"Id môn học = {ppct.Id_mon} không hợp lệ");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // add 
            bool add = _ppct.Add(ppct);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");

            return ApiResult.Success(new
            {
                item = ppct
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] Phanphoi_Chuongtrinh ppct)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }
            // Kiểm tra bản ghi hợp lệ
            var ppctdb = _ppct.GetDetailById(ppct.Id, idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (ppctdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            ppct.Id_don_vi= idDonvi;
            //check validate
            bool checktrung = _ppct.CheckTrung(ppct);
            if (checktrung)
            {
                return ApiResult.BadRequest("Phân phối chương trình này đã được tạo");
            }
            bool checknam = _namhoc.CheckId(ppct.Id_nam_hoc);
            if (!checknam)
            {
                return ApiResult.BadRequest($"Id năm học = {ppct.Id_nam_hoc} không hợp lệ");
            }
            bool checkkhoi = _khoilop.CheckId(ppct.Id_khoi);
            if (!checknam)
            {
                return ApiResult.BadRequest($"Id khối lớp = {ppct.Id_khoi} không hợp lệ");
            }
            bool checkban = _banhoc.CheckId(ppct.Id_ban, idDonvi);
            if (!checknam)
            {
                return ApiResult.BadRequest($"Id ban học = {ppct.Id_ban} không hợp lệ");
            }
            bool checkmon = _monhoc.CheckId(ppct.Id_mon, idDonvi);
            if (!checknam)
            {
                return ApiResult.BadRequest($"Id môn học = {ppct.Id_mon} không hợp lệ");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //sửa
            bool add = _ppct.Update(ppct);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(new
            {
                item = ppct
            },
            "Cập nhật thành công");
        }
        [HttpDelete]
        [RequireToken]
        public IActionResult Delete([FromQuery] int id)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }
            var ppctdb = _ppct.GetDetailById(id, idDonvi);
            if (ppctdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            bool checkConstraint = _chitiet.CheckConstraint(id, idDonvi);
            if (checkConstraint)
                return ApiResult.BadRequest("Đã tạo lịch báo giảng, không thể xoá");
            bool del_chitiet = _chitiet.Delete(id);
            if (!del_chitiet)
                return ApiResult.BadRequest("Xoá chi tiết không thành công");
            bool delete = _ppct.Delete(id);
            if (!delete)
                return ApiResult.BadRequest("Xoá không thành công");
            return ApiResult.Ok("Xóa thành công");
        }
        [HttpGet("export")]
        [RequireToken]
        public IActionResult Export([FromQuery] int id)
        {
            try
            {
                bool check_env = _claimHelperRepository.IsDemoSite();
                if (check_env)
                    return ApiResult.NotFound("Bạn cần đăng ký dùng bản chính thức để sử dụng chức năng này");
                int idDonvi = _claimHelperRepository.GetIdDonvi(User);
                var excelBytes = _ppct.ExportExcel_PPCT(id, idDonvi);
                var ppct = _ppct.GetDetailById(id, idDonvi);
                if (ppct == null)
                    return ApiResult.NotFound("Không tìm thấy phân phối chương trình");
                if (excelBytes == null)
                    return NotFound("Không có dữ liệu");

                var fileName = $"PhanPhoiChuongTrinh_{ppct.Ten}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                //header
                //Response.Headers.Append("Content-Disposition", $"attachment; filename={fileName}; filename*=UTF-8''{Uri.EscapeDataString(fileName)}");
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
