using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class Hocsinh_Lopon
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn học sinh")]
        public int Id_hoc_sinh { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn lớp ôn")]
        public int Id_lop_on { get; set; }
    }
    public class Hocsinh_Lopon_List
    {
        public int Id { get; set; }
        public int Id_hoc_sinh { get; set; }
        public int Id_lop_on { get; set; }
        public string Ma_hoc_sinh { get; set; }
        public string Ten_hoc_sinh { get; set; }
        public string Ten_lop { get; set; }
        public string Ten_lop_chinh { get; set; }
        public int Id_khoi { get; set; }
        public int Total { get; set; }
    }
    public class Hocsinh_Lopon_Multi
    {
        public int Id_lop {  get; set; }
        public List<int> Hoc_sinh { get; set; } = new List<int>();
    }
    public class Lopon_Hocsinh
    {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public List<Hocsinh_List> Hoc_sinh { get; set; } = new List<Hocsinh_List>();
    }
}
