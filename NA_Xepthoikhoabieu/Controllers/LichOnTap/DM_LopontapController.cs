using AutoMapper;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.IRepository.Auth;
using NA_Logic.IRepository.LichOnTap;
using NA_Logic.IRepository.XepThoiKhoaBieu;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers.LichOnTap
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
        private readonly IDM_DonviRepository _donvi;
        private readonly IThongtin_DonviRepository _ttdonvi;
        private readonly IHocsinh_LoponRepository _hl;
        private readonly IValidateRepository _validate;
        public DM_LopontapController(IMapper mapper, IDM_LopontapRepository Lopontap, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, IValidateRepository validate,
            IDM_GiaovienRepository giaovien, IDM_NamhocRepository nam, IDM_MonhocRepository mon, IDM_PhonghocRepository phong, IDM_KhoilopRepository khoilop, 
            IDM_DonviRepository donvi, IThongtin_DonviRepository ttdonvi, IHocsinh_LoponRepository hl)
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
            _donvi = donvi;
            _ttdonvi = ttdonvi;
            _hl = hl;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] int Id_khoi,[FromQuery] int Id_mon, [FromQuery] string search = "")
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            search = search.Trim();
            var list = _Lopontap.GetList_Paging(PageIndex, PageSize, search, idDonvi,Id_khoi, Id_mon);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<DM_Lopontap_ListDto>>(list);
            int totalrecord = list.First().Total;
            return ApiResult.Success(new
            {
                items = listDto,
                totalrecord
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
            bool checkma = _Lopontap.CheckMa(Lopontap.Ma, idDonvi, item.Id);
            if (checkma)
            {
                return ApiResult.BadRequest("Mã lớp ôn đã tồn tại");
            }
            bool checkten = _validate.CheckTrungTen_byDonvi<DM_Lopontap>(idDonvi, Lopontap.Ten);
            if (checkten)
            {
                return ApiResult.BadRequest("Tên lớp học đã tồn tại");
            }
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

            bool checktrung = _Lopontap.CheckTrung(Lopontap, idDonvi);
            if (checktrung)
            {
                return ApiResult.BadRequest("Lớp học đã tồn tại");
            }
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
            bool checkma = _Lopontap.CheckMa(Lopontap.Ma, idDonvi, item.Id);
            if (checkma)
            {
                return ApiResult.BadRequest("Mã lớp ôn đã tồn tại");
            }
            bool checkten = _validate.CheckTrungTen_byDonvi<DM_Lopontap>(idDonvi, Lopontap.Ten, Lopontap.Id);
            if (checkten)
            {
                return ApiResult.BadRequest("Tên lớp học đã tồn tại");
            }
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
            //check ràng buộc
            bool check = _Lopontap.CheckContraint(id, idDonvi);
            if (check)
            {
                return ApiResult.BadRequest("Lớp ôn đã có ràng buộc, không thể xoá");
            }
            bool deleteTietnghi = _Lopontap.DeleteTietBan(id);
            if (!deleteTietnghi)
                return ApiResult.BadRequest("Xoá tiết nghỉ thất bại");
            bool deleteHocsinhLopon = _hl.DeleteByLop(id);
            if (!deleteHocsinhLopon)
                return ApiResult.BadRequest("Xoá các học sinh của lớp thất bại");
            bool result = _Lopontap.Delete(id);
            if (!result)
                return ApiResult.BadRequest("Xoá thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
        [HttpGet("tietnghi")]
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
                var detail = _Lopontap.CheckId(Id, idDonvi);
                if (!detail)
                    return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            }
            var result = _Lopontap.GetListTietBan(Id, idDonvi);
            if (result == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            return ApiResult.Success(result, "Th  ành công");
        }

        [HttpPost("tietnghi")]
        [RequireToken]
        public IActionResult Update([FromBody] Lopontap_TietnghiDto tietnghi)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            if (!_Lopontap.CheckId(tietnghi.Id, idDonvi))
                return ApiResult.BadRequest("Lớp ôn tập không hợp lệ");
            var detailDonvi = _donvi.getDetailById(idDonvi);
            var listCa = _ttdonvi.GetlistCabyDonvi(idDonvi);
            var caDictionary = listCa.ToDictionary(c => c.Id_ca_hoc, c => c.So_tiet);

            var errors = new List<string>();
            foreach (var ca in tietnghi.Ds_Ca)
            {
                if (!caDictionary.ContainsKey(ca.Id))
                {
                    errors.Add($"Ca {ca.Id} không hợp lệ hoặc không thuộc đơn vị");
                    continue;
                }

                int soTietCuaCa = caDictionary[ca.Id];

                foreach (var ngay in ca.Ds_Ngay)
                {
                    var idthu = (int)ngay.Id;
                    if (idthu < 1 || idthu > detailDonvi.So_ngay)
                    {
                        errors.Add($"Ca {ca.Id}: Ngày {idthu} không hợp lệ");
                        continue;
                    }

                    foreach (var tiet in ngay.Ds_Tiet)
                    {
                        var idtiet = (int)tiet.Id;
                        if (idtiet < 1 || idtiet > soTietCuaCa)
                        {
                            errors.Add($"Ca {ca.Id}, Ngày {idthu}: Tiết {idtiet} không hợp lệ");
                        }
                    }
                }
            }

            var danhSachTietBan = new List<Lopontap_Tietnghi>();
            
            var existingCombinations = new HashSet<string>();
            foreach (var ca in tietnghi.Ds_Ca)
            {
                foreach (var ngay in ca.Ds_Ngay)
                {
                    foreach (var tiet in ngay.Ds_Tiet)
                    {
                        var idthu = (int)ngay.Id;
                        var idtiet = (int)tiet.Id;

                        if (tiet.Trang_thai == true)
                        {
                            //Tạo unique key để check trùng
                            string uniqueKey = $"{tietnghi.Id}_{ca.Id}_{idthu}_{idtiet}";
                            //kiểm tra unique tồn tại chưa
                            if (existingCombinations.Contains(uniqueKey))
                            {
                                errors.Add($"Trùng lặp bản ghi");
                                continue;
                            }
                            //nếu chưa tồn tại thì thêm vào combinations
                            existingCombinations.Add(uniqueKey);
                            //thêm các tiết trạng thái bằng true vào danh sách tiết bận
                            danhSachTietBan.Add(new Lopontap_Tietnghi
                            {
                                Id_lop_on = tietnghi.Id,
                                Id_ca = ca.Id,
                                Ngay = idthu,
                                Tiet = idtiet
                            });
                        }
                    }
                }
            }
            if (errors.Any())
                return ApiResult.BadRequest(string.Join("; ", errors));

            bool result = _Lopontap.AddTietBan(danhSachTietBan, tietnghi.Id);
            if (!result)
                return ApiResult.NotFound("Cập nhật tiết bận thất bại");

            return ApiResult.Success(new { id_phong = tietnghi.Id, so_tiet_ban = danhSachTietBan.Count }, "Cập nhật tiết bận thành công");
        }
    }
}
