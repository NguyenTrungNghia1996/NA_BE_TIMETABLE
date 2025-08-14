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
        bool ProcessThoiKhoaBieu(int idtkb, int idDonvi);
        ObjectTiet_theoLopDto GetTkbByLop(int id_lop, int idtkb);
        ObjectTiet_theoGVDto GetTkbByGiaovien(int id_gv, int idtkb);
        ObjectTiet_theoLopDto TimViTriXepDuoc_Lop(ObjectTiet_theoLopDto tietDachon, int idDonvi);
        ObjectTiet_theoGVDto TimViTriXepDuoc_GV(ObjectTiet_theoGVDto tietDachon, int idDonvi);
        ObjectTiet_theoLopDto DoiChoHaiTiet_Lop(ObjectTiet_theoLopDto tietDachon, int idDonvi);
        //ObjectTiet_theoGVDto DoiChoHaiTiet_GV(ObjectTiet_DaChon tiet1, ObjectTiet_DaChon tiet2, int idDonvi);
    }
}
