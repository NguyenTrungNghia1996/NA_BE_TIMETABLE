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
    [Route("api/ngaybydonvi")]
    public class Ngay_DonviController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly INgay_DonviRepository _ngaydv;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_CaphocRepository _caphocRepository;
        private readonly IDM_NgayhocRepository _ngayhocRepository;
        public Ngay_DonviController(IMapper mapper, INgay_DonviRepository ngaydv, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, IDM_CaphocRepository caphocRepository, IDM_NgayhocRepository ngayhocRepository)
        {
            _mapper = mapper;
            _ngaydv = ngaydv;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _caphocRepository = caphocRepository;
            _ngayhocRepository = ngayhocRepository;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetNgayByID()
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy bản ghi từ db
            var detail = _ngaydv.GetlistNgaybyDonvi(idDonvi);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {idDonvi}");
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult addNgay([FromBody] Donvi_NgayDto ngayDv)
        {
            //kiểm tra id đơn vị
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if (ngayDv.Id_Ngay == null || ngayDv.Id_Ngay.Count == 0)
            {
                ModelState.AddModelError("Id_ngay", "Vui lòng chọn ít nhất 1 ngày học");
            }
            //kiểm tra id ngày
            var checkngayhoc = _ngayhocRepository.CheckIds(ngayDv.Id_Ngay);
            if (!checkngayhoc)
                ModelState.AddModelError("Id_ngay", "Id ngày học không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //add ngày
            var addNgayDonvi = _ngaydv.UpdateNgay(idDonvi, ngayDv.Id_Ngay);

            if (!addNgayDonvi)
                return ApiResult.Success(new
                {
                    item = ngayDv
                },
                "Cập nhật ngày cho đơn vị thất bại");
            return ApiResult.Success(
             "Cập nhật ngày cho đơn vị thành công");
        }
    }
}