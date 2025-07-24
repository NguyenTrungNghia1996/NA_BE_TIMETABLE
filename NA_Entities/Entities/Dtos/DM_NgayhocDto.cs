using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_NgayhocDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên đơn vị không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ghi_chu { get; set; }
    }
    public class DM_Ngayhoc_ListDto
    {
        public int? Stt = 0;
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public string Ghi_chu { get; set; } = string.Empty;

    }
    public class Ngay_banDto
    {
        public Ngay Id { get; set; } = Ngay.thu_hai;
        public string Ten { get; set; }
        public List<TietbanDto> Ds_Tiet { get; set; } = new List<TietbanDto>();
    }
}