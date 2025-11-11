using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_DonviRepository
    {
        DM_Donvi getDetailById(int id);
        List<int> GetlistCapbyDonvi(int id);
        List<int> GetlistCabyDonvi(int id);
        List<DM_Donvi> GetDonviChuaCoTaikhoan();
        List<DM_Donvi_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord);
        bool Add(DM_Donvi dm_donvi);
        //bool Add_Demo(DM_Donvi_Demo dm_donvi);
        bool Update(DM_Donvi dm_donvi);
        bool UpdateCap(int Id, List<int> capId);
        bool UpdateCa(int Id, List<int> caId);

         bool Delete(int Id);
        (bool success, string message) DeleteCap(int Id);
        bool DeleteCa(int Id);
        bool AddCap(int Id, List<int> capId);
        bool AddCa(int Id, List<int> caId);
        bool CheckDonviChuaCoTaikhoan(int id);
        bool CheckTrungTen( string ten, int? excludeId = null);
    }
}