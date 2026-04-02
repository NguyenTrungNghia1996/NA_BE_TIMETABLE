using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_Lichthi_ListDto
    {
        public int Id { get; set; }
        public string? Ten_mon { get; set; }
        public int? Id_mon { get; set; }
        public string Ten_diem_thi { get; set; }
        public string Ten_hoi_dong { get; set; }
        public string Ten_nam { get; set; }
        public int Id_diem_thi { get; set; }
        public DateTime Ngay { get; set; }
        public bool Giam_thi_khong_cung_mon { get; set; }
        public bool Bai_thi_tu_chon { get; set; }
    }
}
