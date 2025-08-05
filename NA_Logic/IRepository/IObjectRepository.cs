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

    }
}
