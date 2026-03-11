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
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository.XepThoiKhoaBieu;

namespace NA_Logic.Repository
{
    public class DM_KhoilopRepository : IDM_KhoilopRepository
    {
        private readonly NA_DbContext _context;
        public DM_KhoilopRepository(NA_DbContext context)
        {
            _context = context;
        }

        public List<DM_Khoilop_List> GetList_Paging(int PageIndex, int PageSize, string search,  ref int totalrecord)
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
                var result = _context.Set<DM_Khoilop_List>().FromSqlRaw("EXEC DM_Khoilop_GetList_Paging @pageIndex, @pageSize, @search, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch,  paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Khoilop_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Khoilop getDetailById(int id)
        {
            try
            {
                var data = _context.DM_Khoilop.FirstOrDefault(c => c.Id == id );
                return data;
            }
            catch
            {
                return null;
            }
        }
        public bool Add(DM_Khoilop dM_Khoilop)
        {
            try
            {
                _context.DM_Khoilop.Add(dM_Khoilop);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Khoilop dM_Khoilop)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.DM_Khoilop.Update(dM_Khoilop);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public (bool success, string message) Delete(int Id)
        {
            try
            {
                DM_Khoilop item = new DM_Khoilop();
                item = _context.DM_Khoilop.FirstOrDefault(c => c.Id == Id);
                if (item == null)
                {
                    return (false, "Bản ghi không tồn tại");
                }
                bool check = _context.Tiet_co_dinh.Any(c => c.Id_khoi_lop == Id)
                            || _context.Monhoc_Khoilop.Any(c => c.Id_khoi == Id)
                            || _context.DM_Lophoc.Any(c => c.Id_khoi == Id)
                            || _context.Monhoc_Tohopmon.Any(c=>c.Id_khoi == Id);
                if (check)
                    return (false, "Khối lớp đã có ràng buộc, không thể xoá");

                _context.DM_Khoilop.Remove(item);
                    _context.SaveChanges();
                return (true, "Xoá thành công");
            }
            catch (Exception ex) 
            {
                return (false, $"Lỗi hệ thống: {ex.Message}");
            }
        }
        public List<Khoilop_byDonvi> GetKhoilopByDonvi(int idDonvi, string search)
        {
            try
            {
                var query = _context.Cap_Donvi.Where(cd => cd.Id_Don_vi == idDonvi)
                                    .Join(_context.DM_Caphoc, cd => cd.Id_Cap_hoc, ch => ch.Id, (cd, ch) => ch)
                                    .Join(_context.DM_Khoilop,
                                          ch => ch.Id, kl => kl.Id_Cap_hoc,
                                          (ch, kl) => new Khoilop_byDonvi
                                          {
                                              Id = kl.Id,
                                              Ten = kl.Ten
                                          })
                                    .Distinct();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(kl => kl.Ten.Contains(search));
                }

                return query.ToList();
            }
            catch
            {
                return new List<Khoilop_byDonvi>();
            }
        }
        public bool CheckId(int Id)
        {
            if (Id <= 0) return false;
            try
            {
                return _context.DM_Khoilop.Any(c => c.Id == Id);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckKhoilopByDonvi(int idKhoilop, int idDonvi)
        {
            if (idKhoilop <= 0 || idDonvi <= 0) return false;

            try
            {
                return _context.Cap_Donvi.Where(cd => cd.Id_Don_vi == idDonvi)
                                .Join(_context.DM_Khoilop,
                                      cd => cd.Id_Cap_hoc, kl => kl.Id_Cap_hoc,
                                      (cd, kl) => kl.Id)
                                .Any(id => id == idKhoilop);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _context.DM_Khoilop.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
    }
}