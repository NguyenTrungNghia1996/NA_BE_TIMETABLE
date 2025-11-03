using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IPhanphoi_ChuongtrinhRepository
    {
        List<Phanphoi_Chuongtrinh_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord);
        Phanphoi_Chuongtrinh GetDetailById(int Id);
        bool Add(Phanphoi_Chuongtrinh ppct);
        bool Update(Phanphoi_Chuongtrinh ppct);
        bool Delete(int id);
        public bool CheckId(int Id, int idDonvi);
    }
}
