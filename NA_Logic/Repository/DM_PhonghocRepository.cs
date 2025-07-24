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

        public List<DM_Phonghoc_list> GetList_Paging(int PageIndex, int PageSize, string search, int idDiemtruong,int idLoaiph, int idDonvi, ref int totalrecord)
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
                var paramIdLoaiph = new SqlParameter("idLoaiPhonghoc", SqlDbType.Int)
                {
                    Value = idLoaiph
                };
                var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int)
                {
                    Value = idDonvi
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _context.Set<DM_Phonghoc_list>().FromSqlRaw("EXEC DM_Phonghoc_GetList_Paging @pageIndex, @pageSize, @search, @idDiemtruong, @idLoaiPhonghoc ,@idDonvi, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDiemtruong, paramIdLoaiph,paramIdDonvi, paramTotal)
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
        public DM_Phonghoc getDetailById(int id)
        {
            try
            {
                var data = _context.DM_Phonghoc.FirstOrDefault(c => c.Id == id );
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
        public bool CheckId(int Id)
        {
            if (Id <= 0) return false;
            try
            {
                return _context.DM_Phonghoc.Any(c => c.Id == Id);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckMa(string Ma)
        {
            try
            {
                var check = _context.DM_Phonghoc.Any(c => c.Ma == Ma );
                if (!check)
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
        public bool CheckIds(IEnumerable<int> ids)
        {
            var existingIds = _context.DM_Phonghoc.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
        public Phong_banDto GetListTietBan(int Id)
        {
            var dsCa = _context.DM_Cahoc.ToList();
            var tietBan = _context.Tiet_ban
                        .Where(tb => tb.Id_phong == Id)
                        .Select(tb => new { tb.Id_ca, tb.Thu, tb.Tiet })
                        .ToList();

            // Lấy danh sách ngày, tiết từ enum
            var dsNgay = Enum.GetValues<Ngay>().ToList();
            var dsTiet = Enum.GetValues<Tiet>().ToList();

            var result = new Phong_banDto
            {
                Id = Id,
                Ds_Ca = dsCa.Select(ca => new Ca_banDto
                {
                    Id = ca.Id,
                    Ds_Ngay = dsNgay.Select(ngay => new Ngay_banDto
                    {
                        Id = ngay,
                        Ds_Tiet = dsTiet.Select(tiet => new TietbanDto
                        {
                            Id = tiet,
                            Trang_thai = tietBan.Any(td => td.Id_ca == ca.Id &&
                                                           td.Thu == (int)ngay &&
                                                           td.Tiet == (int)tiet)
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            return result;
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
        //public bool CheckIds_Tietban(int idNgay, int idCa, int idTiet)
        //{
        //    try
        //    {
        //        var paramIdNgay = new SqlParameter("@IdNgay", SqlDbType.Int) { Value = idNgay };
        //        var paramIdCa = new SqlParameter("@IdCa", SqlDbType.Int) { Value = idCa };
        //        var paramIdTiet = new SqlParameter("@IdTiet", SqlDbType.Int) { Value = idTiet };
        //        var paramIdDonvi = new SqlParameter("@IdDonvi", SqlDbType.Int) { Value = idDonvi };

        //        var paramResult = new SqlParameter("@Result", SqlDbType.Bit)
        //        {
        //            Direction = ParameterDirection.Output
        //        };

        //        _context.Database.ExecuteSqlRaw(
        //            "EXEC CheckIds_Tietban @IdNgay, @IdCa, @IdTiet, @IdDonvi, @Result OUTPUT",
        //            paramIdNgay, paramIdCa, paramIdTiet, paramIdDonvi, paramResult);

        //        return Convert.ToBoolean(paramResult.Value);
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
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