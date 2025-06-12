using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_PhonghocRepository
    {
        List<DM_Phonghoc_list> GetList_Paging(int PageIndex, int PageSize, string search,int idDiemtruong,int idDonvi, ref int totalrecord);
        DM_Phonghoc getDetailById(int id);
        bool Add(DM_Phonghoc dM_Phonghoc);
        bool Update(DM_Phonghoc dM_Phonghoc);
        bool Delete(int Id);
    }
}
