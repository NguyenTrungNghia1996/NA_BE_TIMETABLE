using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Auth
{
    [Flags]
    public enum MenuPermission
    {
        None = 0,
        Dashboard = 1 << 0,
        Reports = 1 << 1,
        Users = 1 << 2,
        Settings = 1 << 3,
        // Thêm quyền khác nếu cần
    }
}