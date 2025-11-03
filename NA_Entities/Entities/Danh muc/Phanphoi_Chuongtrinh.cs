using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Phanphoi_Chuongtrinh
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public int Id_nam_hoc { get; set; }
        public int Id_khoi { get; set; }
        public int Id_ban { get; set; }
        public int Id_mon { get; set; }
        public int Id_don_vi { get; set; }
    }
    public class Phanphoi_Chuongtrinh_List
    {
        public int STT { get; set; }
        public int Id { get; set; }
        public string Ten { get; set; }
        public int Id_nam_hoc { get; set; }
        public string Ten_nam_hoc { get; set; }
        public int Id_khoi { get; set; }
        public string Ten_khoi { get;set; }
        public int Id_ban { get; set; }
        public string Ten_ban { get; set; }
        public int Id_mon { get; set; }
        public string Ten_mon { get; set; }
        public int Id_don_vi { get;set; }
    }
}
