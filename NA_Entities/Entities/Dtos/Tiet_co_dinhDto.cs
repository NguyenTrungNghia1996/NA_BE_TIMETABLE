using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class Tiet_co_dinhDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Vui lòng chọn môn")]
        public int Id_mon { get; set; } = 0;
        [Required(ErrorMessage = "Vui lòng chọn ngày")]
        public int Id_ngay { get; set; } = 0;
        [Required(ErrorMessage = "Vui lòng chọn ca học")]
        public int Id_ca { get; set; } = 0;
        [Required(ErrorMessage = "Vui lòng chọn tiết học")]
        public int Id_tiet { get; set; } = 0;
        public int Id_khoi_lop { get; set; } = 0;
        public bool Ap_dung_cho_tat_ca_cac_khoi { get; set; } = false;
        public int Id_don_vi { get; set; } = 0;
    }
    public class Tiet_co_dinh_ListDto
    {
        public int STT { get; set; } = 0;
        public string Ten_mon_hoc { get; set; } = string.Empty;
        public string Ten_khoi_lop { get; set; } = string.Empty;
        public string Ten_ca_hoc { get; set; } = string.Empty;
        public string Ten_ngay_hoc { get; set; } = string.Empty;
        public string Ten_tiet_hoc { get; set; } = string.Empty;
    }
}
