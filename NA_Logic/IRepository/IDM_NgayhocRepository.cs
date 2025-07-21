using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_NgayhocRepository
    {
        List<DM_Ngayhoc_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord);
        DM_Ngayhoc GetDetailById(int Id);
        List<DM_Ngayhoc> GetListNgayhocByDonvi(int idDonvi);
        bool Add(DM_Ngayhoc dm_Ngayhoc);
        bool Update(DM_Ngayhoc dm_Ngayhoc);
        bool Delete(int Id);
        bool CheckId(int Id);
        bool CheckIds(IEnumerable<int> ids);
    }
}
