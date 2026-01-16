using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class Hocsinh_Lopon_ListDto
    {
        public int Id { get; set; }
        public int Id_hoc_sinh { get; set; }
        public int Id_lop_on { get; set; }
        public string Ten_hoc_sinh { get; set; }
        public string Ten_lop { get; set; }
    }
}
