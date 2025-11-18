using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_NamhocRepository
    {
        List<DM_Namhoc_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord);
        DM_Namhoc GetDetailById(int Id);
        bool Add(DM_Namhoc dm_Namhoc);
        bool Update(DM_Namhoc dm_Namhoc);
        bool Delete(int id);
        public bool CheckId(int Id);
        bool CheckKhoangNgay(DM_Namhoc nam);
        bool Check_constraint(int Id);
        int GetMaxTuanByNam(int Id, int IdDonvi);
    }
}
 