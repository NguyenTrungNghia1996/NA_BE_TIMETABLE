using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_DiemtruongDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên điểm trường không được để trống")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Diachi { get; set; } = string.Empty;
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ghichu { get; set; } = string.Empty ;

    }
    public class DM_Diemtruong_ListDto
    {
        public int STT { get; set; } = 0;
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public string Diachi { get; set; } = string.Empty;
        public string Ghichu { get; set; } = string.Empty;
    }
}
