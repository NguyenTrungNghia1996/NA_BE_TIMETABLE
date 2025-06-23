using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class Tiet_tranh_xepDto
    {
        public int Id { get; set; } = 0;
        public int Id_mon { get; set; } = 0;
        public int Id_ca { get; set; } = 0;
        public int Id_thu { get; set; } = 0;
        public int Id_tiet { get; set; } = 0;
        public bool Trang_thai { get; set; } = false;
    }
}
