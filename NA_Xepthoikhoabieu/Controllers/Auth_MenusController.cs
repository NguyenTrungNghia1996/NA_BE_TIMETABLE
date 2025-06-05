using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Auth;
using NA_Logic.IRepository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/menus")]
    [ApiController]
    public class Auth_MenusController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuth_MenusRepository _menus;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        public Auth_MenusController(IMapper mapper,
                                    IAuth_MenusRepository menus,
                                    IClaimHelperRepository claimHelperRepository,
                                    IAuthRepository auth
                                )
        {
            _menus = menus;
            _mapper = mapper;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
        }
        // Get list Roles paging
        [HttpGet]
        [RequireToken]
        public IActionResult Getlist_Pageing([FromQuery] int PageIndex = 0, [FromQuery] int PageSize = 0, [FromQuery] string search = "")
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _menus.GetList_Pagging(PageIndex, PageSize, search, ref totalrecord);
            if (list == null) list = [];
            return ApiResult.Success(new
            {
                items = list,
                totalrecord = totalrecord
            },
            "Thành công");
        }
        // Detail by Id
        [HttpGet("detail")]
        [RequireToken]
        public IActionResult GetDetailByID([FromQuery] int id)
        {
            if (id <= 0)
                return ApiResult.BadRequest($"Id {id} không hợp lệ, vui lòng kiểm tra lại");
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }

            // Lấy bản ghi từ db
            var detailMenu = _menus.GetDetailByID(id);
            if (detailMenu == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {id}");
            return ApiResult.Success(detailMenu,
            "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] Auth_Menus menu)
        {
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // kiểm tra tồn tại bản ghi cha
            if (menu.Parent_Id > 0)
            {
                var parent = _menus.GetDetailByID(menu.Parent_Id.Value);
                if (parent == null)
                    return ApiResult.NotFound($"Không tìm thấy bản ghi cha có Id= {menu.Parent_Id}, vui lòng kiểm tra lại");
            }
            menu.Id = 0;
            // add 
            bool add = _menus.Add(menu);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(menu, "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] Auth_Menus menu)
        {
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            var menudb = _menus.GetDetailByID(menu.Id);
            if (menudb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại id");
            // kiểm tra tồn tại bản ghi cha
            if (menu.Parent_Id > 0)
            {
                var parent = _menus.GetDetailByID(menu.Parent_Id.Value);
                if (parent == null)
                    return ApiResult.NotFound($"Không tìm thấy bản ghi cha có Id= {menu.Parent_Id}, vui lòng kiểm tra lại");
            }
            bool edit = _menus.Update(menu);
            if (!edit)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            // add Roles_Permission
            return ApiResult.Success(menu, "Cập nhật thành công");
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
            var roledb = _menus.GetDetailByID(id);
            if (roledb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var request = _menus.Deleted(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");

            return ApiResult.Ok("Xóa thành công");
        }
    }
}