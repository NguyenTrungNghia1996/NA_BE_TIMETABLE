using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.XepThoiKhoaBieu
{
    public interface IDM_PhonghocRepository
    {
        List<DM_Phonghoc_list> GetList_Paging(int PageIndex, int PageSize, string search,int idDiemtruong, int idLoaiph, int idDonvi,  ref int totalrecord);
        DM_Phonghoc getDetailById(int id, int idDonvi);
        bool Add(DM_Phonghoc dM_Phonghoc);
        bool Update(DM_Phonghoc dM_Phonghoc);
        bool Delete(int Id);
        Phong_banDto GetListTietBan(int Id, int idDonvi);
        bool AddTietBan(List<Tiet_ban> dsTietBan, int idPhong, bool applyAll = false, int idDonvi = 0);
        bool CheckId(int? Id, int idDonvi);
        bool CheckId_Phongchuyendung(int? Id, int idDonvi);
        bool CheckId_Phongtruyenthong(int? Id, int idDonvi);
        //bool CheckIds_Tietban(int idNgay, int idCa, int idTiet);
        bool DeleteTietBan(int Id);
        bool CheckIds(IEnumerable<int> ids, int idDonvi, int IdLoaiPhonghoc);
        bool CheckMa(string Ma, int idDonvi, int? Id);
        bool CheckTrungTen(int idDonvi, string ten, int? excludeId = null);
        (bool success, string message) checkContraints(int Id);
    }
}
