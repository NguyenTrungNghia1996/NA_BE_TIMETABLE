using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Tiet_co_dinh
    {
        public int Id { get; set; } = 0;
        public int Id_mon { get; set; } = 0;
        public int Id_ngay { get; set; } = 0;
        public int Id_ca { get; set; } = 0;
        public int Id_tiet { get; set; } = 0;
        public int Id_khoi_lop { get; set; } = 0;
    }
    public class Tiet_co_dinh_List
    {
        public int STT { get; set; } = 0;
        public string Ten_mon_hoc { get; set; } = string.Empty;
        public string Ten_khoi_lop { get; set; } = string.Empty;
        public string Ten_ca_hoc { get; set; } = string.Empty;
        public string Ten_ngay_hoc { get; set; } = string.Empty;
        public string Ten_tiet_hoc { get; set; } = string.Empty;
    }
}
