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
    public class FileImportRepository : IFileImportRepository
    {
        private readonly NA_DbContext _context;
        public FileImportRepository(NA_DbContext context)
        {
            _context = context;
        }
        public string ImportAccessConvertExcelToJson(Stream stream)
        {
            var result = new Dictionary<string, List<Dictionary<string, object>>>();
            var diemTruongLookup = new Dictionary<string, string>();
            var tkbLookup = new Dictionary<string, string>();

            using (var workbook = new XLWorkbook(stream))
            {
                foreach (var worksheet in workbook.Worksheets)
                {
                    var sheetNameLower = worksheet.Name.ToLower();

                    if (sheetNameLower.Contains("điểm") || sheetNameLower.Contains("diem"))
                    {
                        var range = worksheet.RangeUsed();
                        if (range == null || range.RowCount() <= 1) continue;

                        int maCol = -1, tenCol = -1;
                        for (int col = 1; col <= range.ColumnCount(); col++)
                        {
                            var header = range.Cell(1, col).GetString().ToLower();
                            if (header.Contains("mã")) maCol = col;
                            if (header.Contains("tên")) tenCol = col;
                        }

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
                    }

                    else if (sheetNameLower.Contains("tkb") || sheetNameLower.Contains("thời khóa biểu"))
                    {
                        var range = worksheet.RangeUsed();
                        if (range == null || range.RowCount() <= 1) continue;

                        int headerRow = 1;
                        var firstCell = range.Cell(1, 1).GetString().ToLower();
                        if (firstCell.Contains("mã tkb") || firstCell.Contains("ma tkb"))
                        {
                            headerRow = 2;
                        }

                        int maCol = -1, tenCol = -1;
                        for (int col = 1; col <= range.ColumnCount(); col++)
                        {
                            var header = range.Cell(headerRow, col).GetString().ToLower();
                            if (header.Contains("mã")) maCol = col;
                            if (header.Contains("tên")) tenCol = col;
                        }

                        if (maCol > 0 && tenCol > 0)
                        {
                            for (int row = headerRow + 1; row <= range.RowCount(); row++)
                            {
                                var ma = range.Cell(row, maCol).GetString().Trim();
                                var ten = range.Cell(row, tenCol).GetString().Trim();
                                if (!string.IsNullOrEmpty(ma))
                                    tkbLookup[ma] = ten;
                            }
                        }
                    }
                }

                foreach (var worksheet in workbook.Worksheets)
                {
                    var sheetData = new List<Dictionary<string, object>>();
                    var range = worksheet.RangeUsed();

                    if (range == null || range.RowCount() <= 1) continue;

                    var sheetNameLower = worksheet.Name.ToLower();
                    int headerRow = 1;
                    string maTKB = null;

                    var firstCell = range.Cell(1, 1).GetString().ToLower();
                    if (firstCell.Contains("mã tkb") || firstCell.Contains("ma tkb"))
                    {
                        headerRow = 3;
                        maTKB = range.Cell(1, 2).GetString().Trim();
                    }

                    var headers = new List<string>();
                    int maDiemTruongCol = -1;
                    int maTkbCol = -1;

                    for (int col = 1; col <= range.ColumnCount(); col++)
                    {
                        var header = range.Cell(headerRow, col).GetString().Trim();
                        var headerLower = header.ToLower();

                        if (headerLower.Contains("mã") && headerLower.Contains("điểm"))
                            maDiemTruongCol = col;

                        if (headerLower.Contains("mã") && headerLower.Contains("tkb"))
                            maTkbCol = col;

                        if (string.IsNullOrEmpty(header))
                            header = $"Column{col}";

                        headers.Add(ConvertToPascalCase(header));
                    }

                    for (int row = headerRow + 1; row <= range.RowCount(); row++)
                    {
                        var rowData = new Dictionary<string, object>();
                        bool isEmptyRow = true;

                        for (int col = 1; col <= range.ColumnCount(); col++)
                        {
                            var cell = range.Cell(row, col);
                            object value;

                            if (cell.DataType == XLDataType.Number)
                            {
                                double numValue = cell.GetDouble();
                                if (numValue == Math.Floor(numValue))
                                    value = (int)numValue;
                                else
                                    value = numValue.ToString("0.#####");
                            }
                            else
                                value = cell.GetString().Trim();

                            if (!string.IsNullOrEmpty(value.ToString()))
                                isEmptyRow = false;

                            rowData[headers[col - 1]] = value;
                        }

                        if (isEmptyRow) continue;

                        var firstColValue = rowData[headers[0]].ToString().ToLower();
                        if (firstColValue.Contains("tiết") || firstColValue.Contains("tiet"))
                            continue;

                        // Thêm tên điểm trường
                        if (maDiemTruongCol > 0)
                        {
                            var ma = range.Cell(row, maDiemTruongCol).GetString().Trim();
                            if (diemTruongLookup.ContainsKey(ma))
                                rowData["TenDiemTruong"] = diemTruongLookup[ma];
                        }

                        // Thêm tên TKB 
                        if (maTkbCol > 0)
                        {
                            var ma = range.Cell(row, maTkbCol).GetString().Trim();
                            if (tkbLookup.ContainsKey(ma))
                                rowData["TenTKB"] = tkbLookup[ma];
                        }

                        // Thêm mã TKB
                        if (!string.IsNullOrEmpty(maTKB))
                        {
                            rowData["MaTKB"] = maTKB;
                            if (tkbLookup.ContainsKey(maTKB))
                                rowData["TenTKB"] = tkbLookup[maTKB];
                        }

                        sheetData.Add(rowData);
                    }

                    result[ConvertToPascalCase(worksheet.Name)] = sheetData;
                }
            }

            return JsonConvert.SerializeObject(result);
        }
        public string ImportBackUpConvertExcelToJson(Stream stream)
        {
            var result = new Dictionary<string, List<Dictionary<string, object>>>();

            using (var workbook = new XLWorkbook(stream))
            {
                foreach (var worksheet in workbook.Worksheets)
                {
                    var sheetData = new List<Dictionary<string, object>>();
                    var range = worksheet.RangeUsed();

                    if (range == null || range.RowCount() <= 1)
                    {
                        result[ConvertToPascalCase(worksheet.Name)] = sheetData;
                        continue;
                    }

                    var headers = new List<string>();
                    for (int col = 1; col <= range.ColumnCount(); col++)
                    {
                        var header = range.Cell(1, col).GetString().Trim();
                        if (string.IsNullOrEmpty(header))
                            header = $"Column{col}";
                        headers.Add(ConvertToPascalCase(header));
                    }

                    for (int row = 2; row <= range.RowCount(); row++)
                    {
                        var rowData = new Dictionary<string, object>();
                        bool isEmptyRow = true;

                        for (int col = 1; col <= range.ColumnCount(); col++)
                        {
                            var cell = range.Cell(row, col);
                            object value;

                            if (cell.DataType == XLDataType.Number)
                            {
                                double numValue = cell.GetDouble();
                                if (numValue == Math.Floor(numValue))
                                    value = (int)numValue;
                                else
                                    value = numValue;
                            }
                            else if (cell.DataType == XLDataType.DateTime)
                            {
                                value = cell.GetDateTime();
                            }
                            else if (cell.DataType == XLDataType.Boolean)
                            {
                                value = cell.GetBoolean();
                            }
                            else
                            {
                                value = cell.GetString().Trim();
                            }

                            if (!string.IsNullOrEmpty(value?.ToString()))
                                isEmptyRow = false;

                            rowData[headers[col - 1]] = value;
                        }

                        if (!isEmptyRow)
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
                var json = ImportAccessConvertExcelToJson(stream);
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
        public bool ImportBackUpToDb(Stream stream, int idDonvi)
        {
            try
            {
                var json = ImportBackUpConvertExcelToJson(stream);
                var paramJson = new SqlParameter("json", SqlDbType.NVarChar, -1) { Value = json };
                var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int) { Value = idDonvi };
                var paramMessage = new SqlParameter("ErrorMessage", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };

                _context.Database.ExecuteSqlRaw("EXEC [InsertFromBackUp] @json, @idDonvi, @ErrorMessage OUTPUT", paramJson, paramIdDonvi, paramMessage);

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
