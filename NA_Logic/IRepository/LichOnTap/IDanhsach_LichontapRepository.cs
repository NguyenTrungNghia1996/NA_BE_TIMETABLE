using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.LichOnTap
{
    public interface IDanhsach_LichontapRepository
    {
        List<Danhsach_Lichontap_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, ref int totalrecord);
        Lichontap_Detail GetDetailById(int Id, int idDonvi);
        List<Chitiet_Lichontap> GetDetailLich(int Id);
        bool Add(Danhsach_Lichontap lichon);
        bool AddChiTiet(int Id, int iddonvi);
        bool Update(Danhsach_Lichontap lichon);
        bool Huy_KQ(int Id);
        bool SetStatus(int id, int idDonVi);
        bool Delete(int id);
        bool DeleteDetail(int Id);
        bool CheckId(int Id, int idDonvi);
        bool CheckId_ChiTiet(int Id, int idDonvi);
    }
}
