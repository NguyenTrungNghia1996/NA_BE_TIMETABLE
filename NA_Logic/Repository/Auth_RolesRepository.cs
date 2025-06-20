using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;

namespace NA_Logic.Repository
{
    public class Auth_RolesRepository : IAuth_RolesRepository
    {
        private readonly NA_DbContext _context;
        public Auth_RolesRepository(NA_DbContext context)
        {
            _context = context;
        }
        public List<Auth_RolesList> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord)
        {
            try
            {
                var paramSearch = new SqlParameter("search", SqlDbType.NVarChar) { Value = search ?? string.Empty };
                var paramPageIndex = new SqlParameter("pageIndex", SqlDbType.Int) { Value = PageIndex };
                var paramPageSize = new SqlParameter("pageSize", SqlDbType.Int) { Value = PageSize };
                var paramTotal = new SqlParameter("total", SqlDbType.Int) { Direction = ParameterDirection.Output };

                var result = _context.Set<Auth_RolesList>()
                    .FromSqlRaw("EXEC Auth_GetlistRoles_Pagging @pageIndex, @pageSize, @search, @total OUTPUT",
                        paramPageIndex, paramPageSize, paramSearch, paramTotal)
                    .ToList();
                if (result == null) result = new List<Auth_RolesList>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return new List<Auth_RolesList>();
            }
        }
        public Auth_Roles GetDetailByID(int id)
        {
            try
            {
                var role = _context.Auth_Roles.FirstOrDefault(c => c.Id == id);
                return role;
            }
            catch
            {
                return null;
            }
        }
        public bool Add(Auth_Roles role)
        {
            try
            {
                _context.Auth_Roles.Add(role);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Update(Auth_Roles role)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.Auth_Roles.Update(role);
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
                Auth_Roles item = new Auth_Roles();
                item = _context.Auth_Roles.Find(id);
                if (item != null)
                {
                    _context.Auth_Roles.Remove(item);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public List<Auth_Roles_PermissionDto> GetPermissionByRoleId(int idRole)
        {
            try
            {
                var permissions = _context.Auth_Roles_Permissions
                                          .Where(c => c.Id_Roles == idRole)
                                          .Select(c => new Auth_Roles_PermissionDto
                                          {
                                              Key = c.Key,
                                              PermissionValue = c.PermissionValue
                                          })
                                          .ToList();
                return permissions;
            }
            catch
            {
                return new List<Auth_Roles_PermissionDto>();
            }
        }
        public bool AddPermission(List<Auth_Roles_Permissions> list_permssions)
        {
            try
            {
                var roleId = list_permssions.FirstOrDefault()?.Id_Roles ?? 0;

                // Bước 1: Lấy danh sách hiện tại trong DB với RoleId tương ứng
                var existing = _context.Auth_Roles_Permissions
                                       .Where(x => x.Id_Roles == roleId)
                                       .ToList();

                // Bước 2: Xóa những bản ghi cũ
                if (existing.Any())
                    _context.BulkDelete(existing);
                _context.BulkInsert(list_permssions);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public Auth_Roles_Permissions FindPermissionById(int id)
        {
            try
            {
                var permission = _context.Auth_Roles_Permissions.FirstOrDefault(c => c.Id == id);
                return permission;
            }
            catch
            {
                return null;
            }
        }
        public bool DeletePermissionbyRoleId(int idRole)
        {
            try
            {
                var permissions = _context.Auth_Roles_Permissions.Where(c => c.Id_Roles == idRole).ToList();
                if (permissions.Count > 0)
                {
                    _context.Auth_Roles_Permissions.RemoveRange(permissions);
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