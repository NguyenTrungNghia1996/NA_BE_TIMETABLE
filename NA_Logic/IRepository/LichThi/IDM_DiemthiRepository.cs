using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.LichThi
{
    public interface IDM_DiemthiRepository
    {
        List<DM_Diemthi_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi);
        DM_Diemthi GetDetailById(int Id, int idDonvi);
        bool Add(DM_Diemthi diemthi);
        bool Update(DM_Diemthi diemthi);
        bool Delete(int id, int idDonvi);
        bool CheckId(int Id, int idDonvi);
        bool CheckMa(string Ma, int idHoiDong, int? Id);
        bool Check_constraint(int Id);
    }
}
