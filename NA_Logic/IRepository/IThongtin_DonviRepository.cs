using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IThongtin_DonviRepository
    {
        List<Ca_DonviDto> GetlistCabyDonvi(int id);
        bool Update_ThongTinDv(DM_Donvi dm_donvi);
        bool UpdateCa(int Id, List<Ca_Donvi> caId);

    }
}
