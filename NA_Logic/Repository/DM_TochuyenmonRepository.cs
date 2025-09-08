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
    public class DM_TochuyenmonRepository : IDM_TochuyenmonRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_TochuyenmonRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Tochuyenmon_List> GetList_Paging(int PageIndex, int PageSize, string search, int IdDonvi, ref int totalrecord)
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
                var result = _dbContext.Set<DM_Tochuyenmon_List>().FromSqlRaw("EXEC DM_Tochuyenmon_GetList_Paging @pageIndex, @pageSize, @search, @Id_Donvi, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Tochuyenmon_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Tochuyenmon GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var item = _dbContext.DM_Tochuyenmon.FirstOrDefault(c => c.Id == Id && c.Id_don_vi==idDonvi);
                return item;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Tochuyenmon dM_Tochuyenmon)
        {
            try
            {
                _dbContext.DM_Tochuyenmon.Add(dM_Tochuyenmon);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Tochuyenmon dM_Tochuyenmon)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Tochuyenmon.Update(dM_Tochuyenmon);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public (bool success, string message) Delete(int Id)
        {
            try
            {
                DM_Tochuyenmon item = new DM_Tochuyenmon();
                item = _dbContext.DM_Tochuyenmon.Find(Id);
                if (item == null)
                {
                    return(false, "Bản ghi không tồn tại");
                }
                bool check = _dbContext.DM_Giaovien.Any(c => c.Id_to_chuyen_mon == Id);
                if (check)
                    return (false, "Tổ chuyên môn đã có ràng buộc, không thể xoá");

                _dbContext.DM_Tochuyenmon.Remove(item);
                    _dbContext.SaveChanges();

                return (true, "Xoá thành công");
            }
            catch (Exception ex) 
            {
                return (false, $"Lỗi hệ thống: {ex.Message}");
            }
        }
        public bool CheckId(int Id, int idDonvi)
        {
            if (Id <= 0) return false;
            try
            {
                return _dbContext.DM_Tochuyenmon.Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.DM_Tochuyenmon.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
    }
}
