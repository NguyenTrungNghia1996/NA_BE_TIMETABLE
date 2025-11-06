using DocumentFormat.OpenXml.Office2010.Excel;
using EFCore.BulkExtensions;
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

        public List<Lich_Baogiang_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord)
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
                var paramSearch = new SqlParameter("search", SqlDbType.NVarChar)
                {
                    Value = search ?? string.Empty
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _dbContext.Set<Lich_Baogiang_List>().FromSqlRaw("EXEC Lich_Baogiang_GetList_Paging @pageIndex, @pageSize, @search,  @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramTotal)
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
        public (bool result, string mess) Add(Lich_Baogiang lbg)
        {
            try
            {
                var namHoc = _dbContext.DM_Namhoc.FirstOrDefault(x => x.Id == lbg.Id_nam_hoc);

                var soNgay = (namHoc.Den_ngay - namHoc.Tu_ngay).Days + 1;
                var soTuanToiDa = (int)Math.Ceiling(soNgay / 7.0);
                if (lbg.Tuan > soTuanToiDa)
                {
                    return (false, $"Tuần {lbg.Tuan} nằm ngoài năm học. Năm học này chỉ có {soTuanToiDa} tuần");
                }
                var tuNgay = namHoc.Tu_ngay.AddDays((lbg.Tuan - 1) * 7);
                var denNgay = tuNgay.AddDays(6);
                if (denNgay > namHoc.Den_ngay)
                {
                    denNgay = namHoc.Den_ngay;
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
        public (bool result, string mess) Update(Lich_Baogiang lbg)
        {
            try
            {
                var lichbg = _dbContext.Lich_Baogiang.Any(c => c.Id == lbg.Id);
                if (!lichbg)
                {
                    return (false, "Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
                }
                var namHoc = _dbContext.DM_Namhoc.FirstOrDefault(x => x.Id == lbg.Id_nam_hoc);

                var soNgay = (namHoc.Den_ngay - namHoc.Tu_ngay).Days + 1;
                var soTuanToiDa = (int)Math.Ceiling(soNgay / 7.0);

                var tuNgay = namHoc.Tu_ngay.AddDays((lbg.Tuan - 1) * 7);
                if (tuNgay > namHoc.Den_ngay)
                {
                    return (false, $"Tuần {lbg.Tuan} nằm ngoài năm học. Năm học này chỉ có {soTuanToiDa} tuần");
                }
                var denNgay = tuNgay.AddDays(6);
                if (denNgay > namHoc.Den_ngay)
                {
                    denNgay = namHoc.Den_ngay;
                }
                lbg.Tu_ngay = tuNgay;
                lbg.Den_ngay = denNgay;
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
        public bool CheckTrungTuan(Lich_Baogiang lbg, int idDonvi)
        {
            try
            {

                bool check = (from l in _dbContext.Lich_Baogiang
                              join p in _dbContext.Danhsach_Thoikhoabieu on l.Id_tkb equals p.Id
                              where l.Tuan == lbg.Tuan
                                  && l.Id_nam_hoc == lbg.Id_nam_hoc
                                  && p.Id_don_vi == idDonvi
                                  && (lbg.Id <= 0 || l.Id != lbg.Id)
                              select l).Any();

                if (check) return true;

                return false;
            }
            catch { return true; }
        }
    }
}
