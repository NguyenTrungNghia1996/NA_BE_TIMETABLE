using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Phongthi
    {
        public int Id { get; set; }
        public int So_phong { get; set; }
        public string Toa { get; set; }
        public int Tang { get; set; }
        public int Id_diem_thi { get; set; }
    }
    public class DM_Phongthi_List
    {
        public int Id { get; set; }
        public int So_phong { get; set; }
        public string Toa { get; set; }
        public int Tang { get; set; }
        public int Id_diem_thi { get; set; }
        public int Total { get; set; }
    }
}
