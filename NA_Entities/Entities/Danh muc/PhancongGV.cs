using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class PhancongGV
    {
        public int STT { get; set; } = 0;
        public int Id_giao_vien { get; set; } = 0;
        public int Id_mon { get; set; }
        public string Ten_mon { get; set; }
        public int Id_don_vi { get; set; }
        public string Id_lop { get; set; } = string.Empty;
        public string Ten_lop { get; set; } = string.Empty;
    }
    public class DsLop_ByGVandMon
    {
        public int STT { get; set; } = 0;
        public int Id_lop { get; set; } = 0;
        public string Ten_lop { get; set; } = string.Empty;
        public int Tong_tiet { get; set; } = 0;
        public string Ten_giao_vien { get; set; } = string.Empty;
    }
    public class DsLopMon_byGV
    {
        public int Id_giao_vien { get; set; } = 0;
        public int Id_mon { get; set; } = 0;
        public string Ten_lop { get; set; } = string.Empty;
        public int Tong_tiet { get; set; } = 0;
        public string Ten_giao_vien { get; set; } = string.Empty;
        public string Ten_mon { get; set; } = string.Empty;
    }
}
