using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class DM_GiaovienRepository : IDM_GiaovienRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_GiaovienRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Giaovien_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord)
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
                var result = _dbContext.Set<DM_Giaovien_List>().FromSqlRaw("EXEC DM_Giaovien_GetList_Paging @pageIndex, @pageSize, @search, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Giaovien_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Giaovien GetDetailById(int Id)
        {
            try
            {
                var Giaovien = _dbContext.DM_Giaovien.FirstOrDefault(c => c.Id == Id);
                return Giaovien;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Giaovien dm_Giaovien)
        {
            try
            {
                _dbContext.DM_Giaovien.Add(dm_Giaovien);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Giaovien dm_Giaovien)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Giaovien.Update(dm_Giaovien);
                _dbContext.SaveChanges();
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
                DM_Giaovien Giaovien = new DM_Giaovien();
                Giaovien = _dbContext.DM_Giaovien.FirstOrDefault(c => c.Id == Id);
                if (Giaovien != null)
                {
                    _dbContext.DM_Giaovien.Remove(Giaovien);
                    _dbContext.SaveChanges();
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
                return _dbContext.DM_Giaovien.Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.DM_Giaovien.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }

        public Giaovien_banDto GetListTietBan(int Id, int idDonvi)
        {
            var dsCa = _dbContext.Ca_Donvi.Where(cd => cd.Id_don_vi == idDonvi)
                            .Join(_dbContext.DM_Cahoc,
                                  cd => cd.Id_ca_hoc,
                                  ca => ca.Id,
                                  (cd, ca) => new
                                  {
                                      Id = ca.Id,
                                      Ten = ca.Ten
                                  }).ToList();
            var tietBan = _dbContext.Giaovien_Tiettranhxep
                        .Where(tb => tb.Id_giao_vien == Id)
                        .Select(tb => new { tb.Id_ca, tb.Ngay, tb.Tiet })
                        .ToList();
            var buoiday = _dbContext.Giaovien_Buoiday.FirstOrDefault(bd => bd.Id_giao_vien == Id);
            // Lấy danh sách ngày từ enum
            var dsNgay = Enum.GetValues<Ngay>().ToList();
            // Lấy danh sách tiết từ enum
            var dsTiet = Enum.GetValues<Tiet>().ToList();

            var result = new Giaovien_banDto
            {
                Id_giao_vien = Id,
                Id_buoi_day = buoiday.Id ,
                Chi_day_mot_buoi = buoiday.Chi_day_mot_buoi,
                So_tiet_toi_da = buoiday.So_tiet_toi_da,
                Ds_Ca = dsCa.Select(ca => new Ca_banDto
                {
                    Id = ca.Id,
                    Ds_Ngay = dsNgay.Select(ngay => new Ngay_banDto
                    {
                        Id = ngay,
                        Ten = ngay.GetDisplayName(),
                        Ds_Tiet = dsTiet.Select(tiet => new TietbanDto
                        {
                            Id = tiet,
                            Ten = tiet.GetDisplayName(),
                            Trang_thai = tietBan.Any(td => td.Id_ca == ca.Id &&
                                                           td.Ngay == (int)ngay &&
                                                           td.Tiet == (int)tiet)
                        }).ToList()
                    }).ToList()
                }).ToList()
            }; 

            return result;
        }
        public bool AddTietBan(List<Giaovien_Tiettranhxep> dsTietTranhXep, int Idgv)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var existingTiet = _dbContext.Giaovien_Tiettranhxep.Where(tb => tb.Id_giao_vien == Idgv).ToList();

                //xóa
                if (existingTiet.Any())
                {
                    _dbContext.BulkDelete(existingTiet);
                }
                //thêm
                if (dsTietTranhXep != null && dsTietTranhXep.Any())
                {
                    _dbContext.BulkInsert(dsTietTranhXep);
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
        public bool SaveBuoiday(Giaovien_Buoiday gvbd)
        {
            try
            {
                if (gvbd.Id!= 0) { 
                    _dbContext.Giaovien_Buoiday.Update(gvbd);
                }
                else
                {
                    _dbContext.Giaovien_Buoiday.Add(gvbd);
                }
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public Giaovien_Buoiday GetBuoidayTheoGV(int id)
        {
            try
            {
                if (id != 0)
                {
                    return null;
                }
                var buoiday = _dbContext.Giaovien_Buoiday.FirstOrDefault(bd => bd.Id_giao_vien == id);
                return buoiday;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
