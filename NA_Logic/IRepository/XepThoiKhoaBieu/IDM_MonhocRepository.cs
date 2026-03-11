using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.XepThoiKhoaBieu
{
    public interface IDM_MonhocRepository
    {
        List<DM_Monhoc_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, int idloaiphong, int id_lop, int idKhoi, int idBan, ref int totalrecord);
        DM_Monhoc GetDetailById(int id, int idDonvi);
        List<int> GetlistKhoikienthucbyMon(int id);
        bool Add(DM_Monhoc dM_Monhoc);
        bool AddKhoikienthuc(int Id, List<int> khoiId);
        bool Update(DM_Monhoc dM_Monhoc);
        bool UpdateKhoikienthuc(int Id, List<int> khoiId);
        bool Delete(int Id, int idDonvi);
        bool DeleteKhoi(int Id);
        bool CheckId(int Id, int idDonvi);
        bool CheckIdMonLop(int Id, int Id_lop, int idDonvi);
        bool CheckIds(IEnumerable<int> ids, int idDonvi);
        Mon_banDto GetListTietBan(int Id, int idDonvi);
        bool AddTietBan(List<Tiet_tranh_xep> dsTietTranhXep, int idMon);
        bool DeleteTietTranhXep(int Id);
        List<int> GetlistPhongByDonvi(int id);
        bool AddPhong(int Id, List<int> phongId);
        bool UpdatePhong(int Id, List<int> phongId);
        bool DeletePhong(int Id);
        bool CheckIdMonPhongChuyen(int Id, int idDonvi);
        bool CheckIdMonKhoi(int Id, int Id_khoi, int Id_ban, int idDonvi);
        bool CheckMa(string Ma, int idDonvi, int? Id);
        bool CheckTen(string Ten, int idDonvi, int? Id);
        List<MonLop> GetList_MonLop(int idDonvi, int id_lop);
        (bool success, string message) CheckContraints(int Id);
        bool DeleteTenMonTheoNganh(int Id);
        bool SaveTenMonTheoNganh(int Id, string Ten);
        string GetTenNgayByIdMon(int idmon);
    }
}
