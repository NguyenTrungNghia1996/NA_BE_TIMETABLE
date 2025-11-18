using DocumentFormat.OpenXml.Office2010.Excel;
using EFCore.BulkExtensions;
using Humanizer;
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
    public class Lich_BaogiangRepository : ILich_BaogiangRepository
    {
        private readonly NA_DbContext _dbContext;
        public Lich_BaogiangRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Lich_Baogiang_List> GetList_Paging(int PageIndex, int PageSize, int IdNam, int IdDonvi, ref int totalrecord)
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
                var paramIdNam = new SqlParameter("Id_nam", SqlDbType.Int)
                {
                    Value = IdNam
                };
                var paramIdDonvi = new SqlParameter("Id_don_vi", SqlDbType.Int)
                {
                    Value = IdDonvi
                };

                //var paramSearch = new SqlParameter("search", SqlDbType.NVarChar)
                //{
                //    Value = search ?? string.Empty
                //};
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _dbContext.Set<Lich_Baogiang_List>().FromSqlRaw("EXEC Lich_Baogiang_GetList_Paging @pageIndex, @pageSize, @Id_nam, @Id_don_vi, @total OUTPUT",
                    paramPageIndex, paramPageSize,paramIdNam, paramIdDonvi, paramTotal)
                    .ToList();
                if (result == null) result = new List<Lich_Baogiang_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Lich_Baogiang GetDetailById(int Id)
        {
            try
            {
                var lbg = _dbContext.Lich_Baogiang.FirstOrDefault(c => c.Id == Id);
                return lbg;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private (DateTime tuNgay, DateTime denNgay) TinhNgayTheoTuan(int tuan, int idNamHoc, int idDonvi)
        {
            var soNgayTrongTuan = _dbContext.DM_Donvi
                .Where(c => c.Id == idDonvi)
                .Select(c => c.So_ngay)
                .FirstOrDefault();

            var namHoc = _dbContext.DM_Namhoc.FirstOrDefault(x => x.Id == idNamHoc);

            // Tính thứ 2 đầu tiên của năm học
            var thuTrongTuan = (int)namHoc.Tu_ngay.DayOfWeek;
            var soNgayLeTheoThu2 = (thuTrongTuan == 0) ? 6 : thuTrongTuan - 1;
            var thu2DauTien = namHoc.Tu_ngay.AddDays(-soNgayLeTheoThu2);

            // Nếu năm học bắt đầu sau ngày làm việc cuối cùng của tuần đầu tiên thì tuần 1 sẽ bắt đầu từ thứ 2 tuần sau
            var ngayCuoiTuanDauTien = thu2DauTien.AddDays(soNgayTrongTuan - 1);
            if (namHoc.Tu_ngay > ngayCuoiTuanDauTien)
            {
                thu2DauTien = thu2DauTien.AddDays(7);
            }

            // Tính từ ngày và đến ngày cho tuần hiện tại
            DateTime tuNgay = thu2DauTien.AddDays((tuan - 1) * 7);
            DateTime denNgay = tuNgay.AddDays(soNgayTrongTuan - 1);

            // Đảm bảo không vượt quá ngày kết thúc năm học
            if (denNgay > namHoc.Den_ngay)
            {
                denNgay = namHoc.Den_ngay;
            }

            // Đảm bảo không bắt đầu trước ngày bắt đầu năm học (chỉ áp dụng cho tuần 1)
            if (tuan == 1 && tuNgay < namHoc.Tu_ngay)
            {
                tuNgay = namHoc.Tu_ngay;
            }

            return (tuNgay, denNgay);
        }
        public (bool result, string mess) Add(Lich_Baogiang lbg, int idDonvi)
        {
            try
            {
                var soNgayTrongTuan = _dbContext.DM_Donvi.Where(c => c.Id == idDonvi).Select(c => c.So_ngay).FirstOrDefault();
                var namHoc = _dbContext.DM_Namhoc.FirstOrDefault(x => x.Id == lbg.Id_nam_hoc);
                var soNgay = (namHoc.Den_ngay - namHoc.Tu_ngay).Days + 1;

                var tuanMax = (from lichbg in _dbContext.Lich_Baogiang
                                join tkb in _dbContext.Danhsach_Thoikhoabieu on lichbg.Id_tkb equals tkb.Id
                                where tkb.Id_don_vi == idDonvi && lichbg.Id_nam_hoc == lbg.Id_nam_hoc
                                select (int?)lichbg.Tuan).Max();
                lbg.Tuan = (tuanMax ?? 0) + 1;
                var soTuanToiDa = (int)Math.Ceiling(soNgay / 7.0);
                if (lbg.Tuan > soTuanToiDa)
                {
                    return (false, $"Tuần {lbg.Tuan} nằm ngoài năm học. Năm học này chỉ có {soTuanToiDa} tuần");
                }
                var (tuNgay, denNgay) = TinhNgayTheoTuan(lbg.Tuan, lbg.Id_nam_hoc, idDonvi);
                // Kiểm tra có vượt quá năm học không
                if (tuNgay > namHoc.Den_ngay)
                {
                    return (false, $"Không thể thêm tuần {lbg.Tuan}. Năm học đã kết thúc");
                }

                // Đảm bảo không vượt quá ngày kết thúc năm học
                if (denNgay > namHoc.Den_ngay)
                {
                    denNgay = namHoc.Den_ngay;
                }

                // Đảm bảo không bắt đầu trước ngày bắt đầu năm học (chỉ áp dụng cho tuần 1)
                if (lbg.Tuan == 1 && tuNgay < namHoc.Tu_ngay)
                {
                    tuNgay = namHoc.Tu_ngay;
                }

                lbg.Tu_ngay = tuNgay;
                lbg.Den_ngay = denNgay;
                _dbContext.Lich_Baogiang.Add(lbg);
                _dbContext.SaveChanges();
                return (true,"Thêm lịch báo giảng thành công");
            }
            catch (Exception)
            {
                return (false,"Thêm mới thất bại");
            }
        }
        public (bool result, string mess) Update(Lich_Baogiang lbg, int idDonvi)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                bool checkChangeNam = _dbContext.Lich_Baogiang.Any(c => c.Id == lbg.Id && c.Id_nam_hoc == lbg.Id_nam_hoc);
                if (!checkChangeNam)
                {
                    var soNgayTrongTuan = _dbContext.DM_Donvi.Where(c => c.Id == idDonvi).Select(c => c.So_ngay).FirstOrDefault();
                    var namHoc = _dbContext.DM_Namhoc.FirstOrDefault(x => x.Id == lbg.Id_nam_hoc);
                    var soNgay = (namHoc.Den_ngay - namHoc.Tu_ngay).Days + 1;

                    var tuanMax = (from lichbg in _dbContext.Lich_Baogiang
                                   join tkb in _dbContext.Danhsach_Thoikhoabieu on lichbg.Id_tkb equals tkb.Id
                                   where tkb.Id_don_vi == idDonvi && lichbg.Id_nam_hoc == lbg.Id_nam_hoc
                                   select (int?)lichbg.Tuan).Max();
                    lbg.Tuan = (tuanMax ?? 0) + 1;
                    var soTuanToiDa = (int)Math.Ceiling(soNgay / 7.0);
                    if (lbg.Tuan > soTuanToiDa)
                    {
                        return (false, $"Tuần {lbg.Tuan} nằm ngoài năm học. Năm học này chỉ có {soTuanToiDa} tuần");
                    }
                    var (tuNgay, denNgay) = TinhNgayTheoTuan(lbg.Tuan, lbg.Id_nam_hoc, idDonvi);
                    // Kiểm tra có vượt quá năm học không
                    if (tuNgay > namHoc.Den_ngay)
                    {
                        return (false, $"Không thể thêm tuần {lbg.Tuan}. Năm học đã kết thúc");
                    }

                    // Đảm bảo không vượt quá ngày kết thúc năm học
                    if (denNgay > namHoc.Den_ngay)
                    {
                        denNgay = namHoc.Den_ngay;
                    }

                    // Đảm bảo không bắt đầu trước ngày bắt đầu năm học (chỉ áp dụng cho tuần 1)
                    if (lbg.Tuan == 1 && tuNgay < namHoc.Tu_ngay)
                    {
                        tuNgay = namHoc.Tu_ngay;
                    }

                    lbg.Tu_ngay = tuNgay;
                    lbg.Den_ngay = denNgay;
                }
                _dbContext.Lich_Baogiang.Update(lbg);
                _dbContext.SaveChanges();
                return (true, "Cập nhật lịch báo giảng thành công");
            }
            catch (Exception)
            {
                return (false, "Cập nhật mới thất bại");
            }
        }
        public bool Delete(int id)
        {
            try
            {
                var lbg = _dbContext.Lich_Baogiang.Find(id);
                if (lbg == null)
                {
                    return false;
                }
                _dbContext.Lich_Baogiang.Remove(lbg);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }  
        public bool CheckChangeTKB(Lich_Baogiang lbg)
        {
            try
            {
                return _dbContext.Lich_Baogiang.Any(c => c.Id_tkb == lbg.Id_tkb && c.Id == lbg.Id);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckExistPPCT(int idNam, int idDonvi)
        {
            try
            {
                return _dbContext.Phanphoi_Chuongtrinh.Any(c => c.Id_nam_hoc == idNam && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        //public bool Check_constraint(int Id)
        //{
        //    try
        //    {

        //        return _dbContext.Database.SqlQueryRaw<int>($@"
        //                  select 1 as Value from Ca_Donvi where Id_ca_hoc = {Id}
        //                  union select 1 from Chitiet_Thoikhoabieu where Id_ca = {Id} 
        //                  union select 1 from DM_Lophoc where Id_ca = {Id}
        //                  union select 1 from Giaovien_Tiettranhxep  where Id_ca = {Id}
        //                  union select 1 from Lophoc_Monhoc_Tiettranhxep  where Id_ca = {Id}
        //                  union select 1 from Lophoc_Tietnghi  where Id_ca = {Id}
        //                  union select 1 from Monhoc_Khoilop  where Id_ca = {Id}
        //                  union select 1 from Monhoc_Khoilop_Tiettranhxep  where Id_ca = {Id}
        //                  union select 1 from Tiet_ban  where Id_ca = {Id}
        //                  union select 1 from Tiet_tranh_xep  where Id_ca = {Id}
        //                  union select 1 from Tiet_co_dinh  where Id_ca = {Id}").Any();
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        public bool CheckId(int Id, int idDonvi)
        {
            if (Id <= 0) return false;
            try
            {
                return (from l in _dbContext.Lich_Baogiang join tkb in _dbContext.Danhsach_Thoikhoabieu on l.Id_tkb equals tkb.Id 
                        where tkb.Id_don_vi == idDonvi && l.Id == Id select l).Any();
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.Lich_Baogiang.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
        //public bool CheckTrungTuan(Lich_Baogiang lbg, int idDonvi)
        //{
        //    try
        //    {

        //        bool check = (from l in _dbContext.Lich_Baogiang
        //                      join p in _dbContext.Danhsach_Thoikhoabieu on l.Id_tkb equals p.Id
        //                      where l.Tuan == lbg.Tuan
        //                          && l.Id_nam_hoc == lbg.Id_nam_hoc
        //                          && p.Id_don_vi == idDonvi
        //                          && (lbg.Id <= 0 || l.Id != lbg.Id)
        //                      select l).Any();

        //        if (check) return true;

        //        return false;
        //    }
        //    catch { return true; }
        //}
    }
}

 