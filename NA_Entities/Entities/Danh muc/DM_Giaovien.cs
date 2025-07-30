using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Giaovien
    {
        public int Id { get; set; } = 0;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Ma_giao_vien { get; set; } = string.Empty;
        [Required(ErrorMessage = "Họ và tên đệm không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ho_va_ho_dem { get; set; } = string.Empty ;
        [Required(ErrorMessage = "Tên đệm không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn tổ chuyên môn")]
        public int Id_to_chuyen_mon { get; set; } = 0;
        public int Id_don_vi { get; set; } = 0;
    }
    public class DM_Giaovien_List
    {
        public int Id { get; set; } = 0;
        public string Ma_giao_vien { get; set; } = string.Empty;
        public string Ho_va_ho_dem { get; set; } = string.Empty;
        public string Ten { get; set; } = string.Empty;
        public int Id_to_chuyen_mon { get; set; } = 0;
        public string Ten_to_chuyen_mon { get; set; } = string.Empty;
        public int Id_don_vi { get; set; } = 0;
        public int Ca_sang { get; set; } = 0;
        public int Ca_chieu { get; set; } = 0;
    }
}
