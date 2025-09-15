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

namespace NA_Xepthoikhoabieu.Controllers
{
    [ApiController]
    [Route("api/tietcodinh")]
    public class Tiet_co_dinhController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITiet_co_dinhRepository _tietcodinh;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_KhoilopRepository _khoilop;
        public Tiet_co_dinhController(IMapper mapper, ITiet_co_dinhRepository tietcodinh, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, IDM_KhoilopRepository khoilop)
        {
            _mapper = mapper;
            _tietcodinh = tietcodinh;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _khoilop = khoilop;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _tietcodinh.GetList_Paging(PageIndex, PageSize, idDonvi, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            var listDto = _mapper.Map<List<Tiet_co_dinh_ListDto>>(list);
            foreach (var dto in listDto)
            {
                dto.Ten_ngay = ((Ngay)dto.Ngay).GetDisplayName();
                dto.Ten_tiet = ((Tiet)dto.Tiet).GetDisplayName();
            }
            return ApiResult.Success(new
            {
                items = listDto,
                totalrecord = totalrecord
            },
            "Thành công");
        }
        [HttpGet("detail")]
        [RequireToken]
        public IActionResult GetDetailByID([FromQuery] int Id)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy bản ghi từ db
            var detail = _tietcodinh.GetDetailById(Id);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] Tiet_co_dinhDto tietcd)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            // kiểm tra khối lớp
            if (tietcd.Id_khoi_lop == 0 && tietcd.Ap_dung_cho_tat_ca_cac_khoi==false)
            {
                return ApiResult.BadRequest("Vui lòng chọn khối lớp hoặc áp dụng cho tất cả các khối");
            }

            var tietcdList = new List<Tiet_co_dinh>();
            //nếu chọn áp dụng cho tất cả các khối
            if (tietcd.Ap_dung_cho_tat_ca_cac_khoi == true)
            {
                var allKhoilop = _khoilop.GetKhoilopByDonvi(idDonvi);
                foreach (var khoilop in allKhoilop)
                {
                    bool isValid = _tietcodinh.CheckIds(tietcd.Id_mon, tietcd.Id_ngay, tietcd.Id_ca, tietcd.Id_tiet, khoilop.Id, idDonvi);
                    if (!isValid)
                    {
                        return ApiResult.BadRequest("Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các thông tin môn học, ngày, ca học, tiết học, khối lớp.");
                    }
                    tietcdList.Add(new Tiet_co_dinh
                    {
                        Id_mon = tietcd.Id_mon,
                        Ngay = tietcd.Id_ngay,
                        Id_ca = tietcd.Id_ca,
                        Tiet = tietcd.Id_tiet,
                        Id_khoi_lop = khoilop.Id,

                    });
                }
            }
            //nếu không chọn áp dụng cho tất cả
            else
            {
                bool isValid = _tietcodinh.CheckIds(tietcd.Id_mon, tietcd.Id_ngay, tietcd.Id_ca, tietcd.Id_tiet, tietcd.Id_khoi_lop, idDonvi);
                if (!isValid)
                {
                    return ApiResult.BadRequest("Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các thông tin môn học, ngày, ca học, tiết học, khối lớp.");
                }
                tietcdList.Add(new Tiet_co_dinh
                {
                    Id_mon = tietcd.Id_mon,
                    Ngay = tietcd.Id_ngay,
                    Id_ca = tietcd.Id_ca,
                    Tiet = tietcd.Id_tiet,
                    Id_khoi_lop = tietcd.Id_khoi_lop,
                });
            }
            //check trùng
            bool check = _tietcodinh.CheckTrung(tietcdList);
            if (check)
            {
                return ApiResult.BadRequest("Tiết cố định đã tồn tại");
            }
            //thêm
            bool add = _tietcodinh.Add(tietcdList);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");

            return ApiResult.Success(new
            {
                itemSave = tietcd,
            }, "Thêm tiết cố định thành công");

        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] Tiet_co_dinhDto tietcd)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var tietcddb = _tietcodinh.GetDetailById(tietcd.Id);
            //if (!ModelState.IsValid)
            //    return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (tietcddb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            //kiểm tra khối lớp
            if (tietcd.Id_khoi_lop == 0 && tietcd.Ap_dung_cho_tat_ca_cac_khoi == false)
            {
                return ApiResult.BadRequest("Vui lòng chọn khối lớp hoặc áp dụng cho tất cả các khối");
            }

            var tietcdList = new List<Tiet_co_dinh>();
            bool update = false;
            //nếu áp dụng cho tất cả các khối
            if (tietcd.Ap_dung_cho_tat_ca_cac_khoi == true)
            {
                var allKhoilop = _khoilop.GetKhoilopByDonvi(idDonvi);
                foreach (var khoilop in allKhoilop)
                {
                    bool isValid = _tietcodinh.CheckIds(tietcd.Id_mon, tietcd.Id_ngay, tietcd.Id_ca, tietcd.Id_tiet, khoilop.Id, idDonvi);
                    if (!isValid)
                    {
                        return ApiResult.BadRequest("Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các thông tin môn học, ngày, ca học, tiết học, khối lớp.");
                    }
                    tietcdList.Add(new Tiet_co_dinh
                    {
                        Id_mon = tietcd.Id_mon,
                        Ngay = tietcd.Id_ngay,
                        Id_ca = tietcd.Id_ca,
                        Tiet = tietcd.Id_tiet,
                        Id_khoi_lop = khoilop.Id,

                    });
                }
                update = _tietcodinh.UpdateAllKhoi(tietcdList, tietcd.Id_mon);
            }
            //nếu chỉ cho 1 khối
            else
            {
                bool isValid = _tietcodinh.CheckIds(tietcd.Id_mon, tietcd.Id_ngay, tietcd.Id_ca, tietcd.Id_tiet, tietcd.Id_khoi_lop, idDonvi);
                if (!isValid)
                {
                    return ApiResult.BadRequest("Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các thông tin môn học, ngày, ca học, tiết học, khối lớp.");
                }
                var tietcd_update = new Tiet_co_dinh
                {
                    Id = tietcd.Id,
                    Id_mon = tietcd.Id_mon,
                    Ngay = tietcd.Id_ngay,
                    Id_ca = tietcd.Id_ca,
                    Tiet = tietcd.Id_tiet,
                    Id_khoi_lop = tietcd.Id_khoi_lop,
                };
                update = _tietcodinh.Update(tietcd_update);
            }
            //check trùng
            bool check = _tietcodinh.CheckTrung(tietcdList);
            if (check)
            {
                return ApiResult.BadRequest("Tiết cố định đã tồn tại");
            }
            if (!update)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");

            return ApiResult.Success(new
            {
                itemSave = tietcd,
            }, "Cập nhật tiết cố định thành công");
        }
        [HttpDelete]
        [RequireToken]
        public IActionResult Delete([FromQuery] int id)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var item = _tietcodinh.GetDetailById(id);
            if (item == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var request = _tietcodinh.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");

            return ApiResult.Ok("Xóa tiết học thành công");
        }
    }
}