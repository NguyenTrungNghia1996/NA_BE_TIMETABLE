using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository;
using NA_Logic.IRepository.XepGiamThi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class DM_HoidongthiRepository : IDM_HoidongthiRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_HoidongthiRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Hoidongthi_List> GetList_Paging(int PageIndex, int PageSize, string search)
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

                var result = _dbContext.Set<DM_Hoidongthi_List>().FromSqlRaw("EXEC [DM_Hoidongthi_GetList_Paging] @pageIndex, @pageSize, @search",
                    paramPageIndex, paramPageSize, paramSearch)
                    .ToList();
                if (result == null) result = new List<DM_Hoidongthi_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Hoidongthi GetDetailById(int Id)
        {
            try
            {
                var hoidong = _dbContext.DM_Hoidongthi.FirstOrDefault(c => c.Id == Id);
                return hoidong;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Hoidongthi hoidong)
        {
            try
            {
                _dbContext.DM_Hoidongthi.Add(hoidong);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Hoidongthi hoidong)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Hoidongthi.Update(hoidong);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(int id)
        {
            try
            {
                var hoidong = _dbContext.DM_Hoidongthi.Find(id);
                if (hoidong == null)
                {
                    return false;
                }
                _dbContext.DM_Hoidongthi.Remove(hoidong);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckMa(int Ma, int idNam, int? Id)
        {
            try
            {
                var query = _dbContext.DM_Hoidongthi.Where(c => c.Ma == Ma && c.Id_nam == idNam);

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
        
        public bool CheckId(int Id)
        {
            if (Id <= 0) return false;
            try
            {
                return _dbContext.DM_Hoidongthi.Any(c => c.Id == Id);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.DM_Hoidongthi.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }

    }
}
