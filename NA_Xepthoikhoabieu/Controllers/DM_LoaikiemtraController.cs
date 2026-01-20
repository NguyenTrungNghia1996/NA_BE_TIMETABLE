using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/loaikiemtra")]
    [ApiController]
    public class DM_LoaikiemtraController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_LoaikiemtraRepository _Loaikiemtra;
        private readonly IClaimHelperRepository _claimHelperRepository;
        public DM_LoaikiemtraController(IMapper mapper, IDM_LoaikiemtraRepository Loaikiemtra, IClaimHelperRepository claimHelperRepository, IAuthRepository auth)
        {
            _mapper = mapper;
            _Loaikiemtra = Loaikiemtra;
            _claimHelperRepository = claimHelperRepository;
;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy danh sách dữ liệu

            var list = _Loaikiemtra.GetList_Paging(PageIndex, PageSize, search, idDonvi);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<DM_Loaikiemtra_ListDto>>(list);
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
            var detailLoaikiemtra = _Loaikiemtra.GetDetailById(Id, idDonvi);
            if (detailLoaikiemtra == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");

            return ApiResult.Success(detailLoaikiemtra, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_LoaikiemtraDto Loaikiemtra)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // mapper data 
            var item = _mapper.Map<DM_Loaikiemtra>(Loaikiemtra);
            item.Id = 0;
            item.Id_don_vi = idDonvi;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // add 
            bool add = _Loaikiemtra.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            var itemDto = _mapper.Map<DM_LoaikiemtraDto>(item);

            return ApiResult.Success(new
            {
                item = itemDto
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_LoaikiemtraDto Loaikiemtra)
        {// Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var Loaikiemtradb = _Loaikiemtra.GetDetailById(Loaikiemtra.Id, idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (Loaikiemtradb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var item = _mapper.Map<DM_Loaikiemtra>(Loaikiemtra);
            item.Id_don_vi = idDonvi;

            bool add = _Loaikiemtra.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var itemDto = _mapper.Map<DM_LoaikiemtraDto>(item);
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
            var Loaikiemtradb = _Loaikiemtra.GetDetailById(id, idDonvi);
            if (Loaikiemtradb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            bool request = _Loaikiemtra.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
        
    }
}
