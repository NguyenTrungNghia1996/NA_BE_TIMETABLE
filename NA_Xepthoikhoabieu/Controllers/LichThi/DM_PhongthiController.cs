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
    [Route("api/phongthi")]
    [ApiController]
    public class DM_PhongthiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_PhongthiRepository _phongthi;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_DonviRepository _donvi;
        private readonly IDM_GiaovienRepository _giaovien;
        private readonly IDM_DiemthiRepository _diemthi;
        private readonly IDM_HoidongthiRepository _hoidong;
        public DM_PhongthiController(IMapper mapper, IDM_PhongthiRepository phongthi, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                       IDM_HoidongthiRepository hoidong, IDM_DonviRepository donvi, IDM_GiaovienRepository giaovien, IDM_DiemthiRepository diemthi)
        {
            _mapper = mapper;
            _phongthi = phongthi;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _donvi = donvi;
            _giaovien = giaovien;
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

            var list = _phongthi.GetList_Paging(PageIndex, PageSize, search, idDonvi, idDiemThi, idHoiDong);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<DM_Phongthi_ListDto>>(list);
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
            var detailphongthi = _phongthi.GetDetailById(Id, idDonvi);
            if (detailphongthi == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");

            return ApiResult.Success(detailphongthi, "Thành công");
        }

        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_PhongthiDto data)
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
            var diemThi = _diemthi.GetDetailById(data.Id_diem_thi, idDonvi);
            var item = _mapper.Map<DM_Phongthi>(data);
            item.Id = 0;

            // add 
            bool add = _phongthi.Add(item);
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
        public IActionResult Update([FromBody] DM_PhongthiDto phongthi)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            var item = _mapper.Map<DM_Phongthi>(phongthi);
            var phongthidb = _phongthi.GetDetailById(phongthi.Id, idDonvi);
            if (phongthidb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            if (phongthi.Id_diem_thi == 0 || phongthi.Id_diem_thi == null)
            {
                return ApiResult.BadRequest("Điểm thi không được để trống");
            }
            bool checkDiemthi = _diemthi.CheckId(phongthi.Id_diem_thi, idDonvi);
            if (!checkDiemthi)
            {
                return ApiResult.BadRequest("Id điểm thi không hợp lệ, vui lòng kiểm tra lại");
            }
            item.So_phong = phongthidb.So_phong;
            bool update = _phongthi.Update(item);
            if (!update)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(new
            {
                item = phongthi
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

            var phongthidb = _phongthi.GetDetailById(id, idDonvi);
            if (phongthidb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            //check ràng buộc
            //bool check = _phongthi.CheckContraint(id, idDonvi);
            //if (check)
            //{
            //    return ApiResult.BadRequest("Học sinh đã có ràng buộc, không thể xoá");
            //}

            bool request = _phongthi.Delete(id);
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
            var (result, mess) = (false, "");
            using (var stream = file.OpenReadStream())
            {
                (result, mess) = _phongthi.Import(stream, idDonvi);
            }
            if (result)
            {
                return ApiResult.Success("Import thành công");
            }
            else
            {
                return ApiResult.BadRequest(mess);
            }
        }
    }
}
