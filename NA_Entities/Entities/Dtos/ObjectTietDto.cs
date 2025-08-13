using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class tkb_theo_lop
    {
        public int Id_don_vi { get; set; } = 0;
        public int Id_tkb { get; set; } = 0;
        public int Id_mon { get; set; } = 0;
        public string Ten_mon { get; set; } = string.Empty;
        public int Id_giao_vien { get; set; } = 0;
        public string Ten_giao_vien { get; set; } = string.Empty;
        public int Id_phong { get; set; } = 0;
        public string? Ten_phong { get; set; } = string.Empty;
        public int Tiet_thu_may { get; set; } = 0;
        public int Id_ca { get; set; } = 0;
        public int Ngay { get; set; } = 0;
        public int Tiet { get; set; } = 0;
    }
    public class ObjectTietDto
    {
        public int Id_lop { get; set; } = 0;
        public string Ten_lop { get; set; } = string.Empty;
        public List<tkb_theo_lop> timetable { get; set; } = new List<tkb_theo_lop>();
    }
}
