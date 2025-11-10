using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_TiethocRepository
    {
        List<DM_Tiethoc_List> GetList_Paging(int PageIndex, int PageSize, string search,  ref int totalrecord);
        DM_Tiethoc getDetailById(int id);
        //List<int> GetlistCabyTiethoc(int id);
        bool Add(DM_Tiethoc dM_Tiethoc);
        //bool AddCa(int Id, List<int> caId);
        bool Update(DM_Tiethoc dM_Tiethoc);
        //bool UpdateCa(int Id, List<int> caId);
        bool Delete(int Id);
        //bool DeleteCa(int Id);
        //bool CheckId(int idTiet, int idCa);

    }
}
