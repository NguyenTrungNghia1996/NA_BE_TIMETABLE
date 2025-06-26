using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface INgay_DonviRepository
    {
        List<int> GetlistNgaybyDonvi(int id);
        public bool UpdateNgay(int Id, List<int> ngayId);
    }
}
