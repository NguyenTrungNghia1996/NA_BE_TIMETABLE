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
        // User
        List<Auth_Users_List> GetListUsers_Paging(int PageIndex, int PageSize, string search, ref int totalrecord);
        Auth_Users FindUserByName(string username);
        Auth_Users FindUserById(int id);
        bool checkIsAdmin(int id);
        bool CreateUser(Auth_Users user);
        bool AddUserToRoles(int userId, List<int> rolesId);
        bool UpdateUser(Auth_Users user);
        bool UpdateUserToRoles(int userId, List<int> rolesId);
        bool DeleteUsers_Roles(int idUser);
        bool DeleteUser(int id);

        //Role
        List<int> GetListIdRolesByUser(int id);
        bool checkRolesExist(List<int> idRoles);

    }
}
