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
    public class DM_DiemthiRepository : IDM_DiemthiRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_DiemthiRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Diemthi_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi)
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
                var result = _dbContext.Set<DM_Diemthi_List>().FromSqlRaw("EXEC [DM_Diemthi_GetList_Paging] @pageIndex, @pageSize, @search, @idDonvi",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi).ToList();
                if (result == null) result = new List<DM_Diemthi_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Diemthi GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var hoidong = _dbContext.DM_Hoidongthi.Where(h => h.Id_don_vi == idDonvi).Select(c=> c.Id).ToList();
                var diemthi = _dbContext.DM_Diemthi.FirstOrDefault(c => c.Id == Id && hoidong.Contains(c.Id_hoi_dong));
                return diemthi;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Diemthi diemthi)
        {
            try
            {
                _dbContext.DM_Diemthi.Add(diemthi);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Diemthi diemthi)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Diemthi.Update(diemthi);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(int id, int idDonvi)
        {
            try
            {
                var hoidong = _dbContext.DM_Hoidongthi.Where(h => h.Id_don_vi == idDonvi).Select(c => c.Id).ToList();
                var diemthi = _dbContext.DM_Diemthi.FirstOrDefault(c=> hoidong.Contains(c.Id_hoi_dong) && c.Id == id);
                if (diemthi == null)
                {
                    return false;
                }
                _dbContext.DM_Diemthi.Remove(diemthi);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckMa(string Ma, int idHoiDong, int? Id)
        {
            try
            {
                var query = _dbContext.DM_Diemthi.Where(c => c.Ma == Ma && c.Id_hoi_dong == idHoiDong);

                if (Id.HasValue)
                {
                    query = query.Where(c => c.Id != Id.Value);
                }

                var check = query.Any();
                return check;
            }
            catch
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
                          union select 1 from DM_Phongthi where Id_diem_thi = {Id}").Any();
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
                var hoidong = _dbContext.DM_Hoidongthi.Where(h => h.Id_don_vi == idDonvi).Select(c => c.Id).ToList();
                return _dbContext.DM_Diemthi.Any(c => c.Id == Id && hoidong.Contains(c.Id_hoi_dong));
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.DM_Diemthi.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }

    }
}
