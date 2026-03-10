using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface ISoTietDanhMucRepository
    {
        List<Sotiet_Mon> GetSotiet_Mon(int idDonvi, int idTkb);
        List<Sotiet_Lop> GetSotiet_Lop(int idDonvi, int idTkb, int idLich);
        List<Sotiet_Phong> GetSotiet_Phong(int idDonvi, int idTkb, int idLich);
        List<Sotiet_Giaovien> GetSotiet_Giaovien(int idDonvi, int idTkb, int idLich);
        List<Sotiet_LopMonDto> GetSotiet_LopMon(int idDonvi, int idTkb);
    }
}
