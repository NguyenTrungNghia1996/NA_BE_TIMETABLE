using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class MonHoc_TheoNganh
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Mã môn học không được để trống")]
        public int Id_mon { get; set; }
        [Required(ErrorMessage = "Tên môn theo ngành không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten_mon_theo_nganh { get; set; }
    }
}
