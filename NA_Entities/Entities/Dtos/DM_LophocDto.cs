using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_LophocDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên lớp học không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        public int Id_khoi { get; set; } = 0;
        [Range(1, int.MaxValue, ErrorMessage = "Sĩ số phải là số dương")]
        public int? Si_so { get; set; } = 0;
        [Required(ErrorMessage = "Vui lòng chọn ca học")]
        public int Id_ca { get; set; } = 0;
        [Required(ErrorMessage = "Vui lòng chọn giáo viên chủ nhiệm")]
        public int Id_gvcn { get; set; } = 0;
        [Required(ErrorMessage = "Vui lòng chọn phòng học")]
        public int Id_phong { get; set; } = 0;
        [Required(ErrorMessage = "Vui lòng chọn ban học")]
        public int Id_ban { get; set; } = 0;
        public int Id_don_vi { get; set; } = 0;
    }
    public class DM_Lophoc_ListDto
    {
        public int STT { get; set; } = 0;
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public int Id_khoi { get; set; } = 0;
        public int Si_so { get; set; } = 0;
        public int Id_ca { get; set; } = 0;
        public int Id_gvcn { get; set; } = 0;
        public int Id_phong { get; set; } = 0;
        public int Id_ban { get; set; } = 0;
        public int Id_don_vi { get; set; } = 0;
        public string Ten_khoi { get; set; } = string.Empty;
        public string Ten_ban { get; set; } = string.Empty;
        public string Ten_giao_vien { get; set; } = string.Empty;
        public string Ten_phong { get; set; } = string.Empty;
        public string Ten_ca { get; set; } = string.Empty;
    }
    public class Lophoc_banDto
    {
        public int Id_lop { get; set; } = 0;
        public List<Ca_banDto> Ds_Ca { get; set; } = new List<Ca_banDto>();
    }
}
