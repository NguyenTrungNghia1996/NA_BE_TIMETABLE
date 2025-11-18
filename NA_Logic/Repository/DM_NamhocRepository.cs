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
    public class DM_NamhocRepository : IDM_NamhocRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_NamhocRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Namhoc_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord)
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
                var result = _dbContext.Set<DM_Namhoc_List>().FromSqlRaw("EXEC DM_Namhoc_GetList_Paging @pageIndex, @pageSize, @search,  @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Namhoc_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Namhoc GetDetailById(int Id)
        {
            try
            {
                var namhoc = _dbContext.DM_Namhoc.FirstOrDefault(c => c.Id == Id);
                return namhoc;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Namhoc dm_Namhoc)
        {
            try
            {
                dm_Namhoc.Tu_ngay = dm_Namhoc.Tu_ngay.Date;
                dm_Namhoc.Den_ngay = dm_Namhoc.Den_ngay.Date;
                _dbContext.DM_Namhoc.Add(dm_Namhoc);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Namhoc dm_Namhoc)
        {
            try
            {
                dm_Namhoc.Tu_ngay = dm_Namhoc.Tu_ngay.Date;
                dm_Namhoc.Den_ngay = dm_Namhoc.Den_ngay.Date;
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Namhoc.Update(dm_Namhoc);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(int id) {
            try
            {
                var namhoc = _dbContext.DM_Namhoc.Find(id);
                if(namhoc == null)
                {
                    return false;
                }
                _dbContext.DM_Namhoc.Remove(namhoc);
                _dbContext.SaveChanges();
                return true;
            }
            catch(Exception) 
            {
                return false;
            }
        }
        public bool CheckKhoangNgay(DM_Namhoc nam)
        {
            try
            {
                bool check = false;
                if (nam.Id == 0)
                {
                    check = _dbContext.DM_Namhoc.Any(c => (c.Tu_ngay <= nam.Tu_ngay && c.Den_ngay >= nam.Tu_ngay)
                                                    || (c.Tu_ngay <= nam.Den_ngay && c.Den_ngay >= nam.Den_ngay)
                                                    || (nam.Tu_ngay <= c.Tu_ngay && nam.Den_ngay >= c.Den_ngay));
                }
                else
                {
                    check = _dbContext.DM_Namhoc.Where(c=>c.Id != nam.Id).Any(c => (c.Tu_ngay <= nam.Tu_ngay && c.Den_ngay >= nam.Tu_ngay)
                                                    || (c.Tu_ngay <= nam.Den_ngay && c.Den_ngay >= nam.Den_ngay)
                                                    || (nam.Tu_ngay <= c.Tu_ngay && nam.Den_ngay >= c.Den_ngay));
                }
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
                          select 1 as Value from Phanphoi_Chuongtrinh where Id_nam_hoc = {Id}
                          union select 1 from Lich_Baogiang where Id_nam_hoc = {Id}").Any();
            }
            catch (Exception)
            {
                return false;
            }
        }
        public int GetMaxTuanByNam(int Id, int IdDonvi)
        {
            try
            {
                int TuanMax =(from lbg in _dbContext.Lich_Baogiang 
                              join n in _dbContext.DM_Namhoc on lbg.Id_nam_hoc equals n.Id
                                join ds in _dbContext.Danhsach_Thoikhoabieu on lbg.Id_tkb equals ds.Id
                                where n.Id == Id && ds.Id_don_vi == IdDonvi
                                select (int?)lbg.Tuan).Max() ?? 0;
                return TuanMax;
            }
            catch
            {
                return 0;
            }
        }
        public bool CheckId(int Id)
        {
            if (Id <= 0) return false;
            try
            {
                return _dbContext.DM_Namhoc.Any(c=>c.Id == Id);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.DM_Namhoc.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }

    }
}
