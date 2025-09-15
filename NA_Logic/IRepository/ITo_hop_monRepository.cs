using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface ITo_hop_monRepository
    {
        List<Monhoc_Tohopmon_List> GetList_Paging(int PageIndex, int PageSize, int IdDonvi, ref int totalrecord);
        Monhoc_Tohopmon GetDetailById(int Id);
        bool Add(Monhoc_Tohopmon tohopmon);
        bool Update(Monhoc_Tohopmon tohopmon);
        bool Delete(int Id);
        bool CheckId(int Id, int idDonvi);
        bool CheckTrungTen(int idDonvi, string ten, int? excludeId = null);
    }
}
