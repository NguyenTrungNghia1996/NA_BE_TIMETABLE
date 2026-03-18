using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.IRepository.Auth;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers.Auth
{
    [Route("api/roles")]
    [ApiController]
    public class Auth_RolesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuth_RolesRepository _role;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        public Auth_RolesController(IMapper mapper,
                                    IAuth_RolesRepository role,
                                    IClaimHelperRepository claimHelperRepository,
                                    IAuthRepository auth
                                )
        {
            _role = role;
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
            search = search.Trim();
            var list = _role.GetList_Paging(PageIndex, PageSize, search, ref totalrecord);
            if (list == null) list = [];
            return ApiResult.Success(new
            {
                items = list,
                totalrecord
            },
            "Thành công");
        }
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
            var detailRole = _role.GetDetailByID(id);
            if (detailRole == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {id}");
            var detailRole_Permission = _mapper.Map<Auth_RolesDto>(detailRole);
            var listPerrmission = _role.GetPermissionByRoleId(detailRole_Permission.Id);
            detailRole_Permission.Permission = listPerrmission;
            return ApiResult.Success(detailRole_Permission,
            "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] Auth_RolesDto role)
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
            role.Id = 0;
            var detailRole = _mapper.Map<Auth_Roles>(role);
            // add 
            bool add = _role.Add(detailRole);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            // add Roles_Permission
            if (role.Permission.Count > 0)
            {
                var listPermission = _mapper.Map<List<Auth_Roles_Permissions>>(role.Permission);
                for (int i = 0; i < listPermission.Count; i++)
                {
                    if (listPermission[i].Id != 0)
                    {
                        var exitsPermission = _role.FindPermissionById(listPermission[i].Id);
                        if (exitsPermission == null)
                            return ApiResult.NotFound($"Permission Id {listPermission[i].Id} không tồn tại, vui lòng kiểm tra lại");
                    }
                    listPermission[i].Id_Roles = detailRole.Id;
                }
                var addPermission = _role.AddPermission(listPermission);
                if (!addPermission)
                {
                    role.Permission = [];
                }
            }
            role.Id = detailRole.Id;
            return ApiResult.Success(role, "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] Auth_RolesDto role)
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
            var roledb = _role.GetDetailByID(role.Id);
            if (roledb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại id");
            var detailRole = _mapper.Map<Auth_Roles>(role);
            bool edit = _role.Update(detailRole);
            if (!edit)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            // add Roles_Permission
            if (role.Permission.Count > 0)
            {
                var listPermission = _mapper.Map<List<Auth_Roles_Permissions>>(role.Permission);
                for (int i = 0; i < listPermission.Count; i++)
                {
                    if (listPermission[i].Id != 0)
                    {
                        var exitsPermission = _role.FindPermissionById(listPermission[i].Id);
                        if (exitsPermission == null)
                            return ApiResult.NotFound($"Permission Id {listPermission[i].Id} không tồn tại, vui lòng kiểm tra lại");
                    }
                    listPermission[i].Id_Roles = detailRole.Id;
                }
                var addPermission = _role.AddPermission(listPermission);
                if (!addPermission)
                {
                    role.Permission = [];
                }
            }
            return ApiResult.Success(role, "Cập nhật thành công");
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
            var roledb = _role.GetDetailByID(id);
            if (roledb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var (result, message) = _role.CheckContraints(id);
            if (!result)
            {
                return ApiResult.BadRequest(message);
            }
            var delPermission = _role.DeletePermissionbyRoleId(id);
            if (!delPermission)
                return ApiResult.NotFound("Xóa thất bại, vui lòng kiểm tra lại");
            var request = _role.Deleted(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");

            return ApiResult.Ok("Xóa thành công");
        }
    }
}