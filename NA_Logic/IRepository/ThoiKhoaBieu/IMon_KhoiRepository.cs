using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.XepThoiKhoaBieu
{
    public interface IMon_KhoiRepository
    {
        List<Monhoc_KhoiLopDto> GetMonhocKhoilop(int? idKhoi, int? idBan, int idDonvi);
        bool AddMonKhoi(List<Monhoc_Khoilop> dsMonKhoi, int idKhoi, int idBan);
        Monhoc_Khoilop_BanDto GetListTietBan(int Id_mon, int Id_khoi, int Id_ban, int idDonvi);
        bool AddTietBan(List<Monhoc_Khoilop_Tiettranhxep> dsTietTranhXep, int idMon, int idKhoi, int idBan);
        bool DeleteTietTranhXep(int Id_mon, int Id_khoi);
        bool DongBoLopMon(int idDonvi, int idKhoi, int idBan);
    }
}
