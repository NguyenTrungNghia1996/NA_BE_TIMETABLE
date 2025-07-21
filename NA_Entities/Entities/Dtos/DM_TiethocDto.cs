using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_TiethocDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên tiết học không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        public int Id_Donvi { get; set; } = 0;
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một ca học")]
        public List<int> Id_Ca_hoc { get; set; } = new List<int>();    }
    public class DM_Tiethoc_ListDto
    {
        public int Stt { get; set; } = 0;
        public int Id { get; set; } = 0;
        public string Ten { get; set; } = string.Empty;
        public int Id_Donvi { get; set; } = 0;
    }
    public class DM_Tiethoc_updateDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên tiết học không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một ca học")]
        public List<int> Id_Ca_hoc { get; set; } = new List<int>();
    }
    public class TietbanDto
    {
        public Tiet Id { get; set; } = Tiet.tiet_mot;
        public bool Trang_thai { get; set; } = false;
    }
}
