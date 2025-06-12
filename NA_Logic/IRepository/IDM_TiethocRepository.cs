using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_TiethocRepository
    {
        List<DM_Tiethoc_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord);
        DM_Tiethoc getDetailById(int id);
        bool Add(DM_Tiethoc dM_Tiethoc);
        bool Update(DM_Tiethoc dM_Tiethoc);
        bool Delete(int Id);

    }
}
