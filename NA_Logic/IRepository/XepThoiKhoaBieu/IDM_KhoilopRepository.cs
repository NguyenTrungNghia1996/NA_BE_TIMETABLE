using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.XepThoiKhoaBieu
{
    public interface  IDM_KhoilopRepository
    {
        DM_Khoilop getDetailById(int id);
        List<DM_Khoilop_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord);
        bool Add(DM_Khoilop dM_Khoilop);
        bool Update(DM_Khoilop dM_Khoilop);
        (bool success, string message) Delete(int Id);
        List<Khoilop_byDonvi> GetKhoilopByDonvi(int idDonvi, string search);
        bool CheckId(int Id);
        bool CheckKhoilopByDonvi(int idKhoilop, int idDonvi);
        bool CheckIds(IEnumerable<int> ids);
    }
}
