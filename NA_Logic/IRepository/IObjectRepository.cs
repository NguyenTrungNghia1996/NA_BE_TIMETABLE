using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IObjectRepository
    {
        //Object_Giaovien Object_giaovien(int idgv, int idtkb);
        //Object_Monhoc Object_monhoc(int idmon, int idDonvi);
        //Object_Phonghoc Object_phonghoc(int idph, int idtkb);
        //List<Object_Tohopmon> Object_tohopmon(int idmon,int idlop, int idDonvi);
        //Object_Lophoc Object_lophoc(int idlop, int idtkb);
        //Object_MonKhoi Object_monkhoi(int idmon, int idlop, int idDonvi);
        //bool Check_to_hop_mon(int Ngay, int Tiet, int iddonvi, int idmon, int idlop, int idgv, int id_tkb);
        void LoadObjectsFromTiet_Test(int idTkb, int idDonvi);
        bool ProcessThoiKhoaBieu(int idtkb, int idDonvi);
        ObjectTiet_theoLopDto GetTkbByLop(int id_lop, int idtkb);
        ObjectTiet_theoGVDto GetTkbByGiaovien(int id_gv, int idtkb);
        ObjectTiet_theoLopDto TimViTriXepDuoc_byLop(ObjectTiet_theoLopDto tietDachon, int idDonvi);
        ObjectTiet_theoGVDto TimViTriXepDuoc_byGV(ObjectTiet_theoGVDto tietDachon, int idDonvi);
        (bool success, ObjectTiet_theoLopDto result) DoiChoHaiTiet_Lop(ObjectTiet_theoLopDto tietDachon, int idDonvi);
        (bool success, ObjectTiet_theoGVDto result) DoiChoHaiTiet_GV(ObjectTiet_theoGVDto tietDachon, int idDonvi);
        List<Object_Tiet> TimTietXepDuoc_byLop(ObjectTiet_theoLopDto tietDachon, int idDonvi);
        List<Object_Tiet> TimTietXepDuoc_byGV(ObjectTiet_theoGVDto tietDachon, int idDonvi);
        bool KhoaTiet(int id);
        bool HuyKhoa(int id);
        bool HuyXep(int id);
        bool UpdateTietChuaXep(Object_Tiet tietDachon, int idDonvi);
        bool Xeptkb_byMon(List<int> idmon, int idtkb, int idDonvi);
        bool Xeptkb_byGiaovien(List<int> idgv, int idtkb, int idDonvi);
        bool Xeptkb_byLop(List<int> idlop, int idtkb, int idDonvi);
        bool Xeptkb_byPhong(List<int> idphong, int idtkb, int idDonvi);
        bool Xeptkb_byPhongCN(int idtkb, int idDonvi);
        bool Xeptkb_byGVCN( int idtkb, int idDonvi);
        bool Xeptkb_byLopMon(List<Sotiet_LopMonDto> dsLopMon, int idtkb, int idDonvi);
    }
}
