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
    [Route("api/monhoc")]
    [ApiController]
    public class DM_MonhocController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_MonhocRepository _monhoc;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IDM_LoaiphonghocRepository _loaiphong;
        private readonly IDM_KhoikienthucRepository _khoikienthuc;
        public DM_MonhocController(IMapper mapper, IDM_MonhocRepository monhoc, IClaimHelperRepository claimHelperRepository, IDM_LoaiphonghocRepository loaiphong, IDM_KhoikienthucRepository khoikienthuc)
        {
            _mapper = mapper;
            _monhoc = monhoc;
            _claimHelperRepository = claimHelperRepository;
            _loaiphong = loaiphong;
            _khoikienthuc = khoikienthuc;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _monhoc.GetList_Paging(PageIndex, PageSize, search, idDonvi, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            var listDto = _mapper.Map<List<DM_Monhoc_ListDto>>(list);
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
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy bản ghi từ db
            var detailCahoc = _monhoc.GetDetailById(Id, idDonvi);
            if (detailCahoc == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<DM_MonhocDto>(detailCahoc);
            detailDto.Id_khoi_kien_thuc = _monhoc.GetlistKhoikienthucbyMon(Id);
            return ApiResult.Success(detailDto, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_MonhocDto monhoc)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // mapper data 
            var item = _mapper.Map<DM_Monhoc>(monhoc);
            item.Id = 0;
            item.Id_don_vi = idDonvi;
            var check_loaiphonghoc = _loaiphong.CheckId(monhoc.Id_loai_phong_hoc, idDonvi);
            var check_khoikienthuc = _khoikienthuc.CheckIds(monhoc.Id_khoi_kien_thuc, idDonvi);
            if (!check_loaiphonghoc)
                ModelState.AddModelError("Id_loai_phong_hoc", "Id loại phòng học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_khoikienthuc)
                ModelState.AddModelError("Id_khoi_kien_thuc", "Id khối kiến thức không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // add 
            bool add = _monhoc.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            monhoc.Id = item.Id;
            var addMonkhoikienthuc = _monhoc.AddKhoikienthuc(monhoc.Id, monhoc.Id_khoi_kien_thuc);

            if (!addMonkhoikienthuc)
                return ApiResult.Success(new
                {
                    item = monhoc
                },
                "Tạo môn học thành công, lưu khối kiến thức thất bại");
            return ApiResult.Success(new
            {
                item = monhoc
            }, "Tạo môn học thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_MonhocDto monhoc)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var monhocdb = _monhoc.GetDetailById(monhoc.Id, idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (monhocdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            var item = _mapper.Map<DM_Monhoc>(monhoc);
            item.Id_don_vi = idDonvi;
            var check_loaiphonghoc = _loaiphong.CheckId(monhoc.Id_loai_phong_hoc, idDonvi);
            var check_khoikienthuc = _khoikienthuc.CheckIds(monhoc.Id_khoi_kien_thuc, idDonvi);
            if (!check_loaiphonghoc)
                ModelState.AddModelError("Id_loai_phong_hoc", "Id loại phòng học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_khoikienthuc)
                ModelState.AddModelError("Id_khoi_kien_thuc", "Id khối kiến thức không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool add = _monhoc.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var editCap = _monhoc.UpdateKhoikienthuc(monhoc.Id, monhoc.Id_khoi_kien_thuc);

            if (!editCap)
                return ApiResult.Success(new
                {
                    item = monhoc
                },
                "Cập nhật môn học thành công, cập nhật khối kiến thức thất bại");

            return ApiResult.Success(new
            {
                item = monhoc
            }, "Cập nhật khối kiến thức thành công");
        }
        [HttpDelete]
        [RequireToken]
        public IActionResult Delete([FromQuery] int id)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var monhocdb = _monhoc.GetDetailById(id, idDonvi);
            if (monhocdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var request = _monhoc.Delete(id, idDonvi);
            var deleteKhoi = _monhoc.DeleteKhoi(id);
            if (!deleteKhoi)
                return ApiResult.NotFound("Xóa các khối kiến thức lỗi");
            if (!request)
                if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
