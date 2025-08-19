using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class SoTietDanhMucRepository: ISoTietDanhMucRepository
    {
        private readonly NA_DbContext _context;
        public SoTietDanhMucRepository(NA_DbContext context)
        {
            _context = context;
        }
        public List<Sotiet_Mon> GetSotiet_Mon(int idDonvi, int id_tkb)
        {
            try
            {
                var paramIdDonvi = new SqlParameter("Id_Donvi", SqlDbType.Int)
                {
                    Value = idDonvi
                };

                var paramIdtkb = new SqlParameter("Id_tkb", SqlDbType.Int)
                {
                    Value = id_tkb
                };
                var result = _context.Set<Sotiet_Mon>().FromSqlRaw("EXEC Get_ListMon_SoTiet @Id_tkb, @Id_Donvi",
                      paramIdtkb, paramIdDonvi)
                    .ToList();
                if (result == null) result = new List<Sotiet_Mon>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<Sotiet_Lop> GetSotiet_Lop(int idDonvi, int id_tkb)
        {
            try
            {
                var paramIdDonvi = new SqlParameter("Id_Donvi", SqlDbType.Int)
                {
                    Value = idDonvi
                };

                var paramIdtkb = new SqlParameter("Id_tkb", SqlDbType.Int)
                {
                    Value = id_tkb
                };
                var result = _context.Set<Sotiet_Lop>().FromSqlRaw("EXEC Get_ListLop_SoTiet @Id_tkb, @Id_Donvi",
                      paramIdtkb, paramIdDonvi)
                    .ToList();
                if (result == null) result = new List<Sotiet_Lop>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<Sotiet_Phong> GetSotiet_Phong(int idDonvi, int id_tkb)
        {
            try
            {
                var paramIdDonvi = new SqlParameter("Id_Donvi", SqlDbType.Int)
                {
                    Value = idDonvi
                };

                var paramIdtkb = new SqlParameter("Id_tkb", SqlDbType.Int)
                {
                    Value = id_tkb
                };
                var result = _context.Set<Sotiet_Phong>().FromSqlRaw("EXEC Get_ListPhong_SoTiet @Id_tkb, @Id_Donvi",
                      paramIdtkb, paramIdDonvi)
                    .ToList();
                if (result == null) result = new List<Sotiet_Phong>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<Sotiet_Giaovien> GetSotiet_Giaovien(int idDonvi, int id_tkb)
        {
            try
            {
                var paramIdDonvi = new SqlParameter("Id_Donvi", SqlDbType.Int)
                {
                    Value = idDonvi
                };

                var paramIdtkb = new SqlParameter("Id_tkb", SqlDbType.Int)
                {
                    Value = id_tkb
                };
                var result = _context.Set<Sotiet_Giaovien>().FromSqlRaw("EXEC Get_ListGiaovien_SoTiet @Id_tkb, @Id_Donvi",
                      paramIdtkb, paramIdDonvi)
                    .ToList();
                if (result == null) result = new List<Sotiet_Giaovien>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
