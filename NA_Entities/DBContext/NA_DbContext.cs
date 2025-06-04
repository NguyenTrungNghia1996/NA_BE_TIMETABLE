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
        // Auth_Roles
        public DbSet<Auth_Roles> Auth_Roles { get; set; }
        public DbSet<Auth_RolesList> Auth_RolesList { get; set; }
        // Auth_Users_Roles
        public DbSet<Auth_Users_Roles> Auth_Users_Roles { get; set; }
        public DbSet<Auth_Users_RolesList> Auth_Users_RolesList { get; set; }
        public DbSet<DM_Cahoc> DM_Cahoc { get; set; }
        public DbSet<DM_Cahoc_List> DM_Cahoc_List { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Auth_Users_List>().HasNoKey();
            builder.Entity<DM_Caphoc_List>().HasNoKey();
            builder.Entity<DM_Donvi_List>().HasNoKey();
            builder.Entity<Auth_RolesList>().HasNoKey();
            builder.Entity<Auth_Users_RolesList>().HasNoKey();
            builder.Entity<Auth_Users_Roles>().HasKey(ur => new { ur.Id_Users, ur.Id_Roles });
            builder.Entity<DM_Cahoc_List>().HasNoKey();
            base.OnModelCreating(builder);
        }
    }
}
