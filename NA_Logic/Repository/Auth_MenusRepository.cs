using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Auth;
using NA_Logic.IRepository.Auth;

namespace NA_Logic.Repository
{
    public class Auth_MenusRepository : IAuth_MenusRepository
    {
        private readonly NA_DbContext _context;
        public Auth_MenusRepository(NA_DbContext context)
        {
            _context = context;
        }
        public List<Auth_MenusList> GetList_Pagging(int PageIndex, int PageSize, string search, ref int totalrecord)
        {
            try
            {
                var paramSearch = new SqlParameter("search", SqlDbType.NVarChar) { Value = search ?? string.Empty };
                var paramPageIndex = new SqlParameter("pageIndex", SqlDbType.Int) { Value = PageIndex };
                var paramPageSize = new SqlParameter("pageSize", SqlDbType.Int) { Value = PageSize };
                var paramTotal = new SqlParameter("total", SqlDbType.Int) { Direction = ParameterDirection.Output };

                var result = _context.Set<Auth_MenusList>()
                    .FromSqlRaw("EXEC Auth_GetlistMenus_Pagging @pageIndex, @pageSize, @search, @total OUTPUT",
                        paramPageIndex, paramPageSize, paramSearch, paramTotal)
                    .ToList();
                if (result == null) result = new List<Auth_MenusList>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return new List<Auth_MenusList>();
            }
        }
        public Auth_Menus GetDetailByID(int id)
        {
            try
            {
                var menu = _context.Auth_Menus.FirstOrDefault(c => c.Id == id);
                return menu;
            }
            catch
            {
                return null;
            }
        }
        public bool Add(Auth_Menus menu)
        {
            try
            {
                _context.Auth_Menus.Add(menu);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Update(Auth_Menus menu)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.Auth_Menus.Update(menu);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Deleted(int id)
        {
            try
            {
                Auth_Menus item = new Auth_Menus();
                item = _context.Auth_Menus.Find(id);
                if (item != null)
                {
                    _context.Auth_Menus.Remove(item);
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