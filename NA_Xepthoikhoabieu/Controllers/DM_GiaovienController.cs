using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/giaovien")]
    [ApiController]
    public class DM_GiaovienController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_GiaovienRepository _Giaovien;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_CaphocRepository _cap;
        private readonly IDM_CahocRepository _cahoc;
        private readonly IDM_TochuyenmonRepository _tochuyenmon;
        private readonly IDM_MonhocRepository _monhoc;
        private readonly IDM_DiemtruongRepository _diemtruong;
        public DM_GiaovienController(IMapper mapper, IDM_GiaovienRepository Giaovien, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, 
                                     IDM_CaphocRepository cap, IDM_CahocRepository cahoc,IDM_TochuyenmonRepository tochuyenmon, IDM_MonhocRepository monhoc,
                                     IDM_DiemtruongRepository diemtruong)
        {
            _mapper = mapper;
            _Giaovien = Giaovien;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _cap = cap;
            _cahoc = cahoc;
            _tochuyenmon = tochuyenmon;
            _monhoc = monhoc;
            _diemtruong = diemtruong;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {

            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _Giaovien.GetList_Paging(PageIndex, PageSize, search, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            var listDto = _mapper.Map<List<DM_Giaovien_ListDto>>(list);
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
            var detailGiaovien = _Giaovien.GetDetailById(Id, idDonvi);
            if (detailGiaovien == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<DM_GiaovienDto>(detailGiaovien);
            detailDto.Id_diem_truong = _Giaovien.GetlistDiadiemday(Id);
            return ApiResult.Success(detailDto, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_GiaovienDto Giaovien)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // mapper data 
            var item = _mapper.Map<DM_Giaovien>(Giaovien);
            item.Id = 0;
            item.Id_don_vi = idDonvi;
            var check_tochuyenmon = _tochuyenmon.CheckId(Giaovien.Id_to_chuyen_mon, idDonvi);
            var check_diemtruong = _diemtruong.CheckIds(Giaovien.Id_diem_truong, idDonvi);
            if (!check_tochuyenmon || Giaovien.Id_to_chuyen_mon <=0)
                ModelState.AddModelError("Id_to_chuyen_mon", "Id tổ chuyên môn không hợp lệ, vui lòng kiểm tra lại");
            if (!check_diemtruong)
                ModelState.AddModelError("Id_diem_truong", "Id điểm trường không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // add 
            bool add = _Giaovien.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            var itemDto = _mapper.Map<DM_GiaovienDto>(item);
            bool addDiemtruong = _Giaovien.AddDiadiemday(item.Id,Giaovien.Id_diem_truong);
            if (!addDiemtruong)
            {
                return ApiResult.Success(new
                {
                    item = Giaovien
                },
                "Tạo giáo viên thành công, lưu điểm trường thất bại");
            }
            return ApiResult.Success(new
            {
                item = itemDto
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_GiaovienDto Giaovien)
        {// Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var Giaoviendb = _Giaovien.GetDetailById(Giaovien.Id,idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (Giaoviendb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            var item = _mapper.Map<DM_Giaovien>(Giaovien);
            var check_tochuyenmon = _tochuyenmon.CheckId(Giaovien.Id_to_chuyen_mon, idDonvi);
            var check_diemtruong = _diemtruong.CheckIds(Giaovien.Id_diem_truong, idDonvi);
            if (!check_tochuyenmon || Giaovien.Id_to_chuyen_mon <= 0)
                ModelState.AddModelError("Id_to_chuyen_mon", "Id tổ chuyên môn không hợp lệ, vui lòng kiểm tra lại");
            if (!check_diemtruong)
                ModelState.AddModelError("Id_diem_truong", "Id điểm trường không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool add = _Giaovien.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var itemDto = _mapper.Map<DM_GiaovienDto>(item);
            bool updateDiemtruong = _Giaovien.UpdateDiadiemday(item.Id, Giaovien.Id_diem_truong);
            if (!updateDiemtruong)
            {
                return ApiResult.Success(new
                {
                    item = Giaovien
                },
                "Tạo giáo viên thành công, lưu điểm trường thất bại");
            }
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
            var Giaoviendb = _Giaovien.GetDetailById(id, idDonvi);
            if (Giaoviendb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var request = _Giaovien.Delete(id);
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
                var detail = _Giaovien.CheckId(Id, idDonvi);
                if (!detail)
                    return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            }
            // Lấy bản ghi từ db
            var result = _Giaovien.GetListTietBan(Id, idDonvi);

            if (result == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");

            return ApiResult.Success(result, "Thành công");
        }

        [HttpPost("tiettranhxep")]
        [RequireToken]
        public IActionResult Update([FromBody] Giaovien_banDto giaovienban)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            // Validate môn học
            if (!_Giaovien.CheckId(giaovienban.Id_giao_vien, idDonvi))
                return ApiResult.BadRequest("Giáo viên học không hợp lệ");

            // check id, check trùng 
            var danhSachTiet = new List<Giaovien_Tiettranhxep>();
            var errors = new List<string>();
            var existingCombinations = new HashSet<string>();
            var buoiday = new Giaovien_Buoiday
            {
                Id = giaovienban.Id_buoi_day,
                Id_giao_vien = giaovienban.Id_giao_vien,
                Chi_day_mot_buoi = giaovienban.Chi_day_mot_buoi,
                So_tiet_toi_da = giaovienban.So_tiet_toi_da
            };

            foreach (var ca in giaovienban.Ds_Ca)
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
                            string uniqueKey = $"{giaovienban.Id_giao_vien}_{ca.Id}_{idthu}_{idtiet}";
                            //kiểm tra unique tồn tại chưa
                            if (existingCombinations.Contains(uniqueKey))
                            {
                                errors.Add($"Trùng lặp bản ghi");
                                continue;
                            }
                            //nếu chưa tồn tại thì thêm vào combinations
                            existingCombinations.Add(uniqueKey);
                            //thêm các tiết trạng thái bằng true vào danh sách tiết bận
                            danhSachTiet.Add(new Giaovien_Tiettranhxep
                            {
                                Id_giao_vien = giaovienban.Id_giao_vien,
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
            bool addbuoiday = false;
            bool addtiettranhxep = _Giaovien.AddTietBan(danhSachTiet, giaovienban.Id_giao_vien);
            if (buoiday.Chi_day_mot_buoi == true || buoiday.So_tiet_toi_da > 0)
            {
                var check = _Giaovien.GetBuoidayTheoGV(buoiday.Id_giao_vien);
                if (check==null)
                {
                    return ApiResult.NotFound("Giáo viên đã tồn tại buổi dạy");
                }
                addbuoiday = _Giaovien.SaveBuoiday(buoiday);
                if (!addbuoiday)
                {
                    return ApiResult.NotFound("Cập nhật buổi dạy của giáo viên thất bại");
                }
            }
            if (!addtiettranhxep)
                return ApiResult.NotFound("Cập nhật tiết tránh xếp thất bại");
            if(addbuoiday && addtiettranhxep)
                return ApiResult.Success(new { id_giao_vien = giaovienban.Id_giao_vien, so_tiet_ban = danhSachTiet.Count, buoi_day = buoiday }, 
                                         "Cập nhật tiết tránh xếp và buổi dạy của giáo viên thành công");
            return ApiResult.Success(new { id_giao_vien = giaovienban.Id_giao_vien, so_tiet_ban = danhSachTiet.Count },
                                         "Cập nhật tiết tránh xếp của giáo viên thành công");
        }
        [HttpGet("giaovienmonhoc")]
        [RequireToken]
        public IActionResult GetMonByGV(int id)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy bản ghi từ db
            var detail = _Giaovien.GetMonbyGiaovien(id);
            if (detail.Count == 0)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {id}");
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpPost("giaovienmonhoc")]
        [RequireToken]
        public IActionResult addMon([FromBody] Giaovien_MonDto mongv)
        {
            //kiểm tra id đơn vị
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if (mongv.Id_mon == null || mongv.Id_mon.Count == 0)
            {
                ModelState.AddModelError("Id_mon", "Vui lòng chọn ít nhất 1 môn học");
            }
            //kiểm tra id môn
            var checkmon = _monhoc.CheckIds(mongv.Id_mon, idDonvi);
            if (!checkmon)
                ModelState.AddModelError("Id_mon", "Id môn học không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //add môn
            var addMonGv = _Giaovien.UpdateMonbyGiaovien(mongv.Id, mongv.Id_mon);

            if (!addMonGv)
                return ApiResult.Success(new
                {
                    item = mongv
                },
                "Cập nhật môn cho giáo viên thất bại");
            return ApiResult.Success(
             "Cập nhật môn cho giáo viên thành công");
        }
    }
}
