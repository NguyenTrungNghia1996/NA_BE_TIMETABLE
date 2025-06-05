using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Dtos;

namespace NA_Logic.IRepository
{
    public interface IAuth_RolesRepository
    {
        List<Auth_RolesList> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord);
        Auth_Roles GetDetailByID(int id);
        bool Add(Auth_Roles role);
        bool Update(Auth_Roles role);
        bool Deleted(int id);
        List<Auth_Roles_PermissionDto> GetPermissionByRoleId(int idRole);

        // Role Permission
        bool AddPermission(List<Auth_Roles_Permissions> list_permssions);
        Auth_Roles_Permissions FindPermissionById(int id);
        bool DeletePermissionbyRoleId(int idRole);
    }
}