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
    [Route("api/phonghoc")]
    public class DM_PhonghocController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_PhonghocRepository _phonghoc;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_LoaiphonghocRepository _loaiphonghoc;
        private readonly IDM_DiemtruongRepository _diemtruong;
        private readonly IDM_CahocRepository _cahoc;
        private readonly IDM_NgayhocRepository _ngayhoc;
        private readonly IDM_TiethocRepository _tiethoc;
        private readonly INgay_DonviRepository _ngaydv;
        public DM_PhonghocController(IMapper mapper, IDM_PhonghocRepository phonghoc, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                     IDM_LoaiphonghocRepository loaiphonghoc, IDM_DiemtruongRepository diemtruong, IDM_CahocRepository cahoc, IDM_NgayhocRepository ngayhoc,
                                     IDM_TiethocRepository tiethoc, INgay_DonviRepository ngaydv)
        {
            _mapper = mapper;
            _phonghoc = phonghoc;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _loaiphonghoc = loaiphonghoc;
            _diemtruong = diemtruong;
            _cahoc = cahoc;
            _ngayhoc = ngayhoc;
            _tiethoc = tiethoc;
            _ngaydv = ngaydv;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "", [FromQuery] int idDiemTruong=0)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _phonghoc.GetList_Paging(PageIndex, PageSize, search, idDiemTruong, idDonvi, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            var listDto = _mapper.Map<List<DM_Phonghoc_listDto>>(list);
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
            var detail = _phonghoc.getDetailById(Id);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<DM_PhonghocDto>(detail);
            return ApiResult.Success(detailDto, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_PhonghocDto phonghoc)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // mapper data 
            var addph = _mapper.Map<DM_Phonghoc>(phonghoc);
            addph.Id = 0;

            //kiểm tra mã môn học
            var check_ma = _phonghoc.CheckMa(phonghoc.Ma);
            if (!check_ma)
                ModelState.AddModelError("Ma", "Mã phòng học đã trùng, vui lòng kiểm tra lại");
            //kiểm tra id loại phòng và điểm trường
            var check_loaiphonghoc = _loaiphonghoc.CheckId(phonghoc.Id_Loai_phong_hoc);
            var check_diemtruong = _diemtruong.CheckId(phonghoc.Id_Diem_truong, idDonvi);
            if (!check_loaiphonghoc)
                ModelState.AddModelError("Id_Loai_phong_hoc", "Id loại phòng học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_diemtruong)
                ModelState.AddModelError("Id_Diem_truong", "Id điểm trường không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //thêm
            bool add = _phonghoc.Add(addph);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            var itemDto = _mapper.Map<DM_PhonghocDto>(addph);
            return ApiResult.Success(new
            {
                item = itemDto
            },
            "Thêm mới thành công");

        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_PhonghocDto phonghoc)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra bản ghi hợp lệ
            var phonghocdb = _phonghoc.getDetailById(phonghoc.Id);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (phonghocdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            var item = _mapper.Map<DM_Phonghoc>(phonghoc);

            //kiểm tra mã phòng học
            var check_ma = _phonghoc.CheckMa(phonghoc.Ma);
            if (!check_ma)
                ModelState.AddModelError("Ma", "Mã phòng học đã trùng, vui lòng kiểm tra lại");
            //kiểm tra id loại phòng và điểm trường
            var check_loaiphonghoc = _loaiphonghoc.CheckId(phonghoc.Id_Loai_phong_hoc);
            var check_diemtruong = _diemtruong.CheckId(phonghoc.Id_Diem_truong, idDonvi);
            if (!check_loaiphonghoc)
                ModelState.AddModelError("Id_Loai_phong_hoc", "Id loại phòng học không hợp lệ, vui lòng kiểm tra lại");
            if (!check_diemtruong)
                ModelState.AddModelError("Id_Diem_truong", "Id điểm trường không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //update
            bool add = _phonghoc.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            var itemDto = _mapper.Map<DM_PhonghocDto>(item);
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
            var phonghocdb = _phonghoc.getDetailById(id);
            if (phonghocdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");

            //xóa tiết bận
            var tietban = _phonghoc.DeleteTietBan(id);
            if (!tietban)
                return ApiResult.NotFound("Xóa tiết bận thất bại");
            //xóa phòng
            var request = _phonghoc.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }

        //[HttpGet("tietban")]
        //[RequireToken]
        //public IActionResult GetListTietBan([FromQuery] int Id)
        //{
        //    if (Id < 0)
        //        return ApiResult.BadRequest($"Id {Id} không hợp lệ, vui lòng kiểm tra lại");

        //    // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
        //    int idDonvi = _claimHelperRepository.GetIdDonvi(User);
        //    if (idDonvi == 0)
        //        return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
        //    if (Id > 0)
        //    {
        //        var detail = _phonghoc.CheckId(Id, idDonvi);
        //        if (!detail)
        //            return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
        //    }
        //    // Lấy bản ghi từ db
        //    var result = _phonghoc.GetListTietBan(Id,idDonvi);

        //    if (result == null)
        //        return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");

        //    return ApiResult.Success(result, "Thành công");
        //}

        [HttpPost("tietban")]
        [RequireToken]
        public IActionResult Update([FromBody] Phong_banDto phongban)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0)
                return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            // Validate phòng học
            if (!_phonghoc.CheckId(phongban.Id))
                return ApiResult.BadRequest("Phòng học không hợp lệ");

            // check id, check trùng
            var danhSachTietBan = new List<Tiet_ban>();
            var errors = new List<string>();
            var existingCombinations = new HashSet<string>();
            foreach (var ca in phongban.Ds_Ca)
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
                            string uniqueKey = $"{phongban.Id}_{ca.Id}_{idthu}_{idtiet}";
                            //kiểm tra unique tồn tại chưa
                            if (existingCombinations.Contains(uniqueKey))
                            {
                                errors.Add($"Trùng lặp bản ghi");
                                continue;
                            }
                            //nếu chưa tồn tại thì thêm vào combinations
                            existingCombinations.Add(uniqueKey);
                            //thêm các tiết trạng thái bằng true vào danh sách tiết bận
                            danhSachTietBan.Add(new Tiet_ban
                            {
                                Id_phong = phongban.Id,
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
            bool result = _phonghoc.AddTietBan(danhSachTietBan,phongban.Id);
            if (!result)
                return ApiResult.NotFound("Cập nhật tiết bận thất bại");

            return ApiResult.Success(new { id_phong = phongban.Id, so_tiet_ban = danhSachTietBan.Count }, "Cập nhật tiết bận thành công");
        }
    }
}
