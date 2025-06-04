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
        public List<DM_Caphoc_List> GetList_Paging(int PageIndex, int PageSize, string search, int Id_Donvi, ref int totalrecord)
        {
            try
            {
                var paramSearch = new SqlParameter("search", SqlDbType.NVarChar) { Value = search ?? string.Empty };
                var paramPageIndex = new SqlParameter("pageIndex", SqlDbType.Int) { Value = PageIndex };
                var paramPageSize = new SqlParameter("pageSize", SqlDbType.Int) { Value = PageSize };
                var paramIdDonvi = new SqlParameter("Id_Donvi", SqlDbType.Int) { Value = Id_Donvi };
                var paramTotal = new SqlParameter("total", SqlDbType.Int) { Direction = ParameterDirection.Output };

                var result = _context.Set<DM_Caphoc_List>()
                    .FromSqlRaw("EXEC DM_Caphoc_GetList_Paging @pageIndex, @pageSize, @search, @Id_Donvi, @total OUTPUT",
                        paramPageIndex, paramPageSize, paramSearch, paramIdDonvi, paramTotal)
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
        public DM_Caphoc GetDetailByID(int Id, int Id_Donvi)
        {
            try
            {
                var caphoc = _context.DM_Caphoc.FirstOrDefault(c => c.Id == Id && c.Id_Donvi == Id_Donvi);
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
                item = _context.DM_Caphoc.Find(Id);
                if (item != null)
                {
                    _context.DM_Caphoc.Remove(item);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
