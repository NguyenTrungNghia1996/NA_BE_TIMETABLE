using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository;
using NA_Logic.IRepository.LichThi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class DM_DonviHanhChinhRepository: IDM_DonviHanhchinhRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_DonviHanhChinhRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_DonviHanhchinh> GetList_Tinh_Paging( string search)
        {
            try
            {

                var paramSearch = new SqlParameter("search", SqlDbType.NVarChar)
                {
                    Value = search ?? string.Empty
                };

                var result = _dbContext.Set<DM_DonviHanhchinh>().FromSqlRaw("EXEC DM_DonviHanhchinh_Tinh_GetList @search",
                    paramSearch)
                    .ToList();
                if (result == null) result = new List<DM_DonviHanhchinh>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<DM_DonviHanhchinh> GetList_Xa_Paging( string search, int IdTinh)
        {
            try
            {

                var paramSearch = new SqlParameter("search", SqlDbType.NVarChar)
                {
                    Value = search ?? string.Empty
                };
                var paramIdTinh = new SqlParameter("Id_tinh", SqlDbType.Int)
                {
                    Value = IdTinh
                };

                var result = _dbContext.Set<DM_DonviHanhchinh>().FromSqlRaw("EXEC DM_DonviHanhchinh_Xa_GetList @search, @Id_tinh",
                    paramSearch, paramIdTinh)
                    .ToList();
                if (result == null) result = new List<DM_DonviHanhchinh>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool CheckId(int Id)
        {
            if (Id <= 0) return false;
            try
            {
                return _dbContext.DM_DonviHanhchinh.Any(c => c.Id == Id && c.Id_cha.HasValue);
            }
            catch
            {
                return false;
            }
        }


    }
}
