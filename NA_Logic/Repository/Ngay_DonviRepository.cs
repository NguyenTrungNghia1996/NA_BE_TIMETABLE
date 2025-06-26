using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Ngay_DonviRepository: INgay_DonviRepository
    {
        private readonly NA_DbContext _context;
        public Ngay_DonviRepository(NA_DbContext context)
        {
            _context = context;
        }
        public List<int> GetlistNgaybyDonvi(int id)
        {
            try
            {
                var list = _context.Ngay_Donvi.Where(x => x.Id_don_vi == id).Select(x => x.Id_ngay).ToList();
                if (list == null) return new List<int>();
                return list;
            }
            catch
            {
                return new List<int>();
            }
        }
        public bool UpdateNgay(int Id, List<int> ngayId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var del = _context.Ngay_Donvi.Where(x => x.Id_don_vi == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Ngay_Donvi.RemoveRange(del);
                }
                if (ngayId != null && ngayId.Count > 0)
                {
                    for (int i = 0; i < ngayId.Count; i++)
                    {
                        var ngayDonvi = new Ngay_Donvi
                        {
                            Id_don_vi = Id,
                            Id_ngay = ngayId[i]
                        };
                        _context.Ngay_Donvi.Add(ngayDonvi);
                    }
                    _context.SaveChanges();
                    transaction.Commit();
                }
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}
