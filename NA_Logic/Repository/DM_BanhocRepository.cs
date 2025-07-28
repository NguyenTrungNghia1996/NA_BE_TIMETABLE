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
    public class DM_BanhocRepository : IDM_BanhocRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_BanhocRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Banhoc_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord)
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
                var result = _dbContext.Set<DM_Banhoc_List>().FromSqlRaw("EXEC DM_Banhoc_GetList_Paging @pageIndex, @pageSize, @search, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Banhoc_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Banhoc GetDetailById(int Id)
        {
            try
            {
                var Banhoc = _dbContext.DM_Banhoc.FirstOrDefault(c => c.Id == Id);
                return Banhoc;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Banhoc dm_Banhoc)
        {
            try
            {
                _dbContext.DM_Banhoc.Add(dm_Banhoc);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Banhoc dm_Banhoc)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Banhoc.Update(dm_Banhoc);
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
                DM_Banhoc Banhoc = new DM_Banhoc();
                Banhoc = _dbContext.DM_Banhoc.FirstOrDefault(c => c.Id == Id);
                if (Banhoc != null)
                {
                    Banhoc.Trang_thai_xoa = true;
                    _dbContext.DM_Banhoc.Update(Banhoc);
                    _dbContext.SaveChanges();
                }
                return true;
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
                return _dbContext.DM_Banhoc.Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.DM_Banhoc.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }

    }
}
