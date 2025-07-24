using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_BanhocRepository
    {
        List<DM_Banhoc_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord);
        DM_Banhoc GetDetailById(int Id);
        bool Add(DM_Banhoc dm_Banhoc);
        bool Update(DM_Banhoc dm_Banhoc);
        bool Delete(int Id);
        bool CheckId(int Id);
        bool CheckIds(IEnumerable<int> ids);
    }
}
