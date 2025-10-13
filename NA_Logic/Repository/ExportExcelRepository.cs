using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens.Configuration;
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
                        Id_mon = item.Id_mon,
                        Id_giao_vien = item.Id_giao_vien,
                        Id_lop = item.Id_lop,
                        Ten_truong = item.Ten_don_vi,
                        Ten_lop = item.Ten_lop,
                        Ten_mon = item.Ten_mon,
                        Ho_ho_dem = item.Ho_va_ho_dem,
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
        public byte[] ExportExcel_Class(int idtkb, int show_room, int show_teacher)
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
                            var information = new List<string>();
                            information.Add(lesson.Ten_mon);
                            // Chọn 1: phòng
                            if (show_room == 1)
                            {
                                information.Add(lesson.Ten_phong);
                            }
                            // Chọn 2: giáo viên
                            if (show_teacher == 1)
                            {
                                information.Add(lesson.Ten_giao_vien);
                            }

                            cell.Value = string.Join(" - ", information);
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
                            var information = new List<string>();
                            information.Add(lesson.Ten_mon);
                            // Chọn 1: phòng
                            if (show_room == 1)
                            {
                                information.Add(lesson.Ten_phong);
                            }
                            // Chọn 2: giáo viên
                            if (show_teacher == 1)
                            {
                                information.Add(lesson.Ten_giao_vien);
                            }

                            cell.Value = string.Join(" - ", information);
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
        public byte[] ExportExcel_Teacher(int idtkb, int show_room)
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

                worksheet.Cell(3, 1).Value = $"Thời khóa biểu giáo viên";
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
                            var information = new List<string>();
                            information.Add(lesson.Ten_mon);
                            information.Add(lesson.Ten_lop);
                            // Chọn 1: phòng
                            if (show_room == 1)
                            {
                                information.Add(lesson.Ten_phong);
                            }

                            cell.Value = string.Join(" - ", information);
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
                            var information = new List<string>();
                            information.Add(lesson.Ten_mon);
                            information.Add(lesson.Ten_lop);
                            // Chọn 1: phòng
                            if (show_room == 1)
                            {
                                information.Add(lesson.Ten_phong);
                            }

                            cell.Value = string.Join(" - ", information);
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
        public byte[] ExportExcel_TKB(int idtkb, int show_room, int show_teacher)
        {
            var data = List_Tiet(idtkb);
            if (!data.Any()) return null;

            using var workbook = new XLWorkbook();

            // Lấy danh sách các lớp từ data
            var lopList = data.Select(x => x.Ten_lop).Distinct().OrderBy(x => x).ToList();

            // Tạo sheet cho CA SÁNG
            var worksheetSang = workbook.Worksheets.Add("CA SÁNG");
            CreateCaWorksheet(worksheetSang, data, lopList, 1, "CA SÁNG", show_room, show_teacher);

            // Tạo sheet cho CA CHIỀU  
            var worksheetChieu = workbook.Worksheets.Add("CA CHIỀU");
            CreateCaWorksheet(worksheetChieu, data, lopList, 2, "CA CHIỀU", show_room, show_teacher);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void CreateCaWorksheet(IXLWorksheet worksheet, List<Export> data, List<string> lopList, int idCa, string tenCa, int show_room, int show_teacher)
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

            for (int ngayIndex = 0; ngayIndex < thuNames.Length; ngayIndex++)
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
                        int ngay = ngayIndex + 1;
                        var tenLop = lopList[lopIndex];
                        var lesson = data.FirstOrDefault(x => x.Tiet == tiet && x.Ngay == ngay && x.Id_ca == idCa && x.Ten_lop == tenLop);
                        var cell = worksheet.Cell(currentRow, lopIndex + 3);

                        if (lesson != null)
                        {
                            var information = new List<string>();
                            information.Add(lesson.Ten_mon);
                            // Chọn 1: phòng
                            if (show_room == 1)
                            {
                                information.Add(lesson.Ten_phong);
                            }
                            // Chọn 2: giáo viên
                            if (show_teacher == 1)
                            {
                                information.Add(lesson.Ten_giao_vien);
                            }

                            cell.Value = string.Join(" - ", information);
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

            worksheet.Column(1).Width = 8;  
            worksheet.Column(2).Width = 6; 
            for (int i = 3; i <= lopList.Count + 2; i++) 
            {
                worksheet.Column(i).Width = 18;
            }
        }

        public byte[] ExportExcel_MaTranToanTruong(int idtkb)
        {
            var data = List_Tiet(idtkb);
            if (!data.Any()) return null;

            using var workbook = new XLWorkbook();

            // Lấy danh sách các lớp từ data
            var lopList = data.Select(x => x.Ten_lop).Distinct().OrderBy(x => x).ToList();

            var firstRow = data.FirstOrDefault();
            var worksheet = workbook.Worksheets.Add("Toàn trường");
            // Tiêu đề trường
            worksheet.Cell(1, 1).Value = firstRow?.Ten_truong?.ToUpper() ?? "TRƯỜNG THCS";
            worksheet.Range(1, 1, 1, lopList.Count + 2).Merge();
            worksheet.Cell(1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(1, 1).Style.Font.SetBold(true).Font.SetFontSize(14);

            // Tiêu đề ca
            worksheet.Cell(3, 1).Value = $"MA TRẬN TOÀN TRƯỜNG";
            worksheet.Range(3, 1, 3, lopList.Count + 2).Merge();
            worksheet.Cell(3, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(3, 1).Style.Font.SetBold(true).Font.SetFontSize(12);

            // Headers
            worksheet.Cell(5, 1).Value = "CA";
            worksheet.Cell(5, 1).Style.Font.SetBold(true);
            worksheet.Cell(5, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(5, 1).Style.Fill.SetBackgroundColor(XLColor.LightGray);

            worksheet.Cell(5, 2).Value = "THỨ";
            worksheet.Cell(5, 2).Style.Font.SetBold(true);
            worksheet.Cell(5, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(5, 2).Style.Fill.SetBackgroundColor(XLColor.LightGray);

            worksheet.Cell(5, 3).Value = "TIẾT";
            worksheet.Cell(5, 3).Style.Font.SetBold(true);
            worksheet.Cell(5, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(5, 3).Style.Fill.SetBackgroundColor(XLColor.LightGray);


            // Headers cho các lớp
            for (int i = 0; i < lopList.Count; i++)
            {
                var headerCell = worksheet.Cell(5, i + 4);
                headerCell.Value = lopList[i];
                headerCell.Style.Font.SetBold(true);
                headerCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                headerCell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
            }

            int currentRow = 6;

            // Tạo dữ liệu cho từng CA
            var caNames = new[] { "SÁNG", "CHIỀU" };

            // Tạo dữ liệu cho từng thứ
            var thuNames = new[] { "THỨ\nHAI", "THỨ\nBA", "THỨ\nTƯ", "THỨ\nNĂM", "THỨ\nSÁU", "THỨ\nBẢY" };

            for (int caIndex = 0; caIndex < caNames.Length; caIndex++)
            {
                // Merge cột ca
                var caRange = worksheet.Range(currentRow, 1, currentRow + 29, 1);
                caRange.Merge();
                caRange.Value = caNames[caIndex];
                caRange.Style.Font.SetBold(true);
                caRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                caRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                caRange.Style.Alignment.SetWrapText(true);
                caRange.Style.Fill.SetBackgroundColor(XLColor.LightBlue);
                for (int ngayIndex = 0; ngayIndex < thuNames.Length; ngayIndex++)
                {

                    // Merge cột THỨ cho 5 tiết
                    var thuRange = worksheet.Range(currentRow, 2, currentRow + 4, 2);
                    thuRange.Merge();
                    thuRange.Value = thuNames[ngayIndex];
                    thuRange.Style.Font.SetBold(true);
                    thuRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    thuRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    thuRange.Style.Alignment.SetWrapText(true);

                    // 5 tiết trong ca
                    for (int tiet = 1; tiet <= 5; tiet++)
                    {
                        // Cột TIẾT
                        worksheet.Cell(currentRow, 3).Value = tiet.ToString();
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold(true);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                        // Điền dữ liệu cho từng lớp
                        for (int lopIndex = 0; lopIndex < lopList.Count; lopIndex++)
                        {
                            int idCa = caIndex + 1;
                            int ngay = ngayIndex + 1;
                            var tenLop = lopList[lopIndex];
                            var lesson = data.FirstOrDefault(x => x.Tiet == tiet && x.Ngay == ngay && x.Id_ca == idCa && x.Ten_lop == tenLop);
                            var cell = worksheet.Cell(currentRow, lopIndex + 4);

                            if (lesson != null)
                            {

                                cell.Value = $"{lesson.Ten_mon}\n{lesson.Ten_giao_vien}";
                                cell.Style.Alignment.SetWrapText(true);
                                cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            }
                        }
                        currentRow++;
                    }
                }
            }

            // Tạo border cho toàn bộ bảng
            var dataRange = worksheet.Range(5, 1, currentRow - 1, lopList.Count + 3);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Set chiều cao và rộng
            for (int i = 6; i <= currentRow - 1; i++)
            {
                worksheet.Row(i).Height = 45;
            }

            worksheet.Column(1).Width = 8;
            worksheet.Column(2).Width = 6;
            for (int i = 4; i <= lopList.Count + 3; i++)
            {
                worksheet.Column(i).Width = 18;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        public byte[] ExportExcel_MaTranKhoi(int idtkb)
        {
            var data = List_Tiet(idtkb);
            if (!data.Any()) return null;
            var dataDict = data.GroupBy(x => new { x.Tiet, x.Ngay, x.Id_ca, x.Id_lop }).ToDictionary(g => g.Key, g => g.First());
            using var workbook = new XLWorkbook();
            var lopIds = data.Select(x => x.Id_lop).Distinct().ToList();

            var khoiList = _context.DM_Lophoc.Where(l => lopIds.Contains(l.Id)).Join(_context.DM_Khoilop, l => l.Id_khoi, kl => kl.Id,
                    (l, kl) => new { kl.Ten, kl.Id }).Distinct().ToList();
            for (int k=0; k<khoiList.Count;k++)
            {
                // Lấy danh sách các lớp từ data
                var khoiId = khoiList[k].Id;

                var lopList = _context.DM_Lophoc.Where(l => l.Id_khoi == khoiId && lopIds.Contains(l.Id)).Select(l => new { l.Ten, l.Id }).Distinct().OrderBy(x => x.Ten).ToList();

                var firstRow = data.FirstOrDefault();
                var worksheet = workbook.Worksheets.Add($"{khoiList[k].Ten}");
                // Tiêu đề trường
                worksheet.Cell(1, 1).Value = firstRow?.Ten_truong?.ToUpper() ?? "TRƯỜNG THCS";
                worksheet.Range(1, 1, 1, lopList.Count + 2).Merge();
                worksheet.Cell(1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(1, 1).Style.Font.SetBold(true).Font.SetFontSize(14);

                // Tiêu đề sheet
                worksheet.Cell(3, 1).Value = $"MA TRẬN KHỐI";
                worksheet.Range(3, 1, 3, lopList.Count + 2).Merge();
                worksheet.Cell(3, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(3, 1).Style.Font.SetBold(true).Font.SetFontSize(12);

                // Headers
                worksheet.Range(5,1,6,1).Merge().Value = "THỨ";
                worksheet.Range(5, 1, 6, 1).Merge().Style.Font.SetBold(true);
                worksheet.Range(5, 1, 6, 1).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(5, 1, 6, 1).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Range(5, 1, 6, 1).Merge().Style.Fill.SetBackgroundColor(XLColor.LightGray);

                worksheet.Range(5, 2, 6, 2).Merge().Value = "CA";
                worksheet.Range(5, 2, 6, 2).Merge().Style.Font.SetBold(true);
                worksheet.Range(5, 2, 6, 2).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(5, 2, 6, 2).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Range(5, 2, 6, 2).Merge().Style.Fill.SetBackgroundColor(XLColor.LightGray);

                worksheet.Range(5, 3, 6, 3).Merge().Value = "TIẾT";
                worksheet.Range(5, 3, 6, 3).Merge().Style.Font.SetBold(true);
                worksheet.Range(5, 3, 6, 3).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(5, 3, 6, 3).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Range(5, 3, 6, 3).Merge().Style.Fill.SetBackgroundColor(XLColor.LightGray);


                // Headers cho các lớp
                for (int i = 0; i < lopList.Count; i++)
                {
                    int startCol = 4 + (i * 2);
                    //header cho lớp
                    var headerCell = worksheet.Range(5, startCol, 5, startCol + 1);
                    headerCell.Merge();
                    headerCell.Value = $"{lopList[i].Ten}";
                    headerCell.Style.Font.SetBold(true);
                    headerCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    headerCell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                    //môn, giáo viên
                    var monCell = worksheet.Cell(6, startCol);
                    monCell.Value = "Môn";
                    monCell.Style.Font.SetBold(true);
                    monCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    monCell.Style.Fill.SetBackgroundColor(XLColor.LightGray);

                    var gvCell = worksheet.Cell(6, startCol + 1);
                    gvCell.Value = "Giáo viên";
                    gvCell.Style.Font.SetBold(true);
                    gvCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    gvCell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                }

                int currentRow = 7;

                // Tạo dữ liệu cho từng CA
                var caNames = new[] { "SÁNG", "CHIỀU" };

                // Tạo dữ liệu cho từng thứ
                var thuNames = new[] { "THỨ\nHAI", "THỨ\nBA", "THỨ\nTƯ", "THỨ\nNĂM", "THỨ\nSÁU", "THỨ\nBẢY" };

                
                for (int ngayIndex = 0; ngayIndex < thuNames.Length; ngayIndex++)
                {

                    // Merge cột THỨ cho 5 tiết
                    var thuRange = worksheet.Range(currentRow, 1, currentRow + 9, 1);
                    thuRange.Merge();
                    thuRange.Value = thuNames[ngayIndex];
                    thuRange.Style.Font.SetBold(true);
                    thuRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    thuRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    thuRange.Style.Alignment.SetWrapText(true);
                    thuRange.Style.Fill.SetBackgroundColor(XLColor.LightBlue);
                    for (int caIndex = 0; caIndex < caNames.Length; caIndex++)
                    {
                        // Merge cột ca
                        var caRange = worksheet.Range(currentRow, 2, currentRow + 4, 2);
                        caRange.Merge();
                        caRange.Value = caNames[caIndex];
                        caRange.Style.Font.SetBold(true);
                        caRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        caRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        caRange.Style.Alignment.SetWrapText(true);
                        
                        // 5 tiết trong ca
                        for (int tiet = 1; tiet <= 5; tiet++)
                        {
                            // Cột TIẾT
                            worksheet.Cell(currentRow, 3).Value = tiet.ToString();
                            worksheet.Cell(currentRow, 3).Style.Font.SetBold(true);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                            // Điền dữ liệu cho từng lớp
                            for (int lopIndex = 0; lopIndex < lopList.Count; lopIndex++)
                            {
                                int idCa = caIndex + 1;
                                int ngay = ngayIndex + 1;
                                var tenLop = lopList[lopIndex].Ten;
                                var idLop = lopList[lopIndex].Id;
                                var key = new { Tiet = tiet, Ngay = ngay, Id_ca = idCa, Id_lop = idLop };
                                var lesson = dataDict.ContainsKey(key) ? dataDict[key] : null;

                                int startCol = 4 + (lopIndex * 2);

                                if (lesson != null)
                                {
                                    // Cột Môn
                                    var monCell = worksheet.Cell(currentRow, startCol);
                                    monCell.Value = lesson.Ten_mon;
                                    monCell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                    monCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                    // Cột Giáo viên
                                    var gvCell = worksheet.Cell(currentRow, startCol + 1);
                                    gvCell.Value = $"{lesson.Ho_ho_dem} {lesson.Ten_giao_vien}";
                                    gvCell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                    gvCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                }
                            }
                            currentRow++;
                        }
                    }
                        
                }

                // Tạo border cho toàn bộ bảng
                int lastCol = 3 + (lopList.Count * 2);
                var dataRange = worksheet.Range(5, 1, currentRow - 1, lastCol);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Set chiều cao hàng
                //for (int i = 8; i <= 12; i++)
                //{
                //    worksheet.Row(i).Height = 45;
                //}
                //for (int i = 14; i <= 18; i++)
                //{
                //    worksheet.Row(i).Height = 45;
                //}

                // Set chiều rộng cột
                
                for (int i = 4; i <= lastCol; i++)
                {
                    worksheet.Column(i).Width = 15;
                }
                for (int i = 2; i <= 8; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
            }
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        public byte[] ExportExcel_MaTranGiaoVien(int idtkb)
        {
            var data = List_Tiet(idtkb);
            if (!data.Any()) return null;
            var dataDict = data.GroupBy(x => new { x.Tiet, x.Ngay, x.Id_ca, x.Id_giao_vien }).ToDictionary(g => g.Key, g => g.First());
            using var workbook = new XLWorkbook();

            // Lấy danh sách các lớp từ data
            var giaovienList = data.Select(x => new { x.Ho_ho_dem, x.Ten_giao_vien, x.Id_giao_vien } ).Distinct().OrderBy(x => x.Ten_giao_vien).ToList();

            var firstRow = data.FirstOrDefault();
            var worksheet = workbook.Worksheets.Add("Toàn trường");
            // Tiêu đề trường
            worksheet.Cell(1, 1).Value = firstRow?.Ten_truong?.ToUpper() ?? "TRƯỜNG THCS";
            worksheet.Range(1, 1, 1, giaovienList.Count + 2).Merge();
            worksheet.Cell(1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(1, 1).Style.Font.SetBold(true).Font.SetFontSize(14);

            // Tiêu đề sheet
            worksheet.Cell(3, 1).Value = $"MA TRẬN GIÁO VIÊN";
            worksheet.Range(3, 1, 3, giaovienList.Count + 2).Merge();
            worksheet.Cell(3, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(3, 1).Style.Font.SetBold(true).Font.SetFontSize(12);

            // Headers
            worksheet.Range(5, 1, 6, 1).Merge().Value = "CA";
            worksheet.Range(5, 1, 6, 1).Merge().Style.Font.SetBold(true);
            worksheet.Range(5, 1, 6, 1).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range(5, 1, 6, 1).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Range(5, 1, 6, 1).Merge().Style.Fill.SetBackgroundColor(XLColor.LightGray);

            worksheet.Range(5, 2, 6, 2).Merge().Value = "THỨ";
            worksheet.Range(5, 2, 6, 2).Merge().Style.Font.SetBold(true);
            worksheet.Range(5, 2, 6, 2).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range(5, 2, 6, 2).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Range(5, 2, 6, 2).Merge().Style.Fill.SetBackgroundColor(XLColor.LightGray);

            worksheet.Range(5, 3, 6, 3).Merge().Value = "TIẾT";
            worksheet.Range(5, 3, 6, 3).Merge().Style.Font.SetBold(true);
            worksheet.Range(5, 3, 6, 3).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range(5, 3, 6, 3).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Range(5, 3, 6, 3).Merge().Style.Fill.SetBackgroundColor(XLColor.LightGray);


            // Headers cho các lớp
            for (int i = 0; i < giaovienList.Count; i++)
            {
                int startCol = 4 + (i * 2);
                var headerCell = worksheet.Range(5, startCol, 5, startCol + 1).Merge();
                headerCell.Value = $"{giaovienList[i].Ho_ho_dem} {giaovienList[i].Ten_giao_vien}";
                headerCell.Style.Font.SetBold(true);
                headerCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                headerCell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                //môn, lớp
                var monCell = worksheet.Cell(6, startCol);
                monCell.Value = "Môn";
                monCell.Style.Font.SetBold(true);
                monCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                monCell.Style.Fill.SetBackgroundColor(XLColor.LightGray);

                var lopCell = worksheet.Cell(6, startCol + 1);
                lopCell.Value = "Lớp";
                lopCell.Style.Font.SetBold(true);
                lopCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                lopCell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
            }

            int currentRow = 7;

            // Tạo dữ liệu cho từng CA
            var caNames = new[] { "SÁNG", "CHIỀU" };

            // Tạo dữ liệu cho từng thứ
            var thuNames = new[] { "THỨ\nHAI", "THỨ\nBA", "THỨ\nTƯ", "THỨ\nNĂM", "THỨ\nSÁU", "THỨ\nBẢY" };

            for (int caIndex = 0; caIndex < caNames.Length; caIndex++)
            {
                // Merge cột ca
                var caRange = worksheet.Range(currentRow, 1, currentRow + 29, 1);
                caRange.Merge();
                caRange.Value = caNames[caIndex];
                caRange.Style.Font.SetBold(true);
                caRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                caRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                caRange.Style.Alignment.SetWrapText(true);
                caRange.Style.Fill.SetBackgroundColor(XLColor.LightBlue);
                for (int ngayIndex = 0; ngayIndex < thuNames.Length; ngayIndex++)
                {

                    // Merge cột THỨ cho 5 tiết
                    var thuRange = worksheet.Range(currentRow, 2, currentRow + 4, 2);
                    thuRange.Merge();
                    thuRange.Value = thuNames[ngayIndex];
                    thuRange.Style.Font.SetBold(true);
                    thuRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    thuRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    thuRange.Style.Alignment.SetWrapText(true);

                    // 5 tiết trong ca
                    for (int tiet = 1; tiet <= 5; tiet++)
                    {
                        // Cột TIẾT
                        worksheet.Cell(currentRow, 3).Value = tiet.ToString();
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold(true);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                        // Điền dữ liệu cho từng lớp
                        for (int giaovienIndex = 0; giaovienIndex < giaovienList.Count; giaovienIndex++)
                        {
                            int idCa = caIndex + 1;
                            int ngay = ngayIndex + 1;
                            var tenGv = giaovienList[giaovienIndex].Ten_giao_vien;
                            int? idGv = giaovienList[giaovienIndex].Id_giao_vien;
                            var key = new { Tiet = tiet, Ngay = ngay, Id_ca = idCa, Id_giao_vien = idGv };
                            var lesson = dataDict.ContainsKey(key) ? dataDict[key] : null;
                            var cell = worksheet.Cell(currentRow, giaovienIndex + 4);

                            int startCol = 4 + (giaovienIndex * 2);

                            if (lesson != null)
                            {
                                // Cột Môn
                                var monCell = worksheet.Cell(currentRow, startCol);
                                monCell.Value = lesson.Ten_mon;
                                monCell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                monCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                // Cột lớp
                                var lopCell = worksheet.Cell(currentRow, startCol + 1);
                                lopCell.Value = $"{lesson.Ten_lop}";
                                lopCell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                lopCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            }
                        }
                        currentRow++;
                    }
                }
            }

            // Tạo border cho toàn bộ bảng
            int lastCol = 3 + (giaovienList.Count * 2);
            var dataRange = worksheet.Range(5, 1, currentRow - 1, lastCol);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Set chiều cao hàng
            //for (int i = 8; i <= 12; i++)
            //{
            //    worksheet.Row(i).Height = 45;
            //}
            //for (int i = 14; i <= 18; i++)
            //{
            //    worksheet.Row(i).Height = 45;
            //}

            // Set chiều rộng cột

            for (int i = 4; i <= lastCol; i++)
            {
                worksheet.Column(i).Width = 15;
            }
            for (int i = 2; i <= 8; i++)
            {
                worksheet.Column(i).Width = 20;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        public byte[] ExportExcel_MaTranToHopMon(int idtkb, int idDonvi)
        {
            var data = List_Tiet(idtkb);
            if (!data.Any()) return null;
            using var workbook = new XLWorkbook();
            var monIds = data.Select(x => x.Id_mon).Distinct().ToList();

            var thmList = _context.Monhoc_Tohopmon.Join(_context.DM_Banhoc, thm => thm.Id_ban, b => b.Id, (thm, b) => new {thm.Ten, thm.Id_mon_1, thm.Id_mon_2, thm.Id_mon_3, b.Id_don_vi}).Where(x=>x.Id_don_vi == idDonvi)
                                                    .Distinct().ToList();
            for (int k = 0; k < thmList.Count; k++)
            {
                // Lấy danh sách các lớp từ data
                var monListId = new List<int?>()
                {
                    thmList[k].Id_mon_1,
                    thmList[k].Id_mon_2,
                    thmList[k].Id_mon_3,
                }.ToList();

                var gvList = data.Where(c=> monListId.Contains(c.Id_mon)).Select(c => new { c.Ten_giao_vien, c.Ho_ho_dem, c.Id_giao_vien }).Distinct().OrderBy(x => x.Ten_giao_vien).ToList();
                var dataDetail = data.Where(c => monListId.Contains(c.Id_mon)).ToList();

                var firstRow = data.FirstOrDefault();
                var worksheet = workbook.Worksheets.Add($"{thmList[k].Ten}");
                // Tiêu đề trường
                worksheet.Cell(1, 1).Value = firstRow?.Ten_truong?.ToUpper() ?? "TRƯỜNG THCS";
                worksheet.Range(1, 1, 1, gvList.Count + 2).Merge();
                worksheet.Cell(1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(1, 1).Style.Font.SetBold(true).Font.SetFontSize(14);

                // Tiêu đề sheet
                worksheet.Cell(3, 1).Value = $"MA TRẬN TỔ HỢP MÔN";
                worksheet.Range(3, 1, 3, gvList.Count + 2).Merge();
                worksheet.Cell(3, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(3, 1).Style.Font.SetBold(true).Font.SetFontSize(12);

                // Headers
                worksheet.Range(5, 1, 6, 1).Merge().Value = "THỨ";
                worksheet.Range(5, 1, 6, 1).Merge().Style.Font.SetBold(true);
                worksheet.Range(5, 1, 6, 1).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(5, 1, 6, 1).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Range(5, 1, 6, 1).Merge().Style.Fill.SetBackgroundColor(XLColor.LightGray);

                worksheet.Range(5, 2, 6, 2).Merge().Value = "CA";
                worksheet.Range(5, 2, 6, 2).Merge().Style.Font.SetBold(true);
                worksheet.Range(5, 2, 6, 2).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(5, 2, 6, 2).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Range(5, 2, 6, 2).Merge().Style.Fill.SetBackgroundColor(XLColor.LightGray);

                worksheet.Range(5, 3, 6, 3).Merge().Value = "TIẾT";
                worksheet.Range(5, 3, 6, 3).Merge().Style.Font.SetBold(true);
                worksheet.Range(5, 3, 6, 3).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(5, 3, 6, 3).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Range(5, 3, 6, 3).Merge().Style.Fill.SetBackgroundColor(XLColor.LightGray);


                // Headers
                for (int i = 0; i < gvList.Count; i++)
                {
                    int startCol = 4 + (i * 2);
                    //header cho lớp
                    var headerCell = worksheet.Range(5, startCol, 5, startCol + 1);
                    headerCell.Merge();
                    headerCell.Value = $"{gvList[i].Ho_ho_dem} {gvList[i].Ten_giao_vien}";
                    headerCell.Style.Font.SetBold(true);
                    headerCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    headerCell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                    //môn, lớp
                    var monCell = worksheet.Cell(6, startCol);
                    monCell.Value = "Môn";
                    monCell.Style.Font.SetBold(true);
                    monCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    monCell.Style.Fill.SetBackgroundColor(XLColor.LightGray);

                    var lopCell = worksheet.Cell(6, startCol + 1);
                    lopCell.Value = "Lớp";
                    lopCell.Style.Font.SetBold(true);
                    lopCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    lopCell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                }

                int currentRow = 7;

                // Tạo dữ liệu cho từng CA
                var caNames = new[] { "SÁNG", "CHIỀU" };

                // Tạo dữ liệu cho từng thứ
                var thuNames = new[] { "THỨ\nHAI", "THỨ\nBA", "THỨ\nTƯ", "THỨ\nNĂM", "THỨ\nSÁU", "THỨ\nBẢY" };


                for (int ngayIndex = 0; ngayIndex < thuNames.Length; ngayIndex++)
                {

                    // Merge cột THỨ cho 5 tiết
                    var thuRange = worksheet.Range(currentRow, 1, currentRow + 9, 1);
                    thuRange.Merge();
                    thuRange.Value = thuNames[ngayIndex];
                    thuRange.Style.Font.SetBold(true);
                    thuRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    thuRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    thuRange.Style.Alignment.SetWrapText(true);
                    thuRange.Style.Fill.SetBackgroundColor(XLColor.LightBlue);
                    for (int caIndex = 0; caIndex < caNames.Length; caIndex++)
                    {
                        // Merge cột ca
                        var caRange = worksheet.Range(currentRow, 2, currentRow + 4, 2);
                        caRange.Merge();
                        caRange.Value = caNames[caIndex];
                        caRange.Style.Font.SetBold(true);
                        caRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        caRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        caRange.Style.Alignment.SetWrapText(true);

                        // 5 tiết trong ca
                        for (int tiet = 1; tiet <= 5; tiet++)
                        {
                            // Cột TIẾT
                            worksheet.Cell(currentRow, 3).Value = tiet.ToString();
                            worksheet.Cell(currentRow, 3).Style.Font.SetBold(true);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                            // Điền dữ liệu cho từng lớp
                            for (int gvIndex = 0; gvIndex < gvList.Count; gvIndex++)
                            {
                                int idCa = caIndex + 1;
                                int ngay = ngayIndex + 1;
                                int? idGv = gvList[gvIndex].Id_giao_vien;
                                var lesson = dataDetail.FirstOrDefault(x => x.Tiet == tiet && x.Ngay == ngay && x.Id_ca == idCa && x.Id_giao_vien == idGv);
                                var cell = worksheet.Cell(currentRow, gvIndex + 4);

                                int startCol = 4 + (gvIndex * 2);

                                if (lesson != null)
                                {
                                    // Cột Môn
                                    var monCell = worksheet.Cell(currentRow, startCol);
                                    monCell.Value = lesson.Ten_mon;
                                    monCell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                    monCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                    // Cột lớp
                                    var lopCell = worksheet.Cell(currentRow, startCol + 1);
                                    lopCell.Value = $"{lesson.Ten_lop}";
                                    lopCell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                    lopCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                }
                            }
                            currentRow++;
                        }
                    }

                }

                // Tạo border cho toàn bộ bảng
                int lastCol = 3 + (gvList.Count * 2);
                var dataRange = worksheet.Range(5, 1, currentRow - 1, lastCol);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Set chiều cao hàng
                //for (int i = 8; i <= 12; i++)
                //{
                //    worksheet.Row(i).Height = 45;
                //}
                //for (int i = 14; i <= 18; i++)
                //{
                //    worksheet.Row(i).Height = 45;
                //}

                // Set chiều rộng cột

                for (int i = 4; i <= lastCol; i++)
                {
                    worksheet.Column(i).Width = 15;
                }
                for (int i = 2; i <= 8; i++)
                {
                    worksheet.Column(i).Width = 20;
                }
            }
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
