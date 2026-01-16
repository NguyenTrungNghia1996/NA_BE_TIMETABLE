using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/hocsinh")]
    [ApiController]
    public class DM_HocsinhController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_HocsinhRepository _hocsinh;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IDM_LophocRepository _lop;
        private readonly IValidateRepository _validate;
        public DM_HocsinhController(IMapper mapper, IDM_HocsinhRepository hocsinh, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                     IValidateRepository validate, IDM_LophocRepository lop)
        {
            _mapper = mapper;
            _hocsinh = hocsinh;
            _claimHelperRepository = claimHelperRepository;
            _lop = lop;
            _validate = validate;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _hocsinh.GetList_Paging(PageIndex, PageSize, search, idDonvi);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<DM_Hocsinh_ListDto>>(list);
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
            var detailhocsinh = _hocsinh.GetDetailById(Id, idDonvi);
            if (detailhocsinh == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            
            return ApiResult.Success(detailhocsinh, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_HocsinhDto hocsinh)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // mapper data 
            var item = _mapper.Map<DM_Hocsinh>(hocsinh);
            item.Id = 0;
            item.Id_don_vi = idDonvi;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool checkma = _hocsinh.CheckMa(hocsinh.Ma, idDonvi, item.Id);
            if (checkma)
            {
                return ApiResult.BadRequest("Mã học sinh đã tồn tại");
            }
            bool checklop = _lop.CheckId(hocsinh.Id_lop_chinh, idDonvi);
            if (!checklop)
            {
                return ApiResult.BadRequest("Id lớp học không hợp lệ, vui lòng kiểm tra lại");
            }

            
            // add 
            bool add = _hocsinh.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            var itemDto = _mapper.Map<DM_HocsinhDto>(item);
            
            return ApiResult.Success(new
            {
                item = itemDto
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_HocsinhDto hocsinh)
        {// Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var hocsinhdb = _hocsinh.GetDetailById(hocsinh.Id, idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (hocsinhdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var item = _mapper.Map<DM_Hocsinh>(hocsinh);
            item.Id_don_vi = idDonvi;
            
            bool checkma = _hocsinh.CheckMa(hocsinh.Ma, idDonvi, hocsinh.Id);
            if (checkma)
            {
                return ApiResult.BadRequest("Mã học sinh đã tồn tại");
            }
            bool checklop = _lop.CheckId(hocsinh.Id_lop_chinh, idDonvi);
            if (!checklop)
            {
                return ApiResult.BadRequest("Id lớp học không hợp lệ, vui lòng kiểm tra lại");
            }
            
            bool add = _hocsinh.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var itemDto = _mapper.Map<DM_HocsinhDto>(item);
            return ApiResult.Success(new
            {
                item = itemDto
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
            var hocsinhdb = _hocsinh.GetDetailById(id, idDonvi);
            if (hocsinhdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            bool request = _hocsinh.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
