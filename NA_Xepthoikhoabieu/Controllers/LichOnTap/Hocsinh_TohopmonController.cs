using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.IRepository.Auth;
using NA_Logic.IRepository.LichOnTap;
using NA_Logic.IRepository.XepThoiKhoaBieu;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers.LichOnTap
{
    [Route("api/hocsinh/tohopmon")]
    [ApiController]
    public class Hocsinh_TohopmonController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHocsinh_TohopmonRepository _ht;
        private readonly IDM_HocsinhRepository _hocsinh;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IDM_Tohopmon_OntapRepository _thm;
        private readonly IDM_LophocRepository _lop;
        private readonly IValidateRepository _validate;
        public Hocsinh_TohopmonController(IMapper mapper, IHocsinh_TohopmonRepository hl, IDM_HocsinhRepository hocsinh, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                     IValidateRepository validate, IDM_Tohopmon_OntapRepository thm, IDM_LophocRepository lop)
        {
            _mapper = mapper;
            _ht = hl;
            _hocsinh = hocsinh;
            _claimHelperRepository = claimHelperRepository;
            _thm = thm;
            _lop = lop;
            _validate = validate;
        }
        
        [HttpGet()]
        [RequireToken]
        public IActionResult GetHocSinhByIDLop([FromQuery] int Id_hoc_sinh)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            bool checkhs = _hocsinh.CheckId(Id_hoc_sinh, idDonvi);
            if (!checkhs)
            {
                return ApiResult.BadRequest("Id học sinh không hợp lệ, vui lòng kiểm tra lại");
            }
            // Lấy bản ghi từ db
            var detailhocsinh = _ht.GetToHopByIdHocSinh(Id_hoc_sinh, idDonvi);
            if (detailhocsinh == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id_hoc_sinh}");

            return ApiResult.Success(detailhocsinh, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] Hocsinh_Tohopmon_Multi data)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            bool checkhocsinh = _hocsinh.CheckId(data.Id_hoc_sinh, idDonvi);
            if (!checkhocsinh || data.Id_hoc_sinh <= 0)
            {
                return ApiResult.BadRequest("Id học sinh không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkthm = _thm.CheckIds(data.To_hop_mon, idDonvi);
            if (!checkthm && data.To_hop_mon.Count > 0)
            {
                return ApiResult.BadRequest("Id tổ hợp môn không hợp lệ, vui lòng kiểm tra lại");
            }
            // add 
            bool add = _ht.Add(data);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại");

            return ApiResult.Success("Cập nhật thành công");
        }
        [HttpGet("mau")]
        [RequireToken]
        public IActionResult Export_MauExcel([FromQuery] int idlop)
        {
            try
            {
                bool check_env = _claimHelperRepository.IsDemoSite();
                if (check_env)
                    return ApiResult.NotFound("Bạn cần đăng ký dùng bản chính thức để sử dụng chức năng này");
                int idDonvi = _claimHelperRepository.GetIdDonvi(User);
                bool checkIdLop = _lop.CheckId(idlop, idDonvi);
                if (!checkIdLop)
                    return ApiResult.BadRequest("Id lớp học không hợp lệ");
                var excelBytes = _ht.ExportMauExcel(idlop);

                if (excelBytes == null)
                    return NotFound("Không có dữ liệu");

                var fileName = $"MauImportKetQua.xlsx";
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

        [HttpPost("import")]
        [RequireToken]
        public IActionResult ImportSubjectCombination(IFormFile file)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if (file == null || file.Length == 0)
                return ApiResult.BadRequest("Vui lòng chọn file");


            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
                return ApiResult.BadRequest("Chỉ chấp nhận file Excel (.xlsx, .xls)");

            var result = _ht.Import(file, idDonvi);

            if (result.success)
            {
                return ApiResult.Success("Import thành công");
            }
            else
            {
                return ApiResult.BadRequest(result.mess);
            }
        }
    }
}
