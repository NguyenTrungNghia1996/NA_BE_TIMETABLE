using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository;
using NA_Logic.IRepository.LichThi;
using NA_Logic.IRepository.XepGiamThi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class DM_LichthiRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_LichthiRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Lichthi_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, int idDiemThi, int idHoiDong)
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
                var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int)
                {
                    Value = idDonvi
                };
                var paramIdHoiDong = new SqlParameter("idHoiDong", SqlDbType.Int)
                {
                    Value = idHoiDong
                };
                var paramIdDiemThi = new SqlParameter("idDiemThi", SqlDbType.Int)
                {
                    Value = idDiemThi
                };
                var result = _dbContext.Set<DM_Lichthi_List>().FromSqlRaw("EXEC [DM_Lichthi_GetList_Paging] @pageIndex, @pageSize, @search, @idDonvi, @idDiemThi, @idHoiDong",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi, paramIdDiemThi, paramIdHoiDong).ToList();
                if (result == null) result = new List<DM_Lichthi_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Lichthi_Detail GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var result = (from lich in _dbContext.DM_Lichthi
                              join diem in _dbContext.DM_Diemthi on lich.Id_diem_thi equals diem.Id
                              join hoidong in _dbContext.DM_Hoidongthi on diem.Id_hoi_dong equals hoidong.Id into hoidongGroup
                              from hoidong in hoidongGroup.DefaultIfEmpty()
                              where lich.Id == Id && (diem.Id_don_vi == idDonvi || hoidong.Id_don_vi == idDonvi)
                              select new DM_Lichthi_Detail
                              {
                                  Id = lich.Id,
                                  Id_mon = lich.Id_mon,
                                  Bai_thi_tu_chon = lich.Bai_thi_tu_chon,
                                  Ngay = lich.Ngay,
                                  Giam_thi_khong_cung_mon = lich.Giam_thi_khong_cung_mon,
                                  Id_diem_thi = lich.Id_diem_thi,
                                  Id_hoi_dong = diem.Id_hoi_dong,
                                  Id_nam = hoidong.Id_nam
                              }).FirstOrDefault();

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Lichthi lichthi)
        {
            try
            {
                _dbContext.DM_Lichthi.Add(lichthi);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Lichthi lichthi)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Lichthi.Update(lichthi);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(int id)
        {
            try
            {
                var lichthi = _dbContext.DM_Lichthi.FirstOrDefault(c => c.Id == id);
                if (lichthi == null)
                {
                    return false;
                }
                _dbContext.DM_Lichthi.Remove(lichthi);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Check_constraint(int Id)
        {
            try
            {

                return _dbContext.Database.SqlQuery<int>($@"
                          select 1 as Value from DM_Giamthi where Id_diem_thi = {Id}
                          union select 1 from DM_Lichthi where Id_diem_thi = {Id}").Any();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckId(int Id, int idDonvi)
        {
            if (Id <= 0) return false;
            try
            {
                var idDiemthiByDonvi = _dbContext.DM_Diemthi.Where(c => c.Id_don_vi == idDonvi).Select(c => c.Id).ToList();

                var idHoidong = _dbContext.DM_Hoidongthi.Where(c => c.Id_don_vi == idDonvi).Select(c => c.Id).ToList();

                var idDiemthiByHoidong = _dbContext.DM_Diemthi.Where(c => idHoidong.Contains(c.Id_hoi_dong)).Select(c => c.Id).ToList();

                var idDiemthi = idDiemthiByDonvi.Union(idDiemthiByHoidong).ToList();

                var lichthi = _dbContext.DM_Lichthi.Any(c => c.Id == Id && idDiemthi.Contains(c.Id_diem_thi));
                return lichthi;
            }
            catch
            {
                return false;
            }
        }
        //public bool CheckIds(IEnumerable<int> ids)
        //{
        //    var existingIds = _dbContext.DM_Lichthi.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
        //    return ids.All(id => existingIds.Contains(id));
        //}
    }
}
