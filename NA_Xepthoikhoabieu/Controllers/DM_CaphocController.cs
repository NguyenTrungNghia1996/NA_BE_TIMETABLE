using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/caphoc")]
    [ApiController]
    public class DM_CaphocController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_CaphocRepository _caphocRepository;
        private readonly IClaimHelperRepository _clamHelperRepository;
        public DM_CaphocController(IMapper mapper,
                                   IDM_CaphocRepository caphocRepository,
                                   IClaimHelperRepository clamHelperRepository
                                )
        {
            _caphocRepository = caphocRepository;
            _mapper = mapper;
            _clamHelperRepository = clamHelperRepository;
        }
        // Get list Caphoc paging
        [HttpGet]
        [RequireToken]
        public IActionResult Getlist_Pageing([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _caphocRepository.GetList_Paging(PageIndex, PageSize, search, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.Ok("Không tồn tại bản ghi hợp nào");
            var listDto = _mapper.Map<List<DM_Caphoc_ListDto>>(list);
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
            if (Id <= 0)
                return ApiResult.BadRequest($"Id {Id} không hợp lệ, vui lòng kiểm tra lại");
            // Lấy bản ghi từ db
            var detailCaphoc = _caphocRepository.GetDetailByID(Id);
            if (detailCaphoc == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<DM_Caphoc_Dto>(detailCaphoc);
            return ApiResult.Success(detailDto,
            "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_Caphoc_Dto caphoc)
        {          
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            // mapper data 
            var item = _mapper.Map<DM_Caphoc>(caphoc);
            item.Id = 0;
            item.Trang_thai_xoa = false;
            // add 
            bool add = _caphocRepository.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            var itemDto = _mapper.Map<DM_Caphoc_Dto>(item);
            return ApiResult.Success(new
            {
                item = itemDto
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_Caphoc_Dto caphoc)
        {

            // Kiểm tra bản ghi hợp lệ
            var caphocdb = _caphocRepository.GetDetailByID(caphoc.Id);          
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (caphocdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            var item = _mapper.Map<DM_Caphoc>(caphoc);
            bool add = _caphocRepository.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var itemDto = _mapper.Map<DM_Caphoc_Dto>(item);
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
            // Kiểm tra bản ghi hợp lệ
            var caphocdb = _caphocRepository.GetDetailByID(id);        
            if (caphocdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var request = _caphocRepository.Deleted(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
