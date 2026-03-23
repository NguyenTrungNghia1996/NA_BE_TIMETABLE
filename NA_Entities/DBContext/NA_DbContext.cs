using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.DBContext
{
    public class NA_DbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public NA_DbContext(DbContextOptions<NA_DbContext> options, IConfiguration configuration) : base(options) {
            _configuration = configuration;
        }
        public DbSet<Auth_Users> Auth_Users { get; set; }
        public DbSet<Auth_Users_List> Auth_Users_List { get; set; }
        public DbSet<DM_Caphoc> DM_Caphoc { get; set; }
        public DbSet<DM_Caphoc_List> DM_Caphoc_List { get; set; }
        public DbSet<DM_Donvi> DM_Donvi { get; set; }
        public DbSet<DM_Donvi_List> DM_Donvi_List { get; set; }
        public DbSet<Cap_Donvi> Cap_Donvi { get; set; }
        public DbSet<Ca_Donvi> Ca_Donvi { get; set; }
        // Auth_Roles
        public DbSet<Auth_Roles> Auth_Roles { get; set; }
        public DbSet<Auth_RolesList> Auth_RolesList { get; set; }
        // Auth_Users_Roles
        public DbSet<Auth_Users_Roles> Auth_Users_Roles { get; set; }
        public DbSet<Auth_Users_RolesList> Auth_Users_RolesList { get; set; }
        public DbSet<DM_Cahoc> DM_Cahoc { get; set; }
        public DbSet<DM_Cahoc_List> DM_Cahoc_List { get; set; }
        public DbSet<Auth_Roles_Permissions> Auth_Roles_Permissions { get; set; }
        public DbSet<Auth_Menus> Auth_Menus { get; set; }
        public DbSet<Auth_MenusList> Auth_MenusList { get; set; }
        public DbSet<DM_Diemtruong> DM_Diemtruong { get; set; }
        public DbSet<DM_Diemtruong_List> DM_Diemtruong_List { get; set; }
        public DbSet<DM_Loaiphonghoc> DM_Loaiphonghoc {  get; set; }
        public DbSet<DM_Loaiphonghoc_List> DM_Loaiphonghoc_List { get; set; }
        public DbSet<DM_Khoikienthuc> DM_Khoikienthuc { get; set; }
        public DbSet<DM_Khoikienthuc_List> DM_Khoikienthuc_List { get; set; }
        public DbSet<DM_Tochuyenmon> DM_Tochuyenmon { get; set; }
        public DbSet<DM_Tochuyenmon_List> DM_Tochuyenmon_list { get; set; }
        public DbSet<DM_Khoilop> DM_Khoilop { get; set; }
        public DbSet<DM_Khoilop_List> DM_Khoilop_List { get; set; }
        public DbSet<DM_Phonghoc> DM_Phonghoc { get; set; }
        public DbSet<DM_Phonghoc_list> DM_Phonghoc_List { get; set; }
        public DbSet<Donvi_Tiethoc> Donvi_Tiethoc { get; set; }
        public DbSet<Tiet_ban> Tiet_ban { get; set; }
        public DbSet<DM_Monhoc> Dm_Monhoc { get; set; }
        public DbSet<DM_Monhoc_List> DM_Monhoc_List { get; set; }
        public DbSet<Mon_Khoikienthuc> Mon_Khoikienthuc { get; set; }
        public DbSet<Tiet_tranh_xep> Tiet_Tranh_Xep { get; set; }
        public DbSet<Tiet_co_dinh> Tiet_co_dinh { get; set; }
        public DbSet<Tiet_co_dinh_List> Tiet_co_dinh_list { get; set; }
        public DbSet<Monhoc_Phonghoc> Monhoc_Phonghoc { get; set; }
        public DbSet<DM_Banhoc> DM_Banhoc { get; set; }
        public DbSet<DM_Banhoc_List> DM_Banhoc_List { get; set; }
        public DbSet<Monhoc_Khoilop> Monhoc_Khoilop { get; set; }
        public DbSet<Monhoc_Tohopmon> Monhoc_Tohopmon { get; set; }
        public DbSet<Monhoc_Tohopmon_List> Monhoc_Tohopmon_List { get; set; }
        public DbSet<Monhoc_Khoilop_Tiettranhxep> Monhoc_Khoilop_Tiettranhxep { get; set; }
        public DbSet<DM_Giaovien> DM_Giaovien { get; set; }
        public DbSet<DM_Giaovien_List> DM_Giaovien_List { get; set; }
        public DbSet<Giaovien_Buoiday> Giaovien_Buoiday { get; set; }
        public DbSet<Giaovien_Tiettranhxep> Giaovien_Tiettranhxep { get; set; }
        public DbSet<Giaovien_Monhoc> Giaovien_Monhoc { get; set; }
        public DbSet<Giaovien_Diadiemday> Giaovien_Diadiemday { get; set; }
        public DbSet<DM_Lophoc> DM_Lophoc { get; set; }
        public DbSet<DM_Lophoc_List> DM_Lophoc_List { get; set; }
        public DbSet<Lophoc_Tietnghi> Lophoc_Tietnghi { get; set; }
        public DbSet<Lophoc_Monhoc> Lophoc_Monhoc { get; set; }
        public DbSet<Lophoc_Monhoc_List> Lophoc_Monhoc_List { get; set; }
        public DbSet<Lophoc_Monhoc_Tiettranhxep> Lophoc_Monhoc_Tiettranhxep { get; set; }
        public DbSet<MonLop> MonLop { get; set; }
        public DbSet<Danhsach_Thoikhoabieu> Danhsach_Thoikhoabieu { get; set; }
        public DbSet<Danhsach_ThoikhoabieuList> Danhsach_ThoikhoabieuList { get; set; }
        public DbSet<Chitiet_Thoikhoabieu> Chitiet_Thoikhoabieu { get; set; }
        public DbSet<Chitiet_Thoikhoabieu_List> Chitiet_Thoikhoabieu_List { get; set; }
        public DbSet<Sotiet_Giaovien> Sotiet_Giaovien { get; set; }
        public DbSet<Sotiet_Lop> Sotiet_Lop { get; set; }
        public DbSet<Sotiet_Mon> Sotiet_Mon { get; set; }
        public DbSet<Sotiet_Phong> Sotiet_Phong { get; set; }
        public DbSet<Sotiet_LopMon> Sotiet_LopMon { get; set; }
        public DbSet<Export> Export { get; set; }
        public DbSet<DM_Tinh> DM_Tinh { get; set; }
        public DbSet<Giaovien_Tochuyenmon> Giaovien_Tochuyenmon { get; set; }
        public DbSet<PhancongGV> PhancongGV { get; set; }
        public DbSet<DsLop_ByGVandMon> DsLop_ByGVandMon { get; set; }
        public DbSet<DM_Namhoc> DM_Namhoc { get; set; }
        public DbSet<DM_Namhoc_List> DM_Namhoc_List { get; set; }
        public DbSet<DM_Ngaynghi> DM_Ngaynghi { get; set; }
        public DbSet<DM_Ngaynghi_List> DM_Ngaynghi_List { get; set; }
        public DbSet<Phanphoi_Chuongtrinh> Phanphoi_Chuongtrinh { get; set; }
        public DbSet<Phanphoi_Chuongtrinh_List> Phanphoi_Chuongtrinh_List { get; set; }
        public DbSet<Phanphoi_Chuongtrinh_Chitiet> Phanphoi_Chuongtrinh_Chitiet { get; set; }
        public DbSet<Phanphoi_Chuongtrinh_Chitiet_List> Phanphoi_Chuongtrinh_Chitiet_List { get; set; }
        public DbSet<Lich_Baogiang> Lich_Baogiang { get; set; }
        public DbSet<Lich_Baogiang_List> Lich_Baogiang_List { get; set; }
        public DbSet<Phieu_Baogiang> Phieu_Baogiang { get; set; }
        public DbSet<Phieu_Baogiang_List> Phieu_Baogiang_List { get; set; }
        public DbSet<Chitiet_Phieubaogiang> Chitiet_Phieubaogiang { get; set; }
        public DbSet<Chitiet_Phieubaogiang_List> Chitiet_Phieubaogiang_List { get; set; }
        public DbSet<MonHoc_TheoNganh> MonHoc_TheoNganh { get; set; }
        public DbSet<DM_Lopontap> DM_Lopontap { get; set; }
        public DbSet<DM_Lopontap_List> DM_Lopontap_List { get; set; }
        public DbSet<Lopontap_Tietnghi> Lopontap_Tietnghi { get; set; }
        public DbSet<Danhsach_Lichontap> Danhsach_Lichontap { get; set; }
        public DbSet<Danhsach_Lichontap_List> Danhsach_Lichontap_List { get; set; }
        public DbSet<Chitiet_Lichontap> Chitiet_Lichontap { get; set; }
        public DbSet<Chitiet_Lichontap_List> Chitiet_Lichontap_List { get; set; }
        public DbSet<DM_Hocsinh> DM_Hocsinh { get; set; }
        public DbSet<DM_Hocsinh_List> DM_Hocsinh_List { get; set; }
        public DbSet<Hocsinh_Lopon> Hocsinh_Lopon { get; set; }
        public DbSet<Hocsinh_Lopon_List> Hocsinh_Lopon_List { get; set; }
        public DbSet<DM_Baikiemtra> DM_Baikiemtra { get; set; }
        public DbSet<DM_Baikiemtra_List> DM_Baikiemtra_List { get; set; }
        public DbSet<DM_Loaikiemtra> DM_Loaikiemtra { get; set; }
        public DbSet<DM_Loaikiemtra_List> DM_Loaikiemtra_List { get; set; }
        public DbSet<KetQua_Baikiemtra> KetQua_Baikiemtra { get; set; }
        public DbSet<DM_Tohopmon_Ontap_List> DM_Tohopmon_Ontap_List { get; set; }
        public DbSet<DM_Tohopmon_Ontap> DM_Tohopmon_Ontap { get; set; }
        public DbSet<Monhoc_Tohop_Ontap> Monhoc_Tohop_Ontap { get; set; }
        public DbSet<Hocsinh_Tohopmon> Hocsinh_Tohopmon { get; set; }
        public DbSet<DsLopMon_byGV> DsLopMon_byGV { get; set; }
        public DbSet<Thongtin_Lienhe> Thongtin_Lienhe { get; set; }
        public DbSet<Ketqua_Hocsinh> Ketqua_Hocsinh { get; set; }
        public DbSet<Ketqua_Hocsinh_ToHopMon> Ketqua_Hocsinh_ToHopMon { get; set; }
        public DbSet<DM_DonviHanhchinh> DM_DonviHanhchinh { get; set; }
        public DbSet<DM_Dantoc> DM_Dantoc { get; set; }
        public DbSet<DM_Hoidongthi> DM_Hoidongthi { get; set; }
        public DbSet<DM_Hoidongthi_List> DM_Hoidongthi_List { get; set; }

        //public DbSet<DM_Donvi_Demo> DM_Donvi_Demo { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Auth_Users_List>().HasNoKey();
            builder.Entity<DM_Caphoc_List>().HasNoKey();
            builder.Entity<DM_Donvi_List>().HasNoKey();
            builder.Entity<Auth_RolesList>().HasNoKey();
            builder.Entity<Auth_Users_RolesList>().HasNoKey();
            builder.Entity<Auth_Users_Roles>().HasKey(ur => new { ur.Id_Users, ur.Id_Roles });
            builder.Entity<DM_Cahoc_List>().HasNoKey();
            builder.Entity<DM_Diemtruong_List>().HasNoKey();
            builder.Entity<DM_Loaiphonghoc_List>().HasNoKey();
            builder.Entity<Auth_MenusList>().HasNoKey();
            builder.Entity<DM_Khoikienthuc_List>().HasNoKey();
            builder.Entity<DM_Tochuyenmon_List>().HasNoKey();
            builder.Entity<DM_Khoilop_List>().HasNoKey();
            builder.Entity<DM_Phonghoc_list>().HasNoKey();
            builder.Entity<DM_Monhoc_List>().HasNoKey();
            builder.Entity<Tiet_co_dinh_List>().HasNoKey();
            builder.Entity<DM_Banhoc_List>().HasNoKey();
            builder.Entity<Monhoc_Tohopmon_List>().HasNoKey();
            builder.Entity<DM_Giaovien_List>().HasNoKey();
            builder.Entity<DM_Lophoc_List>().HasNoKey();
            builder.Entity<Lophoc_Monhoc_List>().HasNoKey();
            builder.Entity<MonLop>().HasNoKey();
            builder.Entity<Chitiet_Thoikhoabieu_List>().HasNoKey();
            builder.Entity<Danhsach_ThoikhoabieuList>().HasNoKey();
            builder.Entity<Sotiet_Giaovien>().HasNoKey();
            builder.Entity<Sotiet_Lop>().HasNoKey();
            builder.Entity<Sotiet_Mon>().HasNoKey();
            builder.Entity<Sotiet_Phong>().HasNoKey();
            builder.Entity<Sotiet_LopMon>().HasNoKey();
            builder.Entity<Export>().HasNoKey();
            builder.Entity<PhancongGV>().HasNoKey();
            builder.Entity<DsLop_ByGVandMon>().HasNoKey();
            builder.Entity<DM_Namhoc_List>().HasNoKey();
            builder.Entity<DM_Ngaynghi_List>().HasNoKey();
            builder.Entity<Phanphoi_Chuongtrinh_List>().HasNoKey();
            builder.Entity<Phanphoi_Chuongtrinh_Chitiet_List>().HasNoKey();
            builder.Entity<Lich_Baogiang_List>().HasNoKey();
            builder.Entity<Phieu_Baogiang_List>().HasNoKey();
            builder.Entity<Chitiet_Phieubaogiang_List>().HasNoKey();
            builder.Entity<DM_Lopontap_List>().HasNoKey();
            builder.Entity<Danhsach_Lichontap_List>().HasNoKey();
            builder.Entity<Chitiet_Lichontap_List>().HasNoKey();
            builder.Entity<DM_Hocsinh_List>().HasNoKey();
            builder.Entity<Hocsinh_Lopon_List>().HasNoKey();
            builder.Entity<DM_Baikiemtra_List>().HasNoKey();
            builder.Entity<DM_Loaikiemtra_List>().HasNoKey();
            builder.Entity<DM_Tohopmon_Ontap_List>().HasNoKey();
            builder.Entity<DsLopMon_byGV>().HasNoKey();
            builder.Entity<Ketqua_Hocsinh>().HasNoKey();
            builder.Entity<Ketqua_Hocsinh_ToHopMon>().HasNoKey();
            builder.Entity<DM_Hoidongthi_List>().HasNoKey();
            var environment = _configuration["Environment"];
            var isdemo = environment == "Demo";
            builder.Entity<DM_Donvi>(entity =>
            {
                if (!isdemo)
                {
                    entity.Ignore(e => e.Id_tinh);
                    entity.Ignore(e => e.Nguoi_lien_he);
                }
            });
            base.OnModelCreating(builder);
        }
    }
}
