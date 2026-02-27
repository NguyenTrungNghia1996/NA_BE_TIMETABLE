using Microsoft.AspNetCore.Http;
using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_HocsinhRepository
    {
        List<DM_Hocsinh_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, int idLop, int idKhoi, int idLopOn);
        DM_Hocsinh GetDetailById(int Id, int idDonvi);
        bool CheckMa(string Ma, int idDonvi, int? Id);
        bool Add(DM_Hocsinh hs);
        bool Update(DM_Hocsinh hs);
        bool Delete(int Id);
        bool CheckId(int Id, int idDonvi);
        bool CheckIds(IEnumerable<int> ids, int idDonvi);
        (bool success, string mess) Import(IFormFile file, int idDonvi);
        int CountHocSinhByLop(int idLop);
        bool CheckContraint(int id, int idDonvi);
    }
}
