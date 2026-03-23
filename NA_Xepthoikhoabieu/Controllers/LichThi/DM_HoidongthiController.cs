using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.IRepository.Auth;
using NA_Logic.IRepository.LichOnTap;
using NA_Logic.IRepository.XepGiamThi;
using NA_Logic.IRepository.XepThoiKhoaBieu;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/hoidong")]
    [ApiController]
    public class DM_HoidongthiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_HoidongthiRepository _hoidong;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_DonviRepository _donvi;
        private readonly IDM_NamhocRepository _nam;
        public DM_HoidongthiController(IMapper mapper, IDM_HoidongthiRepository hoidong, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                       IDM_NamhocRepository nam, IDM_DonviRepository donvi)
        {
            _mapper = mapper;
            _hoidong = hoidong;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _nam = nam;
            _donvi = donvi;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (!checkIsAdmin) return ApiResult.Unauthorized("Không có quyền truy cập, vui lòng liên hệ admin");

            var list = _hoidong.GetList_Paging(PageIndex, PageSize, search);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<DM_Hoidongthi_ListDto>>(list);
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
            // Lấy bản ghi từ db
            var detailhoidong = _hoidong.GetDetailById(Id);
            if (detailhoidong == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");

            return ApiResult.Success(detailhoidong, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_Hoidongthi hoidong)
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin) return ApiResult.Unauthorized("Không có quyền truy cập, vui lòng liên hệ admin");

            hoidong.Id = 0;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool checkma = _hoidong.CheckMa(hoidong.Ma, hoidong.Id_nam, hoidong.Id);
            if (checkma)
            {
                return ApiResult.BadRequest("Mã hội đồng đã tồn tại");
            }

            bool checkDonvi = _donvi.CheckId(hoidong.Id_don_vi);
            if (!checkDonvi)
            {
                return ApiResult.BadRequest("Id đơn vị không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkNam = _nam.CheckId(hoidong.Id_nam);
            if (!checkNam)
            {
                return ApiResult.BadRequest("Id năm không hợp lệ, vui lòng kiểm tra lại");
            }
            
            bool add = _hoidong.Add(hoidong);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");

            return ApiResult.Success(new
            {
                item = hoidong
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_Hoidongthi hoidong)
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin) return ApiResult.Unauthorized("Không có quyền truy cập, vui lòng liên hệ admin");

            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            var hoidongdb = _hoidong.GetDetailById(hoidong.Id);
            if (hoidongdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            bool checkma = _hoidong.CheckMa(hoidong.Ma, hoidong.Id_nam, hoidong.Id);
            if (checkma)
            {
                return ApiResult.BadRequest("Mã hội đồng đã tồn tại");
            }

            bool checkDonvi = _donvi.CheckId(hoidong.Id_don_vi);
            if (!checkDonvi)
            {
                return ApiResult.BadRequest("Id đơn vị không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkNam = _nam.CheckId(hoidong.Id_nam);
            if (!checkNam)
            {
                return ApiResult.BadRequest("Id năm không hợp lệ, vui lòng kiểm tra lại");
            }

            bool add = _hoidong.Update(hoidong);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(new
            {
                item = hoidong
            },
            "Cập nhật thành công");
        }
        [HttpDelete]
        [RequireToken]
        public IActionResult Delete([FromQuery] int id)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idUser = _claimHelperRepository.GetUserId(User);
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin) return ApiResult.Unauthorized("Không có quyền truy cập, vui lòng liên hệ admin");
            var hoidongdb = _hoidong.GetDetailById(id);
            if (hoidongdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            //check ràng buộc
            //bool check = _hoidong.CheckContraint(id, idDonvi);
            //if (check)
            //{
            //    return ApiResult.BadRequest("Học sinh đã có ràng buộc, không thể xoá");
            //}

            //bool deletehoidongLopon = _hl.DeleteByhoidong(id);
            //if (!deletehoidongLopon)
            //    return ApiResult.BadRequest("Xoá các lớp ôn của học sinh thất bại");

            //bool deletehoidongTohop = _ht.DeleteByhoidong(id);
            //if (!deletehoidongTohop)
            //    return ApiResult.BadRequest("Xoá các tổ hợp môn của học sinh thất bại");

            bool request = _hoidong.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
        
    }
}
