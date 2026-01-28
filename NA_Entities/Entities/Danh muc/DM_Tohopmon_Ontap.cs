using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Tohopmon_Ontap
    {
        public int Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int Id_don_vi { get; set; }
    }
    public class DM_Tohopmon_Ontap_List
    {
        public int Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int Id_don_vi { get; set; }
        public string Ten_mon { get; set; }
        public int Total { get; set; }
    }
}
