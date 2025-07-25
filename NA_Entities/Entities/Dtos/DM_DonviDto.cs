using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_DonviDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên đơn vị không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string TenDonvi { get; set; } = string.Empty;
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string? Diachi { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Chỉ được nhập số")]
        public string Sodienthoai { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một cấp học")]
        public List<int> IdCap { get; set; } = new List<int>();
        public List<int> Id_cahoc { get; set; } = new List<int>();
    }
    public class DM_Donvi_List_Dto
    {
        public int? Stt = 0;
        public int Id { get; set; } = 0;
        public string TenDonvi { get; set; } = string.Empty;
        public string Diachi { get; set; } = string.Empty;
        public string Sodienthoai { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

    }
    public class DM_Donvi_updateDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên đơn vị không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string TenDonvi { get; set; } = string.Empty;
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string? Diachi { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Chỉ được nhập số")]
        public string Sodienthoai { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một cấp học")]
        public List<int> IdCap { get; set; } = new List<int>();
        public List<int> Id_ca_hoc { get; set; } = new List<int>();

    }
    public class Donvi_NgayDto
    {
        public int Id { get; set; }= 0;
        public List<int> Id_Ngay { get; set; } = new List<int>();

    }
}
