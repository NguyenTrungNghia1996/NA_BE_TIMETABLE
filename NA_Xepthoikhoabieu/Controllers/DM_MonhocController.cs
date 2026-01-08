using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;
using Npgsql.PostgresTypes;
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
        private readonly IDM_LophocRepository _lop;
        private readonly IValidateRepository _validate;
        public DM_MonhocController(IMapper mapper, IDM_MonhocRepository monhoc, IClaimHelperRepository claimHelperRepository, IDM_LoaiphonghocRepository loaiphong, 
                                   IDM_KhoikienthucRepository khoikienthuc, IDM_CahocRepository cahoc, IDM_PhonghocRepository phong, IDM_BanhocRepository ban,
                                   IDM_KhoilopRepository khoilop, IDM_LophocRepository lop, IValidateRepository validate)
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
            _lop = lop;
            _validate = validate;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] int id_loai_phong=0, [FromQuery] int id_lop=0, 
                                            [FromQuery] int id_khoi =0 , [FromQuery] int id_ban=0, [FromQuery] string search = "")
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            search = search.Trim();
            var list = _monhoc.GetList_Paging(PageIndex, PageSize, search, idDonvi, id_loai_phong,id_lop,id_khoi,id_ban, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<DM_Monhoc_ListDto>>(list);
            return ApiResult.Success(new
            {
                items = listDto,
                totalrecord = totalrecord
            },
            "Thành công");
        }
        [HttpGet("monlop")]
        [RequireToken]
        public IActionResult GetList_MonLop( [FromQuery] int idLop)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var check_lop = _lop.CheckId(idLop, idDonvi);
            if (idLop < 0 || !check_lop)
            {
                return ApiResult.BadRequest($"IdLop: {idLop} không hợp lệ");
            }
            var list = _monhoc.GetList_MonLop(idDonvi, idLop);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            return ApiResult.Success(new
            {
                items = list,
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
            detailDto.Ten_theo_nganh = _monhoc.GetTenNgayByIdMon(Id);
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
                return ApiResult.BadRequest( "Mã môn học đã trùng, vui lòng kiểm tra lại");
            bool checkten = _validate.CheckTrungTen_byDonvi<DM_Monhoc>(idDonvi, monhoc.Ten);
            if (checkten)
            {
                return ApiResult.BadRequest("Tên môn học đã tồn tại");
            }
            if(monhoc.So_tiet_toi_da_mot_ca > monhoc.So_tiet_toi_da_hai_ca)
            {
                return ApiResult.BadRequest("Số tiết tối đa 2 ca phải lớn hơn hoặc bằng số tiết tối đa 1 ca");
            }
            //kiểm tra id loại phòng học và khối kiến thức
            var check_loaiphonghoc = _loaiphong.CheckId(monhoc.Id_loai_phong_hoc);
            var check_khoikienthuc = _khoikienthuc.CheckIds(monhoc.Id_khoi_kien_thuc, idDonvi);
            var check_phong = _phong.CheckIds(monhoc.Id_phong, idDonvi, monhoc.Id_loai_phong_hoc);

             if (monhoc.Id_loai_phong_hoc >0 &&!check_loaiphonghoc)
                ModelState.AddModelError("Id_loai_phong_hoc", "Id loại phòng học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_khoikienthuc)
                ModelState.AddModelError("Id_khoi_kien_thuc", "Id khối kiến thức không hợp lệ, vui lòng kiểm tra lại");
            if (monhoc.Id_loai_phong_hoc > 0 && !check_phong)
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
            var addTenNganh = _monhoc.SaveTenMonTheoNganh(monhoc.Id, monhoc.Ten_theo_nganh);
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
            if (!addTenNganh)
                return ApiResult.Success(new
                {
                    item = monhoc
                },
                "Tạo môn học thành công, lưu tên theo ngành thất bại");
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

            var item = _mapper.Map<DM_Monhoc>(monhoc);
            item.Id_don_vi = idDonvi;

            //kiểm tra mã môn học
            var check_ma = _monhoc.CheckMa(monhoc.Ma, idDonvi, item.Id);
            if (!check_ma)
                return ApiResult.BadRequest( "Mã môn học đã trùng, vui lòng kiểm tra lại");
            bool checkten = _validate.CheckTrungTen_byDonvi<DM_Monhoc>(idDonvi, monhoc.Ten, monhoc.Id);
            if (checkten)
            {
                return ApiResult.BadRequest("Tên môn học đã tồn tại");
            }
            if (monhoc.So_tiet_toi_da_mot_ca > monhoc.So_tiet_toi_da_hai_ca)
            {
                return ApiResult.BadRequest("Số tiết tối đa 2 ca phải lớn hơn hoặc bằng số tiết tối đa 1 ca");
            }
            //kiểm tra id loại phòng học và khối kiến thức
            var check_loaiphonghoc = _loaiphong.CheckId(monhoc.Id_loai_phong_hoc);
            var check_khoikienthuc = _khoikienthuc.CheckIds(monhoc.Id_khoi_kien_thuc, idDonvi);
            var check_phong = _phong.CheckIds(monhoc.Id_phong, idDonvi, monhoc.Id_loai_phong_hoc);

            if (monhoc.Id_loai_phong_hoc > 0 && !check_loaiphonghoc)
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
            var editten = _monhoc.SaveTenMonTheoNganh(monhoc.Id, monhoc.Ten_theo_nganh);
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
            if (!editten)
                return ApiResult.Success(new
                {
                    item = monhoc
                },
                "Cập nhật môn học thành công, cập nhật tên theo ngành thất bại");

            return ApiResult.Success(new
            {
                item = monhoc
            }, "Cập nhật môn học thành công");
        }
        [HttpDelete]
        [RequireToken]
        public IActionResult Delete([FromQuery] int id)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            //kiểm tra id môn học
            var monhocdb = _monhoc.CheckId(id, idDonvi);
            if (!monhocdb)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");

            //check ràng buộc
            var (check, message) = _monhoc.CheckContraints(id);
            if (!check)
                return ApiResult.BadRequest(message);
            //delete khối kiến thức
            var deleteKhoi = _monhoc.DeleteKhoi(id);
            if (!deleteKhoi)
                return ApiResult.NotFound("Xóa các khối kiến thức lỗi");

            //delete tiết bận
            var tietban = _monhoc.DeleteTietTranhXep(id);
            if (!tietban)
                return ApiResult.NotFound("Xóa tiết tránh xếp thất bại");

            var khoikienthuc = _monhoc.DeleteKhoi(id);
            if (!tietban)
                return ApiResult.NotFound("Xóa khối kiến thức thất bại");

            //delete phòng chuyên môn
            var phongcm = _monhoc.DeletePhong(id);
            if (!phongcm)
                return ApiResult.NotFound("Xóa các phòng chuyên môn thất bại");

            //delete tên môn
            var tenmontheonganh = _monhoc.DeleteTenMonTheoNganh(id);
            if (!phongcm)
                return ApiResult.NotFound("Xóa các tên môn theo ngành thất bại");
            //delete môn học
            var request = _monhoc.Delete(id, idDonvi);
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
        
    }
}
