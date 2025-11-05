using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Lich_Baogiang
    {
        public int Id { get; set; }
        public int Id_nam_hoc { get; set; }
        public int Tuan { get; set; }
        public DateTime Tu_ngay { get; set; }
        public DateTime Den_ngay { get; set; }
        public int Id_tkb { get; set; }
    }
    public class Lich_Baogiang_List
    {
        public int STT { get; set; }
        public int Id { get; set; }
        public int Id_nam_hoc { get; set; }
        public int Tuan { get; set; }
        public DateTime Tu_ngay { get; set; }
        public DateTime Den_ngay { get; set; }
        public int Id_tkb { get; set; }
    }
}
