using EFCore.BulkExtensions;
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
    public class DM_PhonghocRepository : IDM_PhonghocRepository
    {
        private readonly NA_DbContext _context;
        public DM_PhonghocRepository(NA_DbContext context)
        {
            _context = context;
        }

        public List<DM_Phonghoc_list> GetList_Paging(int PageIndex, int PageSize, string search, int idDiemtruong,int idDonvi, ref int totalrecord)
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
                var paramIdDiemtruong = new SqlParameter("idDiemtruong", SqlDbType.Int)
                {
                    Value = idDiemtruong
                };
                var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int)
                {
                    Value = idDonvi
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _context.Set<DM_Phonghoc_list>().FromSqlRaw("EXEC DM_Phonghoc_GetList_Paging @pageIndex, @pageSize, @search, @idDiemtruong,@idDonvi, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDiemtruong,paramIdDonvi, paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Phonghoc_list>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DM_Phonghoc getDetailById(int id, int idDonvi)
        {
            try
            {
                var data = _context.DM_Phonghoc.FirstOrDefault(c => c.Id == id && c.Id_Don_vi == idDonvi);
                return data;
            }
            catch
            {
                return null;
            }
        }
        public bool Add(DM_Phonghoc dM_Phonghoc)
        {
            try
            {
                _context.DM_Phonghoc.Add(dM_Phonghoc);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Update(DM_Phonghoc dM_Phonghoc)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.DM_Phonghoc.Update(dM_Phonghoc);
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
                DM_Phonghoc ph = new DM_Phonghoc();
                ph = _context.DM_Phonghoc.Find(Id);
                if (ph != null)
                {
                    _context.DM_Phonghoc.Remove(ph);
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
                return _context.DM_Phonghoc.Any(c => c.Id == Id && c.Id_Don_vi==idDonvi);
            }
            catch
            {
                return false;
            }
        }
        public Phong_banDto GetListTietBan(int Id, int idDonvi)
        {
            var rawData = _context.Database
                .SqlQueryRaw<Tiet_banDto>(
                    "EXEC GetList_TietBanPhong @Id_phong, @Id_donvi",
                    new SqlParameter("@Id_phong", Id),
                    new SqlParameter("@Id_donvi", idDonvi))
                .ToList();

            var result = rawData
                .GroupBy(x => new { x.Id_ca })
                .Select(caGroup => new Ca_banDto
                {
                    Id = caGroup.Key.Id_ca,
                    Ds_Ngay = caGroup
                        .GroupBy(x => new { x.Id_thu, x.Ten_thu })
                        .Select(ngayGroup => new Ngay_banDto
                        {
                            Id = ngayGroup.Key.Id_thu,
                            Ten = ngayGroup.Key.Ten_thu,
                            Ds_Tiet = ngayGroup
                                .Select(item => new TietbanDto
                                {
                                    Id = item.Id_tiet,
                                    Ten = item.Ten_tiet,
                                    Trang_thai = item.Trang_thai
                                }).ToList()
                        }).ToList()
                }).ToList();

            return new Phong_banDto { Id = Id, Ds_Ca = result };
        }
        public bool AddTietBan(List<Tiet_ban> dsTietBan, int idPhong)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var existingTietBan = _context.Tiet_ban.Where(tb => tb.Id_phong == idPhong).ToList();

                //xóa
                if (existingTietBan.Any())
                {
                    _context.BulkDelete(existingTietBan);
                }
                //thêm
                if (dsTietBan != null && dsTietBan.Any())
                {
                    _context.BulkInsert(dsTietBan);
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
        public bool CheckIds_Tietban(int idNgay, int idCa, int idTiet, int idDonvi)
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
        public bool DeleteTietBan(int Id)
        {
            try
            {
                var tietban = _context.Tiet_ban.Where(c => c.Id_phong == Id).ToList();
                if (tietban.Count > 0)
                {
                    _context.Tiet_ban.RemoveRange(tietban);
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