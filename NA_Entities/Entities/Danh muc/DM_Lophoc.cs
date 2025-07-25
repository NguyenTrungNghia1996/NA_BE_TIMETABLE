using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Lophoc
    {
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public int Id_khoi { get; set; } = 0;
        public int Si_so { get; set; } = 0;
        public int Id_ca { get; set; } = 0;
        public int Id_gvcn { get; set; } = 0;
        public int Id_phong { get; set; } = 0;
        public int Id_ban { get; set; } = 0;
        public int Id_don_vi { get; set; } = 0;
    }
    public class DM_Lophoc_List
    {
        public int STT { get; set; } = 0;
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public int Id_khoi { get; set; } = 0;
        public int Si_so { get; set; } = 0;
        public int Id_ca { get; set; } = 0;
        public int Id_gvcn { get; set; } = 0;
        public int Id_phong { get; set; } = 0;
        public int Id_ban { get; set; } = 0;
        public int Id_don_vi { get; set; } = 0;
    }
}
