using DocumentFormat.OpenXml.InkML;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class DM_HocsinhRepository : IDM_HocsinhRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_HocsinhRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Hocsinh_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi)
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

                var result = _dbContext.Set<DM_Hocsinh_List>().FromSqlRaw("EXEC [DM_Hocsinh_GetList_Paging] @pageIndex, @pageSize, @search,@idDonvi",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi).ToList();
                if (result == null) result = new List<DM_Hocsinh_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Hocsinh GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var Lopontap = _dbContext.DM_Hocsinh.FirstOrDefault(c => c.Id == Id && c.Id_don_vi == idDonvi);
                return Lopontap;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool CheckMa(string Ma, int idDonvi, int? Id)
        {
            try
            {
                var query = _dbContext.DM_Hocsinh.Where(c => c.Ma == Ma && c.Id_don_vi == idDonvi);

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
        public bool Add(DM_Hocsinh hs)
        {
            try
            {
                _dbContext.DM_Hocsinh.Add(hs);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Hocsinh hs)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Hocsinh.Update(hs);
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
                var hs = _dbContext.DM_Hocsinh.FirstOrDefault(c => c.Id == Id);
                _dbContext.DM_Hocsinh.Remove(hs);
                _dbContext.SaveChanges();
                return true;
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
                return _dbContext.DM_Hocsinh.Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.DM_Hocsinh.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }

    }
}
