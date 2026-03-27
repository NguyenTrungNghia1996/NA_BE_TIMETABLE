using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.LichThi
{
    public interface IDM_PhongthiRepository
    {
        List<DM_Phongthi_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi);
        DM_Phongthi GetDetailById(int Id, int idDonvi);
        bool Add(DM_Phongthi phongthi);
        bool Update(DM_Phongthi phongthi);
        bool Delete(int id);
        bool CheckId(int Id, int idDonvi);
        (bool result, string mess) Import(Stream file, int idDonvi);
    }
}
