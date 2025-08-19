using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Sotiet_Mon
    {
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public int Tong_tiet { get; set; } = 0;
        public int Tiet_chua_xep { get; set; } = 0;
        public int Tiet_da_xep { get; set; } = 0;

    }
    public class Sotiet_Lop
    {
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public int Tong_tiet { get; set; } = 0;
        public int Tiet_chua_xep { get; set; } = 0;
        public int Tiet_da_xep { get; set; } = 0;

    }
    public class Sotiet_Phong
    {
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public int Tong_tiet { get; set; } = 0;
        public int Tiet_chua_xep { get; set; } = 0;
        public int Tiet_da_xep { get; set; } = 0;

    }
    public class Sotiet_Giaovien
    {
        public int Id { get; set; } = 0;
        public string Ho_ten { get; set; } = string.Empty;
        public int Tong_tiet { get; set; } = 0;
        public int Tiet_chua_xep { get; set; } = 0;
        public int Tiet_da_xep { get; set; } = 0;

    }
    public class Mon_Sotiet
    {
        public int Id_mon { get; set; } = 0;
        public string Ten_mon { get; set; } = string.Empty;
        public int Tong_tiet { get; set; } = 0;
        public int Tiet_chua_xep { get; set; } = 0;
        public int Tiet_da_xep { get; set; } = 0;
    }
    public class Sotiet_LopMonDto
    {
        public int Id_lop { get; set; } = 0;
        public string Ten_lop { get; set; } = string.Empty;
        public List<Mon_Sotiet> ds_mon { get; set; } = new List<Mon_Sotiet>();
    }
    public class Sotiet_LopMon
    {
        public int Id_lop { get; set; } = 0;
        public string Ten_lop { get; set;} = string.Empty;
        public int Id_mon { get; set; } = 0;
        public string Ten_mon { get; set;} = string.Empty;
        public int Tong_tiet { get; set; } = 0;
        public int Tiet_chua_xep { get; set; } = 0;
        public int Tiet_da_xep { get; set; } = 0;
    }

}
