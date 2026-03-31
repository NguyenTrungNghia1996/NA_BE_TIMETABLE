using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.LichThi
{
    public interface IDM_MonthiRepository
    {
        List<DM_Monthi_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi);
        DM_Monthi_Detail GetDetailById(int Id, int idDonvi);
        DM_Monthi_Multi GetMonByIdHoiDong(int IdHoiDong, int idDonvi);
        bool Add(DM_Monthi mon);
        bool AddList(DM_Monthi_Multi data);
        bool Update(DM_Monthi mon);
        bool Delete(int id, int idDonvi);
        bool CheckMa(string Ma, int idHoidong, int? Id);
        bool CheckTrungTen(int idHoidong, string Ten, int? Id);
        bool CheckId(int? Id, int idDonvi);
    }
}
