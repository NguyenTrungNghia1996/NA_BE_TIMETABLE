using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository.LichOnTap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class DM_BaikiemtraRepository : IDM_BaikiemtraRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_BaikiemtraRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Baikiemtra_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi)
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

                var result = _dbContext.Set<DM_Baikiemtra_List>().FromSqlRaw("EXEC [DM_Baikiemtra_GetList_Paging] @pageIndex, @pageSize, @search,@idDonvi",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi).ToList();
                if (result == null) result = new List<DM_Baikiemtra_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Baikiemtra GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var baikiemtra = (from kt in _dbContext.DM_Baikiemtra
                                join l in _dbContext.DM_Lopontap on kt.Id_lop_on equals l.Id
                                where l.Id_don_vi == idDonvi && kt.Id == Id
                                select kt).FirstOrDefault();
                return baikiemtra;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Baikiemtra kt)
        {
            try
            {
                _dbContext.DM_Baikiemtra.Add(kt);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Baikiemtra kt)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Baikiemtra.Update(kt);
                _dbContext.SaveChanges();
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
                bool check = (from kq in _dbContext.KetQua_Baikiemtra.AsNoTracking()
                              join bai in _dbContext.DM_Baikiemtra.AsNoTracking() on kq.Id_bai_kiem_tra equals bai.Id
                              join lop in _dbContext.DM_Lopontap.AsNoTracking() on bai.Id_lop_on equals lop.Id
                              where lop.Id_don_vi == idDonvi && bai.Id == id
                              select 1).Any();
                return check;
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
                var kt = _dbContext.DM_Baikiemtra.FirstOrDefault(c => c.Id == Id);
                _dbContext.DM_Baikiemtra.Remove(kt);
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
                return (from kt in _dbContext.DM_Baikiemtra
                        join l in _dbContext.DM_Lopontap on kt.Id_lop_on equals l.Id
                        where l.Id_don_vi == idDonvi && kt.Id == Id
                        select kt).Any();
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids, int idDonvi)
        {
            var existingIds = (from kt in _dbContext.DM_Baikiemtra
                               join l in _dbContext.DM_Lopontap on kt.Id_lop_on equals l.Id
                               where l.Id_don_vi == idDonvi && ids.Contains(kt.Id)
                               select kt.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
    }
}
