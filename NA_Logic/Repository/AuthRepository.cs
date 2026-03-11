using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NA_Entities.DBContext;
using NA_Entities.Entities.Auth;
using NA_Logic.IRepository;
using NA_Logic.IRepository.Auth;

namespace NA_Logic.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly NA_DbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasherRepository _passwordhash;
        public AuthRepository(NA_DbContext context, IConfiguration configuration, IPasswordHasherRepository passwordhash)
        {
            _context = context;
            _configuration = configuration;
            _passwordhash = passwordhash;
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
            catch (Exception)
            {
                return new List<Auth_Users_List> { };
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
        public Auth_Users FindUserById(int id)
        {
            try
            {
                var user = _context.Auth_Users.Find(id);
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool checkIsAdmin(int id)
        {
            try
            {
                var user = _context.Auth_Users.Find(id);
                if (user != null && (user.Username == "admin" || user.IsAdmin == true))
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        public bool CreateUser(Auth_Users user)
        {
            try
            {
                var getPass = _configuration.GetSection("Password").Value;
                if (getPass == null) getPass = "12345";
                string hashPassword = _passwordhash.HashPassword(getPass);
                user.Password = hashPassword;
                _context.Auth_Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool RegisterUser(Auth_Users user)
        {
            try
            {
                string hashPassword = _passwordhash.HashPassword(user.Password);
                user.Password = hashPassword;
                _context.Auth_Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool AddUserToRoles(int userId, List<int> rolesId)
        {
            try
            {
                if (rolesId != null && rolesId.Count > 0)
                {
                    for (int i = 0; i < rolesId.Count; i++)
                    {
                        var userRole = new Auth_Users_Roles
                        {
                            Id_Users = userId,
                            Id_Roles = rolesId[i]
                        };
                        _context.Auth_Users_Roles.Add(userRole);
                    }
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        // Roles
        public List<int> GetListIdRolesByUser(int id)
        {
            try
            {
                var list = _context.Auth_Users_Roles.Where(x => x.Id_Users == id).Select(x => x.Id_Roles).ToList();
                if (list == null) return new List<int>();
                return list;
            }
            catch
            {
                return new List<int>();
            }
        }
        public bool checkRolesExist(List<int> idRoles)
        {
            try
            {
                if (idRoles == null || idRoles.Count == 0)
                    return false; // Không có id thì trả về false luôn

                // Đếm số id tìm được trong database
                int countInDb = _context.Auth_Roles.Count(r => idRoles.Contains(r.Id));

                // Nếu số id tìm được bằng số id truyền vào thì tất cả đều tồn tại
                if (countInDb == idRoles.Count)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateUser(Auth_Users user)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.Auth_Users.Update(user);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdatePassword(Auth_Users user)
        {
            try
            {
                string hashPassword = _passwordhash.HashPassword(user.Password);
                user.Password = hashPassword;
                _context.Auth_Users.Update(user);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateUserToRoles(int userId, List<int> rolesId)
        {
            try
            {
                var del = _context.Auth_Users_Roles.Where(x => x.Id_Users == userId).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Auth_Users_Roles.RemoveRange(del);
                }
                for (int i = 0; i < rolesId.Count; i++)
                {
                    var userRole = new Auth_Users_Roles
                    {
                        Id_Users = userId,
                        Id_Roles = rolesId[i]
                    };
                    _context.Auth_Users_Roles.Add(userRole);
                }
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteUser(int id)
        {
            try
            {
                var user = _context.Auth_Users.Find(id);
                if (user != null)
                {
                    _context.Auth_Users.Remove(user);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public (bool success, string message) DeleteUsers_Roles(int idUser)
        {
            try
            {
                var del = _context.Auth_Users_Roles.Where(x => x.Id_Users == idUser).ToList();
                var check = _context.Auth_Users.Any(c => c.Id == idUser && c.IsActive == true);
                if (check)
                {
                    return (false, "Tài khoản đang hoạt động");
                }
                if (del != null && del.Count > 0)
                {
                    _context.Auth_Users_Roles.RemoveRange(del);
                    _context.SaveChanges();
                }
                return (true, "Xoá thành công");
            }
            catch (Exception ex) 
            {
                return (false, $"Lỗi hệ thống: {ex.Message}"); ;
            }
        }
        public bool CheckUser_DonviExists(int id)
        {
            try
            {
                var exists = (from user in _context.Auth_Users
                              join donvi in _context.DM_Donvi
                              on user.Id_Donvi equals donvi.Id
                              where user.Id == id
                              select user).Any();
                return exists;
            }
            catch
            {
                return false;
            }
        }
        public bool ResetPassword(int id)
        {
            try
            {
                var getPass = _configuration.GetSection("Password").Value;
                if (getPass == null) getPass = "12345";
                string hashPassword = _passwordhash.HashPassword(getPass);
                var user = _context.Auth_Users.Find(id);
                if(user ==null) return false;

                user.Password = hashPassword;
                _context.Auth_Users.Update(user);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
