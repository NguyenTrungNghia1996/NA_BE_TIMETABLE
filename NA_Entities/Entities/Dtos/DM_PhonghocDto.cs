using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_PhonghocDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên phòng học không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [Required(ErrorMessage = "Sức chứa không được để trống")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Sức chứa phải là số nguyên dương")]
        public string Suc_chua { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn loại phòng học")]
        public int Id_Loai_phong_hoc { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn điểm trường học")]
        public int Id_Diem_truong { get; set; }
        public bool Khong_kiem_tra_xung_dot { get; set; }
        public int Id_Don_vi { get; set; }

    }
    public class DM_Phonghoc_listDto
    {
        public int Stt { get; set; } = 0;
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public int Suc_chua { get; set; } = 0;
        public string Ten_loai_phong_hoc { get; set; } = string.Empty;
        public string Ten_diem_truong { get; set; } = string.Empty;
        public bool Khong_kiem_tra_xung_dot { get; set; } = false;
    }
}
