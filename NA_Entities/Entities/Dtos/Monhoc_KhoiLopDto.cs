using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class Monhoc_KhoiLopDto
    {
        public int Id { get; set; }
        public int Id_ban { get; set; }
        public int Id_khoi { get; set; }
        public List<Mon_KhoiDto> ds_Mon { get; set; } = new List<Mon_KhoiDto>();
    }
}
