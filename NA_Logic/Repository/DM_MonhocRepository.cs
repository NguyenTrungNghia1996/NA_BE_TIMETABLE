using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
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
        public bool CheckId(int Id, int idDonvi)
        {
            if (Id <= 0) return false;
            try
            {
                return _context.Dm_Monhoc.Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public Mon_banDto GetListTietBan(int Id, int idDonvi)
        {
            var dsCa = _context.DM_Cahoc.Where(c => c.Id_Donvi == idDonvi).ToList();
            var dsNgay = _context.DM_Ngayhoc.Where(c => c.Id_Donvi == idDonvi).ToList();
            var dsTiet = _context.DM_Tiethoc.Where(c => c.Id_Donvi == idDonvi).ToList();
            var dsCaTiet = _context.Ca_Tiethoc.ToList();

            var tietBan = _context.Tiet_Tranh_Xep
                        .Where(tb => tb.Id_mon == Id)
                        .Select(tb => new { tb.Id_ca, tb.Id_thu, tb.Id_tiet })
                        .ToList();
            var result = new Mon_banDto
            {
                Id = Id,
                Ds_Ca = dsCa.Select(ca => new Ca_banDto
                {
                    Id = ca.Id,
                    Ds_Ngay = dsNgay.Select(ngay => new Ngay_banDto
                    {
                        Id = ngay.Id,
                        Ds_Tiet = dsTiet
                            .Where(tiet => dsCaTiet.Any(ct => ct.Id_Ca_hoc == ca.Id && ct.Id_Tiet_hoc == tiet.Id))
                            .Select(tiet => new TietbanDto
                            {
                                Id = tiet.Id,
                                Trang_thai = tietBan.Any(td => td.Id_ca == ca.Id &&
                                                               td.Id_thu == ngay.Id &&
                                                               td.Id_tiet == tiet.Id)
                            }).ToList()
                    }).ToList()
                }).ToList()
            };

            return result;
        }
        public bool AddTietBan(List<Tiet_tranh_xep> dsTietTranhXep, int idMon)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var existingTiet = _context.Tiet_Tranh_Xep.Where(tb => tb.Id_mon == idMon).ToList();

                //xóa
                if (existingTiet.Any())
                {
                    _context.BulkDelete(existingTiet);
                }
                //thêm
                if (dsTietTranhXep != null && dsTietTranhXep.Any())
                {
                    _context.BulkInsert(dsTietTranhXep);
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return false;
            }
        }
        public bool CheckIds_Tiet(int idNgay, int idCa, int idTiet, int idDonvi)
        {
            try
            {
                var paramIdNgay = new SqlParameter("@IdNgay", SqlDbType.Int) { Value = idNgay };
                var paramIdCa = new SqlParameter("@IdCa", SqlDbType.Int) { Value = idCa };
                var paramIdTiet = new SqlParameter("@IdTiet", SqlDbType.Int) { Value = idTiet };
                var paramIdDonvi = new SqlParameter("@IdDonvi", SqlDbType.Int) { Value = idDonvi };

                var paramResult = new SqlParameter("@Result", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };

                _context.Database.ExecuteSqlRaw(
                    "EXEC CheckIds_Tietban @IdNgay, @IdCa, @IdTiet, @IdDonvi, @Result OUTPUT",
                    paramIdNgay, paramIdCa, paramIdTiet, paramIdDonvi, paramResult);

                return Convert.ToBoolean(paramResult.Value);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteTietTranhXep(int Id)
        {
            try
            {
                var tietban = _context.Tiet_Tranh_Xep.Where(c => c.Id_mon == Id).ToList();
                if (tietban.Count > 0)
                {
                    _context.Tiet_Tranh_Xep.RemoveRange(tietban);
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