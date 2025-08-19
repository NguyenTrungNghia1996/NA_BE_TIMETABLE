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
        public List<Sotiet_LopMonDto> GetSotiet_LopMon(int idDonvi, int id_tkb)
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
                var result = _context.Set<Sotiet_LopMon>().FromSqlRaw("EXEC Get_ListLopMon_SoTiet @Id_tkb, @Id_Donvi",
                      paramIdtkb, paramIdDonvi)
                    .ToList();
                if (result == null) result = new List<Sotiet_LopMon>();
                var kq = result.GroupBy(x => x.Id_lop).Select(g => new Sotiet_LopMonDto()
                {
                    Id_lop = g.Key,
                    Ten_lop = g.First().Ten_lop,
                    ds_mon = g.Select(item => new Mon_Sotiet()
                    {
                        Id_mon = item.Id_mon,
                        Ten_mon = item.Ten_mon, 
                        Tong_tiet = item.Tong_tiet,
                        Tiet_chua_xep = item.Tiet_chua_xep,
                        Tiet_da_xep = item.Tiet_da_xep
                    }).ToList()
                }).ToList();
                return kq;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<Monhoc_KhoiLopDto> GetMonhocKhoilop(int? idKhoi, int? idBan, int idDonvi)
        {
            try
            {
                var allMonhoc = _context.Dm_Monhoc.Where(m => m.Id_don_vi == idDonvi).ToList();
                var dsCa = _context.Ca_Donvi.Where(cd => cd.Id_don_vi == idDonvi)
                            .Join(_context.DM_Cahoc,
                                  cd => cd.Id_ca_hoc,
                                  ca => ca.Id,
                                  (cd, ca) => new
                                  {
                                      Id = ca.Id,
                                      Ten = ca.Ten
                                  }).ToList();
                var Monhoc_Khoi = _context.Monhoc_Khoilop
                    .Where(x => x.Id_khoi == idKhoi && x.Id_ban == idBan)
                    .ToList();

                var ds_Mon = allMonhoc.Select(mon =>
                {
                    return new Mon_KhoiDto
                    {
                        Id_mon = mon.Id,
                        Ten_mon = mon.Ten,
                        ds_Ca = dsCa.Select(ca =>
                        {
                            var saved = Monhoc_Khoi
                                .FirstOrDefault(x => x.Id_mon == mon.Id && x.Id_ca == ca.Id);

                            return new Ca_Khoi_MonDto
                            {
                                Id_ca = ca.Id,
                                Ten_ca = ca.Ten,
                                So_tiet = saved?.So_tiet ?? 0,
                                So_nhom = saved?.So_nhom ?? 0
                            };
                        }).ToList(),
                        Trang_thai = Monhoc_Khoi.Any(x => x.Id_mon == mon.Id)
                    };
                }).ToList();

                var result = new Monhoc_KhoiLopDto
                {
                    Id_khoi = idKhoi ?? 0,
                    Id_ban = idBan ?? 0,
                    ds_Mon = ds_Mon
                };

                return new List<Monhoc_KhoiLopDto> { result };
            }
            catch (Exception ex)
            {
                // Log exception nếu cần
                return new List<Monhoc_KhoiLopDto>();
            }
        }
    }
}
