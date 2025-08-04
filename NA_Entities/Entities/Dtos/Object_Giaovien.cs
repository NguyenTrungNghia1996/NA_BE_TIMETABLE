using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class Ds_tiet_phan_cong
    {
        public int Id_mon { get; set; }
        public string Ten_mon { get; set; }
        public int Id_lop { get; set; }
        public string Ten_lop { get; set; }
        public int Id_phong { get; set; }
        public string Ten_phong { get; set; }
        public int Id_ca { get; set; }
        public int Tiet { get; set; }
        public int Ten_tiet { get; set; }
        public int Ngay { get; set; }
        public int Ten_ngay { get; set; }
    }
    public class Ds_tiet_da_xep
    {
        public int Id_mon { get; set; }
        public string Ten_mon { get; set; }
        public int Id_lop { get; set; }
        public string Ten_lop { get; set; }
        public int Id_phong { get; set; }
        public string Ten_phong { get; set; }
        public int Id_ca { get; set; }
        public int Tiet { get; set; }
        public int Ten_tiet { get; set; }
        public int Ngay { get; set; }
        public int Ten_ngay { get; set; }
    }
    public class Ds_chua_xep
    {
        public int Id_mon { get; set; }
        public string Ten_mon { get; set; }
        public int Id_lop { get; set; }
        public string Ten_lop { get; set; }
        public int Id_phong { get; set; }
        public string Ten_phong { get; set; }
        public int Id_ca { get; set; }
        public int Tiet { get; set; }
        public int Ten_tiet { get; set; }
        public int Ngay { get; set; }
        public int Ten_ngay { get; set; }
    }
    public class Object_Giaovien
    {
        public int Id_don_vi { get; set; } = 0;
        public string Ten_don_vi { get; set; } = string.Empty;
        public int Id_giao_vien { get; set; } = 0;
        public string Ten_giao_vien { get; set; } = string.Empty;
        public List<Ds_tiet_phan_cong> ds_tiet_phan_cong { get; set; } = new List<Ds_tiet_phan_cong>();
        public List<Ds_tiet_da_xep> ds_tiet_da_xep { get; set; } = new List<Ds_tiet_da_xep>();
        public List<Ds_chua_xep> ds_tiet_chua_xep { get; set; } = new List<Ds_chua_xep>();
    }
}
