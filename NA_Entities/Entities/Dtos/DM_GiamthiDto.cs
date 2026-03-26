using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_GiamthiDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Mã giám thị không được để trống")]
        [StringLength(10, ErrorMessage = "Tối đa 10 ký tự")]
        public string Ma { get; set; }
        [Required(ErrorMessage = "Tên giám thị không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ho_va_ten { get; set; }
        public int Id_diem_thi { get; set; }
    }
    public class DM_Giamthi_ListDto
    {
        public int Id { get; set; }
        public string Ma { get; set; }
        public string Ho_va_ten { get; set; }
        public int? Id_giao_vien { get; set; }
        public int Id_diem_thi { get; set; }
    }
}
