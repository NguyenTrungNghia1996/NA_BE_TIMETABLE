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
using NA_Logic.IRepository.Auth;
using NA_Logic.IRepository.XepThoiKhoaBieu;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;
using NetTopologySuite.Index.HPRtree;

namespace NA_Xepthoikhoabieu.Controllers.ThoiKhoaBieu
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
        private readonly IValidateRepository _validate;

        public Danhsach_ThoikhoabieuController(IMapper mapper, IDanhsach_ThoikhoabieuRepository tkb, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                     IDM_GiaovienRepository giaovien, IValidateRepository validate)
        {
            _mapper = mapper;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _tkb = tkb;
            _giaovien = giaovien;
            _validate = validate;
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
                totalrecord
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
        [HttpPut("huykq")]
        [RequireToken]
        public IActionResult Huy_KQ([FromQuery] int Id)
        {
            if (Id <= 0)
                return ApiResult.BadRequest($"Id {Id} không hợp lệ, vui lòng kiểm tra lại");
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy bản ghi từ db
            bool huy = _tkb.Huy_KQ(Id);
            if (!huy)
                return ApiResult.NotFound($"Huỷ xếp không thành công");

            return ApiResult.Success("Huỷ xếp thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] Danhsach_Thoikhoabieu dstkb)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // mapper data 
            //var dstkb = _mapper.Map<Danhsach_Thoikhoabieu>(dstkbDto);
            dstkb.Id = 0;
            dstkb.Id_don_vi = idDonvi;
            //check trùng tên
            bool check = _validate.CheckTrungTen_byDonvi<Danhsach_Thoikhoabieu>(idDonvi, dstkb.Ten);
            if (check)
            {
                return ApiResult.BadRequest("Tên thời khoá biểu đã tồn tại");
            }
            //thêm
            bool add = _tkb.Add(dstkb);
            bool adddetail = _tkb.AddChitiet_Tkb(dstkb.Id, idDonvi);
            if (dstkb.Dang_su_dung)
            {
                bool changestatus = _tkb.SetStatus(dstkb.Id, idDonvi);
                if (!changestatus)
                    return ApiResult.BadRequest("Thêm thời khoá biểu thành công, đổi trạng thái thất bại");
            }
            
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            if (!adddetail)
                return ApiResult.BadRequest("Thêm thời khoá biểu thành công, thêm chi tiết thời khoá biểu thất bại");
            
            // mapper data trả về view
            //var itemDto = _mapper.Map<DM_PhonghocDto>(addph);
            return ApiResult.Success(new
            {
                item = dstkb
            },
            "Thêm mới thành công");

        }
        [HttpPost("sync")]
        [RequireToken]
        public IActionResult Sync(int idtkb)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            bool check_tkb = _tkb.CheckId(idtkb, idDonvi);
            if (!check_tkb)
                return ApiResult.BadRequest($"Id thời khoá biểu = {idtkb} không hợp lệ, vui lòng kiểm tra lại");
            //thêm
            bool add = _tkb.Sync_Tkb(idtkb,idDonvi);
            if (!add)
                return ApiResult.NotFound("Đồng bộ thất bại");
            return ApiResult.Success("Đồng bộ thành công");

        }
        [HttpPost("copy")]
        [RequireToken]
        public IActionResult Copy([FromBody] Danhsach_Thoikhoabieu dstkb)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // mapper data 
            var tkb = new Danhsach_Thoikhoabieu()
            {
                Id = 0,
                Ten = dstkb.Ten,
                Dang_su_dung = dstkb.Dang_su_dung,
                Trang_thai_xep = false,
                Id_don_vi = idDonvi
            };
            //check trùng tên
            bool check = _validate.CheckTrungTen_byDonvi<Danhsach_Thoikhoabieu>(idDonvi, tkb.Ten);
            if (check)
            {
                return ApiResult.BadRequest("Tên thời khoá biểu đã tồn tại");
            }
            //thêm
            bool add = _tkb.Add(tkb);
            bool adddetail = _tkb.AddChitiet_Tkb(tkb.Id, idDonvi);
            bool copytkb = false;
            if (dstkb.Dang_su_dung)
            {
                bool changestatus = _tkb.SetStatus(tkb.Id, idDonvi);
                if (!changestatus)
                    return ApiResult.BadRequest("Thêm thời khoá biểu thành công, đổi trạng thái thất bại");
            }
            if (dstkb.Id > 0)
            {
                bool check_tkb = _tkb.CheckId(dstkb.Id, idDonvi);
                if (!check_tkb)
                    return ApiResult.BadRequest($"Id thời khoá biểu = {dstkb.Id} không hợp lệ, vui lòng kiểm tra lại");
                copytkb = _tkb.Copy_Tkb(dstkb.Id, tkb.Id);
            }
            

            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            if (!adddetail)
                return ApiResult.BadRequest("Thêm danh sách thời khoá biểu thành công, thêm chi tiết thời khoá biểu thất bại");
            if (!copytkb)
                return ApiResult.BadRequest("Thêm danh sách thời khoá biểu thành công, thêm chi tiết thời khoá biểu thành công, sao chép thất bại");
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
            var tkbdb = _tkb.CheckId(dstkb.Id, idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (!tkbdb)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            dstkb.Id_don_vi = idDonvi;
            //check trùng tên
            bool check = _validate.CheckTrungTen_byDonvi<Danhsach_Thoikhoabieu>(idDonvi, dstkb.Ten, dstkb.Id);
            if (check)
            {
                return ApiResult.BadRequest("Tên thời khoá biểu đã tồn tại");
            }
            //update
            bool add = _tkb.Update(dstkb);
            if (dstkb.Dang_su_dung)
            {
                bool changestatus = _tkb.SetStatus(dstkb.Id, idDonvi);
                if (!changestatus)
                    return ApiResult.BadRequest("Thêm thời khoá biểu thành công, đổi trạng thái thất bại");
            }

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
            var tkbdb = _tkb.CheckId(id, idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (!tkbdb)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            bool check_contraint = _tkb.Check_constraint(id);
            if (check_contraint)
            {
                return ApiResult.BadRequest("Thời khoá biểu đã có ràng buộc, không thể xoá");
            }
            var detail = _tkb.DeleteDetail(id);
            if (!detail)
                return ApiResult.NotFound("Xóa chi tiết thời khoá biểu thất bại");
            var request = _tkb.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
