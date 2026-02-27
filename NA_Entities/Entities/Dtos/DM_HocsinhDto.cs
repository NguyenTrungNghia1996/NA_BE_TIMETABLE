using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{

    public class DM_Hocsinh_ListDto
    {
        public int Id { get; set; } = 0;
        public string Ma { get; set; } = string.Empty;
        public string Ten { get; set; } = string.Empty;
        public int Id_lop_chinh { get; set; } = 0;
        public string? Ten_lop { get; set; } = string.Empty;
        public int Id_khoi { get; set; } = 0;
    }
    public class DM_HocsinhDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Mã học sinh không được để trống")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Ma { get; set; } = string.Empty;
        [Required(ErrorMessage = "Tên học sinh không được để trống")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [Required(ErrorMessage = "Lớp chính khoá không được để trống")]
        public int Id_lop_chinh { get; set; }
    }
}
