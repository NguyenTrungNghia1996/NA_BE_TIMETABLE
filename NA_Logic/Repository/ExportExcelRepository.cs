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

        public byte[] ExportAllDataToExcel(int idDonVi, int idTkb)
        {
            var queries = new[]
            {
                new { Query = $"select * from DM_Donvi where Id = {idDonVi}", SheetName = "DM_Donvi" },
                new { Query = $"select * from DM_Diemtruong where Id_don_vi = {idDonVi}", SheetName = "DM_Diemtruong" },
                new { Query = $"select * from DM_Banhoc where Id_don_vi = {idDonVi}", SheetName = "DM_Banhoc" },
                new { Query = $@"select gv.Id, Isnull(gvbd.Chi_day_mot_buoi, 0) as Chi_day_mot_buoi, isnull(gvbd.So_tiet_toi_da, 0) as So_tiet_toi_da 
                               from Giaovien_Buoiday gvbd right join DM_Giaovien gv on gv.Id = gvbd.Id_giao_vien 
                               where gv.Id_don_vi = {idDonVi}", SheetName = "Giaovien_Buoiday" },
                new { Query = $@"select gvttx.Id_giao_vien, gvttx.Id_ca, gvttx.Ngay, gvttx.Tiet 
                               from Giaovien_Tiettranhxep gvttx join DM_Giaovien gv on gv.Id = gvttx.Id_giao_vien 
                               where gv.Id_don_vi = {idDonVi}", SheetName = "Giaovien_Tiettranh" },
                new { Query = $"select Id, Hoc_cach_ngay, Xep_thanh_cap, So_tiet_toi_da_mot_ca, So_tiet_toi_da_hai_ca from DM_Monhoc where Id_don_vi = {idDonVi}", SheetName = "DM_Monhoc" },
                new { Query = $@"select ttx.Id_mon, ttx.Id_ca, ttx.Thu as Ngay, ttx.Tiet 
                               from Tiet_tranh_xep ttx join DM_Monhoc mh on mh.Id = ttx.Id_mon 
                               where mh.Id_don_vi = {idDonVi}", SheetName = "Tiet_tranh_xep" },
                new { Query = $@"select tcd.Id_mon, tcd.Id_ca, tcd.Ngay, tcd.Tiet, lh.Id 
                               from Tiet_co_dinh tcd 
                               join DM_Khoilop kl on tcd.Id_khoi_lop = kl.Id
                               join DM_Lophoc lh on lh.Id_khoi = kl.Id
                               join DM_Monhoc mh on tcd.Id_mon = mh.Id
                               join Lophoc_Monhoc lm on lh.Id = lm.Id_lop and lm.Id_mon = mh.Id
                               where lh.Id_don_vi = {idDonVi} and mh.Id_don_vi = {idDonVi}", SheetName = "Tiet_co_dinh" },
                new { Query = $@"select ph.Id, ph.Id_Loai_phong_hoc, ph.Khong_kiem_tra_xung_dot 
                               from DM_Phonghoc ph join DM_Diemtruong dt on ph.Id_Diem_truong = dt.Id 
                               where dt.Id_don_vi = {idDonVi}", SheetName = "DM_Phonghoc" },
                new { Query = $@"select tb.Id_phong, tb.Id_ca, tb.Thu as Ngay, tb.Tiet 
                               from Tiet_ban tb join DM_Phonghoc ph on tb.Id_phong = ph.Id 
                               join DM_Diemtruong dt on ph.Id_Diem_truong = dt.Id 
                               where dt.Id_don_vi = {idDonVi}", SheetName = "Tiet_ban" },
                new { Query = $@"select thm.* from Monhoc_Tohopmon thm 
                               join DM_Banhoc bh on bh.Id = thm.Id_ban 
                               where bh.Id_don_vi = {idDonVi}", SheetName = "Monhoc_Tohopmon" },
                new { Query = $"select Id, Id_ban, Id_khoi from DM_Lophoc where Id_don_vi = {idDonVi}", SheetName = "DM_Lophoc" },
                new { Query = $@"select lhtn.Id_lop, lhtn.Id_ca, lhtn.Ngay, lhtn.Tiet 
                               from Lophoc_Tietnghi lhtn join DM_Lophoc lh on lh.Id = lhtn.Id_lop 
                               where lh.Id_don_vi = {idDonVi}", SheetName = "Lophoc_Tietnghi" },
                new { Query = $@"select lm.Id_lop, lm.Id_mon from Lophoc_Monhoc lm 
                               join DM_Lophoc lh on lm.Id_lop = lh.Id 
                               where lh.Id_don_vi = {idDonVi}", SheetName = "Lophoc_Monhoc" },
                new { Query = $@"select lm.Id_lop, lm.Id_mon, lm.Id_ca, lm.Ngay, lm.Tiet 
                               from Lophoc_Monhoc_Tiettranhxep lm join DM_Lophoc lh on lm.Id_lop = lh.Id 
                               where Id_don_vi = {idDonVi}", SheetName = "LM_Tiettranhxep" },
                new { Query = $@"select Id_mon, Id_khoi, Id_ban from Monhoc_Khoilop mk 
                               join DM_Monhoc mh on mh.Id = mk.Id_mon 
                               where Id_don_vi = {idDonVi}", SheetName = "Monhoc_Khoilop" },
                new { Query = $@"select mkttx.Id_mon, mkttx.Id_khoi, mkttx.Id_ban, mkttx.Id_ca, mkttx.Ngay, mkttx.Tiet 
                               from Monhoc_Khoilop_Tiettranhxep mkttx 
                               join DM_Monhoc mh on mh.Id = mkttx.Id_mon 
                               where Id_don_vi = {idDonVi}", SheetName = "MK_Tiettranhxep" },
                new { Query = $"select * from Danhsach_Thoikhoabieu where Id_don_vi = {idDonVi}", SheetName = "DS_Thoikhoabieu" },
                new { Query = $@"SELECT ct.Id, ct.Id_tkb, ct.Id_ca, ct.Id_giao_vien, gv.Ten AS Ten_giao_vien,
                               ct.Id_lop, lh.Ten AS Ten_lop, ct.Id_mon, mh.Ten AS Ten_mon,
                               ct.Id_phong, ph.Ten AS Ten_phong, gv.Id_don_vi, ct.Tiet_thu_may,
                               ct.Ngay, ct.Tiet, ct.Khoa
                               FROM Chitiet_Thoikhoabieu ct
                               left JOIN DM_Giaovien gv ON gv.Id = ct.Id_giao_vien
                               left JOIN DM_Lophoc lh ON lh.Id = ct.Id_lop
                               left JOIN DM_Monhoc mh ON mh.Id = ct.Id_mon
                               left JOIN DM_Phonghoc ph ON ph.Id = ct.Id_phong
                               left JOIN DM_Donvi dv ON gv.Id_don_vi = dv.Id
                               WHERE ct.Id_tkb = {idTkb}", SheetName = "Chitiet_TKB" }
            };

            using var workbook = new XLWorkbook();
            bool hasData = false;

            foreach (var queryInfo in queries)
            {
                try
                {
                    DataTable dt = ExecuteQuery(queryInfo.Query);

                    if (dt.Rows.Count == 0)
                        continue;

                    hasData = true;
                    var worksheet = workbook.Worksheets.Add(SanitizeSheetName(queryInfo.SheetName));

                    // Thêm tiêu đề sheet
                    worksheet.Cell(1, 1).Value = queryInfo.SheetName;
                    worksheet.Range(1, 1, 1, dt.Columns.Count).Merge();
                    worksheet.Cell(1, 1).Style.Font.SetBold(true).Font.SetFontSize(14);
                    worksheet.Cell(1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(1, 1).Style.Fill.SetBackgroundColor(XLColor.LightBlue);

                    // Thêm headers (tên cột) ở dòng 3
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        var headerCell = worksheet.Cell(3, i + 1);
                        headerCell.Value = dt.Columns[i].ColumnName;
                        headerCell.Style.Font.SetBold(true);
                        headerCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        headerCell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                    }

                    // Thêm dữ liệu từ dòng 4
                    int currentRow = 4;
                    foreach (DataRow row in dt.Rows)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            var cell = worksheet.Cell(currentRow, i + 1);
                            cell.Value = row[i]?.ToString() ?? "";
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        }
                        currentRow++;
                    }

                    // Tạo border cho toàn bộ bảng dữ liệu
                    var dataRange = worksheet.Range(3, 1, currentRow - 1, dt.Columns.Count);
                    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();
                }
                catch (Exception ex)
                {
                    // Log error nhưng tiếp tục xử lý các sheet khác
                    Console.WriteLine($"Lỗi khi xử lý sheet {queryInfo.SheetName}: {ex.Message}");
                }
            }

            if (!hasData)
                return null;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = 120; // 2 phút timeout

                _context.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    dt.Load(result);
                }
            }

            return dt;
        }

        private string SanitizeSheetName(string name)
        {
            // Excel sheet name giới hạn 31 ký tự và không chứa: \ / ? * [ ]
            char[] invalidChars = { '\\', '/', '?', '*', '[', ']' };
            string sanitized = name;

            foreach (char c in invalidChars)
            {
                sanitized = sanitized.Replace(c, '_');
            }

            if (sanitized.Length > 31)
            {
                sanitized = sanitized.Substring(0, 31);
            }

            return sanitized;
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
                            var information = new List<string>();
                            information.Add(lesson.Ten_mon);
                            information.Add(lesson.Ten_giao_vien);
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
                            information.Add(lesson.Ten_giao_vien);
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
    }
}
