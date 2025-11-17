using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;
using System.Composition;

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
        private readonly IExportWordRepository _export;
        private readonly IWebHostEnvironment _env;
        public Lich_BaogiangController(IMapper mapper, ILich_BaogiangRepository lgb, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, 
                                        IDM_NamhocRepository namhoc, IDanhsach_ThoikhoabieuRepository tkb, IPhieu_BaogiangRepository pbg, IExportWordRepository export, IWebHostEnvironment env)
        {
            _mapper = mapper;
            _lgb = lgb;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _namhoc = namhoc;
            _tkb = tkb;
            _pbg = pbg;
            _export = export;
            _env = env;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] int IdNam)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }
            if (IdNam > 0)
            {
                bool checknam = _namhoc.CheckId(IdNam);
                if (!checknam)
                {
                    return ApiResult.BadRequest($"Id năm học = {IdNam} không hợp lệ");
                }
            }
            if(IdNam < 0)
            {
                return ApiResult.BadRequest("Id năm học phải là số dương");
            }
                
            
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            //search = search.Trim();
            var list = _lgb.GetList_Paging(PageIndex, PageSize, IdNam, ref totalrecord);
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
            //bool checktrung = _lgb.CheckTrungTuan(lgb, idDonvi);
            //if (checktrung)
            //{
            //    return ApiResult.BadRequest("Lịch báo giảng tuần này đã được tạo");
            //}
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
            var lgbdb = _lgb.GetDetailById(lgb.Id);
            if (lgbdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {lgb.Id} không tồn tại, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());

            //check validate
            //bool checktrung = _lgb.CheckTrungTuan(lgb, idDonvi);
            //if (checktrung)
            //{
            //    return ApiResult.BadRequest("Lịch báo giảng tuần này đã được tạo");
            //}
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
            lgbdb.Id_tkb = lgb.Id_tkb;
            lgbdb.Id_nam_hoc = lgb.Id_nam_hoc;
            //sửa
            var (result, mess) = _lgb.Update(lgbdb, idDonvi);
            if (!result)
                return ApiResult.NotFound(mess);
            //nếu thay đổi tkb thì insert lại pbg
            bool checkchangetkb = _lgb.CheckChangeTKB(lgbdb);
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
        [HttpGet("phieu/export")]
        [RequireToken]
        public IActionResult Export_TungPhieu([FromQuery] int idpbg)
        {
            try
            {
                //bool check_env = _claimHelperRepository.IsDemoSite();
                //if (check_env)
                //    return ApiResult.NotFound("Bạn cần đăng ký dùng bản chính thức để sử dụng chức năng này");
                int idDonvi = _claimHelperRepository.GetIdDonvi(User);
                var templatePath = Path.Combine(_env.ContentRootPath, "Templates", "template.docx");
                var data = _export.GetList_Chitiet(idpbg, idDonvi);
                var excelBytes = _export.FillTemplate( templatePath , data);

                if (excelBytes == null)
                    return NotFound("Không có dữ liệu thời khóa biểu");

                var fileName = $"{data.Ten_giao_vien}_Tuan {data.Tuan}(Từ ngày {data.Tu_ngay.ToString("dd/MM/yyyy")} - đến ngày {data.Den_ngay.ToString("dd/MM/yyyy")}).docx";
                //header
                //Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"; filename*=UTF-8''{Uri.EscapeDataString(fileName)}");
                Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition, Content-Length");
                return File(excelBytes,
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
        [HttpGet("export")]
        [RequireToken]
        public IActionResult Export_TatCaPhieu([FromQuery] int idlbg)
        {
            try
            {
                //bool check_env = _claimHelperRepository.IsDemoSite();
                //if (check_env)
                //    return ApiResult.NotFound("Bạn cần đăng ký dùng bản chính thức để sử dụng chức năng này");
                int idDonvi = _claimHelperRepository.GetIdDonvi(User);
                var lgbdb = _lgb.GetDetailById(idlbg);
                if (lgbdb == null)
                    return ApiResult.NotFound($"Bản ghi có Id= {idlbg} không tồn tại, vui lòng kiểm tra lại");
                var templatePath = Path.Combine(_env.ContentRootPath, "Templates", "template.docx");
                var zipBytes = _export.FillMultipleAndZip( templatePath , idlbg, idDonvi);

                var fileName = $"PhieuBaoGiang_Tuần {lgbdb.Tuan}(Từ ngày {lgbdb.Tu_ngay.ToString("dd/MM/yyyy")} - đến ngày {lgbdb.Den_ngay.ToString("dd/MM/yyyy")}).zip";
                //header
                //Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"; filename*=UTF-8''{Uri.EscapeDataString(fileName)}");
                Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition, Content-Length");
                return File(zipBytes, "application/zip", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
    }
}
