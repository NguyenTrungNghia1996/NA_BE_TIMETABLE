using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Diemtruong
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên ca học không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string? Diachi { get; set; } 
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string? Ghichu { get; set; }
        public int Id_Donvi { get; set; }

    }
    public class DM_Diemtruong_List
    {
        public int STT { get; set; } = 0;
        public int Id { get; set; } = 0;
        public string Ten { get; set; }=string.Empty;
        public string Ghichu { get; set; } =string.Empty;
        public int Id_Donvi { get; set; }
    }
}
