using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.InkML;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Phanphoi_Chuongtrinh_ChitietRepository : IPhanphoi_Chuongtrinh_ChitietRepository
    {
        private readonly NA_DbContext _dbContext;
        public Phanphoi_Chuongtrinh_ChitietRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Phanphoi_Chuongtrinh_Chitiet_List> GetList_Paging(int PageIndex, int PageSize, string search, int idPpct, ref int totalrecord)
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
                var paramIdppct = new SqlParameter("Id_ppct", SqlDbType.Int)
                {
                    Value = idPpct
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _dbContext.Set<Phanphoi_Chuongtrinh_Chitiet_List>().FromSqlRaw("EXEC Phanphoi_Chuongtrinh_Chitiet_GetList_Paging @pageIndex, @pageSize, @search, @Id_ppct,  @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramIdppct, paramTotal)
                    .ToList();
                if (result == null) result = new List<Phanphoi_Chuongtrinh_Chitiet_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Phanphoi_Chuongtrinh_Chitiet GetDetailById(int Id)
        {
            try
            {
                var namhoc = _dbContext.Phanphoi_Chuongtrinh_Chitiet.FirstOrDefault(c => c.Id == Id);
                return namhoc;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(Phanphoi_Chuongtrinh_Chitiet ppct_ct)
        {
            try
            {
                _dbContext.Phanphoi_Chuongtrinh_Chitiet.Add(ppct_ct);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(Phanphoi_Chuongtrinh_Chitiet ppct_ct)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.Phanphoi_Chuongtrinh_Chitiet.Update(ppct_ct);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(int id)
        {
            try
            {
                var namhoc = _dbContext.Phanphoi_Chuongtrinh_Chitiet.Find(id);
                if (namhoc == null)
                {
                    return false;
                }
                _dbContext.Phanphoi_Chuongtrinh_Chitiet.Remove(namhoc);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //public bool Check_constraint(int Id)
        //{
        //    try
        //    {

        //        return _dbContext.Database.SqlQueryRaw<int>($@"
        //                  select 1 as Value from Ca_Donvi where Id_ca_hoc = {Id}
        //                  union select 1 from Chitiet_Thoikhoabieu where Id_ca = {Id} 
        //                  union select 1 from DM_Lophoc where Id_ca = {Id}
        //                  union select 1 from Giaovien_Tiettranhxep  where Id_ca = {Id}
        //                  union select 1 from Lophoc_Monhoc_Tiettranhxep  where Id_ca = {Id}
        //                  union select 1 from Lophoc_Tietnghi  where Id_ca = {Id}
        //                  union select 1 from Monhoc_Khoilop  where Id_ca = {Id}
        //                  union select 1 from Monhoc_Khoilop_Tiettranhxep  where Id_ca = {Id}
        //                  union select 1 from Tiet_ban  where Id_ca = {Id}
        //                  union select 1 from Tiet_tranh_xep  where Id_ca = {Id}
        //                  union select 1 from Tiet_co_dinh  where Id_ca = {Id}").Any();
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        public bool CheckId(int Id, int idDonvi)
        {
            if (Id <= 0) return false;
            try
            {
                return _dbContext.Phanphoi_Chuongtrinh.Join(_dbContext.Phanphoi_Chuongtrinh_Chitiet, ppct=>ppct.Id, ppctct=>ppctct.Id_ppct, (ppct, ppctct) => new{ ppct.Id_don_vi, ppctct.Id})
                        .Any(c => c.Id == Id && c.Id_don_vi==idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.Phanphoi_Chuongtrinh_Chitiet.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
        public bool CheckTrungTuanTiet(Phanphoi_Chuongtrinh_Chitiet ppctct, int idDonvi)
        {
            try
            {

                bool check = (from pct in _dbContext.Phanphoi_Chuongtrinh_Chitiet
                              join p in _dbContext.Phanphoi_Chuongtrinh on pct.Id_ppct equals p.Id
                              join m in _dbContext.Dm_Monhoc on p.Id_mon equals m.Id
                              where pct.Tuan == ppctct.Tuan
                                  && pct.Thu_tu_tiet == ppctct.Thu_tu_tiet
                                  && pct.Id_ppct == ppctct.Id_ppct
                                  && m.Id_don_vi == idDonvi
                                  && (ppctct.Id <= 0 || pct.Id != ppctct.Id)
                              select pct).Any();

                if (check) return true;

                return false;
            }
            catch { return true; }
        }
        public (bool result, string mess) Import (int idppct, Stream file, int idDonvi)
        {
            if (idppct == 0) return (false,"Id không hợp lệ");
            try
            {
                var listPPCT = new List<Phanphoi_Chuongtrinh_Chitiet>();

                using var workbook = new XLWorkbook(file);
                var worksheet = workbook.Worksheet(1);
                var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;
                // 1. Kiểm tra file có dữ liệu không
                if (lastRow < 2)
                {
                    return (false, "File Excel không có dữ liệu");
                }

                // 2. Kiểm tra file đúng định dạng không
                var headers = new[] { "Tuần", "Tiết", "Phân môn", "Tên bài học" };
                for (int col = 1; col <= 4; col++)
                {
                    var headerValue = worksheet.Cell(1, col).Value.ToString()?.Trim();
                    if (string.IsNullOrEmpty(headerValue) ||
                        !headerValue.Equals(headers[col - 1], StringComparison.OrdinalIgnoreCase))
                    {
                        return (false, $"Cột {col} không đúng định dạng. Cần: {headers[col - 1]}");
                    }
                }
                //đọc file
                for (int row = 2; row <= lastRow; row++)
                {
                    var tuan = worksheet.Cell(row, 1).Value;
                    var tiet = worksheet.Cell(row, 2).Value;
                    var phanMon = worksheet.Cell(row, 3).Value;
                    var tenBai = worksheet.Cell(row, 4).Value;

                    if (tuan.IsBlank && tiet.IsBlank &&
                        phanMon.IsBlank && tenBai.IsBlank)
                        continue;

                    var itemPPCT = new Phanphoi_Chuongtrinh_Chitiet
                    {
                        Id_ppct = idppct,
                        Tuan = tuan.IsBlank ? 0 :
                               tuan.IsNumber ? (int)tuan.GetNumber() :
                               int.TryParse(tuan.ToString(), out int t) ? t : 0,
                        Thu_tu_tiet = tiet.IsBlank ? 0 :
                               tiet.IsNumber ? (int)tiet.GetNumber() :
                               int.TryParse(tiet.ToString(), out int ti) ? ti : 0,
                        Phan_mon = phanMon.ToString() ?? "",
                        Ten_bai = tenBai.ToString() ?? ""
                    };
                    if (itemPPCT.Tuan <= 0)
                    {
                        return (false, $"Dòng {row}: Tuần phải là số nguyên dương");
                    }

                    if (itemPPCT.Thu_tu_tiet <= 0)
                    {
                        return (false, $"Dòng {row}: Tiết phải là số nguyên dương");
                    }
                    
                    listPPCT.Add(itemPPCT);
                }
                //xoá dữ liệu cũ
                var ppctOld = _dbContext.Phanphoi_Chuongtrinh_Chitiet.Where(c => c.Id_ppct == idppct).ToList();
                _dbContext.BulkDelete(ppctOld);
                //thêm dữ liệu
                _dbContext.BulkInsert(listPPCT);

                return (true,"Import thành công");
            }
            catch
            {
                return (false, "Import thất bại");
            }
        }
    }
}
