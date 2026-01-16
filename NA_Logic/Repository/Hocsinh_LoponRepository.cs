using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Hocsinh_LoponRepository : IHocsinh_LoponRepository
    {
        private readonly NA_DbContext _dbContext;
        public Hocsinh_LoponRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Hocsinh_Lopon_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi)
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

                var result = _dbContext.Set<Hocsinh_Lopon_List>().FromSqlRaw("EXEC [Hocsinh_Lopon_GetList_Paging] @pageIndex, @pageSize, @search,@idDonvi",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi).ToList();
                if (result == null) result = new List<Hocsinh_Lopon_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Hocsinh_Lopon GetDetailById(int Id, int idDonvi)
        {
            try
            {
                Hocsinh_Lopon Lopontap = (from l in _dbContext.DM_Lopontap
                               join hl in _dbContext.Hocsinh_Lopon on l.Id equals hl.Id_lop_on
                                          where hl.Id == Id && l.Id_don_vi == idDonvi
                               select hl).FirstOrDefault();
                return Lopontap;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Hocsinh_Lopon_Multi GetHocSinhByIdLop(int IdLop, int idDonvi)
        {
            try
            {
                var lopOn = _dbContext.DM_Lopontap
                    .Where(l => l.Id == IdLop && l.Id_don_vi == idDonvi)
                    .Select(l => new Hocsinh_Lopon_Multi
                    {
                        Id_lop = l.Id,
                        Hoc_sinh = _dbContext.Hocsinh_Lopon .Where(hl => hl.Id_lop_on == IdLop).Select(hl => hl.Id_hoc_sinh).ToList()
                    })
                    .FirstOrDefault();

                return lopOn;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //public Lopon_Hocsinh GetHocSinhByIdLop(int IdLop, int idDonvi)
        //{
        //    try
        //    {
        //        var lopOn = _dbContext.DM_Lopontap.Where(c=>c.Id == IdLop && c.Id_don_vi == idDonvi)
        //            .Select( c=> new Lopon_Hocsinh
        //            {
        //                Id = c.Id,
        //                Ten = c.Ten,
        //                Hoc_sinh = (from hs in _dbContext.DM_Hocsinh
        //                           join hl in _dbContext.Hocsinh_Lopon on hs.Id equals hl.Id_hoc_sinh
        //                           where hl.Id_lop_on == IdLop
        //                           select new Hocsinh_List
        //                           {
        //                               Id_hoc_sinh = hs.Id,
        //                               Ten = hs.Ten
        //                           }).ToList()
        //            }).FirstOrDefault();
        //        return lopOn;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        public bool CheckTrung(Hocsinh_Lopon hs, int idDonvi)
        {
            try
            {
                bool check = (from hl in _dbContext.Hocsinh_Lopon
                              join l in _dbContext.DM_Lopontap on hl.Id_lop_on equals l.Id
                              where hl.Id_hoc_sinh == hs.Id_hoc_sinh
                                  && hl.Id_lop_on == hs.Id_lop_on
                                  && l.Id_don_vi == idDonvi
                                  && (hs.Id <= 0 || hl.Id != hs.Id)
                              select hl).Any();

                return check;
            }
            catch (Exception ex) {
                return false;
            }
        }
        public bool Add(Hocsinh_Lopon_Multi data)
        {
            using var tranc = _dbContext.Database.BeginTransaction();
            try
            {
                var listOld = _dbContext.Hocsinh_Lopon.Where(c=>c.Id_lop_on==data.Id_lop).ToList();

                if(listOld.Any()) 
                    _dbContext.BulkDelete(listOld);

                var list = data.Hoc_sinh.Select(c=> new Hocsinh_Lopon
                {
                    Id_lop_on = data.Id_lop,
                    Id_hoc_sinh = c
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
        public bool Update(Hocsinh_Lopon hs)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.Hocsinh_Lopon.Update(hs);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(int Id)
        {
            try
            {
                var hs = _dbContext.Hocsinh_Lopon.FirstOrDefault(c => c.Id == Id);
                _dbContext.Hocsinh_Lopon.Remove(hs);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteByLop(int Id)
        {
            try
            {
                var hs = _dbContext.Hocsinh_Lopon.Where(c=>c.Id_lop_on == Id).ToList();
                _dbContext.Hocsinh_Lopon.RemoveRange(hs);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteByHocSinh(int Id)
        {
            try
            {
                var hs = _dbContext.Hocsinh_Lopon.Where(c=>c.Id_lop_on == Id).ToList();
                _dbContext.Hocsinh_Lopon.RemoveRange(hs);
                _dbContext.SaveChanges();
                return true;
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
                return _dbContext.Hocsinh_Lopon.Any(c => c.Id == Id);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.Hocsinh_Lopon.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }

    }
}
