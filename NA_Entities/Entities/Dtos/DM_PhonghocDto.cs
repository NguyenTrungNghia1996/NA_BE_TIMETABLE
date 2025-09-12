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
        public string Ma { get; set; } = string.Empty;
        [Required(ErrorMessage = "Tên phòng học không được để trống")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [Required(ErrorMessage = "Sức chứa không được để trống")]
        [Range(1,99999, ErrorMessage = "Sức chứa phải là số dương và tối đa 5 chữ số")]
        public int Suc_chua { get; set; } = 0;
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn loại phòng học")]
        public int Id_Loai_phong_hoc { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn điểm trường học")]
        public int Id_Diem_truong { get; set; }
        public bool Khong_kiem_tra_xung_dot { get; set; }

    }
    public class DM_Phonghoc_listDto
    {
        public int Stt { get; set; } = 0;
        public int Id { get; set; } = 0;
        public string Ma { get; set; } = string.Empty;
        public string Ten { get; set; } = string.Empty;
        public int Suc_chua { get; set; } = 0;
        public int Id_Loai_phong_hoc { get; set; }
        public string Ten_loai_phong_hoc { get; set; } = string.Empty;
        public string Ten_diem_truong { get; set; } = string.Empty;
        public bool Khong_kiem_tra_xung_dot { get; set; } = false;
        public int Ca_sang { get; set; } = 0;
        public int Ca_chieu { get; set; } = 0;
    }
    public class Phong_banDto
    {
        public int Id { get; set; } = 0;
        public List<Ca_banDto> Ds_Ca { get; set; } = new List<Ca_banDto>();
    }
}
