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
    [Route("api/lichthi")]
    [ApiController]
    public class DM_LichthiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_LichthiRepository _lichthi;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_DonviRepository _donvi;
        private readonly IDM_MonthiRepository _monthi;
        private readonly IDM_DiemthiRepository _diemthi;
        private readonly IDM_HoidongthiRepository _hoidong;
        public DM_LichthiController(IMapper mapper, IDM_LichthiRepository lichthi, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                       IDM_HoidongthiRepository hoidong, IDM_DonviRepository donvi, IDM_MonthiRepository monthi, IDM_DiemthiRepository diemthi)
        {
            _mapper = mapper;
            _lichthi = lichthi;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _donvi = donvi;
            _monthi = monthi;
            _diemthi = diemthi;
            _hoidong = hoidong;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "",
                                            [FromQuery] int idDiemThi = 0, [FromQuery] int idHoiDong = 0)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            bool checkDiemthi = _diemthi.CheckId(idDiemThi, idDonvi);
            if (!checkDiemthi && idDiemThi > 0)
                return ApiResult.BadRequest("Id điểm thi không hợp lệ");

            bool checkHoiDong = _hoidong.CheckId(idHoiDong, idDonvi);
            if (!checkHoiDong && idHoiDong > 0)
                return ApiResult.BadRequest("Id hội đồng không hợp lệ");

            var list = _lichthi.GetList_Paging(PageIndex, PageSize, search, idDonvi, idDiemThi, idHoiDong);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<DM_Lichthi_ListDto>>(list);
            int totalrecord = list.First().Total;
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
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            // Lấy bản ghi từ db
            var detaillichthi = _lichthi.GetDetailById(Id, idDonvi);
            if (detaillichthi == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");

            return ApiResult.Success(detaillichthi, "Thành công");
        }

        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_Lichthi data)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if (data.Id_diem_thi == 0 || data.Id_diem_thi == null)
            {
                return ApiResult.BadRequest("Điểm thi không được để trống");
            }
            bool checkDiemthi = _diemthi.CheckId(data.Id_diem_thi, idDonvi);
            if (!checkDiemthi)
            {
                return ApiResult.BadRequest("Id điểm thi không hợp lệ, vui lòng kiểm tra lại");
            }
            if(data.Bai_thi_tu_chon == false && data.Id_mon == null)
            {
                return ApiResult.BadRequest("Vui lòng chọn môn hoặc đánh dấu bài thi là môn tự chọn");
            }
            bool checkMonTuChon = _monthi.CheckIdTuChon(data.Id_mon, idDonvi);
            bool checkMon = _monthi.CheckId(data.Id_mon, idDonvi);
            if (data.Id_mon != null && (!checkMon || checkMonTuChon))
            {
                return ApiResult.BadRequest("Id môn thi không hợp lệ, vui lòng kiểm tra lại");
            }
            var item = _mapper.Map<DM_Lichthi>(data);
            item.Id = 0;

            // add 
            bool add = _lichthi.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");

            return ApiResult.Success(new
            {
                item = data
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_Lichthi lichthi)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            var item = _mapper.Map<DM_Lichthi>(lichthi);
            var lichthidb = _lichthi.GetDetailById(lichthi.Id, idDonvi);
            if (lichthidb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            if (lichthi.Id_diem_thi == 0 || lichthi.Id_diem_thi == null)
            {
                return ApiResult.BadRequest("Điểm thi không được để trống");
            }
            bool checkDiemthi = _diemthi.CheckId(lichthi.Id_diem_thi, idDonvi);
            if (!checkDiemthi)
            {
                return ApiResult.BadRequest("Id điểm thi không hợp lệ, vui lòng kiểm tra lại");
            }
            if (lichthi.Bai_thi_tu_chon == false && lichthi.Id_mon == null)
            {
                return ApiResult.BadRequest("Vui lòng chọn môn hoặc đánh dấu bài thi là môn tự chọn");
            }
            bool checkMonTuChon = _monthi.CheckIdTuChon(lichthi.Id_mon, idDonvi);
            bool checkMon = _monthi.CheckId(lichthi.Id_mon, idDonvi);
            if (lichthi.Id_mon != null && (!checkMon || checkMonTuChon))
            {
                return ApiResult.BadRequest("Id môn thi không hợp lệ, vui lòng kiểm tra lại");
            }
            bool update = _lichthi.Update(item);
            if (!update)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(new
            {
                item = lichthi
            },
            "Cập nhật thành công");
        }
        [HttpDelete]
        [RequireToken]
        public IActionResult Delete([FromQuery] int id)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var lichthidb = _lichthi.GetDetailById(id, idDonvi);
            if (lichthidb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            //check ràng buộc
            //bool check = _lichthi.CheckContraint(id, idDonvi);
            //if (check)
            //{
            //    return ApiResult.BadRequest("Học sinh đã có ràng buộc, không thể xoá");
            //}
            bool deleteChiTiet = _lichthi.DeleteChiTiet(id);
            if (!deleteChiTiet)
                return ApiResult.BadRequest("Xoá chi tiết thất bại");
            bool request = _lichthi.Delete(id);
            if (!request)
                return ApiResult.BadRequest("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
