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
    public class Mon_KhoiRepository : IMon_KhoiRepository
    {
        private readonly NA_DbContext _context;
        public Mon_KhoiRepository(NA_DbContext context)
        {
            _context = context;
        }

        public Monhoc_Khoilop_BanDto GetListTietBan(int Id_mon, int Id_khoi, int Id_ban, int idDonvi)
        {
            var dsCa = _context.Ca_Donvi.Where(cd => cd.Id_don_vi == idDonvi)
                            .Join(_context.DM_Cahoc,
                                  cd => cd.Id_ca_hoc,
                                  ca => ca.Id,
                                  (cd, ca) => new
                                  {
                                      Id = ca.Id,
                                      Ten = ca.Ten, 
                                      So_tiet  = cd.So_tiet
                                  }).ToList();
            var tietBan = _context.Monhoc_Khoilop_Tiettranhxep
                        .Where(tb => tb.Id_mon == Id_mon && tb.Id_khoi==Id_khoi && tb.Id_ban == Id_ban)
                        .Select(tb => new { tb.Id_ca, tb.Ngay, tb.Tiet })
                        .ToList();

            var donvi = _context.DM_Donvi.Find(idDonvi);
            // Lấy danh sách ngày từ enum
            var dsNgay = Enum.GetValues<Ngay>().Take(donvi.So_ngay).ToList();
            // Lấy danh sách tiết từ enum
            var dsTiet = Enum.GetValues<Tiet>().ToList();

            var result = new Monhoc_Khoilop_BanDto
            {
                Id_mon = Id_mon,
                Id_khoi = Id_khoi,
                Id_ban = Id_ban,
                Ds_Ca = dsCa.Select(ca => new Ca_banDto
                {
                    Id = ca.Id,
                    Ds_Ngay = dsNgay.Select(ngay => new Ngay_banDto
                    {
                        Id = ngay,
                        Ten = ngay.GetDisplayName(),
                        Ds_Tiet = dsTiet.Take(ca.So_tiet).Select(tiet => new TietbanDto
                        {
                            Id = tiet,
                            Ten = tiet.GetDisplayName(),
                            Trang_thai = tietBan.Any(td => td.Id_ca == ca.Id &&
                                                           td.Ngay == (int)ngay &&
                                                           td.Tiet == (int)tiet)
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            return result;
        }
        public bool AddTietBan(List<Monhoc_Khoilop_Tiettranhxep> dsTietTranhXep, int idMon, int idKhoi, int idBan)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var existingTiet = _context.Monhoc_Khoilop_Tiettranhxep.Where(tb => tb.Id_mon == idMon && tb.Id_khoi==idKhoi && tb.Id_ban==idBan).ToList();

                //xóa
                if (existingTiet.Any())
                {
                    _context.BulkDelete(existingTiet);
                }
                //thêm
                if (dsTietTranhXep != null && dsTietTranhXep.Any())
                {
                    _context.BulkInsert(dsTietTranhXep);
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return false;
            }
        }
        
        public bool DeleteTietTranhXep(int Id_mon, int Id_khoi)
        {
            try
            {
                var tietban = _context.Monhoc_Khoilop_Tiettranhxep.Where(c => c.Id_mon == Id_mon && c.Id_khoi==Id_khoi).ToList();
                if (tietban.Count > 0)
                {
                    _context.Monhoc_Khoilop_Tiettranhxep.RemoveRange(tietban);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool AddMonKhoi(List<Monhoc_Khoilop> dsMonKhoi, int idKhoi, int idBan)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var existingTiet = _context.Monhoc_Khoilop.Where(tb => tb.Id_khoi == idKhoi && tb.Id_ban == idBan).ToList();

                //xóa
                if (existingTiet.Any())
                {
                    _context.BulkDelete(existingTiet);
                }
                //thêm
                if (dsMonKhoi != null && dsMonKhoi.Any())
                {
                    _context.BulkInsertOrUpdate(dsMonKhoi);
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return false;
            }
        }

        public List<Monhoc_KhoiLopDto> GetMonhocKhoilop(int? idKhoi, int? idBan,  int idDonvi)
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
                                     Ten = ca.Ten, So_tiet  = cd.So_tiet
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
        public bool DongBoLopMon(int idDonvi)
        {
            try
            {
                int check = _context.Database.ExecuteSqlRaw($@"
                    update lm 
                    set lm.So_tiet_ca_sang_truyen_thong = case when mk.Id_ca = 1 and mk.So_tiet > 0 then mk.So_tiet else 0 end,
	                    lm.So_tiet_ca_chieu_truyen_thong = case when mk.Id_ca = 2 and mk.So_tiet > 0 then mk.So_tiet else 0 end
                    from Lophoc_Monhoc lm 
                    join DM_Lophoc l on l.Id = lm.Id_lop
                    join Monhoc_Khoilop mk on lm.Id_mon = mk.Id_mon and l.Id_ban = mk.Id_ban and l.Id_khoi = mk.Id_khoi
                    where l.Id_don_vi = {{0}}", idDonvi);
                if (check > 0)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}