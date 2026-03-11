using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.LichOnTap
{
    public interface IDM_LopontapRepository
    {
        List<DM_Lopontap_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, int idKhoi, int idMon);
        DM_Lopontap GetDetailById(int Id);
        bool CheckMa(string Ma, int idDonvi, int? Id);
        bool Add(DM_Lopontap dm_Lopontap);
        bool Update(DM_Lopontap dm_Lopontap);
        bool Delete(int Id);
        bool CheckId(int Id, int idDonvi);
        bool CheckIds(IEnumerable<int> ids);
        Lopontap_TietnghiDto GetListTietBan(int Id, int idDonvi);
        bool AddTietBan(List<Lopontap_Tietnghi> dsTietBan, int idPhong);
        bool DeleteTietBan(int Id);
        bool CheckContraint(int id, int idDonvi);
        bool CheckTrung(DM_LopontapDto dm_Lopontap, int idDonvi);
    }
}
