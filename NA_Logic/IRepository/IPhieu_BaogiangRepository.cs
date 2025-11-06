using NA_Entities.Entities.Danh_muc;
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
        bool Add(int idLgb, int idtkb);
        bool Delete(int idlgb);
    }
}
