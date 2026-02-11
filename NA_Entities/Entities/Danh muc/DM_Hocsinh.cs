using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Hocsinh
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Mã học sinh không được để trống")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Ma { get; set; } = string.Empty;
        [Required(ErrorMessage = "Tên học sinh không được để trống")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Ten { get; set; } = string.Empty;
        public int Id_don_vi { get; set; }
        [Required(ErrorMessage = "Lớp chính khoá không được để trống")]
        public int Id_lop_chinh { get; set; }

    }
    public class DM_Hocsinh_List
    {
        public int Id { get; set; } = 0;
        public string Ma { get; set; } = string.Empty;
        public string Ten { get; set; } = string.Empty;
        public int Id_don_vi { get; set; } = 0;
        public int Id_lop_chinh { get; set; } = 0;
        public string? Ten_lop { get; set; } = string.Empty;
        public int Total { get; set; }
    }
    public class Hocsinh_List
    {
        public int Id_hoc_sinh { get; set; } = 0;
        public string Ten { get; set;} = string.Empty;
    }
    public class Ketqua_Hocsinh
    {
        public string Ma_hoc_sinh { get; set; }
        public string Ten_hoc_sinh { get; set; }
        public string Ten_lop_on { get; set; }
        public int? Id_loai_kiem_tra { get; set; }
        public string? Ten_loai_kiem_tra { get; set; }
        public decimal? Diem_so { get; set; }
    }
}
