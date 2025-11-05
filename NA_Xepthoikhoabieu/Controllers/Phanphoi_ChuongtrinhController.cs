using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/phanphoi_chuongtrinh")]
    [ApiController]
    public class Phanphoi_ChuongtrinhController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPhanphoi_ChuongtrinhRepository _ppct;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IValidateRepository _validate;
        private readonly IDM_BanhocRepository _banhoc;
        private readonly IDM_KhoilopRepository _khoilop;
        private readonly IDM_NamhocRepository _namhoc;
        private readonly IDM_MonhocRepository _monhoc;
        public Phanphoi_ChuongtrinhController(IMapper mapper, IPhanphoi_ChuongtrinhRepository ppct, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, IValidateRepository validate, 
                                              IDM_BanhocRepository banhoc, IDM_NamhocRepository namhoc, IDM_KhoilopRepository khoilop, IDM_MonhocRepository monhoc)
        {
            _mapper = mapper;
            _ppct = ppct;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _validate = validate;
            _monhoc = monhoc;
            _namhoc = namhoc;
            _banhoc = banhoc;
            _khoilop = khoilop;
        }
        [HttpGet]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            int idDonvi = 0;
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            search = search.Trim();
            var list = _ppct.GetList_Paging(PageIndex, PageSize, search, ref totalrecord);
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
            var detailppct = _ppct.GetDetailById(Id);
            if (detailppct == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            return ApiResult.Success(detailppct, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] Phanphoi_Chuongtrinh ppct)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi==0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }

            ppct.Id = 0;
            ppct.Id_don_vi = idDonvi;
            //check validate
            bool checktrung = _ppct.CheckTrung(ppct);
            if (checktrung)
            {
                return ApiResult.BadRequest("Phân phối chương trình này đã được tạo");
            }
            bool checknam = _namhoc.CheckId(ppct.Id_nam_hoc);
            if (!checknam) {
                return ApiResult.BadRequest($"Id năm học = {ppct.Id_nam_hoc} không hợp lệ");
            }
            bool checkkhoi = _khoilop.CheckId(ppct.Id_khoi);
            if (!checkkhoi) {
                return ApiResult.BadRequest($"Id khối lớp = {ppct.Id_khoi} không hợp lệ");
            }
            bool checkban = _banhoc.CheckId(ppct.Id_ban, idDonvi);
            if (!checkban) {
                return ApiResult.BadRequest($"Id ban học = {ppct.Id_ban} không hợp lệ");
            }
            bool checkmon = _monhoc.CheckId(ppct.Id_mon, idDonvi);
            if (!checkmon) {
                return ApiResult.BadRequest($"Id môn học = {ppct.Id_mon} không hợp lệ");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // add 
            bool add = _ppct.Add(ppct);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");

            return ApiResult.Success(new
            {
                item = ppct
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] Phanphoi_Chuongtrinh ppct)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }
            // Kiểm tra bản ghi hợp lệ
            var ppctdb = _ppct.GetDetailById(ppct.Id);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (ppctdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            ppct.Id_don_vi= idDonvi;
            //check validate
            bool checktrung = _ppct.CheckTrung(ppct);
            if (checktrung)
            {
                return ApiResult.BadRequest("Phân phối chương trình này đã được tạo");
            }
            bool checknam = _namhoc.CheckId(ppct.Id_nam_hoc);
            if (!checknam)
            {
                return ApiResult.BadRequest($"Id năm học = {ppct.Id_nam_hoc} không hợp lệ");
            }
            bool checkkhoi = _khoilop.CheckId(ppct.Id_khoi);
            if (!checknam)
            {
                return ApiResult.BadRequest($"Id khối lớp = {ppct.Id_khoi} không hợp lệ");
            }
            bool checkban = _banhoc.CheckId(ppct.Id_ban, idDonvi);
            if (!checknam)
            {
                return ApiResult.BadRequest($"Id ban học = {ppct.Id_ban} không hợp lệ");
            }
            bool checkmon = _monhoc.CheckId(ppct.Id_mon, idDonvi);
            if (!checknam)
            {
                return ApiResult.BadRequest($"Id môn học = {ppct.Id_mon} không hợp lệ");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //sửa
            bool add = _ppct.Update(ppct);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(new
            {
                item = ppct
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
            var ppctdb = _ppct.GetDetailById(id);
            if (ppctdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            bool delete = _ppct.Delete(id);
            if (!delete)
                return ApiResult.BadRequest("Xoá không thành công");
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
