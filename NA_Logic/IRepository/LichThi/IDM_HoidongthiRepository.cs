using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.XepGiamThi
{
    public interface IDM_HoidongthiRepository
    {
        List<DM_Hoidongthi_List> GetList_Paging(int PageIndex, int PageSize, string search);
        DM_Hoidongthi GetDetailById(int Id);
        bool Add(DM_Hoidongthi hoidong);
        bool Update(DM_Hoidongthi hoidong);
        bool Delete(int id);
        bool CheckMa(int Ma, int idNam, int? Id);
        bool CheckId(int Id);
    }
}
