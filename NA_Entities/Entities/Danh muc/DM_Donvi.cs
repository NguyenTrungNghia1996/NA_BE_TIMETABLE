using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danhmuc
{
    public class DM_Donvi
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
        public int Id_tinh { get; set; } = 0;
        public string Nguoi_lien_he { get; set; } = string.Empty;
        public int So_ngay { get; set; } = 0;
        public int? Id_cha { get; set; }
        public bool La_so_giao_duc { get; set; } = false;

    }
    //public class DM_Donvi_Demo
    //{
    //    public int Id { get; set; } = 0;
    //    [Required(ErrorMessage = "Tên đơn vị không được để trống")]
    //    [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
    //    public string TenDonvi { get; set; } = string.Empty;
    //    [Required(ErrorMessage = "Địa chỉ không được để trống")]
    //    [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
    //    public string? Diachi { get; set; }
    //    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    //    [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
    //    [RegularExpression(@"^\d+$", ErrorMessage = "Chỉ được nhập số")]
    //    public string Sodienthoai { get; set; } = string.Empty;
    //    [Required(ErrorMessage = "Email không được để trống")]
    //    [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
    //    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    //    public string Email { get; set; } = string.Empty;
    //    public int Id_tinh { get; set; } = 0;
    //    public string Nguoi_lien_he { get; set; } = string.Empty;
    //    public bool Trang_thai_xoa { get; set; } = false;
    //}
    public class DM_Donvi_List
    {
        public int? Stt = 0;
        public int Id { get; set; } = 0;
        public string TenDonvi { get; set; } = string.Empty;
        public string Diachi { get; set; } = string.Empty;
        public string Sodienthoai { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Ten_cap { get; set;} = string.Empty;
        public string Ten_ca { get; set;} = string.Empty;
        public int So_ngay { get; set; } = 0;
        public int? Id_cha { get; set; }
        public string Ten_cha { get; set; } = string.Empty;
    }
}