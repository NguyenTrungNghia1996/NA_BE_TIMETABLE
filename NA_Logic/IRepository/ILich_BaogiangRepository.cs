using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface ILich_BaogiangRepository
    {
        List<Lich_Baogiang_List> GetList_Paging(int PageIndex, int PageSize,int IdNam, int IdDonvi, ref int totalrecord);
        Lich_Baogiang GetDetailById(int Id);
        (bool result, string mess) Add(Lich_Baogiang lbg, int idDonvi);
        (bool result, string mess) Update(Lich_Baogiang lbg, int idDonvi);
        bool Delete(int id);
        public bool CheckId(int Id, int idDonvi);
        //bool CheckTrungTuan(Lich_Baogiang lbg, int idDonvi);
        bool CheckChangeTKB(Lich_Baogiang lbg);
        bool CheckExistPPCT(int idNam, int idDonvi);
    }
}
