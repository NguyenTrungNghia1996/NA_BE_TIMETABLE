using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA_Entities.Entities.Auth;

namespace NA_Logic.IRepository
{
    public interface IAuthRepository
    {
        List<Auth_Users_List> GetListUsers_Paging(int PageIndex, int PageSize, string search, ref int totalrecord);
        Auth_Users FindUserByName(string username);
    }
}
