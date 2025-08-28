using Microsoft.Data.SqlClient;
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
    public class DM_TinhRepository : IDM_TinhRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_TinhRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Tinh> GetList()
        {
            try
            {
                var result = _dbContext.DM_Tinh.ToList();
                if (result == null) result = new List<DM_Tinh>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
