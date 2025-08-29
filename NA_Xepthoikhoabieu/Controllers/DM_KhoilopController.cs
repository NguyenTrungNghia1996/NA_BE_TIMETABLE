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
    [Route("api/khoilop")]
    public class DM_KhoilopController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_KhoilopRepository _khoilop;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_CahocRepository _cahocRepository;
        private readonly IDM_CaphocRepository _caphocRepository;
        public DM_KhoilopController(IMapper mapper, IDM_KhoilopRepository khoilop, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, IDM_CaphocRepository caphocRepository)
        {
            _mapper = mapper;
            _khoilop = khoilop;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _caphocRepository = caphocRepository;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {

            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _khoilop.GetList_Paging(PageIndex, PageSize, search, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            var listDto = _mapper.Map<List<DM_Khoilop_ListDto>>(list);
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

            // Lấy bản ghi từ db
            var detail = _khoilop.getDetailById(Id);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<DM_KhoilopDto>(detail);
            return ApiResult.Success(detailDto, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_KhoilopDto khoilop)
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // mapper data 
            var item = _mapper.Map<DM_Khoilop>(khoilop);
            item.Id = 0;
            item.Trang_thai_xoa = false;
            var checkcaphoc = _caphocRepository.CheckId(item.Id_Cap_hoc);
            if (!checkcaphoc)
                ModelState.AddModelError("Id_Caphoc", "Id cấp học không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool add = _khoilop.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            var itemDto = _mapper.Map<DM_KhoilopDto>(item);
            return ApiResult.Success(new
            {
                item = itemDto
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_KhoilopDto khoilop)
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // Kiểm tra bản ghi hợp lệ
            var khoilopdb = _khoilop.getDetailById(khoilop.Id);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (khoilopdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            var item = _mapper.Map<DM_Khoilop>(khoilop);
            var checkcaphoc = _caphocRepository.CheckId(item.Id_Cap_hoc);
            if (!checkcaphoc)
                ModelState.AddModelError("Id_Caphoc", "Id cấp học không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool add = _khoilop.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var itemDto = _mapper.Map<DM_KhoilopDto>(item);
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
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }   // Kiểm tra bản ghi hợp lệ
            var item = _khoilop.getDetailById(id);
            if (item == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var request = _khoilop.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
        [HttpGet("khoiloptheodonvi")]
        [RequireToken]
        public IActionResult GetKhoiLopByID()
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy bản ghi từ db
            var detail = _khoilop.GetKhoilopByDonvi(idDonvi);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho đơn vị có Id= {idDonvi}");
            return ApiResult.Success(detail, "Thành công");
        }
    }
}