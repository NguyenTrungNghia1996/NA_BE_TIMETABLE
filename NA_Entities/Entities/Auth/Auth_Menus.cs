using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Auth
{
    public class Auth_Menus
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        public int Id_Parent { get; set; } = 0;
        public int Thutu { get; set; } = 0;
        public string? Url { get; set; } = string.Empty;
        public bool? Is_Active { get; set; }
        public string? Mota { get; set; } = string.Empty;
    }
    public class Auth_MenusList
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        public int Id_Parent { get; set; } = 0;
        public int Thutu { get; set; } = 0;
        public string? Url { get; set; } = string.Empty;
        public bool? Is_Active { get; set; }
        public string? Mota { get; set; } = string.Empty;
    }
}