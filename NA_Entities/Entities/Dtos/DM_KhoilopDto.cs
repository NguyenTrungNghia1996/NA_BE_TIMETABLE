using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_KhoilopDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên khối lớp không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn cấp học")]
        public int Id_Cap_hoc { get; set; }
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ghi_chu { get; set; } = string.Empty;
    }
    public class DM_Khoilop_ListDto
    {
        public int? Stt = 0;
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public string Ten_cap_hoc { get; set; } = string.Empty;
        public string Ghi_chu { get; set; } = string.Empty;

    }
    public class Khoilop_byDonvi
    {
        public int Id { get; set; }
        public string Ten { get; set; }
    }
}