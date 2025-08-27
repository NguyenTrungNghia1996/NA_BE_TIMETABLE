using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using NA_Entities.Entities.Auth;

namespace NA_Entities.Entities.Dtos
{
    public class Auth_UsersDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Tên đầy đủ không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Hoten { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn đơn vị")]
        public int Id_Donvi { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAdmin { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một nhóm người dùng")]
        public List<int> IdRoles { get; set; } = new List<int>();

    }
    public class Auth_Users_UpdateDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên đầy đủ không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Hoten { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn đơn vị")]
        public int Id_Donvi { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAdmin { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một nhóm người dùng")]
        public List<int> IdRoles { get; set; } = new List<int>();
    }
    public class Auth_PermissionDto
    {
        public List<Auth_Roles_PermissionDto> Permission { get; set; } = new List<Auth_Roles_PermissionDto>();
    }
    public class Register { 
        public string Ten_truong { get; set; }
        public string Dia_chi { get; set; }
    }
}