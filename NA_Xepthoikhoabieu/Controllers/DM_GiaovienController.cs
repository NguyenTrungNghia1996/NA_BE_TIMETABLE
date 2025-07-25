using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/giaovien")]
    [ApiController]
    public class DM_GiaovienController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_GiaovienRepository _Giaovien;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_CaphocRepository _cap;
        public DM_GiaovienController(IMapper mapper, IDM_GiaovienRepository Giaovien, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, IDM_CaphocRepository cap)
        {
            _mapper = mapper;
            _Giaovien = Giaovien;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _cap = cap;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {

            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _Giaovien.GetList_Paging(PageIndex, PageSize, search, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            var listDto = _mapper.Map<List<DM_Giaovien_ListDto>>(list);
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

            // Lấy bản ghi từ db
            var detailGiaovien = _Giaovien.GetDetailById(Id);
            if (detailGiaovien == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<DM_GiaovienDto>(detailGiaovien);
            return ApiResult.Success(detailDto, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_GiaovienDto Giaovien)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // mapper data 
            var item = _mapper.Map<DM_Giaovien>(Giaovien);
            item.Id = 0;
            item.Id_don_vi = idDonvi;
            //var check_cap = _cap.CheckId(Giaovien.Id_cap_hoc);
            //if (!check_cap)
            //    ModelState.AddModelError("Id_cap_hoc", "Id cấp học không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // add 
            bool add = _Giaovien.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            var itemDto = _mapper.Map<DM_GiaovienDto>(item);
            return ApiResult.Success(new
            {
                item = itemDto
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_GiaovienDto Giaovien)
        {// Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var Giaoviendb = _Giaovien.GetDetailById(Giaovien.Id);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (Giaoviendb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            var item = _mapper.Map<DM_Giaovien>(Giaovien);
            //var check_cap = _cap.CheckId(Giaovien.Id_cap_hoc);
            //if (!check_cap)
            //    ModelState.AddModelError("Id_cap_hoc", "Id cấp học không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool add = _Giaovien.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var itemDto = _mapper.Map<DM_GiaovienDto>(item);
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
            var Giaoviendb = _Giaovien.GetDetailById(id);
            if (Giaoviendb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var request = _Giaovien.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
