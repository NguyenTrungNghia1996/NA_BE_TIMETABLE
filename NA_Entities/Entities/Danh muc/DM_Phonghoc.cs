using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Phonghoc
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Mã môn học không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 20 ký tự")]
        public string Ma { get; set; } = string.Empty;                                                                                     
        [Required(ErrorMessage = "Tên phòng học không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [Required(ErrorMessage = "Sức chứa không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Sức chứa phải là số dương")]
        public int Suc_chua { get; set; } = 0;
        [Required(ErrorMessage = "Vui lòng chọn loại phòng học")]
        public int Id_Loai_phong_hoc { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ca học")]
        public int Id_Diem_truong { get; set; }
        public bool Khong_kiem_tra_xung_dot {  get; set; }

    }
    public class DM_Phonghoc_list
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
