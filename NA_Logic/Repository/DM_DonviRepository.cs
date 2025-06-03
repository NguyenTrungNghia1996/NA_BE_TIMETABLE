using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danhmuc;
using NA_Logic.IRepository;

namespace NA_Logic.Repository
{
    public class DM_DonviRepository : IDM_DonviRepository
    {
        private readonly NA_DbContext _context;
         public DM_DonviRepository(NA_DbContext context)
        {
            _context = context;
        }
        public DM_Donvi getDonviById(int id)
        {
            try
            {
                var data = _context.DM_Donvi.Find(id);
                return data;
            }
            catch
            {
                return null;
            }
        }
    }
}