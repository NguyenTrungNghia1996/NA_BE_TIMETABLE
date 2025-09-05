using ClosedXML.Excel;
using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Logic.IRepository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace NA_Logic.Repository
{
    public class FileImportRepository: IFileImportRepository
    {
        private readonly NA_DbContext _context;
        public FileImportRepository(NA_DbContext context)
        {
            _context = context;
        }
        public string ConvertExcelToJson(Stream stream)
        {
            var result = new Dictionary<string, List<Dictionary<string, object>>>();
            var diemTruongLookup = new Dictionary<string, string>(); // Mã -> Tên

            using (var workbook = new XLWorkbook(stream))
            {
                // Bước 1: Tìm và đọc sheet điểm trường trước
                foreach (var worksheet in workbook.Worksheets)
                {
                    if (worksheet.Name.ToLower().Contains("điểm") || worksheet.Name.ToLower().Contains("diem"))
                    {
                        var range = worksheet.RangeUsed();
                        if (range == null || range.RowCount() <= 1) continue;

                        // Tìm cột mã và tên
                        int maCol = -1, tenCol = -1;
                        for (int col = 1; col <= range.ColumnCount(); col++)
                        {
                            var header = range.Cell(1, col).GetString().ToLower();
                            if (header.Contains("mã")) maCol = col;
                            if (header.Contains("tên")) tenCol = col;
                        }

                        // Đọc lookup data
                        if (maCol > 0 && tenCol > 0)
                        {
                            for (int row = 2; row <= range.RowCount(); row++)
                            {
                                var ma = range.Cell(row, maCol).GetString().Trim();
                                var ten = range.Cell(row, tenCol).GetString().Trim();
                                if (!string.IsNullOrEmpty(ma))
                                    diemTruongLookup[ma] = ten;
                            }
                        }
                        break;
                    }
                }

                // Bước 2: Xử lý tất cả sheet
                foreach (var worksheet in workbook.Worksheets)
                {
                    var sheetData = new List<Dictionary<string, object>>();
                    var range = worksheet.RangeUsed();

                    if (range == null || range.RowCount() <= 1) continue;

                    // Lấy headers
                    var headers = new List<string>();
                    int maDiemTruongCol = -1;

                    for (int col = 1; col <= range.ColumnCount(); col++)
                    {
                        var header = range.Cell(1, col).GetString().Trim();
                        var headerLower = header.ToLower();

                        // Tìm cột mã điểm trường
                        if (headerLower.Contains("mã") && headerLower.Contains("điểm"))
                            maDiemTruongCol = col;

                        headers.Add(ConvertToPascalCase(header));
                    }

                    // Đọc data
                    for (int row = 2; row <= range.RowCount(); row++)
                    {
                        var rowData = new Dictionary<string, object>();

                        for (int col = 1; col <= range.ColumnCount(); col++)
                        {
                            var cell = range.Cell(row, col);
                            object value;

                            if (cell.DataType == XLDataType.Number)
                                value = (int)cell.GetDouble();
                            else
                                value = cell.GetString().Trim();

                            rowData[headers[col - 1]] = value;
                        }

                        // Thêm tên điểm trường nếu có
                        if (maDiemTruongCol > 0)
                        {
                            var ma = range.Cell(row, maDiemTruongCol).GetString().Trim();
                            if (diemTruongLookup.ContainsKey(ma))
                                rowData["TenDiemTruong"] = diemTruongLookup[ma];
                        }

                        sheetData.Add(rowData);
                    }

                    result[ConvertToPascalCase(worksheet.Name)] = sheetData;
                }
            }

            return JsonConvert.SerializeObject(result);
        }

        private string ConvertToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            input = Regex.Replace(input, "[àáạảãâầấậẩẫăằắặẳẵ]", "a");
            input = Regex.Replace(input, "[èéẹẻẽêềếệểễ]", "e");
            input = Regex.Replace(input, "[ìíịỉĩ]", "i");
            input = Regex.Replace(input, "[òóọỏõôồốộổỗơờớợởỡ]", "o");
            input = Regex.Replace(input, "[ùúụủũưừứựửữ]", "u");
            input = Regex.Replace(input, "[ỳýỵỷỹ]", "y");
            input = Regex.Replace(input, "[đĐ]", "d");
            input = Regex.Replace(input, "[/]", "_");

            return string.Join("", input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(w => char.ToUpper(w[0]) + w.Substring(1).ToLower()));
        }

        public bool ImportExcelToDb(Stream stream, int idDonvi)
        {
            try
            {
                var json = ConvertExcelToJson(stream); 
                var paramJson = new SqlParameter("json", SqlDbType.NVarChar, -1) { Value = json };
                var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int) { Value = idDonvi };
                var paramMessage = new SqlParameter("ErrorMessage", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };

                _context.Database.ExecuteSqlRaw("EXEC [InsertFromAccess] @json, @idDonvi, @ErrorMessage OUTPUT", paramJson, paramIdDonvi, paramMessage);

                var errorMessage = paramMessage.Value?.ToString() ?? "";
                return errorMessage == "success";
            }
            catch
            {
                return false;
            }
        }
    }
}
