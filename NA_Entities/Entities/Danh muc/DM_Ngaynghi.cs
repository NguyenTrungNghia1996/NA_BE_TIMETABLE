using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Ngaynghi
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public DateTime Tu_ngay { get; set; }
        public DateTime Den_ngay { get; set; }
    }
    public class DM_Ngaynghi_List
    {
        public int STT { get; set; }
        public int Id { get; set; }
        public string Ten { get; set; }
        public DateTime Tu_ngay { get; set; }
        public DateTime Den_ngay { get; set; }
    }

}
