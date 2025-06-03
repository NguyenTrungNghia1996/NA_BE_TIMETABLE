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
        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string TenDonvi { get; set; } = string.Empty;
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string? Diachi { get; set; }
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string? Sodienthoai { get; set; }
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string? Mota { get; set; }
    }
    public class DM_Donvi_List
    {
        public int? Stt = 0;
        public int Id { get; set; } = 0;
        public string TenDonvi { get; set; } = string.Empty;
        public string? Diachi { get; set; }
        public string? Sodienthoai { get; set; }
        public string? Mota { get; set; }
    }
}