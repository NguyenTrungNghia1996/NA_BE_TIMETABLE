using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/ngayhoc")]
    [ApiController]
    public class DM_NgayhocController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_NgayhocRepository _Ngayhoc;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        public DM_NgayhocController(IMapper mapper, IDM_NgayhocRepository Ngayhoc, IClaimHelperRepository claimHelperRepository, IAuthRepository auth)
        {
            _mapper = mapper;
            _Ngayhoc = Ngayhoc;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _Ngayhoc.GetList_Paging(PageIndex, PageSize, search, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            var listDto = _mapper.Map<List<DM_Ngayhoc_ListDto>>(list);
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
            var detailNgayhoc = _Ngayhoc.GetDetailById(Id);
            if (detailNgayhoc == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<DM_NgayhocDto>(detailNgayhoc);
            return ApiResult.Success(detailDto, "Thành công");
        }
        [HttpGet("dsngaytheodonvi")]
        [RequireToken]
        public IActionResult GetListByID()
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy bản ghi từ db
            var listngaybydv = _Ngayhoc.GetListNgayhocByDonvi(idDonvi);
            if (listngaybydv == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho đơn vị có Id= {idDonvi}");
            return ApiResult.Success(listngaybydv, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_NgayhocDto Ngayhoc)
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // mapper data 
            var item = _mapper.Map<DM_Ngayhoc>(Ngayhoc);
            item.Id = 0;
            // add 
            bool add = _Ngayhoc.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            var itemDto = _mapper.Map<DM_NgayhocDto>(item);
            return ApiResult.Success(new
            {
                item = itemDto
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_NgayhocDto Ngayhoc)
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // Kiểm tra bản ghi hợp lệ
            var Ngayhocdb = _Ngayhoc.GetDetailById(Ngayhoc.Id);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (Ngayhocdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            var item = _mapper.Map<DM_Ngayhoc>(Ngayhoc);
            bool add = _Ngayhoc.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var itemDto = _mapper.Map<DM_NgayhocDto>(item);
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
            int idUser = _claimHelperRepository.GetUserId(User);
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            var Ngayhocdb = _Ngayhoc.GetDetailById(id);
            if (Ngayhocdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var request = _Ngayhoc.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
        
    }
}
