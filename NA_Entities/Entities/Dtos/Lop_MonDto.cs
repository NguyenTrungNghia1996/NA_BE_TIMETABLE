using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class Lop_MonDto
    {
        public int Id_lop { get; set; } = 0;
        public List<Mon_LopDto> Ds_mon { get; set; } = new List<Mon_LopDto>();
    }
    public class Mon_LopDto
    {
        public int Id_mon { get; set; } = 0;
        public string Ten_mon { get; set; } = string.Empty;
        public int Id_giao_vien { get; set; } = 0;
        public string Ten_giao_vien { get; set; } = string.Empty;
        public int? Id_phong_chuyen_dung { get; set; } = 0;
        public string? Ten_phong_chuyen_dung { get; set; } = string.Empty;
        public int? Id_phong_truyen_thong { get; set; } = 0;
        public string? Ten_phong_truyen_thong { get; set; } = string.Empty;
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Không được nhập số thập phân")]
        [Range(0, 9, ErrorMessage = "Số tiết ca sáng truyền thống phải là số nguyên dương và nhỏ hơn 10")]
        public double? So_tiet_ca_sang_truyen_thong { get; set; } = 0;
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Không được nhập số thập phân")]
        [Range(0, 9, ErrorMessage = "Số tiết ca chiều truyền thống phải là số nguyên dương và nhỏ hơn 10")]
        public double? So_tiet_ca_chieu_truyen_thong { get; set; } = 0;
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Không được nhập số thập phân")]
        [Range(0, 9, ErrorMessage = "Số tiết ca sáng chuyên dụng phải là số nguyên dương và nhỏ hơn 10")]
        public double? So_tiet_ca_sang_phong_chuyen_dung { get; set; } = 0;
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Không được nhập số thập phân")]
        [Range(0, 9, ErrorMessage = "Số tiết ca chiều chuyên dụng phải là số nguyên dương và nhỏ hơn 10")]
        public double? So_tiet_ca_chieu_phong_chuyen_dung { get; set; } = 0;
        public bool Trang_thai { get; set; } = false;
    }
    public class LopMon_banDto
    {
        public int Id_lop { get; set; } = 0;
        public int Id_mon { get; set; } = 0;
        public List<Ca_banDto> Ds_Ca { get; set; } = new List<Ca_banDto>();
    }
}
