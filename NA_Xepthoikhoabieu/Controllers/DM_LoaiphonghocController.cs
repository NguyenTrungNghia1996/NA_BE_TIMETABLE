using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/loaiphonghoc")]
    [ApiController]
    public class DM_LoaiphonghocController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_LoaiphonghocRepository _loaiphonghoc;
        private readonly IClaimHelperRepository _claimHelperRepository;
        public DM_LoaiphonghocController(IMapper mapper, IDM_LoaiphonghocRepository loaiphonghoc, IClaimHelperRepository claimHelperRepository)
        {
            _mapper = mapper;
            _loaiphonghoc = loaiphonghoc;
            _claimHelperRepository = claimHelperRepository;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {

            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _loaiphonghoc.GetList_Paging(PageIndex, PageSize, search, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            var listDto = _mapper.Map<List<DM_Loaiphonghoc_ListDto>>(list);
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
            var detail = _loaiphonghoc.GetDetailByID(Id);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<DM_LoaiphonghocDto>(detail);
            return ApiResult.Success(detailDto, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_LoaiphonghocDto loaiphonghoc)
        {
            // mapper data 
            var item = _mapper.Map<DM_Loaiphonghoc>(loaiphonghoc);
            item.Id = 0;
            // add 
            bool add = _loaiphonghoc.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            var itemDto = _mapper.Map<DM_LoaiphonghocDto>(item);
            return ApiResult.Success(new
            {
                item = itemDto
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_LoaiphonghocDto loaiphonghoc)
        {

            // Kiểm tra bản ghi hợp lệ
            var db = _loaiphonghoc.GetDetailByID(loaiphonghoc.Id);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (db == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            var item = _mapper.Map<DM_Loaiphonghoc>(loaiphonghoc);
            bool add = _loaiphonghoc.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var itemDto = _mapper.Map<DM_LoaiphonghocDto>(item);
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

            var item = _loaiphonghoc.GetDetailByID(id);
            if (item == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var request = _loaiphonghoc.Deleted(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
