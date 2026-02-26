using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class DM_Tohopmon_OntapRepository : IDM_Tohopmon_OntapRepository
    {
        private readonly NA_DbContext _context;
        public DM_Tohopmon_OntapRepository(NA_DbContext context)
        {
            _context = context;
        }
        
        public List<DM_Tohopmon_Ontap_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi)
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
                var result = _context.Set<DM_Tohopmon_Ontap_List>().FromSqlRaw("EXEC DM_Tohopmon_Ontap_GetList_Paging @pageIndex, @pageSize, @search, @idDonvi",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi)
                    .ToList();
                if (result == null) result = new List<DM_Tohopmon_Ontap_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Tohopmon_Ontap GetDetailById(int id, int idDonvi)
        {
            try
            {
                var data = _context.DM_Tohopmon_Ontap.FirstOrDefault(c=>c.Id == id && c.Id_don_vi == idDonvi);
                return data;
            }
            catch
            {
                return null;
            }
        }
        public List<int> GetlistMon(int id)
        {
            try
            {
                var list = _context.Monhoc_Tohop_Ontap.Where(x => x.Id_to_hop == id).Select(x => x.Id_mon).ToList();
                if (list == null) return new List<int>();
                return list;
            }
            catch
            {
                return new List<int>();
            }
        }

        public bool Add(DM_Tohopmon_Ontap thm)
        {
            try
            {
                _context.DM_Tohopmon_Ontap.Add(thm);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckMa(string Ma, int idDonvi, int? Id)
        {
            try
            {
                var query = _context.DM_Tohopmon_Ontap.Where(c => c.Ma == Ma && c.Id_don_vi == idDonvi);

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
        public bool AddMon(int Id, List<int> monId)
        {
            try
            {
                if (monId != null && monId.Count > 0)
                {
                    for (int i = 0; i < monId.Count; i++)
                    {
                        var monTH = new Monhoc_Tohop_Ontap
                        {
                            Id_to_hop = Id,
                            Id_mon = monId[i]
                        };
                        _context.Monhoc_Tohop_Ontap.Add(monTH);
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

        public bool Update(DM_Tohopmon_Ontap thm)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.DM_Tohopmon_Ontap.Update(thm);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateMon(int Id, List<int> monId)
        {
            try
            {
                var del = _context.Monhoc_Tohop_Ontap.Where(x => x.Id_to_hop == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Monhoc_Tohop_Ontap.RemoveRange(del);
                }
                for (int i = 0; i < monId.Count; i++)
                {
                    var monTH = new Monhoc_Tohop_Ontap
                    {
                        Id_to_hop = Id,
                        Id_mon = monId[i]
                    };
                    _context.Monhoc_Tohop_Ontap.Add(monTH);
                }
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckContraint(int id, int idDonvi)
        {
            try
            {
                bool check = (from thm in _context.DM_Tohopmon_Ontap.AsNoTracking()
                              join ht in _context.Hocsinh_Tohopmon.AsNoTracking() on thm.Id equals ht.Id_to_hop
                              where thm.Id_don_vi == idDonvi && ht.Id_to_hop == id
                              select 1).Any();
                return check;
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
                DM_Tohopmon_Ontap thm = new DM_Tohopmon_Ontap();
                thm = _context.DM_Tohopmon_Ontap.Find(Id);
                if (thm == null)
                {
                    return false;
                }
                _context.DM_Tohopmon_Ontap.Remove(thm);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DeleteMon(int Id)
        {
            try
            {
                var del = _context.Monhoc_Tohop_Ontap.Where(x => x.Id_to_hop == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Monhoc_Tohop_Ontap.RemoveRange(del);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckId(int Id, int idDonvi)
        {
            if (Id <= 0) return false;
            try
            {
                return _context.DM_Tohopmon_Ontap.Any(c => c.Id == Id);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids, int idDonvi)
        {
            var existingIds = _context.DM_Tohopmon_Ontap.Where(c => c.Id_don_vi == idDonvi && ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
    }
}