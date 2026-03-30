using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_ThisinhDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Ho_va_ten { get; set; }
        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        public DateTime Ngay_sinh { get; set; }
        [Required(ErrorMessage = "Nơi sinh không được để trống")]
        public int Noi_sinh_xa { get; set; }
        public int Dan_toc { get; set; }
        [Required(ErrorMessage = "CCCD không được để trống")]
        public string CCCD { get; set; }
        [Required(ErrorMessage = "Thường trú không được để trống")]
        public int Thuong_tru_xa { get; set; }
        [Required(ErrorMessage = "Điểm thi không được để trống")]
        public int Id_diem_thi { get; set; }
        [Required(ErrorMessage = "Môn thi 1 không được để trống")]
        public int Mon_thi_1 { get; set; }
        [Required(ErrorMessage = "Môn thi 2 không được để trống")]
        public int Mon_thi_2 { get; set; }
    }
    public class DM_Thisinh_ListDto
    {
        public int Id { get; set; }
        public string? So_bao_danh { get; set; }
        public string Ho_va_ten { get; set; }
        public DateTime Ngay_sinh { get; set; }
        public string Ten_noi_sinh { get; set; }
        public int Noi_sinh_xa { get; set; }
        public string Ten_dan_toc { get; set; }
        public int Dan_toc { get; set; }
        public string CCCD { get; set; }
        public string Ten_thuong_tru { get; set; }
        public int Thuong_tru_xa { get; set; }
        public int Id_diem_thi { get; set; }
        public string Ten_mon_1 { get; set; }
        public int Mon_thi_1 { get; set; }
        public string Ten_mon_2 { get; set; }
        public int Mon_thi_2 { get; set; }
    }
}
