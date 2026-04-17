using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.LichThi
{
    public interface IPhongthi_ThisinhRepository
    {
        bool XepPhongTheoHoiDong(int idHoiDong);
        bool XepPhongTheoDiemThi(int idDiemThi);
        bool CheckPhongCoThiSinh(int idDiemThi);
        (bool isValid, string message) ValidateSoPhongTheoDiemThi(int idDiemThi);
        (bool isValid, string message) ValidateSoPhongTheoHoiDong(int idHoiDong);
        bool Delete(int id);
    }
}
