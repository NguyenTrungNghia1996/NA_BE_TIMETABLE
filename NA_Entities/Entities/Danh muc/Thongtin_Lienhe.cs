using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Thongtin_Lienhe
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ho_ten { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Chỉ được nhập số")]
        public string So_dien_thoai { get; set; }
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }
        public string Dia_chi { get; set; }
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten_don_vi { get; set; }
        public string Ghi_chu { get; set; }
    }
}
