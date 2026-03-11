using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository.LichOnTap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Danhsach_LichontapRepository : IDanhsach_LichontapRepository
    {
        private readonly NA_DbContext _context;
        public Danhsach_LichontapRepository(NA_DbContext context)
        {
            _context = context;
        }
        public List<Danhsach_Lichontap_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, ref int totalrecord)
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
                var result = _context.Set<Danhsach_Lichontap_List>().FromSqlRaw("EXEC [DS_Lichontap_GetList_Paging] @pageIndex, @pageSize, @search, @idDonvi, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi, paramTotal)
                    .ToList();
                if (result == null) result = new List<Danhsach_Lichontap_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Lichontap_Detail GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var lichon = _context.Danhsach_Lichontap.FirstOrDefault(c => c.Id == Id && c.Id_don_vi == idDonvi);
                var tiet = _context.Chitiet_Lichontap.Where(c => c.Id_lich == Id).ToList();
                Lichontap_Detail tkb_detail = new Lichontap_Detail()
                {
                    Id = lichon.Id,
                    Ten = lichon.Ten,
                    Trang_thai = lichon.Trang_thai,
                    Id_don_vi = idDonvi,
                    Tong_tat_ca_tiet = tiet.Count(),
                    Tong_tiet_da_xep = tiet.Where(c => c.Ngay > 0 && c.Tiet > 0 && c.Id_lich == Id).Count(),
                    Tong_tiet_chua_xep = tiet.Where(c => c.Ngay <= 0 && c.Id_lich == Id).Count()
                };
                return tkb_detail;
            }
            catch (Exception)
            {
                return new Lichontap_Detail();
            }
        }

        public List<Chitiet_Lichontap> GetDetailLich(int Id)
        {
            try
            {
                var list = _context.Chitiet_Lichontap.Where(x => x.Id_lich == Id).ToList();
                if (list == null)
                    return new List<Chitiet_Lichontap>();
                return list;
            }
            catch
            {
                return new List<Chitiet_Lichontap>();
            }
        }
        public bool Add(Danhsach_Lichontap lichon)
        {
            try
            {
                _context.Danhsach_Lichontap.Add(lichon);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddChiTiet(int Id, int iddonvi)
        {
            try
            {
                var paramIdLich = new SqlParameter("@IdLich", SqlDbType.Int) { Value = Id };
                var paramIdDonvi = new SqlParameter("@IdDonvi", SqlDbType.Int) { Value = iddonvi };

                var paramResult = new SqlParameter("@Result", SqlDbType.Bit) { Direction = ParameterDirection.Output };

                _context.Database.ExecuteSqlRaw("EXEC [InsertChitietTKBOnTap] @IdLich, @IdDonvi, @Result OUTPUT", paramIdLich, paramIdDonvi, paramResult);
                return Convert.ToBoolean(paramResult.Value);
            }
            catch (Exception)
            {
                return false;
            }
        }
        //public bool Copy_Tkb(int id_tkb_nguon, int id_tkb_dich)
        //{
        //    try
        //    {
        //        var paramIdTkbNguon = new SqlParameter("id_tkb_nguon", SqlDbType.Int) { Value = id_tkb_nguon };
        //        var paramIdTkbDich = new SqlParameter("id_tkb_dich", SqlDbType.Int) { Value = id_tkb_dich };
        //        var paramResult = new SqlParameter("result", SqlDbType.Bit) { Direction = ParameterDirection.Output };
        //        _context.Database.ExecuteSqlRaw("EXEC Copy_Tkb @id_tkb_nguon, @id_tkb_dich, @result output", paramIdTkbNguon, paramIdTkbDich, paramResult);
        //        return Convert.ToBoolean(paramResult.Value);
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        //public bool Sync_Tkb(int idtkb, int idDonvi)
        //{
        //    try
        //    {
        //        var paramIdTkb = new SqlParameter("IdTKB", SqlDbType.Int) { Value = idtkb };
        //        var paramIdDonvi = new SqlParameter("IdDonvi", SqlDbType.Int) { Value = idDonvi };
        //        var paramMessage = new SqlParameter("Message", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };

        //        _context.Database.ExecuteSqlRaw("EXEC [Sync_TKB] @IdTKB, @IdDonvi, @Message OUTPUT", paramIdTkb, paramIdDonvi, paramMessage);

        //        var Message = paramMessage.Value?.ToString() ?? "";
        //        return Message == "success";
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        public bool Update(Danhsach_Lichontap lichon)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.Danhsach_Lichontap.Update(lichon);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        //public bool Update_TrangThaiXep(int idtkb)
        //{
        //    try
        //    {
        //        _context.ChangeTracker.Clear();
        //        var lichon = _context.Danhsach_Lichontap.Find(idtkb);
        //        lichon.Trang_thai = true;
        //        _context.Danhsach_Lichontap.Update(lichon);
        //        _context.SaveChanges();
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        public bool Huy_KQ(int Id)
        {
            try
            {
                var del = _context.Chitiet_Lichontap.Where(x => x.Id_lich == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    foreach (var item in del)
                    {
                        item.Id_ca = 0;
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
                    var allRecords = _context.Danhsach_Lichontap
                        .Where(x => x.Id_don_vi == idDonVi)
                        .ToList();

                    foreach (var record in allRecords)
                    {
                        record.Trang_thai = false;
                    }

                    // Đặt bản ghi được chọn thành true
                    var activeRecord = _context.Danhsach_Lichontap
                        .FirstOrDefault(x => x.Id == id);

                    if (activeRecord != null)
                    {
                        activeRecord.Trang_thai = true;
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
        public bool Delete(int Id)
        {
            try
            {
                Danhsach_Lichontap lichon = new Danhsach_Lichontap();
                lichon = _context.Danhsach_Lichontap.FirstOrDefault(c => c.Id == Id);
                if (lichon != null)
                {
                    _context.Danhsach_Lichontap.Remove(lichon);
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
                var del = _context.Chitiet_Lichontap.Where(x => x.Id_lich == Id).ToList();
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
                return _context.Danhsach_Lichontap.Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckId_ChiTiet(int Id, int idDonvi)
        {
            if (Id <= 0) return false;
            try
            {
                bool ct = _context.Chitiet_Lichontap.Join(_context.Danhsach_Lichontap, c => c.Id_lich, d => d.Id, (c, d) => new { Id = c.Id, Id_donvi = d.Id_don_vi }).Any(x => x.Id == Id && x.Id_donvi == idDonvi);
                return ct;
            }
            catch
            {
                return false;
            }
        }
        //public bool Check_constraint(int Id)
        //{
        //    try
        //    {

        //        return _context.Database.SqlQueryRaw<int>($@"
        //                  select 1 as Value from Lich_Baogiang where Id_tkb = {Id}").Any();
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
    }
}
