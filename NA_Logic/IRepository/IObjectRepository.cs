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
        Object_Giaovien Object_giaovien(int idgv, int idtkb);
        Object_Monhoc Object_monhoc(int idmon, int idDonvi);
        Object_Phonghoc Object_phonghoc(int idph, int idtkb);
        Object_Tohopmon Object_tohopmon(int idthm, int idDonvi);
        Object_Lophoc Object_lophoc(int idlop, int idtkb);
        Object_MonKhoi Object_monkhoi(int idmon, int idlop, int idDonvi);
        bool Check_gv(int Ngay, int Tiet, int Ca, int id_giaovien, int id_tkb);

    }
}
