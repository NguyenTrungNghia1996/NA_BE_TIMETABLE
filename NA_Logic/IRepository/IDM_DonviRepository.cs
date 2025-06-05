using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;

namespace NA_Logic.IRepository
{
    public interface IDM_DonviRepository
    {
        DM_Donvi getDonviById(int id);
        List<DM_Donvi_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord);
        bool Add(DM_Donvi dm_donvi);
        bool Update(DM_Donvi dm_donvi);
        bool Delete(int Id);
    }
}