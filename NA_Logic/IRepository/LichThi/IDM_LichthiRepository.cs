using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.LichThi
{
    public interface IDM_LichthiRepository
    {
        List<DM_Lichthi_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, int idDiemThi, int idHoiDong);
        DM_Lichthi_Detail GetDetailById(int Id, int idDonvi);
        bool Add(DM_Lichthi lichthi);
        bool Update(DM_Lichthi lichthi);
        bool Delete(int id);
        bool CheckId(int Id, int idDonvi);
    }
}
