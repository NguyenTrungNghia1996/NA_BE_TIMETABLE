using DocumentFormat.OpenXml.InkML;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository.XepThoiKhoaBieu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NA_Logic.Repository.ThoiKhoaBieu
{
    public class Tiet_co_dinhRepository : ITiet_co_dinhRepository
    {
        private readonly NA_DbContext _dbContext;
        public Tiet_co_dinhRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Tiet_co_dinh_List> GetList_Paging(int PageIndex, int PageSize, int IdDonvi, ref int totalrecord)
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
                var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int)
                {
                    Value = IdDonvi
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _dbContext.Set<Tiet_co_dinh_List>().FromSqlRaw("EXEC Tietcodinh_GetList_Paging @pageIndex, @pageSize, @idDonvi, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramIdDonvi, paramTotal)
                    .ToList();

                if (result == null) result = new List<Tiet_co_dinh_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Tiet_co_dinh GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var tietcodinh = (from c in _dbContext.Tiet_co_dinh
                                  join d in _dbContext.Dm_Monhoc on c.Id_mon equals d.Id
                                  where c.Id == Id && d.Id_don_vi == idDonvi
                                  select c).FirstOrDefault();
                return tietcodinh;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(List<Tiet_co_dinh> tiet_cd)
        {
            try
            {
                _dbContext.BulkInsert(tiet_cd);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(Tiet_co_dinh tiet_cd)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                _dbContext.Tiet_co_dinh.Update(tiet_cd);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateAllKhoi(List<Tiet_co_dinh> tiet_cd, int idMon)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var existingRecords = _dbContext.Tiet_co_dinh.Where(x => x.Id_mon == idMon).ToList();

                var existingDict = existingRecords.ToDictionary(x => x.Id_khoi_lop);
                var recordsToAdd = new List<Tiet_co_dinh>();

                // Update các bản ghi hiện có trong memory
                foreach (var item in tiet_cd)
                {
                    if (existingDict.TryGetValue(item.Id_khoi_lop, out var existing))
                    {
                        existing.Tiet = item.Tiet;
                        existing.Ngay = item.Ngay;
                        existing.Id_ca = item.Id_ca;
                    }
                    else
                    {
                        recordsToAdd.Add(item);
                    }
                }

                if (existingRecords.Any())
                {
                    _dbContext.BulkUpdate(existingRecords);
                }

                if (recordsToAdd.Any())
                {
                    _dbContext.BulkInsert(recordsToAdd);
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
        public bool Delete(int Id)
        {
            try
            {
                Tiet_co_dinh tiet_cd = new Tiet_co_dinh();
                tiet_cd = _dbContext.Tiet_co_dinh.FirstOrDefault(c => c.Id == Id);
                if (tiet_cd != null)
                {
                    _dbContext.Tiet_co_dinh.Remove(tiet_cd);
                    _dbContext.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(int idMon, int idNgay, int idCa, int idTiet, int idKhoi, int idDonvi)
        {
            try
            {
                var check_mon_ca_khoi = (from mon in _dbContext.Dm_Monhoc.AsNoTracking()
                                         where mon.Id == idMon && mon.Id_don_vi == idDonvi
                                         from ca in _dbContext.DM_Cahoc.AsNoTracking()
                                         join cdv in _dbContext.Ca_Donvi.AsNoTracking() on ca.Id equals cdv.Id_ca_hoc
                                         where ca.Id == idCa && cdv.Id_don_vi==idDonvi
                                         from cd in _dbContext.Cap_Donvi.AsNoTracking()
                                         join kl in _dbContext.DM_Khoilop.AsNoTracking() on cd.Id_Cap_hoc equals kl.Id_Cap_hoc
                                         where kl.Id == idKhoi && cd.Id_Don_vi == idDonvi
                                         from cd2 in _dbContext.Cap_Donvi.AsNoTracking()
                                         where cd2.Id_Don_vi == idDonvi
                                         join ch in _dbContext.DM_Caphoc on cd2.Id_Cap_hoc equals ch.Id
                                         join kl2 in _dbContext.DM_Khoilop on ch.Id equals kl2.Id_Cap_hoc
                                         where kl2.Id == idKhoi

                                         select mon
                                        ).Any();
                if (!Enum.IsDefined(typeof(Ngay), idNgay))
                {
                    return false;
                }

                if (!Enum.IsDefined(typeof(Tiet), idTiet))
                {
                    return false;
                }
                if (!check_mon_ca_khoi)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckTrungList(List<Tiet_co_dinh> list_tietcd, int idDonvi)
        {
            try
            {
                foreach (var tietcd in list_tietcd)
                {
                    bool check = (from tcd in _dbContext.Tiet_co_dinh
                                  join m in _dbContext.Dm_Monhoc on tcd.Id_mon equals m.Id
                                  where tcd.Id_ca == tietcd.Id_ca
                                     && tcd.Ngay == tietcd.Ngay
                                     && tcd.Tiet == tietcd.Tiet
                                     && tcd.Id_khoi_lop == tietcd.Id_khoi_lop
                                     && m.Id_don_vi == idDonvi
                                     && (tietcd.Id <= 0 || tcd.Id != tietcd.Id) 
                                  select tcd).Any();

                    if (check) return true;
                }
                return false;
            }
            catch { return true; }
        }
        public bool CheckTrung(Tiet_co_dinh tietcd, int idDonvi)
        {
            try
            {

                bool check = (from tcd in _dbContext.Tiet_co_dinh
                                join m in _dbContext.Dm_Monhoc on tcd.Id_mon equals m.Id
                                where tcd.Id_ca == tietcd.Id_ca
                                    && tcd.Ngay == tietcd.Ngay
                                    && tcd.Tiet == tietcd.Tiet
                                    && tcd.Id_khoi_lop == tietcd.Id_khoi_lop
                                    && m.Id_don_vi == idDonvi
                                    && (tietcd.Id <= 0 || tcd.Id != tietcd.Id) 
                                select tcd).Any();

                if (check) return true;

                return false;
            }
            catch { return true; }
        }
        //public bool CheckId(int Id, int idDonvi)
        //{
        //    if (Id <= 0) return false;
        //    try
        //    {
        //        return _dbContext.DM_Cahoc.Any(c => c.Id == Id && c.Id_Donvi == idDonvi);
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        //public bool CheckIds(IEnumerable<int> ids, int idDonvi)
        //{
        //    var existingIds = _dbContext.DM_Cahoc.Where(c => ids.Contains(c.Id) && c.Id_Donvi == idDonvi).Select(c => c.Id).ToList();
        //    return ids.All(id => existingIds.Contains(id));
        //}

    }
}
