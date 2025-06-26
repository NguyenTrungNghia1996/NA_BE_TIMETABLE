using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface ITiet_co_dinhRepository
    {
        List<Tiet_co_dinh_List> GetList_Paging(int PageIndex, int PageSize, int IdDonvi, ref int totalrecord);
        Tiet_co_dinh GetDetailById(int Id, int idDonvi);
        bool Add(List<Tiet_co_dinh> tiet_cd);
        bool Update(Tiet_co_dinh tiet_cd);
        bool UpdateAllKhoi(List<Tiet_co_dinh> tiet_cd, int idMon, int idDonvi);
        bool Delete(int Id, int idDonvi);
        bool CheckIds(int idMon, int idNgay, int idCa, int idTiet, int idKhoi, int idDonvi);
    }
}
