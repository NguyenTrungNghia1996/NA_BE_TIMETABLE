using Microsoft.AspNetCore.Http;
using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository.LichOnTap
{
    public interface IHocsinh_TohopmonRepository
    {
        Hocsinh_Tohopmon_Multi GetToHopByIdHocSinh(int idHocSinh, int idDonvi);
        bool CheckTrung(Hocsinh_Tohopmon hs, int idDonvi);
        bool Add(Hocsinh_Tohopmon_Multi data);
        bool DeleteByHocSinh(int Id);
        byte[] ExportMauExcel(int idlop);
        (bool success, string mess) Import(IFormFile file, int idDonvi);
        bool CheckId(int Id, int idDonvi);
        bool CheckIds(IEnumerable<int> ids);
    }
}
