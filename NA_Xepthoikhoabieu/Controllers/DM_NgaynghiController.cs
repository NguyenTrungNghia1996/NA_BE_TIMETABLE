using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/ngaynghi")]
    [ApiController]
    public class DM_NgaynghiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_NgaynghiRepository _ngaynghi;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IValidateRepository _validate;
        public DM_NgaynghiController(IMapper mapper, IDM_NgaynghiRepository ngaynghi, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, IValidateRepository validate)
        {
            _mapper = mapper;
            _ngaynghi = ngaynghi;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _validate = validate;
        }
        [HttpGet]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            search = search.Trim();
            var list = _ngaynghi.GetList_Paging(PageIndex, PageSize, search, ref totalrecord);
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
            var detailNgaynghi = _ngaynghi.GetDetailById(Id);
            if (detailNgaynghi == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            return ApiResult.Success(detailNgaynghi, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_Ngaynghi ngaynghi)
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }

            ngaynghi.Id = 0;
            bool checkten = _validate.CheckTrungTen<DM_Ngaynghi>(ngaynghi.Ten);
            if (checkten)
            {
                return ApiResult.BadRequest("Tên năm học đã tồn tại");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // check từ ngày, đến ngày
            if (ngaynghi.Tu_ngay > ngaynghi.Den_ngay)
            {
                return ApiResult.BadRequest("Từ ngày phải nhỏ hơn đến ngày");
            }
            // add 
            bool add = _ngaynghi.Add(ngaynghi);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            return ApiResult.Success(new
            {
                item = ngaynghi
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_Ngaynghi ngaynghi)
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // Kiểm tra bản ghi hợp lệ
            var ngaynghidb = _ngaynghi.GetDetailById(ngaynghi.Id);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (ngaynghidb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            bool checkten = _validate.CheckTrungTen<DM_Ngaynghi>(ngaynghi.Ten, ngaynghi.Id);
            if (checkten)
            {
                return ApiResult.BadRequest("Tên nam học đã tồn tại");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // check từ ngày, đến ngày
            if (ngaynghi.Tu_ngay > ngaynghi.Den_ngay)
            {
                return ApiResult.BadRequest("Từ ngày phải nhỏ hơn đến ngày");
            }
            bool add = _ngaynghi.Update(ngaynghi);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(new
            {
                item = ngaynghi
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
            var ngaynghidb = _ngaynghi.GetDetailById(id);
            if (ngaynghidb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            bool delete = _ngaynghi.Delete(id);
            if (!delete)
                return ApiResult.BadRequest("Xoá không thành công");
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
