using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_DonviHanhchinh
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public int? Id_cha { get; set; }
        public string? Ten_cha { get; set; }
    }
}
