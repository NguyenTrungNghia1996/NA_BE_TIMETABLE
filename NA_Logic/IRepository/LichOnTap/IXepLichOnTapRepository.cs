using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.LichOnTap
{
    public interface IXepLichOnTapRepository
    {
        bool ProcessThoiKhoaBieu(int idlich, int idDonvi);
        ObjectTietOnTap_theoLopDto GetLichByLop(int id_lop, int idlich, int idDonvi);
        ObjectTietOnTap_theoGVDto GetLichByGiaovien(int id_gv, int idlich, int idDonvi);
        List<Object_TietOnTapChuaXep> GetTietChuaXep(int idlich);
        ObjectTietOnTap_theoLopDto TimViTriXepDuoc_byLop(ObjectTietOnTap_theoLopDto tietDachon, int idDonvi);
        ObjectTietOnTap_theoGVDto TimViTriXepDuoc_byGV(ObjectTietOnTap_theoGVDto tietDachon, int idDonvi);
        (bool success, ObjectTietOnTap_theoLopDto result) DoiChoHaiTiet_Lop(ObjectTietOnTap_theoLopDto tietDachon, int idDonvi);
        (bool success, ObjectTietOnTap_theoGVDto result) DoiChoHaiTiet_GV(ObjectTietOnTap_theoGVDto tietDachon, int idDonvi);
        List<Object_TietOnTap> TimTietXepDuoc_byLop(ObjectTietOnTap_theoLopDto tietDachon, int idDonvi);
        List<Object_TietOnTap> TimTietXepDuoc_byGV(ObjectTietOnTap_theoGVDto tietDachon, int idDonvi);
        bool UpdateTietChuaXep(Object_TietOnTap tietDachon, int idDonvi);
        ObjectTietOnTap_theoLopDto TimViTriXepDuoc_TietChuaXep_byLop(ObjectTietOnTap_theoLopDto tietDachon, int idDonvi);
        ObjectTietOnTap_theoGVDto TimViTriXepDuoc_TietChuaXep_byGV(ObjectTietOnTap_theoGVDto tietDachon, int idDonvi);
        bool Xeplich_byLop(List<int> idlop, int idlich, int idDonvi);
        bool Xeplich_byGV(List<int> idgv, int idlich, int idDonvi);
        bool Xeplich_byPhong(List<int> idphong, int idlich, int idDonvi);
        bool HuyXep(int id);
    }
}
