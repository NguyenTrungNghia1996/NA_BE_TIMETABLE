using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danhmuc
{
    public class DM_Ngayhoc
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên đơn vị không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ghi_chu { get; set; }
    }
    public class DM_Ngayhoc_List
    {
        public int? Stt = 0;
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public string Ghi_chu { get; set; } = string.Empty;

    }
}