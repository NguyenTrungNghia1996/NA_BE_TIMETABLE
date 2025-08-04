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
    public class ThoiKhoaBieuRepository : IDanhsach_ThoikhoabieuRepository
    {
        private readonly NA_DbContext _context;
        public ThoiKhoaBieuRepository(NA_DbContext context) { 
            _context = context;        
        }
        public List<Danhsach_ThoikhoabieuList> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi, ref int totalrecord)
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
                var result = _context.Set<Danhsach_ThoikhoabieuList>().FromSqlRaw("EXEC DS_TKB_GetList_Paging @pageIndex, @pageSize, @search, @idDonvi, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramIdDonvi, paramTotal)
                    .ToList();
                if (result == null) result = new List<Danhsach_ThoikhoabieuList>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Danhsach_Thoikhoabieu GetDetailById(int Id, int idDonvi)
        {
            try
            {
                var Giaovien = _context.Danhsach_Thoikhoabieu.FirstOrDefault(c => c.Id == Id && c.Id_don_vi == idDonvi);
                return Giaovien;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<Chitiet_Thoikhoabieu> GetDetailTKB(int Id)
        {
            try
            {
                var list = _context.Chitiet_Thoikhoabieu.Where(x => x.Id_tkb == Id).ToList();
                if (list == null) return null;
                return list;
            }
            catch
            {
                return new List<Chitiet_Thoikhoabieu>();
            }
        }
        public bool Add(Danhsach_Thoikhoabieu ds_thoikhoabieu)
        {
            try
            {
                _context.Danhsach_Thoikhoabieu.Add(ds_thoikhoabieu);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddChitiet_tkb(int Id)
        {
            try
            {
                var paramIdTkb = new SqlParameter("@IdTKB", SqlDbType.Int) { Value = Id };

                var paramResult = new SqlParameter("@Result", SqlDbType.Bit) { Direction = ParameterDirection.Output };

                _context.Database.ExecuteSqlRaw("EXEC InsertChitietTKB @IdTKB, @Result OUTPUT", paramIdTkb, paramResult);
                return Convert.ToBoolean(paramResult.Value);
            }
            catch(Exception) 
            {
                return false;
            }
        }
        public bool Update(Danhsach_Thoikhoabieu danhsach_Thoikhoabieu)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.Danhsach_Thoikhoabieu.Update(danhsach_Thoikhoabieu);
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
                Danhsach_Thoikhoabieu dstkb = new Danhsach_Thoikhoabieu();
                dstkb = _context.Danhsach_Thoikhoabieu.FirstOrDefault(c => c.Id == Id);
                if (dstkb != null)
                {
                    _context.Danhsach_Thoikhoabieu.Remove(dstkb);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteDetail(int Id)
        {
            try
            {
                var del = _context.Chitiet_Thoikhoabieu.Where(x => x.Id_tkb == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.BulkDelete(del);
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
                return _context.Danhsach_Thoikhoabieu.Any(c => c.Id == Id && c.Id_don_vi == idDonvi);
            }
            catch
            {
                return false;
            }
        }

        //object giáo viên
        public Object_Giaovien Object_giaovien(int idgv, int idtkb)
        {
            try
            {
                var paramIdGiaovien = new SqlParameter("Id_giao_vien", SqlDbType.Int)
                {
                    Value = idgv
                };
                var paramIdTkb = new SqlParameter("Id_tkb", SqlDbType.Int)
                {
                    Value = idtkb
                };

                
                var result = _context.Set<Chitiet_Thoikhoabieu_List>().FromSqlRaw("EXEC Get_Object_Giaovien @Id_giao_vien, @Id_tkb",
                      paramIdGiaovien, paramIdTkb)
                    .ToList();
                if (result == null) result = new List<Chitiet_Thoikhoabieu_List>();
                var teacher = new Object_Giaovien
                {
                    Id_don_vi = result[0].Id_don_vi,
                    Ten_don_vi = result[0].Ten_don_vi,
                    Id_giao_vien = result[0].Id_giao_vien,
                    Ten_giao_vien = result[0].Ten_giao_vien,
                    ds_tiet_phan_cong = new List<Ds_tiet_phan_cong>(),
                    ds_tiet_da_xep = new List<Ds_tiet_da_xep>(),
                    ds_tiet_chua_xep = new List<Ds_chua_xep>()
                };
                foreach (var r in result)
                {
                    teacher.ds_tiet_phan_cong.Add(new Ds_tiet_phan_cong
                    {
                        Id_mon = r.Id_mon,
                        Ten_mon = r.Ten_mon,
                        Id_lop = r.Id_lop,
                        Ten_lop = r.Ten_lop,
                        Id_phong = r.Id_phong,
                        Ten_phong = r.Ten_phong,
                        Id_ca = r.Id_ca,
                        Tiet = r.Tiet,
                        Ngay = r.Ngay
                    });

                    if (r.Tiet > 0 && r.Ngay > 0)
                    {
                        teacher.ds_tiet_da_xep.Add(new Ds_tiet_da_xep
                        {
                            Id_mon = r.Id_mon,
                            Ten_mon = r.Ten_mon,
                            Id_lop = r.Id_lop,
                            Ten_lop = r.Ten_lop,
                            Id_phong = r.Id_phong,
                            Ten_phong = r.Ten_phong,
                            Id_ca = r.Id_ca,
                            Tiet = r.Tiet,
                            Ngay = r.Ngay
                        });
                    }
                    else if (r.Tiet == 0 && r.Ngay == 0)
                    {
                        teacher.ds_tiet_chua_xep.Add(new Ds_chua_xep
                        {
                            Id_mon = r.Id_mon,
                            Ten_mon = r.Ten_mon,
                            Id_lop = r.Id_lop,
                            Ten_lop = r.Ten_lop,
                            Id_phong = r.Id_phong,
                            Ten_phong = r.Ten_phong,
                            Id_ca = r.Id_ca,
                            Tiet = r.Tiet,
                            Ngay = r.Ngay
                        });
                    }
                }

                return teacher;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
