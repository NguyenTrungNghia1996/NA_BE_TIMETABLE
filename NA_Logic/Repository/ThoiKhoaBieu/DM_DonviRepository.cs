using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository.XepThoiKhoaBieu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Logic.Repository.LichBaoGiang
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
        public List<DM_Donvi> GetList_SoGiaoDuc()
        {
            try
            {
                var list = _context.DM_Donvi.Where(c=>c.La_so_giao_duc == true).ToList();
                return list;
            }
            catch
            {
                return new List<DM_Donvi>();
            }
        }
        public List<DM_Donvi_List> GetList_Paging(int PageIndex, int PageSize, string search, bool isAdmin, int idDonvi, ref int totalrecord)
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
                var paramIsAdmin = new SqlParameter("isAdmin", SqlDbType.Bit)
                {
                    Value = isAdmin
                };
                var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int)
                {
                    Value = idDonvi
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _context.Set<DM_Donvi_List>().FromSqlRaw("EXEC DM_Donvi_GetList_Paging @pageIndex, @pageSize, @search, @isAdmin, @idDonvi, @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch, paramIsAdmin, paramIdDonvi,  paramTotal)
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
        public List<DM_Donvi> GetDonviChuaCoTaikhoan()
        {
            var query = from donvi in _context.DM_Donvi
                        join user in _context.Auth_Users on donvi.Id equals user.Id_Donvi into userGroup
                        from user in userGroup.DefaultIfEmpty()
                        where user == null
                        select donvi;

            return query.ToList();
        }
        public bool CheckDonviChuaCoTaikhoan(int id)
        {
            var query = from donvi in _context.DM_Donvi
                        join user in _context.Auth_Users on donvi.Id equals user.Id_Donvi into userGroup
                        from user in userGroup.DefaultIfEmpty()
                        where user == null
                        select donvi;
            var list = query.ToList();
            bool check = list.Any(c => c.Id == id);
            return check;
        }
        public bool Add(DM_Donvi dm_donvi)
        {
            try
            {
                dm_donvi.So_ngay = 7;
                _context.DM_Donvi.Add(dm_donvi);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckIdCha(int Id)
        {
            if (Id <= 0) return false;
            try
            {
                return _context.DM_Donvi.Any(c => c.La_so_giao_duc == true && c.Id == Id);
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
                return _context.DM_Donvi.Any(c => c.Id == Id);
            }
            catch
            {
                return false;
            }
        }
        public bool CheckIdCon(int Id, int idCha)
        {
            if (Id <= 0) return false;
            try
            {
                return _context.DM_Donvi.Any(c => c.Id == Id && c.Id_cha == idCha);
            }
            catch
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
        public bool AddCa(int Id, List<int> listca)
        {
            try
            {
                if (listca != null && listca.Count > 0)
                {
                    for (int i = 0; i < listca.Count; i++)
                    {
                        var caDonvi = new Ca_Donvi
                        {
                            Id_don_vi = Id,
                            Id_ca_hoc = listca[i],
                            So_tiet = 5
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
                dm_donvi.So_ngay = 7;
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
                        Id_ca_hoc = caId[i],
                        So_tiet = 5
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
                if (donvi == null)
                {
                    return false;
                }
                _context.DM_Donvi.Remove(donvi);
                    _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return  false;
            }
        }
        public (bool success, string message) DeleteCap(int Id)
        {
            try
            {
                var del = _context.Cap_Donvi.Where(x => x.Id_Don_vi == Id).ToList();
                bool check = _context.Database.SqlQueryRaw<int>($@"
                             select 1 as Value from Ca_Donvi where Id_don_vi = {Id}
                             union select 1 from Cap_Donvi where Id_Don_vi = {Id}
                             union select 1 from Danhsach_Thoikhoabieu where Id_don_vi = {Id}
                             union select 1 from DM_Banhoc where Id_don_vi ={Id}
                             union select 1 from DM_Diemtruong where Id_don_vi = {Id}
                             union select 1 from DM_Giaovien where Id_don_vi = {Id}
                             union select 1 from Auth_Users where Id_Donvi = {Id}
                             union select 1 from DM_Khoikienthuc where Id_don_vi = {Id}
                             union select 1 from DM_Lophoc where Id_don_vi = {Id}
                             union select 1 from DM_Monhoc where Id_don_vi = {Id}
                             union select 1 from DM_Tochuyenmon where Id_don_vi = {Id}").Any();

                if (check)
                    return (false, "Đơn vị đã có ràng buộc, không thể xoá");
                if (del != null && del.Count > 0)
                {
                    _context.Cap_Donvi.RemoveRange(del);
                    _context.SaveChanges();
                }
                return (true,"Xoá thành công");
            }
            catch(Exception ex) 
            {
                return (false, $"Lỗi hệ thống: {ex.Message}");
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