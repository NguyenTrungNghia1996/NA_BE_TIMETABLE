using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class Lop_Mon_TiettranhxepDto
    {
        public int Id { get; set; } = 0;
        public int Id_lop { get; set; } = 0;
        public int Id_mon { get; set; } = 0;
        public int Id_ca { get; set; } = 0;
        public int Ngay { get; set; } = 0;
        public string Ten_ngay { get; set; } = string.Empty;
        public int Tiet { get; set; } = 0;
        public string Ten_tiet { get; set; } = string.Empty;
    }
}
