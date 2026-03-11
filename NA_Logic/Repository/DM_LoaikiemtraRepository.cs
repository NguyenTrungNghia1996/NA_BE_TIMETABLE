using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository.XepThoiKhoaBieu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class DM_LoaikiemtraRepository : IDM_LoaikiemtraRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_LoaikiemtraRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Loaikiemtra_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi)
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

                var result = _dbContext.Set<DM_Loaikiemtra_List>().FromSqlRaw("EXEC [DM_Loaikiemtra_GetList_Paging] @pageIndex, @pageSize, @search,@idDonvi",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi).ToList();
                if (result == null) result = new List<DM_Loaikiemtra_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Loaikiemtra GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var Lopontap = _dbContext.DM_Loaikiemtra.FirstOrDefault(c => c.Id == Id && c.Id_don_vi == idDonvi);
                return Lopontap;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Loaikiemtra hs)
        {
            try
            {
                _dbContext.DM_Loaikiemtra.Add(hs);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Loaikiemtra hs)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Loaikiemtra.Update(hs);
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
                bool check = (from bai in _dbContext.DM_Baikiemtra.AsNoTracking()
                              join loai in _dbContext.DM_Loaikiemtra.AsNoTracking() on bai.Id_loai_kiem_tra equals loai.Id
                              where loai.Id_don_vi == idDonvi && loai.Id == id
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
                var hs = _dbContext.DM_Loaikiemtra.FirstOrDefault(c => c.Id == Id);
                _dbContext.DM_Loaikiemtra.Remove(hs);
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
                return _dbContext.DM_Loaikiemtra.Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids, int idDonvi)
        {
            var existingIds = _dbContext.DM_Loaikiemtra.Where(c => c.Id_don_vi == idDonvi && ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
    }
}
