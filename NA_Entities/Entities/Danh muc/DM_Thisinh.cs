using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Thisinh
    {
        public int Id { get; set; }
        public string? So_bao_danh { get; set; }
        public string Ho_va_ten { get;set; }
        public DateTime Ngay_sinh { get; set; }
        public int Noi_sinh_xa { get; set; }
        public int Dan_toc { get; set; }
        public string CCCD { get; set; }
        public int Thuong_tru_xa { get; set; }
        public int? Id_truong { get; set; }
        public int Id_diem_thi { get; set; }
        public int? Mon_thi_1 { get; set; }
        public int? Mon_thi_2 { get; set; }
    }
    public class DM_Thisinh_Detail
    {
        public int Id { get; set; }
        public string? So_bao_danh { get; set; }
        public string Ho_va_ten { get;set; }
        public DateTime Ngay_sinh { get; set; }
        public int Noi_sinh_xa { get; set; }
        public int Noi_sinh_tinh { get; set; }
        public int Dan_toc { get; set; }
        public string CCCD { get; set; }
        public int Thuong_tru_xa { get; set; }
        public int Thuong_tru_tinh { get; set; }
        public int? Id_truong { get; set; }
        public int Id_diem_thi { get; set; }
        public int? Mon_thi_1 { get; set; }
        public int? Mon_thi_2 { get; set; }
        public int Id_hoi_dong { get; set; }
        public int Id_nam { get; set; }
    }
    public class ThiSinhCheck
    {
        public string? Ho_va_ten { get; set; }
        public DateTime? Ngay_sinh { get; set; }
        public int? Noi_sinh { get; set; }
        public int? Noi_thuong_tru { get; set; }
        public int? Dan_toc { get; set; }
        public string? CCCD { get; set; }
        public string? Ma_diem_thi { get; set; }
        public int? Mon_1 { get; set; }
        public int? Mon_2 { get; set; }
        public bool? IsDuplicate { get; set; }
    }
    public class DM_Thisinh_List
    {
        public int Id { get; set; }
        public string? So_bao_danh { get; set; }
        public string Ho_va_ten { get;set; }
        public DateTime Ngay_sinh { get; set; }
        public string Ten_noi_sinh { get; set; }
        public int Noi_sinh_xa { get; set; }
        public string Ten_dan_toc { get; set; }
        public int Dan_toc { get; set; }
        public string CCCD { get; set; }
        public string Ten_thuong_tru { get; set; }
        public int Thuong_tru_xa { get; set; }
        public int? Id_truong { get; set; }
        public int Id_diem_thi { get; set; }
        public string? Ten_mon_1 { get; set; }
        public int? Mon_thi_1 { get; set; }
        public string? Ten_mon_2 { get; set; }
        public int? Mon_thi_2 { get; set; }
        public int Total { get; set; }
    }
}
