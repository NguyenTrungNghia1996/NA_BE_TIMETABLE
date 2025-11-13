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
        private readonly IPhieu_BaogiangRepository _pbg;
        public Lich_BaogiangController(IMapper mapper, ILich_BaogiangRepository lgb, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, 
                                        IDM_NamhocRepository namhoc, IDanhsach_ThoikhoabieuRepository tkb, IPhieu_BaogiangRepository pbg)
        {
            _mapper = mapper;
            _lgb = lgb;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _namhoc = namhoc;
            _tkb = tkb;
            _pbg = pbg;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }
            
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
        [HttpGet("phieu")]
        [RequireToken]
        public IActionResult GetList_Paging_Phieu([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] int Idlbg, [FromQuery] string search = "")
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }
            bool checkIdlbg = _lgb.CheckId(Idlbg, idDonvi);
            if (!checkIdlbg)
                return ApiResult.BadRequest("Id lịch báo giảng không hợp lệ");
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            search = search.Trim();
            var list = _pbg.GetList_Paging(PageIndex, PageSize, Idlbg, search, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            return ApiResult.Success(new
            {
                items = list,
                totalrecord = totalrecord
            },
            "Thành công");
        }
        [HttpGet("phieu/chitiet")]
        [RequireToken]
        public IActionResult GetList_Paging_Phieu_Chitiet([FromQuery] int Idpbg)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }
            //bool checkIdlbg = _lgb.CheckId(Idlbg, idDonvi);
            //if (!checkIdlbg)
            //    return ApiResult.BadRequest("Id lịch báo giảng không hợp lệ");
            // Lấy danh sách dữ liệu
            
            var list = _pbg.GetList_Chitiet(Idpbg, idDonvi);
            if (list == null)
                return ApiResult.Ok();
            return ApiResult.Success(new
            {
                items = list
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
            bool checkppct = _lgb.CheckExistPPCT(lgb.Id_nam_hoc);
            if (!checkppct)
                return ApiResult.BadRequest("Năm học chưa có phân phối chương trình");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // add 
            var (result_lbg, mess_lbg) = _lgb.Add(lgb, idDonvi);
            if (!result_lbg)
                return ApiResult.NotFound(mess_lbg);

            bool addpbg = _pbg.Add(lgb.Id, lgb.Id_tkb, idDonvi);
            if (!addpbg)
                return ApiResult.BadRequest("Thêm phiếu báo giảng thất bại");

            return ApiResult.Success(new
            {
                item = lgb
            }, mess_lbg);
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

            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());

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
            bool checkppct = _lgb.CheckExistPPCT(lgb.Id_nam_hoc);
            if (!checkppct)
                return ApiResult.BadRequest("Năm học chưa có phân phối chương trình");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //sửa
            var (result, mess) = _lgb.Update(lgb);
            if (!result)
                return ApiResult.NotFound(mess);
            //nếu thay đổi tkb thì insert lại pbg
            bool checkchangetkb = _lgb.CheckChangeTKB(lgb);
            if (!checkchangetkb)
            {
                var deletepgb = _pbg.Delete(lgb.Id);
                if (!deletepgb.result)
                    return ApiResult.BadRequest(deletepgb.mess);
                bool addpbg = _pbg.Add(lgb.Id, lgb.Id_tkb, idDonvi);
                if (!addpbg)
                    return ApiResult.BadRequest("Thêm phiếu báo giảng thất bại");
            }
            return ApiResult.Success(new
            {
                item = lgb
            },mess);
        }
        [HttpDelete]
        [RequireToken]
        public IActionResult Delete([FromQuery] int id)
        {
            //check id đơn vị
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }
            //check id lbg
            var lgbdb = _lgb.GetDetailById(id);
            if (lgbdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            //xoá pbg
            var deletepgb = _pbg.Delete(id);
            if (!deletepgb.result)
                return ApiResult.BadRequest(deletepgb.mess);
            //xoá lbg
            bool delete = _lgb.Delete(id);
            if (!delete)
                return ApiResult.BadRequest("Xoá không thành công");

            return ApiResult.Ok("Xóa thành công");
        }
    }
}
