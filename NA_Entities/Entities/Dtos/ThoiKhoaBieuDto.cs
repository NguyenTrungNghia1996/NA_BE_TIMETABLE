using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class Danhsach_ThoikhoabieuDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên thời khoá biểu không được để trống")]
        [StringLength(100, ErrorMessage = "Tên thời khoá biểu không được quá 100 ký tự")]
        public string Ten { get; set; } = string.Empty;
        public bool Dang_su_dung { get; set; } = false;
        public int Id_don_vi { get; set; } = 0;
        public bool Trang_thai_xep { get; set; } = false;
        public int Id_tkb_nguon { get; set; } = 0;
    }
    public class ThoiKhoaBieuDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên thời khoá biểu không được để trống")]
        [StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string Ten { get; set; } = string.Empty;
        public bool Dang_su_dung { get; set; } = false;
        public int Id_don_vi { get; set; } = 0;
        public bool Trang_thai_xep { get; set; } = false;
        public int Tong_tat_ca_tiet { get; set; } = 0;
        public int Tong_tiet_da_xep { get; set; } = 0;
        public int Tong_tiet_chua_xep { get; set; } = 0;
    }
    public class Xep_tkb
    {
        public int Id_tkb { get; set; } = 0;
        public List<int> Ids { get; set; } = new List<int>();
    }
    public class Ds_mon_bylop
    {
        public int Id_mon { get; set; }
    }
    public class Ds_lop
    {
        public int Id_lop { get; set; }
        public List<Ds_mon_bylop> Ds_mon { get; set; } = new List<Ds_mon_bylop>();
    }
    public class Xep_LopMon 
    {
        public int Id_tkb { get; set; } = 0;
        public List<Ds_lop> Ds_lop { get; set;} = new List<Ds_lop>();
    }
    public class ExportDto
    {
        public int Id_tkb { get; set; }
        public int show_room { get; set; }
        public int show_teacher { get; set; }
    }
}
