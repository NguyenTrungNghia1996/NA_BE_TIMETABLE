using ClosedXML.Excel;
using NA_Entities.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Ketqua_BaikiemtraRepository
    {
        private readonly NA_DbContext _context;
        public Ketqua_BaikiemtraRepository(NA_DbContext context) { 
            context = _context;
        }

    }
}
