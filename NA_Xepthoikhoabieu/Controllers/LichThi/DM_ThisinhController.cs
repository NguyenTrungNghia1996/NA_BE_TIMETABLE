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
    [Route("api/thisinh")]
    [ApiController]
    public class DM_ThisinhController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_ThisinhRepository _thisinh;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IDM_DiemthiRepository _diemthi;
        private readonly IDM_HoidongthiRepository _hoidong;
        private readonly IDM_DantocRepository _dantoc;
        private readonly IDM_DonviHanhchinhRepository _hanhchinh;
        private readonly IDM_MonthiRepository _monthi;
        public DM_ThisinhController(IMapper mapper, IDM_ThisinhRepository thisinh, IClaimHelperRepository claimHelperRepository, IDM_MonthiRepository monthi,
                                       IDM_HoidongthiRepository hoidong, IDM_DiemthiRepository diemthi, IDM_DonviHanhchinhRepository hanhchinh,
                                       IDM_DantocRepository dantoc)
        {
            _mapper = mapper;
            _thisinh = thisinh;
            _claimHelperRepository = claimHelperRepository;
            _diemthi = diemthi;
            _hoidong = hoidong;
            _hanhchinh = hanhchinh;
            _dantoc = dantoc;
            _monthi = monthi;
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

            var list = _thisinh.GetList_Paging(PageIndex, PageSize, search, idDonvi, idDiemThi, idHoiDong);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<DM_Thisinh_ListDto>>(list);
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
            var detailthisinh = _thisinh.GetDetailById(Id, idDonvi);
            if (detailthisinh == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");

            return ApiResult.Success(detailthisinh, "Thành công");
        }

        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_ThisinhDto data)
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
            bool checkNoisinh = _hanhchinh.CheckId(data.Noi_sinh_xa);
            if (!checkNoisinh)
            {
                return ApiResult.BadRequest("Id xã không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkThuongtru = _hanhchinh.CheckId(data.Thuong_tru_xa);
            if (!checkThuongtru)
            {
                return ApiResult.BadRequest("Id xã không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkDantoc = _dantoc.CheckId(data.Dan_toc);
            if (!checkDiemthi)
            {
                return ApiResult.BadRequest("Id dân tộc không hợp lệ, vui lòng kiểm tra lại");
            }
            if (data.Mon_thi_1 == null && data.Mon_thi_2 == null)
                return ApiResult.BadRequest("Vui lòng chọn ít nhất 1 môn thi");
            bool checkMon1 = _monthi.CheckId(data.Mon_thi_1, idDonvi);
            if (!checkMon1)
            {
                return ApiResult.BadRequest("Id môn thi 1 không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkMon2 = _monthi.CheckId(data.Mon_thi_2, idDonvi);
            if (!checkMon2)
            {
                return ApiResult.BadRequest("Id môn thi 2 không hợp lệ, vui lòng kiểm tra lại");
            }
            var item = _mapper.Map<DM_Thisinh>(data);
            item.Id = 0;

            // add 
            bool add = _thisinh.Add(item);
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
        public IActionResult Update([FromBody] DM_ThisinhDto thisinh)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            var item = _mapper.Map<DM_Thisinh>(thisinh);
            var thisinhdb = _thisinh.GetDetailById(thisinh.Id, idDonvi);
            if (thisinhdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            if (thisinh.Id_diem_thi == 0 || thisinh.Id_diem_thi == null)
            {
                return ApiResult.BadRequest("Điểm thi không được để trống");
            }
            bool checkDiemthi = _diemthi.CheckId(thisinh.Id_diem_thi, idDonvi);
            if (!checkDiemthi)
            {
                return ApiResult.BadRequest("Id điểm thi không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkNoisinh = _hanhchinh.CheckId(thisinh.Noi_sinh_xa);
            if (!checkNoisinh)
            {
                return ApiResult.BadRequest("Id xã không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkThuongtru = _hanhchinh.CheckId(thisinh.Thuong_tru_xa);
            if (!checkThuongtru)
            {
                return ApiResult.BadRequest("Id xã không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkDantoc = _dantoc.CheckId(thisinh.Dan_toc);
            if (!checkDiemthi)
            {
                return ApiResult.BadRequest("Id dân tộc không hợp lệ, vui lòng kiểm tra lại");
            }
            if (thisinh.Mon_thi_1 == null && thisinh.Mon_thi_2 == null)
                return ApiResult.BadRequest("Vui lòng chọn ít nhất 1 môn thi");
            bool checkMon1 = _monthi.CheckId(thisinh.Mon_thi_1, idDonvi);
            if (!checkMon1 && thisinh.Mon_thi_1 != null)
            {
                return ApiResult.BadRequest("Id môn thi 1 không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkMon2 = _monthi.CheckId(thisinh.Mon_thi_2, idDonvi);
            if (!checkMon2 && thisinh.Mon_thi_2 != null)
            {
                return ApiResult.BadRequest("Id môn thi 2 không hợp lệ, vui lòng kiểm tra lại");
            }
            item.So_bao_danh = thisinhdb.So_bao_danh;
            bool update = _thisinh.Update(item);
            if (!update)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(new
            {
                item = thisinh
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

            var thisinhdb = _thisinh.GetDetailById(id, idDonvi);
            if (thisinhdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            //check ràng buộc
            //bool check = _thisinh.CheckContraint(id, idDonvi);
            //if (check)
            //{
            //    return ApiResult.BadRequest("Học sinh đã có ràng buộc, không thể xoá");
            //}

            bool request = _thisinh.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
        [HttpPost("import")]
        [RequireToken]
        public IActionResult Import(IFormFile file)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if (file == null || file.Length == 0)
                return ApiResult.BadRequest("Vui lòng chọn file");


            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
                return ApiResult.BadRequest("Chỉ chấp nhận file Excel (.xlsx, .xls)");
            var (result, mess, list) = (false, "", new List<ThiSinhCheck?>());
            using (var stream = file.OpenReadStream())
            {
                (result, mess, list) = _thisinh.Import(stream, idDonvi);
            }
            if (result)
            {
                return ApiResult.Success(new
                {
                    item = list
                },
            "Thành công");
            }
            else
            {
                return ApiResult.BadRequest(mess);
            }
        }
        [HttpPost("sbd")]
        [RequireToken]
        public IActionResult DanhSBD([FromQuery] int idHoiDong)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if (idHoiDong == 0 || idHoiDong == null)
            {
                return ApiResult.BadRequest("Hội đồng thi không được để trống");
            }
            bool checkHoiDong = _hoidong.CheckId(idHoiDong, idDonvi);
            if (!checkHoiDong)
            {
                return ApiResult.BadRequest("Id hội đồng không hợp lệ, vui lòng kiểm tra lại");
            }

            bool add = _thisinh.DanhSoBaoDanh(idHoiDong);
            if (!add)
                return ApiResult.NotFound("Thất bại, lưu dữ liệu không thành công");

            return ApiResult.Success("Thành công");
        }
    }
}
