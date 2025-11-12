using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Phieu_BaogiangRepository : IPhieu_BaogiangRepository
    {
        private readonly NA_DbContext _dbContext;
        public Phieu_BaogiangRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Phieu_Baogiang_List> GetList_Paging(int PageIndex, int PageSize, int idlbg,  string search, ref int totalrecord)
        {
            try
            {
                var paramPageIndex = new SqlParameter("pageIndex", SqlDbType.Int)
                {
                    Value = PageIndex
                };
                var paramPageSize = new SqlParameter("pageSize", SqlDbType.Int)
                {
                    Value = PageSize
                };
                var paramIdLbg = new SqlParameter("IdLBG", SqlDbType.Int)
                {
                    Value = idlbg
                };
                var paramSearch = new SqlParameter("search", SqlDbType.NVarChar)
                {
                    Value = search ?? string.Empty
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _dbContext.Set<Phieu_Baogiang_List>().FromSqlRaw("EXEC Phieu_Baogiang_GetList_Paging @pageIndex, @pageSize, @search, @IdLBG,  @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch,paramIdLbg, paramTotal)
                    .ToList();
                if (result == null) result = new List<Phieu_Baogiang_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Phieu_Baogiang GetDetailById(int Id)
        {
            try
            {
                var namhoc = _dbContext.Phieu_Baogiang.FirstOrDefault(c => c.Id == Id);
                return namhoc;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public (bool result, string mess) Add(int idLgb, int idtkb)
        {
            try
            {
                var listIdgv = _dbContext.Chitiet_Thoikhoabieu.Where(c => c.Id_tkb == idtkb).Select(c => c.Id_giao_vien).Distinct().ToList();
                var listPBG = new List<Phieu_Baogiang>();
                for (int i = 0; i < listIdgv.Count; i++)
                {
                    var itemPBG = new Phieu_Baogiang
                    {
                        Id_lich_bao_giang = idLgb,
                        Id_giao_vien = listIdgv[i]
                    };
                    listPBG.Add(itemPBG);
                }
                _dbContext.BulkInsert(listPBG);
                for (int i = 0; i < listPBG.Count; i++)
                {
                    var result = Add_Chitiet(listPBG[i].Id);
                    if (!result.resultchitiet)
                    {
                        return (false, result.messchitiet);
                    }
                }
                return (true,"");
            }
            catch (Exception)
            {
                return (false, "");
            }
        }
        public (bool result, string mess) Delete(int idlgb)
        {
            try
            {
                var listPBG = _dbContext.Phieu_Baogiang.Where(c => c.Id_lich_bao_giang == idlgb).ToList();
                var listgv = (from pgb in _dbContext.Phieu_Baogiang
                             join gv in _dbContext.DM_Giaovien on pgb.Id_giao_vien equals gv.Id
                             select new { gv.Ten, pgb.Id }).ToList();
                for (int i = 0; i < listPBG.Count; i++)
                {
                    bool result = Delete_Chitiet(listPBG[i].Id);
                    var tengv = listgv.Where(c => c.Id == listPBG[i].Id).Select(c => c.Ten).FirstOrDefault();
                    if (!result)
                    {
                        return (false, $"Xoá chi tiết của giáo viên {tengv} thất bại");
                    }
                }
                _dbContext.BulkDelete(listPBG);
                return (true,"");
            }
            catch (Exception)
            {
                return (false,"");
            }
        }
        public Chitiet_PhieubaogiangDto GetList_Chitiet(int idpbg, int idDonvi)
        {
            try
            {
                var paramIdpbg = new SqlParameter("idPBG", SqlDbType.Int)
                {
                    Value = idpbg
                };
                var chitiet = _dbContext.Set<Chitiet_Phieubaogiang_List>().FromSqlRaw("EXEC [GetList_ChitietPbg] @idPBG",paramIdpbg).ToList();

                var listNgayNghi = _dbContext.DM_Ngaynghi.Where(n => n.Tu_ngay <= chitiet.Max(c => c.Ngay) && n.Den_ngay >= chitiet.Min(c => c.Ngay)).ToList();

                var donvi = _dbContext.DM_Donvi.Find(idDonvi);
                // Lấy danh sách ngày từ enum
                var dsNgay = Enum.GetValues<Ngay>().Take(donvi.So_ngay).ToList();
                // Lấy danh sách tiết từ enum
                var dsTiet = Enum.GetValues<Tiet>().ToList();

                var result = new Chitiet_PhieubaogiangDto
                {
                    Ten_giao_vien = chitiet.FirstOrDefault()?.Ten_giao_vien,
                    Lich_theo_ngay = chitiet.GroupBy(x => x.Ngay.Date).OrderBy(g => g.Key)
                        .Select(ngayGroup =>
                        {
                        // check ngày nghỉ
                        var ngayHienTai = ngayGroup.Key;
                        var laNgayNghi = listNgayNghi.Any(n => ngayHienTai >= n.Tu_ngay.Date && ngayHienTai <= n.Den_ngay.Date);

                            return new NgayhocDto
                            {
                                Ngay = $"{new CultureInfo("vi-VN").DateTimeFormat.GetDayName(ngayGroup.Key.DayOfWeek)} ({ngayGroup.Key:dd/MM})",
                                Buoi_hoc = ngayGroup.GroupBy(x => x.Ten_ca)
                                    .OrderBy(g => g.Key.Contains("Sáng") ? 1 : g.Key.Contains("Chiều") ? 2 : 3)
                                    .Select(buoiGroup => new BuoihocDto
                                    {
                                        Ten_buoi = buoiGroup.Key,
                                        Cac_tiet_hoc = buoiGroup
                                            .OrderBy(x => x.Tiet)
                                            .Select(item => new Thongtin_tietDto
                                            {
                                                Tiet_tkb = item.Tiet,
                                                Tiet_ppct = item.Id_chi_tiet_PPCT,
                                                Ten_lop = item.Ten_lop,
                                                Ten_mon = item.Ten_mon,
                                                Phan_mon = item.Phan_mon,
                                                Ten_bai = laNgayNghi ? "Nghỉ" : item.Ten_bai,
                                                Ghi_chu = item.Ghi_chu
                                            }).ToList()
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
        public (bool resultchitiet, string messchitiet) Add_Chitiet(int idpbg)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var paramIdpbg = new SqlParameter("Idpbg", SqlDbType.Int) { Value = idpbg };
                var paramMessage = new SqlParameter("ErrorMessage", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };

                _dbContext.Database.ExecuteSqlRaw("EXEC [Insert_ChitietPbg] @json, @idDonvi, @ErrorMessage OUTPUT", paramIdpbg, paramMessage);

                var Message = paramMessage.Value?.ToString() ?? "";

                var tengv = (from pgb in _dbContext.Phieu_Baogiang
                             join gv in _dbContext.DM_Giaovien on pgb.Id_giao_vien equals gv.Id
                             where pgb.Id == idpbg
                             select gv.Ten).FirstOrDefault();
                return (Message == "success", $"Phiếu báo giảng của giáo viên {tengv} bị lỗi: {Message}" );
            }
            catch (Exception ex)
            {
                 return (false,"Thêm chi tiết thất bại");
            }
        }
        public bool Delete_Chitiet(int idpgb)
        {
            try
            {
                var listPBG = _dbContext.Chitiet_Phieubaogiang.Where(c => c.Id_phieu_bao_giang == idpgb).ToList();
                _dbContext.BulkDelete(listPBG);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
