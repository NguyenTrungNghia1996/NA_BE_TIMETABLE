using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;

namespace NA_Entities.DBContext
{
    public class NA_DbContext : DbContext
    {
        public NA_DbContext(DbContextOptions<NA_DbContext> options) : base(options) { }
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
        public DbSet<DM_Tiethoc> DM_Tiethoc { get; set; }
        public DbSet<DM_Tiethoc_List> DM_Tiethoc_List { get; set; }
        public DbSet<DM_Ngayhoc> DM_Ngayhoc { get; set; }
        public DbSet<DM_Ngayhoc_List> DM_Ngayhoc_List { get; set; }
        public DbSet<Ca_Tiethoc> Ca_Tiethoc { get; set; }
        public DbSet<Tiet_ban> Tiet_ban { get; set; }
        public DbSet<DM_Monhoc> Dm_Monhoc { get; set; }
        public DbSet<DM_Monhoc_List> DM_Monhoc_List { get; set; }
        public DbSet<Mon_Khoikienthuc> Mon_Khoikienthuc { get; set; }
        public DbSet<Tiet_tranh_xep> Tiet_Tranh_Xep { get; set; }
        public DbSet<Ngay_Donvi> Ngay_Donvi    { get; set; }
        public DbSet<Tiet_co_dinh> Tiet_co_dinh { get; set; }
        public DbSet<Tiet_co_dinh_List> Tiet_co_dinh_list { get; set; }
        public DbSet<Monhoc_Phonghoc> Monhoc_Phonghoc { get; set; }
        public DbSet<DM_Banhoc> DM_Banhoc { get; set; }
        public DbSet<DM_Banhoc_List> DM_Banhoc_List { get; set; }
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
            builder.Entity<DM_Tiethoc_List>().HasNoKey();
            builder.Entity<DM_Ngayhoc_List>().HasNoKey();
            builder.Entity<DM_Monhoc_List>().HasNoKey();
            builder.Entity<Tiet_co_dinh_List>().HasNoKey();
            builder.Entity<DM_Banhoc_List>().HasNoKey();
            base.OnModelCreating(builder);
        }
    }
}
