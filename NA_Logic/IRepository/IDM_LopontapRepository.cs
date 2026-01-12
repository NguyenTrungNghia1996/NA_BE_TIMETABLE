using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_LopontapRepository
    {
        List<DM_Lopontap_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi);
        DM_Lopontap GetDetailById(int Id);
        bool Add(DM_Lopontap dm_Lopontap);
        bool Update(DM_Lopontap dm_Lopontap);
        bool Delete(int Id);
        bool CheckId(int Id, int idDonvi);
        bool CheckIds(IEnumerable<int> ids);
    }
}
