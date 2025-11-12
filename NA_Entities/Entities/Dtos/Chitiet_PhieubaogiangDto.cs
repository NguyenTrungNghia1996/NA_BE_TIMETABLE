using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    
    public class Thongtin_tietDto
    {
        public int Tiet_tkb { get; set; }
        public int Tiet_ppct { get; set; }
        public string Ten_lop { get; set; }
        public string Ten_mon { get; set; }
        public string Phan_mon { get; set; }
        public string Ten_bai { get; set; }
        public string Ghi_chu { get; set; }
    }
    public class BuoihocDto
    {
        public int Id_ca { get; set; }
        public string Ten_buoi { get; set; }
        public List<Thongtin_tietDto> Cac_tiet_hoc { get; set; }
    }
    public class NgayhocDto
    {
        public string Ngay { get; set; }
        public List<BuoihocDto> Buoi_hoc { get; set; }
    }
    public class Chitiet_PhieubaogiangDto
    {
        public string Ten_giao_vien { get; set; }
        public List<NgayhocDto> Lich_theo_ngay { get; set; }
    }
}
