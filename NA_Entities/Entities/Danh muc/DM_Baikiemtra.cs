using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Baikiemtra
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên bài kiểm tra không được để trống")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Ten { get; set; }
        [Required(ErrorMessage = "Loại kiểm tra không được để trống")]
        public int Id_loai_kiem_tra { get; set; }
        [Required(ErrorMessage = "Lớp ôn không được để trống")]
        public int Id_lop_on { get; set; }
    }
    public class DM_Baikiemtra_List
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public int Id_loai_kiem_tra { get; set; }
        public int Id_lop_on { get; set; }
        public string Ten_loai_kiem_tra { get; set; }
        public string Ten_lop_on { get; set; }
        public int Total { get; set; }
    }
}
