using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository;
using NA_Logic.IRepository.LichThi;
using NA_Logic.IRepository.XepGiamThi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class DM_GiamthiRepository : IDM_GiamthiRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_GiamthiRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Giamthi_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi)
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

                var result = _dbContext.Set<DM_Giamthi_List>().FromSqlRaw("EXEC [DM_Giamthi_GetList_Paging] @pageIndex, @pageSize, @search, @idDonvi",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi).ToList();
                if (result == null) result = new List<DM_Giamthi_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Giamthi GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var idDiemthiByDonvi = _dbContext.DM_Diemthi.Where(c => c.Id_don_vi == idDonvi).Select(c => c.Id).ToList();

                var idHoidong = _dbContext.DM_Hoidongthi.Where(c => c.Id_don_vi == idDonvi).Select(c => c.Id).ToList();

                var idDiemthiByHoidong = _dbContext.DM_Diemthi.Where(c => idHoidong.Contains(c.Id_hoi_dong)).Select(c => c.Id).ToList();

                var idDiemthi = idDiemthiByDonvi.Union(idDiemthiByHoidong).ToList();

                var giamthi = _dbContext.DM_Giamthi
                    .FirstOrDefault(c => c.Id == Id && idDiemthi.Contains(c.Id_diem_thi));

                return giamthi;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Giamthi_Multi GetGiamThiByDiemThi(int IdDiemThi)
        {
            try
            {
                var gt = _dbContext.DM_Diemthi
                    .Where(l => l.Id == IdDiemThi)
                    .Select(l => new DM_Giamthi_Multi
                    {
                        Id_diem_thi = l.Id,
                        Id_giao_vien = _dbContext.DM_Giamthi.Where(hl => hl.Id_diem_thi == IdDiemThi).Select(hl => hl.Id_giao_vien).ToList()
                    })
                    .FirstOrDefault();

                return gt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool Add(DM_Giamthi gt)
        {
            try
            {
                _dbContext.DM_Giamthi.Add(gt);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddList(DM_Giamthi_Multi data)
        {
            using var tranc = _dbContext.Database.BeginTransaction();
            try
            {
                var listOld = _dbContext.DM_Giamthi.Where(c => c.Id_diem_thi == data.Id_diem_thi && c.Id_giao_vien.HasValue).ToList();

                if (listOld.Any())
                    _dbContext.BulkDelete(listOld);

                var giaoVienList = _dbContext.DM_Giaovien.Where(m => data.Id_giao_vien.Contains(m.Id)).ToList();

                var list = data.Id_giao_vien.Select(c => {
                    var giaovien = giaoVienList.FirstOrDefault(m => m.Id == c);
                    return new DM_Giamthi
                    {
                        Id_diem_thi = data.Id_diem_thi,
                        Id_giao_vien = c,
                        Ma = giaovien?.Ma_giao_vien,
                        Ho_va_ten = (giaovien?.Ho_va_ho_dem + " " + giaovien?.Ten )
                    };
                }).ToList();

                _dbContext.BulkInsert(list);
                tranc.Commit();
                return true;
            }
            catch (Exception)
            {
                tranc.Rollback();
                return false;
            }
        }
        public bool Update(DM_Giamthi gt)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Giamthi.Update(gt);
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
                var gt = _dbContext.DM_Giamthi.FirstOrDefault(c => c.Id == id );
                if (gt == null)
                {
                    return false;
                }
                _dbContext.DM_Giamthi.Remove(gt);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckMa(string Ma, int idDiemThi, int? Id)
        {
            try
            {
                var query = _dbContext.DM_Giamthi.Where(c => c.Ma == Ma && c.Id_diem_thi == idDiemThi);

                if (Id.HasValue)
                {
                    query = query.Where(c => c.Id != Id.Value);
                }

                var check = query.Any();
                return check;
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

        //        return _dbContext.Database.SqlQuery<int>($@"
        //                  select 1 as Value from Phanphoi_Chuongtrinh where Id_nam_hoc = {Id}
        //                  union select 1 from Lich_Baogiang where Id_nam_hoc = {Id}").Any();
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        public bool CheckId(int Id, int idDiemThi)
        {
            if (Id <= 0) return false;
            try
            {
                return _dbContext.DM_Giamthi.Any(c => c.Id == Id && c.Id_diem_thi == idDiemThi);
            }
            catch
            {
                return false;
            }
        }
        //public bool CheckIds(IEnumerable<int> ids)
        //{
        //    var existingIds = _dbContext.DM_Giamthi.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
        //    return ids.All(id => existingIds.Contains(id));
        //}
        public (bool success, string mess) Import(IFormFile file, int idDonvi)
        {
            try
            {

                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                var rows = worksheet.RangeUsed().RowsUsed().Skip(1).ToList();

                if (!rows.Any())
                    return (false, "File không có dữ liệu");
                var headers = new[] { "STT", "Mã điểm thi", "Mã giám thị", "Tên giám thị" };
                for (int col = 1; col <= 4; col++)
                {
                    var headerValue = worksheet.Cell(1, col).Value.ToString()?.Trim();
                    if (string.IsNullOrEmpty(headerValue) ||
                        !headerValue.Equals(headers[col - 1], StringComparison.OrdinalIgnoreCase))
                    {
                        return (false, $"File không đúng định dạng");
                    }
                }

                int rowNumber = 2;
                var listTrung = new HashSet<string>();
                foreach (var row in rows)
                {
                    var maDiemThi = row.Cell(2).GetValue<string>()?.Trim();
                    var maGiamThi = row.Cell(3).GetValue<string>()?.Trim();
                    var tenGiamThi = row.Cell(4).GetValue<string>()?.Trim();

                    if (string.IsNullOrEmpty(maDiemThi))
                        return (false, "Mã điểm thi không được để trống");

                    if (string.IsNullOrEmpty(maGiamThi))
                        return (false, "Mã giám thị không được để trống");

                    if (string.IsNullOrEmpty(tenGiamThi))
                        return (false, "Tên giám thị không được để trống");

                    var cum = $"{maDiemThi}_{maGiamThi}";
                    if (!listTrung.Add(cum))
                        return (false, $"Điểm thi {maDiemThi} - Giám thị {maGiamThi} bị trùng trong file Excel");

                    rowNumber++;
                }
                int stt = 1;
                var dataTable = new DataTable();
                dataTable.Columns.Add("STT", typeof(int));
                dataTable.Columns.Add("Ma_diem_thi", typeof(string));
                dataTable.Columns.Add("Ma_giam_thi", typeof(string));
                dataTable.Columns.Add("Ten_giam_thi", typeof(string));

                foreach (var row in rows)
                {
                    dataTable.Rows.Add(
                        stt,
                        row.Cell(2).GetValue<string>()?.Trim(),
                        row.Cell(3).GetValue<string>()?.Trim(),
                        row.Cell(4).GetValue<string>()?.Trim()
                    );
                    stt++;
                }

                var paramIdDonvi = new SqlParameter("Id_don_vi", SqlDbType.Int) { Value = idDonvi };
                var dataParam = new SqlParameter("@Data", SqlDbType.Structured)
                {
                    TypeName = "dbo.DMGiamThi",
                    Value = dataTable
                };
                var paramMessage = new SqlParameter("Message", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };
                var results = _dbContext.Database.ExecuteSqlRaw("EXEC [ImportGiamThi] @Id_don_vi, @Data, @Message OUTPUT ", paramIdDonvi, dataParam, paramMessage);
                var Message = paramMessage.Value?.ToString() ?? "";

                if (Message == "Thành công")
                    return (true, Message);
                return (false, Message);
            }
            catch (Exception)
            {
                return (false, "Có lỗi hệ thống");
            }
        }
    }
}
