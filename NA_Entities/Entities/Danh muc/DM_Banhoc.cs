using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Banhoc
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Tên ca học không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; } = string.Empty;
        public int Id_cap_hoc { get; set; } = 0;
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ghichu { get; set; } =string.Empty;
        public int Id_don_vi { get; set; } = 0;
    }
    public class DM_Banhoc_List
    {
        public int STT { get; set; }
        public int Id { get; set; }
        public string Ten { get; set; }
        public string Ten_cap { get; set; }
        public string Ghichu { get; set; }
        public int Id_don_vi { get;set; }
    }
}
