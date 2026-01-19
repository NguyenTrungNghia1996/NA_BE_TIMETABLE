using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/Phanphoi_Chuongtrinh_Chitiet")]
    [ApiController]
    public class Phanphoi_Chuongtrinh_ChitietController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPhanphoi_Chuongtrinh_ChitietRepository _ppctct;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IValidateRepository _validate;
        private readonly IPhanphoi_ChuongtrinhRepository _ppct;
        public Phanphoi_Chuongtrinh_ChitietController(IMapper mapper, IPhanphoi_Chuongtrinh_ChitietRepository ppctct, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, IValidateRepository validate,
                                                      IPhanphoi_ChuongtrinhRepository ppct)
        {
            _mapper = mapper;
            _ppctct = ppctct;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _validate = validate;
            _ppct = ppct;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] int idPpct, [FromQuery] string search = "" )
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }
            
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            search = search.Trim();
            var list = _ppctct.GetList_Paging(PageIndex, PageSize, search, idPpct, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            return ApiResult.Success(new
            {
                items = list,
                totalrecord = totalrecord
            },
            "Thành công");
        }

        //[HttpGet("detail")]
        //[RequireToken]
        //public IActionResult GetDetailByID([FromQuery] int Id)
        //{

        //    // Lấy bản ghi từ db
        //    var detailppctct = _ppctct.GetDetailById(Id);
        //    if (detailppctct == null)
        //        return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
        //    return ApiResult.Success(detailppctct, "Thành công");
        //}
        //[HttpPost]
        //[RequireToken]
        //public IActionResult Create([FromBody] Phanphoi_Chuongtrinh_Chitiet ppctct)
        //{
        //    int idDonvi = _claimHelperRepository.GetIdDonvi(User);
        //    if (idDonvi == 0)
        //    {
        //        return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
        //    }

        //    ppctct.Id = 0;
        //    //check validate
        //    bool checktrung = _ppctct.CheckTrungTuanTiet(ppctct, idDonvi);
        //    if (checktrung)
        //    {
        //        return ApiResult.BadRequest("Cặp tuần - tiết này đã được tạo");
        //    }
        //    bool checkppct = _ppct.CheckId(ppctct.Id_ppct, idDonvi);
        //    if (!checkppct)
        //    {
        //        return ApiResult.BadRequest($"Id phân phối chương trình = {ppctct.Id_ppct} không hợp lệ");
        //    }
            
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    // add 
        //    bool add = _ppctct.Add(ppctct);
        //    if (!add)
        //        return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");

        //    return ApiResult.Success(new
        //    {
        //        item = ppctct
        //    },
        //    "Thêm mới thành công");
        //}
        //[HttpPut]
        //[RequireToken]
        //public IActionResult Update([FromBody] Phanphoi_Chuongtrinh_Chitiet ppctct)
        //{
        //    int idDonvi = _claimHelperRepository.GetIdDonvi(User);
        //    if (idDonvi == 0)
        //    {
        //        return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
        //    }
        //    // Kiểm tra bản ghi hợp lệ
        //    var ppctctdb = _ppctct.GetDetailById(ppctct.Id);
        //    if (!ModelState.IsValid)
        //        return ApiResult.BadRequest(ModelState.GetErrorsAsString());
        //    if (ppctctdb == null)
        //        return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
        //    //check validate
        //    bool checktrung = _ppctct.CheckTrungTuanTiet(ppctct, idDonvi);
        //    if (checktrung)
        //    {
        //        return ApiResult.BadRequest("Cặp tuần - tiết này đã được tạo");
        //    }
        //    bool checkppct = _ppct.CheckId(ppctct.Id_ppct, idDonvi);
        //    if (!checkppct)
        //    {
        //        return ApiResult.BadRequest($"Id phân phối chương trình = {ppctct.Id_ppct} không hợp lệ");
        //    }
            
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);
        //    //sửa
        //    bool add = _ppctct.Update(ppctct);
        //    if (!add)
        //        return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
        //    return ApiResult.Success(new
        //    {
        //        item = ppctct
        //    },
        //    "Cập nhật thành công");
        //}
        //[HttpDelete]
        //[RequireToken]
        //public IActionResult Delete([FromQuery] int id)
        //{
        //    int idUser = _claimHelperRepository.GetUserId(User);
        //    // kiểm tra nếu là admin thì được truy cập
        //    bool checkIsAdmin = _auth.checkIsAdmin(idUser);
        //    if (!checkIsAdmin)
        //    {
        //        return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
        //    }
        //    var ppctctdb = _ppctct.GetDetailById(id);
        //    if (ppctctdb == null)
        //        return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
        //    bool delete = _ppctct.Delete(id);
        //    if (!delete)
        //        return ApiResult.BadRequest("Xoá không thành công");
        //    return ApiResult.Ok("Xóa thành công");
        //}
        [HttpPost("import")]
        [RequireToken]
        public IActionResult Import(IFormFile file, [FromForm] int idppct)
        {
            try
            {
                int idDonvi = _claimHelperRepository.GetIdDonvi(User);
                if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
                bool checkppct = _ppct.CheckId(idppct, idDonvi);
                if (!checkppct)
                {
                    return ApiResult.BadRequest($"Id phân phối chương trình = {idppct} không hợp lệ");
                }
                if (file == null || file.Length == 0)
                    return ApiResult.BadRequest("Vui lòng chọn file");

                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension != ".xlsx" && extension != ".xls")
                    return ApiResult.BadRequest("Chỉ chấp nhận file Excel (.xlsx, .xls)");
                var (result, mess) = (false,"");
                using (var stream = file.OpenReadStream())
                {
                    (result, mess) = _ppctct.Import(idppct, stream, idDonvi);
                }
                if (!result)
                    return ApiResult.BadRequest(mess);
                return ApiResult.Success(mess);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
