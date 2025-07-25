using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class Giaovien_TiettranhxepDto
    {
        public int Id { get; set; } = 0;
        public int Id_giao_vien { get; set; } = 0;
        public int Id_ca { get; set; } = 0;
        public int Ngay { get; set; } = 0;
        public string Ten_ngay { get; set; } = string.Empty;
        public int Tiet { get; set; } = 0;
        public string Ten_tiet { get; set; } = string.Empty;
        public bool Chi_day_mot_buoi { get; set; } = false;
        public int So_tiet_toi_da { get; set; } = 0;
    }
}
