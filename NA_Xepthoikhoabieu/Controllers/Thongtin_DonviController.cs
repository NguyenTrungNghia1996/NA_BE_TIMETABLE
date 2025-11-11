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
    [Route("api/thongtin_donvi")]
    public class Thongtin_DonviController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IThongtin_DonviRepository _ttdonvi;
        private readonly IDM_DonviRepository _donvi;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_CaphocRepository _caphocRepository;
        private readonly IDM_NgayhocRepository _ngayhocRepository;
        private readonly IDM_CahocRepository _cahocRepository;
        public Thongtin_DonviController(IMapper mapper, IDM_DonviRepository donvi, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
            IDM_CaphocRepository caphocRepository, IDM_NgayhocRepository ngayhocRepository, IDM_CahocRepository cahocRepository, IThongtin_DonviRepository ttdonvi)
        {
            _mapper = mapper;
            _donvi = donvi;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _caphocRepository = caphocRepository;
            _ngayhocRepository = ngayhocRepository;
            _cahocRepository = cahocRepository;
            _ttdonvi = ttdonvi;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetDetailByID()
        {

            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy bản ghi từ db
            var detail = _donvi.getDetailById(idDonvi);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {idDonvi}");
            var detailDto = _mapper.Map<Thongtin_Donvi_updateDto>(detail);
            detailDto.IdCap = _donvi.GetlistCapbyDonvi(idDonvi);
            var listca = _ttdonvi.GetlistCabyDonvi(idDonvi);
            detailDto.List_ca = _mapper.Map<List<Ca_DonviDto>>(listca);
            return ApiResult.Success(detailDto, "Thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] Thongtin_Donvi_updateDto donvi)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var donvidb = _donvi.getDetailById(idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (donvidb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            //check chọn ca, cấp
            if (donvi.IdCap == null || donvi.IdCap.Count == 0)
            {
                ModelState.AddModelError("IdCap", "Vui lòng chọn ít nhất 1 cấp học");
            }
            if (donvi.List_ca == null || donvi.List_ca.Count == 0)
            {
                ModelState.AddModelError("Id_ca_hoc", "Vui lòng chọn ít nhất 1 ca học");
            }

            var item = _mapper.Map<DM_Donvi>(donvi);
            item.Id = idDonvi;
            var listca = _mapper.Map<List<Ca_Donvi>>(donvi.List_ca);
            List<int> sotiet = listca.Select(c => c.So_tiet).ToList();
            List<int> idsCa = listca.Select(c => c.Id_ca_hoc).ToList();
            //check id ca, cấp
            var checkcaphoc = _caphocRepository.CheckIds(donvi.IdCap);
            var checkcahoc = _cahocRepository.CheckIds(idsCa);

            if (!checkcaphoc)
                ModelState.AddModelError("IdCap", "Id cấp học không hợp lệ, vui lòng kiểm tra lại");
            if (!checkcahoc)
                ModelState.AddModelError("Id_ca_hoc", "Id ca học không hợp lệ, vui lòng kiểm tra lại");
            bool checkten = _donvi.CheckTrungTen(donvi.TenDonvi, idDonvi);
            if (checkten)
            {
                return ApiResult.BadRequest("Tên đơn vị đã tồn tại");
            }
            if (donvi.So_ngay > 7 || donvi.So_ngay < 0)
            {
                return ApiResult.BadRequest("Số ngày không hợp lệ");
            }
            for (int i = 0; i < sotiet.Count; i++)
            {
                if (sotiet[i] < 0 || sotiet[i] > 5)
                    return ApiResult.BadRequest("Số tiết không hợp lệ");
            }

            //hiển thị thông báo lỗi
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //update
            bool update = _donvi.Update(item);
            if (!update)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var editCap = _donvi.UpdateCap(idDonvi, donvi.IdCap);

            var editCa = _ttdonvi.UpdateCa(idDonvi, listca);
            if (!editCap)
                return ApiResult.Success(new
                {
                    item = donvi
                },
                "Cập nhật đơn vị thành công, cập nhật cấp học thất bại");
            if (!editCa)
                return ApiResult.Success(new
                {
                    item = donvi
                },
                "Cập nhật đơn vị thành công, cập nhật ca học thất bại");

            return ApiResult.Success(new
            {
                item = donvi
            }, "Cập nhật đơn vị thành công");
        }
    }
}