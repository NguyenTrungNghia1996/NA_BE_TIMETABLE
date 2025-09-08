using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository;

namespace NA_Logic.Repository
{
    public class DM_CaphocRepository : IDM_CaphocRepository
    {
        private readonly NA_DbContext _context;
        public DM_CaphocRepository(NA_DbContext context)
        {
            _context = context;
        }
        public List<DM_Caphoc_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord)
        {
            try
            {
                var paramSearch = new SqlParameter("search", SqlDbType.NVarChar) { Value = search ?? string.Empty };
                var paramPageIndex = new SqlParameter("pageIndex", SqlDbType.Int) { Value = PageIndex };
                var paramPageSize = new SqlParameter("pageSize", SqlDbType.Int) { Value = PageSize };
                var paramTotal = new SqlParameter("total", SqlDbType.Int) { Direction = ParameterDirection.Output };

                var result = _context.Set<DM_Caphoc_List>()
                    .FromSqlRaw("EXEC DM_Caphoc_GetList_Paging @pageIndex, @pageSize, @search, @total OUTPUT",
                        paramPageIndex, paramPageSize, paramSearch, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Caphoc_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Caphoc GetDetailByID(int Id)
        {
            try
            {
                var caphoc = _context.DM_Caphoc.FirstOrDefault(c => c.Id == Id && c.Trang_thai_xoa == false);
                return caphoc;
            }
            catch
            {
                return null;
            }
        }
        public bool Add(DM_Caphoc dm_caphoc)
        {
            try
            {
                _context.DM_Caphoc.Add(dm_caphoc);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Update(DM_Caphoc dm_caphoc)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.DM_Caphoc.Update(dm_caphoc);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Deleted(int Id)
        {
            try
            {
                DM_Caphoc item = new DM_Caphoc();
                item = _context.DM_Caphoc.FirstOrDefault(c => c.Id == Id && c.Trang_thai_xoa == false);
                if(item == null) 
                    return false;
                bool check = _context.Cap_Donvi.Any(c=>c.Id_Cap_hoc == Id)
                             || _context.DM_Banhoc.Any(c=>c.Id_cap_hoc == Id)
                             || _context.DM_Khoilop.Any(c=>c.Id_Cap_hoc== Id);
                if (check)
                    throw new Exception("Cấp học đã có ràng buộc, không thể xoá");

                    _context.DM_Caphoc.Remove(item);
                    _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckId(int Id)
        {
            if (Id <= 0) return false;
            try
            {
                return _context.DM_Caphoc.Any(c => c.Id == Id && c.Trang_thai_xoa==false);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _context.DM_Caphoc.Where(c => ids.Contains(c.Id) && c.Trang_thai_xoa == false).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
    }
}
