using Microsoft.AspNetCore.Http;
using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.LichThi
{
    public interface IDM_GiamthiRepository
    {
        List<DM_Giamthi_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDiemthi);
        DM_Giamthi GetDetailById(int Id, int idDonvi);
        DM_Giamthi_Multi GetGiamThiByDiemThi(int IdDiemThi);
        bool Add(DM_Giamthi gt);
        bool AddList(DM_Giamthi_Multi data);
        bool Update(DM_Giamthi gt);
        bool Delete(int id);
        bool CheckMa(string Ma, int idDiemThi, int? Id);
        bool CheckId(int Id, int idDiemThi);
        bool Check_constraint(int Id);
        (bool success, string mess) Import(IFormFile file, int idDonvi);
    }
}
