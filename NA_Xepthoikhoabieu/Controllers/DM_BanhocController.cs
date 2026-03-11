using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.IRepository.Auth;
using NA_Logic.IRepository.XepThoiKhoaBieu;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/banhoc")]
    [ApiController]
    public class DM_BanhocController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_BanhocRepository _Banhoc;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_CaphocRepository _cap;
        private readonly IValidateRepository _validate;
        public DM_BanhocController(IMapper mapper, IDM_BanhocRepository Banhoc, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, IDM_CaphocRepository cap, IValidateRepository validate)
        {
            _mapper = mapper;
            _Banhoc = Banhoc;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _cap = cap;
            _validate = validate;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            search = search.Trim();
            var list = _Banhoc.GetList_Paging(PageIndex, PageSize, search, idDonvi, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<DM_Banhoc_ListDto>>(list);
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
            var detailBanhoc = _Banhoc.GetDetailById(Id);
            if (detailBanhoc == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<DM_BanhocDto>(detailBanhoc);
            return ApiResult.Success(detailDto, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_BanhocDto Banhoc)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // mapper data 
            var item = _mapper.Map<DM_Banhoc>(Banhoc);
            item.Id = 0;
            item.Id_don_vi = idDonvi;
            bool checkten = _validate.CheckTrungTen_byDonvi<DM_Banhoc>(idDonvi,Banhoc.Ten);
            if (checkten)
            {
               return ApiResult.BadRequest("Tên ban học đã tồn tại");
            }
            var check_cap = _cap.CheckId(Banhoc.Id_cap_hoc);
            if (!check_cap)
                ModelState.AddModelError("Id_cap_hoc", "Id cấp học không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // add 
            bool add = _Banhoc.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            var itemDto = _mapper.Map<DM_BanhocDto>(item);
            return ApiResult.Success(new
            {
                item = itemDto
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_BanhocDto Banhoc)
        {// Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var Banhocdb = _Banhoc.GetDetailById(Banhoc.Id);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (Banhocdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            var item = _mapper.Map<DM_Banhoc>(Banhoc);
            item.Id_don_vi = idDonvi;
            bool checkten = _validate.CheckTrungTen_byDonvi<DM_Banhoc>(idDonvi, Banhoc.Ten, Banhoc.Id);
            if (checkten)
            {
                return ApiResult.BadRequest("Tên ban học đã tồn tại");
            }
            var check_cap = _cap.CheckId(Banhoc.Id_cap_hoc);
            if (!check_cap)
                ModelState.AddModelError("Id_cap_hoc", "Id cấp học không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool add = _Banhoc.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var itemDto = _mapper.Map<DM_BanhocDto>(item);
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
            var Banhocdb = _Banhoc.GetDetailById(id);
            if (Banhocdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var (success, message) = _Banhoc.Delete(id);
            if (!success)
                return ApiResult.BadRequest(message);
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
