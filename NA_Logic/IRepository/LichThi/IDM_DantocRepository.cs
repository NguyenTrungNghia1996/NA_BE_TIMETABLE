using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.LichThi
{
    public interface IDM_DantocRepository
    {
        List<DM_Dantoc> GetList_Paging(string search);
        public bool CheckId(int Id);
    }
}
