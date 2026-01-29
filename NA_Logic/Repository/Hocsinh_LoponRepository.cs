using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Hocsinh_LoponRepository : IHocsinh_LoponRepository
    {
        private readonly NA_DbContext _dbContext;
        public Hocsinh_LoponRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Hocsinh_Lopon_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi)
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

                var result = _dbContext.Set<Hocsinh_Lopon_List>().FromSqlRaw("EXEC [Hocsinh_Lopon_GetList_Paging] @pageIndex, @pageSize, @search,@idDonvi",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi).ToList();
                if (result == null) result = new List<Hocsinh_Lopon_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Hocsinh_Lopon GetDetailById(int Id, int idDonvi)
        {
            try
            {
                Hocsinh_Lopon Lopontap = (from l in _dbContext.DM_Lopontap
                               join hl in _dbContext.Hocsinh_Lopon on l.Id equals hl.Id_lop_on
                                          where hl.Id == Id && l.Id_don_vi == idDonvi
                               select hl).FirstOrDefault();
                return Lopontap;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Hocsinh_Lopon_Multi GetHocSinhByIdLop(int IdLop, int idDonvi)
        {
            try
            {
                var lopOn = _dbContext.DM_Lopontap
                    .Where(l => l.Id == IdLop && l.Id_don_vi == idDonvi)
                    .Select(l => new Hocsinh_Lopon_Multi
                    {
                        Id_lop = l.Id,
                        Hoc_sinh = _dbContext.Hocsinh_Lopon .Where(hl => hl.Id_lop_on == IdLop).Select(hl => hl.Id_hoc_sinh).ToList()
                    })
                    .FirstOrDefault();

                return lopOn;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool CheckTrung(Hocsinh_Lopon hs, int idDonvi)
        {
            try
            {
                bool check = (from hl in _dbContext.Hocsinh_Lopon
                              join l in _dbContext.DM_Lopontap on hl.Id_lop_on equals l.Id
                              where hl.Id_hoc_sinh == hs.Id_hoc_sinh
                                  && hl.Id_lop_on == hs.Id_lop_on
                                  && l.Id_don_vi == idDonvi
                                  && (hs.Id <= 0 || hl.Id != hs.Id)
                              select hl).Any();

                return check;
            }
            catch (Exception ex) {
                return false;
            }
        }
        public bool Add(Hocsinh_Lopon_Multi data)
        {
            using var tranc = _dbContext.Database.BeginTransaction();
            try
            {
                var listOld = _dbContext.Hocsinh_Lopon.Where(c=>c.Id_lop_on==data.Id_lop).ToList();

                if(listOld.Any()) 
                    _dbContext.BulkDelete(listOld);

                var list = data.Hoc_sinh.Select(c=> new Hocsinh_Lopon
                {
                    Id_lop_on = data.Id_lop,
                    Id_hoc_sinh = c
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
        public bool Update(Hocsinh_Lopon hs)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.Hocsinh_Lopon.Update(hs);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(int Id)
        {
            try
            {
                var hs = _dbContext.Hocsinh_Lopon.FirstOrDefault(c => c.Id == Id);
                _dbContext.Hocsinh_Lopon.Remove(hs);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteByLop(int Id)
        {
            try
            {
                var hs = _dbContext.Hocsinh_Lopon.Where(c=>c.Id_lop_on == Id).ToList();
                _dbContext.Hocsinh_Lopon.RemoveRange(hs);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteByHocSinh(int Id)
        {
            try
            {
                var hs = _dbContext.Hocsinh_Lopon.Where(c=>c.Id_hoc_sinh == Id).ToList();
                _dbContext.Hocsinh_Lopon.RemoveRange(hs);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckId(int Id, int idDonvi)
        {
            if (Id <= 0) return false;
            try
            {
                return _dbContext.Hocsinh_Lopon.Any(c => c.Id == Id);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.Hocsinh_Lopon.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
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
                var headers = new[] { "STT", "Mã lớp", "Tên lớp ôn tập", "Mã học sinh", "Họ và tên học sinh" };
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
                var listMa = new HashSet<string>();
                foreach (var row in rows)
                {
                    var ma = row.Cell(2).GetValue<string>()?.Trim();
                    var ten = row.Cell(3).GetValue<string>()?.Trim();
                    var lop = row.Cell(4).GetValue<string>()?.Trim();

                    if (string.IsNullOrEmpty(ma))
                        return (false, "Mã học sinh không được để trống");

                    if (string.IsNullOrEmpty(ten))
                        return (false, "Họ tên không được để trống");

                    if (string.IsNullOrEmpty(lop))
                        return (false, "Lớp không được để trống");

                    if (!string.IsNullOrEmpty(ma))
                    {
                        if (listMa.Contains(ma))
                        {
                            return (false, $"Mã học sinh \"{ma}\" bị trùng trong file Excel");
                        }
                        else
                        {
                            listMa.Add(ma);
                        }
                    }

                    rowNumber++;
                }
                int stt = 1;
                var dataTable = new DataTable();
                dataTable.Columns.Add("STT", typeof(int));
                dataTable.Columns.Add("Ma_lop", typeof(string));
                dataTable.Columns.Add("Ten_lop", typeof(string));
                dataTable.Columns.Add("Ma_hoc_sinh", typeof(string));
                dataTable.Columns.Add("Ten_hoc_sinh", typeof(string));
                
                foreach (var row in rows)
                {
                    dataTable.Rows.Add(
                        stt,
                        row.Cell(2).GetValue<string>()?.Trim(),
                        row.Cell(3).GetValue<string>()?.Trim(),
                        row.Cell(4).GetValue<string>()?.Trim(),
                        row.Cell(5).GetValue<string>()?.Trim()
                    );
                    stt++;
                }

                var paramIdDonvi = new SqlParameter("Id_don_vi", SqlDbType.Int) { Value = idDonvi };
                var dataParam = new SqlParameter("@Data", SqlDbType.Structured)
                {
                    TypeName = "dbo.HocSinhLopOn",
                    Value = dataTable
                };
                var paramMessage = new SqlParameter("Message", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };
                var results = _dbContext.Database.ExecuteSqlRaw("EXEC ImportHocSinhLopOn @Id_don_vi, @Data, @Message OUTPUT ", paramIdDonvi, dataParam, paramMessage);
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
