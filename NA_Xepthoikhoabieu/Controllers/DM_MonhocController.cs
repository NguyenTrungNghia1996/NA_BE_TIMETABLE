using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private readonly IDM_CahocRepository _cahoc;
        private readonly IDM_PhonghocRepository _phong;
        private readonly IDM_BanhocRepository _ban;
        private readonly IDM_KhoilopRepository _khoilop;
        public DM_MonhocController(IMapper mapper, IDM_MonhocRepository monhoc, IClaimHelperRepository claimHelperRepository, IDM_LoaiphonghocRepository loaiphong, 
                                   IDM_KhoikienthucRepository khoikienthuc, IDM_CahocRepository cahoc, IDM_PhonghocRepository phong, IDM_BanhocRepository ban, IDM_KhoilopRepository khoilop)
        {
            _mapper = mapper;
            _monhoc = monhoc;
            _claimHelperRepository = claimHelperRepository;
            _loaiphong = loaiphong;
            _khoikienthuc = khoikienthuc;
            _cahoc = cahoc;
            _phong = phong;
            _ban = ban;
            _khoilop = khoilop;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] int id_loai_phong=0, [FromQuery] string search = "")
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _monhoc.GetList_Paging(PageIndex, PageSize, search, idDonvi, id_loai_phong, ref totalrecord);
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
            detailDto.Id_phong = _monhoc.GetlistPhongByDonvi(Id);
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

            //kiểm tra mã môn học
            var check_ma = _monhoc.CheckMa(monhoc.Ma, idDonvi, item.Id);
            if (!check_ma)
                ModelState.AddModelError("Ma", "Mã môn học đã trùng, vui lòng kiểm tra lại");
            var check_ten = _monhoc.CheckTen(monhoc.Ten, idDonvi, item.Id);
            if (!check_ten)
                ModelState.AddModelError("Ten", "Tên môn học đã trùng, vui lòng kiểm tra lại");

            //kiểm tra id loại phòng học và khối kiến thức
            var check_loaiphonghoc = _loaiphong.CheckId(monhoc.Id_loai_phong_hoc);
            var check_khoikienthuc = _khoikienthuc.CheckIds(monhoc.Id_khoi_kien_thuc, idDonvi);
            var check_phong = _phong.CheckIds(monhoc.Id_phong, idDonvi, monhoc.Id_loai_phong_hoc);

            if (!check_loaiphonghoc)
                ModelState.AddModelError("Id_loai_phong_hoc", "Id loại phòng học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_khoikienthuc)
                ModelState.AddModelError("Id_khoi_kien_thuc", "Id khối kiến thức không hợp lệ, vui lòng kiểm tra lại");
            if (!check_phong)
                ModelState.AddModelError("Id_phong", "Id phòng không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // thêm
            bool add = _monhoc.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            monhoc.Id = item.Id;
            var addMonkhoikienthuc = _monhoc.AddKhoikienthuc(monhoc.Id, monhoc.Id_khoi_kien_thuc);
            var addMonphong = _monhoc.AddPhong(monhoc.Id, monhoc.Id_phong);
            if (!addMonkhoikienthuc)
                return ApiResult.Success(new
                {
                    item = monhoc
                },
                "Tạo môn học thành công, lưu khối kiến thức thất bại");
            if (!addMonphong)
                return ApiResult.Success(new
                {
                    item = monhoc
                },
                "Tạo môn học thành công, lưu phòng thất bại");
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
            var monhocdb = _monhoc.CheckId(monhoc.Id, idDonvi);
            if (!monhocdb)
            {
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            }
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (monhocdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            var item = _mapper.Map<DM_Monhoc>(monhoc);
            item.Id_don_vi = idDonvi;

            //kiểm tra mã môn học
            var check_ma = _monhoc.CheckMa(monhoc.Ma, idDonvi, item.Id);
            if (!check_ma)
                ModelState.AddModelError("Ma", "Mã môn học đã trùng, vui lòng kiểm tra lại");
            var check_ten = _monhoc.CheckTen(monhoc.Ten, idDonvi, item.Id);
            if (!check_ten)
                ModelState.AddModelError("Ten", "Tên môn học đã trùng, vui lòng kiểm tra lại");
            //kiểm tra id loại phòng học và khối kiến thức
            var check_loaiphonghoc = _loaiphong.CheckId(monhoc.Id_loai_phong_hoc);
            var check_khoikienthuc = _khoikienthuc.CheckIds(monhoc.Id_khoi_kien_thuc, idDonvi);
            var check_phong = _phong.CheckIds(monhoc.Id_phong, idDonvi, monhoc.Id_loai_phong_hoc);

            if (!check_loaiphonghoc)
                ModelState.AddModelError("Id_loai_phong_hoc", "Id loại phòng học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_khoikienthuc)
                ModelState.AddModelError("Id_khoi_kien_thuc", "Id khối kiến thức không hợp lệ, vui lòng kiểm tra lại");
            if (!check_phong)
                ModelState.AddModelError("Id_phong", "Id phòng không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //update
            bool add = _monhoc.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var editkhoi = _monhoc.UpdateKhoikienthuc(monhoc.Id, monhoc.Id_khoi_kien_thuc);
            var editphong = _monhoc.UpdatePhong(monhoc.Id, monhoc.Id_phong);
            if (!editkhoi)
                return ApiResult.Success(new
                {
                    item = monhoc
                },
                "Cập nhật môn học thành công, cập nhật khối kiến thức thất bại");
            if (!editphong)
                return ApiResult.Success(new
                {
                    item = monhoc
                },
                "Cập nhật môn học thành công, cập nhật phòng học thất bại");

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

            //kiểm tra id mô học
            var monhocdb = _monhoc.CheckId(id, idDonvi);
            if (monhocdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var request = _monhoc.Delete(id, idDonvi);

            //delete khối kiến thức
            var deleteKhoi = _monhoc.DeleteKhoi(id);
            if (!deleteKhoi)
                return ApiResult.NotFound("Xóa các khối kiến thức lỗi");

            //delete tiết bận
            var tietban = _monhoc.DeleteTietTranhXep(id);
            if (!tietban)
                return ApiResult.NotFound("Xóa tiết tránh xếp thất bại");
            //delete phòng chuyên môn
            var phongcm = _monhoc.DeletePhong(id);
            if (!phongcm)
                return ApiResult.NotFound("Xóa các phòng chuyên môn thất bại");
            //delete môn học
            if (!request)
                if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
        [HttpGet("tiettranhxep")]
        [RequireToken]
        public IActionResult GetListTietBan([FromQuery] int Id)
        {
            if (Id < 0)
                return ApiResult.BadRequest($"Id {Id} không hợp lệ, vui lòng kiểm tra lại");

            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if (Id > 0)
            {
                var detail = _monhoc.CheckId(Id, idDonvi);
                if (!detail)
                    return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            }
            // Lấy bản ghi từ db
            var result = _monhoc.GetListTietBan(Id, idDonvi);

            if (result == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");

            return ApiResult.Success(result, "Thành công");
        }

        [HttpPost("tiettranhxep")]
        [RequireToken]
        public IActionResult Update([FromBody] Mon_banDto monban)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            // Validate môn học
            if (!_monhoc.CheckId(monban.Id, idDonvi))
                return ApiResult.BadRequest("Môn học không hợp lệ");

            // check id, check trùng
            var danhSachTiet = new List<Tiet_tranh_xep>();
            var errors = new List<string>();
            var existingCombinations = new HashSet<string>();
            foreach (var ca in monban.Ds_Ca)
            {
                foreach (var ngay in ca.Ds_Ngay)
                {
                    foreach (var tiet in ngay.Ds_Tiet)
                    {
                        var idthu = (int)ngay.Id;
                        var idtiet = (int)tiet.Id;
                        //check các validate
                        if (!Enum.IsDefined(typeof(Ngay), ngay.Id))
                        {
                            errors.Add($"Ngày không hợp lệ: {ngay.Id}");
                            break;
                        }

                        if (!Enum.IsDefined(typeof(Tiet), tiet.Id))
                        {
                            errors.Add($"Tiết không hợp lệ: {tiet.Id}");
                            break;
                        }
                        bool isValid = _cahoc.CheckId(ca.Id, idDonvi);
                        if (!isValid)
                        {
                            errors.Add($"Ca không hợp lệ: {ca.Id}");
                            break;
                        }

                        if (tiet.Trang_thai == true)
                        {
                            //Tạo unique key để check trùng
                            string uniqueKey = $"{monban.Id}_{ca.Id}_{idthu}_{idtiet}";
                            //kiểm tra unique tồn tại chưa
                            if (existingCombinations.Contains(uniqueKey))
                            {
                                errors.Add($"Trùng lặp bản ghi");
                                continue;
                            }
                            //nếu chưa tồn tại thì thêm vào combinations
                            existingCombinations.Add(uniqueKey);
                            //thêm các tiết trạng thái bằng true vào danh sách tiết bận
                            danhSachTiet.Add(new Tiet_tranh_xep
                            {
                                Id_mon = monban.Id,
                                Id_ca = ca.Id,
                                Thu = idthu,
                                Tiet = idtiet
                            });
                        }
                    }
                }
            }
            // Kiểm tra có lỗi không
            if (errors.Any())
                return ApiResult.BadRequest(string.Join("; ", errors));
            //add
            bool result = _monhoc.AddTietBan(danhSachTiet, monban.Id);
            if (!result)
                return ApiResult.NotFound("Cập nhật tiết tránh xếp thất bại");

            return ApiResult.Success(new { id_phong = monban.Id, so_tiet_ban = danhSachTiet.Count }, "Cập nhật tiết tránh xếp thành công");
        }

        [HttpGet("monkhoilop")]
        [RequireToken]
        public IActionResult GetListMonKhoilop([FromQuery] int Idkhoi, [FromQuery] int IdBan)
        {
            if (Idkhoi <0 )
                return ApiResult.BadRequest($"Id khối: {Idkhoi} không hợp lệ, vui lòng kiểm tra lại");
            if (IdBan <0 )
                return ApiResult.BadRequest($"Id ban:  {IdBan} không hợp lệ, vui lòng kiểm tra lại");

            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if (Idkhoi > 0)
            {
                var detail = _khoilop.CheckId(Idkhoi);
                if (!detail)
                    return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id khối = {Idkhoi}");
            }
            if (IdBan > 0)
            {
                var detail = _ban.CheckId(IdBan, idDonvi);
                if (!detail)
                    return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id ban = {IdBan}");
            }
            // Lấy bản ghi từ db
            var result = _monhoc.GetMonhocKhoilop(Idkhoi,IdBan, idDonvi);

            if (result == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id khối = {Idkhoi} và Id ban = {IdBan}");

            return ApiResult.Success(result, "Thành công");
        }
        [HttpPost("monkhoilop")]
        [RequireToken]
        public IActionResult UpdateMonKhoi([FromBody] Monhoc_KhoiLopDto monkhoidto)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var monkhoi = new List<Monhoc_Khoilop>();
            if (monkhoidto.Id_khoi > 0)
            {
                var detail = _khoilop.CheckId(monkhoidto.Id_khoi);
                if (!detail)
                    return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id khối = {monkhoidto.Id_khoi}");
            }
            if (monkhoidto.Id_ban > 0)
            {
                var detail = _ban.CheckId(monkhoidto.Id_ban, idDonvi);
                if (!detail)
                    return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id ban = {monkhoidto.Id_ban}");
            }
            var errors = new List<string>(); ;
            foreach (var ds_mon in monkhoidto.ds_Mon)
            {
                foreach(var ds_ca in ds_mon.ds_Ca)
                {
                    if (ds_mon.Trang_thai == true)
                    {
                        var check_mon = _monhoc.CheckId(ds_mon.Id_mon, idDonvi);
                        var check_ca = _cahoc.CheckId(ds_ca.Id_ca,idDonvi);
                        if (!check_mon)
                        {
                            errors.Add($"Môn không hợp lệ: {ds_mon.Id_mon}");
                            break;
                        }
                        if (!check_ca)
                        {
                            errors.Add($"Ca không hợp lệ: {ds_ca.Id_ca}");
                            break;
                        }
                        monkhoi.Add(new Monhoc_Khoilop
                        {
                            Id_ban = monkhoidto.Id_ban,
                            Id_khoi = monkhoidto.Id_khoi,
                            Id_mon = ds_mon.Id_mon,
                            Id_ca = ds_ca.Id_ca,
                            So_tiet = ds_ca.So_tiet??0,
                            So_nhom = ds_ca.So_nhom??0,
                        });
                    }
                }
            }
            if (errors.Any())
                return ApiResult.BadRequest(string.Join("; ", errors));
            //add
            bool result = _monhoc.AddMonKhoi(monkhoi, monkhoidto.Id_khoi);
            if (!result)
                return ApiResult.NotFound("Cập nhật tiết tránh xếp thất bại");

            return ApiResult.Success(new { id_khoi = monkhoidto.Id_khoi, id_ban = monkhoidto.Id_ban, so_mon = monkhoi.Count }, "Cập nhật môn khối thành công");
        }
    }
}
