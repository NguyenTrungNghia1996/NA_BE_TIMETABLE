using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.LichThi
{
    public interface IXepGiamThiRepository
    {
        bool XepGiamThi(int idLich);
        (bool success, string mess) XepMotGiamThi(int idLich, int idGiamThi);
        bool HuyKetQua(int idLich);
        object GetChiTietLichCoiThi(int idLich);
        List<DM_Giamthi> GetListChuaXep(int idLich);
        List<DM_Giamthi> GetListPhongCho(int idLich);
        bool HuyKetQuaPhong(int idPhong);
        bool HuyKetQuaGiamSat(int idGiamSat);
    }
}
