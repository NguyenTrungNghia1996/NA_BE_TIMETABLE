using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Danhsach_Thoikhoabieu
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên thời khoá biểu không được để trống")]
        [StringLength(200, ErrorMessage = "Tên thời khoá biểu không được quá 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        public bool Dang_su_dung { get; set; } = false;
        public int Id_don_vi { get; set; } = 0;
        public bool Trang_thai_xep { get; set; } = false;
    }
    public class Danhsach_ThoikhoabieuList
    {
        public int STT { get; set; } = 0;
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên thời khoá biểu không được để trống")]
        [StringLength(200, ErrorMessage = "Tên thời khoá biểu không được quá 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        public bool Dang_su_dung { get; set; } = false;
        public int Id_don_vi { get; set; } = 0;
        public bool Trang_thai_xep { get; set; } = false;
    }
    public class ThoiKhoaBieu_Detail
    {
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public bool Dang_su_dung { get; set; } = false;
        public int Id_don_vi { get; set; } = 0;
        public bool Trang_thai_xep { get; set; } = false;
        public int Tong_tat_ca_tiet { get; set; } = 0;
        public int Tong_tiet_da_xep { get; set; } = 0;
        public int Tong_tiet_chua_xep { get; set; } = 0;
    }
}
