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
                                      Ten = ca.Ten
                                  }).ToList();
            var tietBan = _context.Monhoc_Khoilop_Tiettranhxep
                        .Where(tb => tb.Id_mon == Id_mon && tb.Id_khoi==Id_khoi && tb.Id_ban == Id_ban)
                        .Select(tb => new { tb.Id_ca, tb.Ngay, tb.Tiet })
                        .ToList();

            // Lấy danh sách ngày từ enum
            var dsNgay = Enum.GetValues<Ngay>().ToList();
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
                        Ds_Tiet = dsTiet.Select(tiet => new TietbanDto
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
                                      Ten = ca.Ten
                                  }).ToList();
                var Monhoc_Khoi = _context.Monhoc_Khoilop
                    .Where(x => x.Id_khoi == idKhoi && x.Id_ban == idBan)
                    .ToList();

                var ds_Mon = allMonhoc.Select(mon =>
                {
                    var saved = Monhoc_Khoi.FirstOrDefault(x => x.Id_mon == mon.Id);
                    return new Mon_KhoiDto
                    {
                        Id_mon = mon.Id,
                        Ten_mon = mon.Ten,
                        ds_Ca = dsCa.Select(ca => new Ca_Khoi_MonDto
                        {
                            Id_ca = ca.Id,
                            Ten_ca = ca.Ten,
                            So_tiet = saved?.So_tiet ?? 0,
                            So_nhom = saved?.So_nhom ?? 0
                        }).ToList(),
                        Trang_thai = saved != null
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