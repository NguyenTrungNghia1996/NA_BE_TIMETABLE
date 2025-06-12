using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Tiethoc
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên tiết học không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn ca học")]
        public int Id_Ca_hoc { get; set; }
    }
    public class DM_Tiethoc_List
    {
        public int Stt { get; set; } = 0;
        public int Id { get; set;} = 0;
        public string Ten { get; set; }= string.Empty;
    }
}
