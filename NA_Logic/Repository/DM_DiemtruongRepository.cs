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
    public class DM_DiemtruongRepository : IDM_DiemtruongRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_DiemtruongRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Diemtruong_List> GetList_Paging(int PageIndex, int PageSize, string search, int Id_Donvi, ref int totalrecord)
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
                var result = _dbContext.Set<DM_Diemtruong_List>().FromSqlRaw("EXEC DM_Diemtruong_GetList_Paging @pageIndex, @pageSize, @search, @Id_Donvi, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Diemtruong_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Diemtruong GetDetailById(int Id, int Id_Donvi)
        {
            try
            {
                var diemtruong = _dbContext.DM_Diemtruong.FirstOrDefault(c => c.Id == Id && c.Id_don_vi == Id_Donvi);
                return diemtruong;
            }
            catch (Exception) 
            {
                return null;
            }
        }
        public bool Add(DM_Diemtruong dM_Diemtruong)
        {
            try
            {
                _dbContext.DM_Diemtruong.Add(dM_Diemtruong);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Diemtruong dM_Diemtruong)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Diemtruong.Update(dM_Diemtruong);
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
                DM_Diemtruong diemtruong = new DM_Diemtruong();
                diemtruong = _dbContext.DM_Diemtruong.Find(Id);
                if (diemtruong == null)
                    { return (false, "Bản ghi không tồn tại"); }
                bool check = _dbContext.DM_Phonghoc.Any(c => c.Id_Diem_truong == Id)
                             || _dbContext.Giaovien_Diadiemday.Any(c => c.Id_diem_truong == Id);
                if (check)
                    return (false, "Điểm trường đã có ràng buộc, không thể xoá");

                _dbContext.DM_Diemtruong.Remove(diemtruong);
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
                return _dbContext.DM_Diemtruong.Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids, int idDonvi)
        {
            var existingIds = _dbContext.DM_Diemtruong.Where(c => ids.Contains(c.Id) && c.Id_don_vi == idDonvi).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
    }
}
