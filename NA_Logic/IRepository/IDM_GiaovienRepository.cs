using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_GiaovienRepository
    {
        List<DM_Giaovien_List> GetList_Paging(int PageIndex, int PageSize, string search, int idMon, int idDonvi, ref int totalrecord);
        DM_Giaovien GetDetailById(int Id, int idDonvi);
        List<int> GetlistDiadiemday(int id);
        bool Add(DM_Giaovien dm_Giaovien);
        bool AddDiadiemday(int Id, List<int> diemtruongId);
        bool AddTochuyenmon(int Id, List<int> diemtruongId);
        bool Update(DM_Giaovien dm_Giaovien);
        bool UpdateDiadiemday(int Id, List<int> diemtruongId);
        bool UpdateTochuyenmon(int Id, List<int> diemtruongId);
        bool Delete(int Id);
        bool DeleteDiadiemday(int Id);
        bool DeleteTochuyenmon(int Id);
        bool checkContraints(int Id);
        bool CheckMa(string Ma, int idDonvi, int? Id);
        bool CheckId(int Id, int idDonvi);
        bool CheckIds(IEnumerable<int> ids);
        Giaovien_banDto GetListTietBan(int Id, int idDonvi);
        bool AddTietBan(List<Giaovien_Tiettranhxep> dsTietTranhXep, int Idgv);
        bool SaveBuoiday(Giaovien_Buoiday gvbd);
        Giaovien_Buoiday GetBuoidayTheoGV(int? id);
        Giaovien_MonDto GetMonbyGiaovien(int id, int idDonvi);
        bool UpdateMonbyGiaovien(List<Giaovien_Monhoc> dsGiaovienMon, int Idgv);
        bool DeleteMonbyGiaovien(int Idgv);
        bool DeleteTietBan(int id);
        bool DeleteBuoiday(int id);
    }
}
