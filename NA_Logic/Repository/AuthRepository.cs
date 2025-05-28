using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Auth;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class AuthRepository :IAuthRepository
    {
        private readonly NA_DbContext _context;
        public AuthRepository(NA_DbContext context)
        {
            _context = context;
        }
        public List<Auth_Users_List> GetListUsers_Paging(int PageIndex, int PageSize, string search, ref int totalrecord)
        {
            try
            {
                var paramSearch = new SqlParameter("search", SqlDbType.NVarChar) { Value = search ?? string.Empty };
                var paramPageIndex = new SqlParameter("pageIndex", SqlDbType.Int) { Value = PageIndex };
                var paramPageSize = new SqlParameter("pageSize", SqlDbType.Int) { Value = PageSize };
                var paramTotal = new SqlParameter("total", SqlDbType.Int) { Direction = ParameterDirection.Output };

                var result = _context.Set<Auth_Users_List>()
                    .FromSqlRaw("EXEC Auth_GetListUsers_Paging @pageIndex, @pageSize, @search, @total OUTPUT",
                        paramPageIndex, paramPageSize, paramSearch, paramTotal)
                    .ToList();

                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception ex)
            {
                return new List<Auth_Users_List>{ };
            }
        }
        public Auth_Users FindUserByName(string username)
        {
            try
            {
                var user = _context.Auth_Users.Where(x => x.Username == username).FirstOrDefault();
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
