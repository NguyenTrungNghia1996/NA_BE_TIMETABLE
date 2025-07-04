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
    public class DM_CahocRepository : IDM_CahocRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_CahocRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Cahoc_List> GetList_Paging(int PageIndex, int PageSize, string search,  ref int totalrecord)
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

                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _dbContext.Set<DM_Cahoc_List>().FromSqlRaw("EXEC DM_Cahoc_GetList_Paging @pageIndex, @pageSize, @search, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Cahoc_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Cahoc GetDetailById(int Id)
        {
            try
            {
                var cahoc = _dbContext.DM_Cahoc.FirstOrDefault(c => c.Id == Id && c.Trang_thai_xoa == false );
                return cahoc;
            }
            catch(Exception) 
            {
                return null;
            }
        }
        public bool Add(DM_Cahoc dm_cahoc)
        {
            try
            {
                _dbContext.DM_Cahoc.Add(dm_cahoc);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Cahoc dm_cahoc)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Cahoc.Update(dm_cahoc);
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
                DM_Cahoc cahoc = new DM_Cahoc();
                cahoc = _dbContext.DM_Cahoc.FirstOrDefault(c => c.Id == Id && c.Trang_thai_xoa == false);
                if (cahoc != null)
                {
                    cahoc.Trang_thai_xoa = true;
                    _dbContext.DM_Cahoc.Update(cahoc);
                    _dbContext.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckId(int Id)
        {
            if (Id <= 0) return false;
            try
            {
                return _dbContext.DM_Cahoc.Any(c => c.Id == Id);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.DM_Cahoc.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }

    }
}
