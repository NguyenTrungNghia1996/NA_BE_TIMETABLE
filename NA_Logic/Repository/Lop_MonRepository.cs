using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Lop_MonRepository : ILop_MonRepository
    {
        private readonly NA_DbContext _context;
        public Lop_MonRepository(NA_DbContext context)
        {
            _context = context;
        }
        public Lop_MonDto GetLopMon(int idLop, int idDonvi)
        {
            var paramIdLop = new SqlParameter("IdLop", SqlDbType.Int)
            {
                Value = idLop
            };
            var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int)
            {
                Value = idDonvi
            };

            var spResults = _context.Set<Lophoc_Monhoc_List>()
                .FromSqlRaw("EXEC GetLopMon @IdLop, @idDonvi", paramIdLop, paramIdDonvi)
                .ToList();

            if (!spResults.Any())
            {
                return null;
            }

            var result = new Lop_MonDto
            {
                Id_lop = spResults.First().Id_lop,
                Ds_mon = spResults.Select(item => new Mon_LopDto
                {
                    Id_mon = item.Id_mon,
                    Ten_mon = item.Ten_mon,
                    Id_giao_vien = item.Id_giao_vien,
                    Ten_giao_vien = item.Ten_giao_vien,
                    Id_phong_truyen_thong = item.Id_phong_truyen_thong,
                    Ten_phong_truyen_thong = item.Ten_phong_truyen_thong,
                    Id_phong_chuyen_dung = item.Id_phong_chuyen_dung,
                    Ten_phong_chuyen_dung = item.Ten_phong_chuyen_dung,
                    So_tiet_ca_sang_truyen_thong = item.So_tiet_ca_sang_truyen_thong,
                    So_tiet_ca_chieu_truyen_thong = item.So_tiet_ca_chieu_truyen_thong,
                    So_tiet_ca_sang_phong_chuyen_dung = item.So_tiet_ca_sang_phong_chuyen_dung,
                    So_tiet_ca_chieu_phong_chuyen_dung = item.So_tiet_ca_chieu_phong_chuyen_dung,
                    Trang_thai = item.Trang_thai
                }).ToList()
            };

            return result;
        }
        public bool AddMonLop(List<Lophoc_Monhoc> dsLopMon, int idLop)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var LopMonCu = _context.Lophoc_Monhoc.Where(tb => tb.Id_lop == idLop).ToList();

                //xóa
                if (LopMonCu != null && LopMonCu.Any())
                {
                    _context.BulkDelete(LopMonCu);
                }
                //thêm
                if (dsLopMon != null && dsLopMon.Any())
                {
                    _context.BulkInsert(dsLopMon);
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return false;
            }
        }
        //public List<string> CheckIds_LopMon(int? idMon, int? idgv, int? idphongcd, int? idphongtt, int idDonvi)
        //{
        //    try
        //    {
        //        var paramIdMon = new SqlParameter("@IdMon", SqlDbType.Int)
        //        {
        //            Value = idMon.HasValue ? idMon.Value : (object)DBNull.Value
        //        };
        //        var paramIdgv = new SqlParameter("@IdGiaovien", SqlDbType.Int)
        //        {
        //            Value = idgv.HasValue ? idgv.Value : (object)DBNull.Value
        //        };
        //        var paramIdphongcd = new SqlParameter("@IdPhongChuyendung", SqlDbType.Int)
        //        {
        //            Value = idphongcd.HasValue ? idphongcd.Value : (object)DBNull.Value
        //        };
        //        var paramIdphongtt = new SqlParameter("@IdPhongTruyenthong", SqlDbType.Int)
        //        {
        //            Value = idphongtt.HasValue ? idphongtt.Value : (object)DBNull.Value
        //        };
        //        var paramIdDonvi = new SqlParameter("@IdDonvi", SqlDbType.Int) { Value = idDonvi };

        //        var results = _context.Set<CheckIds>().FromSqlRaw(
        //                "EXEC CheckIds_Tietban @IdDonvi, @IdMon, @IdGiaovien, @IdPhongChuyendung, @IdPhongTruyenthong",
        //                paramIdDonvi, paramIdMon, paramIdgv, paramIdphongcd, paramIdphongtt)
        //            .ToList();

        //        var errorMessages = results
        //            .Where(r => !r.IsValid)
        //            .Select(r => r.Field switch
        //            {
        //                "Mon" => "Môn học không hợp lệ hoặc không thuộc đơn vị này",
        //                "Giaovien" => "Giáo viên không hợp lệ hoặc không thuộc đơn vị này",
        //                "PhongChuyendung" => "Phòng chuyên dụng không hợp lệ hoặc không thuộc đơn vị này",
        //                "PhongTruyenthong" => "Phòng truyền thống không hợp lệ hoặc không thuộc đơn vị này",
        //                _ => "Dữ liệu không hợp lệ"
        //            })
        //            .ToList();

        //        return errorMessages;
        //    }
        //    catch (Exception)
        //    {
        //        return new List<string> { "Có lỗi xảy ra khi kiểm tra dữ liệu" };
        //    }
        //}

        public bool DeleteMonLop(int Id_lop)
        {
            try
            {
                var LopMon = _context.Lophoc_Monhoc.Where(c => c.Id_lop == Id_lop).ToList();
                if (LopMon.Count > 0)
                {
                    _context.BulkDelete(LopMon);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public LopMon_banDto GetListTietBan_MonLop(int Id_lop, int Id_mon, int idDonvi)
        {
            var dsCa = _context.Ca_Donvi.Where(cd => cd.Id_don_vi == idDonvi)
                            .Join(_context.DM_Cahoc,
                                  cd => cd.Id_ca_hoc,
                                  ca => ca.Id,
                                  (cd, ca) => new
                                  {
                                      Id = ca.Id,
                                      Ten = ca.Ten
                                  }).ToList();
            var tietBan = _context.Lophoc_Monhoc_Tiettranhxep
                        .Where(tb => tb.Id_lop == Id_lop && tb.Id_mon == Id_mon)
                        .Select(tb => new { tb.Id_ca, tb.Ngay, tb.Tiet })
                        .ToList();

            // Lấy danh sách ngày từ enum
            var dsNgay = Enum.GetValues<Ngay>().ToList();
            // Lấy danh sách tiết từ enum
            var dsTiet = Enum.GetValues<Tiet>().ToList();

            var result = new LopMon_banDto
            {
                Id_lop = Id_lop,
                Id_mon = Id_mon,
                Ds_Ca = dsCa.Select(ca => new Ca_banDto
                {
                    Id = ca.Id,
                    Ds_Ngay = dsNgay.Select(ngay => new Ngay_banDto
                    {
                        Id = ngay,
                        Ten = ngay.GetDisplayName(),
                        Ds_Tiet = dsTiet.Select(tiet => new TietbanDto
                        {
                            Id = tiet,
                            Ten = tiet.GetDisplayName(),
                            Trang_thai = tietBan.Any(td => td.Id_ca == ca.Id &&
                                                           td.Ngay == (int)ngay &&
                                                           td.Tiet == (int)tiet)
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            return result;
        }
        public bool AddTietBan_MonLop(List<Lophoc_Monhoc_Tiettranhxep> dsTietBan, int idLop, int idMon)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var existingTietBan = _context.Lophoc_Monhoc_Tiettranhxep.Where(tb => tb.Id_lop == idLop && tb.Id_mon == idMon).ToList();

                //xóa
                if (existingTietBan.Any())
                {
                    _context.BulkDelete(existingTietBan);
                }
                //thêm
                if (dsTietBan != null && dsTietBan.Any())
                {
                    _context.BulkInsert(dsTietBan);
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return false;
            }
        }
        public bool DeleteTietBan_MonLop(int Id_lop, int Id_mon)
        {
            try
            {
                var tietban = _context.Lophoc_Monhoc_Tiettranhxep.Where(c => c.Id_lop == Id_lop && c.Id_mon == Id_mon).ToList();
                if (tietban.Count > 0)
                {
                    _context.BulkDelete(tietban);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}