using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;
using NetTopologySuite.Index.HPRtree;

namespace NA_Xepthoikhoabieu.Controllers
{
    [ApiController]
    [Route("api/lophoc")]
    public class DM_LophocController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_LophocRepository _Lophoc;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_BanhocRepository _ban;
        private readonly IDM_CahocRepository _cahoc;
        private readonly IDM_PhonghocRepository _phong;
        private readonly IDM_GiaovienRepository _giaovien;
        private readonly IDM_KhoilopRepository _khoilop;
        private readonly IValidateRepository _validate;
        public DM_LophocController(IMapper mapper, IDM_LophocRepository Lophoc, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                     IDM_GiaovienRepository giaovien, IDM_CahocRepository cahoc, IDM_BanhocRepository ban, IDM_PhonghocRepository phong, IDM_KhoilopRepository khoilop, IValidateRepository validate)
        {
            _mapper = mapper;
            _Lophoc = Lophoc;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _ban = ban;
            _phong = phong;
            _giaovien = giaovien;
            _cahoc = cahoc;
            _khoilop = khoilop;
            _validate = validate;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] int id_khoilop = 0 , [FromQuery] string search = "")
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _Lophoc.GetList_Paging(PageIndex, PageSize, search, idDonvi, id_khoilop, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            var listDto = _mapper.Map<List<DM_Lophoc_ListDto>>(list);
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
            if (Id <= 0)
                return ApiResult.BadRequest($"Id {Id} không hợp lệ, vui lòng kiểm tra lại");
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy bản ghi từ db
            var detail = _Lophoc.getDetailById(Id);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<DM_LophocDto>(detail);
            return ApiResult.Success(detailDto, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_LophocDto Lophoc)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            bool check_env = _claimHelperRepository.IsDemoSite();
            if (check_env)
            {
                if (!_Lophoc.Check_limit(idDonvi))
                    return ApiResult.BadRequest("Bạn đã đạt giới hạn 8 lớp");
            }
            
            // mapper data 
            var addph = _mapper.Map<DM_Lophoc>(Lophoc);
            addph.Id = 0;
            addph.Id_don_vi = idDonvi;

            //kiểm tra id phòng, ban, giáo viên, ca, khối
            var check_phong = _phong.CheckId(Lophoc.Id_phong,idDonvi);
            var check_khoi = _khoilop.CheckKhoilopByDonvi(Lophoc.Id_khoi,idDonvi);
            var check_ban = _ban.CheckId(Lophoc.Id_ban,idDonvi);
            var check_giaovien = _giaovien.CheckId(Lophoc.Id_gvcn,idDonvi);
            var check_ca = _cahoc.CheckId(Lophoc.Id_ca,idDonvi);
            
            if (!check_phong)
                ModelState.AddModelError("Id_phong", "Id phòng học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_khoi)
                ModelState.AddModelError("Id_khoi", "Id khối học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_ban)
                ModelState.AddModelError("Id_ban", "Id ban học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_giaovien)
                ModelState.AddModelError("Id_gvcn", "Id giáo viên không hợp lệ, vui lòng kiểm tra lại");
            if (!check_ca)
                ModelState.AddModelError("Id_ca", "Id ca học không hợp lệ, vui lòng kiểm tra lại");
            bool checkten = _validate.CheckTrungTen_byDonvi<DM_Lophoc>(idDonvi, Lophoc.Ten);
            if (checkten)
            {
                return ApiResult.BadRequest( "Tên lớp học đã tồn tại");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //thêm
            bool add = _Lophoc.Add(addph);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            var itemDto = _mapper.Map<DM_LophocDto>(addph);
            return ApiResult.Success(new
            {
                item = itemDto
            },
            "Thêm mới thành công");

        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_LophocDto Lophoc)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var Lophocdb = _Lophoc.getDetailById(Lophoc.Id);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (Lophocdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            var item = _mapper.Map<DM_Lophoc>(Lophoc);
            item.Id_don_vi = idDonvi;
            //kiểm tra id phòng, ban, giáo viên, ca, khối
            var check_phong = _phong.CheckId(Lophoc.Id_phong, idDonvi);
            var check_khoi = _khoilop.CheckKhoilopByDonvi(Lophoc.Id_khoi, idDonvi);
            var check_ban = _ban.CheckId(Lophoc.Id_ban, idDonvi);
            var check_giaovien = _giaovien.CheckId(Lophoc.Id_gvcn, idDonvi);
            var check_ca = _cahoc.CheckId(Lophoc.Id_ca, idDonvi);

            if (!check_phong)
                ModelState.AddModelError("Id_phong", "Id phòng học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_khoi)
                ModelState.AddModelError("Id_khoi", "Id khối học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_ban)
                ModelState.AddModelError("Id_ban", "Id ban học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_giaovien)
                ModelState.AddModelError("Id_gvcn", "Id giáo viên không hợp lệ, vui lòng kiểm tra lại");
            if (!check_ca)
                ModelState.AddModelError("Id_ca", "Id ca học không hợp lệ, vui lòng kiểm tra lại");
            bool checkten = _validate.CheckTrungTen_byDonvi<DM_Lophoc>(idDonvi, Lophoc.Ten, Lophoc.Id);
            if (checkten)
            {
                return ApiResult.BadRequest("Tên lớp học đã tồn tại");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //update
            bool add = _Lophoc.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            var itemDto = _mapper.Map<DM_LophocDto>(item);
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
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var Lophocdb = _Lophoc.getDetailById(id);
            if (Lophocdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");

            //xóa tiết bận
            var tietban = _Lophoc.DeleteTietBan(id);
            if (!tietban)
                return ApiResult.NotFound("Xóa tiết bận thất bại");
            //xóa phòng
            var request = _Lophoc.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
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
                var detail = _Lophoc.CheckId(Id, idDonvi);
                if (!detail)
                    return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            }
            // Lấy bản ghi từ db
            var result = _Lophoc.GetListTietBan(Id, idDonvi);
            if (result == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            return ApiResult.Success(result, "Thành công");
        }

        [HttpPost("tietnghi")]
        [RequireToken]
        public IActionResult Update([FromBody] Lophoc_banDto lopban)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            // Validate lớp học
            if (!_Lophoc.CheckId(lopban.Id_lop, idDonvi))
                return ApiResult.BadRequest("Lớp học không hợp lệ");

            // check id, check trùng
            var danhSachTietBan = new List<Lophoc_Tietnghi>();
            var errors = new List<string>();
            var existingCombinations = new HashSet<string>();
            foreach (var ca in lopban.Ds_Ca)
            {
                foreach (var ngay in ca.Ds_Ngay)
                {
                    foreach (var tiet in ngay.Ds_Tiet)
                    {
                        var idthu = (int)ngay.Id;
                        var idtiet = (int)tiet.Id;
                        //check các id
                        //bool isValid = _monhoc.CheckIds_Tiet( ca.Id, idDonvi);
                        //if (!isValid)
                        //{
                        //    errors.Add($"Dữ liệu không hợp lệ cho Ca: {ca.Id}, Ngày: {ngay.Id}, Tiết: {tiet.Id}");
                        //    break;
                        //}

                        if (tiet.Trang_thai == true)
                        {
                            //Tạo unique key để check trùng
                            string uniqueKey = $"{lopban.Id_lop}_{ca.Id}_{idthu}_{idtiet}";
                            //kiểm tra unique tồn tại chưa
                            if (existingCombinations.Contains(uniqueKey))
                            {
                                errors.Add($"Trùng lặp bản ghi");
                                continue;
                            }
                            //nếu chưa tồn tại thì thêm vào combinations
                            existingCombinations.Add(uniqueKey);
                            //thêm các tiết trạng thái bằng true vào danh sách tiết bận
                            danhSachTietBan.Add(new Lophoc_Tietnghi
                            {
                                Id_lop = lopban.Id_lop,
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
            bool result = _Lophoc.AddTietBan(danhSachTietBan, lopban.Id_lop);
            if (!result)
                return ApiResult.NotFound("Cập nhật tiết bận thất bại");

            return ApiResult.Success(new { id_lop = lopban.Id_lop, so_tiet_ban = danhSachTietBan.Count }, "Cập nhật tiết bận thành công");
        }
        
    }
}
