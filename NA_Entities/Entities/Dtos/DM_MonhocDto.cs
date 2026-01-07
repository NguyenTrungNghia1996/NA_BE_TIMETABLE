using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_MonhocDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Mã môn học không được để trống")]
        [StringLength(30, ErrorMessage = "Tối đa 30 ký tự")]
        public string Ma { get; set; } = string.Empty;
        [Required(ErrorMessage = "Tên môn học không được để trống")]
        [StringLength(30, ErrorMessage = "Tối đa 30 ký tự")]
        public string Ten { get; set; } = string.Empty;
        public int Id_loai_phong_hoc { get; set; } = 0;
        public bool Do_GVCN_phu_trach { get; set; } = false;
        public bool Hoc_cach_ngay { get; set; } = false;
        public bool Xep_thanh_cap { get; set; } = false;
        [Required(ErrorMessage = "Số tiết tối đa một ca không được để trống")]
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Không được nhập số thập phân")]
        [Range(1, 9, ErrorMessage = "Số tiết tối đa một ca phải lớn hơn 0 và nhỏ hơn 10")]
        public double? So_tiet_toi_da_mot_ca { get; set; }
        [Required(ErrorMessage = "Số tiết tối đa hai ca không được để trống")]
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Không được nhập số thập phân")]
        [Range(1, 9, ErrorMessage = "Số tiết tối đa hai ca phải lớn hơn 0 và nhỏ hơn 10")]
        public double? So_tiet_toi_da_hai_ca { get; set; }
        public bool La_mon_tu_chon { get; set; } = false;
        public int Id_don_vi { get; set; } = 0;
        public List<int> Id_khoi_kien_thuc { get; set; } = new List<int>();
        public List<int> Id_phong { get; set; } = new List<int>();
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten_theo_nganh { get; set; } = string.Empty;
    }
    public class DM_Monhoc_ListDto
    {
        public int STT { get; set; } = 0;
        public int Id { get; set; } = 0;
        public string Ma { get; set; } = string.Empty;
        public string Ten { get; set; } = string.Empty;
        public bool Do_GVCN_phu_trach { get; set; } = false;
        public bool Hoc_cach_ngay { get; set; } = false;
        public bool Xep_thanh_cap { get; set; } = false;
        public int So_tiet_toi_da_mot_ca { get; set; } = 0;
        public int So_tiet_toi_da_hai_ca { get; set; } = 0;
        public bool La_mon_tu_chon { get; set; } = false;
        public string Ten_loai_phong_hoc { get; set; } = string.Empty;
        public string? Ten_mon_theo_nganh { get; set; } = string.Empty;

    }
    public class DM_Monhoc_updateDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên môn học không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn loại phòng học")]
        public int Id_loai_phong_hoc { get; set; } = 0;
        public bool Do_GVCN_phu_trach { get; set; } = false;
        public bool Hoc_cach_ngay { get; set; } = false;
        public bool Xep_thanh_cap { get; set; } = false;
        [Required(ErrorMessage = "Số tiết tối đa một ca không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Sức chứa phải là số dương")]
        public int So_tiet_toi_da_mot_ca { get; set; } = 0;
        [Required(ErrorMessage = "Số tiết tối đa hai ca không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Sức chứa phải là số dương")]
        public int So_tiet_toi_da_hai_ca { get; set; } = 0;
        public bool La_mon_tu_chon { get; set; } = false;
        public int Id_don_vi { get; set; } = 0;
        public List<int> Id_khoi_kien_thuc { get; set; } = new List<int>();
        public List<int> Id_phong { get; set; } = new List<int>();
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten_theo_nganh { get; set; } = string.Empty;

    }
    public class Mon_banDto
    {
        public int Id { get; set; } = 0;
        public List<Ca_banDto> Ds_Ca { get; set; } = new List<Ca_banDto>();
    }
    public class Mon_KhoiDto
    {
        public int Id_mon { get;set; } = 0;
        public string Ten_mon { get; set; } = string.Empty;
        public List<Ca_Khoi_MonDto> ds_Ca { get; set; } = new List<Ca_Khoi_MonDto>();
        public bool Trang_thai { get; set; }

    }
    public class Mon_GiaovienDto {
        public int Id_mon { get; set; } = 0;
        public string Ten_mon { get; set; } = string.Empty;
        public bool Trang_thai { get; set; } = false;
    }
}
