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
    public class DM_TiethocRepository : IDM_TiethocRepository
    {
        private readonly NA_DbContext _context;
        public DM_TiethocRepository(NA_DbContext context)
        {
            _context = context;
        }

        public List<DM_Tiethoc_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord)
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
                var result = _context.Set<DM_Tiethoc_List>().FromSqlRaw("EXEC DM_Tiethoc_GetList_Paging @pageIndex, @pageSize, @search, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Tiethoc_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Tiethoc getDetailById(int id)
        {
            try
            {
                var data = _context.DM_Tiethoc.Find(id);
                return data;
            }
            catch
            {
                return null;
            }
        }
        public bool Add(DM_Tiethoc dM_Tiethoc)
        {
            try
            {
                _context.DM_Tiethoc.Add(dM_Tiethoc);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Tiethoc dM_Tiethoc)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.DM_Tiethoc.Update(dM_Tiethoc);
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
                DM_Tiethoc item = new DM_Tiethoc();
                item = _context.DM_Tiethoc.Find(Id);
                if (item != null)
                {
                    _context.DM_Tiethoc.Remove(item);
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