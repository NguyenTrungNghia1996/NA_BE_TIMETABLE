using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;
using System.Composition;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/baikiemtra")]
    [ApiController]
    public class DM_BaikiemtraController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_BaikiemtraRepository _kt;
        private readonly IDM_LoaikiemtraRepository _loaikt;
        private readonly IDM_LopontapRepository _lopon;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IKetqua_BaikiemtraRepository _ketqua;
        public DM_BaikiemtraController(IMapper mapper, IDM_BaikiemtraRepository kt, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                       IDM_LopontapRepository lopon, IDM_LoaikiemtraRepository loaikt, IKetqua_BaikiemtraRepository ketqua)
        {
            _mapper = mapper;
            _kt = kt;
            _loaikt = loaikt;
            _lopon = lopon;
            _claimHelperRepository = claimHelperRepository;
            _ketqua = ketqua;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy danh sách dữ liệu

            var list = _kt.GetList_Paging(PageIndex, PageSize, search, idDonvi);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<DM_Baikiemtra_ListDto>>(list);
            int totalrecord = list.First().Total;
            return ApiResult.Success(new
            {
                items = listDto,
                totalrecord = totalrecord
            },
            "Thành công");
        }
        [HttpGet("detail")]
        [RequireToken]
        public IActionResult GetDetailByID([FromQuery] int Id)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            // Lấy bản ghi từ db
            var detailBaikiemtra = _kt.GetDetailById(Id, idDonvi);
            if (detailBaikiemtra == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");

            return ApiResult.Success(detailBaikiemtra, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_Baikiemtra kt)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            kt.Id = 0;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool checklop = _lopon.CheckId(kt.Id_lop_on, idDonvi);
            if (!checklop)
            {
                return ApiResult.BadRequest("Id lớp học không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkloaikt = _loaikt.CheckId(kt.Id_loai_kiem_tra, idDonvi);
            if (!checkloaikt)
            {
                return ApiResult.BadRequest("Id loại bài kiểm tra không hợp lệ, vui lòng kiểm tra lại");
            }
            // add 
            bool add = _kt.Add(kt);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");

            return ApiResult.Success(new
            {
                item = kt
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_Baikiemtra kt)
        {// Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var Baikiemtradb = _kt.GetDetailById(kt.Id, idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (Baikiemtradb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool checklop = _lopon.CheckId(kt.Id_lop_on, idDonvi);
            if (!checklop)
            {
                return ApiResult.BadRequest("Id lớp học không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkloaikt = _loaikt.CheckId(kt.Id_loai_kiem_tra, idDonvi);
            if (!checkloaikt)
            {
                return ApiResult.BadRequest("Id loại bài kiểm tra không hợp lệ, vui lòng kiểm tra lại");
            }

            bool update = _kt.Update(kt);
            if (!update)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(new
            {
                item = kt
            },
            "Cập nhật thành công");
        }
        [HttpDelete]
        [RequireToken]
        public IActionResult Delete([FromQuery] int id)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var Baikiemtradb = _kt.GetDetailById(id, idDonvi);
            if (Baikiemtradb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            bool request = _kt.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
        [HttpGet("csdlnganh")]
        [RequireToken]
        public IActionResult Export_CSDLNganh([FromQuery] int idlop)
        {
            try
            {
                bool check_env = _claimHelperRepository.IsDemoSite();
                if (check_env)
                    return ApiResult.NotFound("Bạn cần đăng ký dùng bản chính thức để sử dụng chức năng này");
                int idDonvi = _claimHelperRepository.GetIdDonvi(User);
                var excelBytes = _ketqua.ExportMauExcel(idlop);

                if (excelBytes == null)
                    return NotFound("Không có dữ liệu thời khóa biểu");

                var fileName = $"DongBoCSDLNganh_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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
