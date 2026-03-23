using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository.XepGiamThi
{
    public class DM_DantocRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_DantocRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Dantoc> GetList_Paging(string search)
        {
            try
            {

                var paramSearch = new SqlParameter("search", SqlDbType.NVarChar)
                {
                    Value = search ?? string.Empty
                };

                var result = _dbContext.Set<DM_Dantoc>().FromSqlRaw("EXEC DM_Dantoc_GetList @search",
                    paramSearch)
                    .ToList();
                if (result == null) result = new List<DM_Dantoc>();
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
                return _dbContext.DM_Dantoc.Any(c => c.Id == Id);
            }
            catch
            {
                return false;
            }
        }
    }
}
