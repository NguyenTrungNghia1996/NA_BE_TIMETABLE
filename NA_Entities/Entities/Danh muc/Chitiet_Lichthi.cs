using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Chitiet_Lichthi
    {
        public int Id { get; set; }
        public int Id_lich { get; set; }
        public int? Id_phong { get; set; }
        public int Id_giam_thi { get; set; }
        public int Loai_giam_thi { get; set; }
        public bool La_phong_cho { get; set; } = false;
    }
    public class PhongTheoGiamThi
    {
        public DM_Giamthi GiamThi { get; set; }
        public List<DM_Phongthi> PhongCoThe { get; set; }
    }
}
