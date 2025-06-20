using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class DM_NgayhocRepository : IDM_NgayhocRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_NgayhocRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Ngayhoc_List> GetList_Paging(int PageIndex, int PageSize, string search, int IdDonvi, ref int totalrecord)
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
                    Value = IdDonvi
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _dbContext.Set<DM_Ngayhoc_List>().FromSqlRaw("EXEC DM_Ngayhoc_GetList_Paging @pageIndex, @pageSize, @search,@Id_Donvi, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Ngayhoc_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Ngayhoc GetDetailById(int Id)
        {
            try
            {
                var Ngayhoc = _dbContext.DM_Ngayhoc.FirstOrDefault(c => c.Id == Id);
                return Ngayhoc;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Ngayhoc dm_Ngayhoc)
        {
            try
            {
                _dbContext.DM_Ngayhoc.Add(dm_Ngayhoc);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Ngayhoc dm_Ngayhoc)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Ngayhoc.Update(dm_Ngayhoc);
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
                DM_Ngayhoc Ngayhoc = new DM_Ngayhoc();
                Ngayhoc = _dbContext.DM_Ngayhoc.Find(Id);
                if (Ngayhoc != null)
                {
                    _dbContext.DM_Ngayhoc.Remove(Ngayhoc);
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
                return _dbContext.DM_Ngayhoc.Any(c => c.Id == Id && c.Id_Donvi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        //public bool CheckIds(IEnumerable<int> ids)
        //{
        //    var existingIds = _dbContext.DM_Cahoc.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
        //    return ids.All(id => existingIds.Contains(id));
        //}

    }
}
