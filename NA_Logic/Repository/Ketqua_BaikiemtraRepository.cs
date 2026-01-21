using ClosedXML.Excel;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Ketqua_BaikiemtraRepository: IKetqua_BaikiemtraRepository
    {

        private readonly NA_DbContext _context;
        public Ketqua_BaikiemtraRepository(NA_DbContext context) { 
            _context = context;
        }
        public byte[] ExportMauExcel(int idlop)
        {
            var data = (from hl in _context.Hocsinh_Lopon
                        join hs in _context.DM_Hocsinh on hl.Id_hoc_sinh equals hs.Id
                        where hl.Id_lop_on == idlop
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

            worksheet.Cell(1, 3).Value = "Tên học sinh";
            worksheet.Cell(1, 3).Style.Font.SetBold(true);
            worksheet.Cell(1, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(1, 3).Style.Fill.SetBackgroundColor(XLColor.LightGray);

            worksheet.Cell(1, 4).Value = "Điểm";
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
            worksheet.Column(2).Width = 8;
            worksheet.Column(3).Width = 13;
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        public bool  Add( int id_bai)
        {
            try
            {

                var paramIdBai = new SqlParameter("Id_bai", SqlDbType.Int) { Value = id_bai };
                
                var paramMessage = new SqlParameter("Message", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };
                var results = _context.Database.ExecuteSqlRaw("EXEC [InsertHocSinh_KetQua] @Id_bai, @Message OUTPUT ", 
                     paramIdBai,  paramMessage);
                var Message = paramMessage.Value?.ToString() ?? "";

                if (Message == "Thành công")
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public (bool success, string mess) Import(IFormFile file, int id_bai)
        {
            try
            {

                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                var rows = worksheet.RangeUsed().RowsUsed().Skip(1).ToList();

                if (!rows.Any())
                    return (false, "File không có dữ liệu");
                var headers = new[] { "STT", "Mã học sinh", "Tên học sinh", "Điểm" };
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

                    if (string.IsNullOrEmpty(ma))
                        return (false, "Mã học sinh không được để trống");

                    if (string.IsNullOrEmpty(ten))
                        return (false, "Họ tên không được để trống");

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
                dataTable.Columns.Add("Ma_hoc_sinh", typeof(string));
                dataTable.Columns.Add("Ten_hoc_sinh", typeof(string));
                dataTable.Columns.Add("Diem", typeof(decimal));

                foreach (var row in rows)
                {
                    dataTable.Rows.Add(
                        stt,
                        row.Cell(2).GetValue<string>()?.Trim(),
                        row.Cell(3).GetValue<string>()?.Trim(),
                        row.Cell(4).GetValue<string>()?.Trim()
                    );
                    stt++;
                }

                var paramIdBai = new SqlParameter("Id_bai", SqlDbType.Int) { Value = id_bai };
                var dataParam = new SqlParameter("@Data", SqlDbType.Structured)
                {
                    TypeName = "dbo.KetQuaBaiKiemTra",
                    Value = dataTable
                };
                var paramMessage = new SqlParameter("Message", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };
                var results = _context.Database.ExecuteSqlRaw("EXEC [ImportKetQuaBaiKiemTra] @Id_bai, @Data, @Message OUTPUT ", 
                     paramIdBai, dataParam, paramMessage);
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
        public List<KetQua_Baikiemtra_List> Getlist(int idbai)
        {
            try
            {
                var data = (from kq in _context.KetQua_Baikiemtra
                            join hs in _context.DM_Hocsinh on kq.Id_hoc_sinh equals hs.Id
                            where kq.Id_bai_kiem_tra == idbai
                            select new KetQua_Baikiemtra_List
                            {
                                Id = kq.Id,
                                Ma_hoc_sinh = hs.Ma,
                                Ten_hoc_sinh = hs.Ten,
                                Diem = kq.Diem_so
                            }).ToList();
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool UpdateDiem(List<KetQua_Baikiemtra_List> listKetQua)
        {
            try
            {
                var updateList = listKetQua.Select(x => new KetQua_Baikiemtra
                {
                    Id = x.Id,
                    Diem_so = x.Diem
                }).ToList();
                _context.BulkUpdate(updateList, new BulkConfig
                {
                    PropertiesToInclude = new List<string> { nameof(KetQua_Baikiemtra.Diem_so) }
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
