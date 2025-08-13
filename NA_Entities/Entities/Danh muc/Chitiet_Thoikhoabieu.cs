using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Chitiet_Thoikhoabieu
    {
        public int Id { get; set; } = 0;
        public int Id_tkb { get; set; } = 0;
        public int Id_lop { get; set; } = 0;
        public int Id_mon { get; set; } = 0;
        public int Id_giao_vien { get; set; } = 0;
        public int Id_phong { get; set; } = 0;
        public int Id_ca { get; set; } = 0;
        public int Tiet_thu_may { get; set; } = 0;
        public int Ngay { get; set; } = 0;
        public int Tiet { get; set; } = 0;
        public bool Khoa { get; set; } = false;
    }
    public class Chitiet_Thoikhoabieu_List
    {
        public int Id { get; set; } = 0;
        public int? Id_don_vi { get; set; } = 0;
        public string? Ten_don_vi { get; set; } = string.Empty;
        public int Id_tkb { get; set; } = 0;
        public int Id_lop { get; set; } = 0;
        public string? Ten_lop { get; set; }
        public int Id_mon { get; set; } = 0;
        public string Ten_mon { get; set; }
        public int? Id_giao_vien { get; set; } = 0;
        public string? Ten_giao_vien { get; set; }
        public int Id_phong { get; set; } = 0;
        public string? Ten_phong { get; set; }
        public int Id_ca { get; set; } = 0;
        public string Ten_ca { get; set; }
        public int Tiet_thu_may { get; set; } = 0;
        public int Ngay { get; set; } = 0;
        public int Tiet { get; set; } = 0;
        public bool Khoa { get; set; } = false;
    }
}
