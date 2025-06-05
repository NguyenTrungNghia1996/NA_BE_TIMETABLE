using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_DiemtruongRepository
    {
        List<DM_Diemtruong_List> GetList_Paging(int PageIndex, int PageSize, string search, int Id_Donvi, ref int totalrecord);
        DM_Diemtruong GetDetailById(int Id, int Id_Donvi);
        bool Add(DM_Diemtruong dM_Diemtruong);
        bool Update(DM_Diemtruong dM_Diemtruong);
        bool Delete(int Id);
    }
}
