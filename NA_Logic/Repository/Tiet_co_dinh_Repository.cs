using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Tiet_co_dinhRepository : ITiet_co_dinhRepository
    {
        private readonly NA_DbContext _dbContext;
        public Tiet_co_dinhRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Tiet_co_dinh_List> GetList_Paging(int PageIndex, int PageSize,  int IdDonvi, ref int totalrecord)
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
        public Tiet_co_dinh GetDetailById(int Id)
        {
            try
            {
                var tietcodinh = _dbContext.Tiet_co_dinh.FirstOrDefault(c => c.Id == Id );
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
                // Lấy danh sách khối đã có
                var existingKhoiIds = _dbContext.Tiet_co_dinh
                    .Where(x => x.Id_mon == idMon)
                    .Select(x => x.Id_khoi_lop)
                    .Distinct()
                    .ToList();

                // Lọc chỉ các khối chưa có
                var recordsToAdd = tiet_cd
                    .Where(x => !existingKhoiIds.Contains(x.Id_khoi_lop))
                    .ToList();
                //thêm các khối chưa có
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
                return _dbContext.Dm_Monhoc
                    .AsNoTracking()
                    .Where(mon => mon.Id == idMon && mon.Id_don_vi == idDonvi)
                    .Any(mon =>
                        _dbContext.Ngay_Donvi.AsNoTracking()
                            .Any(ngay => ngay.Id_ngay == idNgay && ngay.Id_don_vi == idDonvi) &&
                        _dbContext.DM_Cahoc.AsNoTracking()
                            .Any(ca => ca.Id == idCa) &&
                        _dbContext.Cap_Donvi.AsNoTracking()
                            .Join(_dbContext.DM_Khoilop.AsNoTracking(),
                                cd => cd.Id_Cap_hoc,
                                kl => kl.Id_Cap_hoc,
                                (cd, kl) => new { cd, kl })
                            .Any(x => x.kl.Id == idKhoi && x.cd.Id_Don_vi == idDonvi) &&
                        _dbContext.Ca_Tiethoc.AsNoTracking()
                            .Any(ct => ct.Id_Tiet_hoc == idTiet && ct.Id_Ca_hoc == idCa)
                    );
            }
            catch
            {
                return false;
            }
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
