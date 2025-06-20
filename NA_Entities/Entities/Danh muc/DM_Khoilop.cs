using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danhmuc
{
    public class DM_Khoilop
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên đơn vị không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn cấp học")]
        public int Id_Cap_hoc { get; set; }
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ghi_chu { get; set; } = string.Empty ;
        public bool Trang_thai_xoa { get; set; } = false;
    }
    public class DM_Khoilop_List
    {
        public int? Stt = 0;
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public string Ten_cap_hoc { get; set; } = string.Empty;
        public string Ghi_chu { get; set; }= string.Empty;

    }
}