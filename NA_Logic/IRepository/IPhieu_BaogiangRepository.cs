using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IPhieu_BaogiangRepository
    {
        List<Phieu_Baogiang_List> GetList_Paging(int PageIndex, int PageSize, int idlbg, string search, ref int totalrecord);
        Phieu_Baogiang GetDetailById(int Id);
        bool Add(int idLgb, int idtkb, int idDonvi);
        (bool result, string mess) Delete(int idlgb);
        Chitiet_PhieubaogiangDto GetList_Chitiet(int idpbg, int idDonvi);
    }
}
