using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NA_Entities.Entities.Auth;

namespace NA_Logic.IRepository
{
    public interface IAuth_RolesRepository
    {
        List<Auth_RolesList> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord);
        Auth_Roles GetDetailByID (int id);
        bool Add (Auth_Roles role);
        bool Update(Auth_Roles role);
        bool Deleted(int id);
    }
}