using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_GiaovienRepository
    {
        List<DM_Giaovien_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord);
        DM_Giaovien GetDetailById(int Id);
        bool Add(DM_Giaovien dm_Giaovien);
        bool Update(DM_Giaovien dm_Giaovien);
        bool Delete(int Id);
        bool CheckId(int Id, int idDonvi);
        bool CheckIds(IEnumerable<int> ids);
        Giaovien_banDto GetListTietBan(int Id, int idDonvi);
        bool AddTietBan(List<Giaovien_Tiettranhxep> dsTietTranhXep, int Idgv);
        bool SaveBuoiday(Giaovien_Buoiday gvbd);
        Giaovien_Buoiday GetBuoiday(int id);
    }
}
