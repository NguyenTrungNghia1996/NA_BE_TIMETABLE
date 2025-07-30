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
using System.Security.Cryptography;
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

        public List<DM_Monhoc_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, int idloaiphong,int id_lop, ref int totalrecord)
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
                var paramIdLoaiPhong = new SqlParameter("id_loai_phong", SqlDbType.Int)
                {
                    Value = idloaiphong
                };
                var paramIdLop = new SqlParameter("id_lop", SqlDbType.Int)
                {
                    Value = id_lop
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _context.Set<DM_Monhoc_List>().FromSqlRaw("EXEC DM_Monhoc_GetList_Paging @pageIndex, @pageSize, @search, @idDonvi, @id_loai_phong, @id_lop, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi, paramIdLoaiPhong,paramIdLop, paramTotal)
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
        public List<MonLop> GetList_MonLop(int idDonvi,  int id_lop)
        {
            try
            {
                var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int)
                {
                    Value = idDonvi
                };

                var paramIdLop = new SqlParameter("id_lop", SqlDbType.Int)
                {
                    Value = id_lop
                };
                var result = _context.Set<MonLop>().FromSqlRaw("EXEC MonLop_GetList @id_lop, @idDonvi",
                      paramIdLop, paramIdDonvi)
                    .ToList();
                if (result == null) result = new List<MonLop>();
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
        public bool CheckIdMonLop(int Id, int Id_lop, int idDonvi)
        {
            if (Id <= 0) return false;
            try
            {
                var check = _context.Dm_Monhoc.AsNoTracking().Where(lm => lm.Id_don_vi == idDonvi)
                           .Join(_context.Lophoc_Monhoc, lm=>lm.Id, dm => dm.Id_mon, (lm, dm) => new{dm.Id_lop, dm.Id_mon})
                           .Any(x => x.Id_mon==Id && x.Id_lop == Id_lop);
                if (check)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIds(IEnumerable<int> ids, int idDonvi)
        {
            var existingIds = _context.Dm_Monhoc.Where(c => ids.Contains(c.Id) && c.Id_don_vi==idDonvi).Select(c => c.Id).ToList();
            return ids.All(id => existingIds.Contains(id));
        }
        public bool CheckMa(string Ma, int idDonvi, int? Id)
        {
            try
            {
                var query = _context.Dm_Monhoc.Where(c => c.Ma == Ma && c.Id_don_vi == idDonvi);

                if (Id.HasValue)
                {
                    query = query.Where(c => c.Id != Id.Value);
                }

                var check = query.Any();
                return !check;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckTen(string Ten, int idDonvi, int? Id)
        {
            try
            {
                var query = _context.Dm_Monhoc.Where(c => c.Ten == Ten && c.Id_don_vi == idDonvi);

                if (Id.HasValue)
                {
                    query = query.Where(c => c.Id != Id.Value);
                }

                var check = query.Any();
                return !check;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIdMonPhongChuyen(int Id, int idDonvi)
        {
            if (Id <= 0) return false;
            try
            {
                var check = _context.Dm_Monhoc.Any(c => c.Id == Id && c.Id_don_vi == idDonvi && c.Id_loai_phong_hoc == 2);
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
        public Mon_banDto GetListTietBan(int Id, int idDonvi)
        {
            var dsCa = _context.Ca_Donvi.Where(cd => cd.Id_don_vi == idDonvi)
                            .Join(_context.DM_Cahoc,
                                  cd => cd.Id_ca_hoc,
                                  ca => ca.Id,
                                  (cd, ca) => new
                                  {
                                      Id = ca.Id,
                                      Ten = ca.Ten
                                  }).ToList();
            var tietBan = _context.Tiet_Tranh_Xep
                        .Where(tb => tb.Id_mon == Id)
                        .Select(tb => new { tb.Id_ca, tb.Thu, tb.Tiet })
                        .ToList();

            // Lấy danh sách ngày từ enum
            var dsNgay = Enum.GetValues<Ngay>().ToList();
            // Lấy danh sách tiết từ enum
            var dsTiet = Enum.GetValues<Tiet>().ToList();

            var result = new Mon_banDto
            {
                Id = Id,
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
                                                           td.Thu == (int)ngay &&
                                                           td.Tiet == (int)tiet)
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

        public List<int> GetlistPhongByDonvi(int id)
        {
            try
            {
                var list = _context.Monhoc_Phonghoc.Where(x => x.Id_mon == id).Select(x => x.Id_phong).ToList();
                if (list == null) return new List<int>();
                return list;
            }
            catch
            {
                return new List<int>();
            }
        }
        public bool AddPhong(int Id, List<int> phongId)
        {
            try
            {
                if (phongId != null && phongId.Count > 0)
                {
                    for (int i = 0; i < phongId.Count; i++)
                    {
                        var mon_phong = new Monhoc_Phonghoc
                        {
                            Id_mon = Id,
                            Id_phong = phongId[i]
                        };
                        _context.Monhoc_Phonghoc.Add(mon_phong);
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
        public bool UpdatePhong(int Id, List<int> phongId)
        {
            try
            {
                var del = _context.Monhoc_Phonghoc.Where(x => x.Id_mon == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Monhoc_Phonghoc.RemoveRange(del);
                }
                for (int i = 0; i < phongId.Count; i++)
                {
                    var phongMon = new Monhoc_Phonghoc
                    {
                        Id_mon = Id,
                        Id_phong = phongId[i]
                    };
                    _context.Monhoc_Phonghoc.Add(phongMon);
                }
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeletePhong(int Id)
        {
            try
            {
                var del = _context.Monhoc_Phonghoc.Where(x => x.Id_mon == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Monhoc_Phonghoc.RemoveRange(del);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddMonKhoi(List<Monhoc_Khoilop> dsMonKhoi, int idKhoi, int idBan)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var existingTiet = _context.Monhoc_Khoilop.Where(tb => tb.Id_khoi == idKhoi && tb.Id_ban==idBan).ToList();

                //xóa
                if (existingTiet.Any())
                {
                    _context.BulkDelete(existingTiet);
                }
                //thêm
                if (dsMonKhoi != null && dsMonKhoi.Any())
                {
                    _context.BulkInsertOrUpdate(dsMonKhoi);
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

        public List<Monhoc_KhoiLopDto> GetMonhocKhoilop(int? idKhoi, int? idBan,int idCa, int idDonvi)
        {
            try
            {
                var allMonhoc = _context.Dm_Monhoc.Where(m => m.Id_don_vi == idDonvi).ToList();
                var dsCa = _context.Ca_Donvi.Where(cd => cd.Id_don_vi == idDonvi)
                            .Join(_context.DM_Cahoc,
                                  cd => cd.Id_ca_hoc,
                                  ca => ca.Id,
                                  (cd, ca) => new
                                  {
                                      Id = ca.Id,
                                      Ten = ca.Ten
                                  }).ToList();
                var Monhoc_Khoi = _context.Monhoc_Khoilop
                    .Where(x => x.Id_khoi == idKhoi && x.Id_ban == idBan)
                    .ToList();

                var ds_Mon = allMonhoc.Select(mon =>
                {
                    var saved = Monhoc_Khoi.FirstOrDefault(x => x.Id_mon == mon.Id);
                    return new Mon_KhoiDto
                    {
                        Id_mon = mon.Id,
                        Ten_mon = mon.Ten,
                        ds_Ca = dsCa.Select(ca => new Ca_Khoi_MonDto
                        {
                            Id_ca = ca.Id,
                            Ten_ca = ca.Ten, 
                            So_tiet = saved?.So_tiet ?? 0,
                            So_nhom = saved?.So_nhom ?? 0
                        }).ToList(),
                        Trang_thai = saved != null
                    };
                }).ToList();

                var result = new Monhoc_KhoiLopDto
                {
                    Id_khoi = idKhoi ?? 0,
                    Id_ban = idBan ?? 0,
                    ds_Mon = ds_Mon
                };

                return new List<Monhoc_KhoiLopDto> { result };
            }
            catch (Exception ex)
            {
                // Log exception nếu cần
                return new List<Monhoc_KhoiLopDto>();
            }
        }
    }
}