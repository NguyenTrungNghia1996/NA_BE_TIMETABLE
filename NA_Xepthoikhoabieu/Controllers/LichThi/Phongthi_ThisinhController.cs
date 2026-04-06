using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.IRepository.Auth;
using NA_Logic.IRepository.LichOnTap;
using NA_Logic.IRepository.LichThi;
using NA_Logic.IRepository.XepGiamThi;
using NA_Logic.IRepository.XepThoiKhoaBieu;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/phongthi/thisinh")]
    [ApiController]
    public class Phongthi_ThisinhController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPhongthi_ThisinhRepository _pt;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IDM_DiemthiRepository _diemthi;
        private readonly IDM_HoidongthiRepository _hoidong;

        public Phongthi_ThisinhController(IMapper mapper, IPhongthi_ThisinhRepository pt, IClaimHelperRepository claimHelperRepository,
                                       IDM_HoidongthiRepository hoidong, IDM_DiemthiRepository diemthi)
        {
            _mapper = mapper;
            _pt = pt;
            _claimHelperRepository = claimHelperRepository;
            _diemthi = diemthi;
            _hoidong = hoidong;
        }

        [HttpPost]
        [RequireToken]
        public IActionResult XepPhongTheoHoiDong([FromQuery] int idHoiDong = 0, [FromQuery] int idDiemThi = 0)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if ((idHoiDong == 0 || idHoiDong == null) && (idDiemThi == 0 || idDiemThi == null))
            {
                return ApiResult.BadRequest("Vui lòng chọn hội đồng hoặc điểm thi");
            }
            bool checkHoiDong = _hoidong.CheckId(idHoiDong, idDonvi);
            if (!checkHoiDong && idHoiDong != 0 )
            {
                return ApiResult.BadRequest("Id hội đồng không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkDiemthi = _diemthi.CheckId(idDiemThi, idDonvi);
            if (!checkDiemthi && idDiemThi != 0)
            {
                return ApiResult.BadRequest("Id điểm thi không hợp lệ, vui lòng kiểm tra lại");
            }
            bool add = false;
            if (idHoiDong != 0 && idHoiDong != null)
                add = _pt.XepPhongTheoHoiDong(idHoiDong);
            
            else if (idDiemThi != 0 && idDiemThi != null)
                add = _pt.XepPhongTheoDiemThi(idDiemThi);
            if (!add)
                return ApiResult.NotFound("Thất bại");

            return ApiResult.Success("Thành công");
        }
        
        
    }
}
