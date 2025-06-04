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
        public IActionResult Getlist_Pageing([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
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
            var list = _role.GetList_Paging(PageIndex, PageSize, search, ref totalrecord);
            if (list == null) list = [];
            return ApiResult.Success(new
            {
                items = list,
                totalrecord = totalrecord
            },
            "Thành công");
        }
        [HttpGet("detail")]
        [RequireToken]
        public IActionResult GetDetailByID([FromQuery] int id)
        {
            if (id <= 0)
                return ApiResult.BadRequest($"Id {id} không hợp lệ, vui lòng kiểm tra lại");

            // Lấy bản ghi từ db
            var detailRole = _role.GetDetailByID(id);
            if (detailRole == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {id}");
            return ApiResult.Success(detailRole,
            "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] Auth_Roles role)
        {          
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            role.Id = 0;
            // add 
            bool add = _role.Add(role);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(role,"Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] Auth_Roles role)
        {    
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            var roledb = _role.GetDetailByID(role.Id);
            if (roledb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại id");
            bool edit = _role.Update(role);
            if (!edit)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(role,"Cập nhật thành công");
        }
        [HttpDelete]
        [RequireToken]
        public IActionResult Delete([FromQuery] int id)
        {
            // Kiểm tra bản ghi hợp lệ
            var roledb = _role.GetDetailByID(id);    
            if (roledb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var request = _role.Deleted(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
    }
}