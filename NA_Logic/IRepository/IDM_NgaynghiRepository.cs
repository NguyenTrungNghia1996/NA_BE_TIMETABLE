using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_NgaynghiRepository
    {
        List<DM_Ngaynghi_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord);
        DM_Ngaynghi GetDetailById(int Id);
        bool Add(DM_Ngaynghi dm_Ngaynghi);
        bool Update(DM_Ngaynghi dm_Ngaynghi);
        bool Delete(int id);
    }
}
