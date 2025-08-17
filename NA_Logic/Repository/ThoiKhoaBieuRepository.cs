using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class ThoiKhoaBieuRepository : IDanhsach_ThoikhoabieuRepository
    {
        private readonly NA_DbContext _context;
        public ThoiKhoaBieuRepository(NA_DbContext context) { 
            _context = context;        
        }
        public List<Danhsach_ThoikhoabieuList> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, ref int totalrecord)
        {
            try
            {
                var paramPageIndex = new SqlParameter("pageIndex", SqlDbType.Int)
                {
                    Value = PageIndex
                };
                var paramPageSize = new SqlParameter("pageSize", SqlDbType.Int)
                {
                    Value = PageSize
                };
                var paramSearch = new SqlParameter("search", SqlDbType.NVarChar)
                {
                    Value = search ?? string.Empty
                };
                var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int)
                {
                    Value = idDonvi
                };

                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _context.Set<Danhsach_ThoikhoabieuList>().FromSqlRaw("EXEC DS_TKB_GetList_Paging @pageIndex, @pageSize, @search, @idDonvi, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi, paramTotal)
                    .ToList();
                if (result == null) result = new List<Danhsach_ThoikhoabieuList>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public ThoiKhoaBieu_Detail GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var tkb = _context.Danhsach_Thoikhoabieu.FirstOrDefault(c => c.Id == Id && c.Id_don_vi == idDonvi);
                var tiet = _context.Chitiet_Thoikhoabieu.Where(c => c.Id_tkb == Id).ToList();
                ThoiKhoaBieu_Detail tkb_detail = new ThoiKhoaBieu_Detail(){
                    Id = tkb.Id,
                    Ten = tkb.Ten,
                    Dang_su_dung = tkb.Dang_su_dung,
                    Id_don_vi = idDonvi,
                    Trang_thai_xep = tkb.Trang_thai_xep,
                    Tong_tat_ca_tiet = tiet.Count(),
                    Tong_tiet_da_xep = tiet.Where(c => c.Ngay > 0 && c.Tiet > 0).Count(),
                    Tong_tiet_chua_xep = tiet.Where(c => c.Ngay == 0 && c.Tiet == 0).Count()
                };
                return tkb_detail;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public List<Chitiet_Thoikhoabieu> GetDetailTKB(int Id)
        {
            try
            {
                var list = _context.Chitiet_Thoikhoabieu.Where(x => x.Id_tkb == Id).ToList();
                if (list == null) return null;
                return list;
            }
            catch
            {
                return new List<Chitiet_Thoikhoabieu>();
            }
        }
        public bool Add(Danhsach_Thoikhoabieu ds_thoikhoabieu)
        {
            try
            {
                _context.Danhsach_Thoikhoabieu.Add(ds_thoikhoabieu);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddChitiet_tkb(int Id)
        {
            try
            {
                var paramIdTkb = new SqlParameter("@IdTKB", SqlDbType.Int) { Value = Id };

                var paramResult = new SqlParameter("@Result", SqlDbType.Bit) { Direction = ParameterDirection.Output };

                _context.Database.ExecuteSqlRaw("EXEC InsertChitietTKB @IdTKB, @Result OUTPUT", paramIdTkb, paramResult);
                return Convert.ToBoolean(paramResult.Value);
            }
            catch(Exception) 
            {
                return false;
            }
        }
        public bool Update(Danhsach_Thoikhoabieu danhsach_Thoikhoabieu)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.Danhsach_Thoikhoabieu.Update(danhsach_Thoikhoabieu);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Huy_KQ(int Id)
        {
            try
            {
                var del = _context.Chitiet_Thoikhoabieu.Where(x => x.Id_tkb == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    foreach (var item in del)
                    {
                        item.Ngay = 0;
                        item.Tiet = 0; 
                    }
                    _context.BulkUpdate(del);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool SetStatus(int id, int idDonVi)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Đặt tất cả về false cho cùng đơn vị
                    var allRecords = _context.Danhsach_Thoikhoabieu
                        .Where(x => x.Id_don_vi == idDonVi)
                        .ToList();

                    foreach (var record in allRecords)
                    {
                        record.Dang_su_dung = false;
                    }

                    // Đặt bản ghi được chọn thành true
                    var activeRecord = _context.Danhsach_Thoikhoabieu
                        .FirstOrDefault(x => x.Id == id);

                    if (activeRecord != null)
                    {
                        activeRecord.Dang_su_dung = true;
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                    return false;
                }
            }
        }
        public bool Delete(int Id)
        {
            try
            {
                Danhsach_Thoikhoabieu dstkb = new Danhsach_Thoikhoabieu();
                dstkb = _context.Danhsach_Thoikhoabieu.FirstOrDefault(c => c.Id == Id);
                if (dstkb != null)
                {
                    _context.Danhsach_Thoikhoabieu.Remove(dstkb);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteDetail(int Id)
        {
            try
            {
                var del = _context.Chitiet_Thoikhoabieu.Where(x => x.Id_tkb == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.BulkDelete(del);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckId(int Id, int idDonvi)
        {
            if (Id <= 0) return false;
            try
            {
                return _context.Danhsach_Thoikhoabieu.Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
    }
}
