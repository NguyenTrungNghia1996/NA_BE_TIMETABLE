using System;
using System.Collections.Generic;
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
        public int Id_phong_chuyen_dung { get; set; } = 0;
        public string Ten_phong_chuyen_dung { get; set; } = string.Empty;
        public int Id_phong_truyen_thong { get; set; } = 0;
        public string Ten_phong_truyen_thong { get; set; } = string.Empty;
        public int So_tiet_ca_sang_truyen_thong { get; set; } = 0;
        public int So_tiet_ca_chieu_truyen_thong { get; set; } = 0;
        public int So_tiet_ca_sang_phong_chuyen_dung { get; set; } = 0;
        public int So_tiet_ca_chieu_phong_chuyen_dung { get; set; } = 0;
        public bool Trang_thai { get; set; } = false;
    }
    public class LopMon_banDto
    {
        public int Id_lop { get; set; } = 0;
        public int Id_mon { get; set; } = 0;
        public List<Ca_banDto> Ds_Ca { get; set; } = new List<Ca_banDto>();
    }
}
