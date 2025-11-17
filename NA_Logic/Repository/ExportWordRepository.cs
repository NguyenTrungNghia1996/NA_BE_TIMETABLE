using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NA_Logic.Repository
{
    public class ExportWordRepository: IExportWordRepository
    {
        private readonly NA_DbContext _dbContext;
        public ExportWordRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public PhieubaogiangDto GetList_Chitiet(int idpbg, int idDonvi)
        {
            try
            {
                var paramIdpbg = new SqlParameter("idPBG", SqlDbType.Int)
                {
                    Value = idpbg
                };
                var chitiet = _dbContext.Set<Chitiet_Phieubaogiang_List>().FromSqlRaw("EXEC [GetList_ChitietPbg] @idPBG", paramIdpbg).ToList();
                if (chitiet == null)
                    return null;
                var maxNgay = chitiet.Max(c => c.Ngay);
                var minNgay = chitiet.Min(c => c.Ngay);
                var listNgayNghi = _dbContext.DM_Ngaynghi.Where(n => n.Tu_ngay <= maxNgay && n.Den_ngay >= minNgay).ToList();

                var tuNgay = chitiet.First().Tu_Ngay.Date;
                var denNgay = chitiet.First().Den_Ngay.Date;
                var so_ngay = (denNgay - tuNgay).Days + 1;

                var dsCa = _dbContext.Ca_Donvi.Where(c => c.Id_don_vi == idDonvi).ToList();
                // Lấy danh sách ngày từ enum
                var dsNgay = Enum.GetValues<Ngay>().Take(so_ngay).ToList();
                // Lấy danh sách tiết từ enum
                var dsTiet = Enum.GetValues<Tiet>().ToList();

                var result = new PhieubaogiangDto
                {
                    Ten_giao_vien = chitiet.FirstOrDefault()?.Ten_giao_vien,
                    Tuan = chitiet.First().Tuan,
                    Tu_ngay = chitiet.First().Tu_Ngay,
                    Den_ngay = chitiet.First().Den_Ngay,
                    Lich_theo_ngay = dsNgay.Select((ngayEnum, index) =>
                    {
                        var ngayHienTai = tuNgay.AddDays(index);

                        var laNgayNghi = listNgayNghi.Any(n => ngayHienTai >= n.Tu_ngay.Date && ngayHienTai <= n.Den_ngay.Date);
                        var duLieuNgay = chitiet.Where(c => c.Ngay.Date == ngayHienTai).ToList();

                        return new NgayhocDto
                        {
                            Ngay = $"{ngayEnum.GetDisplayName()} ({ngayHienTai:dd / MM})",
                            Buoi_hoc = dsCa.Select(caInfo =>
                            {
                                var duLieuCa = duLieuNgay.Where(c => c.Id_ca == caInfo.Id_ca_hoc).ToList();
                                var cacTietCuaCa = dsTiet.Take(caInfo.So_tiet).ToList();

                                return new BuoihocDto
                                {
                                    Ten_buoi = caInfo.Id_ca_hoc == 1 ? "Sáng" : caInfo.Id_ca_hoc == 2 ? "Chiều" : "Không xác định",
                                    Cac_tiet_hoc = cacTietCuaCa.Select(tiet =>
                                    {
                                        var soTiet = (int)tiet;
                                        var duLieuTiet = duLieuCa.FirstOrDefault(c => c.Tiet == soTiet);
                                        return new Thongtin_tietDto
                                        {
                                            Tiet_tkb = soTiet,
                                            Tiet_ppct = duLieuTiet != null ? duLieuTiet.Thu_tu_tiet : 0,
                                            Ten_lop = duLieuTiet != null ? duLieuTiet.Ten_lop : "",
                                            Ten_mon = duLieuTiet != null ? duLieuTiet.Ten_mon : "",
                                            Phan_mon = duLieuTiet != null ? duLieuTiet.Phan_mon : "",
                                            Ten_bai = laNgayNghi ? "Nghỉ" : (duLieuTiet != null ? duLieuTiet.Ten_bai : ""),
                                            Ghi_chu = duLieuTiet != null ? duLieuTiet.Ghi_chu : ""
                                        };
                                    }).ToList()
                                };
                            }).ToList()
                        };
                    }).ToList()
                };
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private void SetCellText(TableCell cell, string text)
        {
            var para = cell.Descendants<Paragraph>().FirstOrDefault() ?? cell.AppendChild(new Paragraph());
            var run = para.Descendants<Run>().FirstOrDefault() ?? para.AppendChild(new Run());
            var textElem = run.Descendants<Text>().FirstOrDefault() ?? run.AppendChild(new Text());
            textElem.Text = text;
        }
        private void MergeCells(Table table, int startRow, int rowCount, int colIndex)
        {
            var rows = table.Descendants<TableRow>().ToList();
            for (int i = 0; i < rowCount; i++)
            {
                if (startRow + i >= rows.Count) break;
                var cell = rows[startRow + i].Descendants<TableCell>().ElementAtOrDefault(colIndex);
                if (cell == null) continue;

                var tcPr = cell.GetFirstChild<TableCellProperties>() ?? cell.PrependChild(new TableCellProperties());
                var vMerge = tcPr.GetFirstChild<VerticalMerge>() ?? tcPr.AppendChild(new VerticalMerge());
                vMerge.Val = i == 0 ? MergedCellValues.Restart : null;
            }
        }
        private void FillTable(Table table, List<NgayhocDto> lichTheoNgay)
        {
            var rows = table.Descendants<TableRow>().ToList();
            if (rows.Count < 2) return;

            var templateRow = rows[1];
            for (int i = rows.Count - 1; i >= 1; i--)
            {
                rows[i].Remove();
            }

            int currentRow = 0;

            foreach (var ngay in lichTheoNgay)
            {
                int ngayStart = currentRow;
                int ngayCount = 0;

                foreach (var buoi in ngay.Buoi_hoc)
                {
                    int buoiStart = currentRow;
                    int buoiCount = 0;

                    foreach (var tiet in buoi.Cac_tiet_hoc)
                    {
                        var newRow = (TableRow)templateRow.CloneNode(true);
                        var cells = newRow.Descendants<TableCell>().ToList();

                        if (cells.Count >= 7)
                        {
                            SetCellText(cells[0], buoiCount == 0 && ngayCount == 0 ? ngay.Ngay : "");
                            SetCellText(cells[1], buoiCount == 0 ? buoi.Ten_buoi : "");
                            SetCellText(cells[2], tiet.Tiet_tkb.ToString());
                            SetCellText(cells[3], tiet.Tiet_ppct > 0 ? tiet.Tiet_ppct.ToString() : "");
                            SetCellText(cells[4], tiet.Ten_lop);

                            var monText = tiet.Ten_mon;
                            if (!string.IsNullOrEmpty(tiet.Phan_mon))
                                monText += $" ({tiet.Phan_mon})";
                            SetCellText(cells[5], monText);

                            SetCellText(cells[6], tiet.Ten_bai);
                            if (cells.Count >= 8)
                                SetCellText(cells[7], tiet.Ghi_chu);
                        }

                        table.AppendChild(newRow);
                        currentRow++;
                        buoiCount++;
                        ngayCount++;
                    }

                    if (buoiCount > 1)
                        MergeCells(table, buoiStart + 1, buoiCount, 1);
                }

                if (ngayCount > 1)
                    MergeCells(table, ngayStart + 1, ngayCount, 0);
            }
        }
        private void ReplaceText(Body body, string placeholder, string value)
        {
            foreach (var para in body.Descendants<Paragraph>())
            {
                if (para.InnerText.Contains(placeholder))
                {
                    var runs = para.Descendants<Run>().ToList();
                    if (runs.Any())
                    {
                        var textElement = runs[0].Descendants<Text>().FirstOrDefault()
                            ?? runs[0].AppendChild(new Text());
                        textElement.Text = para.InnerText.Replace(placeholder, value);
                        runs.Skip(1).ToList().ForEach(r => r.Remove());
                    }
                }
            }
        }
        public byte[] FillTemplate(string templatePath, PhieubaogiangDto data)
        {
            try
            {
                using var memStream = new MemoryStream();
                using (var fileStream = File.OpenRead(templatePath))
                {
                    fileStream.CopyTo(memStream);
                }
                memStream.Position = 0;
                using (var doc = WordprocessingDocument.Open(memStream, true))
                {
                    var body = doc.MainDocumentPart!.Document.Body!;

                    // fill tuần
                    ReplaceText(body, "{{Tuan}}", data.Tuan.ToString());
                    ReplaceText(body, "{{Tu_ngay}}", data.Tu_ngay.ToString("dd/MM"));
                    ReplaceText(body, "{{Den_ngay}}", data.Den_ngay.ToString("dd/MM"));

                    // Fill bảng
                    var table = body.Descendants<Table>().FirstOrDefault();
                    if (table != null)
                    {
                        FillTable(table, data.Lich_theo_ngay);
                    }

                    doc.MainDocumentPart.Document.Save();
                }

                return memStream.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi fill template: {ex.Message}", ex);
            }
        }
        public byte[] FillMultipleAndZip(string templatePath, int idlbg, int idDonvi)
        {
            using var zipStream = new MemoryStream();
            List<PhieubaogiangDto> dataList = new List<PhieubaogiangDto>();
            var listIdPbg = _dbContext.Phieu_Baogiang.Where(c=>c.Id_lich_bao_giang == idlbg).Select(c=>c.Id).ToList();
            for(int i = 0; i < listIdPbg.Count; i++)
            {
                var data = GetList_Chitiet(listIdPbg[i], idDonvi);
                if(data != null )
                    dataList.Add(data);
            }
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var data in dataList)
                {
                    var wordBytes = FillTemplate(templatePath, data);
                    var fileName = $"{data.Ten_giao_vien} - Tuần {data.Tuan}(Từ ngày {data.Tu_ngay:dd.MM.yyyy} - Đến ngày {data.Den_ngay:dd.MM.yyyy}).docx";
                    var entry = archive.CreateEntry(fileName, CompressionLevel.Fastest);
                    using var entryStream = entry.Open();
                    entryStream.Write(wordBytes, 0, wordBytes.Length);
                }
            }
            return zipStream.ToArray();
        }

    }
}
