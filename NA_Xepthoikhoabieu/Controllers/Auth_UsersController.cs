using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class Auth_UsersController : ControllerBase
    {
        private readonly IAuthRepository _auth;
        private readonly IPasswordHasherRepository _passwordHasher;
        private readonly IJwtHelperRepository _jwtHelperRepository;
        private readonly IMapper _mapper;
        private readonly IDM_DonviRepository _donviRepository;
        private readonly IClaimHelperRepository _claimHelperRepository;
        public Auth_UsersController(IAuthRepository auth,
                                    IJwtHelperRepository jwtHelperRepository,
                                    IPasswordHasherRepository passwordHasher,
                                    IMapper mapper,
                                    IClaimHelperRepository claimHelperRepository,
                                    IDM_DonviRepository donviRepository
                                    )
        {
            _auth = auth;
            _jwtHelperRepository = jwtHelperRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _claimHelperRepository = claimHelperRepository;
            _donviRepository = donviRepository;

        }

        // Get list users paging
        [HttpGet]
        [RequireToken]
        public IActionResult GetlistUsers_Pageing([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            // kiểm tra tính hợp lệ của thông tin trong token
            var check = _claimHelperRepository.CheckUserExists(User);
            if (check == false) return ApiResult.Unauthorized("Thông tin user không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            if (PageIndex < 1 || PageSize < 1)
                return ApiResult.BadRequest($"PageIndex hoặc PageSize không hợp lệ, vui lòng kiểm tra lại (PageIndex >= 1; PageSize >= 1)");
            int totalrecord = 0;
            var list = _auth.GetListUsers_Paging(PageIndex, PageSize, search, ref totalrecord);
            return ApiResult.Success(new
            {
                items = list,
                totalrecord = totalrecord
            },
            "Lấy danh sách người dùng thành công");
        }
        // Get detail user by id
        [HttpGet("detail")]
        [RequireToken]
        public IActionResult DetailUser_ById([FromQuery] int id)
        {
            // kiểm tra tính hợp lệ của thông tin trong token
            var check = _claimHelperRepository.CheckUserExists(User);
            if (check == false) return ApiResult.Unauthorized("Thông tin user không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            int idUser = _claimHelperRepository.GetUserId(User);
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            // Lấy bản ghi từ db
            var detailUser = _auth.FindUserById(id);
            if (detailUser == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {id}");
            var detailDto = _mapper.Map<Auth_UsersDto>(detailUser);
            detailDto.IdRoles = _auth.GetListIdRolesByUser(id);
            return ApiResult.Success(detailDto,
            "Thành công");
        }
        // Create new user
        [HttpPost]
        [RequireToken]
        public IActionResult CreateUser([FromBody] Auth_UsersDto user)
        {
            // kiểm tra tính hợp lệ của thông tin trong token
            var check = _claimHelperRepository.CheckUserExists(User);
            if (check == false) return ApiResult.Unauthorized("Thông tin user không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // Check validation
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (_auth.FindUserByName(user.Username) != null)
                return ApiResult.NotFound("Tên đăng nhập đã tồn tại");
            if (user.IdRoles == null || user.IdRoles.Count == 0)
            {
                return ApiResult.NotFound("Vui lòng chọn ít nhất 1 nhóm quyền");
            }
            if (!_auth.checkRolesExist(user.IdRoles))
                return ApiResult.NotFound("Danh sách id nhóm quyền không hợp lệ, vui lòng chọn id tồn tại");
            // mapping data to Auth_Users
            if (_donviRepository.getDonviById(user.Id_Donvi) == null)
                return ApiResult.NotFound($"Id đơn vị = {user.Id_Donvi} không tồn tại");
            var addUser = _mapper.Map<Auth_Users>(user);
            addUser.Id = 0;
            // thêm tài khoản
            var request = _auth.CreateUser(addUser);
            if (!request)
                return ApiResult.NotFound("Thêm mới user lỗi");
            user.Id = addUser.Id; // gán id trở lại sau khi tạo

            var addGroup = _auth.AddUserToRoles(user.Id, user.IdRoles);
            if (!addGroup)
                return ApiResult.Success(new
                {
                    item = user
                },
                "Tạo tài khoản thành công, lưu nhóm quyền thất bại");
            return ApiResult.Success(new
            {
                item = user
            }, "Tạo tài khoản thành công");
        }
        // Update new user
        [HttpPut]
        [RequireToken]
        public IActionResult UpdateUser([FromBody] Auth_Users_UpdateDto user)
        {
            // kiểm tra tính hợp lệ của thông tin trong token
            var check = _claimHelperRepository.CheckUserExists(User);
            if (check == false) return ApiResult.Unauthorized("Thông tin đăng nhập không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // Check validation
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            var detailUser = _auth.FindUserById(user.Id);
            if (detailUser == null) return ApiResult.NotFound($"Không tồn tại user có id = {user.Id}");
            if (user.IdRoles == null || user.IdRoles.Count == 0)
            {
                return ApiResult.NotFound("Vui lòng chọn ít nhất 1 nhóm quyền");
            }
            if (!_auth.checkRolesExist(user.IdRoles))
                return ApiResult.NotFound("Danh sách id nhóm quyền không hợp lệ, vui lòng chọn id tồn tại");
            if (_donviRepository.getDonviById(user.Id_Donvi) == null)
                return ApiResult.NotFound($"Id đơn vị = {user.Id_Donvi} không tồn tại");
            // mapping data to Auth_Users
            var editUser = _mapper.Map<Auth_Users>(user);
            editUser.Username = detailUser.Username;
            // thêm tài khoản
            var request = _auth.UpdateUser(editUser);
            if (!request)
                return ApiResult.NotFound("Thêm mới user lỗi");
            // edit nhóm quyền          
            var editGroup = _auth.UpdateUserToRoles(user.Id, user.IdRoles);
            if (!editGroup)
                return ApiResult.Success(new
                {
                    item = user
                },
                "Cập nhật thông tin tài khoản thành công, lưu nhóm quyền thất bại");
            return ApiResult.Success(new
            {
                item = user
            }, "Cập nhật tài khoản thành công");
        }
        // Update new user
        [HttpDelete]
        [RequireToken]
        public IActionResult DeleteUser([FromQuery] int id)
        {
            // kiểm tra tính hợp lệ của thông tin trong token
            var check = _claimHelperRepository.CheckUserExists(User);
            if (check == false) return ApiResult.Unauthorized("Thông tin đăng nhập không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            // Check validation
            var detailUser = _auth.FindUserById(id);
            if (detailUser == null) return ApiResult.NotFound($"Không tồn tại user có id = {id}");

            // Xóa nhóm quyền
            var request = _auth.DeleteUser(id);
            if (!request)
                return ApiResult.NotFound("Xóa tài khoản lỗi");
            // edit nhóm quyền          
            var deleteRoles = _auth.DeleteUsers_Roles(id);
            if (!deleteRoles)
                return ApiResult.NotFound("Xóa nhóm quyền tài khoản lỗi");
            return ApiResult.Ok("Xóa tài khoản thành công");
        }

        //Login
        [HttpPost("login")]
        public IActionResult Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = _auth.FindUserByName(loginDto.Username);
            if (user == null)
            {
                return ApiResult.NotFound("Tên đăng nhập không tồn tại");
            }
            if (user.IsActive != true)
            {
                return ApiResult.Forbidden("Tài khoản chưa kích hoạt");
            }

            var validPassword = _passwordHasher.VerifyPassword(user.Password, loginDto.Password);
            if (!validPassword)
                return ApiResult.Unauthorized("Mật khẩu không chính xác");
            // Nếu đăng nhập thành công, tạo JWT token
            var toke = _jwtHelperRepository.GenerateJwtToken(user.Id.ToString(), user.Id_Donvi.ToString());
            var userDto = _mapper.Map<UserDto>(user);
            return ApiResult.Success(new
            {
                item = userDto,
                token = toke,
            }, "Đăng nhập thành công");
        }
        //
    }
}
