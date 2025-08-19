using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class ThoiKhoaBieuDto
    {
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public bool Dang_su_dung { get; set; } = false;
        public int Id_don_vi { get; set; } = 0;
        public bool Trang_thai_xep { get; set; } = false;
        public int Tong_tat_ca_tiet { get; set; } = 0;
        public int Tong_tiet_da_xep { get; set; } = 0;
        public int Tong_tiet_chua_xep { get; set; } = 0;
    }
    public class Xep_tkb
    {
        public int Id_tkb { get; set; } = 0;
        public List<int> Ids { get; set; } = new List<int>();
    }
    public class Ds_mon_bylop
    {
        public int Id_mon { get; set; }
    }
    public class Ds_lop
    {
        public int Id_lop { get; set; }
        public List<Ds_mon_bylop> Ds_mon { get; set; } = new List<Ds_mon_bylop>();
    }
    public class Xep_LopMon 
    {
        public int Id_tkb { get; set; } = 0;
        public List<Ds_lop> Ds_lop { get; set;} = new List<Ds_lop>();
    }
}
