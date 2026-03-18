using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.IRepository.Auth;
using NA_Logic.IRepository.LichOnTap;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers.LichOnTap
{
    [Route("api/lopontap/hocsinh")]
    [ApiController]
    public class Hocsinh_LoponController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHocsinh_LoponRepository _hl;
        private readonly IDM_HocsinhRepository _hocsinh;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IDM_LopontapRepository _lop;
        private readonly IValidateRepository _validate;
        public Hocsinh_LoponController(IMapper mapper, IHocsinh_LoponRepository hl, IDM_HocsinhRepository hocsinh, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                     IValidateRepository validate, IDM_LopontapRepository lop)
        {
            _mapper = mapper;
            _hl = hl;
            _hocsinh = hocsinh;
            _claimHelperRepository = claimHelperRepository;
            _lop = lop;
            _validate = validate;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] int Id_lop_on, [FromQuery] string search = "")
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy danh sách dữ liệu

            var list = _hl.GetList_Paging(PageIndex, PageSize, search, idDonvi, Id_lop_on);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<Hocsinh_Lopon_ListDto>>(list);
            int totalrecord = list.First().Total;
            return ApiResult.Success(new
            {
                items = listDto,
                totalrecord
            },
            "Thành công");
        }
        [HttpGet("detail")]
        [RequireToken]
        public IActionResult GetDetailByID([FromQuery] int Id)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            // Lấy bản ghi từ db
            var detailhocsinh = _hl.GetDetailById(Id, idDonvi);
            if (detailhocsinh == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");

            return ApiResult.Success(detailhocsinh, "Thành công");
        }
        [HttpGet("list")]
        [RequireToken]
        public IActionResult GetHocSinhByIDLop([FromQuery] int IdLop)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            bool checklop = _lop.CheckId(IdLop, idDonvi);
            if (!checklop)
            {
                return ApiResult.BadRequest("Id lớp học không hợp lệ, vui lòng kiểm tra lại");
            }
            // Lấy bản ghi từ db
            var detailhocsinh = _hl.GetHocSinhByIdLop(IdLop, idDonvi);
            if (detailhocsinh == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {IdLop}");

            return ApiResult.Success(detailhocsinh, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] Hocsinh_Lopon_Multi data)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            bool checklop = _lop.CheckId(data.Id_lop, idDonvi);
            if (!checklop || data.Id_lop <= 0)
            {
                return ApiResult.BadRequest("Id lớp học không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkhocsinh = _hocsinh.CheckIds(data.Hoc_sinh, idDonvi);
            if (!checklop || data.Id_lop <= 0)
            {
                return ApiResult.BadRequest("Id học sinh không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkkhoi = _hl.CheckListStudentsSameGrade(data, idDonvi);
            if (!checkkhoi)
                return ApiResult.BadRequest("Chỉ được thêm học sinh cùng khối với lớp ôn");
            // add 
            bool add = _hl.Add(data);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");

            return ApiResult.Success("Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] Hocsinh_Lopon hocsinh)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var hocsinhdb = _hl.GetDetailById(hocsinh.Id, idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (hocsinhdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool checklop = _lop.CheckId(hocsinh.Id_lop_on, idDonvi);
            if (!checklop)
            {
                return ApiResult.BadRequest("Id lớp học không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkhs = _hocsinh.CheckId(hocsinh.Id_hoc_sinh, idDonvi);
            if (!checkhs)
            {
                return ApiResult.BadRequest("Id học sinh không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkkhoi = _hl.CheckStudentSameGrade(hocsinh, idDonvi);
            if (!checkkhoi)
                return ApiResult.BadRequest("Học sinh phải cùng khối với lớp ôn");
            bool checktrung = _hl.CheckTrung(hocsinh, idDonvi);
            if (checktrung)
            {
                return ApiResult.BadRequest("Học sinh này đã ở trong lớp này");
            }    
            bool add = _hl.Update(hocsinh);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success("Cập nhật thành công");
        }
        [HttpDelete]
        [RequireToken]
        public IActionResult Delete([FromQuery] int id)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var hocsinhdb = _hl.GetDetailById(id, idDonvi);
            if (hocsinhdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            bool request = _hl.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
        [HttpPost("import")]
        [RequireToken]
        public IActionResult ImportStudents(IFormFile file)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if (file == null || file.Length == 0)
                return ApiResult.BadRequest("Vui lòng chọn file" );


            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
                return ApiResult.BadRequest("Chỉ chấp nhận file Excel (.xlsx, .xls)");

            var result = _hl.Import(file, idDonvi);

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
