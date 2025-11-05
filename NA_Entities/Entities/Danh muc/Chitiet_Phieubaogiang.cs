using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Chitiet_Phieubaogiang
    {
        public int Id { get; set; }
        public int Id_phieu_bao_giang { get; set; }
        public DateTime Ngay { get; set; }
        public int Thu { get; set; }
        public int Tiet { get; set; }
        public int Id_chi_tiet_PPCT { get; set; }
        public int Id_lop { get; set; }
        public int Id_mon { get; set; }
        public string Ghi_chu { get; set; }
    }
}
