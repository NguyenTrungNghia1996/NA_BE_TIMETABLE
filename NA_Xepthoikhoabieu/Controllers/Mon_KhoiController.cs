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
    [Route("api/monhoc/monkhoilop")]
    [ApiController]
    public class Mon_KhoiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_MonhocRepository _monhoc;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IMon_KhoiRepository _monkhoi;
        private readonly IDM_KhoilopRepository _khoilop;
        private readonly IDM_BanhocRepository _ban;
        private readonly IDM_CahocRepository _cahoc;
        public Mon_KhoiController(IMapper mapper, IDM_MonhocRepository monhoc, IClaimHelperRepository claimHelperRepository, IMon_KhoiRepository monkhoi
                                  , IDM_KhoilopRepository khoilop, IDM_BanhocRepository ban, IDM_CahocRepository cahoc)
        {
            _mapper = mapper;
            _monhoc = monhoc;
            _claimHelperRepository = claimHelperRepository;
            _monkhoi = monkhoi;
            _khoilop = khoilop;
            _ban = ban;
            _cahoc = cahoc;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetListMonKhoilop([FromQuery] int Idkhoi, [FromQuery] int IdBan)
        {
            if (Idkhoi < 0)
                return ApiResult.BadRequest($"Id khối: {Idkhoi} không hợp lệ, vui lòng kiểm tra lại");
            if (IdBan < 0)
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
            var result = _monkhoi.GetMonhocKhoilop(Idkhoi, IdBan, idDonvi);

            if (result == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id khối = {Idkhoi} và Id ban = {IdBan}");

            return ApiResult.Success(result, "Thành công");
        }
        [HttpPost]
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
                    return ApiResult.NotFound($"Id khối = {monkhoidto.Id_khoi} không hợp lệ");
            }
            if (monkhoidto.Id_ban > 0)
            {
                var detail = _ban.CheckId(monkhoidto.Id_ban, idDonvi);
                if (!detail)
                    return ApiResult.NotFound($"Id ban = {monkhoidto.Id_ban} không hợp lệ");
            }
            var errors = new List<string>(); ;
            foreach (var ds_mon in monkhoidto.ds_Mon)
            {
                foreach (var ds_ca in ds_mon.ds_Ca)
                {
                    if (ds_mon.Trang_thai == true)
                    {
                        var check_mon = _monhoc.CheckId(ds_mon.Id_mon, idDonvi);
                        var check_ca = _cahoc.CheckId(ds_ca.Id_ca, idDonvi);
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
                            So_tiet = ds_ca.So_tiet ?? 0,
                            So_nhom = ds_ca.So_nhom ?? 0,
                        });
                    }
                }
            }
            if (errors.Any())
                return ApiResult.BadRequest(string.Join("; ", errors));
            //add
            bool result = _monkhoi.AddMonKhoi(monkhoi, monkhoidto.Id_khoi, monkhoidto.Id_ban);
            if (!result)
                return ApiResult.NotFound("Cập nhật tiết tránh xếp thất bại");

            return ApiResult.Success(new { id_khoi = monkhoidto.Id_khoi, id_ban = monkhoidto.Id_ban, so_mon = monkhoi.Count }, "Cập nhật môn khối thành công");
        }
        [HttpGet("tiettranhxep")]
        [RequireToken]
        public IActionResult GetListTietBan([FromQuery] int Id_khoi, [FromQuery] int Id_ban, [FromQuery] int Id_mon)
        {
            if (Id_mon < 0)
                return ApiResult.BadRequest($"Id_mon {Id_mon} không hợp lệ, vui lòng kiểm tra lại");
            if (Id_khoi < 0)
                return ApiResult.BadRequest($"Id_khoi {Id_khoi} không hợp lệ, vui lòng kiểm tra lại");

            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            
            if (Id_khoi > 0)
            {
                var detail = _khoilop.CheckKhoilopByDonvi(Id_khoi, idDonvi);
                if (!detail)
                    return ApiResult.NotFound($"Id_khoi = {Id_khoi} không hợp lệ");
            }
            if (Id_ban > 0)
            {
                var detail = _ban.CheckId(Id_ban, idDonvi);
                if (!detail)
                    return ApiResult.NotFound($"Id_ban = {Id_ban} không hợp lệ");
            }
            if (Id_mon > 0)
            {
                var detail = _monhoc.CheckIdMonKhoi(Id_mon, Id_khoi, Id_ban, idDonvi);
                if (!detail)
                    return ApiResult.NotFound($"Id_mon = {Id_mon} không hợp lệ");
            }
            // Lấy bản ghi từ db
            var result = _monkhoi.GetListTietBan(Id_mon,Id_khoi, Id_ban, idDonvi);

            if (result == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id_mon= {Id_mon}, Id_khoi= {Id_khoi} và Id_ban = {Id_ban}");

            return ApiResult.Success(result, "Thành công");
        }

        [HttpPost("tiettranhxep")]
        [RequireToken]
        public IActionResult Update([FromBody] Monhoc_Khoilop_BanDto monban)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            // Validate môn học, khối, ban
            if (!_monhoc.CheckIdMonKhoi(monban.Id_mon, monban.Id_khoi, monban.Id_ban, idDonvi))
                return ApiResult.BadRequest("Môn học không hợp lệ");
            if (monban.Id_khoi > 0)
            {
                var detail = _khoilop.CheckId(monban.Id_khoi);
                if (!detail)
                    return ApiResult.NotFound($"Id khối = {monban.Id_khoi} không hợp lệ");
            }
            if (monban.Id_ban > 0)
            {
                var detail = _ban.CheckId(monban.Id_ban, idDonvi);
                if (!detail)
                    return ApiResult.NotFound($"Id ban = {monban.Id_ban} không hợp lệ");
            }
            if (monban.Id_mon > 0)
            {
                var detail = _monhoc.CheckIdMonKhoi(monban.Id_mon, monban.Id_khoi, monban.Id_ban, idDonvi);
                if (!detail)
                    return ApiResult.NotFound($"Id_mon = {monban.Id_mon} không hợp lệ");
            }
            // check id, check trùng
            var danhSachTiet = new List<Monhoc_Khoilop_Tiettranhxep>();
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
                            string uniqueKey = $"{monban.Id_mon}_{monban.Id_ban}_{monban.Id_khoi}_{ca.Id}_{idthu}_{idtiet}";
                            //kiểm tra unique tồn tại chưa
                            if (existingCombinations.Contains(uniqueKey))
                            {
                                errors.Add($"Trùng lặp bản ghi");
                                continue;
                            }
                            //nếu chưa tồn tại thì thêm vào combinations
                            existingCombinations.Add(uniqueKey);
                            //thêm các tiết trạng thái bằng true vào danh sách tiết bận
                            danhSachTiet.Add(new Monhoc_Khoilop_Tiettranhxep
                            {
                                Id_mon = monban.Id_mon,
                                Id_ban = monban.Id_ban,
                                Id_khoi = monban.Id_khoi,
                                Id_ca = ca.Id,
                                Ngay = idthu,
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
            bool result = _monkhoi.AddTietBan(danhSachTiet, monban.Id_mon, monban.Id_khoi, monban.Id_ban);
            if (!result)
                return ApiResult.NotFound("Cập nhật tiết tránh xếp thất bại");

            return ApiResult.Success(new { id_mon = monban.Id_mon, so_tiet_ban = danhSachTiet.Count }, "Cập nhật tiết tránh xếp thành công");
        }
    }
}
