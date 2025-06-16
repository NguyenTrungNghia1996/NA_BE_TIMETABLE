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
    public class DM_KhoilopRepository : IDM_KhoilopRepository
    {
        private readonly NA_DbContext _context;
        public DM_KhoilopRepository(NA_DbContext context)
        {
            _context = context;
        }

        public List<DM_Khoilop_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord)
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
                var result = _context.Set<DM_Khoilop_List>().FromSqlRaw("EXEC DM_Khoilop_GetList_Paging @pageIndex, @pageSize, @search, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Khoilop_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Khoilop getDetailById(int id)
        {
            try
            {
                var data = _context.DM_Khoilop.FirstOrDefault(c => c.Id == id && c.Trang_thai_xoa == false);
                return data;
            }
            catch
            {
                return null;
            }
        }
        public bool Add(DM_Khoilop dM_Khoilop)
        {
            try
            {
                _context.DM_Khoilop.Add(dM_Khoilop);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Khoilop dM_Khoilop)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.DM_Khoilop.Update(dM_Khoilop);
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
                DM_Khoilop item = new DM_Khoilop();
                item = _context.DM_Khoilop.FirstOrDefault(c => c.Id == Id && c.Trang_thai_xoa == false);
                if (item != null)
                {
                    item.Trang_thai_xoa = true;
                    _context.DM_Khoilop.Update(item);
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