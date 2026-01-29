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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Hocsinh_TohopmonRepository : IHocsinh_TohopmonRepository
    {
        private readonly NA_DbContext _dbContext;
        public Hocsinh_TohopmonRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //public Hocsinh_Tohopmon GetDetailById(int Id, int idDonvi)
        //{
        //    try
        //    {
        //        Hocsinh_Tohopmon Tohopmontap = (from l in _dbContext.DM_Tohopmon_Ontap
        //                                  join hl in _dbContext.Hocsinh_Tohopmon on l.Id equals hl.Id_to_hop
        //                                  where hl.Id == Id && l.Id_don_vi == idDonvi
        //                                  select hl).FirstOrDefault();
        //        return Tohopmontap;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        public Hocsinh_Tohopmon_Multi GetToHopByIdHocSinh(int idHocSinh, int idDonvi)
        {
            try
            {
                var Tohopmon = _dbContext.DM_Hocsinh
                    .Where(l => l.Id == idHocSinh && l.Id_don_vi == idDonvi)
                    .Select(l => new Hocsinh_Tohopmon_Multi
                    {
                        Id_hoc_sinh = l.Id,
                        To_hop_mon = _dbContext.Hocsinh_Tohopmon.Where(hl => hl.Id_hoc_sinh == idHocSinh).Select(hl => hl.Id_to_hop).ToList()
                    })
                    .FirstOrDefault();

                return Tohopmon;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool CheckTrung(Hocsinh_Tohopmon hs, int idDonvi)
        {
            try
            {
                bool check = (from hl in _dbContext.Hocsinh_Tohopmon
                              join l in _dbContext.DM_Tohopmon_Ontap on hl.Id_to_hop equals l.Id
                              where hl.Id_hoc_sinh == hs.Id_hoc_sinh
                                  && hl.Id_to_hop == hs.Id_to_hop
                                  && l.Id_don_vi == idDonvi
                                  && (hs.Id <= 0 || hl.Id != hs.Id)
                              select hl).Any();

                return check;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool Add(Hocsinh_Tohopmon_Multi data)
        {
            using var tranc = _dbContext.Database.BeginTransaction();
            try
            {
                var listOld = _dbContext.Hocsinh_Tohopmon.Where(c => c.Id_hoc_sinh == data.Id_hoc_sinh).ToList();

                if (listOld.Any())
                    _dbContext.BulkDelete(listOld);

                var list = data.To_hop_mon.Select(c => new Hocsinh_Tohopmon
                {
                    Id_to_hop = c,
                    Id_hoc_sinh = data.Id_hoc_sinh
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
        
        public bool DeleteByHocSinh(int Id)
        {
            try
            {
                var hs = _dbContext.Hocsinh_Tohopmon.Where(c => c.Id_hoc_sinh == Id).ToList();
                _dbContext.Hocsinh_Tohopmon.RemoveRange(hs);
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
                return _dbContext.Hocsinh_Tohopmon.Any(c => c.Id == Id);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.Hocsinh_Tohopmon.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
        public byte[] ExportMauExcel(int idlop)
        {
            var data = (from hs in _dbContext.DM_Hocsinh
                        where hs.Id_lop_chinh == idlop
                        select hs).ToList();
            if (!data.Any()) return null;

            using var workbook = new XLWorkbook();

            var firstRow = data.FirstOrDefault();
            var worksheet = workbook.Worksheets.Add("DS học sinh");

            // Headers
            worksheet.Cell(1, 1).Value = "STT";
            worksheet.Cell(1, 1).Style.Font.SetBold(true);
            worksheet.Cell(1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(1, 1).Style.Fill.SetBackgroundColor(XLColor.LightGray);

            worksheet.Cell(1, 2).Value = "Mã học sinh";
            worksheet.Cell(1, 2).Style.Font.SetBold(true);
            worksheet.Cell(1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(1, 2).Style.Fill.SetBackgroundColor(XLColor.LightGray);

            worksheet.Cell(1, 3).Value = "Họ và tên học sinh";
            worksheet.Cell(1, 3).Style.Font.SetBold(true);
            worksheet.Cell(1, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(1, 3).Style.Fill.SetBackgroundColor(XLColor.LightGray);

            worksheet.Cell(1, 4).Value = "Mã tổ hợp môn";
            worksheet.Cell(1, 4).Style.Font.SetBold(true);
            worksheet.Cell(1, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(1, 4).Style.Fill.SetBackgroundColor(XLColor.LightGray);


            int currentRow = 2;

            for (int i = 0; i < data.Count; i++)
            {
                worksheet.Cell(currentRow, 1).Value = i + 1;
                worksheet.Cell(currentRow, 2).Value = data[i].Ma;
                worksheet.Cell(currentRow, 3).Value = data[i].Ten;

                currentRow++;
            }
            worksheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
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
                var headers = new[] { "STT", "Mã học sinh", "Họ và tên học sinh", "Mã tổ hợp môn" };
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
                int stt = 1;
                var listMa = new HashSet<string>();
                var dataTable = new DataTable();
                dataTable.Columns.Add("STT", typeof(int));
                dataTable.Columns.Add("Ma_hoc_sinh", typeof(string));
                dataTable.Columns.Add("Ten_hoc_sinh", typeof(string));
                dataTable.Columns.Add("Ma_to_hop", typeof(string));

                foreach (var row in rows)
                {
                    var ma = row.Cell(2).GetValue<string>()?.Trim();
                    var ten = row.Cell(3).GetValue<string>()?.Trim();
                    var maToHop = row.Cell(4).GetValue<string>()?.Trim();

                    if (string.IsNullOrEmpty(ma))
                        return (false, "Mã học sinh không được để trống");
                    if (string.IsNullOrEmpty(ten))
                        return (false, "Họ tên không được để trống");

                    if (!string.IsNullOrEmpty(maToHop))
                    {
                        var danhSachMa = maToHop.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(m => m.Trim())
                                                .ToArray();

                        foreach (var m in danhSachMa)
                        {
                            if (!Regex.IsMatch(m, @"^[A-Za-z0-9._-]+$"))
                                return (false, $"Mã tổ hợp môn \"{m}\" chứa ký tự không hợp lệ");
                        }

                        if (danhSachMa.Length != danhSachMa.Distinct().Count())
                            return (false, $"Mã học sinh \"{ma}\" có mã tổ hợp môn bị trùng");
                    }

                    if (listMa.Contains(ma))
                        return (false, $"Mã học sinh \"{ma}\" bị trùng trong file");

                    listMa.Add(ma);

                    dataTable.Rows.Add(stt, ma, ten, maToHop);
                    stt++;
                    rowNumber++;
                }

                var paramIdDonvi = new SqlParameter("Id_don_vi", SqlDbType.Int) { Value = idDonvi };
                var dataParam = new SqlParameter("@Data", SqlDbType.Structured)
                {
                    TypeName = "dbo.HocSinhTohopmon",
                    Value = dataTable
                };
                var paramMessage = new SqlParameter("Message", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };
                var results = _dbContext.Database.ExecuteSqlRaw("EXEC ImportHocSinhTohopmon @Id_don_vi, @Data, @Message OUTPUT ", paramIdDonvi, dataParam, paramMessage);
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
