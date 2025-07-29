using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Lop_MonRepository : ILop_MonRepository
    {
        private readonly NA_DbContext _context;
        public Lop_MonRepository(NA_DbContext context)
        {
            _context = context;
        }
        public LopMon_banDto GetListTietBan_MonLop(int Id_lop, int Id_mon, int idDonvi)
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
            var tietBan = _context.Lophoc_Monhoc_Tiettranhxep
                        .Where(tb => tb.Id_lop == Id_lop && tb.Id_mon == Id_mon)
                        .Select(tb => new { tb.Id_ca, tb.Ngay, tb.Tiet })
                        .ToList();

            // Lấy danh sách ngày từ enum
            var dsNgay = Enum.GetValues<Ngay>().ToList();
            // Lấy danh sách tiết từ enum
            var dsTiet = Enum.GetValues<Tiet>().ToList();

            var result = new LopMon_banDto
            {
                Id_lop = Id_lop,
                Id_mon = Id_mon,
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
        public bool AddTietBan_MonLop(List<Lophoc_Monhoc_Tiettranhxep> dsTietBan, int idLop, int idMon)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var existingTietBan = _context.Lophoc_Monhoc_Tiettranhxep.Where(tb => tb.Id_lop == idLop && tb.Id_mon == idMon).ToList();

                //xóa
                if (existingTietBan.Any())
                {
                    _context.BulkDelete(existingTietBan);
                }
                //thêm
                if (dsTietBan != null && dsTietBan.Any())
                {
                    _context.BulkInsert(dsTietBan);
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
        public bool DeleteTietBan_MonLop(int Id_lop, int Id_mon)
        {
            try
            {
                var tietban = _context.Lophoc_Monhoc_Tiettranhxep.Where(c => c.Id_lop == Id_lop && c.Id_mon == Id_mon).ToList();
                if (tietban.Count > 0)
                {
                    _context.BulkDelete(tietban);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}