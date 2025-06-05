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
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Key không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Key { get; set; } = string.Empty;
        public string? Url { get; set; } = string.Empty;
        public string? Icon { get; set; } = string.Empty;
        public int? Parent_Id { get; set; }
        [Required(ErrorMessage = "Vị trí bit không được để trống")]
        public int PermissionBit { get; set; } = 0;
    }
    public class Auth_MenusList
    {
        public int? Stt { get; set; } = 0;
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string? Url { get; set; } = string.Empty;
        public string? Icon { get; set; } = string.Empty;
        public int? Parent_Id { get; set; }
        public int PermissionBit { get; set; } = 0;
    }
}