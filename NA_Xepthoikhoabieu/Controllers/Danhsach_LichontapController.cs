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
    [Route("api/lichontap")]
    public class Danhsach_LichontapController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDanhsach_LichontapRepository _lich;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_GiaovienRepository _giaovien;
        private readonly IValidateRepository _validate;

        public Danhsach_LichontapController(IMapper mapper, IDanhsach_LichontapRepository lich, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                     IDM_GiaovienRepository giaovien, IValidateRepository validate)
        {
            _mapper = mapper;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _lich = lich;
            _giaovien = giaovien;
            _validate = validate;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            int totalrecord = 0;
            var list = _lich.GetList_Paging(PageIndex, PageSize, search, idDonvi, ref totalrecord);
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
            if (Id <= 0)
                return ApiResult.BadRequest($"Id {Id} không hợp lệ, vui lòng kiểm tra lại");
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var detail = _lich.GetDetailById(Id, idDonvi);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpGet("chitiet_lich")]
        [RequireToken]
        public IActionResult GetDetaillich([FromQuery] int Id)
        {
            if (Id <= 0)
                return ApiResult.BadRequest($"Id {Id} không hợp lệ, vui lòng kiểm tra lại");
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var detail = _lich.GetDetailLich(Id);
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
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            bool huy = _lich.Huy_KQ(Id);
            if (!huy)
                return ApiResult.NotFound($"Huỷ xếp không thành công");

            return ApiResult.Success("Huỷ xếp thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] Danhsach_Lichontap dslich)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            dslich.Id = 0;
            dslich.Id_don_vi = idDonvi;
            //check trùng tên
            bool check = _validate.CheckTrungTen_byDonvi<Danhsach_Lichontap>(idDonvi, dslich.Ten);
            if (check)
            {
                return ApiResult.BadRequest("Tên lịch ôn tập đã tồn tại");
            }
            bool add = _lich.Add(dslich);
            bool adddetail = _lich.AddChiTiet(dslich.Id, idDonvi);
            if (dslich.Trang_thai)
            {
                bool changestatus = _lich.SetStatus(dslich.Id, idDonvi);
                if (!changestatus)
                    return ApiResult.BadRequest("Thêm lịch ôn tập thành công, đổi trạng thái thất bại");
            }

            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            if (!adddetail)
                return ApiResult.BadRequest("Thêm lịch ôn tập thành công, thêm chi tiết lịch ôn tập thất bại");

            return ApiResult.Success(new
            {
                item = dslich
            },
            "Thêm mới thành công");

        }
        
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] Danhsach_Lichontap dslich)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var lichdb = _lich.CheckId(dslich.Id, idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (!lichdb)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            dslich.Id_don_vi = idDonvi;
            //check trùng tên
            bool check = _validate.CheckTrungTen_byDonvi<Danhsach_Lichontap>(idDonvi, dslich.Ten, dslich.Id);
            if (check)
            {
                return ApiResult.BadRequest("Tên lịch ôn tập đã tồn tại");
            }
            bool add = _lich.Update(dslich);
            if (dslich.Trang_thai)
            {
                bool changestatus = _lich.SetStatus(dslich.Id, idDonvi);
                if (!changestatus)
                    return ApiResult.BadRequest("Thêm lịch ôn tập thành công, đổi trạng thái thất bại");
            }
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(new
            {
                item = dslich
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
            var lichdb = _lich.CheckId(id, idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (!lichdb)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            //bool check_contraint = _lich.Check_constraint(id);
            //if (check_contraint)
            //{
            //    return ApiResult.BadRequest("lịch ôn tập đã có ràng buộc, không thể xoá");
            //}
            var detail = _lich.DeleteDetail(id);
            if (!detail)
                return ApiResult.NotFound("Xóa chi tiết lịch ôn tập thất bại");
            var request = _lich.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
