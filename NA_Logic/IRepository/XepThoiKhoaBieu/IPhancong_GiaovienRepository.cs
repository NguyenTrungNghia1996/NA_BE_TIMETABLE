using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.XepThoiKhoaBieu
{
    public interface IPhancong_GiaovienRepository
    {
        List<PhancongGVDto> GetList_Paging(int idgv, int idDonvi, int type);
        List<DsLop_ByGVandMon> GetList_Lop_ByGvAndMon(int idgv, int idDonvi, int idMon);
        bool Add(List<PhancongGVDto> phancongList);
        byte[] Export(int idDonvi);
    }
}
