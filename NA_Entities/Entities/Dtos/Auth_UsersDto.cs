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
        [Required(ErrorMessage = "Tên trường không được để trống")]
        public string Ten_truong { get; set; }
        [Required(ErrorMessage = "Tỉnh không được để trống")]
        public int Id_tinh { get; set; }
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Dia_chi { get; set; }
        [Required(ErrorMessage = "Người liên hệ không được để trống")]
        public string Nguoi_lien_he { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Chỉ được nhập số")]
        public string So_dien_thoai { get; set; }
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Tên tài khoản không được để trống")]
        public string Ten_tai_khoan { get; set; }
        public List<int> Id_ca { get; set; } = new List<int>();
        public List<int> Id_cap { get; set; } = new List<int>();
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Mat_khau { get; set; }
        [Required(ErrorMessage = "Nhập lại mật khẩu không được để trống")]
        public string Nhap_lai_mat_khau { get; set; }
    }
}