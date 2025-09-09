using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Xepthoikhoabieu.Helpers;
using System.ComponentModel.DataAnnotations;

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
        private readonly IAuth_RolesRepository _rolesRepository;
        private readonly IDM_DonviRepository _donvi;
        private readonly IDM_CaphocRepository _caphocRepository;
        private readonly IDM_CahocRepository _cahocRepository;
        public Auth_UsersController(IAuthRepository auth,
                                    IJwtHelperRepository jwtHelperRepository,
                                    IPasswordHasherRepository passwordHasher,
                                    IMapper mapper,
                                    IClaimHelperRepository claimHelperRepository,
                                    IDM_DonviRepository donviRepository, 
                                    IAuth_RolesRepository rolesRepository,
                                    IDM_DonviRepository donvi,
                                    IDM_CahocRepository cahoc,
                                    IDM_CaphocRepository caphoc
                                    )
        {
            _auth = auth;
            _jwtHelperRepository = jwtHelperRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _claimHelperRepository = claimHelperRepository;
            _donviRepository = donviRepository;
            _rolesRepository = rolesRepository;
            _donvi = donvi;
            _cahocRepository = cahoc;
            _caphocRepository = caphoc;

        }

        // Get list users paging
        [HttpGet]
        [RequireToken]
        public IActionResult GetlistUsers_Pageing([FromQuery] int PageIndex = 0, [FromQuery] int PageSize = 0, [FromQuery] string search = "")
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            // kiểm tra nếu là admin thì được truy cập
            bool checkIsAdmin = _auth.checkIsAdmin(idUser);
            if (!checkIsAdmin)
            {
                return ApiResult.Forbidden("Không có quyền truy cập, vui lòng liên hệ admin");
            }
            int totalrecord = 0;
            search = search.Trim();
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
            int idUser = _claimHelperRepository.GetUserId(User);
            if (idUser <= 0)
                return ApiResult.Unauthorized($"Thông tin user id = {idUser} không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
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
            if (_donviRepository.getDetailById(user.Id_Donvi) == null)
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
        [HttpPost("register")]
        public IActionResult Register([FromBody] Register user)
        {
            bool check_env = _claimHelperRepository.IsDemoSite();
            if (!check_env)
                return ApiResult.BadRequest("Chỉ site demo mới được đăng ký");
            // Check validation
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (_auth.FindUserByName(user.Ten_tai_khoan) != null)
                return ApiResult.NotFound("Tên đăng nhập đã tồn tại");
            if (user.Mat_khau != user.Nhap_lai_mat_khau)
            {
                return ApiResult.NotFound("Mật khẩu không trùng khớp");
            }

            var donvi = new DM_Donvi()
            {
                Id = 0,
                TenDonvi = user.Ten_truong,
                Diachi = user.Dia_chi,
                Nguoi_lien_he = user.Nguoi_lien_he,
                Id_tinh = user.Id_tinh,
                Sodienthoai = user.So_dien_thoai,
                Email = user.Email
            };
            //check chọn ca, cấp
            if (user.Id_cap == null || user.Id_cap.Count == 0)
            {
                ModelState.AddModelError("Id_cap", "Vui lòng chọn ít nhất 1 cấp học");
            }
            if (user.Id_ca == null || user.Id_ca.Count == 0)
            {
                ModelState.AddModelError("Id_ca", "Vui lòng chọn ít nhất 1 ca học");
            }
            //check id ca, cấp
            var checkcaphoc = _caphocRepository.CheckIds(user.Id_cap);
            var checkcahoc = _cahocRepository.CheckIds(user.Id_ca);
            if (!checkcaphoc)
                ModelState.AddModelError("Id_cap", "Id cấp học không hợp lệ, vui lòng kiểm tra lại");
            if (!checkcahoc)
                ModelState.AddModelError("Id_ca", "Id ca học không hợp lệ, vui lòng kiểm tra lại");
            //hiển thị lỗi
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool addDonvi = _donvi.Add(donvi);
            if (!addDonvi) {
                return ApiResult.BadRequest("Thêm thông tin đơn vị thất bại");
            }
            var addCapDonvi = _donvi.AddCap(donvi.Id, user.Id_cap);
            var addCaDv = _donvi.AddCa(donvi.Id, user.Id_ca);
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
            var authUser = new Auth_Users()
            {
                Id = 0,
                Username = user.Ten_tai_khoan,
                Hoten = user.Ten_truong,
                Password = user.Mat_khau,
                Id_Donvi = donvi.Id,
                IsActive = true,
                IsAdmin = false
            };

            // thêm tài khoản
            var request = _auth.RegisterUser(authUser);
            if (!request)
                return ApiResult.NotFound("Thêm mới user lỗi");
            List<int> Idroles = new List<int> { 5 };
            var addGroup = _auth.AddUserToRoles(authUser.Id, Idroles);
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
            if (_donviRepository.getDetailById(user.Id_Donvi) == null)
                return ApiResult.NotFound($"Id đơn vị = {user.Id_Donvi} không tồn tại");
            // mapping data to Auth_Users
            var editUser = _mapper.Map<Auth_Users>(user);
            editUser.Username = detailUser.Username;
            editUser.Password = detailUser.Password;
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
        //
        // Update new user
        [HttpDelete]
        [RequireToken]
        public IActionResult DeleteUser([FromQuery] int id)
        {
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
            var (success, message) = _auth.DeleteUsers_Roles(id);
            if (!success)
                return ApiResult.BadRequest(message);
            var request = _auth.DeleteUser(id);
            if (!request)
                return ApiResult.NotFound("Xóa tài khoản lỗi");
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
        //get permission
        [HttpGet("permission")]
        [RequireToken]
        public IActionResult GetPermission ()
        {
            int idUser = _claimHelperRepository.GetUserId(User);
            if (idUser <= 0)
                return ApiResult.Unauthorized($"Thông tin user id = {idUser} không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            // Lấy bản ghi từ db
            var detailPerbyUser = _rolesRepository.GetPermissionsByUserId(idUser);
            if (detailPerbyUser == null)
                return ApiResult.NotFound($"Không tìm thấy quyền nào nào cho Id= {idUser}");
            var detailDto = new Auth_PermissionDto
            {
                Permission = detailPerbyUser
            };
            return ApiResult.Success(detailDto,
            "Thành công");
        }
    }
}
