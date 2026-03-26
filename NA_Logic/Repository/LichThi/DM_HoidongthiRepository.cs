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

        public List<DM_Hoidongthi_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, int idNam)
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
                var paramIdNam = new SqlParameter("idNam", SqlDbType.Int)
                {
                    Value = idNam
                };
                var result = _dbContext.Set<DM_Hoidongthi_List>().FromSqlRaw("EXEC [DM_Hoidongthi_GetList_Paging] @pageIndex, @pageSize, @search, @idDonvi, @idNam",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi, paramIdNam).ToList();
                if (result == null) result = new List<DM_Hoidongthi_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Hoidongthi GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var hoidong = _dbContext.DM_Hoidongthi.FirstOrDefault(c => c.Id == Id && c.Id_don_vi == idDonvi);
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
        public bool Delete(int id, int idDonvi)
        {
            try
            {
                var hoidong = _dbContext.DM_Hoidongthi.FirstOrDefault(c=> c.Id == id && c.Id_don_vi == idDonvi);
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
        public bool CheckMa(string Ma, int idDonVi, int idNam, int? Id)
        {
            try
            {
                var query = _dbContext.DM_Hoidongthi.Where(c => c.Ma == Ma && c.Id_don_vi == idDonVi && c.Id_nam == idNam);

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
                          select 1 as Value from DM_Diemthi where Id_hoi_dong = {Id}
                          union select 1 from DM_Monthi where Id_hoi_dong = {Id}").Any();
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
                return _dbContext.DM_Hoidongthi.Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
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
