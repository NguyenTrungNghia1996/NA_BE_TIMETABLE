using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class PhancongGVDto
    {
        public int STT { get; set; } = 0;
        public int Id_giao_vien { get; set; } = 0;
        public int Id_mon { get; set; }
        public string Ten_mon { get; set; }
        public int Id_don_vi { get; set; }
        public List<int> Id_lop { get; set; } = new List<int>();
        public string Ten_lop { get; set; } = string.Empty;
    }
    public class Phancong_gv
    {
        public int Id_giao_vien { get; set; } = 0;
        public int Id_mon { get; set; } = 0;
        public List<int> Id_lop { get; set; } = new List<int>();
    }
}
