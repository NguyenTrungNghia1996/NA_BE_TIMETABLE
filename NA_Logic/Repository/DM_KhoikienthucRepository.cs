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
    public class DM_KhoikienthucRepository : IDM_KhoikienthucRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_KhoikienthucRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Khoikienthuc_List> GetList_Paging(int PageIndex, int PageSize, string search, int IdDonvi, ref int totalrecord)
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
                var paramIddonvi = new SqlParameter("Id_Donvi", SqlDbType.Int)
                {
                    Value = IdDonvi
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _dbContext.Set<DM_Khoikienthuc_List>().FromSqlRaw("EXEC DM_Khoikienthuc_GetList_Paging @pageIndex, @pageSize, @search,@Id_Donvi, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch,paramIddonvi, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Khoikienthuc_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Khoikienthuc GetDetailById(int Id)
        {
            try
            {
                var item = _dbContext.DM_Khoikienthuc.FirstOrDefault(c => c.Id == Id);
                return item;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Khoikienthuc dM_Khoikienthuc)
        {
            try
            {
                _dbContext.DM_Khoikienthuc.Add(dM_Khoikienthuc);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Khoikienthuc dM_Khoikienthuc)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Khoikienthuc.Update(dM_Khoikienthuc);
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
                DM_Khoikienthuc item = new DM_Khoikienthuc();
                item = _dbContext.DM_Khoikienthuc.Find(Id);
                if (item != null)
                {
                    _dbContext.DM_Khoikienthuc.Remove(item);
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
                return _dbContext.DM_Khoikienthuc.Any(c => c.Id == Id && c.Id_Donvi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids, int idDonvi)
        {
            var existingIds = _dbContext.DM_Khoikienthuc.Where(c => ids.Contains(c.Id) && c.Id_Donvi == idDonvi).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
    }
}
