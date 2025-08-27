using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/export")]
    [ApiController]
    public class ExportExcelController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IExportExcelRepository _export;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        public ExportExcelController(IMapper mapper, IExportExcelRepository export, IClaimHelperRepository claimHelperRepository, IAuthRepository auth)
        {
            _mapper = mapper;
            _export = export;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
        }
        [HttpGet("lop")]
        [RequireToken]
        public IActionResult ExportTKB_Lop(int idtkb)
        {
            try
            {
                return BadRequest("Bạn cần đăng ký dùng bản chính thức để sử dụng chức năng này");
                //var excelBytes = _export.ExportExcel_Class(idtkb);

                //if (excelBytes == null)
                //    return NotFound("Không có dữ liệu thời khóa biểu");

                //var fileName = $"ThoiKhoaBieu_Lop_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                ////header
                //Response.Headers.Append("Content-Disposition", $"attachment; filename={fileName}; filename*=UTF-8''{Uri.EscapeDataString(fileName)}");
                //Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition, Content-Length");

                //return File(excelBytes,
                //    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                //    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
        [HttpGet("giaovien")]
        [RequireToken]
        public IActionResult ExportTKB_GV(int idtkb)
        {
            try
            {
                return BadRequest("Bạn cần đăng ký dùng bản chính thức để sử dụng chức năng này");
                //var excelBytes = _export.ExportExcel_Teacher(idtkb);

                //if (excelBytes == null)
                //    return NotFound("Không có dữ liệu thời khóa biểu");

                //var fileName = $"ThoiKhoaBieu_Giaovien_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                ////header
                //Response.Headers.Append("Content-Disposition", $"attachment; filename={fileName}; filename*=UTF-8''{Uri.EscapeDataString(fileName)}");
                //Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition, Content-Length");

                //return File(excelBytes,
                //    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                //    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
        [HttpGet]
        [RequireToken]
        public IActionResult ExportTKB(int idtkb)
        {
            try
            {
                return BadRequest("Bạn cần đăng ký dùng bản chính thức để sử dụng chức năng này");

                //var excelBytes = _export.ExportExcel_TKB(idtkb);

                //if (excelBytes == null)
                //    return NotFound("Không có dữ liệu thời khóa biểu");

                //var fileName = $"ThoiKhoaBieu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                ////header
                //Response.Headers.Append("Content-Disposition",$"attachment; filename={fileName}; filename*=UTF-8''{Uri.EscapeDataString(fileName)}");
                //Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition, Content-Length");
                //return File(excelBytes,
                //    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                //    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
    }
}
