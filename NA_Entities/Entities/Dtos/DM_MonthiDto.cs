using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_MonthiDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Mã môn không được để trống")]
        [StringLength(10, ErrorMessage = "Tối đa 10 ký tự")]
        public string Ma { get; set; }
        [Required(ErrorMessage = "Tên môn không được để trống")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Ten { get; set; }
        public int Id_hoi_dong { get; set; }
        public int? Id_cha { get; set; }
        public bool La_mon_tu_chon { get; set; }
    }
    public class DM_Monthi_ListDto
    {
        public int Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string Ten_hoi_dong { get; set; }
        public string Ten_nam { get; set; }
        public int Id_hoi_dong { get; set; }
        public int? Id_cha { get; set; }
        public bool La_mon_tu_chon { get; set; }
    }
}
