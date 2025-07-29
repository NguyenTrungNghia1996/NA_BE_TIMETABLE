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
        public Lop_MonController(IMapper mapper, IClaimHelperRepository claimHelperRepository, IDM_MonhocRepository mon, IDM_LophocRepository lop, ILop_MonRepository lopmon)
        {
            _mapper = mapper;
            _claimHelperRepository = claimHelperRepository;
            _lop = lop;
            _mon = mon;
            _lopmon = lopmon;
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
