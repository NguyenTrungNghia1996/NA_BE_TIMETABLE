using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Phieu_Baogiang
    {
        public int Id { get; set; }
        public int Id_lich_bao_giang { get; set; }
        public int Id_giao_vien { get; set; }
    }
    public class Phieu_Baogiang_List
    {
        public int Id { get; set; }
        public int Id_lich_bao_giang { get; set; }
        public int Id_giao_vien { get; set; }
    }
}
