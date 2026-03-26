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
    [Route("api/giamthi")]
    [ApiController]
    public class DM_GiamthiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_GiamthiRepository _giamthi;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_DonviRepository _donvi;
        private readonly IDM_GiaovienRepository _giaovien;
        private readonly IDM_DiemthiRepository _diemthi;
        public DM_GiamthiController(IMapper mapper, IDM_GiamthiRepository giamthi, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                       IDM_HoidongthiRepository hoidong, IDM_DonviRepository donvi, IDM_GiaovienRepository giaovien, IDM_DiemthiRepository diemthi)
        {
            _mapper = mapper;
            _giamthi = giamthi;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _donvi = donvi;
            _giaovien = giaovien;
            _diemthi = diemthi;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var list = _giamthi.GetList_Paging(PageIndex, PageSize, search, idDonvi);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<DM_Giamthi_ListDto>>(list);
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
            var detailgiamthi = _giamthi.GetDetailById(Id, idDonvi);
            if (detailgiamthi == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");

            return ApiResult.Success(detailgiamthi, "Thành công");
        }
        [HttpGet("list")]
        [RequireToken]
        public IActionResult GetListMon([FromQuery] int idDiemThi)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            bool checkDiemthi = _diemthi.CheckId(idDiemThi, idDonvi);
            if (!checkDiemthi)
            {
                return ApiResult.BadRequest("Id điểm thi không hợp lệ, vui lòng kiểm tra lại");
            }
            // Lấy bản ghi từ db
            var detailhocsinh = _giamthi.GetGiamThiByDiemThi(idDiemThi);
            if (detailhocsinh == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {idDiemThi}");

            return ApiResult.Success(detailhocsinh, "Thành công");
        }
        [HttpPost("list")]
        [RequireToken]
        public IActionResult CreateList([FromBody] DM_Giamthi_Multi data)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if (data.Id_diem_thi == 0 || data.Id_diem_thi == null)
            {
                return ApiResult.BadRequest("Điểm thi không được để trống");
            }
            if (data.Id_giao_vien.Count == 0 || data.Id_giao_vien == null)
            {
                return ApiResult.BadRequest("Giáo viên không được để trống");
            }
            bool checkDiemthi = _diemthi.CheckId(data.Id_diem_thi, idDonvi);
            if (!checkDiemthi)
            {
                return ApiResult.BadRequest("Id điểm thi không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkGiaoVien = _giaovien.CheckIds(data.Id_giao_vien.OfType<int>(), idDonvi);
            if (!checkGiaoVien)
            {
                return ApiResult.BadRequest("Id giáo viên không hợp lệ, vui lòng kiểm tra lại");
            }

            // add 
            bool add = _giamthi.AddList(data);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");

            return ApiResult.Success("Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_GiamthiDto giamthi)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            var item = _mapper.Map<DM_Giamthi>(giamthi);
            var giamthidb = _giamthi.GetDetailById(giamthi.Id, idDonvi);
            if (giamthidb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            bool checkma = _giamthi.CheckMa(giamthi.Ma, giamthi.Id_diem_thi, giamthi.Id);
            if (checkma)
            {
                return ApiResult.BadRequest("Mã hội đồng đã tồn tại");
            }
            if (giamthi.Id_diem_thi == 0 || giamthi.Id_diem_thi == null)
            {
                return ApiResult.BadRequest("Điểm thi không được để trống");
            }
            bool checkDiemthi = _diemthi.CheckId(giamthi.Id_diem_thi, idDonvi);
            if (!checkDiemthi)
            {
                return ApiResult.BadRequest("Id điểm thi không hợp lệ, vui lòng kiểm tra lại");
            }
            item.Id_giao_vien = giamthidb.Id_diem_thi;
            bool update = _giamthi.Update(item);
            if (!update)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(new
            {
                item = giamthi
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

            var giamthidb = _giamthi.GetDetailById(id, idDonvi);
            if (giamthidb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            //check ràng buộc
            //bool check = _giamthi.CheckContraint(id, idDonvi);
            //if (check)
            //{
            //    return ApiResult.BadRequest("Học sinh đã có ràng buộc, không thể xoá");
            //}

            bool request = _giamthi.Delete(id);
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

            var result = _giamthi.Import(file, idDonvi);

            if (result.success)
            {
                return ApiResult.Success("Import thành công");
            }
            else
            {
                return ApiResult.BadRequest(result.mess);
            }
        }
    }
}
