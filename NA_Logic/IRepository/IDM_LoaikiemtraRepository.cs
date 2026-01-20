using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_LoaikiemtraRepository
    {
        List<DM_Loaikiemtra_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi);
        DM_Loaikiemtra GetDetailById(int Id, int idDonvi);
        bool Add(DM_Loaikiemtra loaiKT);
        bool Update(DM_Loaikiemtra loaiKT);
        bool Delete(int Id);
        bool CheckId(int Id, int idDonvi);
        bool CheckIds(IEnumerable<int> ids, int idDonvi);
    }
}
