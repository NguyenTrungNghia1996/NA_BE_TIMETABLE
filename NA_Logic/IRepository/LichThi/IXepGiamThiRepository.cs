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
        bool XepMotGiamThi(int idLich, int idGiamThi);
        bool HuyKetQua(int idLich);
    }
}
