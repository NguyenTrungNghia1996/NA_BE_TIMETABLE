using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class Ca_DonviDto
    {
        public int Id_ca_hoc { get; set; }
        public string Ten_ca { get; set; }
        public int So_tiet { get;set; }
    }
    public class DM_DonviDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên đơn vị không được để trống")]
        [StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string TenDonvi { get; set; } = string.Empty;
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string? Diachi { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(12, ErrorMessage = "Tối đa 12 ký tự")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Chỉ được nhập số")]
        public string Sodienthoai { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một cấp học")]
        public List<int> IdCap { get; set; } = new List<int>();
        public List<int> Id_cahoc { get; set; } = new List<int>();
        public int Id_cha { get; set; } = 0;
    }
    public class DM_Donvi_List_Dto
    {
        public int? Stt = 0;
        public int Id { get; set; } = 0;
        public string TenDonvi { get; set; } = string.Empty;
        public string Diachi { get; set; } = string.Empty;
        public string Sodienthoai { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Ten_cap { get; set; } = string.Empty;
        public string Ten_ca { get; set; } = string.Empty;
        public string Ten_cha { get; set; } = string.Empty;
    }
    public class DM_Donvi_updateDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên đơn vị không được để trống")]
        [StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string TenDonvi { get; set; } = string.Empty;
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string? Diachi { get; set; }
        public int Id_tinh { get; set; } = 0;
        public string Nguoi_lien_he { get; set; } = string.Empty;
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(12, ErrorMessage = "Tối đa 12 ký tự")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Chỉ được nhập số")]
        public string Sodienthoai { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một cấp học")]
        public List<int> IdCap { get; set; } = new List<int>();
        public List<int> Id_cahoc { get; set; } = new List<int>();
        public int Id_cha { get; set; } = 0;
    }
    public class Thongtin_Donvi_updateDto
    {
        [Required(ErrorMessage = "Tên đơn vị không được để trống")]
        [StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string TenDonvi { get; set; } = string.Empty;
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string? Diachi { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(12, ErrorMessage = "Tối đa 12 ký tự")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Chỉ được nhập số")]
        public string Sodienthoai { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một cấp học")]
        public List<int> IdCap { get; set; } = new List<int>();
        public List<Ca_DonviDto> List_ca { get; set; } = new List<Ca_DonviDto>();
        public int So_ngay { get; set; } = 0;
        public int Id_cha { get; set; } = 0;
    }

}
