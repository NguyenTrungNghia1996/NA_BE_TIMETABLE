using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_GiaovienDto
    {
        public int Id { get; set; } = 0;
        //public string Ma_giao_vien { get; set; } = string.Empty;
        [Required(ErrorMessage = "Họ và tên đệm không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ho_va_ho_dem { get; set; } = string.Empty;
        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn tổ chuyên môn")]
        public int Id_to_chuyen_mon { get; set; } = 0;
        public int Id_don_vi { get; set; } = 0;
        public List<int> Id_diem_truong { get; set; } = new List<int>();
    }
    public class DM_Giaovien_ListDto
    {
        public int Id { get; set; } = 0;
        public string Ma_giao_vien { get; set; } = string.Empty;
        public string Ho_va_ho_dem { get; set; } = string.Empty;
        public string Ten { get; set; } = string.Empty;
        public int Id_to_chuyen_mon { get; set; } = 0;
        public string Ten_to_chuyen_mon { get; set; } = string.Empty;
        public int Id_don_vi { get; set; } = 0;
        public int Ca_sang { get; set; } = 0;
        public int Ca_chieu { get; set; } = 0;
    }
    public class Giaovien_banDto
    {
        public int Id_giao_vien { get; set; } = 0;
        public int? Id_buoi_day { get; set; } = 0;
        public bool? Chi_day_mot_buoi { get; set; } = false;
        public int? So_tiet_toi_da { get; set; } = 0;
        public List<Ca_banDto> Ds_Ca { get; set; } = new List<Ca_banDto>();
    }
    public class Giaovien_MonDto
    {
        public int Id_giao_vien { get; set; }
        public List<Mon_GiaovienDto> Ds_mon { get; set; } = new List<Mon_GiaovienDto>();
    }
}
