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

        public Auth_UsersController(IAuthRepository auth,
                                    IJwtHelperRepository jwtHelperRepository,
                                    IPasswordHasherRepository passwordHasher,
                                    IMapper mapper
                                    )
        {
            _auth = auth;
            _jwtHelperRepository = jwtHelperRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;

        }

        // Get list users paging
        [HttpGet]
        [RequireToken]
        public IActionResult GetlistUsers_Pageing([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
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
        [HttpGet("{id}")]
        public IActionResult DetailUser_ById(int id)
        {
            return Ok();
        }
        // Create new user
        [HttpPost]
        public IActionResult CreateUser(object user)
        {
            return Ok();
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
