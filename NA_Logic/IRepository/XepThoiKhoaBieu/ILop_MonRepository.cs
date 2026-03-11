using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.XepThoiKhoaBieu
{
    public interface ILop_MonRepository
    {
        Lop_MonDto GetLopMon(int idLop, int idDonvi);
        bool AddMonLop(List<Lophoc_Monhoc> dsTietBan, int idLop);
        bool DeleteMonLop(int Id_lop);
        LopMon_banDto GetListTietBan_MonLop(int Id_lop, int Id_mon, int idDonvi);
        bool AddTietBan_MonLop(List<Lophoc_Monhoc_Tiettranhxep> dsTietBan, int idLop, int idMon);
        bool DeleteTietBan_MonLop(int Id_lop, int Id_mon);
    }
}
