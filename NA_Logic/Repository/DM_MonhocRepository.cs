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
    public class DM_MonhocRepository : IDM_MonhocRepository
    {
        private readonly NA_DbContext _context;
        public DM_MonhocRepository(NA_DbContext context)
        {
            _context = context;
        }

        public List<DM_Monhoc_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, ref int totalrecord)
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
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _context.Set<DM_Monhoc_List>().FromSqlRaw("EXEC DM_Monhoc_GetList_Paging @pageIndex, @pageSize, @search, @idDonvi, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Monhoc_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Monhoc GetDetailById(int id, int idDonvi)
        {
            try
            {
                var data = _context.Dm_Monhoc.FirstOrDefault(c => c.Id == id && c.Id_don_vi==idDonvi);
                return data;
            }
            catch
            {
                return null;
            }
        }
        public List<int> GetlistKhoikienthucbyMon(int id)
        {
            try
            {
                var list = _context.Mon_Khoikienthuc.Where(x => x.Id_mon == id).Select(x => x.Id_khoi_kien_thuc).ToList();
                if (list == null) return new List<int>();
                return list;
            }
            catch
            {
                return new List<int>();
            }
        }
        public bool Add(DM_Monhoc dM_Monhoc)
        {
            try
            {
                _context.Dm_Monhoc.Add(dM_Monhoc);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddKhoikienthuc(int Id, List<int> khoiId)
        {
            try
            {
                if (khoiId != null && khoiId.Count > 0)
                {
                    for (int i = 0; i < khoiId.Count; i++)
                    {
                        var mon_khoi = new Mon_Khoikienthuc
                        {
                            Id_mon = Id,
                            Id_khoi_kien_thuc = khoiId[i]
                        };
                        _context.Mon_Khoikienthuc.Add(mon_khoi);
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
        public bool Update(DM_Monhoc dM_Monhoc)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.Dm_Monhoc.Update(dM_Monhoc);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateKhoikienthuc(int Id, List<int> khoiId)
        {
            try
            {
                var del = _context.Mon_Khoikienthuc.Where(x => x.Id_mon == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Mon_Khoikienthuc.RemoveRange(del);
                }
                for (int i = 0; i < khoiId.Count; i++)
                {
                    var mon_khoi = new Mon_Khoikienthuc
                    {
                        Id_mon = Id,
                        Id_khoi_kien_thuc = khoiId[i]
                    };
                    _context.Mon_Khoikienthuc.Add(mon_khoi);
                }
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(int Id, int idDonvi)
        {
            try
            {
                DM_Monhoc item = new DM_Monhoc();
                item = _context.Dm_Monhoc.FirstOrDefault(c => c.Id == Id && c.Id_don_vi == idDonvi);
                if (item != null)
                {
                    _context.Dm_Monhoc.Remove(item);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteKhoi(int Id)
        {
            try
            {
                var del = _context.Mon_Khoikienthuc.Where(x => x.Id_mon == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Mon_Khoikienthuc.RemoveRange(del);
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