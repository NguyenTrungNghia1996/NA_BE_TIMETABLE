using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_Hoidongthi_ListDto
    {
        public int Id { get; set; }
        public int Ma { get; set; }
        public string Ten { get; set; }
        public int Id_don_vi { get; set; }
        public string Ten_don_vi { get; set; }
        public int Id_nam { get; set; }
        public string Ten_nam { get; set; }
    }
}
