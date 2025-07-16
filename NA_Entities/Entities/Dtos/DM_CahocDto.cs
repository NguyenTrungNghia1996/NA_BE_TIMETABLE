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
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ghichu { get; set; } = string.Empty;
        public bool Trang_thai_xoa = false;
    }
    public class DM_Cahoc_ListDto
    {
        public int STT { get; set; } = 0;
        public int Id { get; set; } = 0;
        public string Ten { get; set; }= string.Empty;
        public string Ghichu { get; set; }=string.Empty;
    }
    public class Ca_banDto
    {
        public int Id { get; set; } = 0;
        public List<Ngay_banDto> Ds_Ngay { get; set; } = new List<Ngay_banDto>();
    }

}
