using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_CahocRepository
    {
        List<DM_Cahoc_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord);
        DM_Cahoc GetDetailById(int Id);
        bool Add(DM_Cahoc dm_cahoc);
        bool Update(DM_Cahoc dm_cahoc);
        bool Delete(int Id);
        bool CheckId(int Id);
        bool CheckIds(IEnumerable<int> ids);
    }
}
