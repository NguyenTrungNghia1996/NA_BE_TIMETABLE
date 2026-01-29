using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Hocsinh_Tohopmon
    {
        public int Id { get; set; }
        public int Id_to_hop { get; set; }
        public int Id_hoc_sinh { get; set; }
    }
    public class Hocsinh_Tohopmon_Multi
    {
        public int Id_hoc_sinh { get; set; }
        public List<int> To_hop_mon { get; set; } = new List<int>();
    }
}
