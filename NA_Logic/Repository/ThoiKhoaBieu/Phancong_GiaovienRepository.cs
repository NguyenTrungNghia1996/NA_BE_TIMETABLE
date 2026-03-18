using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository.XepThoiKhoaBieu;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Pipelines;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository.ThoiKhoaBieu
{
    public class Phancong_GiaovienRepository : IPhancong_GiaovienRepository
    {
        private readonly NA_DbContext _dbContext;
        public Phancong_GiaovienRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<PhancongGVDto> GetList_Paging( int idgv, int idDonvi,  int type)
        {
            try
            {

                var paramIdDonvi = new SqlParameter("Id_don_vi", SqlDbType.Int)
                {
                    Value = idDonvi
                };
                var paramIdgv = new SqlParameter("Id_giao_vien", SqlDbType.Int)
                {
                    Value = idgv
                };
                var paramType = new SqlParameter("Loai_hien_thi", SqlDbType.Int)
                {
                    Value = type
                };


                var raw = _dbContext.Set<PhancongGV>().FromSqlRaw("EXEC Getlist_PhancongGiaovien @Id_giao_vien, @Id_don_vi, @Loai_hien_thi",
                    paramIdgv, paramIdDonvi, paramType)
                    .ToList();
                if (raw == null) raw = new List<PhancongGV>();
                var result = raw.Select(item => new PhancongGVDto
                {
                    STT = item.STT,
                    Id_giao_vien = item.Id_giao_vien,
                    Id_mon = item.Id_mon,
                    Ten_mon = item.Ten_mon ?? string.Empty,
                    Id_don_vi = item.Id_don_vi,
                    Ten_lop = item.Ten_lop ?? string.Empty,
                    Id_lop = item.Id_lop?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?.Select(int.Parse) ?.ToList() ?? []
                }).ToList();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<DsLop_ByGVandMon> GetList_Lop_ByGvAndMon( int idgv, int idDonvi,  int idMon)
        {
            try
            {

                var paramIdDonvi = new SqlParameter("Id_don_vi", SqlDbType.Int)
                {
                    Value = idDonvi
                };
                var paramIdgv = new SqlParameter("Id_giao_vien", SqlDbType.Int)
                {
                    Value = idgv
                };
                var paramIdMon = new SqlParameter("Id_mon", SqlDbType.Int)
                {
                    Value = idMon
                };

                var result = _dbContext.Set<DsLop_ByGVandMon>().FromSqlRaw("EXEC GetList_LopByMonAndGV @Id_giao_vien, @Id_mon, @Id_don_vi",
                    paramIdgv,paramIdMon, paramIdDonvi)
                    .ToList();
                if (result == null) result = new List<DsLop_ByGVandMon>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool Add(List<PhancongGVDto> phancongList)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var grouped = phancongList.GroupBy(x => new { x.Id_giao_vien, x.Id_mon });
                var ListAdd = new List<Lophoc_Monhoc>();
                var ListUpdate = new List<Lophoc_Monhoc>();

                foreach (var group in grouped)
                {
                    var idgv = group.Key.Id_giao_vien;
                    var idmon = group.Key.Id_mon;
                    var newLopIds = group.SelectMany(x => x.Id_lop).Distinct().ToList();

                    var exist = _dbContext.Lophoc_Monhoc.Where(x => x.Id_giao_vien == idgv && x.Id_mon == idmon).ToList();

                    var toUpdateGV = exist.Where(e => !newLopIds.Contains(e.Id_lop)).ToList();
                    foreach (var x in toUpdateGV)
                    {
                        x.Id_giao_vien = 0;
                    }
                    ListUpdate.AddRange(toUpdateGV);

                    var existingLopIds = exist.Select(x => x.Id_lop).ToList();
                    var listIdLop = newLopIds.Where(lopId => !existingLopIds.Contains(lopId)).ToList();

                    if (listIdLop.Any())
                    {
                        var oldTeacher = _dbContext.Lophoc_Monhoc.Where(x => x.Id_mon == idmon && listIdLop.Contains(x.Id_lop) && x.Id_giao_vien != idgv).ToList();
                        foreach (var x in oldTeacher)
                        {
                            x.Id_giao_vien = idgv;
                        }
                        ListUpdate.AddRange(oldTeacher);

                        var IdLopUpdate = oldTeacher.Select(x => x.Id_lop).ToList();
                        var IdLopAdd = listIdLop.Except(IdLopUpdate).ToList();
                        var ListtoAdd = IdLopAdd
                            .Select(lopId => new Lophoc_Monhoc
                            {
                                Id_giao_vien = idgv,
                                Id_mon = idmon,
                                Id_lop = lopId
                            }).ToList();
                        ListAdd.AddRange(ListtoAdd);
                    }
                }

                if (ListUpdate.Any())
                    _dbContext.BulkUpdate(ListUpdate);
                if (ListAdd.Any())
                    _dbContext.BulkInsert(ListAdd);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
        public byte[] Export(int idDonvi)
        {
            try
            {
                var paramIdDonvi = new SqlParameter("Id_don_vi", SqlDbType.Int)
                {
                    Value = idDonvi
                };
                var data = _dbContext.Set<DsLopMon_byGV>().FromSqlRaw("EXEC [GetList_PhancongGiaovien_ToExport] @Id_don_vi",
                    paramIdDonvi)
                    .ToList();

                if (!data.Any()) return null;

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Phân công giáo viên");

                worksheet.Cell(1, 1).Value = "Họ tên giáo viên";
                worksheet.Cell(1, 2).Value = "Môn học";
                worksheet.Cell(1, 3).Value = "Lớp";
                worksheet.Cell(1, 4).Value = "Tổng số tiết";

                var headerRange = worksheet.Range(1, 1, 1, 4);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var listGV = data.GroupBy(x => new { x.Id_giao_vien, x.Ten_giao_vien }).OrderBy(g => g.Key.Ten_giao_vien.Split(' ').LastOrDefault());

                int currentRow = 2;

                foreach (var gv in listGV)
                {
                    int startRow = currentRow;
                    int tongTietGV = gv.Sum(x => x.Tong_tiet);
                    var monHocGroups = gv.GroupBy(x => x.Ten_mon).OrderBy(m => m.Key);

                    foreach (var monGroup in monHocGroups)
                    {
                        var danhSachLop = string.Join("; ", monGroup.Select(x => x.Ten_lop).OrderBy(l => l));
                        var tongTiet = monGroup.Sum(x => x.Tong_tiet);

                        worksheet.Cell(currentRow, 2).Value = monGroup.Key;
                        worksheet.Cell(currentRow, 3).Value = danhSachLop;
                        worksheet.Cell(currentRow, 4).Value = tongTiet;

                        currentRow++;
                    }

                    int endRow = currentRow - 1;
                    worksheet.Cell(startRow, 1).Value = gv.Key.Ten_giao_vien;
                    worksheet.Cell(startRow, 4).Value = tongTietGV;

                    if (endRow > startRow)
                    {
                        worksheet.Range(startRow, 1, endRow, 1).Merge();
                        worksheet.Range(startRow, 4, endRow, 4).Merge();
                    }

                    worksheet.Range(startRow, 1, endRow, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    worksheet.Range(startRow, 4, endRow, 4).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                }

                var dataRange = worksheet.Range(1, 1, currentRow - 1, 4);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
