using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Danh_muc;
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
        public List<int> GetlistCapbyDonvi(int id)
        {
            try
            {
                var list = _context.Cap_Donvi.Where(x => x.Id_Don_vi == id).Select(x => x.Id_Cap_hoc).ToList();
                if (list == null) return new List<int>();
                return list;
            }
            catch
            {
                return new List<int>();
            }
        }
        public List<DM_Donvi_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord)
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
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _context.Set<DM_Donvi_List>().FromSqlRaw("EXEC DM_Donvi_GetList_Paging @pageIndex, @pageSize, @search, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch,  paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Donvi_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Donvi dm_donvi)
        {
            try
            {
                _context.DM_Donvi.Add(dm_donvi);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddCap(int Id, List<int> capId)
        {
            try
            {
                if (capId != null && capId.Count > 0)
                {
                    for (int i = 0; i < capId.Count; i++)
                    {
                        var capDonvi = new Cap_Donvi
                        {
                            Id_Don_vi = Id,
                            Id_Cap_hoc = capId[i]
                        };
                        _context.Cap_Donvi.Add(capDonvi);
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
        public bool Update(DM_Donvi dm_donvi)
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
        public bool UpdateCap(int Id, List<int> capId)
        {
            try
            {
                var del = _context.Cap_Donvi.Where(x => x.Id_Don_vi == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Cap_Donvi.RemoveRange(del);
                }
                for (int i = 0; i < capId.Count; i++)
                {
                    var capDv = new Cap_Donvi
                    {
                        Id_Don_vi = Id,
                        Id_Cap_hoc = capId[i]
                    };
                    _context.Cap_Donvi.Add(capDv);
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
                DM_Donvi donvi = new DM_Donvi();
                donvi = _context.DM_Donvi.Find(Id);
                if (donvi != null)
                {
                    _context.DM_Donvi.Remove(donvi);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteCap(int Id)
        {
            try
            {
                var del = _context.Cap_Donvi.Where(x => x.Id_Don_vi == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Cap_Donvi.RemoveRange(del);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}