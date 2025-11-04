using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IPhanphoi_Chuongtrinh_ChitietRepository
    {
        List<Phanphoi_Chuongtrinh_Chitiet_List> GetList_Paging(int PageIndex, int PageSize, string search, int idPpct, ref int totalrecord);
        Phanphoi_Chuongtrinh_Chitiet GetDetailById(int Id);
        bool Add(Phanphoi_Chuongtrinh_Chitiet ppct);
        bool Update(Phanphoi_Chuongtrinh_Chitiet ppct);
        bool Delete(int id);
        public bool CheckId(int Id, int idDonvi);
        bool Import(int idppct, Stream file);
    }
}
