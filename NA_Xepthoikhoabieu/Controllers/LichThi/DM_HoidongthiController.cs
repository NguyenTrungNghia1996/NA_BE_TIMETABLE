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
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] int IdNam, [FromQuery] string search = "")
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var list = _hoidong.GetList_Paging(PageIndex, PageSize, search, idDonvi, IdNam);
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
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            // Lấy bản ghi từ db
            var detailhoidong = _hoidong.GetDetailById(Id, idDonvi);
            if (detailhoidong == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");

            return ApiResult.Success(detailhoidong, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_HoidongthiDto hoidong)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var item = _mapper.Map<DM_Hoidongthi>(hoidong);
            item.Id = 0;
            item.Id_don_vi = idDonvi;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool checkma = _hoidong.CheckMa(hoidong.Ma, idDonvi, hoidong.Id_nam, hoidong.Id);
            if (checkma)
            {
                return ApiResult.BadRequest("Mã hội đồng đã tồn tại");
            }
            
            if(hoidong.Id_nam == 0 || hoidong.Id_nam == null)
            {
                return ApiResult.BadRequest("Vui lòng chọn năm");
            }
            
            bool checkNam = _nam.CheckId(hoidong.Id_nam);
            if (!checkNam)
            {
                return ApiResult.BadRequest("Id năm không hợp lệ, vui lòng kiểm tra lại");
            }
            
            bool add = _hoidong.Add(item);
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
        public IActionResult Update([FromBody] DM_HoidongthiDto hoidong)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var item = _mapper.Map<DM_Hoidongthi>(hoidong);
            item.Id_don_vi = idDonvi;

            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            var hoidongdb = _hoidong.GetDetailById(hoidong.Id, idDonvi);
            if (hoidongdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            bool checkma = _hoidong.CheckMa(hoidong.Ma, idDonvi, hoidong.Id_nam, hoidong.Id);
            if (checkma)
            {
                return ApiResult.BadRequest("Mã hội đồng đã tồn tại");
            }
            if (hoidong.Id_nam == 0 || hoidong.Id_nam == null)
            {
                return ApiResult.BadRequest("Vui lòng chọn năm");
            }
            bool checkNam = _nam.CheckId(hoidong.Id_nam);
            if (!checkNam)
            {
                return ApiResult.BadRequest("Id năm không hợp lệ, vui lòng kiểm tra lại");
            }

            bool add = _hoidong.Update(item);
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
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var hoidongdb = _hoidong.GetDetailById(id, idDonvi);
            if (hoidongdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            //check ràng buộc
            bool check = _hoidong.Check_constraint(id);
            if (check)
            {
                return ApiResult.BadRequest("Hội đồng đã có ràng buộc, không thể xoá");
            }

            bool request = _hoidong.Delete(id, idDonvi);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
        
    }
}
