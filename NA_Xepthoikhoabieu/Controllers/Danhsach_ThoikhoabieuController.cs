using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;
using NetTopologySuite.Index.HPRtree;

namespace NA_Xepthoikhoabieu.Controllers
{
    [ApiController]
    [Route("api/thoikhoabieu")]
    public class Danhsach_ThoikhoabieuController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDanhsach_ThoikhoabieuRepository _tkb;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_GiaovienRepository _giaovien;

        public Danhsach_ThoikhoabieuController(IMapper mapper, IDanhsach_ThoikhoabieuRepository tkb, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                     IDM_GiaovienRepository giaovien)
        {
            _mapper = mapper;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _tkb = tkb;
            _giaovien = giaovien;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _tkb.GetList_Paging(PageIndex, PageSize, search, idDonvi, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
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
            if (Id <= 0)
                return ApiResult.BadRequest($"Id {Id} không hợp lệ, vui lòng kiểm tra lại");
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy bản ghi từ db
            var detail = _tkb.GetDetailById(Id, idDonvi);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpGet("chitiet_tkb")]
        [RequireToken]
        public IActionResult GetDetailTKB([FromQuery] int Id)
        {
            if (Id <= 0)
                return ApiResult.BadRequest($"Id {Id} không hợp lệ, vui lòng kiểm tra lại");
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy bản ghi từ db
            var detail = _tkb.GetDetailTKB(Id);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");

            return ApiResult.Success(detail, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] Danhsach_Thoikhoabieu dstkb)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // mapper data 
            //var addph = _mapper.Map<DM_Phonghoc>(phonghoc);
            dstkb.Id = 0;
            dstkb.Id_don_vi = idDonvi;
            //thêm
            bool add = _tkb.Add(dstkb);
            bool adddetail = _tkb.AddChitiet_tkb(dstkb.Id);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            if (!adddetail)
                return ApiResult.BadRequest("Thêm danh sách thời khoá biểu thành công, thêm chi tiết thời khoá biểu thất bại");
            // mapper data trả về view
            //var itemDto = _mapper.Map<DM_PhonghocDto>(addph);
            return ApiResult.Success(new
            {
                item = dstkb
            },
            "Thêm mới thành công");

        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] Danhsach_Thoikhoabieu dstkb)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var tkbdb = _tkb.GetDetailById(dstkb.Id, idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (tkbdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            dstkb.Id_don_vi = idDonvi;
            //update
            bool add = _tkb.Update(dstkb);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(new
            {
                item = dstkb
            },
            "Cập nhật thành công");

        }
        [HttpDelete]
        [RequireToken]
        public IActionResult Delete([FromQuery] int id)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var tkbdb = _tkb.GetDetailById(id, idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (tkbdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            var detail = _tkb.DeleteDetail(id);
            if (!detail)
                return ApiResult.NotFound("Xóa chi tiết thời khoá biểu thất bại");
            var request = _tkb.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
        [HttpGet("object_gv")]
        [RequireToken]
        public IActionResult Get_Oject_giaovien([FromQuery] int Idgv, [FromQuery] int Idtkb)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var check_tkb = _tkb.CheckId(Idtkb, idDonvi);
            var check_gv = _giaovien.CheckId(Idgv, idDonvi);
            if (Idgv <= 0 || !check_gv)
                return ApiResult.BadRequest($"Id giáo viên =  {Idgv} không hợp lệ, vui lòng kiểm tra lại");
            if (Idtkb <= 0 || !check_tkb)
                return ApiResult.BadRequest($"Id thời khoá biểu =  {Idgv} không hợp lệ, vui lòng kiểm tra lại");
            // Lấy bản ghi từ db
            var detail = _tkb.Object_giaovien(Idgv, Idtkb);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id giáo viên = {Idgv} và Id thời khoá biểu = {Idtkb}");
            return ApiResult.Success(detail, "Thành công");
        }
    }
}
