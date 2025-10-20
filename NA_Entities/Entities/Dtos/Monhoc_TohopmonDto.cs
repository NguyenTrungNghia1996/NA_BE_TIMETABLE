using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class Monhoc_TohopmonDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên tổ hợp môn không được để trống")]
        [StringLength(30, ErrorMessage = "Tối đa 30 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [Required(ErrorMessage = "Tên môn 1 không được để trống")]
        public int Id_mon_1 { get; set; } = 0;
        [Required(ErrorMessage = "Tên môn 2 không được để trống")]
        public int Id_mon_2 { get; set; } = 0;
        public int Id_mon_3 { get; set; } = 0;
        [Required(ErrorMessage = "Số tiết tối đa một ca không được để trống")]
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Không được nhập số thập phân")]
        [Range(1, 9, ErrorMessage = "Số tiết tối đa một ca phải lớn hơn 0 và nhỏ hơn 10")]
        public double? So_tiet_toi_da_1_ca { get; set; }
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Không được nhập số thập phân")]
        public double? So_tiet_toi_da_2_ca { get; set; }
        [Required(ErrorMessage = "Ban học không được để trống")]
        public int Id_ban { get; set; } = 0;
        [Required(ErrorMessage = "Khối lớp không được để trống")]
        public int Id_khoi { get; set; } = 0;
    }
    public class Monhoc_Tohopmon_ListDto
    {
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public int Id_mon_1 { get; set; } = 0;
        public int Id_mon_2 { get; set; } = 0;
        public int? Id_mon_3 { get; set; } = 0;
        public int So_tiet_toi_da_1_ca { get; set; } = 0;
        public int? So_tiet_toi_da_2_ca { get; set; }
        public int Id_ban { get; set; } = 0;
        public int Id_khoi { get; set; } = 0;
        public string Ten_khoi { get; set; } = string.Empty;
        public string Ten_ban { get; set; } = string.Empty;
        public string Ten_mon_hoc_1 { get; set; } = string.Empty;
        public string Ten_mon_hoc_2 { get; set; } = string.Empty;
        public string? Ten_mon_hoc_3 { get; set; } = string.Empty;
        public int Id_don_vi { get; set; } = 0;

    }
}
