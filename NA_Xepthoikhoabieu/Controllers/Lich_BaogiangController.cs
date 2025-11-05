using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/lich_baogiang")]
    [ApiController]
    public class Lich_BaogiangController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILich_BaogiangRepository _lgb;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_NamhocRepository _namhoc;
        private readonly IDanhsach_ThoikhoabieuRepository _tkb;
        public Lich_BaogiangController(IMapper mapper, ILich_BaogiangRepository lgb, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, 
                                        IDM_NamhocRepository namhoc, IDanhsach_ThoikhoabieuRepository tkb)
        {
            _mapper = mapper;
            _lgb = lgb;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _namhoc = namhoc;
            _tkb = tkb;
        }
        [HttpGet]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            int idDonvi = 0;
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            search = search.Trim();
            var list = _lgb.GetList_Paging(PageIndex, PageSize, search, ref totalrecord);
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
            var detaillgb = _lgb.GetDetailById(Id);
            if (detaillgb == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            return ApiResult.Success(detaillgb, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] Lich_Baogiang lgb)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }

            lgb.Id = 0;
            //check validate
            bool checktrung = _lgb.CheckTrungTuan(lgb, idDonvi);
            if (checktrung)
            {
                return ApiResult.BadRequest("Lịch báo giảng tuần này đã được tạo");
            }
            bool checktkb = _tkb.CheckId(lgb.Id_tkb, idDonvi);
            if (!checktkb)
            {
                return ApiResult.BadRequest($"Id tkb = {lgb.Id_tkb} không hợp lệ");
            }
            bool checknam = _namhoc.CheckId(lgb.Id_nam_hoc);
            if (!checknam)
            {
                return ApiResult.BadRequest($"Id năm học = {lgb.Id_nam_hoc} không hợp lệ");
            }
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // add 
            var (result, mess) = _lgb.Add(lgb);
            if (!result)
                return ApiResult.NotFound(mess);
            return ApiResult.Success(new
            {
                item = lgb
            }, mess);
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] Lich_Baogiang lgb)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }
            // Kiểm tra bản ghi hợp lệ
            var lgbdb = _lgb.GetDetailById(lgb.Id);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (lgbdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            //check validate
            bool checktrung = _lgb.CheckTrungTuan(lgb, idDonvi);
            if (checktrung)
            {
                return ApiResult.BadRequest("Lịch báo giảng tuần này đã được tạo");
            }
            bool checktkb = _tkb.CheckId(lgb.Id_tkb, idDonvi);
            if (!checktkb)
            {
                return ApiResult.BadRequest($"Id tkb = {lgb.Id_tkb} không hợp lệ");
            }
            bool checknam = _namhoc.CheckId(lgb.Id_nam_hoc);
            if (!checknam)
            {
                return ApiResult.BadRequest($"Id năm học = {lgb.Id_nam_hoc} không hợp lệ");
            }
           
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //sửa
            var (result, mess) = _lgb.Update(lgb);
            if (!result)
                return ApiResult.NotFound(mess);
            return ApiResult.Success(new
            {
                item = lgb
            },mess);
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
            var lgbdb = _lgb.GetDetailById(id);
            if (lgbdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            bool delete = _lgb.Delete(id);
            if (!delete)
                return ApiResult.BadRequest("Xoá không thành công");
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
