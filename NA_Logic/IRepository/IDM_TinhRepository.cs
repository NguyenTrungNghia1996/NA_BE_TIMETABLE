using Microsoft.VisualBasic;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_TinhRepository
    {
        List<DM_Tinh> GetList();
    }
}
