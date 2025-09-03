using ClosedXML.Excel;
using ClosedXML.Excel;
using NA_Logic.IRepository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NA_Logic.Repository
{
    public class FileImportRepository: IFileImportRepository
    {
        public string ConvertExcelToJson(Stream stream)
        {
            var result = new Dictionary<string, List<Dictionary<string, object>>>();

            using (var workbook = new XLWorkbook(stream))
            {
                foreach (var worksheet in workbook.Worksheets)
                {
                    var sheetName = worksheet.Name;
                    var sheetData = new List<Dictionary<string, object>>();

                    var range = worksheet.RangeUsed();
                    if (range == null) continue;

                    var rowCount = range.RowCount();
                    var colCount = range.ColumnCount();

                    if (rowCount > 1)
                    {
                        // Headers
                        var headers = new List<string>();
                        for (int col = 1; col <= colCount; col++)
                        {
                            headers.Add(range.Cell(1, col).GetString().Trim());
                        }

                        // Data rows
                        for (int row = 2; row <= rowCount; row++)
                        {
                            var rowData = new Dictionary<string, object>();

                            for (int col = 1; col <= colCount; col++)
                            {
                                var header = headers[col - 1];
                                var cell = range.Cell(row, col);

                                object value = cell.DataType == XLDataType.Number ?
                                              (int)cell.GetDouble() :
                                              cell.GetString().Trim();

                                rowData[header] = value;
                            }

                            sheetData.Add(rowData);
                        }
                    }

                    result[sheetName] = sheetData;
                }
            }

            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }
    }
}
