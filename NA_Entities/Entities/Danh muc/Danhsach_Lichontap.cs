using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Danhsach_Lichontap
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên ca học không được để trống")]
        [StringLength(50,ErrorMessage = "Tối đa 50 kí tự")]
        public string Ten { get; set; } = string.Empty;
        public int Id_don_vi { get; set; }
        public bool Trang_thai { get; set; } = false;
    }
    public class Danhsach_Lichontap_List
    {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public int Id_don_vi { get; set; }
        public bool Trang_thai { get; set; } = false;
    }
    public class Lichontap_Detail
    {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public int Id_don_vi { get; set; }
        public bool Trang_thai { get; set; } = false;
        public int Tong_tat_ca_tiet { get; set; } = 0;
        public int Tong_tiet_da_xep { get; set; } = 0;
        public int Tong_tiet_chua_xep { get; set; } = 0;
    }
}
