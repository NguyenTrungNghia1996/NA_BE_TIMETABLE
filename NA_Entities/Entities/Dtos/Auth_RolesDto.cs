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
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string? Mota { get; set; }
        [Required(ErrorMessage = "Danh sách quyền không được để trống")]
        [MinLength(1, ErrorMessage = "Vui lòng chọn ít nhất 1 quyền")]
        public List<Auth_Roles_PermissionDto> Permission { get; set; } = new List<Auth_Roles_PermissionDto>();
    }
}