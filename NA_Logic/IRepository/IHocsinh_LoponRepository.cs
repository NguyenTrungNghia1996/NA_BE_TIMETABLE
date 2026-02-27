using Microsoft.AspNetCore.Http;
using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IHocsinh_LoponRepository
    {
        List<Hocsinh_Lopon_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, int idLopOn);
        Hocsinh_Lopon GetDetailById(int Id, int idDonvi);
        Hocsinh_Lopon_Multi GetHocSinhByIdLop(int IdLop, int idDonvi);
        bool Add(Hocsinh_Lopon_Multi data);
        bool Update(Hocsinh_Lopon hs);
        bool Delete(int Id);
        bool DeleteByLop(int Id);
        bool DeleteByHocSinh(int Id);
        bool CheckId(int Id, int idDonvi);
        bool CheckIds(IEnumerable<int> ids);
        bool CheckTrung(Hocsinh_Lopon hs, int idDonvi);
        (bool success, string mess) Import(IFormFile file, int idDonvi);
        bool CheckListStudentsSameGrade(Hocsinh_Lopon_Multi data, int idDonvi);
        bool CheckStudentSameGrade(Hocsinh_Lopon ds, int idDonvi);
    }
}
