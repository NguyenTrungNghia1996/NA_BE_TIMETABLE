using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IThongtin_LienheRepository
    {
        bool Add(Thongtin_Lienhe thongtin);
        bool SendMail(Thongtin_Lienhe thongtin);
    }
}
