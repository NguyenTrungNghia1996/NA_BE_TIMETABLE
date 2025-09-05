using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/tochuyenmon")]
    [ApiController]
    public class DM_TochuyenmonController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_TochuyenmonRepository _tochuyenmon;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IValidateRepository _validate;
        public DM_TochuyenmonController(IMapper mapper, IDM_TochuyenmonRepository tochuyenmon, IClaimHelperRepository claimHelperRepository, IValidateRepository validate)
        {
            _mapper = mapper;
            _tochuyenmon = tochuyenmon;
            _claimHelperRepository = claimHelperRepository;
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
            var list = _tochuyenmon.GetList_Paging(PageIndex, PageSize, search, idDonvi, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            var listDto = _mapper.Map<List<DM_Tochuyenmon_ListDto>>(list);
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
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy bản ghi từ db
            var detail = _tochuyenmon.GetDetailById(Id, idDonvi);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<DM_TochuyenmonDto>(detail);
            return ApiResult.Success(detailDto, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_TochuyenmonDto tochuyenmon)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // mapper data 
            var item = _mapper.Map<DM_Tochuyenmon>(tochuyenmon);
            item.Id = 0;
            item.Id_don_vi = idDonvi;
            bool checkten = _validate.CheckTrungTen_byDonvi<DM_Tochuyenmon>(idDonvi, tochuyenmon.Ten);
            if (checkten)
            {
                ModelState.AddModelError("Ten", "Tên tổ chuyên môn đã tồn tại");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // add 
            bool add = _tochuyenmon.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            var itemDto = _mapper.Map<DM_TochuyenmonDto>(item);
            return ApiResult.Success(new
            {
                item = itemDto
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_TochuyenmonDto tochuyenmon)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var db = _tochuyenmon.GetDetailById(tochuyenmon.Id, idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (db == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            bool checkten = _validate.CheckTrungTen_byDonvi<DM_Tochuyenmon>(idDonvi, tochuyenmon.Ten, tochuyenmon.Id);
            if (checkten)
            {
                ModelState.AddModelError("Ten", "Tên tổ chuyên môn đã tồn tại");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var item = _mapper.Map<DM_Tochuyenmon>(tochuyenmon);
            item.Id_don_vi = idDonvi;
            bool add = _tochuyenmon.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var itemDto = _mapper.Map<DM_TochuyenmonDto>(item);
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
            var item = _tochuyenmon.GetDetailById(id, idDonvi);
            if (item == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var request = _tochuyenmon.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
