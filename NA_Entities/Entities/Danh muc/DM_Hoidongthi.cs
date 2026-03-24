using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Hoidongthi
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Mã hội đồng không được để trống")]
        [StringLength(2, ErrorMessage = "Tối đa 2 ký tự")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Chỉ được nhập số")]
        public string Ma { get; set; }
        [Required(ErrorMessage = "Tên hội đồng không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; }
        public int Id_don_vi { get; set; }
        public int Id_nam { get; set; }

    }
    public class DM_Hoidongthi_List
    {
        public int Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int Id_don_vi { get; set; }
        public string Ten_don_vi { get; set; }
        public int Id_nam { get; set; }
        public string Ten_nam { get; set; }
        public int Total { get; set; }

    }
}
