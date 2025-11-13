using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [ApiController]
    [Route("api/lopmon")]
    public class Lop_MonController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly ILop_MonRepository _lopmon;
        private readonly IDM_LophocRepository _lop;
        private readonly IDM_MonhocRepository _mon;
        private readonly IDM_GiaovienRepository _giaovien;
        private readonly IDM_PhonghocRepository _phong;
        public Lop_MonController(IMapper mapper, IClaimHelperRepository claimHelperRepository, IDM_MonhocRepository mon, IDM_LophocRepository lop, ILop_MonRepository lopmon,
            IDM_PhonghocRepository phong, IDM_GiaovienRepository giaovien)
        {
            _mapper = mapper;
            _claimHelperRepository = claimHelperRepository;
            _lop = lop;
            _mon = mon;
            _lopmon = lopmon;
            _phong = phong;
            _giaovien = giaovien;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList([FromQuery] int IdLop)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy danh sách dữ liệu
            var list = _lopmon.GetLopMon(IdLop, idDonvi);
            if (list == null)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            return ApiResult.Success(list,"Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Update([FromBody] Lop_MonDto LopMon)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) { return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết"); }
            var check_lop = _lop.CheckId(LopMon.Id_lop, idDonvi);
            if (LopMon.Id_lop < 0 || !check_lop)
            {
                return ApiResult.BadRequest($"Id_lop: {LopMon.Id_lop} không hợp lệ");
            }
            var dslopmon = new List<Lophoc_Monhoc>();
            var existingCombinations = new HashSet<string>();
            foreach (var mon in LopMon.Ds_mon)
            {
                if (mon.Trang_thai == true)
                {
                    // Check gv
                    if (mon.Id_giao_vien == 0)
                    {
                        return ApiResult.BadRequest("Vui lòng nhập giáo viên");
                    }
                    if (!_giaovien.CheckId(mon.Id_giao_vien, idDonvi))
                    {
                        return ApiResult.BadRequest($"Giáo viên với Id = {mon.Id_giao_vien} không tồn tại");
                    }

                    // Check môn
                    if (mon.Id_mon == 0)
                    {
                        return ApiResult.BadRequest("Vui lòng nhập môn học");
                    }
                    if (!_mon.CheckId(mon.Id_mon, idDonvi))
                    {
                        return ApiResult.BadRequest($"Môn học với Id = {mon.Id_mon} không tồn tại");
                    }

                    // Check phòng chuyên dụng
                    if (mon.Id_phong_chuyen_dung > 0 && !_phong.CheckId(mon.Id_phong_chuyen_dung, idDonvi))
                    {
                        return ApiResult.BadRequest($"Phòng chuyên dụng với Id = {mon.Id_phong_chuyen_dung} không tồn tại");
                    }

                    // Check phòng truyền thống
                    if (mon.Id_phong_truyen_thong > 0 && !_phong.CheckId(mon.Id_phong_truyen_thong, idDonvi))
                    {
                        return ApiResult.BadRequest($"Phòng truyền thống với Id = {mon.Id_phong_truyen_thong} không tồn tại");
                    }

                    // Check trùng lặp
                    string uniqueKey = $"{LopMon.Id_lop}_{mon.Id_mon}_{mon.Id_giao_vien}_{mon.Id_phong_chuyen_dung}_{mon.Id_phong_truyen_thong}";
                    if (existingCombinations.Contains(uniqueKey))
                    {
                        return ApiResult.BadRequest("Trùng lặp bản ghi");
                    }
                    existingCombinations.Add(uniqueKey);
                    dslopmon.Add(new Lophoc_Monhoc
                    {
                        Id_lop = LopMon.Id_lop,
                        Id_mon = mon.Id_mon,
                        Id_giao_vien = mon.Id_giao_vien,
                        Id_phong_chuyen_dung = mon.Id_phong_chuyen_dung,
                        Id_phong_truyen_thong = mon.Id_phong_truyen_thong,
                        So_tiet_ca_chieu_phong_chuyen_dung = Convert.ToInt32( mon.So_tiet_ca_chieu_phong_chuyen_dung??0),
                        So_tiet_ca_chieu_truyen_thong = Convert.ToInt32(mon.So_tiet_ca_chieu_truyen_thong ?? 0),
                        So_tiet_ca_sang_phong_chuyen_dung = Convert.ToInt32(mon.So_tiet_ca_sang_phong_chuyen_dung ?? 0),
                        So_tiet_ca_sang_truyen_thong = Convert.ToInt32(mon.So_tiet_ca_sang_truyen_thong ?? 0)
                    });
                }
            }

            bool result = _lopmon.AddMonLop(dslopmon, LopMon.Id_lop);
            if (!result)
            {
                return ApiResult.BadRequest("Cập nhật thông tin môn cho lớp không thành công");
            }
            return ApiResult.Success(new { id_lop = LopMon.Id_lop }, "Cập nhật thông tin môn cho lớp thành công");
        }
        [HttpGet("tiettranhxep")]
        [RequireToken]
        public IActionResult GetTiettranhxep([FromQuery] int Id_lop, [FromQuery] int Id_mon)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
            {
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            }
            var check_lop = _lop.CheckId(Id_lop, idDonvi);
            var check_mon = _mon.CheckIdMonLop(Id_mon, Id_lop, idDonvi);
            if (Id_lop < 0 || !check_lop)
            {
                return ApiResult.BadRequest($"Id_lop: {Id_lop} không hợp lệ");
            }
            if (Id_mon < 0 || !check_mon)
            {
                return ApiResult.BadRequest($"Id_mon: {Id_mon} không hợp lệ");
            }
            var result = _lopmon.GetListTietBan_MonLop(Id_lop, Id_mon, idDonvi);
            if (result == null)
            {
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id_lop = {Id_lop} và Id_mon = {Id_mon}");
            }
            return ApiResult.Success(result, "Thành công");
        }
        [HttpPost("tiettranhxep")]
        [RequireToken]
        public IActionResult UpdateTietban([FromBody] LopMon_banDto Tietban)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) { return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết"); }
            var check_lop = _lop.CheckId(Tietban.Id_lop, idDonvi);
            var check_mon = _mon.CheckIdMonLop(Tietban.Id_mon, Tietban.Id_lop, idDonvi);
            if (Tietban.Id_lop < 0 || !check_lop)
            {
                return ApiResult.BadRequest($"Id_lop: {Tietban.Id_lop} không hợp lệ");
            }
            if (Tietban.Id_mon < 0 || !check_mon)
            {
                return ApiResult.BadRequest($"Id_mon: {Tietban.Id_mon} không hợp lệ");
            }
            var dstietban = new List<Lophoc_Monhoc_Tiettranhxep>();
            var errors = new List<string>();
            var existingCombinations = new HashSet<string>();
            foreach(var ca in Tietban.Ds_Ca)
            {
                foreach(var ngay in ca.Ds_Ngay)
                {
                    foreach(var tiet in ngay.Ds_Tiet)
                    {
                        var idtiet = (int)tiet.Id;
                        var idngay = (int)ngay.Id;
                        if(tiet.Trang_thai == true)
                        {
                            string uniqueKey = $"{Tietban.Id_mon}_{Tietban.Id_lop}_{ca.Id}_{idngay}_{idtiet}";
                            if (existingCombinations.Contains(uniqueKey))
                            {
                                errors.Add("Trùng lặp bản ghi");
                                continue;
                            }
                            dstietban.Add(new Lophoc_Monhoc_Tiettranhxep
                            {
                                Id_lop = Tietban.Id_lop,
                                Id_mon = Tietban.Id_mon,
                                Id_ca = ca.Id,
                                Ngay = idngay,
                                Tiet = idtiet
                            });
                        }
                    }
                }
            }
            if (errors.Any())
            {
                return ApiResult.BadRequest(string.Join(", ", errors));
            }

            bool result = _lopmon.AddTietBan_MonLop(dstietban, Tietban.Id_lop, Tietban.Id_mon);
            if (!result)
            {
                return ApiResult.BadRequest("Cập nhật tiết tránh xếp không thành công");
            }
            return ApiResult.Success(new {id_lop = Tietban.Id_lop, id_mon = Tietban.Id_mon, so_tiet_tranh_xep = dstietban.Count},"Cập nhật tiết tránh xếp thành công");
        }
    }
}
