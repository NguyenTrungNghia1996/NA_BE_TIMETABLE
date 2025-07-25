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
        public int Id_lop { get; set; } = 0;
        public int Id_mon { get; set; } = 0;
        public int Id_giao_vien { get; set; } = 0;
        public int Id_phong { get; set; } = 0;
        public int So_tiet_ca_sang_truyen_thong { get; set; } = 0;
        public int So_tiet_ca_chieu_truyen_thong { get; set; } = 0;
        public int So_tiet_ca_sang_phong_chuyen_dung { get; set; } = 0;
        public int So_tiet_ca_chieu_phong_chuyen_dung { get; set; } = 0;
    }
}
