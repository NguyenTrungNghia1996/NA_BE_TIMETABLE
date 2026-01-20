using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_LoaikiemtraDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên loại kiểm tra không được để trống")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Ten { get; set; }
    }
    public class DM_Loaikiemtra_ListDto
    {
        public int Id { get; set; }
        public string Ten { get; set; }
    }
}
