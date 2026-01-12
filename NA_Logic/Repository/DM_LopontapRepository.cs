using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class DM_LopontapRepository : IDM_LopontapRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_LopontapRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Lopontap_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi)
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

                var result = _dbContext.Set<DM_Lopontap_List>().FromSqlRaw("EXEC DM_Lopontap_GetList_Paging @pageIndex, @pageSize, @search,@idDonvi",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi).ToList();
                if (result == null) result = new List<DM_Lopontap_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Lopontap GetDetailById(int Id)
        {
            try
            {
                var Lopontap = _dbContext.DM_Lopontap.FirstOrDefault(c => c.Id == Id);
                return Lopontap;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Lopontap dm_Lopontap)
        {
            try
            {
                _dbContext.DM_Lopontap.Add(dm_Lopontap);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Lopontap dm_Lopontap)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Lopontap.Update(dm_Lopontap);
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
                var Lopontap = _dbContext.DM_Lopontap.FirstOrDefault(c => c.Id == Id);
                _dbContext.DM_Lopontap.Remove(Lopontap);
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
                return _dbContext.DM_Lopontap.Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.DM_Lopontap.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }

    }
}
