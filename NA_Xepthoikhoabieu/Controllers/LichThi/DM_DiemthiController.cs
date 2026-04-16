using AutoMapper;
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
    [Route("api/diemthi")]
    [ApiController]
    public class DM_DiemthiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_DiemthiRepository _diemthi;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_DonviRepository _donvi;
        private readonly IDM_HoidongthiRepository _hoidong;
        public DM_DiemthiController(IMapper mapper, IDM_DiemthiRepository diemthi, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                       IDM_HoidongthiRepository hoidong, IDM_DonviRepository donvi)
        {
            _mapper = mapper;
            _diemthi = diemthi;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _hoidong = hoidong;
            _donvi = donvi;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "", [FromQuery] int idHoiDong = 0)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var list = _diemthi.GetList_Paging(PageIndex, PageSize, search, idDonvi, idHoiDong);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<DM_Diemthi_ListDto>>(list);
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
            var detaildiemthi = _diemthi.GetDetailById(Id, idDonvi);
            if (detaildiemthi == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");

            return ApiResult.Success(detaildiemthi, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_Diemthi diemthi)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            diemthi.Id = 0;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool checkma = _diemthi.CheckMa(diemthi.Ma, diemthi.Id_hoi_dong, diemthi.Id);
            if (checkma)
            {
                return ApiResult.BadRequest("Mã điểm thi đã tồn tại");
            }

            if (diemthi.Id_hoi_dong == 0 || diemthi.Id_hoi_dong == null)
            {
                return ApiResult.BadRequest("Vui lòng chọn hội đồng");
            }
            if (diemthi.Id_don_vi == 0 || diemthi.Id_don_vi == null)
            {
                return ApiResult.BadRequest("Vui lòng chọn đơn vị");
            }

            bool checkhoidong = _hoidong.CheckId(diemthi.Id_hoi_dong, idDonvi);
            if (!checkhoidong)
            {
                return ApiResult.BadRequest("Id hội đồng không hợp lệ, vui lòng kiểm tra lại");
            }
            int idUser = _claimHelperRepository.GetUserId(User);
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            bool checkDonvi = _donvi.CheckId(diemthi.Id_don_vi, idDonvi);
            if (!checkDonvi)
            {
                return ApiResult.BadRequest("Id đơn vị không hợp lệ, vui lòng kiểm tra lại");
            }

            bool add = _diemthi.Add(diemthi);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");

            return ApiResult.Success(new
            {
                item = diemthi
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_Diemthi diemthi)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            var diemthidb = _diemthi.GetDetailById(diemthi.Id, idDonvi);
            if (diemthidb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            bool checkma = _diemthi.CheckMa(diemthi.Ma, diemthi.Id_hoi_dong, diemthi.Id);
            if (checkma)
            {
                return ApiResult.BadRequest("Mã điểm thi đã tồn tại");
            }
            if (diemthi.Id_hoi_dong == 0 || diemthi.Id_hoi_dong == null)
            {
                return ApiResult.BadRequest("Vui lòng chọn hội đồng");
            }
            if (diemthi.Id_don_vi == 0 || diemthi.Id_don_vi == null)
            {
                return ApiResult.BadRequest("Vui lòng chọn đơn vị");
            }
            bool checkhoidong = _hoidong.CheckId(diemthi.Id_hoi_dong, idDonvi);
            if (!checkhoidong)
            {
                return ApiResult.BadRequest("Id hội đồng không hợp lệ, vui lòng kiểm tra lại");
            }
            int idUser = _claimHelperRepository.GetUserId(User);
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            bool checkDonvi = _donvi.CheckId(diemthi.Id_don_vi, idDonvi);
            if (!checkDonvi)
            {
                return ApiResult.BadRequest("Id đơn vị không hợp lệ, vui lòng kiểm tra lại");
            }
            bool add = _diemthi.Update(diemthi);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(new
            {
                item = diemthi
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

            var diemthidb = _diemthi.GetDetailById(id, idDonvi);
            if (diemthidb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            //check ràng buộc
            bool check = _diemthi.Check_constraint(id);
            if (check)
            {
                return ApiResult.BadRequest("Điểm thi đã có ràng buộc, không thể xoá");
            }

            bool request = _diemthi.Delete(id, idDonvi);
            if (!request)
                return ApiResult.BadRequest("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }

    }
}
