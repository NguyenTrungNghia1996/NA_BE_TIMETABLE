using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class Tiet_banDto
    {
        public int Id_phong { get; set; } = 0;
        public int Id_ca { get; set; } = 0;
        public int Thu { get; set; } = 0;
        public string Ten_thu {get;set; } = string.Empty;
        public int Tiet { get; set; } = 0;
        public string Ten_tiet { get; set; } = string.Empty;
        public bool Trang_thai { get; set; } = false;
    }
}
