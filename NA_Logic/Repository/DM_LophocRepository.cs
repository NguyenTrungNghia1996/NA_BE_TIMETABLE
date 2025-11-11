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
    public class DM_LophocRepository : IDM_LophocRepository
    {
        private readonly NA_DbContext _context;
        public DM_LophocRepository(NA_DbContext context)
        {
            _context = context;
        }

        public List<DM_Lophoc_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, int idKhoilop, ref int totalrecord)
        {
            try
            {
                var paramPageIndex = new SqlParameter("pageIndex", SqlDbType.Int)
                {
                    Value = PageIndex
                };
                var paramPageSize = new SqlParameter("pageSize", SqlDbType.Int)
                {
                    Value = PageSize
                };
                var paramSearch = new SqlParameter("search", SqlDbType.NVarChar)
                {
                    Value = search ?? string.Empty
                };
                var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int)
                {
                    Value = idDonvi
                };
                var paramIdKhoi = new SqlParameter("idKhoilop", SqlDbType.Int)
                {
                    Value = idKhoilop
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _context.Set<DM_Lophoc_List>().FromSqlRaw("EXEC DM_Lophoc_GetList_Paging @pageIndex, @pageSize, @search ,@idDonvi, @idKhoilop, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch,  paramIdDonvi, paramIdKhoi, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Lophoc_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Lophoc getDetailById(int id)
        {
            try
            {
                var data = _context.DM_Lophoc.FirstOrDefault(c => c.Id == id);
                return data;
            }
            catch
            {
                return null;
            }
        }
        public bool Add(DM_Lophoc dM_Lophoc)
        {
            try
            {
                _context.DM_Lophoc.Add(dM_Lophoc);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Update(DM_Lophoc dM_Lophoc)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.DM_Lophoc.Update(dM_Lophoc);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Check_limit(int idDonvi)
        {
            try
            {
                var soluong = _context.DM_Lophoc.Count(c => c.Id_don_vi == idDonvi);
                if (soluong < 8)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Delete(int Id)
        {
            try
            {
                DM_Lophoc ph = new DM_Lophoc();
                ph = _context.DM_Lophoc.Find(Id);
                if (ph != null)
                {
                    _context.DM_Lophoc.Remove(ph);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckContraint(int id, int idDonvi)
        {
            try
            {
                bool check = (from lopmon in _context.Lophoc_Monhoc.AsNoTracking()
                              join lop in _context.DM_Lophoc.AsNoTracking() on lopmon.Id_lop equals lop.Id
                              where lop.Id_don_vi == idDonvi && lopmon.Id_lop == id
                              select 1).Any();
                return check;
            }
            catch
            {
                return false;    
            }
        }

        public bool CheckId(int Id, int idDonvi)
        {
            if (Id <= 0) return false;
            try
            {
                var check = _context.DM_Lophoc.Any(dt => dt.Id_don_vi == idDonvi && dt.Id == Id);
                if(check)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids, int idDonvi)
        {
            var existingIds = _context.DM_Lophoc.Where(c => ids.Contains(c.Id) && c.Id_don_vi==idDonvi).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
        public Lophoc_banDto GetListTietBan(int Id, int idDonvi)
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
            var tietBan = _context.Lophoc_Tietnghi
                        .Where(tb => tb.Id_lop == Id)
                        .Select(tb => new { tb.Id_ca, tb.Ngay, tb.Tiet })
                        .ToList();

            var donvi = _context.DM_Donvi.Find(idDonvi);
            // Lấy danh sách ngày từ enum
            var dsNgay = Enum.GetValues<Ngay>().Take(donvi.So_ngay).ToList();
            // Lấy danh sách tiết từ enum
            var dsTiet = Enum.GetValues<Tiet>().ToList();

            var result = new Lophoc_banDto
            {
                Id_lop = Id,
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
        public bool AddTietBan(List<Lophoc_Tietnghi> dsTietBan, int idLop)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var existingTietBan = _context.Lophoc_Tietnghi.Where(tb => tb.Id_lop == idLop).ToList();

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
        public bool DeleteTietBan(int Id)
        {
            try
            {
                var tietban = _context.Lophoc_Tietnghi.Where(c => c.Id_lop == Id).ToList();
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
        public LopMon_banDto GetListTietBan_MonLop(int Id_lop, int Id_mon, int idDonvi)
        {
            var dsCa = _context.Ca_Donvi.Where(cd => cd.Id_don_vi == idDonvi)
                            .Join(_context.DM_Cahoc,
                                  cd => cd.Id_ca_hoc,
                                  ca => ca.Id,
                                  (cd, ca) => new
                                  {
                                      Id = ca.Id,
                                     Ten = ca.Ten, So_tiet  = cd.So_tiet
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
        public bool DeleteTietBan_MonLop(int Id_lop, int  Id_mon)
        {
            try
            {
                var tietban = _context.Lophoc_Monhoc_Tiettranhxep.Where(c => c.Id_lop == Id_lop && c.Id_mon==Id_mon).ToList();
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