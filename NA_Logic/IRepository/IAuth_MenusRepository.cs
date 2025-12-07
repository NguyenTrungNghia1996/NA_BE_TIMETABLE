using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NA_Entities.Entities.Auth;

namespace NA_Logic.IRepository
{
    public interface IAuth_MenusRepository
    {
        List<Auth_MenusList> GetList_Pagging(int PageIndex, int PageSize, string search, ref int totalrecord);
        Auth_Menus GetDetailByID(int id);
        bool Add(Auth_Menus menu);
        bool Update(Auth_Menus menu);
        bool Deleted(int id);
    }
}