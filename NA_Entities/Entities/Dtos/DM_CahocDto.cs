using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_CahocDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên ca học không được để trống")]
        [StringLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ghichu { get; set; } = string.Empty;
    }
    public class DM_Cahoc_ListDto
    {
        public int STT { get; set; } = 0;
        public int Id { get; set; } = 0;
        public string Ten { get; set; }= string.Empty;
        public string Ghichu { get; set; }=string.Empty;
    }
    public class TietbanDto
    {
        public Tiet Id { get; set; } = Tiet.tiet_mot;
        public string Ten { get; set; }
        public bool Trang_thai { get; set; } = false;
    }
    public class Ngay_banDto
    {
        public Ngay Id { get; set; } = Ngay.thu_hai;
        public string Ten { get; set; }
        public List<TietbanDto> Ds_Tiet { get; set; } = new List<TietbanDto>();
    }
    public class Ca_banDto
    {
        public int Id { get; set; } = 0;
        //public string Ten { get; set; } = string.Empty;
        public List<Ngay_banDto> Ds_Ngay { get; set; } = new List<Ngay_banDto>();
    }
    public class Ca_Khoi_MonDto
    {
        public int Id_ca { get; set; }
        public string Ten_ca { get; set; }
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Không được nhập số thập phân")]
        [Range(0, 99, ErrorMessage = "Số tiết phải là số nguyên dương và nhỏ hơn 100")]
        public double? So_tiet { get; set; } = 0;
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Không được nhập số thập phân")]
        [Range(0, 99, ErrorMessage = "Số nhóm phải là số nguyên dương và nhỏ hơn 100")]
        public double? So_nhom { get; set; } = 0;
    }

}
