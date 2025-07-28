using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Giaovien_Buoiday
    {
        public int? Id { get; set; } = 0;
        public int? Id_giao_vien { get; set; } = 0;
        public bool? Chi_day_mot_buoi { get; set; } = false;
        public int? So_tiet_toi_da { get; set; } = 0;
    }
}
