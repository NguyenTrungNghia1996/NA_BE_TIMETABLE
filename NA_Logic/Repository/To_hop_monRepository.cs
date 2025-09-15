using DocumentFormat.OpenXml.InkML;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class To_hop_monRepository : ITo_hop_monRepository
    {
        private readonly NA_DbContext _dbContext;
        public To_hop_monRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Monhoc_Tohopmon_List> GetList_Paging(int PageIndex, int PageSize, int IdDonvi, ref int totalrecord)
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
                var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int)
                {
                    Value = IdDonvi
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _dbContext.Set<Monhoc_Tohopmon_List>().FromSqlRaw("EXEC MonTohop_GetList_Paging @pageIndex = @pageIndex, @pageSize = @pageSize, @idDonvi = @idDonvi, @total = @total OUTPUT",
                    paramPageIndex, paramPageSize, paramIdDonvi, paramTotal)
                    .ToList();
                if (result == null) result = new List<Monhoc_Tohopmon_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Monhoc_Tohopmon GetDetailById(int Id)
        {
            try
            {
                var monTohop = _dbContext.Monhoc_Tohopmon.FirstOrDefault(c => c.Id == Id);
                return monTohop;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(Monhoc_Tohopmon tohopmon)
        {
            try
            {
                _dbContext.Monhoc_Tohopmon.Add(tohopmon);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(Monhoc_Tohopmon tohopmon)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.Monhoc_Tohopmon.Update(tohopmon);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool Delete(int Id)
        {
            try
            {
                Monhoc_Tohopmon tohopmon = new Monhoc_Tohopmon();
                tohopmon = _dbContext.Monhoc_Tohopmon.FirstOrDefault(c => c.Id == Id);
                if (tohopmon != null)
                {
                    _dbContext.Monhoc_Tohopmon.Remove(tohopmon);
                    _dbContext.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckId(int Id, int idDonvi)
        {
            if (Id <= 0) return false;
            try
            {
                return _dbContext.Monhoc_Tohopmon.Join(_dbContext.Dm_Monhoc, thm => thm.Id_mon_1, mh => mh.Id, (thm, mh) => new {thm.Id, mh.Id_don_vi})
                       .Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckTrungTen(int idDonvi, string ten, int? excludeId = null)
        {
            var ten_input = ten?.Trim().ToLower().Replace(" ", "") ?? "";

            var sql = excludeId == null
                ? $"SELECT thm.Ten FROM Monhoc_Tohopmon thm join DM_Monhoc mh on thm.Id_mon_1 = mh.Id where mh.Id_don_vi = {idDonvi} "
                : $"SELECT thm.Ten FROM Monhoc_Tohopmon thm join DM_Monhoc mh on thm.Id_mon_1 = mh.Id where mh.Id_don_vi = {idDonvi} and p.Id != {excludeId}";

            var ds_ten = _dbContext.Database.SqlQueryRaw<string>(sql).ToList();

            return ds_ten.Any(existingName =>
            {
                var ten_tontai = existingName?.Trim().ToLower().Replace(" ", "") ?? "";
                return ten_input == ten_tontai;
            });
        }
        public bool CheckTrung(Monhoc_Tohopmon thm)
        {
            try
            {
                bool check = _dbContext.Monhoc_Tohopmon.Any(c => c.Id_ban == thm.Id_ban && c.Id_khoi==thm.Id_khoi && (c.Id_mon_1 == thm.Id_mon_1 || c.Id_mon_1==thm.Id_mon_2 || c.Id_mon_1 == thm.Id_mon_3) 
                                && (c.Id_mon_2 == thm.Id_mon_1 || c.Id_mon_2 == thm.Id_mon_2 || c.Id_mon_2 == thm.Id_mon_3) && (c.Id_mon_3 == thm.Id_mon_1 || c.Id_mon_3 == thm.Id_mon_2 || c.Id_mon_3 == thm.Id_mon_3));

                return check;
            }
            catch { return true; }
        }
    }
}
