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
    public class Phanphoi_ChuongtrinhRepository : IPhanphoi_ChuongtrinhRepository
    {
        private readonly NA_DbContext _dbContext;
        public Phanphoi_ChuongtrinhRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Phanphoi_Chuongtrinh_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord)
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
                var result = _dbContext.Set<Phanphoi_Chuongtrinh_List>().FromSqlRaw("EXEC Phanphoi_Chuongtrinh_GetList_Paging @pageIndex, @pageSize, @search,  @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramTotal)
                    .ToList();
                if (result == null) result = new List<Phanphoi_Chuongtrinh_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Phanphoi_Chuongtrinh GetDetailById(int Id)
        {
            try
            {
                var namhoc = _dbContext.Phanphoi_Chuongtrinh.FirstOrDefault(c => c.Id == Id);
                return namhoc;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(Phanphoi_Chuongtrinh ppct)
        {
            try
            {
                _dbContext.Phanphoi_Chuongtrinh.Add(ppct);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(Phanphoi_Chuongtrinh ppct)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.Phanphoi_Chuongtrinh.Update(ppct);
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
                var namhoc = _dbContext.Phanphoi_Chuongtrinh.Find(id);
                if (namhoc == null)
                {
                    return false;
                }
                _dbContext.Phanphoi_Chuongtrinh.Remove(namhoc);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
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
                return _dbContext.Phanphoi_Chuongtrinh.Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.Phanphoi_Chuongtrinh.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
        public bool CheckTrung(Phanphoi_Chuongtrinh ppct)
        {
            try
            {

                bool check = (from p in _dbContext.Phanphoi_Chuongtrinh
                              join m in _dbContext.Dm_Monhoc on p.Id_mon equals m.Id
                              where p.Id_mon == ppct.Id_mon
                                  && p.Id_khoi == ppct.Id_khoi
                                  && p.Id_ban == ppct.Id_ban
                                  && p.Id_nam_hoc == ppct.Id_nam_hoc
                                  && m.Id_don_vi == ppct.Id_don_vi
                                  && (ppct.Id <= 0 || p.Id != ppct.Id)
                              select p).Any();
                if (check) return true;

                return false;
            }
            catch { return true; }
        }
    }
}
