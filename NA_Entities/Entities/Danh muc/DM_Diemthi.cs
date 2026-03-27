using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Diemthi
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Mã điểm thi không được để trống")]
        [StringLength(10, ErrorMessage = "Tối đa 10 ký tự")]
        public string Ma { get; set; }
        [Required(ErrorMessage = "Tên điểm thi không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ten { get; set; }
        [Required(ErrorMessage = "Số giám thị 1 phòng không được để trống")]
        public int So_giam_thi_1_phong { get; set; }
        public bool Co_giam_sat { get; set; }
        [Required(ErrorMessage = "Số phòng giám sát không được để trống")]
        public int? So_phong_giam_sat_toi_da { get; set; }
        [Required(ErrorMessage = "Số thí sinh 1 phòng không được để trống")]
        public int? So_thi_sinh_1_phong{ get; set; }
        public int Id_don_vi { get; set; }
        public int Id_hoi_dong { get; set; }
    }
    public class DM_Diemthi_List
    {
        public int Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int So_giam_thi_1_phong { get; set; }
        public int So_thi_sinh_1_phong { get; set; }
        public bool Co_giam_sat { get; set; }
        public int? So_phong_giam_sat_toi_da { get; set; }
        public string Ten_don_vi { get; set; }
        public int Id_don_vi { get; set; }
        public string Ten_hoi_dong { get; set; }
        public int Id_hoi_dong { get; set; }
        public int Total { get; set; }
    }
}
