using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.LichThi
{
    public interface IDM_ThisinhRepository
    {
        List<DM_Thisinh_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, int idDiemThi, int idHoiDong, int idPhong, bool? trangThai);
        DM_Thisinh_Detail GetDetailById(int Id, int idDonvi);
        bool Add(DM_Thisinh thisinh);
        bool Update(DM_Thisinh thisinh);
        bool Delete(int id);
        bool CheckId(int Id, int idDonvi);
        bool Check_constraint(int Id);
        (bool result, string mess, List<ThiSinhCheck?> list) Import(Stream file, int idDonvi);
        bool DanhSoBaoDanh(int idHoiDong);
        bool CheckCCCD(string CCCD, int idDiemThi, int? Id);
    }
}
