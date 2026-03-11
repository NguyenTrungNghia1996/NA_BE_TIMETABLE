using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository.LichBaoGiang;
using NetTopologySuite.Mathematics;
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
        public bool Add(int idLgb, int idtkb, int idDonvi)
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
                _dbContext.Phieu_Baogiang.AddRange(listPBG);
                _dbContext.SaveChanges();

                for (int i = 0; i < listPBG.Count; i++)
                {
                    bool addchitiet = Add_Chitiet(listPBG[i].Id, idDonvi);
                    if (!addchitiet)
                        return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
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
                var maxNgay = chitiet.Max(c => c.Ngay);
                var minNgay = chitiet.Min(c => c.Ngay);
                var listNgayNghi = _dbContext.DM_Ngaynghi.Where(n => n.Tu_ngay <= maxNgay && n.Den_ngay >= minNgay).ToList();

                var tuNgay = chitiet.First().Tu_Ngay.Date;
                var denNgay = chitiet.First().Den_Ngay.Date;
                var so_ngay = (denNgay - tuNgay).Days + 1;

                var dsCa = _dbContext.Ca_Donvi.Where(c=>c.Id_don_vi== idDonvi).ToList();
                // Lấy danh sách ngày từ enum
                var thuBatDau = (int)tuNgay.DayOfWeek; 
                var thuBatDauEnum = thuBatDau == 0 ? 7 : thuBatDau;
                var allNgayEnum = Enum.GetValues<Ngay>().ToList();
                var dsNgay = Enumerable.Range(0, so_ngay).Select(i => allNgayEnum.FirstOrDefault(n => (int)n == ((thuBatDauEnum - 1 + i) % 7) + 1)).ToList();                
                // Lấy danh sách tiết từ enum
                var dsTiet = Enum.GetValues<Tiet>().ToList();

                var result = new Chitiet_PhieubaogiangDto
                {
                    Ten_giao_vien = chitiet.FirstOrDefault()?.Ten_giao_vien,
                    Lich_theo_ngay = dsNgay.Select((ngayEnum, index) =>
                    {
                        var ngayHienTai = tuNgay.AddDays(index);

                        var laNgayNghi = listNgayNghi.Any(n =>ngayHienTai >= n.Tu_ngay.Date && ngayHienTai <= n.Den_ngay.Date);
                        var duLieuNgay = chitiet.Where(c => c.Ngay.Date == ngayHienTai).ToList();

                        return new NgayhocDto
                        {
                            Ngay = $"{ ngayEnum.GetDisplayName() } ({ ngayHienTai:dd / MM})",
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
        public bool Add_Chitiet(int idpbg, int idDonvi)
        {
            try
            {
                if(idpbg <= 0)
                {
                    return false;
                }
                var paramIdpbg = new SqlParameter("Idpbg", SqlDbType.Int) { Value = idpbg };
                var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int) { Value = idDonvi };
                var paramMessage = new SqlParameter("Message", SqlDbType.NVarChar, -1) { Direction = ParameterDirection.Output };

                _dbContext.Database.ExecuteSqlRaw("EXEC [Insert_ChitietPbg] @Idpbg, @idDonvi, @Message OUTPUT", paramIdpbg,paramIdDonvi, paramMessage);

                var Message = paramMessage.Value?.ToString() ?? "";

                //var tengv = (from pgb in _dbContext.Phieu_Baogiang
                //             join gv in _dbContext.DM_Giaovien on pgb.Id_giao_vien equals gv.Id
                //             where pgb.Id == idpbg  
                //             select gv.Ten).FirstOrDefault();
                return Message == "success";
            }
            catch (Exception ex)
            {
                 return false;
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
