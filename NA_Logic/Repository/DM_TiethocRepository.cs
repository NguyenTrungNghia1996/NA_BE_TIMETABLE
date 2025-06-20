using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Logic.IRepository;

namespace NA_Logic.Repository
{
    public class DM_TiethocRepository : IDM_TiethocRepository
    {
        private readonly NA_DbContext _context;
        public DM_TiethocRepository(NA_DbContext context)
        {
            _context = context;
        }

        public List<DM_Tiethoc_List> GetList_Paging(int PageIndex, int PageSize, string search, int IdDonvi, ref int totalrecord)
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
                var paramIdDonvi = new SqlParameter("Id_Donvi", SqlDbType.Int)
                {
                    Value = IdDonvi
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _context.Set<DM_Tiethoc_List>().FromSqlRaw("EXEC DM_Tiethoc_GetList_Paging @pageIndex, @pageSize, @search, @Id_Donvi, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Tiethoc_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Tiethoc getDetailById(int id)
        {
            try
            {
                var data = _context.DM_Tiethoc.FirstOrDefault(c=>c.Id == id);
                return data;
            }
            catch
            {
                return null;
            }
        }
        public List<int> GetlistCabyTiethoc(int id)
        {
            try
            {
                var list = _context.Ca_Tiethoc.Where(x => x.Id_Tiet_hoc == id).Select(x => x.Id_Ca_hoc).ToList();
                if (list == null) return new List<int>();
                return list;
            }
            catch
            {
                return new List<int>();
            }
        }
        public bool Add(DM_Tiethoc dM_Tiethoc)
        {
            try
            {
                _context.DM_Tiethoc.Add(dM_Tiethoc);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddCa(int Id, List<int> caId)
        {
            try
            {
                if (caId != null && caId.Count > 0)
                {
                    for (int i = 0; i < caId.Count; i++)
                    {
                        var caTiethoc = new Ca_Tiethoc
                        {
                            Id_Tiet_hoc = Id,
                            Id_Ca_hoc = caId[i]
                        };
                        _context.Ca_Tiethoc.Add(caTiethoc);
                    }
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Update(DM_Tiethoc dM_Tiethoc)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.DM_Tiethoc.Update(dM_Tiethoc);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateCa(int Id, List<int> caId)
        {
            try
            {
                var del = _context.Ca_Tiethoc.Where(x => x.Id_Tiet_hoc == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Ca_Tiethoc.RemoveRange(del);
                }
                for (int i = 0; i < caId.Count; i++)
                {
                    var Catiethoc = new Ca_Tiethoc
                    {
                        Id_Tiet_hoc = Id,
                        Id_Ca_hoc = caId[i]
                    };
                    _context.Ca_Tiethoc.Add(Catiethoc);
                }
                _context.SaveChanges();
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
                DM_Tiethoc item = new DM_Tiethoc();
                item = _context.DM_Tiethoc.Find(Id);
                if (item != null)
                {
                    _context.DM_Tiethoc.Remove(item);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteCa(int Id)
        {
            try
            {
                var del = _context.Ca_Tiethoc.Where(x => x.Id_Tiet_hoc == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Ca_Tiethoc.RemoveRange(del);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckId(int idTiet, int idDonvi, int idCa)
        {
            if (idTiet <= 0) return false;
            try
            {
                // Check ca thuộc đơn vị trước
                var caValid = _context.DM_Cahoc
                    .AsNoTracking()
                    .Any(c => c.Id == idCa && c.Id_Donvi == idDonvi);

                if (!caValid) return false;

                // Sau đó check tiết trong ca
                return _context.Ca_Tiethoc
                    .AsNoTracking()
                    .Any(ct => ct.Id_Tiet_hoc == idTiet && ct.Id_Ca_hoc == idCa);
            }
            catch
            {
                return false;
            }
        }

    }
}