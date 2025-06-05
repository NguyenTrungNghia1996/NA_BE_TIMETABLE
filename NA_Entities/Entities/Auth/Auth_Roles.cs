using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Auth
{
    public class Auth_Roles
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên nhóm không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        public string? Mota { get; set; } = "";
    }
    public class Auth_RolesList
    {
        public int? Stt { get; set; } = 0;
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public string? Mota { get; set; } = "";
    }
}
