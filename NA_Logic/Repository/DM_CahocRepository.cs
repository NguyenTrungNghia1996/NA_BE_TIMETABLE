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

        public List<DM_Cahoc_List> GetList_Paging(int PageIndex, int PageSize, string search, int Id_Donvi, ref int totalrecord)
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
                var paramIdDonvi = new SqlParameter("Id_Donvi", SqlDbType.Int)
                {
                    Value = Id_Donvi
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _dbContext.Set<DM_Cahoc_List>().FromSqlRaw("EXEC DM_Cahoc_GetList_Paging @pageIndex, @pageSize, @search, @Id_Donvi, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi, paramTotal)
                    .ToList();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Cahoc GetDetailById(int Id, int Id_Donvi)
        {
            try
            {
                var cahoc = _dbContext.DM_Cahoc.FirstOrDefault(c => c.Id == Id && c.Id_Donvi == Id_Donvi);
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
                cahoc = _dbContext.DM_Cahoc.Find(Id);
                _dbContext.DM_Cahoc.Remove(cahoc);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
