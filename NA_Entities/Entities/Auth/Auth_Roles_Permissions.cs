using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Auth
{
    public class Auth_Roles_Permissions
    {
        public int Id { get; set; } = 0;
        public int Id_Roles { get; set; }
        public string Key { get; set; } = string.Empty;
        public int PermissionValue { get; set; }
    }

}