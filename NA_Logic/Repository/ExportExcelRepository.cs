using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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
    public class ExportExcelRepository: IExportExcelRepository
    {
        private readonly NA_DbContext _context;
        public ExportExcelRepository(NA_DbContext context)
        {
            _context = context;
        }

        public List<Export> List_Tiet(int idtkb)
        {
            try
            {
                var paramIdTkb = new SqlParameter("Id_tkb", SqlDbType.Int)
                {
                    Value = idtkb
                };
                var result = _context.Set<Chitiet_Thoikhoabieu_List>().FromSqlRaw("EXEC Get_Object  @Id_tkb = @Id_tkb", paramIdTkb)
                    .ToList();

                if (result == null) return null;
                var ds_tiet = new List<Export>();
                foreach (var item in result)
                {
                    ds_tiet.Add(new Export
                    {
                        Ten_truong = item.Ten_don_vi,
                        Ten_lop = item.Ten_lop,
                        Ten_mon = item.Ten_mon,
                        Ten_giao_vien = item.Ten_giao_vien,
                        Ten_phong = item.Ten_phong,
                        Id_ca = item.Id_ca,
                        Ngay = item.Ngay,
                        Tiet = item.Tiet
                    });
                }
                return ds_tiet;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public byte[] ExportExcel_Class(int idtkb)
        {
            var data = List_Tiet(idtkb);
            if (!data.Any()) return null;

            using var workbook = new XLWorkbook();

            var lopGroups = data.GroupBy(x => x.Ten_lop).ToList();

            foreach (var lopGroup in lopGroups)
            {
                var tenLop = lopGroup.Key;
                var lopData = lopGroup.ToList();

                var worksheet = workbook.Worksheets.Add($"{tenLop}");

                // Tiêu đề
                var firstRow = lopData.FirstOrDefault();
                worksheet.Cell(1, 1).Value = firstRow?.Ten_truong?.ToUpper() ?? "TRƯỜNG THCS";
                worksheet.Range(1, 1, 1, 7).Merge();
                worksheet.Cell(1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(1, 1).Style.Font.SetBold(true).Font.SetFontSize(14);

                worksheet.Cell(3, 1).Value = $"Thời khóa biểu lớp: {tenLop}";
                worksheet.Range(3, 1, 3, 7).Merge();
                worksheet.Cell(3, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(3, 1).Style.Font.SetBold(true).Font.SetFontSize(12);

                // Headers
                var headers = new[] { "Tiết - Thứ", "Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var headerCell = worksheet.Cell(6, i + 1);
                    headerCell.Value = headers[i];
                    headerCell.Style.Font.SetBold(true);
                    headerCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    headerCell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                }

                // sáng
                var caSangCell = worksheet.Range(7, 1, 7, 7).Merge();
                caSangCell.Value = "CA SÁNG";
                caSangCell.Style.Font.SetBold(true);
                caSangCell.Style.Fill.SetBackgroundColor(XLColor.LightBlue);

                // tiết
                for (int tiet = 1; tiet <= 5; tiet++)
                {
                    int row = 7 + tiet;
                    var tietCell = worksheet.Cell(row, 1);
                    tietCell.Value = $"Tiết {tiet}";
                    tietCell.Style.Font.SetBold(true);
                    tietCell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    // Điền dữ liệu cho từng ngày
                    for (int ngay = 1; ngay <= 7; ngay++)
                    {
                        int col = ngay + 1;
                        var lesson = lopData.FirstOrDefault(x => x.Tiet == tiet && x.Ngay == ngay && x.Id_ca == 1);
                        var cell = worksheet.Cell(row,  col);

                        if (lesson != null)
                        {
                            cell.Value = $"{lesson.Ten_mon} - {lesson.Ten_phong}\n{lesson.Ten_giao_vien}";
                            cell.Style.Alignment.SetWrapText(true);
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }
                    }
                }

                // chiều
                var caChieuCell = worksheet.Range(13, 1, 13, 7).Merge();
                caChieuCell.Value = "CA CHIỀU";
                caChieuCell.Style.Font.SetBold(true);
                caChieuCell.Style.Fill.SetBackgroundColor(XLColor.LightBlue);

                // Tiết
                for (int tiet = 1; tiet <= 5; tiet++)
                {
                    int row = 13 + tiet;
                    var tietCell = worksheet.Cell(row, 1);
                    tietCell.Value = $"Tiết {tiet}";
                    tietCell.Style.Font.SetBold(true);
                    tietCell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    // Điền dữ liệu cho từng ngày
                    for (int ngay = 1; ngay <= 7; ngay++)
                    {
                        int col = ngay + 1;
                        var lesson = lopData.FirstOrDefault(x => x.Tiet == tiet && x.Ngay == ngay && x.Id_ca == 2);
                        var cell = worksheet.Cell(row, col);

                        if (lesson != null)
                        {
                            cell.Value = $"{lesson.Ten_mon} - {lesson.Ten_phong}\n{lesson.Ten_giao_vien}";
                            cell.Style.Alignment.SetWrapText(true);
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }
                    }
                }

                // Tạo border cho toàn bộ bảng
                var dataRange = worksheet.Range(6, 1, 18, 7);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Set chiều cao hàng
                for (int i = 8; i <= 12; i++)
                {
                    worksheet.Row(i).Height = 45;
                }
                for (int i = 14; i <= 18; i++)
                {
                    worksheet.Row(i).Height = 45;
                }

                // Set chiều rộng cột
                worksheet.Column(1).Width = 12;
                for (int i = 2; i <= 8; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        public byte[] ExportExcel_Teacher(int idtkb)
        {
            var data = List_Tiet(idtkb);
            if (!data.Any()) return null;

            using var workbook = new XLWorkbook();

            var Gvgroup = data.GroupBy(x => x.Ten_giao_vien).ToList();

            foreach (var Giaoviengroup in Gvgroup)
            {
                var tenGV = Giaoviengroup.Key;
                var lopData = Giaoviengroup.ToList();

                var worksheet = workbook.Worksheets.Add($"{tenGV}");

                // Tiêu đề
                var firstRow = lopData.FirstOrDefault();
                worksheet.Cell(1, 1).Value = firstRow?.Ten_truong?.ToUpper() ?? "TRƯỜNG THCS";
                worksheet.Range(1, 1, 1, 7).Merge();
                worksheet.Cell(1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(1, 1).Style.Font.SetBold(true).Font.SetFontSize(14);

                worksheet.Cell(3, 1).Value = $"Thời khóa biểu giáo viên: {tenGV}";
                worksheet.Range(3, 1, 3, 7).Merge();
                worksheet.Cell(3, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(3, 1).Style.Font.SetBold(true).Font.SetFontSize(12);

                // Headers
                var headers = new[] { "Tiết - Thứ", "Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var headerCell = worksheet.Cell(6, i + 1);
                    headerCell.Value = headers[i];
                    headerCell.Style.Font.SetBold(true);
                    headerCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    headerCell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                }

                // sáng
                var caSangCell = worksheet.Range(7, 1, 7, 7).Merge();
                caSangCell.Value = "CA SÁNG";
                caSangCell.Style.Font.SetBold(true);
                caSangCell.Style.Fill.SetBackgroundColor(XLColor.LightBlue);

                // tiết
                for (int tiet = 1; tiet <= 5; tiet++)
                {
                    int row = 7 + tiet;
                    var tietCell = worksheet.Cell(row, 1);
                    tietCell.Value = $"Tiết {tiet}";
                    tietCell.Style.Font.SetBold(true);
                    tietCell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    // Điền dữ liệu cho từng ngày
                    for (int ngay = 1; ngay <= 7; ngay++)
                    {
                        int col = ngay + 1;
                        var lesson = lopData.FirstOrDefault(x => x.Tiet == tiet && x.Ngay == ngay && x.Id_ca == 1);
                        var cell = worksheet.Cell(row,  col);

                        if (lesson != null)
                        {
                            cell.Value = $"{lesson.Ten_mon} - {lesson.Ten_phong}\n{lesson.Ten_giao_vien}";
                            cell.Style.Alignment.SetWrapText(true);
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }
                    }
                }

                // chiều
                var caChieuCell = worksheet.Range(13, 1, 13, 7).Merge();
                caChieuCell.Value = "CA CHIỀU";
                caChieuCell.Style.Font.SetBold(true);
                caChieuCell.Style.Fill.SetBackgroundColor(XLColor.LightBlue);

                // Tiết
                for (int tiet = 1; tiet <= 5; tiet++)
                {
                    int row = 13 + tiet;
                    var tietCell = worksheet.Cell(row, 1);
                    tietCell.Value = $"Tiết {tiet}";
                    tietCell.Style.Font.SetBold(true);
                    tietCell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    // Điền dữ liệu cho từng ngày
                    for (int ngay = 1; ngay <= 7; ngay++)
                    {
                        int col = ngay + 1;
                        var lesson = lopData.FirstOrDefault(x => x.Tiet == tiet && x.Ngay == ngay && x.Id_ca == 2);
                        var cell = worksheet.Cell(row, col);

                        if (lesson != null)
                        {
                            cell.Value = $"{lesson.Ten_mon} - {lesson.Ten_phong}\n{lesson.Ten_giao_vien}";
                            cell.Style.Alignment.SetWrapText(true);
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }
                    }
                }

                // Tạo border cho toàn bộ bảng
                var dataRange = worksheet.Range(6, 1, 18, 7);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Set chiều cao hàng
                for (int i = 8; i <= 12; i++)
                {
                    worksheet.Row(i).Height = 45;
                }
                for (int i = 14; i <= 18; i++)
                {
                    worksheet.Row(i).Height = 45;
                }

                // Set chiều rộng cột
                worksheet.Column(1).Width = 12;
                for (int i = 2; i <= 8; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        public byte[] ExportExcel_TKB(int idtkb)
        {
            var data = List_Tiet(idtkb);
            if (!data.Any()) return null;

            using var workbook = new XLWorkbook();

            // Lấy danh sách các lớp từ data
            var lopList = data.Select(x => x.Ten_lop).Distinct().OrderBy(x => x).ToList();

            // Tạo sheet cho CA SÁNG
            var worksheetSang = workbook.Worksheets.Add("CA SÁNG");
            CreateCaWorksheet(worksheetSang, data, lopList, 1, "CA SÁNG");

            // Tạo sheet cho CA CHIỀU  
            var worksheetChieu = workbook.Worksheets.Add("CA CHIỀU");
            CreateCaWorksheet(worksheetChieu, data, lopList, 2, "CA CHIỀU");

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void CreateCaWorksheet(IXLWorksheet worksheet, List<Export> data, List<string> lopList, int idCa, string tenCa)
        {
            var firstRow = data.FirstOrDefault();

            // Tiêu đề trường
            worksheet.Cell(1, 1).Value = firstRow?.Ten_truong?.ToUpper() ?? "TRƯỜNG THCS";
            worksheet.Range(1, 1, 1, lopList.Count + 2).Merge();
            worksheet.Cell(1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(1, 1).Style.Font.SetBold(true).Font.SetFontSize(14);

            // Tiêu đề ca
            worksheet.Cell(3, 1).Value = $"THỜI KHÓA BIỂU - {tenCa}";
            worksheet.Range(3, 1, 3, lopList.Count + 2).Merge();
            worksheet.Cell(3, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(3, 1).Style.Font.SetBold(true).Font.SetFontSize(12);

            // Headers
            worksheet.Cell(5, 1).Value = "THỨ";
            worksheet.Cell(5, 1).Style.Font.SetBold(true);
            worksheet.Cell(5, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(5, 1).Style.Fill.SetBackgroundColor(XLColor.LightGray);

            worksheet.Cell(5, 2).Value = "TIẾT";
            worksheet.Cell(5, 2).Style.Font.SetBold(true);
            worksheet.Cell(5, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(5, 2).Style.Fill.SetBackgroundColor(XLColor.LightGray);

            // Headers cho các lớp
            for (int i = 0; i < lopList.Count; i++)
            {
                var headerCell = worksheet.Cell(5, i + 3);
                headerCell.Value = lopList[i];
                headerCell.Style.Font.SetBold(true);
                headerCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                headerCell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
            }

            int currentRow = 6;

            // Tạo dữ liệu cho từng thứ
            var thuNames = new[] { "THỨ\nHAI", "THỨ\nBA", "THỨ\nTƯ", "THỨ\nNĂM", "THỨ\nSÁU", "THỨ\nBẢY" };

            for (int ngayIndex = 1; ngayIndex < thuNames.Length; ngayIndex++)
            {

                // Merge cột THỨ cho 5 tiết
                var thuRange = worksheet.Range(currentRow, 1, currentRow + 4, 1);
                thuRange.Merge();
                thuRange.Value = thuNames[ngayIndex];
                thuRange.Style.Font.SetBold(true);
                thuRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                thuRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                thuRange.Style.Alignment.SetWrapText(true);
                thuRange.Style.Fill.SetBackgroundColor(XLColor.LightBlue);

                // 5 tiết trong ca
                for (int tiet = 1; tiet <= 5; tiet++)
                {
                    // Cột TIẾT
                    worksheet.Cell(currentRow, 2).Value = tiet.ToString();
                    worksheet.Cell(currentRow, 2).Style.Font.SetBold(true);
                    worksheet.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(currentRow, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    // Điền dữ liệu cho từng lớp
                    for (int lopIndex = 0; lopIndex < lopList.Count; lopIndex++)
                    {
                        var tenLop = lopList[lopIndex];
                        var lesson = data.FirstOrDefault(x => x.Tiet == tiet && x.Ngay == ngayIndex && x.Id_ca == idCa && x.Ten_lop == tenLop);
                        var cell = worksheet.Cell(currentRow, lopIndex + 3);

                        if (lesson != null)
                        {
                            cell.Value = $"{lesson.Ten_mon} - {lesson.Ten_phong}\n{lesson.Ten_giao_vien}";
                            cell.Style.Alignment.SetWrapText(true);
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }
                    }
                    currentRow++;
                }
            }

            // Tạo border cho toàn bộ bảng
            var dataRange = worksheet.Range(5, 1, currentRow - 1, lopList.Count + 2);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Set chiều cao và rộng
            for (int i = 6; i <= currentRow - 1; i++)
            {
                worksheet.Row(i).Height = 45;
            }

            worksheet.Column(1).Width = 8;  // Cột THỨ
            worksheet.Column(2).Width = 6;  // Cột TIẾT
            for (int i = 3; i <= lopList.Count + 2; i++)   // Cột các lớp
            {
                worksheet.Column(i).Width = 18;
            }
        }
    }
}
