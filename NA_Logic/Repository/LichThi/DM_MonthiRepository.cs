using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository;
using NA_Logic.IRepository.LichThi;
using NA_Logic.IRepository.XepGiamThi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class DM_MonthiRepository : IDM_MonthiRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_MonthiRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Monthi_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi)
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

                var result = _dbContext.Set<DM_Monthi_List>().FromSqlRaw("EXEC [DM_Monthi_GetList_Paging] @pageIndex, @pageSize, @search, @idDonvi",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi).ToList();
                if (result == null) result = new List<DM_Monthi_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Monthi_Detail GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var result = (from mon in _dbContext.DM_Monthi
                              join hoidong in _dbContext.DM_Hoidongthi
                                  on mon.Id_hoi_dong equals hoidong.Id
                              where mon.Id == Id && hoidong.Id_don_vi == idDonvi
                              select new DM_Monthi_Detail
                              {
                                  Id = mon.Id,
                                  Ma = mon.Ma,
                                  Ten = mon.Ten,
                                  Id_hoi_dong = mon.Id_hoi_dong,
                                  Id_mon = mon.Id_mon,
                                  Id_nam = hoidong.Id_nam
                              }).FirstOrDefault();

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Monthi_Multi GetMonByIdHoiDong(int IdHoiDong, int idDonvi)
        {
            try
            {
                var mon = _dbContext.DM_Hoidongthi
                    .Where(l => l.Id == IdHoiDong && l.Id_don_vi == idDonvi)
                    .Select(l => new DM_Monthi_Multi
                    {
                        Id_hoi_dong = l.Id,
                        Id_mon = _dbContext.DM_Monthi.Where(hl => hl.Id_hoi_dong == IdHoiDong).Select(hl => hl.Id_mon).ToList()
                    })
                    .FirstOrDefault();

                return mon;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool Add(DM_Monthi mon)
        {
            try
            {
                _dbContext.DM_Monthi.Add(mon);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddList(DM_Monthi_Multi data)
        {
            using var tranc = _dbContext.Database.BeginTransaction();
            try
            {
                var listOld = _dbContext.DM_Monthi.Where(c => c.Id_hoi_dong == data.Id_hoi_dong && c.Id_mon.HasValue).ToList();

                if (listOld.Any())
                    _dbContext.BulkDelete(listOld);

                var monHocList = _dbContext.Dm_Monhoc.Where(m => data.Id_mon.Contains(m.Id)).ToList();

                var list = data.Id_mon.Select(c => {
                    var monHoc = monHocList.FirstOrDefault(m => m.Id == c);
                    return new DM_Monthi
                    {
                        Id_hoi_dong = data.Id_hoi_dong,
                        Id_mon = c,
                        Ma = monHoc?.Ma,
                        Ten = monHoc?.Ten
                    };
                }).ToList();

                _dbContext.BulkInsert(list);
                tranc.Commit();
                return true;
            }
            catch (Exception)
            {
                tranc.Rollback();
                return false;
            }
        }
        public bool Update(DM_Monthi mon)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Monthi.Update(mon);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(int id, int idDonvi)
        {
            try
            {
                var hoidong = _dbContext.DM_Hoidongthi.Where(h => h.Id_don_vi == idDonvi).Select(h => h.Id).ToList();
                var mon = _dbContext.DM_Monthi.FirstOrDefault(c => c.Id == id && hoidong.Contains(c.Id_hoi_dong));
                if (mon == null)
                {
                    return false;
                }
                _dbContext.DM_Monthi.Remove(mon);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckMa(string Ma, int idHoidong, int? Id)
        {
            try
            {
                var query = _dbContext.DM_Monthi.Where(c => c.Ma == Ma && c.Id_hoi_dong == idHoidong);

                if (Id.HasValue)
                {
                    query = query.Where(c => c.Id != Id.Value);
                }

                var check = query.Any();
                return check;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckTrungTen(int idHoidong, string Ten, int? Id)
        {
            var TenFomat = Ten?.Trim().ToLower().Replace(" ", "") ?? "";
            var TenTonTai = _dbContext.DM_Monthi.Where(c => c.Id_hoi_dong == idHoidong).Select(c => new { c.Id, c.Ten });

            if (Id.HasValue)
                TenTonTai = TenTonTai.Where(c => c.Id != Id.Value);

            var TenTonTaiFomat = TenTonTai.AsEnumerable().Select(c => c.Ten?.Trim().ToLower().Replace(" ", "") ?? "");

            return TenTonTaiFomat.Any(c => c == TenFomat);
        }

        public bool Check_constraint(int Id)
        {
            try
            {

                return _dbContext.Database.SqlQuery<int>($@"
                          select 1 as Value from Phanphoi_Chuongtrinh where Id_nam_hoc = {Id}
                          union select 1 from Lich_Baogiang where Id_nam_hoc = {Id}").Any();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckId(int Id, int idDonvi)
        {
            if (Id <= 0) return false;
            try
            {
                var hoidong = _dbContext.DM_Hoidongthi.Where(h => h.Id_don_vi == idDonvi).Select(h => h.Id).ToList();
                return _dbContext.DM_Monthi.Any(c => c.Id == Id && hoidong.Contains(c.Id_hoi_dong));
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.DM_Monthi.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }

    }
}
