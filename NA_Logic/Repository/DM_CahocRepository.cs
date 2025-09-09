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

        public List<DM_Cahoc_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, ref int totalrecord)
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
                var paramIddonvi = new SqlParameter("idDonvi", SqlDbType.Int)
                {
                    Value = idDonvi
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _dbContext.Set<DM_Cahoc_List>().FromSqlRaw("EXEC DM_Cahoc_GetList_Paging @pageIndex, @pageSize, @search, @idDonvi, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch,paramIddonvi, paramTotal)
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
                var cahoc = _dbContext.DM_Cahoc.FirstOrDefault(c => c.Id == Id);
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
        public (bool success, string message) Delete(int Id)
        {
            try
            {
                DM_Cahoc cahoc = new DM_Cahoc();
                cahoc = _dbContext.DM_Cahoc.FirstOrDefault(c => c.Id == Id);
                if (cahoc == null) {
                    return (false, "Bản ghi không tồn tại");
                }
                bool check = _dbContext.Database.SqlQueryRaw<int>($@"
                             select 1 as Value from Ca_Donvi where Id_ca_hoc = {Id}
                             union select 1 from Chitiet_Thoikhoabieu where Id_ca = {Id} 
                             union select 1 from DM_Lophoc where Id_ca = {Id}
                             union select 1 from Giaovien_Tiettranhxep  where Id_ca = {Id}
                             union select 1 from Lophoc_Monhoc_Tiettranhxep  where Id_ca = {Id}
                             union select 1 from Lophoc_Tietnghi  where Id_ca = {Id}
                             union select 1 from Monhoc_Khoilop  where Id_ca = {Id}
                             union select 1 from Monhoc_Khoilop_Tiettranhxep  where Id_ca = {Id}
                             union select 1 from Tiet_ban  where Id_ca = {Id}
                             union select 1 from Tiet_tranh_xep  where Id_ca = {Id}
                             union select 1 from Tiet_co_dinh  where Id_ca = {Id}").Any();
                if (check)
                    return (false, "Ca học đã có ràng buộc, không thể xoá");

                _dbContext.DM_Cahoc.Remove(cahoc);
                    _dbContext.SaveChanges();
                return (true,"Xoá thành công");
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
                var caValid = _dbContext.Ca_Donvi
                    .AsNoTracking()
                    .Any(c => c.Id_ca_hoc == Id && c.Id_don_vi == idDonvi);
                if (caValid)
                {
                    return true;
                }
                return false;
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
