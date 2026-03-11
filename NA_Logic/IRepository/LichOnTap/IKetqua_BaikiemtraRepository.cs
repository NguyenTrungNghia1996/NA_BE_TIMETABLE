using Microsoft.AspNetCore.Http;
using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.LichOnTap
{
    public interface IKetqua_BaikiemtraRepository
    {
        byte[] ExportMauExcel(int idlop);
        bool Add(int id_bai);
        (bool success, string mess) Import(IFormFile file, int id_bai);
        List<KetQua_Baikiemtra_List> Getlist(int idbai);
        bool UpdateDiem(List<KetQua_Baikiemtra_List> listKetQua);
        bool CheckId(int Id, int idDonvi);
        object GetKetQuaHocSinh(int PageIndex, int PageSize, int id_lop_on, int id_don_vi);
        object GetKetQuaHocSinh_ToHopMon(int PageIndex, int PageSize, int id_to_hop, int id_khoi, int id_don_vi);
    }
}
