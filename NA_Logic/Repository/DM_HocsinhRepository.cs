using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
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
    public class DM_HocsinhRepository : IDM_HocsinhRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_HocsinhRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Hocsinh_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, int idLop)
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
                var paramIdLop = new SqlParameter("idLop", SqlDbType.Int)
                {
                    Value = idLop
                };

                var result = _dbContext.Set<DM_Hocsinh_List>().FromSqlRaw("EXEC [DM_Hocsinh_GetList_Paging] @pageIndex, @pageSize, @search,@idDonvi, @idLop",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi, paramIdLop).ToList();
                if (result == null) result = new List<DM_Hocsinh_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Hocsinh GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var Lopontap = _dbContext.DM_Hocsinh.FirstOrDefault(c => c.Id == Id && c.Id_don_vi == idDonvi);
                return Lopontap;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public int CountHocSinhByLop(int idLop)
        {
            try
            {
                int Tong = _dbContext.DM_Hocsinh.Where(c => c.Id_lop_chinh == idLop).Count();
                return Tong;
            }
            catch
            {
                return 0;
            }
        }
        public bool CheckMa(string Ma, int idDonvi, int? Id)
        {
            try
            {
                var query = _dbContext.DM_Hocsinh.Where(c => c.Ma == Ma && c.Id_don_vi == idDonvi);

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
        public bool Add(DM_Hocsinh hs)
        {
            try
            {
                _dbContext.DM_Hocsinh.Add(hs);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Hocsinh hs)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Hocsinh.Update(hs);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckContraint(int id, int idDonvi)
        {
            try
            {
                bool check = (from kq in _dbContext.KetQua_Baikiemtra.AsNoTracking()
                              join hs in _dbContext.DM_Hocsinh.AsNoTracking() on kq.Id_hoc_sinh equals hs.Id
                              where hs.Id_don_vi == idDonvi && hs.Id == id
                              select 1).Any();
                return check;
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
                var hs = _dbContext.DM_Hocsinh.FirstOrDefault(c => c.Id == Id);
                _dbContext.DM_Hocsinh.Remove(hs);
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
                return _dbContext.DM_Hocsinh.Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids, int idDonvi)
        {
            var existingIds = _dbContext.DM_Hocsinh.Where(c => c.Id_don_vi == idDonvi && ids.Contains(c.Id)).Select(c => c.Id).ToList();
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
                    return (false,"File không có dữ liệu");

                var headers = new[] { "STT", "Mã học sinh", "Họ và tên học sinh", "Lớp chính khóa" };
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
                    else if(ma.Length > 50)
                        return (false, "Mã học sinh không được quá 50 kí tự");

                    if (string.IsNullOrEmpty(ten))
                        return (false, "Họ tên không được để trống");
                    else if (ten.Length > 200)
                        return (false, "Tên học sinh không được quá 200 kí tự");

                    if (string.IsNullOrEmpty(lop))
                        return (false, "Lớp không được để trống");

                    if (!string.IsNullOrEmpty(ma))
                    {
                        if (listMa.Contains(ma))
                        {
                            return (false,$"Mã học sinh \"{ma}\" bị trùng trong file Excel");
                        }
                        else
                        {
                            listMa.Add(ma);
                        }
                    }

                    rowNumber++;
                }

                var dataTable = new DataTable();
                dataTable.Columns.Add("Id", typeof(int));
                dataTable.Columns.Add("Ma_hoc_sinh", typeof(string));
                dataTable.Columns.Add("Ten_hoc_sinh", typeof(string));
                dataTable.Columns.Add("Ten_lop", typeof(string));

                
                foreach (var row in rows)
                {
                    dataTable.Rows.Add(
                        rowNumber,
                        row.Cell(2).GetValue<string>()?.Trim(),  
                        row.Cell(3).GetValue<string>()?.Trim(),  
                        row.Cell(4).GetValue<string>()?.Trim()  
                    );
                    rowNumber++;
                }
                var paramIdDonvi = new SqlParameter("Id_don_vi", SqlDbType.Int) { Value = idDonvi };
                var dataParam = new SqlParameter("@Data", SqlDbType.Structured)
                {
                    TypeName = "dbo.HocSinh",
                    Value = dataTable
                };
                var paramMessage = new SqlParameter("Message", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };
                var results =  _dbContext.Database.ExecuteSqlRaw("EXEC ImportHocSinh @Id_don_vi, @Data, @Message OUTPUT ",paramIdDonvi, dataParam, paramMessage);
                var Message = paramMessage.Value?.ToString() ?? "";
                
                if(Message == "Thành công")
                    return(true,Message);
                return(false,Message);

            }
            catch (Exception ex)
            {
                return (false, "Có lỗi hệ thống");
            }
        }

    }
}
