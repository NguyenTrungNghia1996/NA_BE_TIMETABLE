using DocumentFormat.OpenXml.InkML;
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
    public class DM_LopontapRepository : IDM_LopontapRepository
    {
        private readonly NA_DbContext _dbContext;
        public DM_LopontapRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DM_Lopontap_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi)
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

                var result = _dbContext.Set<DM_Lopontap_List>().FromSqlRaw("EXEC DM_Lopontap_GetList_Paging @pageIndex, @pageSize, @search,@idDonvi",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi).ToList();
                if (result == null) result = new List<DM_Lopontap_List>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Lopontap GetDetailById(int Id)
        {
            try
            {
                var Lopontap = _dbContext.DM_Lopontap.FirstOrDefault(c => c.Id == Id);
                return Lopontap;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Lopontap dm_Lopontap)
        {
            try
            {
                _dbContext.DM_Lopontap.Add(dm_Lopontap);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(DM_Lopontap dm_Lopontap)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.DM_Lopontap.Update(dm_Lopontap);
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
                var Lopontap = _dbContext.DM_Lopontap.FirstOrDefault(c => c.Id == Id);
                _dbContext.DM_Lopontap.Remove(Lopontap);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public Lopontap_TietnghiDto GetListTietBan(int Id, int idDonvi)
        {
            var dsCa = _dbContext.Ca_Donvi.Where(cd => cd.Id_don_vi == idDonvi)
                            .Join(_dbContext.DM_Cahoc,
                                  cd => cd.Id_ca_hoc,
                                  ca => ca.Id,
                                  (cd, ca) => new
                                  {
                                      Id = ca.Id,
                                      Ten = ca.Ten,
                                      So_tiet = cd.So_tiet,
                                  }).ToList();
            var tietBan = _dbContext.Tiet_ban
                        .Where(tb => tb.Id_phong == Id)
                        .Select(tb => new { tb.Id_ca, tb.Thu, tb.Tiet })
                        .ToList();
            var donvi = _dbContext.DM_Donvi.Find(idDonvi);
            // Lấy danh sách ngày từ enum
            var dsNgay = Enum.GetValues<Ngay>().Take(donvi.So_ngay).ToList();
            // Lấy danh sách tiết từ enum
            var dsTiet = Enum.GetValues<Tiet>().ToList();

            var result = new Lopontap_TietnghiDto
            {
                Id = Id,
                Ds_Ca = dsCa.Select(ca => new Ca_banDto
                {
                    Id = ca.Id,
                    Ds_Ngay = dsNgay.Select(ngay => new Ngay_banDto
                    {
                        Id = ngay,
                        Ten = ngay.GetDisplayName(),
                        Ds_Tiet = dsTiet.Take(ca.So_tiet).Select(tiet => new TietbanDto
                        {
                            Id = tiet,
                            Ten = tiet.GetDisplayName(),
                            Trang_thai = tietBan.Any(td => td.Id_ca == ca.Id &&
                                                           td.Thu == (int)ngay &&
                                                           td.Tiet == (int)tiet)
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            return result;
        }
        public bool AddTietBan(List<Lopontap_Tietnghi> dsTietBan, int idLopOn)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var existingTietBan = _dbContext.Lopontap_Tietnghi.Where(tb => tb.Id_lop_on == idLopOn).ToList();

                //xóa
                if (existingTietBan.Any())
                {
                    _dbContext.BulkDelete(existingTietBan);
                }
                //thêm
                if (dsTietBan != null && dsTietBan.Any())
                {
                    _dbContext.BulkInsert(dsTietBan);
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

        public bool DeleteTietBan(int Id)
        {
            try
            {
                var tietban = _dbContext.Lopontap_Tietnghi.Where(c => c.Id_lop_on == Id).ToList();
                if (tietban.Count > 0)
                {
                    _dbContext.Lopontap_Tietnghi.RemoveRange(tietban);
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
                return (from lo in _dbContext.DM_Lopontap
                        join m in _dbContext.Dm_Monhoc on lo.Id_mon equals m.Id
                        where lo.Id == Id && m.Id_don_vi == idDonvi
                        select 1).Any();
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _dbContext.DM_Lopontap.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }

    }
}
