using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;

namespace NA_Logic.IRepository
{
    public interface IDM_LoaiphonghocRepository
    {
        DM_Loaiphonghoc GetDetailByID(int id);
        List<DM_Loaiphonghoc_List> GetList_Paging(int PageIndex, int PageSize, string search,  ref int totalrecord);
        bool Add(DM_Loaiphonghoc dM_Loaiphonghoc);
        bool Update(DM_Loaiphonghoc dM_Loaiphonghoc);
        (bool success, string message) Delete(int Id);
        bool CheckId(int Id);
    }
}