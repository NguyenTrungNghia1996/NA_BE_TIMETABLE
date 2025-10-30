using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDanhsach_ThoikhoabieuRepository
    {
        List<Danhsach_ThoikhoabieuList> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, ref int totalrecord);
        ThoiKhoaBieu_Detail GetDetailById(int Id, int idDonvi);
        List<Chitiet_Thoikhoabieu> GetDetailTKB(int Id);
        bool Add(Danhsach_Thoikhoabieu ds_thoikhoabieu);
        bool AddChitiet_Tkb(int Id, int iddonvi);
        bool Copy_Tkb(int id_tkb_nguon, int id_tkb_dich);
        bool Sync_Tkb(int idtkb, int idDonvi);
        bool Update(Danhsach_Thoikhoabieu danhsach_Thoikhoabieu);
        bool Update_TrangThaiXep(int idtkb);
        bool SetStatus(int id, int idDonVi);
        bool Delete(int Id);
        bool DeleteDetail(int Id);
        bool CheckId(int Id, int idDonvi);
        bool Huy_KQ(int Id);
        bool CheckId_ChiTiet(int Id, int idDonvi);
    }
}
