using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Thongtin_DonviRepository: IThongtin_DonviRepository
    {
        private readonly NA_DbContext _context;
        public Thongtin_DonviRepository(NA_DbContext dbContext) { 
            _context = dbContext;
        }
        public List<Ca_DonviDto> GetlistCabyDonvi(int id)
        {
            try
            {
                var list = (from cadv in _context.Ca_Donvi
                            join ca in _context.DM_Cahoc on cadv.Id_ca_hoc equals ca.Id
                            where cadv.Id_don_vi == id
                            select new Ca_DonviDto { Id_ca_hoc = cadv.Id_ca_hoc, Ten_ca = ca.Ten, So_tiet = cadv.So_tiet }).ToList();
                if (list == null) return new List<Ca_DonviDto>();
                return list;
            }
            catch
            {
                return new List<Ca_DonviDto>();
            }
        }
        public bool Update_ThongTinDv(DM_Donvi dm_donvi)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.DM_Donvi.Update(dm_donvi);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateCa(int Id, List<Ca_Donvi> caId)
        {
            try
            {
                var del = _context.Ca_Donvi.Where(x => x.Id_don_vi == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Ca_Donvi.RemoveRange(del);
                }
                for (int i = 0; i < caId.Count; i++)
                {
                    var cadv = new Ca_Donvi
                    {
                        Id_don_vi = Id,
                        Id_ca_hoc = caId[i].Id_ca_hoc,
                        So_tiet = caId[i].So_tiet,
                    };
                    _context.Ca_Donvi.Add(cadv);
                }
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
