using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_Baikiemtra_ListDto
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public int Id_loai_kiem_tra { get; set; }
        public int Id_lop_on { get; set; }
        public string Ten_loai_kiem_tra { get; set; }
        public string Ten_lop_on { get; set; }
    }
}
