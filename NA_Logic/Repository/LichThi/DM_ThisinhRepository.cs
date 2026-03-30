using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EFCore.BulkExtensions;
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
    public class DM_ThisinhRepository : IDM_ThisinhRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_ThisinhRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Thisinh_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, int idDiemThi, int idHoiDong)
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
                var paramIdHoiDong = new SqlParameter("idHoiDong", SqlDbType.Int)
                {
                    Value = idHoiDong
                };
                var paramIdDiemThi = new SqlParameter("idDiemThi", SqlDbType.Int)
                {
                    Value = idDiemThi
                };
                var result = _dbContext.Set<DM_Thisinh_List>().FromSqlRaw("EXEC [DM_Thisinh_GetList_Paging] @pageIndex, @pageSize, @search, @idDonvi, @idDiemThi, @idHoiDong",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi, paramIdDiemThi, paramIdHoiDong).ToList();
                if (result == null) result = new List<DM_Thisinh_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Thisinh_Detail GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var result = (from thisinh in _dbContext.DM_Thisinh
                              join diem in _dbContext.DM_Diemthi on thisinh.Id_diem_thi equals diem.Id
                              join hoidong in _dbContext.DM_Hoidongthi on diem.Id_hoi_dong equals hoidong.Id into hoidongGroup from hoidong in hoidongGroup.DefaultIfEmpty()
                              join noisinh in _dbContext.DM_DonviHanhchinh on thisinh.Noi_sinh_xa equals noisinh.Id
                              join thuongtru in _dbContext.DM_DonviHanhchinh on thisinh.Thuong_tru_xa equals thuongtru.Id
                              where thisinh.Id == Id && (diem.Id_don_vi == idDonvi || hoidong.Id_don_vi == idDonvi)
                              select new DM_Thisinh_Detail
                              {
                                  Id = thisinh.Id,
                                  So_bao_danh = thisinh.So_bao_danh,
                                  Ho_va_ten = thisinh.Ho_va_ten,
                                  Ngay_sinh = thisinh.Ngay_sinh,
                                  Noi_sinh_xa = thisinh.Noi_sinh_xa,
                                  Noi_sinh_tinh = (int)noisinh.Id_cha,
                                  Dan_toc = thisinh.Dan_toc,
                                  CCCD = thisinh.CCCD,
                                  Thuong_tru_xa = thisinh.Thuong_tru_xa,
                                  Thuong_tru_tinh = (int)thuongtru.Id_cha,
                                  Mon_thi_1 = thisinh.Mon_thi_1,
                                  Mon_thi_2 = thisinh.Mon_thi_2,
                                  Id_diem_thi = thisinh.Id_diem_thi,
                                  Id_hoi_dong = diem.Id_hoi_dong,
                                  Id_nam = hoidong.Id_nam
                              }).FirstOrDefault();

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Thisinh thisinh)
        {
            try
            {
                _dbContext.DM_Thisinh.Add(thisinh);
                _dbContext.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UQ_CCCD") == true)
            {
                return false;
            }
        }
        public bool Update(DM_Thisinh thisinh)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Thisinh.Update(thisinh);
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
                var thisinh = _dbContext.DM_Thisinh.FirstOrDefault(c => c.Id == id);
                if (thisinh == null)
                {
                    return false;
                }
                _dbContext.DM_Thisinh.Remove(thisinh);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Check_constraint(int Id)
        {
            try
            {

                return _dbContext.Database.SqlQuery<int>($@"
                          select 1 as Value from DM_Giamthi where Id_diem_thi = {Id}
                          union select 1 from DM_Thisinh where Id_diem_thi = {Id}").Any();
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
                var idDiemthiByDonvi = _dbContext.DM_Diemthi.Where(c => c.Id_don_vi == idDonvi).Select(c => c.Id).ToList();

                var idHoidong = _dbContext.DM_Hoidongthi.Where(c => c.Id_don_vi == idDonvi).Select(c => c.Id).ToList();

                var idDiemthiByHoidong = _dbContext.DM_Diemthi.Where(c => idHoidong.Contains(c.Id_hoi_dong)).Select(c => c.Id).ToList();

                var idDiemthi = idDiemthiByDonvi.Union(idDiemthiByHoidong).ToList();

                var thisinh = _dbContext.DM_Thisinh.Any(c => c.Id == Id && idDiemthi.Contains(c.Id_diem_thi));
                return thisinh;
            }
            catch
            {
                return false;
            }
        }
        //public bool CheckIds(IEnumerable<int> ids)
        //{
        //    var existingIds = _dbContext.DM_Thisinh.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
        //    return ids.All(id => existingIds.Contains(id));
        //}
        public (bool result, string mess, List<ThiSinhCheck> list) Import(Stream file, int idDonvi)
        {
            try
            {
                using var workbook = new XLWorkbook(file);
                var worksheet = workbook.Worksheet(1);
                var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;
                if (lastRow < 2)
                    return (false, "File Excel không có dữ liệu", null);

                var headers = new[] { "STT", "Mã điểm thi", "Họ và tên", "CCCD", "Ngày sinh", "Nơi sinh", "Nơi thường trú", "Dân tộc", "Môn thi 1", "Môn thi 2" };
                for (int col = 1; col <= headers.Length; col++)
                {
                    var headerValue = worksheet.Cell(1, col).Value.ToString()?.Trim()
                                        .Normalize(NormalizationForm.FormC);
                    var expected = headers[col - 1].Normalize(NormalizationForm.FormC);

                    if (string.IsNullOrEmpty(headerValue) ||
                        !headerValue.Equals(expected, StringComparison.OrdinalIgnoreCase))
                        return (false, "File không đúng định dạng",null);
                }

                var rows = worksheet.RowsUsed().Skip(1).ToList();

                var dataTable = new DataTable();
                dataTable.Columns.Add("STT", typeof(int));
                dataTable.Columns.Add("Ma_diem_thi", typeof(string));
                dataTable.Columns.Add("Ho_va_ten", typeof(string));
                dataTable.Columns.Add("CCCD", typeof(string));
                dataTable.Columns.Add("Ngay_sinh", typeof(DateTime));
                dataTable.Columns.Add("Noi_sinh", typeof(int));
                dataTable.Columns.Add("Noi_thuong_tru", typeof(int));
                dataTable.Columns.Add("Dan_toc", typeof(int));
                dataTable.Columns.Add("Mon_1", typeof(string));
                dataTable.Columns.Add("Mon_2", typeof(string));


                int stt = 1;
                foreach (var row in rows)
                {
                    var maDiemThi = row.Cell(2).GetValue<string>()?.Trim();
                    var hoVaTen = row.Cell(3).GetValue<string>()?.Trim();
                    var cccd = row.Cell(4).GetValue<string>()?.Trim();
                    var ngaySinhRaw = row.Cell(5).Value.ToString()?.Trim();
                    var noiSinhRaw = row.Cell(6).Value.ToString()?.Trim();
                    var noiThuongTruRaw = row.Cell(7).Value.ToString()?.Trim();
                    var danTocRaw = row.Cell(8).Value.ToString()?.Trim();
                    var mon1 = row.Cell(9).Value.ToString()?.Trim();
                    var mon2 = row.Cell(10).Value.ToString()?.Trim();

                    // Check không được để trống
                    if (string.IsNullOrEmpty(maDiemThi)) 
                        return (false, $"Dòng {stt + 1}: Mã điểm thi không được để trống", null);
                    if (string.IsNullOrEmpty(hoVaTen)) 
                        return (false, $"Dòng {stt + 1}: Họ và tên không được để trống", null);
                    if (string.IsNullOrEmpty(cccd)) 
                        return (false, $"Dòng {stt + 1}: CCCD không được để trống", null);
                    if (string.IsNullOrEmpty(ngaySinhRaw)) 
                        return (false, $"Dòng {stt + 1}: Ngày sinh không được để trống", null);
                    if (string.IsNullOrEmpty(noiSinhRaw)) 
                        return (false, $"Dòng {stt + 1}: Nơi sinh không được để trống", null);
                    if (string.IsNullOrEmpty(noiThuongTruRaw)) 
                        return (false, $"Dòng {stt + 1}: Nơi thường trú không được để trống", null);
                    if (string.IsNullOrEmpty(danTocRaw)) 
                        return (false, $"Dòng {stt + 1}: Dân tộc không được để trống", null);
                    if (string.IsNullOrEmpty(mon1)) 
                        return (false, $"Dòng {stt + 1}: Môn thi 1 không được để trống", null);
                    if (string.IsNullOrEmpty(mon2)) 
                        return (false, $"Dòng {stt + 1}: Môn thi 2 không được để trống", null);

                    // Check đúng kiểu dữ liệu
                    if (!DateTime.TryParse(ngaySinhRaw, out var ngaySinh)) 
                        return (false, $"Dòng {stt + 1}: Ngày sinh không đúng định dạng", null);
                    if (!int.TryParse(noiSinhRaw, out var noiSinh)) 
                        return (false, $"Dòng {stt + 1}: Nơi sinh phải là số", null);
                    if (!int.TryParse(noiThuongTruRaw, out var noiThuongTru)) 
                        return (false, $"Dòng {stt + 1}: Nơi thường trú phải là số", null);
                    if (!int.TryParse(danTocRaw, out var danToc)) 
                        return (false, $"Dòng {stt + 1}: Dân tộc phải là số", null);

                    dataTable.Rows.Add(stt, maDiemThi, hoVaTen, cccd, ngaySinh, noiSinh, noiThuongTru, danToc, mon1, mon2);
                    stt++;
                }

                var paramIdDonvi = new SqlParameter("Id_don_vi", SqlDbType.Int) { Value = idDonvi };
                var dataParam = new SqlParameter("@Data", SqlDbType.Structured)
                {
                    TypeName = "dbo.DMThiSinh",
                    Value = dataTable
                };
                var paramMessage = new SqlParameter("Message", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };

                var danhSach = _dbContext.Database.SqlQueryRaw<ThiSinhCheck>("EXEC [CheckThiSinh] @Id_don_vi, @Data, @Message OUTPUT",paramIdDonvi, dataParam, paramMessage)
                    .ToList();

                var message = paramMessage.Value?.ToString() ?? "";
                if (message == "Thành công")
                    return (true, message, danhSach);
                return (false, message, null);
            }
            catch
            {
                return (false, "Import thất bại", null);
            }
        }
    }
}
