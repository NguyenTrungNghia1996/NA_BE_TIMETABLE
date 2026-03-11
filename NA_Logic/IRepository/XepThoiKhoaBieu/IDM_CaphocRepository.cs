using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.XepThoiKhoaBieu
{
    public interface IDM_CaphocRepository
    {
        List<DM_Caphoc_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, ref int totalrecord);
        DM_Caphoc GetDetailByID (int Id);
        bool Add (DM_Caphoc dm_caphoc);
        bool Update(DM_Caphoc dm_caphoc);
        (bool success, string message) Delete(int Id);
        bool CheckId(int Id);
        bool CheckIds(IEnumerable<int> ids);
    }
}
