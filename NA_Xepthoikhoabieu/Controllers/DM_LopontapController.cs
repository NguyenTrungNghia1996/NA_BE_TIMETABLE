using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/lopontap")]
    [ApiController]
    public class DM_LopontapController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_LopontapRepository _Lopontap;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_PhonghocRepository _phong;
        private readonly IDM_GiaovienRepository _giaovien;
        private readonly IDM_KhoilopRepository _khoilop;
        private readonly IDM_MonhocRepository _mon;
        private readonly IDM_NamhocRepository _nam;
        private readonly IValidateRepository _validate;
        public DM_LopontapController(IMapper mapper, IDM_LopontapRepository Lopontap, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, IValidateRepository validate,
            IDM_GiaovienRepository giaovien, IDM_NamhocRepository nam, IDM_MonhocRepository mon, IDM_PhonghocRepository phong, IDM_KhoilopRepository khoilop)
        {
            _mapper = mapper;
            _Lopontap = Lopontap;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _phong = phong;
            _giaovien = giaovien;
            _nam = nam;
            _mon = mon;
            _phong = phong;
            _khoilop = khoilop;
            _validate = validate;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            search = search.Trim();
            var list = _Lopontap.GetList_Paging(PageIndex, PageSize, search, idDonvi);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<DM_Lopontap_ListDto>>(list);
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

            // Lấy bản ghi từ db
            var detailLopontap = _Lopontap.GetDetailById(Id);
            if (detailLopontap == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<DM_LopontapDto>(detailLopontap);
            return ApiResult.Success(detailDto, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_LopontapDto Lopontap)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // mapper data 
            var item = _mapper.Map<DM_Lopontap>(Lopontap);
            item.Id = 0;
            item.Id_don_vi = idDonvi;
            //bool checkten = _validate.CheckTrungTen_byDonvi<DM_Lopontap>(idDonvi, Lopontap.Ten);
            //if (checkten)
            //{
            //    return ApiResult.BadRequest("Tên ban học đã tồn tại");
            //}
            var check_phong = _phong.CheckId(Lopontap.Id_phong, idDonvi);
            var check_khoi = _khoilop.CheckKhoilopByDonvi(Lopontap.Id_khoi, idDonvi);
            var check_mon = _mon.CheckId(Lopontap.Id_mon, idDonvi);
            var check_giaovien = _giaovien.CheckId(Lopontap.Id_giao_vien, idDonvi);
            var check_nam = _nam.CheckId(Lopontap.Id_nam);

            if (!check_phong)
                return ApiResult.BadRequest("Id phòng học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_khoi)
                return ApiResult.BadRequest("Id khối học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_mon)
                return ApiResult.BadRequest("Id môn học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_giaovien)
                return ApiResult.BadRequest("Id giáo viên không hợp lệ, vui lòng kiểm tra lại");
            if (!check_nam)
                return ApiResult.BadRequest("Id năm không hợp lệ, vui lòng kiểm tra lại");

            // add 
            bool add = _Lopontap.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            var itemDto = _mapper.Map<DM_LopontapDto>(item);
            return ApiResult.Success(new
            {
                item = itemDto
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_LopontapDto Lopontap)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var Lopontapdb = _Lopontap.GetDetailById(Lopontap.Id);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (Lopontapdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            var item = _mapper.Map<DM_Lopontap>(Lopontap);
            item.Id_don_vi = idDonvi;
            //bool checkten = _validate.CheckTrungTen_byDonvi<DM_Lopontap>(idDonvi, Lopontap.Ten, Lopontap.Id);
            //if (checkten)
            //{
            //    return ApiResult.BadRequest("Tên ban học đã tồn tại");
            //}
            var check_phong = _phong.CheckId(Lopontap.Id_phong, idDonvi);
            var check_khoi = _khoilop.CheckKhoilopByDonvi(Lopontap.Id_khoi, idDonvi);
            var check_mon = _mon.CheckId(Lopontap.Id_mon, idDonvi);
            var check_giaovien = _giaovien.CheckId(Lopontap.Id_giao_vien, idDonvi);
            var check_nam = _nam.CheckId(Lopontap.Id_nam);

            if (!check_phong)
                return ApiResult.BadRequest("Id phòng học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_khoi)
                return ApiResult.BadRequest("Id khối học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_mon)
                return ApiResult.BadRequest("Id môn học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_giaovien)
                return ApiResult.BadRequest("Id giáo viên không hợp lệ, vui lòng kiểm tra lại");
            if (!check_nam)
                return ApiResult.BadRequest("Id năm không hợp lệ, vui lòng kiểm tra lại");
            
            bool add = _Lopontap.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var itemDto = _mapper.Map<DM_LopontapDto>(item);
            return ApiResult.Success(new
            {
                item = itemDto
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
            var Lopontapdb = _Lopontap.GetDetailById(id);
            if (Lopontapdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            bool result = _Lopontap.Delete(id);
            if (!result)
                return ApiResult.BadRequest("Xoá thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
