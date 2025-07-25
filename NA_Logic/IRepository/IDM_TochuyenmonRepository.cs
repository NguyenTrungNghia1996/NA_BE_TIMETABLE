using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_TochuyenmonRepository
    {
        List<DM_Tochuyenmon_List> GetList_Paging(int PageIndex, int PageSize, string search, int IdDonvi, ref int totalrecord);
        DM_Tochuyenmon GetDetailById(int Id);
        bool Add(DM_Tochuyenmon dM_Tochuyenmon);
        bool Update(DM_Tochuyenmon dM_Tochuyenmon);
        bool Delete(int Id);
        bool CheckId(int Id, int idDonvi);
    }
}
