using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Monhoc
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
        [Range(1, int.MaxValue, ErrorMessage = "Số tiết tối đa 1 ca phải là số dương")]
        public int So_tiet_toi_da_mot_ca { get; set; } = 0;
        [Required(ErrorMessage = "Số tiết tối đa hai ca không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số tiết tối đa 2 ca phải là số dương")]
        public int So_tiet_toi_da_hai_ca { get; set; } = 0;
        public bool La_mon_tu_chon { get; set; } = false;
        public int Id_don_vi { get; set; }
    }
    public class DM_Monhoc_List
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
        public string? Ten_mon_theo_nganh { get; set;} = string.Empty;
    }
    public class MonLop
    {
        public int STT { get; set; } = 0;
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public int? So_tiet_phong_chuyen_dung { get; set; } = 0;
        public int? So_tiet_phong_truyen_thong { get; set; } = 0;
        public int? Tong_so_tiet { get; set; } = 0;
    }

}
