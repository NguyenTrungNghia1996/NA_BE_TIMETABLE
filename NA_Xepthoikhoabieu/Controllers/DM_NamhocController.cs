using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/namhoc")]
    [ApiController]
    public class DM_NamhocController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_NamhocRepository _namhoc;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IValidateRepository _validate;
        public DM_NamhocController(IMapper mapper, IDM_NamhocRepository namhoc, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, IValidateRepository validate)
        {
            _mapper = mapper;
            _namhoc = namhoc;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _validate = validate;
        }
        [HttpGet]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            int idDonvi = 0;
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            search = search.Trim();
            var list = _namhoc.GetList_Paging(PageIndex, PageSize, search, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            return ApiResult.Success(new
            {
                items = list,
                totalrecord = totalrecord
            },
            "Thành công");
        }
        
        [HttpGet("detail")]
        [RequireToken]
        public IActionResult GetDetailByID([FromQuery] int Id)
        {

            // Lấy bản ghi từ db
            var detailNamhoc = _namhoc.GetDetailById(Id);
            if (detailNamhoc == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            return ApiResult.Success(detailNamhoc, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_Namhoc namhoc)
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // mapper data 
            namhoc.Id = 0;
            bool checkten = _validate.CheckTrungTen<DM_Namhoc>(namhoc.Ten);
            if (checkten)
            {
                return ApiResult.BadRequest("Tên năm học đã tồn tại");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // add 
            bool add = _namhoc.Add(namhoc);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            return ApiResult.Success(new
            {
                item = namhoc
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_Namhoc namhoc)
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // Kiểm tra bản ghi hợp lệ
            var namhocdb = _namhoc.GetDetailById(namhoc.Id);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (namhocdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            bool checkten = _validate.CheckTrungTen<DM_Namhoc>(namhoc.Ten, namhoc.Id);
            if (checkten)
            {
                return ApiResult.BadRequest("Tên nam học đã tồn tại");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool add = _namhoc.Update(namhoc);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(new
            {
                item = namhoc
            },
            "Cập nhật thành công");
        }
        [HttpDelete]
        [RequireToken]
        public IActionResult Delete([FromQuery] int id)
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            var namhocdb = _namhoc.GetDetailById(id);
            if (namhocdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var (success, message) = _namhoc.Delete(id);
            if (!success)
                return ApiResult.BadRequest(message);
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
