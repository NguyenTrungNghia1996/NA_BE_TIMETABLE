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

namespace NA_Xepthoikhoabieu.Controllers
{
    [ApiController]
    [Route("api/donvi")]
    public class DM_DonviController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_DonviRepository _donvi;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_CaphocRepository _caphocRepository;
        private readonly IDM_NgayhocRepository _ngayhocRepository;
        private readonly IDM_CahocRepository _cahocRepository;
        public DM_DonviController(IMapper mapper, IDM_DonviRepository donvi, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,  
            IDM_CaphocRepository caphocRepository, IDM_NgayhocRepository ngayhocRepository, IDM_CahocRepository cahocRepository)
        {
            _mapper = mapper;
            _donvi = donvi;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _caphocRepository = caphocRepository;
            _ngayhocRepository = ngayhocRepository;
            _cahocRepository = cahocRepository;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _donvi.GetList_Paging(PageIndex, PageSize, search, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            var listDto = _mapper.Map<List<DM_Donvi_List_Dto>>(list);
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

            // Lấy bản ghi từ db
            var detail = _donvi.getDetailById(Id);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<DM_DonviDto>(detail);
            detailDto.IdCap = _donvi.GetlistCapbyDonvi(Id);
            detailDto.Id_cahoc = _donvi.GetlistCabyDonvi(Id);
            return ApiResult.Success(detailDto, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_DonviDto donvi)
        {

            int idUser = _claimHelperRepository.GetUserId(User);
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }

            // mapper data 
            var addDonvi = _mapper.Map<DM_Donvi>(donvi);
            addDonvi.Id = 0;
            addDonvi.Trang_thai_xoa = false;
            //check chọn ca, cấp
            if (donvi.IdCap == null || donvi.IdCap.Count == 0)
            {
                ModelState.AddModelError("IdCap", "Vui lòng chọn ít nhất 1 cấp học");
            }
            if (donvi.Id_cahoc == null || donvi.Id_cahoc.Count == 0)
            {
                ModelState.AddModelError("Id_ca_hoc", "Vui lòng chọn ít nhất 1 ca học");
            }
            //check id ca, cấp
            var checkcaphoc = _caphocRepository.CheckIds(donvi.IdCap);
            var checkcahoc = _cahocRepository.CheckIds(donvi.Id_cahoc);
            if (!checkcaphoc)
                ModelState.AddModelError("IdCap", "Id cấp học không hợp lệ, vui lòng kiểm tra lại");
            if (!checkcahoc)
                ModelState.AddModelError("Id_cahoc", "Id ca học không hợp lệ, vui lòng kiểm tra lại");
            bool checkten = _donvi.CheckTrungTen(donvi.TenDonvi);
            if (checkten)
            {
                ModelState.AddModelError("Ten", "Tên đơn vị đã tồn tại");
            }
            //hiển thị lỗi
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //add
            bool add = _donvi.Add(addDonvi);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            donvi.Id = addDonvi.Id;
            var addCapDonvi = _donvi.AddCap(donvi.Id, donvi.IdCap);
            var addCaDv = _donvi.AddCa(donvi.Id, donvi.Id_cahoc);
            if (!addCapDonvi)
                return ApiResult.Success(new
                {
                    item = donvi
                },
                "Tạo đơn vị thành công, lưu cấp học thất bại");
            if (!addCaDv)
                return ApiResult.Success(new
                {
                    item = donvi
                },
                "Tạo đơn vị thành công, lưu ca học thất bại");
            return ApiResult.Success(new
            {
                item = donvi
            }, "Tạo đơn vị thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_Donvi_updateDto donvi)
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // Kiểm tra bản ghi hợp lệ
            var donvidb = _donvi.getDetailById(donvi.Id);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (donvidb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            //check chọn ca, cấp
            if (donvi.IdCap == null || donvi.IdCap.Count == 0)
            {
                ModelState.AddModelError("IdCap", "Vui lòng chọn ít nhất 1 cấp học");
            }
            if (donvi.Id_cahoc == null || donvi.Id_cahoc.Count == 0)
            {
                ModelState.AddModelError("Id_ca_hoc", "Vui lòng chọn ít nhất 1 ca học");
            }

            var item = _mapper.Map<DM_Donvi>(donvi);

            //check id ca, cấp
            var checkcaphoc = _caphocRepository.CheckIds(donvi.IdCap);
            var checkcahoc = _cahocRepository.CheckIds(donvi.Id_cahoc);

            if (!checkcaphoc)
                ModelState.AddModelError("IdCap", "Id cấp học không hợp lệ, vui lòng kiểm tra lại");
            if (!checkcahoc)
                ModelState.AddModelError("Id_ca_hoc", "Id ca học không hợp lệ, vui lòng kiểm tra lại");
            bool checkten = _donvi.CheckTrungTen(donvi.TenDonvi, donvi.Id);
            if (checkten)
            {
                ModelState.AddModelError("Ten", "Tên đơn vị đã tồn tại");
            }
            //hiển thị thông báo lỗi
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //update
            bool update = _donvi.Update(item);
            if (!update)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var editCap = _donvi.UpdateCap(donvi.Id, donvi.IdCap);
            var editCa = _donvi.UpdateCa(donvi.Id, donvi.Id_cahoc);
            if (!editCap)
                return ApiResult.Success(new
                {
                    item = donvi
                },
                "Cập nhật đơn vị thành công, cập nhật cấp học thất bại");
            if (!editCa)
                return ApiResult.Success(new
                {
                    item = donvi
                },
                "Cập nhật đơn vị thành công, cập nhật ca học thất bại");

            return ApiResult.Success(new
            {
                item = donvi
            }, "Cập nhật đơn vị thành công");
        }
        [HttpDelete]
        [RequireToken]
        public IActionResult Delete([FromQuery] int id)
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // Kiểm tra bản ghi hợp lệ
            var donvidb = _donvi.getDetailById(id);
            if (donvidb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");

            var request = _donvi.Delete(id);
            var deleteCap = _donvi.DeleteCap(id);
            var deleteCa = _donvi.DeleteCa(id);
            //xóa
            if (!deleteCap)
                return ApiResult.NotFound("Xóa các cấp học lỗi");
            if (!deleteCa)
                return ApiResult.NotFound("Xóa các ca học lỗi");
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            
            return ApiResult.Ok("Xóa đơn vị thành công");
        }
        
    }
}