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
    [Route("api/tiethoc")]
    public class DM_TiethocController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_TiethocRepository _tiethoc;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_CahocRepository _cahocRepository;
        public DM_TiethocController(IMapper mapper, IDM_TiethocRepository tiethoc, IClaimHelperRepository claimHelperRepository, IAuthRepository auth, IDM_CahocRepository cahocRepository)
        {
            _mapper = mapper;
            _tiethoc = tiethoc;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _cahocRepository = cahocRepository;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _tiethoc.GetList_Paging(PageIndex, PageSize, search, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            var listDto = _mapper.Map<List<DM_Tiethoc_ListDto>>(list);
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
            var detail = _tiethoc.getDetailById(Id);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<DM_TiethocDto>(detail);
            //detailDto.Id_Ca_hoc = _tiethoc.GetlistCabyTiethoc(Id);
            return ApiResult.Success(detailDto, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_TiethocDto tiethoc)
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            
            var item = _mapper.Map<DM_Tiethoc>(tiethoc);
            item.Id = 0;
            if (tiethoc.Id_Ca_hoc == null || tiethoc.Id_Ca_hoc.Count == 0)
            {
                ModelState.AddModelError("Id_Ca_hoc", "Vui lòng chọn ít nhất 1 ca học");
            }
            var checkcahoc = _cahocRepository.CheckIds(tiethoc.Id_Ca_hoc);
            if (!checkcahoc)
                ModelState.AddModelError("Id_Ca_hoc", "Id ca học không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool add = _tiethoc.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            tiethoc.Id = item.Id;
            //var addCa = _tiethoc.AddCa(tiethoc.Id, tiethoc.Id_Ca_hoc);

            //if (!addCa)
            //    return ApiResult.Success(new
            //    {
            //        itemSave = tiethoc
            //    },
            //    "Tạo đơn vị thành công, lưu cấp học thất bại");
            return ApiResult.Success(new
            {
                itemSave = tiethoc
            }, "Tạo đơn vị thành công");
        
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_TiethocDto tiethoc)
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // Kiểm tra bản ghi hợp lệ
            var tiethocdb = _tiethoc.getDetailById(tiethoc.Id);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (tiethocdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            if (tiethoc.Id_Ca_hoc == null || tiethoc.Id_Ca_hoc.Count == 0)
            {
                ModelState.AddModelError("Id_Ca_hoc", "Vui lòng chọn ít nhất 1 ca học");
            }
            var item = _mapper.Map<DM_Tiethoc>(tiethoc);
            var checkcahoc = _cahocRepository.CheckIds(tiethoc.Id_Ca_hoc);
            if (!checkcahoc)
                ModelState.AddModelError("Id_Ca_hoc", "Id ca học không hợp lệ, vui lòng kiểm tra lại");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool add = _tiethoc.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            //var editCa = _tiethoc.UpdateCa(tiethoc.Id, tiethoc.Id_Ca_hoc);

            //if (!editCa)
            //    return ApiResult.Success(new
            //    {
            //        itemSave = tiethoc
            //    },
            //    "Cập nhật đơn vị thành công, cập nhật cấp học thất bại");

            return ApiResult.Success(new
            {
                itemSave = tiethoc
            }, "Cập nhật đơn vị thành công");
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
            }
            // Kiểm tra bản ghi hợp lệ
            var item = _tiethoc.getDetailById(id);
            if (item == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var request = _tiethoc.Delete(id);
            //var deleteCa = _tiethoc.DeleteCa(id);
            //if (!deleteCa)
            //    return ApiResult.NotFound("Xóa các ca học lỗi");
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            
            return ApiResult.Ok("Xóa tiết học thành công");
        }
    }
}