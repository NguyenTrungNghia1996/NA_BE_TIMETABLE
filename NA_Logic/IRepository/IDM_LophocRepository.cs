using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_LophocRepository
    {
        List<DM_Lophoc_List> GetList_Paging(int PageIndex, int PageSize, string search,  int idDonvi, int idKhoilop, ref int totalrecord);
        DM_Lophoc getDetailById(int id);
        bool Add(DM_Lophoc dM_Lophoc);
        bool Update(DM_Lophoc dM_Lophoc);
        bool Check_limit(int idDonvi);
        bool Delete(int Id);
        bool CheckContraint(int id, int idDonvi);
        Lophoc_banDto GetListTietBan(int Id, int idDonvi);
        bool AddTietBan(List<Lophoc_Tietnghi> dsTietBan, int idLop);
        bool CheckId(int Id, int idDonvi);
        bool DeleteTietBan(int Id);
        bool CheckIds(IEnumerable<int> ids, int idDonvi);
    }
}
