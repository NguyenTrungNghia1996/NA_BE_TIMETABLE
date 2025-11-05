using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Phanphoi_Chuongtrinh_Chitiet
    {
        public int Id { get; set; }
        public int Id_ppct {get;set;}
        public int Tuan { get;set; }
        public int Thu_tu_tiet { get; set; }
        public string Phan_mon { get; set; }
        public string Ten_bai { get; set; }
    }
    public class Phanphoi_Chuongtrinh_Chitiet_List
    {
        public int STT { get; set; }
        public int Id { get; set; }
        public int Id_ppct {get;set;}
        public int Tuan { get;set; }
        public int Thu_tu_tiet { get; set; }
        public string Phan_mon { get; set; }
        public string Ten_bai { get; set; }
    }
}
