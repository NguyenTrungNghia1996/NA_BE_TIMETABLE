using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.LichThi
{
    public interface IDM_DonviHanhchinhRepository
    {
        List<DM_DonviHanhchinh> GetList_Tinh_Paging(string search);
        List<DM_DonviHanhchinh> GetList_Xa_Paging(string search, int IdTinh);
        bool CheckId(int Id);
    }
}
