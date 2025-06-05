using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class Auth_Roles_PermissionDto
    {
        [Required(ErrorMessage = "Key nhóm không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Key { get; set; } = string.Empty;
        [Required(ErrorMessage = "Permission Value không được để trống")]
        public int PermissionValue { get; set; }
    }
}