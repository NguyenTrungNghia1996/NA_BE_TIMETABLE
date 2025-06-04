using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class Auth_RolesDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên nhóm không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        public string? Mota { get; set; }
        public List<Auth_Roles_Menus> listMenus { get; set; } = new List<Auth_Roles_Menus>();
    }
    public class Auth_Roles_Menus {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public int Id_Parent { get; set; } = 0;
    }
}