using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_MonhocRepository
    {
        List<DM_Monhoc_List> GetList_Paging(int PageIndex, int PageSize, string search, int IdDonvi, ref int totalrecord);
        DM_Monhoc GetDetailById(int id, int idDonvi);
        bool Add(DM_Monhoc dM_Monhoc);
        bool Update(DM_Monhoc dM_Monhoc);
        bool Delete(int Id, int idDonvi);

    }
}
