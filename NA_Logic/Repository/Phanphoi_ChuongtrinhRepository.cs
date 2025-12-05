using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Phanphoi_ChuongtrinhRepository : IPhanphoi_ChuongtrinhRepository
    {
        private readonly NA_DbContext _dbContext;
        public Phanphoi_ChuongtrinhRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Phanphoi_Chuongtrinh_List> GetList_Paging(int PageIndex, int PageSize,int IdBan, int IdKhoi, int IdMon, int IdNam, int IdDonvi, string search, ref int totalrecord)
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
                var paramIdBan = new SqlParameter("Id_ban", SqlDbType.Int)
                {
                    Value = IdBan
                };
                var paramIdKhoi = new SqlParameter("Id_khoi", SqlDbType.Int)
                {
                    Value = IdKhoi
                };
                var paramIdMon = new SqlParameter("Id_mon", SqlDbType.Int)
                {
                    Value = IdMon
                };
                var paramIdNam = new SqlParameter("Id_nam", SqlDbType.Int)
                {
                    Value = IdNam
                };
                var paramIdDonvi = new SqlParameter("Id_don_vi", SqlDbType.Int)
                {
                    Value = IdDonvi
                };
                var paramSearch = new SqlParameter("search", SqlDbType.NVarChar)
                {
                    Value = search ?? string.Empty
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _dbContext.Set<Phanphoi_Chuongtrinh_List>().FromSqlRaw("EXEC Phanphoi_Chuongtrinh_GetList_Paging @pageIndex, @pageSize, @Id_khoi, @Id_ban," +
                                                                                    "@Id_nam, @Id_mon, @Id_don_vi, @search,  @total OUTPUT",
                    paramPageIndex, paramPageSize, paramIdKhoi, paramIdBan, paramIdNam, paramIdMon, paramIdDonvi, paramSearch, paramTotal)
                    .ToList();
                if (result == null) result = new List<Phanphoi_Chuongtrinh_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Phanphoi_Chuongtrinh GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var namhoc = _dbContext.Phanphoi_Chuongtrinh.FirstOrDefault(c => c.Id == Id && c.Id_don_vi == idDonvi);
                return namhoc;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(Phanphoi_Chuongtrinh ppct)
        {
            try
            {
                _dbContext.Phanphoi_Chuongtrinh.Add(ppct);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(Phanphoi_Chuongtrinh ppct)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.Phanphoi_Chuongtrinh.Update(ppct);
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
                var namhoc = _dbContext.Phanphoi_Chuongtrinh.Find(id);
                if (namhoc == null)
                {
                    return false;
                }
                _dbContext.Phanphoi_Chuongtrinh.Remove(namhoc);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public byte[] ExportExcel_PPCT(int id, int idDonvi)
        {
            var data = (from ct in _dbContext.Phanphoi_Chuongtrinh_Chitiet
                        join pp in _dbContext.Phanphoi_Chuongtrinh on ct.Id_ppct equals pp.Id
                        where ct.Id_ppct == id && pp.Id_don_vi == idDonvi
                        select ct).ToList();

            if (!data.Any()) return null;

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet1");

            // Headers
            var headers = new[] { "Tuần", "Tiết", "Phân môn", "Tên bài học" };
            for (int i = 0; i < headers.Length; i++)
            {
                var headerCell = worksheet.Cell(1, i + 1);
                headerCell.Value = headers[i];
                headerCell.Style.Font.SetBold(true);
                headerCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                headerCell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
            }

            // Đổ dữ liệu
            int currentRow = 2;

            foreach (var ct in data)
            {
                worksheet.Cell(currentRow, 1).Value = ct.Tuan;
                worksheet.Cell(currentRow, 2).Value = ct.Thu_tu_tiet;
                worksheet.Cell(currentRow, 3).Value = ct.Phan_mon;
                worksheet.Cell(currentRow, 4).Value = ct.Ten_bai;

                // căn giữa cột số
                worksheet.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                currentRow++;
            }

            // Tự động căn chỉnh width
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
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
                return _dbContext.Phanphoi_Chuongtrinh.Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.Phanphoi_Chuongtrinh.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
        public bool CheckTrung(Phanphoi_Chuongtrinh ppct)
        {
            try
            {

                bool check = (from p in _dbContext.Phanphoi_Chuongtrinh
                              join m in _dbContext.Dm_Monhoc on p.Id_mon equals m.Id
                              where p.Id_mon == ppct.Id_mon
                                  && p.Id_khoi == ppct.Id_khoi
                                  && p.Id_ban == ppct.Id_ban
                                  && p.Id_nam_hoc == ppct.Id_nam_hoc
                                  && m.Id_don_vi == ppct.Id_don_vi
                                  && (ppct.Id <= 0 || p.Id != ppct.Id)
                              select p).Any();
                if (check) return true;

                return false;
            }
            catch { return true; }
        }
    }
}
