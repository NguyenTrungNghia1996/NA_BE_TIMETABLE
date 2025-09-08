using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Logic.IRepository;

namespace NA_Logic.Repository
{
    public class DM_DonviRepository : IDM_DonviRepository
    {
        private readonly NA_DbContext _context;
         public DM_DonviRepository(NA_DbContext context)
        {
            _context = context;
        }
        public DM_Donvi getDetailById(int id)
        {
            try
            {
                var data = _context.DM_Donvi.Find(id);
                return data;
            }
            catch
            {
                return null;
            }
        }
        public List<int> GetlistCapbyDonvi(int id)
        {
            try
            {
                var list = _context.Cap_Donvi.Where(x => x.Id_Don_vi == id).Select(x => x.Id_Cap_hoc).ToList();
                if (list == null) return new List<int>();
                return list;
            }
            catch
            {
                return new List<int>();
            }
        }

        public List<int> GetlistCabyDonvi(int id)
        {
            try
            {
                var list = _context.Ca_Donvi.Where(x => x.Id_don_vi == id).Select(x => x.Id_ca_hoc).ToList();
                if (list == null) return new List<int>();
                return list;
            }
            catch
            {
                return new List<int>();
            }
        }
        public List<DM_Donvi_List> GetList_Paging(int PageIndex, int PageSize, string search, ref int totalrecord)
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
                var result = _context.Set<DM_Donvi_List>().FromSqlRaw("EXEC DM_Donvi_GetList_Paging @pageIndex, @pageSize, @search, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch,  paramTotal)
                    .ToList();
                if (result == null) result = new List<DM_Donvi_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(DM_Donvi dm_donvi)
        {
            try
            {
                _context.DM_Donvi.Add(dm_donvi);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckTrungTen(string ten, int? excludeId = null)
        {
            var ten_input = ten?.Trim().ToLower().Replace(" ", "") ?? "";

            var sql = excludeId == null
                ? $"SELECT TenDonvi FROM DM_Donvi "
                : $"SELECT TenDonvi FROM DM_Donvi  WHERE  Id != {excludeId}";

            var ds_ten = _context.Database.SqlQueryRaw<string>(sql).ToList();

            return ds_ten.Any(existingName =>
            {
                var ten_tontai = existingName?.Trim().ToLower().Replace(" ", "") ?? "";
                return ten_input == ten_tontai;
            });
        }
        //public bool Add_Demo(DM_Donvi_Demo dm_donvi)
        //{
        //    try
        //    {
        //        _context.DM_Donvi_Demo.Add(dm_donvi);
        //        _context.SaveChanges();
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        public bool AddCap(int Id, List<int> capId)
        {
            try
            {
                if (capId != null && capId.Count > 0)
                {
                    for (int i = 0; i < capId.Count; i++)
                    {
                        var capDonvi = new Cap_Donvi
                        {
                            Id_Don_vi = Id,
                            Id_Cap_hoc = capId[i]
                        };
                        _context.Cap_Donvi.Add(capDonvi);
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
        public bool AddCa(int Id, List<int> caId)
        {
            try
            {
                if (caId != null && caId.Count > 0)
                {
                    for (int i = 0; i < caId.Count; i++)
                    {
                        var caDonvi = new Ca_Donvi
                        {
                            Id_don_vi = Id,
                            Id_ca_hoc = caId[i]
                        };
                        _context.Ca_Donvi.Add(caDonvi);
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

        public bool Update(DM_Donvi dm_donvi)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _context.DM_Donvi.Update(dm_donvi);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateCap(int Id, List<int> capId)
        {
            try
            {
                var del = _context.Cap_Donvi.Where(x => x.Id_Don_vi == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Cap_Donvi.RemoveRange(del);
                }
                for (int i = 0; i < capId.Count; i++)
                {
                    var capDv = new Cap_Donvi
                    {
                        Id_Don_vi = Id,
                        Id_Cap_hoc = capId[i]
                    };
                    _context.Cap_Donvi.Add(capDv);
                }
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateCa(int Id, List<int> caId)
        {
            try
            {
                var del = _context.Ca_Donvi.Where(x => x.Id_don_vi == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Ca_Donvi.RemoveRange(del);
                }
                for (int i = 0; i < caId.Count; i++)
                {
                    var cadv = new Ca_Donvi
                    {
                        Id_don_vi = Id,
                        Id_ca_hoc = caId[i]
                    };
                    _context.Ca_Donvi.Add(cadv);
                }
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
                DM_Donvi donvi = new DM_Donvi();
                donvi = _context.DM_Donvi.Find(Id);
                if (donvi != null)
                {
                    return false;
                }

                bool check = _context.Database.SqlQueryRaw<int>($@"
                             select 1 from Ca_Donvi where Id_don_vi = {Id}
                             union select 1 from Cap_Donvi where Id_Don_vi = {Id}
                             union select 1 from Danhsach_Thoikhoabieu where Id_don_vi = {Id}
                             union select 1 from DM_Banhoc where Id_don_vi ={Id}
                             union select 1 from DM_Diemtruong where Id_don_vi = {Id}
                             union select 1 from DM_Giaovien where Id_don_vi = {Id}
                             union select 1 from Auth_Users where Id_Donvi = {Id}
                             union select 1 from DM_Khoikienthuc where Id_don_vi = {Id}
                             union select 1 from DM_Lophoc where Id_don_vi = {Id}
                             union select 1 from DM_Monhoc where Id_don_vi = {Id}
                             union select 1 from DM_Tochuyenmon where Id_don_vi = {Id}").First() == 1;
                if (check)
                    throw new Exception("Đơn vị đã có ràng buộc, không thể xoá");

                    _context.DM_Donvi.Remove(donvi);
                    _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteCap(int Id)
        {
            try
            {
                var del = _context.Cap_Donvi.Where(x => x.Id_Don_vi == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Cap_Donvi.RemoveRange(del);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteCa(int Id)
        {
            try
            {
                var del = _context.Ca_Donvi.Where(x => x.Id_don_vi == Id).ToList();
                if (del != null && del.Count > 0)
                {
                    _context.Ca_Donvi.RemoveRange(del);
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