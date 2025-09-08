using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository;

namespace NA_Logic.Repository
{
    public class DM_LoaiphonghocRepository : IDM_LoaiphonghocRepository
    {
        private readonly NA_DbContext _context;
        public DM_LoaiphonghocRepository(NA_DbContext context)
        {
            _context = context;
        }
        public List<DM_Loaiphonghoc_List> GetList_Paging(int PageIndex, int PageSize, string search,  ref int totalrecord)
        {
            try
            {
                var paramSearch = new SqlParameter("search", SqlDbType.NVarChar) { Value = search ?? string.Empty };
                var paramPageIndex = new SqlParameter("pageIndex", SqlDbType.Int) { Value = PageIndex };
                var paramPageSize = new SqlParameter("pageSize", SqlDbType.Int) { Value = PageSize };
                var paramTotal = new SqlParameter("total", SqlDbType.Int) { Direction = ParameterDirection.Output };

                var result = _context.Set<DM_Loaiphonghoc_List>()
                    .FromSqlRaw("EXEC DM_Loaiphonghoc_GetList_Paging @pageIndex, @pageSize, @search,  @total OUTPUT",
                        paramPageIndex, paramPageSize, paramSearch, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Loaiphonghoc_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Loaiphonghoc GetDetailByID(int Id)
        {
            try
            {
                var loaiph = _context.DM_Loaiphonghoc.FirstOrDefault(c => c.Id == Id);
                return loaiph;
            }
            catch
            {
                return null;
            }
        }
        public bool Add(DM_Loaiphonghoc dM_Loaiphonghoc)
        {
            try
            {
                _context.DM_Loaiphonghoc.Add(dM_Loaiphonghoc);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Update(DM_Loaiphonghoc dM_Loaiphonghoc)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.DM_Loaiphonghoc.Update(dM_Loaiphonghoc);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Deleted(int Id)
        {
            try
            {
                DM_Loaiphonghoc item = new DM_Loaiphonghoc();
                item = _context.DM_Loaiphonghoc.Find(Id);
                if (item == null)
                {
                    return false;
                }
                bool check = _context.Dm_Monhoc.Any(c=>c.Id_loai_phong_hoc == Id)
                           || _context.DM_Phonghoc.Any(p=>p.Id_Loai_phong_hoc== Id);
                if (check)
                {
                    throw new Exception("Loại phòng học đã có ràng buộc, không thể xoá");
                }
                    _context.DM_Loaiphonghoc.Remove(item);
                    _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckId(int Id)
        {
            if (Id <= 0) return false;
            try
            {
                return _context.DM_Loaiphonghoc.Any(c => c.Id == Id);
            }
            catch
            {
                return false;
            }
        }
    }
}
