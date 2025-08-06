using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Lophoc_Monhoc
    {
        public int Id { get; set; } = 0;
        public int? Id_lop { get; set; } = 0;
        public int Id_mon { get; set; } = 0;
        public int? Id_giao_vien { get; set; } = 0;
        public int? Id_phong_chuyen_dung { get; set; } = 0;
        public int? Id_phong_truyen_thong { get; set; } = 0;
        public int? So_tiet_ca_sang_truyen_thong { get; set; } = 0;
        public int? So_tiet_ca_chieu_truyen_thong { get; set; } = 0;
        public int? So_tiet_ca_sang_phong_chuyen_dung { get; set; } = 0;
        public int? So_tiet_ca_chieu_phong_chuyen_dung { get; set; } = 0;
    }
    public class Lophoc_Monhoc_List
    {
        public int Id_lop { get; set; } = 0;
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
    public class CheckIds
    {
        public string Field { get; set; } 
        public bool IsValid { get; set; }    
    }
}
