using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class NgayDto
    {
        public int Ngay { get; set; }
        public bool Trang_thai { get; set; }
    }
    public class TietDto
    {
        public int Tiet { get; set; }
        public bool Trang_thai { get; set; }
    }
    public class Donvi_Tiet
    {
        public int Id_ca { get; set; }
        public List<TietDto> list_tiet { get; set; } = new List<TietDto>();
    }
}
