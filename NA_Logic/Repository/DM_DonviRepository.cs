using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Logic.IRepository;

namespace NA_Logic.Repository
{
    public class DM_DonviRepository : IDM_DonviRepository
    {
        private readonly NA_DbContext _context;
         public DM_DonviRepository(NA_DbContext context)
        {
            _context = context;
        }
        public DM_Donvi getDonviById(int id)
        {
            try
            {
                var data = _context.DM_Donvi.Find(id);
                return data;
            }
            catch
            {
                return null;
            }
        }
        public List<DM_Donvi_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord)
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
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _context.Set<DM_Donvi_List>().FromSqlRaw("EXEC DM_Cahoc_GetList_Paging @pageIndex, @pageSize, @search, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch,  paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Donvi_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Donvi dm_donvi)
        {
            try
            {
                _context.DM_Donvi.Add(dm_donvi);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Donvi dm_donvi)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.DM_Donvi.Update(dm_donvi);
                _context.SaveChanges();
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
                DM_Donvi donvi = new DM_Donvi();
                donvi = _context.DM_Donvi.Find(Id);
                if (donvi != null)
                {
                    _context.DM_Donvi.Remove(donvi);
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