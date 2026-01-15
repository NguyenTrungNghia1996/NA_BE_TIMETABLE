using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_LopontapDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Mã lớp không được để trống")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Ma { get; set; } = string.Empty;
        [Required(ErrorMessage = "Tên lớp không được để trống")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [Required(ErrorMessage = "Khối lớp không được để trống")]
        public int Id_khoi { get; set; } = 0;
        [Required(ErrorMessage = "Môn học không được để trống")]
        public int Id_mon { get; set; } = 0;
        [Required(ErrorMessage = "Phòng học không được để trống")]
        public int Id_phong { get; set; } = 0;
        [Required(ErrorMessage = "Giáo viên không được để trống")]
        public int Id_giao_vien { get; set; } = 0;
        [Required(ErrorMessage = "Năm học không được để trống")]
        public int Id_nam { get; set; } = 0;
        [Required(ErrorMessage = "Số tiết tối đa trên tuần không được để trống")]
        public int So_tiet_toi_da_tren_tuan { get; set; } = 0;
    }
    public class DM_Lopontap_ListDto
    {
        public int Id { get; set; }
        public string? Ma { get; set; }
        public string? Ten { get; set; }
        public int Id_khoi { get; set; }
        public string? Ten_khoi { get; set; }
        public int Id_mon { get; set; }
        public string? Ten_mon { get; set; }
        public int Id_phong { get; set; }
        public string? Ten_phong { get; set; }
        public int Id_giao_vien { get; set; }
        public string? Ten_giao_vien { get; set; }
        public int Id_nam { get; set; }
        public string? Ten_nam { get; set; }
        public int So_tiet_toi_da_tren_tuan { get; set; }
    }
    public class Lopontap_TietnghiDto
    {
        public int Id { get; set; } = 0;
        public List<Ca_banDto> Ds_Ca { get; set; } = new List<Ca_banDto>();
    }
}
