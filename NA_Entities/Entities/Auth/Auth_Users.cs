using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NA_Entities.Entities.Auth
{
    public class Auth_Users
    {
        [Key]
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Tên đầy đủ không được để trống")]
        [StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string Hoten { get; set; } = string.Empty;
        public DateTime? Ngaysinh { get; set; }
        public int? Id_Gioitinh { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn đơn vị")]
        public int Id_Donvi { get; set; }
        public int? Id_Chucvu { get; set; }
        public bool IsActive { get; set; }
    }
    public class Auth_Users_List
    {
        public int Stt { get; set; } = 0;
        public int Id { get; set; } = 0;
        public string Username { get; set; } = string.Empty;
        public string Hoten { get; set; } = string.Empty;
        public DateTime? Ngaysinh { get; set; }
        public int? Id_Gioitinh { get; set; }
        public string Tengioitinh { get; set; } = string.Empty;
        public int Id_Donvi { get; set; }
        public string Tendonvi { get; set; } = string.Empty;
        public int? Id_Chucvu { get; set; }
        public string Tenchucvu { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}