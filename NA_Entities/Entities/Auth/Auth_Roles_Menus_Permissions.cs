using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Auth
{
    public class Auth_Roles_Menus_Permissions
    {
        public int Id { get; set; } = 0;
        public int Id_Menu { get; set; }
        public int Id_Role { get; set; }
        public bool? Read_Only { get; set; }
    }
}