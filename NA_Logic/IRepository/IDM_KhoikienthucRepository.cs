using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_KhoikienthucRepository
    {
        List<DM_Khoikienthuc_List> GetList_Paging(int PageIndex, int PageSize, string search,int IdDonvi, ref int totalrecord);
        DM_Khoikienthuc GetDetailById(int Id, int idDonvi);
        bool Add(DM_Khoikienthuc dM_Khoikienthuc);
        bool Update(DM_Khoikienthuc dM_Khoikienthuc);
        (bool success, string message) Delete(int Id);
        bool CheckId(int Id, int idDonvi);
        bool CheckIds(IEnumerable<int> ids, int idDonvi);
    }
}
