using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{

    public class tkb_theo_lop
    {
        public int Id_chitiet { get; set; } = 0;
        public int Id_don_vi { get; set; } = 0;
        public int Id_tkb { get; set; } = 0;
        public int Id_mon { get; set; } = 0;
        public string Ten_mon { get; set; } = string.Empty;
        public int Id_giao_vien { get; set; } = 0;
        public string Ten_giao_vien { get; set; } = string.Empty;
        public int Id_phong { get; set; } = 0;
        public string? Ten_phong { get; set; } = string.Empty;
        public int Tiet_thu_may { get; set; } = 0;
        public int Id_ca { get; set; } = 0;
        public int Ngay { get; set; } = 0;
        public int Tiet { get; set; } = 0;
        public bool isDrag { get; set; } = false;
        public bool isLock { get; set; } = false;
        public bool isRest { get; set; } = false;
    }
    public class tkb_chuaxep_lop
    {
        public int Id_chitiet { get; set; } = 0;
        public int Id_don_vi { get; set; } = 0;
        public int Id_tkb { get; set; } = 0;
        public int Id_mon { get; set; } = 0;
        public string Ten_mon { get; set; } = string.Empty;
        public int Id_giao_vien { get; set; } = 0;
        public string Ten_giao_vien { get; set; } = string.Empty;
        public int Id_phong { get; set; } = 0;
        public string? Ten_phong { get; set; } = string.Empty;
        public int So_tiet { get; set; } = 0;
    }

    public class ObjectTiet_theoLopDto
    {
        public int Id_lop { get; set; } = 0;
        public string Ten_lop { get; set; } = string.Empty;
        public List<tkb_theo_lop> timetable { get; set; } = new List<tkb_theo_lop>();
        public List<tkb_chuaxep_lop> ds_chua_xep { get; set; } = new List<tkb_chuaxep_lop>();
    }
    public class tkb_theo_giaovien
    {
        public int Id_chitiet { get; set; } = 0;
        public int Id_don_vi { get; set; } = 0;
        public int Id_tkb { get; set; } = 0;
        public int? Id_lop { get; set; } = 0;
        public string? Ten_lop { get; set; } = string.Empty;
        public int? Id_mon { get; set; } = 0;
        public string? Ten_mon { get; set; } = string.Empty;
        public int? Id_phong { get; set; } = 0;
        public string? Ten_phong { get; set; } = string.Empty;
        public int Tiet_thu_may { get; set; } = 0;
        public int Id_ca { get; set; } = 0;
        public int Ngay { get; set; } = 0;
        public int Tiet { get; set; } = 0;
        public bool isDrag { get; set; } = false;
        public bool isLock { get; set; } = false;
        public bool isRest { get; set; } = false;
    }
    public class tkb_chuaxep_giaovien
    {
        public int Id_chitiet { get; set; } = 0;
        public int Id_don_vi { get; set; } = 0;
        public int Id_tkb { get; set; } = 0;
        public int? Id_lop { get; set; } = 0;
        public string? Ten_lop { get; set; } = string.Empty;
        public int? Id_mon { get; set; } = 0;
        public string? Ten_mon { get; set; } = string.Empty;
        public int? Id_phong { get; set; } = 0;
        public string? Ten_phong { get; set; } = string.Empty;
        public int So_tiet { get; set; } = 0;
    }
    public class ObjectTiet_theoGVDto
    {
        public int Id_giao_vien { get; set; } = 0;
        public string Ten_giao_vien { get; set; } = string.Empty;
        public List<tkb_theo_giaovien> timetable { get; set; } = new List<tkb_theo_giaovien>();
        public List<tkb_chuaxep_giaovien> ds_chua_xep { get; set; } = new List<tkb_chuaxep_giaovien>();
    }
    public class ObjectTiet_DaChon
    {

        public int? Id_chitiet { get; set; } = 0;
        public int Id_don_vi { get; set; } = 0;
        public int Id_tkb { get; set; } = 0;
        public int? Id_lop { get; set; } = 0;
        public string? Ten_lop { get; set; } = string.Empty;
        public int? Id_mon { get; set; } = 0;
        public string? Ten_mon { get; set; } = string.Empty;
        public int? Id_giao_vien { get; set; } = 0;
        public string? Ten_giao_vien { get; set; } = string.Empty;
        public int? Id_phong { get; set; } = 0;
        public string? Ten_phong { get; set; } = string.Empty;
        public int? Tiet_thu_may { get; set; } = 0;
        public int? Id_ca { get; set; } = 0;
        public int? Ngay { get; set; } = 0;
        public int? Tiet { get; set; } = 0;
        public bool isDrag { get; set; } = false;
        public bool isLock { get; set; } = false;
        public bool isRest { get; set; } = false;
    }
}
