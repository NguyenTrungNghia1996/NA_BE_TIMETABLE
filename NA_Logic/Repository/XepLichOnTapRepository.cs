using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository.LichOnTap;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class XepLichOnTapRepository : IXepLichOnTapRepository
    {
        private readonly NA_DbContext _context;
        private Object_Monhoc _ObjectMon = new Object_Monhoc();
        private Object_Lophoc _ObjectLop = new Object_Lophoc();
        private Object_GiaovienOnTap _ObjectGiaovien = new Object_GiaovienOnTap();
        private Object_Phonghoc _ObjectPhong = new Object_Phonghoc();
        private List<Object_TietOnTap> _dsTietGoc = new List<Object_TietOnTap>();
        private List<Object_Tiet> _dsTatCaTietlich = new List<Object_Tiet>();
        private List<Object_Tiet> _dsTietlich = new List<Object_Tiet>();

        private List<Object_Monhoc> _dsObjectMon = new List<Object_Monhoc>();
        private List<Object_Lophoc> _dsObjectLop = new List<Object_Lophoc>();
        private List<Object_GiaovienOnTap> _dsObjectGiaovien = new List<Object_GiaovienOnTap>();
        private List<Object_Phonghoc> _dsObjectPhong = new List<Object_Phonghoc>();
        private List<Object_ca> _dsCa;
        private int _soNgay;
        public XepLichOnTapRepository(NA_DbContext context)
        {
            _context = context;
        }
        //object tiết
        public List<Object_TietOnTap> List_Object_tiet(int idlich)
        {
            try
            {
                var paramIdlich = new SqlParameter("Id_lich", SqlDbType.Int)
                {
                    Value = idlich
                };
                var result = _context.Set<Chitiet_Lichontap_List>().FromSqlRaw("EXEC Get_Chitiet_Lichontap  @Id_lich = @Id_lich", paramIdlich)
                    .ToList();

                if (result == null) return null;
                var ds_tiet = new List<Object_TietOnTap>();
                foreach (var item in result)
                {
                    ds_tiet.Add(new Object_TietOnTap
                    {
                        Id = item.Id,
                        Id_don_vi = item.Id_don_vi,
                        Id_lich = item.Id_lich,
                        Id_lop = item.Id_lop,
                        Ten_lop = item.Ten_lop,
                        Id_mon = item.Id_mon,
                        Ten_mon = item.Ten_mon,
                        Id_giao_vien = item.Id_giao_vien,
                        Ten_giao_vien = item.Ten_giao_vien,
                        Id_phong = item.Id_phong,
                        Ten_phong = item.Ten_phong,
                        Tiet_thu_may = item.Tiet_thu_may,
                        Id_ca = item.Id_ca,
                        Ngay = item.Ngay,
                        Tiet = item.Tiet,
                        Ds_vi_tri_xep_duoc = new List<Ds_vi_tri_xep_duoc>()
                    });
                }
                return ds_tiet;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void LoadAllInformation(int idLich, int idDonvi)
        {
            try
            {
                var ob_tiet = new List<Object_TietOnTap>();
                var ob_tiet_lich = new List<Object_Tiet>();
                var gv_tietnghi = new List<(int Id_giao_vien, int Id_ca, int Ngay, int Tiet)>();
                var mh_tietnghi = new List<(int Id_mon, int Id_ca, int Ngay, int Tiet)>();
                var ph_tietnghi = new List<(int Id_phong, int Id_ca, int Ngay, int Tiet)>();
                var lh_tietnghi = new List<(int Id_lop, int Id_ca, int Ngay, int Tiet)>();
                var ob_ca = new List<Object_ca>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "GetAll_Information_ToXepLich";
                    cmd.CommandType = CommandType.StoredProcedure;

                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@idDonvi";
                    p1.Value = idDonvi;
                    cmd.Parameters.Add(p1);

                    var p2 = cmd.CreateParameter();
                    p2.ParameterName = "@idLich";
                    p2.Value = idLich;
                    cmd.Parameters.Add(p2);

                    _context.Database.OpenConnection();

                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            ob_tiet.Add(new Object_TietOnTap
                            {
                                Id = (int)reader["Id"],
                                Id_don_vi = (int)reader["Id_don_vi"],
                                Id_lich = (int)reader["Id_lich"],
                                Id_ca = (int)reader["Id_ca"],
                                Id_giao_vien = (int)reader["Id_giao_vien"],
                                Ten_giao_vien = reader["Ten_giao_vien"] != DBNull.Value? (string)reader["Ten_giao_vien"] : "",
                                Id_lop = (int)reader["Id_lop"],
                                Ten_lop = reader["Ten_lop"] != DBNull.Value ? (string)reader["Ten_lop"] : "",
                                Id_mon = (int)reader["Id_mon"],
                                Ten_mon = reader["Ten_mon"] != DBNull.Value ? (string)reader["Ten_mon"] : "",
                                Id_phong = (int)reader["Id_phong"],
                                Ten_phong = reader["Ten_phong"] != DBNull.Value ? (string)reader["Ten_phong"] : "",
                                Tiet_thu_may = (int)reader["Tiet_thu_may"],
                                Ngay = (int)reader["Ngay"],
                                Tiet = (int)reader["Tiet"],
                            });
                        }

                        reader.NextResult();
                        while (reader.Read())
                        {
                            gv_tietnghi.Add((
                                Id_giao_vien: (int)reader["Id_giao_vien"],
                                Id_ca: (int)reader["Id_ca"],
                                Ngay: (int)reader["Ngay"],
                                Tiet: (int)reader["Tiet"]
                            ));
                        }
                        
                        reader.NextResult();
                        while (reader.Read())
                        {
                            mh_tietnghi.Add((
                                Id_mon: (int)reader["Id_mon"],
                                Id_ca: (int)reader["Id_ca"],
                                Ngay: (int)reader["Ngay"],
                                Tiet: (int)reader["Tiet"]
                            ));
                        }
                        
                        reader.NextResult();
                        while (reader.Read())
                        {
                            ph_tietnghi.Add((
                                Id_phong: (int)reader["Id_phong"],
                                Id_ca: (int)reader["Id_ca"],
                                Ngay: (int)reader["Ngay"],
                                Tiet: (int)reader["Tiet"]
                            ));
                        }
                       
                        reader.NextResult();
                        while (reader.Read())
                        {
                            lh_tietnghi.Add((
                                Id_lop: (int)reader["Id_lop_on"],
                                Id_ca: (int)reader["Id_ca"],
                                Ngay: (int)reader["Ngay"],
                                Tiet: (int)reader["Tiet"]
                            ));
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            ob_ca.Add(new Object_ca
                            {
                                Id_ca = (int)reader["Id_ca_hoc"],
                                So_tiet = (int)reader["So_tiet"]

                            });
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            _soNgay = (int)reader["So_ngay"];
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            ob_tiet_lich.Add(new Object_Tiet
                            {
                                Id = (int)reader["Id"],
                                Id_tkb = (int)reader["Id_tkb"],
                                Id_ca = (int)reader["Id_ca"],
                                Id_giao_vien = (int)reader["Id_giao_vien"],
                                Id_lop = (int)reader["Id_lop"],
                                Id_mon = (int)reader["Id_mon"],
                                Id_phong = (int)reader["Id_phong"],
                                Tiet_thu_may = (int)reader["Tiet_thu_may"],
                                Ngay = (int)reader["Ngay"],
                                Tiet = (int)reader["Tiet"],
                            });
                        }
                    }
                }

                var ob_giaovien = gv_tietnghi.GroupBy(t => t.Id_giao_vien)
                    .Select(g => new Object_GiaovienOnTap
                    {
                        Id_giao_vien = g.Key,
                        ds_tiet_tranh_xep = g.Select(t => new Ds_tiet_tranh_xep
                        {
                            Id_ca = t.Id_ca,
                            Ngay = t.Ngay,
                            Tiet = t.Tiet
                        }).ToList()
                    }).ToList();
                var ob_mon = mh_tietnghi.GroupBy(t => t.Id_mon)
                    .Select(g => new Object_Monhoc
                    {
                        Id_mon = g.Key,
                        ds_tiet_tranh_xep = g.Select(t => new Ds_tiet_tranh_xep
                        {
                            Id_ca = t.Id_ca,
                            Ngay = t.Ngay,
                            Tiet = t.Tiet
                        }).ToList()
                    }).ToList();
                var ob_phong = ph_tietnghi.GroupBy(t => t.Id_phong)
                    .Select(g => new Object_Phonghoc
                    {
                        Id_phong = g.Key,
                        ds_tiet_tranh_xep = g.Select(t => new Ds_tiet_tranh_xep
                        {
                            Id_ca = t.Id_ca,
                            Ngay = t.Ngay,
                            Tiet = t.Tiet
                        }).ToList(),
                    }).ToList();
                var ob_lop = lh_tietnghi.GroupBy (t => t.Id_lop)
                    .Select(g => new Object_Lophoc
                    {
                        Id_lop = g.Key,
                        ds_tiet_tranh_xep = g.Select(t => new Ds_tiet_tranh_xep
                        {
                            Id_ca = t.Id_ca,
                            Ngay = t.Ngay,
                            Tiet = t.Tiet
                        }).ToList(),
                    }).ToList();

                _dsTietGoc = ob_tiet;
                _dsObjectGiaovien = ob_giaovien;
                _dsObjectMon = ob_mon;
                _dsObjectPhong = ob_phong;
                _dsObjectLop = ob_lop;
                _dsTatCaTietlich = ob_tiet_lich;
                _dsCa = ob_ca;

            }
            catch (Exception)
            {
                return;
            }
        }
        public void LoadObjectsFromTiet(Object_TietOnTap ObjectTietOnTap, int idDonvi, List<Object_TietOnTap> ds_da_xep, List<Object_TietOnTap> ds_chua_xep)
        {
            try
            {
                if (ObjectTietOnTap == null)
                {
                    return;
                }
                _dsTietlich = _dsTatCaTietlich.Where(c => c.Id_giao_vien == ObjectTietOnTap.Id_giao_vien && c.Id_phong == ObjectTietOnTap.Id_phong).ToList();
                _ObjectMon = _dsObjectMon.FirstOrDefault(c => c.Id_mon == ObjectTietOnTap.Id_mon);
                _ObjectLop = _dsObjectLop.FirstOrDefault(c => c.Id_lop == ObjectTietOnTap.Id_lop);
                _ObjectGiaovien = _dsObjectGiaovien.FirstOrDefault(c => c.Id_giao_vien == ObjectTietOnTap.Id_giao_vien);
                _ObjectGiaovien.ds_tiet_da_xep = ds_da_xep;
                _ObjectGiaovien.ds_tiet_chua_xep = ds_chua_xep;
                if (ObjectTietOnTap.Id_phong == 0)
                {
                    _ObjectPhong = null;
                }
                else
                {
                    _ObjectPhong = _dsObjectPhong.FirstOrDefault(c => c.Id_phong == ObjectTietOnTap.Id_phong);
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
        public void LoadObjectsFromTiet_TietBan(Object_TietOnTap ObjectTietOnTap, int? idDonvi)
        {
            try
            {
                if (ObjectTietOnTap == null)
                {
                    return;
                }

                _ObjectMon = _dsObjectMon.FirstOrDefault(c => c.Id_mon == ObjectTietOnTap.Id_mon);
                _ObjectLop = _dsObjectLop.FirstOrDefault(c => c.Id_lop == ObjectTietOnTap.Id_lop);
                _ObjectGiaovien = _dsObjectGiaovien.FirstOrDefault(c => c.Id_giao_vien == ObjectTietOnTap.Id_giao_vien);
                if (ObjectTietOnTap.Id_phong == 0)
                {
                    _ObjectPhong = null;
                }
                else
                {
                    _ObjectPhong = _dsObjectPhong.FirstOrDefault(c => c.Id_phong == ObjectTietOnTap.Id_phong);
                }
                
            }
            catch (Exception)
            {
                return;
            }
        }
        public void TimViTriXepDuoc(Object_TietOnTap ObjectTietOnTap, int idDonvi, List<Object_TietOnTap> ds_da_xep, List<Object_TietOnTap> ds_chua_xep)
        {
            try
            {
                ObjectTietOnTap.Ds_vi_tri_xep_duoc.Clear();
                LoadObjectsFromTiet(ObjectTietOnTap, idDonvi, ds_da_xep, ds_chua_xep);

                if (_ObjectMon == null || _ObjectGiaovien == null)
                    return;

                var tietban = DsTietTranhXep(ObjectTietOnTap);
                var dsCa = _dsCa;
                for (int i = 0; i < dsCa.Count; i++)
                {

                    for (int ngay = 1; ngay <= _soNgay; ngay++)
                    {
                        for (int tiet = 1; tiet <= dsCa[i].So_tiet; tiet++)
                        {
                            var slotKey = $"{ngay}_{dsCa[i].Id_ca}_{tiet}";

                            if (tietban.Contains(slotKey))
                                continue;
                            if (CheckDieuKienConLai(ngay, tiet, dsCa[i].Id_ca, ObjectTietOnTap))
                            {
                                ObjectTietOnTap.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
                                {
                                    Ca = dsCa[i].Id_ca,
                                    Ngay = ngay,
                                    Tiet = tiet,
                                });
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {
                ObjectTietOnTap.Ds_vi_tri_xep_duoc.Clear();
            }
        }
        public bool CheckDieuKienConLai(int Ngay, int Tiet, int Ca, Object_TietOnTap ob)
        {
            var object_gv = _ObjectGiaovien;
            if (object_gv == null)
            {
                return false;
            }
            bool check_trung_gv = object_gv.ds_tiet_da_xep.Any(t => t.Id_phong == ob.Id_phong && t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);
            bool check_trung_phong = object_gv.ds_tiet_da_xep.Any(t => t.Id_giao_vien == ob.Id_giao_vien && t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);
            bool check_trung_lop = object_gv.ds_tiet_da_xep.Any(t => t.Id_lop == ob.Id_lop && t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);

            if (check_trung_gv || check_trung_phong || check_trung_lop ) { return false; }

            return true;
        }
        private HashSet<string> DsTietTranhXep(Object_TietOnTap ObjectTietOnTap)
        {
            var tietTranhXep = new HashSet<string>();

            AddTietTranhXep(tietTranhXep, _ObjectGiaovien?.ds_tiet_tranh_xep);
            AddTietTranhXep(tietTranhXep, _ObjectMon?.ds_tiet_tranh_xep);
            AddTietTranhXep(tietTranhXep, _ObjectLop?.ds_tiet_tranh_xep);
            
            if (_ObjectPhong != null)
            {
                if (_ObjectPhong.ds_tiet_tranh_xep != null && _ObjectPhong.Khong_kiem_tra_xung_dot == false)
                {
                    AddTietTranhXep(tietTranhXep, _ObjectPhong.ds_tiet_tranh_xep);
                }
            }
            if(_dsTietlich != null && _dsTietlich.Count > 0)
                AddTietTranhXep(tietTranhXep, _dsTietlich);

            return tietTranhXep;
        }

        private void AddTietTranhXep<T>(HashSet<string> tietTranhXep, IEnumerable<T> dsTiet) where T : class
        {
            if (dsTiet == null) return;

            foreach (dynamic tiet in dsTiet)
            {
                tietTranhXep.Add($"{tiet.Ngay}_{tiet.Id_ca}_{tiet.Tiet}");
            }
        }
        private bool UpdateListTiet(List<Object_TietOnTap> ds_tiet)
        {
            try
            {
                if (ds_tiet != null && ds_tiet.Any())
                {
                    var chitietlist = ds_tiet.Select(tiet => new Chitiet_Lichontap
                    {
                        Id = tiet.Id,
                        Id_lich = tiet.Id_lich,
                        Id_lop = tiet.Id_lop ?? 0,
                        Id_ca = tiet.Id_ca,
                        Tiet_thu_may = tiet.Tiet_thu_may,
                        Ngay = tiet.Ngay,
                        Tiet = tiet.Tiet
                    }).ToList();
                    var chitiet = chitietlist.Where(c => c.Id_ca > 0).ToList();
                    if (chitiet != null)
                        _context.BulkUpdate(chitiet);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private bool UpdateTiet(Object_TietOnTap tiet, int Id_ca, int ngay, int tietSo)
        {
            try
            {
                var record = _context.Chitiet_Lichontap
                    .FirstOrDefault(x => x.Id_lich == tiet.Id_lich &&
                                         x.Id_lop == tiet.Id_lop &&
                                         x.Tiet_thu_may == tiet.Tiet_thu_may);

                if (record != null)
                {
                    record.Ngay = ngay;
                    record.Tiet = tietSo;
                    record.Id_ca = Id_ca;
                    _context.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool ProcessThoiKhoaBieu(int idlich, int idDonvi)
        {
            try
            {
                LoadAllInformation(idlich, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return false;
                }
                var dsTietChuaXep = _dsTietGoc.Where(c => c.Ngay <= 0).ToList();
                var dsTietDaXep = _dsTietGoc.Where(c => c.Id_ca > 0 && c.Ngay > 0 && c.Tiet > 0).ToList();
                var dsTietBoqua = new List<Object_TietOnTap>();

                int vongLap = 0;
                while (dsTietChuaXep.Count > 0)
                {
                    vongLap++;
                    for (int i = 0; i < dsTietChuaXep.Count; i++)
                    {
                        TimViTriXepDuoc(dsTietChuaXep[i], idDonvi, dsTietDaXep, dsTietChuaXep);
                    }
                    var dsTietCoTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count > 0).ToList();
                    var dsTietKhongTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count == 0).ToList();
                    if (dsTietKhongTheXep != null && dsTietKhongTheXep.Count > 0)
                    {
                        for (int i = 0; i < dsTietKhongTheXep.Count; i++)
                        {
                            dsTietBoqua.Add(dsTietKhongTheXep[i]);
                            dsTietChuaXep.Remove(dsTietKhongTheXep[i]);
                        }
                    }
                    if (dsTietCoTheXep.Count == 0)
                    {
                        break;
                    }
                    var dsTietSorted = dsTietCoTheXep.OrderBy(t => t.Ds_vi_tri_xep_duoc.Count).ToList();
                    var tietCanXep = dsTietSorted.First();
                    
                    var viTriChon = tietCanXep.Ds_vi_tri_xep_duoc.First();
                    tietCanXep.Id_ca = viTriChon.Ca;
                    tietCanXep.Ngay = viTriChon.Ngay;
                    tietCanXep.Tiet = viTriChon.Tiet;
                    dsTietChuaXep.Remove(tietCanXep);
                    dsTietDaXep.Add(tietCanXep);
                    if (vongLap > 1000)
                    {
                        break;
                    }
                }
                bool update = UpdateListTiet(dsTietDaXep);
                if (update)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool Xeplich_byPhong(List<int> idphong, int idlich, int idDonvi)
        {
            try
            {
                LoadAllInformation(idlich, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return false;
                }
                var dsTietChuaXep = new List<Object_TietOnTap>();
                var dsTietDaXep = new List<Object_TietOnTap>();
                var dsTietBoqua = new List<Object_TietOnTap>();
                for (int i = 0; i < idphong.Count; i++)
                {
                    int id = idphong[i];
                    dsTietChuaXep.AddRange(_dsTietGoc.Where(c => c.Id_phong == id && c.Ngay == 0 && c.Tiet == 0));
                    dsTietDaXep.AddRange(_dsTietGoc.Where(c => c.Id_phong == id && c.Ngay > 0 && c.Tiet > 0));
                }
                int vongLap = 0;
                while (dsTietChuaXep.Count > 0)
                {
                    vongLap++;
                    for (int i = 0; i < dsTietChuaXep.Count; i++)
                    {
                        TimViTriXepDuoc(dsTietChuaXep[i], idDonvi, dsTietDaXep, dsTietChuaXep);
                    }
                    var dsTietCoTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count > 0).ToList();
                    var dsTietKhongTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count == 0).ToList();
                    if (dsTietKhongTheXep != null && dsTietKhongTheXep.Count > 0)
                    {
                        for (int i = 0; i < dsTietKhongTheXep.Count; i++)
                        {
                            dsTietBoqua.Add(dsTietKhongTheXep[i]);
                            dsTietChuaXep.Remove(dsTietKhongTheXep[i]);
                        }
                    }
                    if (dsTietCoTheXep.Count == 0)
                    {
                        break;
                    }
                    var dsTietSorted = dsTietCoTheXep.OrderBy(t => t.Ds_vi_tri_xep_duoc.Count).ToList();
                    var tietCanXep = dsTietSorted.First();

                    var viTriChon = tietCanXep.Ds_vi_tri_xep_duoc.First();
                    tietCanXep.Id_ca = viTriChon.Ca;
                    tietCanXep.Ngay = viTriChon.Ngay;
                    tietCanXep.Tiet = viTriChon.Tiet;
                    dsTietChuaXep.Remove(tietCanXep);
                    dsTietDaXep.Add(tietCanXep);
                    if (vongLap > 1000)
                    {
                        break;
                    }
                }
                bool update = UpdateListTiet(dsTietDaXep);
                if (update)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessThoiKhoaBieu: {ex.Message}");
                return false;
            }
        }
        public bool Xeplich_byGV(List<int> idgv, int idlich, int idDonvi)
        {
            try
            {
                LoadAllInformation(idlich, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return false;
                }
                var dsTietChuaXep = new List<Object_TietOnTap>();
                var dsTietDaXep = new List<Object_TietOnTap>();
                var dsTietBoqua = new List<Object_TietOnTap>();
                for (int i = 0; i < idgv.Count; i++)
                {
                    int id = idgv[i];
                    dsTietChuaXep.AddRange(_dsTietGoc.Where(c => c.Id_giao_vien == id && c.Ngay == 0 && c.Tiet == 0));
                    dsTietDaXep.AddRange(_dsTietGoc.Where(c => c.Id_giao_vien == id && c.Ngay > 0 && c.Tiet > 0));
                }
                int vongLap = 0;
                while (dsTietChuaXep.Count > 0)
                {
                    vongLap++;
                    for (int i = 0; i < dsTietChuaXep.Count; i++)
                    {
                        TimViTriXepDuoc(dsTietChuaXep[i], idDonvi, dsTietDaXep, dsTietChuaXep);
                    }
                    var dsTietCoTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count > 0).ToList();
                    var dsTietKhongTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count == 0).ToList();
                    if (dsTietKhongTheXep != null && dsTietKhongTheXep.Count > 0)
                    {
                        for (int i = 0; i < dsTietKhongTheXep.Count; i++)
                        {
                            dsTietBoqua.Add(dsTietKhongTheXep[i]);
                            dsTietChuaXep.Remove(dsTietKhongTheXep[i]);
                        }
                    }
                    if (dsTietCoTheXep.Count == 0)
                    {
                        break;
                    }
                    var dsTietSorted = dsTietCoTheXep.OrderBy(t => t.Ds_vi_tri_xep_duoc.Count).ToList();
                    var tietCanXep = dsTietSorted.First();

                    var viTriChon = tietCanXep.Ds_vi_tri_xep_duoc.First();
                    tietCanXep.Id_ca = viTriChon.Ca;
                    tietCanXep.Ngay = viTriChon.Ngay;
                    tietCanXep.Tiet = viTriChon.Tiet;
                    dsTietChuaXep.Remove(tietCanXep);
                    dsTietDaXep.Add(tietCanXep);
                    if (vongLap > 1000)
                    {
                        break;
                    }
                }
                bool update = UpdateListTiet(dsTietDaXep);
                if (update)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessThoiKhoaBieu: {ex.Message}");
                return false;
            }
        }
        public bool Xeplich_byLop(List<int> idlop, int idlich, int idDonvi)
        {
            try
            {
                LoadAllInformation(idlich, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return false;
                }
                var dsTietChuaXep = new List<Object_TietOnTap>();
                var dsTietDaXep = new List<Object_TietOnTap>();
                var dsTietBoqua = new List<Object_TietOnTap>();
                for (int i = 0; i < idlop.Count; i++)
                {
                    int id = idlop[i];
                    dsTietChuaXep.AddRange(_dsTietGoc.Where(c => c.Id_lop == id && c.Ngay == 0 && c.Tiet == 0));
                    dsTietDaXep.AddRange(_dsTietGoc.Where(c => c.Id_lop == id && c.Ngay > 0 && c.Tiet > 0));
                }
                int vongLap = 0;
                while (dsTietChuaXep.Count > 0)
                {
                    vongLap++;
                    for (int i = 0; i < dsTietChuaXep.Count; i++)
                    {
                        TimViTriXepDuoc(dsTietChuaXep[i], idDonvi, dsTietDaXep, dsTietChuaXep);
                    }
                    var dsTietCoTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count > 0).ToList();
                    var dsTietKhongTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count == 0).ToList();
                    if (dsTietKhongTheXep != null && dsTietKhongTheXep.Count > 0)
                    {
                        for (int i = 0; i < dsTietKhongTheXep.Count; i++)
                        {
                            dsTietBoqua.Add(dsTietKhongTheXep[i]);
                            dsTietChuaXep.Remove(dsTietKhongTheXep[i]);
                        }
                    }
                    if (dsTietCoTheXep.Count == 0)
                    {
                        break;
                    }
                    var dsTietSorted = dsTietCoTheXep.OrderBy(t => t.Ds_vi_tri_xep_duoc.Count).ToList();
                    var tietCanXep = dsTietSorted.First();

                    var viTriChon = tietCanXep.Ds_vi_tri_xep_duoc.First();
                    tietCanXep.Id_ca = viTriChon.Ca;
                    tietCanXep.Ngay = viTriChon.Ngay;
                    tietCanXep.Tiet = viTriChon.Tiet;
                    dsTietChuaXep.Remove(tietCanXep);
                    dsTietDaXep.Add(tietCanXep);
                    if (vongLap > 1000)
                    {
                        break;
                    }
                }
                bool update = UpdateListTiet(dsTietDaXep);
                if (update)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessThoiKhoaBieu: {ex.Message}");
                return false;
            }
        }
        public ObjectTietOnTap_theoLopDto GetLichByLop(int id_lop, int idlich, int idDonvi)
        {
            LoadAllInformation(idlich, idDonvi);
            if (_dsTietGoc?.Count == 0 || id_lop < 0) return null;

            var tiet = _dsTietGoc.Where(t => t.Id_lop == id_lop).ToList();


            if (!tiet.Any())
            {
                return null;
            }

            var firstTiet = tiet.First();
            _ObjectLop = _dsObjectLop.FirstOrDefault(c => c.Id_lop == id_lop);
            var tietTranhXep = _ObjectLop.ds_tiet_tranh_xep;

            var dsNgay = Enum.GetValues<Ngay>().Take(_soNgay).ToList();
            var dsTietEnum = Enum.GetValues<Tiet>().ToList();
            var tietTrungLap = tiet.Where(t => t.Ngay > 0 && t.Tiet > 0)
                              .GroupBy(t => new { t.Id_ca, t.Ngay, t.Tiet }).Where(g => g.Count() > 1)
                              .Select(g => $"{g.Key.Ngay}_{g.Key.Id_ca}_{g.Key.Tiet}").ToHashSet();
            var result = new ObjectTietOnTap_theoLopDto
            {
                Id_lop = id_lop,
                Ten_lop = firstTiet.Ten_lop,
                timetable = new List<lich_theo_lop>(),
                ds_chua_xep = new List<lich_chuaxep_lop>()
            };

            foreach (var ca in _dsCa)
            {
                foreach (var ngay in dsNgay)
                {
                    foreach (var tietEnum in dsTietEnum.Take(ca.So_tiet))
                    {
                        var tietHoc = tiet.FirstOrDefault(t =>
                            t.Id_ca == ca.Id_ca &&
                            t.Ngay == (int)ngay &&
                            t.Tiet == (int)tietEnum);

                        bool isBreak = tietTranhXep.Any(tx =>
                            tx.Id_ca == ca.Id_ca &&
                            tx.Ngay == (int)ngay &&
                            tx.Tiet == (int)tietEnum);

                        var tietItem = new lich_theo_lop
                        {
                            Id_chitiet = tietHoc?.Id ?? 0,
                            Id_don_vi = idDonvi,
                            Id_lich = idlich,
                            Id_ca = ca.Id_ca,
                            Ngay = (int)ngay,
                            Tiet = (int)tietEnum,
                            Tiet_thu_may = tietHoc?.Tiet_thu_may ?? 0
                        };

                        if (tietHoc != null)
                        {
                            tietItem.Id_mon = tietHoc.Id_mon ?? 0;
                            tietItem.Ten_mon = tietHoc.Ten_mon ?? "";
                            tietItem.Id_giao_vien = tietHoc.Id_giao_vien ?? 0;
                            tietItem.Ten_giao_vien = tietHoc.Ten_giao_vien ?? "";
                            tietItem.Id_phong = tietHoc.Id_phong ?? 0;
                            tietItem.Ten_phong = tietHoc.Ten_phong ?? "Không cần phòng";
                            tietItem.isDrag = false;
                            tietItem.isRest = isBreak;

                            string currentKey = $"{(int)ngay}_{ca.Id_ca}_{(int)tietEnum}";
                            // check trùng tiết nghỉ
                            LoadObjectsFromTiet_TietBan(tietHoc, idDonvi);
                            var tietban = DsTietTranhXep(tietHoc);
                            bool isErrorTietBan = tietban.Contains(currentKey);

                            // check trùng tiết đã xếp
                            bool isErrorTrungLap = tietTrungLap.Contains(currentKey);
                            tietItem.isError = isErrorTietBan || isErrorTrungLap;

                            result.timetable.Add(tietItem);
                        }
                        else
                        {
                            tietItem.Id_mon = 0;
                            tietItem.Ten_mon = "";
                            tietItem.Id_giao_vien = 0;
                            tietItem.Ten_giao_vien = "";
                            tietItem.Id_phong = 0;
                            tietItem.Ten_phong = "";
                            tietItem.isDrag = false;
                            tietItem.isRest = isBreak;
                            result.timetable.Add(tietItem);
                        }
                    }
                }
            }

            var tietChuaXep = tiet.Where(t => t.Id_lop == id_lop && t.Id_lich == idlich && t.Ngay <= 0 && t.Tiet <= 0).GroupBy(t => new { t.Id_mon, t.Id_phong, t.Id_giao_vien })
                                  .Select(g => new {
                                      FirstItem = g.First(),
                                      SoTiet = g.Count()
                                  }).ToList();
            foreach (var tietcx in tietChuaXep)
            {
                var t = tietcx.FirstItem;
                var tietItem = new lich_chuaxep_lop
                {
                    Id_chitiet = t.Id,
                    Id_don_vi = t.Id_don_vi ?? 0,
                    Id_lich = t.Id_lich,
                    Id_mon = t.Id_mon ?? 0,
                    Ten_mon = t.Ten_mon ?? "",
                    Id_giao_vien = t.Id_giao_vien ?? 0,
                    Ten_giao_vien = t.Ten_giao_vien ?? "",
                    Id_phong = t.Id_phong ?? 0,
                    Ten_phong = t.Ten_phong ?? "Không cần phòng",
                    So_tiet = tietcx.SoTiet
                };

                result.ds_chua_xep.Add(tietItem);
            }

            return result;
        }
        public ObjectTietOnTap_theoGVDto GetLichByGiaovien(int id_gv, int idlich, int idDonvi)
        {
            LoadAllInformation(idlich, idDonvi);

            if (_dsTietGoc == null || _dsTietGoc.Count == 0)
            {
                return null;
            }

            var tiet = _dsTietGoc.Where(t => t.Id_giao_vien == id_gv).ToList();

            if (!tiet.Any())
            {
                return null;
            }

            var firstTiet = tiet.First();

            _ObjectGiaovien = _dsObjectGiaovien.FirstOrDefault(c => c.Id_giao_vien == id_gv);
            var tietTranhXep = _ObjectGiaovien.ds_tiet_tranh_xep;

            var dsNgay = Enum.GetValues<Ngay>().Take(_soNgay).ToList();
            var dsTietEnum = Enum.GetValues<Tiet>().ToList();
            var tietTrungLap = tiet.Where(t => t.Ngay > 0 && t.Tiet > 0)
                              .GroupBy(t => new { t.Id_ca, t.Ngay, t.Tiet }).Where(g => g.Count() > 1)
                              .Select(g => $"{g.Key.Ngay}_{g.Key.Id_ca}_{g.Key.Tiet}").ToHashSet();
            var result = new ObjectTietOnTap_theoGVDto
            {
                Id_giao_vien = id_gv,
                Ten_giao_vien = firstTiet.Ten_giao_vien,
                Tong_so_tiet = tiet.Count(),
                So_tiet_da_xep = tiet.Where(c => c.Ngay > 0 && c.Tiet > 0).Count(),
                timetable = new List<lich_theo_giaovien>(),
                ds_chua_xep = new List<lich_chuaxep_giaovien>()
            };

            foreach (var ca in _dsCa)
            {
                foreach (var ngay in dsNgay)
                {
                    foreach (var tietEnum in dsTietEnum.Take(ca.So_tiet))
                    {
                        var tietHoc = tiet.FirstOrDefault(t =>
                            t.Id_ca == ca.Id_ca &&
                            t.Ngay == (int)ngay &&
                            t.Tiet == (int)tietEnum);

                        bool isBreak = tietTranhXep.Any(tx =>
                            tx.Id_ca == ca.Id_ca &&
                            tx.Ngay == (int)ngay &&
                            tx.Tiet == (int)tietEnum);

                        var tietItem = new lich_theo_giaovien
                        {
                            Id_chitiet = tietHoc?.Id ?? 0,
                            Id_don_vi = idDonvi,
                            Id_lich = idlich,
                            Id_ca = ca.Id_ca,
                            Ngay = (int)ngay,
                            Tiet = (int)tietEnum,
                            Tiet_thu_may = tietHoc?.Tiet_thu_may ?? 0
                        };

                        if (tietHoc != null)
                        {
                            tietItem.Id_mon = tietHoc.Id_mon ?? 0;
                            tietItem.Ten_mon = tietHoc.Ten_mon ?? "";
                            tietItem.Id_lop = tietHoc.Id_lop ?? 0;
                            tietItem.Ten_lop = tietHoc.Ten_lop ?? "";
                            tietItem.Id_phong = tietHoc.Id_phong ?? 0;
                            tietItem.Ten_phong = tietHoc.Ten_phong ?? "Không cần phòng";
                            tietItem.isDrag = false;
                            tietItem.isRest = isBreak;

                            string currentKey = $"{(int)ngay}_{ca.Id_ca}_{(int)tietEnum}";
                            // check trùng tiết nghỉ
                            LoadObjectsFromTiet_TietBan(tietHoc, idDonvi);
                            var tietban = DsTietTranhXep(tietHoc);
                            bool isErrorTietBan = tietban.Contains(currentKey);

                            // check trùng tiết đã xếp
                            bool isErrorTrungLap = tietTrungLap.Contains(currentKey);
                            tietItem.isError = isErrorTietBan || isErrorTrungLap;

                            result.timetable.Add(tietItem);
                        }
                        else
                        {
                            tietItem.Id_mon = 0;
                            tietItem.Ten_mon = "";
                            tietItem.Id_lop = 0;
                            tietItem.Ten_lop = "";
                            tietItem.Id_phong = 0;
                            tietItem.Ten_phong = "";
                            tietItem.isDrag = false;
                            tietItem.isRest = isBreak;
                            result.timetable.Add(tietItem);
                        }
                    }
                }
            }

            var tietChuaXep = tiet.Where(t => t.Id_giao_vien == id_gv && t.Id_lich == idlich && t.Ngay <= 0 && t.Tiet <= 0).GroupBy(t => new { t.Id_mon, t.Id_lop, t.Id_phong })
                                  .Select(g => new {
                                      FirstItem = g.First(),
                                      SoTiet = g.Count()
                                  }).ToList();
            foreach (var tietcx in tietChuaXep)
            {
                var t = tietcx.FirstItem;
                var tietItem = new lich_chuaxep_giaovien
                {
                    Id_chitiet = t.Id,
                    Id_don_vi = t.Id_don_vi ?? 0,
                    Id_lich = t.Id_lich,
                    Id_mon = t.Id_mon ?? 0,
                    Ten_mon = t.Ten_mon ?? "",
                    Id_lop = t.Id_lop ?? 0,
                    Ten_lop = t.Ten_lop ?? "",
                    Id_phong = t.Id_phong ?? 0,
                    Ten_phong = t.Ten_phong ?? "Không cần phòng",
                    So_tiet = tietcx.SoTiet
                };

                result.ds_chua_xep.Add(tietItem);
            }

            return result;
        }
        public List<Object_TietOnTapChuaXep> GetTietChuaXep(int idlich)
        {
            _dsTietGoc = List_Object_tiet(idlich);
            if (_dsTietGoc == null || _dsTietGoc.Count == 0)
            {
                return null;
            }
            var result = new List<Object_TietOnTapChuaXep>();
            var tietChuaXep = _dsTietGoc.Where(t => t.Id_lich == idlich && t.Ngay <= 0 && t.Tiet <= 0).GroupBy(t => new { t.Id_mon, t.Id_lop, t.Id_giao_vien, t.Id_phong })
                      .Select(g => new {
                          FirstItem = g.First(),
                          SoTiet = g.Count()
                      }).ToList();
            foreach (var tietcx in tietChuaXep)
            {
                var t = tietcx.FirstItem;
                var tietItem = new Object_TietOnTapChuaXep
                {
                    Id = t.Id,
                    Id_don_vi = t.Id_don_vi ?? 0,
                    Id_lich = t.Id_lich,
                    Id_mon = t.Id_mon ?? 0,
                    Ten_mon = t.Ten_mon ?? "",
                    Id_lop = t.Id_lop ?? 0,
                    Ten_lop = t.Ten_lop ?? "",
                    Id_giao_vien = t.Id_giao_vien,
                    Ten_giao_vien = t.Ten_giao_vien,
                    Id_phong = t.Id_phong ?? 0,
                    Ten_phong = t.Ten_phong ?? "Không cần phòng",
                    So_tiet = tietcx.SoTiet
                };
                result.Add(tietItem);
            }

            return result;
        }
        public void TimViTriXepDuoc_Lop(Object_TietOnTap ObjectTietOnTapOnTap, int idDonvi)
        {
            try
            {
                ObjectTietOnTapOnTap.Ds_vi_tri_xep_duoc.Clear();
                LoadObjectsFromTiet_TietBan(ObjectTietOnTapOnTap, idDonvi);
                var ds_da_xep = _dsTietGoc.Where(c => c.Ngay > 0 && c.Tiet > 0).ToList();
                var tietban = DsTietTranhXep(ObjectTietOnTapOnTap);
                var dsCa = _dsCa;
                var ds_tiet_da_xep_gv = ds_da_xep.Where(t => t.Id_giao_vien == ObjectTietOnTapOnTap.Id_giao_vien).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                for (int i = 0; i < dsCa.Count; i++)
                {
                    for (int ngay = 1; ngay <= _soNgay; ngay++)
                    {
                        for (int tiet = 1; tiet <= dsCa[i].So_tiet; tiet++)
                        {
                            var slotKey = $"{ngay}_{dsCa[i].Id_ca}_{tiet}";

                            if (tietban.Contains(slotKey) || ds_tiet_da_xep_gv.Contains(slotKey))
                                continue;
                            else
                            {
                                ObjectTietOnTapOnTap.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
                                {
                                    Ca = dsCa[i].Id_ca,
                                    Ngay = ngay,
                                    Tiet = tiet,
                                });
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {
                ObjectTietOnTapOnTap.Ds_vi_tri_xep_duoc.Clear();
            }
        }
        public Object_TietOnTap TimViTriXepDuoc_Lop_Tietdaxep(lich_theo_lop tietdaxep, int idlop, int idDonvi)
        {
            try
            {
                Object_TietOnTap ObjectTietOnTap = new Object_TietOnTap
                {
                    Id = tietdaxep.Id_chitiet,
                    Id_don_vi = idDonvi,
                    Id_ca = tietdaxep.Id_ca,
                    Id_giao_vien = tietdaxep.Id_giao_vien,
                    Id_lop = idlop,
                    Id_mon = tietdaxep.Id_mon,
                    Id_phong = tietdaxep.Id_phong,
                    Tiet_thu_may = tietdaxep.Tiet_thu_may,
                    Ngay = tietdaxep.Ngay,
                    Tiet = tietdaxep.Tiet
                };
                LoadObjectsFromTiet_TietBan(ObjectTietOnTap, idDonvi);
                var ds_da_xep = _dsTietGoc.Where(c => c.Ngay > 0 && c.Tiet > 0).ToList();
                var tietban = DsTietTranhXep(ObjectTietOnTap);
                var dsCa = _dsCa;
                var ds_tiet_da_xep_gv = ds_da_xep
                    .Where(t => t.Id_giao_vien == ObjectTietOnTap.Id_giao_vien)
                    .Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}")
                    .ToList();
                for (int i = 0; i < dsCa.Count; i++)
                {
                    for (int ngay = 1; ngay <= _soNgay; ngay++)
                    {
                        for (int tiet = 1; tiet <= dsCa[i].So_tiet; tiet++)
                        {
                            var slotKey = $"{ngay}_{dsCa[i].Id_ca}_{tiet}";
                            if (tietban.Contains(slotKey) || ds_tiet_da_xep_gv.Contains(slotKey))
                                continue;
                            else
                            {
                                ObjectTietOnTap.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
                                {
                                    Ca = dsCa[i].Id_ca,
                                    Ngay = ngay,
                                    Tiet = tiet,
                                });
                            }
                        }
                    }
                }
                return ObjectTietOnTap;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void TimViTriXepDuoc_TietChuaXep(Object_TietOnTap ObjectTietOnTap, int idDonvi)
        {
            try
            {
                ObjectTietOnTap.Ds_vi_tri_xep_duoc.Clear();
                LoadObjectsFromTiet_TietBan(ObjectTietOnTap, idDonvi);
                var ds_da_xep = _dsTietGoc.Where(c => c.Ngay > 0 && c.Tiet > 0).ToList();
                var tietban = DsTietTranhXep(ObjectTietOnTap);
                var dsCa = _dsCa;
                var ds_tiet_da_xep_gv = ds_da_xep.Where(t => t.Id_giao_vien == ObjectTietOnTap.Id_giao_vien).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                var ds_tiet_da_xep_phong = ds_da_xep.Where(t => t.Id_phong == ObjectTietOnTap.Id_phong).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                var ds_tiet_da_xep_lop = ds_da_xep.Where(t => t.Id_lop == ObjectTietOnTap.Id_lop).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                for (int i = 0; i < dsCa.Count; i++)
                {
                    for (int ngay = 1; ngay <= _soNgay; ngay++)
                    {
                        for (int tiet = 1; tiet <= dsCa[i].So_tiet; tiet++)
                        {
                            var slotKey = $"{ngay}_{dsCa[i].Id_ca}_{tiet}";

                            if (tietban.Contains(slotKey) || ds_tiet_da_xep_gv.Contains(slotKey) || ds_tiet_da_xep_phong.Contains(slotKey) || ds_tiet_da_xep_lop.Contains(slotKey))
                                continue;
                            else
                            {
                                ObjectTietOnTap.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
                                {
                                    Ca = dsCa[i].Id_ca,
                                    Ngay = ngay,
                                    Tiet = tiet,
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                ObjectTietOnTap.Ds_vi_tri_xep_duoc.Clear();
            }
        }
        public bool CheckViTriXepDuoc_Lop(Object_TietOnTap ObjectTietOnTap, int Ca, int Ngay, int Tiet, int idDonvi)
        {
            try
            {
                var tietban = DsTietTranhXep(ObjectTietOnTap);
                LoadObjectsFromTiet_TietBan(ObjectTietOnTap, idDonvi);
                var ds_da_xep = _dsTietGoc.Where(c => c.Ngay > 0 && c.Tiet > 0).ToList();
                var ds_tiet_da_xep_gv = ds_da_xep.Where(t => t.Id_giao_vien == ObjectTietOnTap.Id_giao_vien).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();

                var slotKey = $"{Ngay}_{Ca}_{Tiet}";

                if (tietban.Contains(slotKey) || ds_tiet_da_xep_gv.Contains(slotKey))
                    return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public ObjectTietOnTap_theoLopDto TimViTriXepDuoc_byLop(ObjectTietOnTap_theoLopDto tietDachon, int idDonvi)
        {
            try
            {
                int idLop = tietDachon.Id_lop;
                var tiet = tietDachon.timetable?[0];
                if (tiet == null)
                {
                    return new ObjectTietOnTap_theoLopDto();
                }

                int idChitiet = tiet.Id_chitiet;
                int idlich = tiet.Id_lich;
                int idCa = tiet.Id_ca;
                int ngay = tiet.Ngay;
                int tietSo = tiet.Tiet;
                int idMon = tiet.Id_mon;
                int idGiaoVien = tiet.Id_giao_vien;
                int idPhong = tiet.Id_phong;
                bool isRest = tiet.isRest;
                LoadAllInformation(idlich, idDonvi);

                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return new ObjectTietOnTap_theoLopDto();
                }
                var lichBase = GetLichByLop(idLop, idlich, idDonvi);
                if (lichBase == null)
                {
                    return new ObjectTietOnTap_theoLopDto();
                }
                if ( isRest || tiet.isError)
                {
                    return lichBase;
                }

                var dsViTriXepDuoc = new List<(int Ca, int Ngay, int Tiet)>();

                // TH1: ObjectTietOnTap_DaChon có đủ thông tin
                if (idMon > 0)
                {
                    var tietGoc = _dsTietGoc.FirstOrDefault(t => t.Id == idChitiet);
                    if (tietGoc != null)
                    {
                        TimViTriXepDuoc_Lop(tietGoc, idDonvi);
                        if (tietGoc.Ds_vi_tri_xep_duoc != null)
                        {
                            foreach (var viTri in tietGoc.Ds_vi_tri_xep_duoc)
                            {
                                dsViTriXepDuoc.Add((viTri.Ca, viTri.Ngay, viTri.Tiet));
                            }
                        }
                    }
                }
                // TH2: ObjectTietOnTap_DaChon chỉ có thông tin cơ bản
                else
                {
                    var cacTietCuaLop = _dsTietGoc.Where(t => t.Id_lop == idLop).ToList();
                    foreach (var tietGoc in cacTietCuaLop)
                    {
                        TimViTriXepDuoc_Lop(tietGoc, idDonvi);
                        if (tietGoc.Ds_vi_tri_xep_duoc != null && tietGoc.Ds_vi_tri_xep_duoc.Count > 0)
                        {
                            bool coViTriTrung = tietGoc.Ds_vi_tri_xep_duoc.Any(viTri =>
                                viTri.Ngay == ngay && viTri.Tiet == tietSo);

                            if (coViTriTrung)
                            {
                                dsViTriXepDuoc.Add((tietGoc.Id_ca, tietGoc.Ngay, tietGoc.Tiet));
                            }
                        }
                    }
                }
                dsViTriXepDuoc.Add((idCa, ngay, tietSo));
                // Cập nhật isDrag cho các tiết trong lichBase
                foreach (var tietdaxep in lichBase.timetable)
                {
                    bool isDrag = false;
                    if (tietdaxep.Id_mon > 0)
                    {
                        var dsvitri_tietdaxep = new List<(int Ca, int Ngay, int Tiet)>();
                        var ObjectTietOnTap = TimViTriXepDuoc_Lop_Tietdaxep(tietdaxep, idLop, idDonvi);

                        if (ObjectTietOnTap?.Ds_vi_tri_xep_duoc != null)
                        {
                            foreach (var viTri in ObjectTietOnTap.Ds_vi_tri_xep_duoc)
                            {
                                dsvitri_tietdaxep.Add((viTri.Ca, viTri.Ngay, viTri.Tiet));
                            }
                        }


                        //if (!tietdaxep.isLock)
                        //{
                        //    var vitri = (tietdaxep.Id_ca, tietdaxep.Ngay, tietdaxep.Tiet);

                        //    // Kiểm tra tiết gốc có trong ds vị trí xếp được của tiết đã xếp không
                        //    bool check_tietgoc = dsvitri_tietdaxep.Any(vt =>
                        //        vt.Ca == idCa &&
                        //        vt.Ngay == ngay &&
                        //        vt.Tiet == tietSo);

                        //    // Kiểm tra tiết đã xếp có trong dsViTriXepDuoc không
                        //    bool check_tietdaxep = dsViTriXepDuoc.Any(vt =>
                        //        vt.Ca == vitri.Id_ca &&
                        //        vt.Ngay == vitri.Ngay &&
                        //        vt.Tiet == vitri.Tiet);

                        //    isDrag = check_tietdaxep && check_tietgoc;
                        //}
                    }

                    else
                    {
                        isDrag = dsViTriXepDuoc.Any(vt =>
                                vt.Ca == tietdaxep.Id_ca &&
                                vt.Ngay == tietdaxep.Ngay &&
                                vt.Tiet == tietdaxep.Tiet);
                    }
                    if (tietdaxep.Id_ca == idCa && tietdaxep.Ngay == ngay && tietdaxep.Tiet == tietSo)
                    {
                        isDrag = true;
                    }
                    tietdaxep.isDrag = isDrag;
                }
                return lichBase;
            }
            catch (Exception)
            {
                return new ObjectTietOnTap_theoLopDto();
            }
        }
        public ObjectTietOnTap_theoLopDto TimViTriXepDuoc_TietChuaXep_byLop(ObjectTietOnTap_theoLopDto tietDachon, int idDonvi)
        {
            try
            {
                int idLop = tietDachon.Id_lop;
                var tiet = tietDachon.ds_chua_xep?[0];
                if (tiet == null)
                {
                    return new ObjectTietOnTap_theoLopDto();
                }

                int idChitiet = tiet.Id_chitiet;
                int idlich = tiet.Id_lich;

                LoadAllInformation(idlich, idDonvi);

                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return new ObjectTietOnTap_theoLopDto();
                }
                var lichBase = GetLichByLop(idLop, idlich, idDonvi);
                if (lichBase == null)
                {
                    return new ObjectTietOnTap_theoLopDto();
                }

                var dsViTriXepDuoc = new List<(int Ca, int Ngay, int Tiet)>();
                var tietGoc = _dsTietGoc.FirstOrDefault(t => t.Id == idChitiet);
                if (tietGoc != null)
                {
                    TimViTriXepDuoc_TietChuaXep(tietGoc, idDonvi);
                    if (tietGoc.Ds_vi_tri_xep_duoc != null)
                    {
                        foreach (var viTri in tietGoc.Ds_vi_tri_xep_duoc)
                        {
                            dsViTriXepDuoc.Add((viTri.Ca, viTri.Ngay, viTri.Tiet));
                        }
                    }
                }
                // Cập nhật isDrag cho các tiết trong lichBase
                foreach (var tietdaxep in lichBase.timetable)
                {
                    bool isDrag = dsViTriXepDuoc.Any(vt =>
                                          vt.Ca == tietdaxep.Id_ca &&
                                          vt.Ngay == tietdaxep.Ngay &&
                                          vt.Tiet == tietdaxep.Tiet);
                    tietdaxep.isDrag = isDrag;
                }
                return lichBase;
            }
            catch (Exception)
            {
                return new ObjectTietOnTap_theoLopDto();
            }
        }
        public ObjectTietOnTap_theoGVDto TimViTriXepDuoc_TietChuaXep_byGV(ObjectTietOnTap_theoGVDto tietDachon, int idDonvi)
        {
            try
            {
                int idgv = tietDachon.Id_giao_vien;
                var tiet = tietDachon.ds_chua_xep?[0];
                if (tiet == null)
                {
                    return new ObjectTietOnTap_theoGVDto();
                }

                int idChitiet = tiet.Id_chitiet;
                int idlich = tiet.Id_lich;

                LoadAllInformation(idlich, idDonvi);

                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return new ObjectTietOnTap_theoGVDto();
                }
                var lichBase = GetLichByGiaovien(idgv, idlich, idDonvi);
                if (lichBase == null)
                {
                    return new ObjectTietOnTap_theoGVDto();
                }

                var dsViTriXepDuoc = new List<(int Ca, int Ngay, int Tiet)>();
                var tietGoc = _dsTietGoc.FirstOrDefault(t => t.Id == idChitiet);
                if (tietGoc != null)
                {
                    TimViTriXepDuoc_TietChuaXep(tietGoc, idDonvi);
                    if (tietGoc.Ds_vi_tri_xep_duoc != null)
                    {
                        foreach (var viTri in tietGoc.Ds_vi_tri_xep_duoc)
                        {
                            dsViTriXepDuoc.Add((viTri.Ca, viTri.Ngay, viTri.Tiet));
                        }
                    }
                }
                // Cập nhật isDrag cho các tiết trong lichBase
                foreach (var tietdaxep in lichBase.timetable)
                {
                    bool isDrag = dsViTriXepDuoc.Any(vt =>
                                        vt.Ca == tietdaxep.Id_ca &&
                                        vt.Ngay == tietdaxep.Ngay &&
                                        vt.Tiet == tietdaxep.Tiet);

                    tietdaxep.isDrag = isDrag;
                }
                return lichBase;
            }
            catch (Exception)
            {
                return new ObjectTietOnTap_theoGVDto();
            }
        }
        public List<Object_TietOnTap> TimTietXepDuoc_byLop(ObjectTietOnTap_theoLopDto tietDachon, int idDonvi)
        {
            try
            {
                int idLop = tietDachon.Id_lop;
                var tiet = tietDachon.timetable?[0];
                if (tiet == null)
                {
                    return new List<Object_TietOnTap>();
                }

                int idChitiet = tiet.Id_chitiet;
                int idlich = tiet.Id_lich;
                int idCa = tiet.Id_ca;
                int ngay = tiet.Ngay;
                int tietSo = tiet.Tiet;

                LoadAllInformation(idlich, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return new List<Object_TietOnTap>();
                }

                var lichBase = GetLichByLop(idLop, idlich, idDonvi);
                if (lichBase == null)
                {
                    return new List<Object_TietOnTap>();
                }

                var dsViTriXepDuoc = new List<(int Ca, int Ngay, int Tiet)>();
                var tietChuaXep = new List<Object_TietOnTap>();

                var cacTietCuaLop = _dsTietGoc.Where(t => t.Id_lop == idLop && t.Ngay == 0 && t.Tiet == 0).ToList();
                foreach (var tietGoc in cacTietCuaLop)
                {
                    TimViTriXepDuoc_TietChuaXep(tietGoc, idDonvi);
                    if (tietGoc.Ds_vi_tri_xep_duoc != null && tietGoc.Ds_vi_tri_xep_duoc.Count > 0)
                    {
                        bool coViTriTrung = tietGoc.Ds_vi_tri_xep_duoc.Any(viTri => viTri.Ca == idCa &&
                            viTri.Ngay == ngay && viTri.Tiet == tietSo);

                        if (coViTriTrung)
                        {
                            var tietMoi = new Object_TietOnTap
                            {
                                Id = tietGoc.Id,
                                Id_don_vi = tietGoc.Id_don_vi ?? 0,
                                Id_lich = tietGoc.Id_lich,
                                Id_lop = tietGoc.Id_lop,
                                Ten_lop = tietGoc.Ten_lop,
                                Id_mon = tietGoc.Id_mon ?? 0,
                                Ten_mon = tietGoc.Ten_mon ?? "",
                                Id_giao_vien = tietGoc.Id_giao_vien ?? 0,
                                Ten_giao_vien = tietGoc.Ten_giao_vien ?? "",
                                Id_phong = tietGoc.Id_phong ?? 0,
                                Ten_phong = tietGoc.Ten_phong ?? "Không cần phòng",
                                Tiet_thu_may = tietGoc.Tiet_thu_may,
                                Id_ca = tietGoc.Id_ca,
                                Ngay = tietGoc.Ngay,
                                Tiet = tietGoc.Tiet,
                            };
                            tietChuaXep.Add(tietMoi);
                        }
                    }
                }

                return tietChuaXep;
            }
            catch (Exception)
            {
                return new List<Object_TietOnTap>();
            }
        }
        public void TimViTriXepDuoc_GV(Object_TietOnTap ObjectTietOnTap, int idDonvi)
        {
            try
            {
                ObjectTietOnTap.Ds_vi_tri_xep_duoc.Clear();
                var ds_da_xep = _dsTietGoc.Where(c => c.Ngay > 0 && c.Tiet > 0).ToList();
                LoadObjectsFromTiet_TietBan(ObjectTietOnTap, idDonvi);
                var tietban = DsTietTranhXep(ObjectTietOnTap);
                var dsCa = _dsCa;
                var ds_tiet_da_xep_phong = ds_da_xep.Where(t => t.Id_phong == ObjectTietOnTap.Id_phong).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                var ds_tiet_da_xep_lop = ds_da_xep.Where(t => t.Id_lop == ObjectTietOnTap.Id_lop).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                for (int i = 0; i < dsCa.Count; i++)
                {
                    for (int ngay = 1; ngay <= _soNgay; ngay++)
                    {
                        for (int tiet = 1; tiet <= dsCa[i].So_tiet; tiet++)
                        {
                            var slotKey = $"{ngay}_{dsCa[i].Id_ca}_{tiet}";

                            if (tietban.Contains(slotKey) || ds_tiet_da_xep_phong.Contains(slotKey) || ds_tiet_da_xep_lop.Contains(slotKey))
                                continue;
                            else
                            {
                                ObjectTietOnTap.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
                                {
                                    Ca = dsCa[i].Id_ca,
                                    Ngay = ngay,
                                    Tiet = tiet,
                                });
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {
                ObjectTietOnTap.Ds_vi_tri_xep_duoc.Clear();
            }
        }
        public Object_TietOnTap TimViTriXepDuoc_GV_Tietdaxep(lich_theo_giaovien tietdaxep, int idgiaovien, int idDonvi)
        {
            try
            {
                Object_TietOnTap ObjectTietOnTap = new Object_TietOnTap
                {
                    Id = tietdaxep.Id_chitiet,
                    Id_don_vi = idDonvi,
                    Id_ca = tietdaxep.Id_ca,
                    Id_giao_vien = idgiaovien,
                    Id_lop = tietdaxep.Id_lop,
                    Id_mon = tietdaxep.Id_mon,
                    Id_phong = tietdaxep.Id_phong,
                    Tiet_thu_may = tietdaxep.Tiet_thu_may,
                    Ngay = tietdaxep.Ngay,
                    Tiet = tietdaxep.Tiet
                };
                LoadObjectsFromTiet_TietBan(ObjectTietOnTap, idDonvi);
                var ds_da_xep = _dsTietGoc.Where(c => c.Ngay > 0 && c.Tiet > 0).ToList();
                var tietban = DsTietTranhXep(ObjectTietOnTap);
                var dsCa = _dsCa;
                var ds_tiet_da_xep_phong = ds_da_xep.Where(t => t.Id_phong == ObjectTietOnTap.Id_phong).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                var ds_tiet_da_xep_lop = ds_da_xep.Where(t => t.Id_lop == ObjectTietOnTap.Id_lop).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                for (int i = 0; i < dsCa.Count; i++)
                {
                    for (int ngay = 1; ngay <= _soNgay; ngay++)
                    {
                        for (int tiet = 1; tiet <= dsCa[i].So_tiet; tiet++)
                        {
                            var slotKey = $"{ngay}_{dsCa[i].Id_ca}_{tiet}";
                            if (tietban.Contains(slotKey) || ds_tiet_da_xep_phong.Contains(slotKey) || ds_tiet_da_xep_lop.Contains(slotKey))
                                continue;
                            else
                            {
                                ObjectTietOnTap.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
                                {
                                    Ca = dsCa[i].Id_ca,
                                    Ngay = ngay,
                                    Tiet = tiet,
                                });
                            }
                        }
                    }
                }
                return ObjectTietOnTap;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool CheckViTriXepDuoc_GV(Object_TietOnTap ObjectTietOnTap, int Ca, int Ngay, int Tiet, int idDonvi)
        {
            try
            {
                LoadObjectsFromTiet_TietBan(ObjectTietOnTap, idDonvi);
                var tietban = DsTietTranhXep(ObjectTietOnTap);
                var ds_da_xep = _dsTietGoc.Where(c => c.Ngay > 0 && c.Tiet > 0).ToList();
                var ds_tiet_da_xep_phong = ds_da_xep.Where(t => t.Id_phong == ObjectTietOnTap.Id_phong).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                var slotKey = $"{Ngay}_{Ca}_{Tiet}";
                if (tietban.Contains(slotKey) || ds_tiet_da_xep_phong.Contains(slotKey))
                    return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public ObjectTietOnTap_theoGVDto TimViTriXepDuoc_byGV(ObjectTietOnTap_theoGVDto tietDachon, int idDonvi)
        {
            try
            {
                int idGV = tietDachon.Id_giao_vien;
                var tiet = tietDachon.timetable?[0];
                if (tiet == null)
                {
                    return new();
                }

                int idChitiet = tiet.Id_chitiet;
                int idlich = tiet.Id_lich;
                int idCa = tiet.Id_ca;
                int ngay = tiet.Ngay;
                int tietSo = tiet.Tiet;
                int idMon = tiet.Id_mon ?? 0;
                int idlop = tiet.Id_lop ?? 0;
                int idPhong = tiet.Id_phong ?? 0;
                bool isRest = tiet.isRest;
                //bool isLock = tiet.isLock;

                LoadAllInformation(idlich, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return new ObjectTietOnTap_theoGVDto();
                }

                var lichBase = GetLichByGiaovien(idGV, idlich, idDonvi);
                if (lichBase == null)
                {
                    return new ObjectTietOnTap_theoGVDto();
                }
                if ( isRest || tiet.isError)
                {
                    return lichBase;
                }
                var dsViTriXepDuoc = new List<(int Ca, int Ngay, int Tiet)>();

                // TH1: ObjectTietOnTap_DaChon có đủ thông tin
                if (idMon > 0)
                {
                    var tietGoc = _dsTietGoc.FirstOrDefault(t => t.Id == idChitiet);
                    if (tietGoc != null)
                    {
                        TimViTriXepDuoc_GV(tietGoc, idDonvi);
                        if (tietGoc.Ds_vi_tri_xep_duoc != null && tietGoc.Ds_vi_tri_xep_duoc.Count > 0)
                        {
                            foreach (var viTri in tietGoc.Ds_vi_tri_xep_duoc)
                            {
                                dsViTriXepDuoc.Add((viTri.Ca, viTri.Ngay, viTri.Tiet));
                            }
                        }
                    }
                }
                // TH2: ObjectTietOnTap_DaChon chỉ có thông tin cơ bản
                else
                {
                    var cacTietCuaGV = _dsTietGoc.Where(t => t.Id_giao_vien == idGV).ToList();
                    foreach (var tietGoc in cacTietCuaGV)
                    {
                        TimViTriXepDuoc_GV(tietGoc, idDonvi);
                        if (tietGoc.Ds_vi_tri_xep_duoc != null && tietGoc.Ds_vi_tri_xep_duoc.Count > 0)
                        {
                            bool coViTriTrung = tietGoc.Ds_vi_tri_xep_duoc.Any(viTri =>
                                viTri.Ngay == ngay && viTri.Tiet == tietSo);

                            if (coViTriTrung)
                            {
                                dsViTriXepDuoc.Add((tietGoc.Id_ca, tietGoc.Ngay, tietGoc.Tiet));
                            }
                        }
                    }
                }
                dsViTriXepDuoc.Add((idCa, ngay, tietSo));
                // Cập nhật isDrag cho các tiết trong lichBase
                foreach (var tietdaxep in lichBase.timetable)
                {
                    bool isDrag = false;
                    if (tietdaxep.Id_mon > 0)
                    {
                        var dsvitri_tietdaxep = new List<(int Ca, int Ngay, int Tiet)>();
                        var ObjectTietOnTap = TimViTriXepDuoc_GV_Tietdaxep(tietdaxep, idGV, idDonvi);

                        if (ObjectTietOnTap?.Ds_vi_tri_xep_duoc != null)
                        {
                            foreach (var viTri in ObjectTietOnTap.Ds_vi_tri_xep_duoc)
                            {
                                dsvitri_tietdaxep.Add((viTri.Ca, viTri.Ngay, viTri.Tiet));
                            }
                        }


                        //if (!tietdaxep.isLock)
                        //{
                        //    var vitri = (tietdaxep.Id_ca, tietdaxep.Ngay, tietdaxep.Tiet);

                        //    // Kiểm tra vị trí của tiết gốc có trong ds xếp được của tiết đang xét không
                        //    bool check_tietgoc = dsvitri_tietdaxep.Any(vt =>
                        //        vt.Ca == idCa &&
                        //        vt.Ngay == ngay &&
                        //        vt.Tiet == tietSo);

                        //    // Kiểm tra vị trí hiện tại có trong dsViTriXepDuoc không
                        //    bool check_tietdaxep = dsViTriXepDuoc.Any(vt =>
                        //        vt.Ca == vitri.Id_ca &&
                        //        vt.Ngay == vitri.Ngay &&
                        //        vt.Tiet == vitri.Tiet);

                        //    isDrag = check_tietdaxep && check_tietgoc;
                        //}
                    }
                    else
                    {
                        isDrag = dsViTriXepDuoc.Any(vt =>
                                vt.Ca == tietdaxep.Id_ca &&
                                vt.Ngay == tietdaxep.Ngay &&
                                vt.Tiet == tietdaxep.Tiet);
                    }
                    if (tietdaxep.Id_ca == idCa && tietdaxep.Ngay == ngay && tietdaxep.Tiet == tietSo)
                    {
                        isDrag = true;
                    }
                    tietdaxep.isDrag = isDrag;
                }
                return lichBase;
            }
            catch (Exception)
            {
                return new ObjectTietOnTap_theoGVDto();
            }
        }
        public List<Object_TietOnTap> TimTietXepDuoc_byGV(ObjectTietOnTap_theoGVDto tietDachon, int idDonvi)
        {
            try
            {
                int idGV = tietDachon.Id_giao_vien;
                var tiet = tietDachon.timetable?[0];
                if (tiet == null)
                {
                    return new();
                }

                int idChitiet = tiet.Id_chitiet;
                int idlich = tiet.Id_lich;
                int idCa = tiet.Id_ca;
                int ngay = tiet.Ngay;
                int tietSo = tiet.Tiet;
                int idMon = tiet.Id_mon ?? 0;
                int idlop = tiet.Id_lop ?? 0;
                int idPhong = tiet.Id_phong ?? 0;

                LoadAllInformation(idlich, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return new List<Object_TietOnTap>();
                }

                var lichBase = GetLichByGiaovien(idGV, idlich, idDonvi);
                if (lichBase == null)
                {
                    return new List<Object_TietOnTap>();
                }

                var dsViTriXepDuoc = new List<(int Ca, int Ngay, int Tiet)>();
                var tietChuaXep = new List<Object_TietOnTap>();

                var cacTietCuaGV = _dsTietGoc.Where(t => t.Id_giao_vien == idGV && t.Ngay == 0 && t.Tiet == 0).ToList();
                foreach (var tietGoc in cacTietCuaGV)
                {
                    TimViTriXepDuoc_TietChuaXep(tietGoc, idDonvi);
                    if (tietGoc.Ds_vi_tri_xep_duoc != null && tietGoc.Ds_vi_tri_xep_duoc.Count > 0)
                    {
                        bool coViTriTrung = tietGoc.Ds_vi_tri_xep_duoc.Any(viTri => viTri.Ca == idCa &&
                            viTri.Ngay == ngay && viTri.Tiet == tietSo);

                        if (coViTriTrung)
                        {
                            var tietMoi = new Object_TietOnTap
                            {
                                Id = tietGoc.Id,
                                Id_don_vi = tietGoc.Id_don_vi ?? 0,
                                Id_lich = tietGoc.Id_lich,
                                Id_lop = tietGoc.Id_lop,
                                Ten_lop = tietGoc.Ten_lop,
                                Id_mon = tietGoc.Id_mon ?? 0,
                                Ten_mon = tietGoc.Ten_mon ?? "",
                                Id_giao_vien = tietGoc.Id_giao_vien ?? 0,
                                Ten_giao_vien = tietGoc.Ten_giao_vien ?? "",
                                Id_phong = tietGoc.Id_phong ?? 0,
                                Ten_phong = tietGoc.Ten_phong ?? "Không cần phòng",
                                Tiet_thu_may = tietGoc.Tiet_thu_may,
                                Id_ca = tietGoc.Id_ca,
                                Ngay = tietGoc.Ngay,
                                Tiet = tietGoc.Tiet,
                            };
                            tietChuaXep.Add(tietMoi);
                        }
                    }
                }

                return tietChuaXep;
            }
            catch (Exception)
            {
                return new List<Object_TietOnTap>();
            }
        }
        public (bool success, ObjectTietOnTap_theoLopDto result) DoiChoHaiTiet_Lop(ObjectTietOnTap_theoLopDto tietDachon, int idDonvi)
        {
            try
            {
                if (tietDachon?.timetable == null || tietDachon.timetable.Count < 2)
                {
                    return (false, new ObjectTietOnTap_theoLopDto());
                }

                var tiet1 = tietDachon.timetable[0];
                var tiet2 = tietDachon.timetable[1];

                if (tiet1 == null || tiet2 == null)
                {
                    return (false, new ObjectTietOnTap_theoLopDto());
                }

                int ngay1 = tiet1.Ngay;
                int tietSo1 = tiet1.Tiet;
                int ca1 = tiet1.Id_ca;
                int ngay2 = tiet2.Ngay;
                int tietSo2 = tiet2.Tiet;
                int ca2 = tiet2.Id_ca;
                //bool lock1 = tiet1.isLock;
                //bool lock2 = tiet2.isLock;
                // Tạo Object_Tiet từ tiết 1
                var ObjectTietOnTap1 = new Object_TietOnTap
                {
                    Id_lich = tiet1.Id_lich,
                    Id_lop = tietDachon.Id_lop,
                    Id_mon = tiet1.Id_mon,
                    Id_giao_vien = tiet1.Id_giao_vien,
                    Id_phong = tiet1.Id_phong,
                    Id_ca = tiet1.Id_ca,
                    Tiet_thu_may = tiet1.Tiet_thu_may,
                };

                // Tạo Object_Tiet từ tiết 2  
                var ObjectTietOnTap2 = new Object_TietOnTap
                {
                    Id_lich = tiet2.Id_lich,
                    Id_lop = tietDachon.Id_lop,
                    Id_mon = tiet2.Id_mon,
                    Id_giao_vien = tiet2.Id_giao_vien,
                    Id_phong = tiet2.Id_phong,
                    Id_ca = tiet2.Id_ca,
                    Tiet_thu_may = tiet2.Tiet_thu_may
                };
                LoadAllInformation(ObjectTietOnTap1.Id_lich, idDonvi);
                bool check = true;
                bool updateTiet1 = false;
                bool updateTiet2 = false;
                if (ObjectTietOnTap1.Id_mon == 0)
                {
                    check = CheckViTriXepDuoc_Lop(ObjectTietOnTap2, ca1, ngay1, tietSo1, idDonvi);

                    if (check)
                    {
                        updateTiet2 = UpdateTiet(ObjectTietOnTap2, ca1, ngay1, tietSo1);
                        if (!updateTiet2)
                        {
                            return (false, new ObjectTietOnTap_theoLopDto());
                        }
                    }
                    else
                    {
                        return (false, new ObjectTietOnTap_theoLopDto());
                    }
                }
                else if (ObjectTietOnTap2.Id_mon == 0)
                {
                    check = CheckViTriXepDuoc_Lop(ObjectTietOnTap1, ca2, ngay2, tietSo2, idDonvi);
                    if (check)
                    {
                        updateTiet1 = UpdateTiet(ObjectTietOnTap1, ca2, ngay2, tietSo2);
                        if (!updateTiet1)
                        {
                            return (false, new ObjectTietOnTap_theoLopDto());
                        }
                    }
                    else
                    {
                        return (false, new ObjectTietOnTap_theoLopDto());
                    }
                }
                else
                {
                    var check_t1 = CheckViTriXepDuoc_Lop(ObjectTietOnTap1, ca2, ngay2, tietSo2, idDonvi);
                    var check_t2 = CheckViTriXepDuoc_Lop(ObjectTietOnTap2, ca1, ngay1, tietSo1, idDonvi);
                    if (check_t1 && check_t2)
                    {
                        updateTiet1 = UpdateTiet(ObjectTietOnTap1, ca2, ngay2, tietSo2);
                        updateTiet2 = UpdateTiet(ObjectTietOnTap2, ca1, ngay1, tietSo1);
                        if (!updateTiet1 || !updateTiet2)
                        {
                            return (false, new ObjectTietOnTap_theoLopDto());
                        }
                    }
                    else
                    {
                        return (false, new ObjectTietOnTap_theoLopDto());
                    }

                }

                var ketQuaCheckViTri = TimViTriXepDuoc_byLop(tietDachon, idDonvi);
                return (true, ketQuaCheckViTri);

            }
            catch (Exception ex)
            {
                return (false, new ObjectTietOnTap_theoLopDto());
            }
        }
        public (bool success, ObjectTietOnTap_theoGVDto result) DoiChoHaiTiet_GV(ObjectTietOnTap_theoGVDto tietDachon, int idDonvi)
        {
            try
            {
                if (tietDachon?.timetable == null || tietDachon.timetable.Count < 2)
                {
                    return (false, new ObjectTietOnTap_theoGVDto());
                }

                var tiet1 = tietDachon.timetable[0];
                var tiet2 = tietDachon.timetable[1];

                if (tiet1 == null || tiet2 == null)
                {
                    return (false, new ObjectTietOnTap_theoGVDto());
                }

                int ngay1 = tiet1.Ngay;
                int tietSo1 = tiet1.Tiet;
                int ca1 = tiet1.Id_ca;
                int ngay2 = tiet2.Ngay;
                int tietSo2 = tiet2.Tiet;
                int ca2 = tiet2.Id_ca;
                //bool lock1 = tiet1.isLock;
                //bool lock2 = tiet2.isLock;
                // Tạo Object_Tiet từ tiết 1
                var ObjectTietOnTap1 = new Object_TietOnTap
                {
                    Id_lich = tiet1.Id_lich,
                    Id_lop = tiet1.Id_lop,
                    Id_mon = tiet1.Id_mon,
                    Id_giao_vien = tietDachon.Id_giao_vien,
                    Id_phong = tiet1.Id_phong,
                    Id_ca = tiet1.Id_ca,
                    Tiet_thu_may = tiet1.Tiet_thu_may
                };

                // Tạo Object_Tiet từ tiết 2  
                var ObjectTietOnTap2 = new Object_TietOnTap
                {
                    Id_lich = tiet2.Id_lich,
                    Id_lop = tiet2.Id_lop,
                    Id_mon = tiet2.Id_mon,
                    Id_giao_vien = tietDachon.Id_giao_vien,
                    Id_phong = tiet2.Id_phong,
                    Id_ca = tiet2.Id_ca,
                    Tiet_thu_may = tiet2.Tiet_thu_may
                };
                LoadAllInformation(ObjectTietOnTap1.Id_lich, idDonvi);
                bool check = true;
                bool updateTiet1 = false;
                bool updateTiet2 = false;
                if (ObjectTietOnTap1.Id_mon == 0)
                {
                    check = CheckViTriXepDuoc_GV(ObjectTietOnTap2, ca1, ngay1, tietSo1, idDonvi);
                    if (check)
                    {
                        updateTiet2 = UpdateTiet(ObjectTietOnTap2, ca1, ngay1, tietSo1);
                        if (!updateTiet2)
                        {
                            return (false, new ObjectTietOnTap_theoGVDto());
                        }
                    }
                    else
                    {
                        return (false, new ObjectTietOnTap_theoGVDto());
                    }
                }
                else if (ObjectTietOnTap2.Id_mon == 0)
                {
                    check = CheckViTriXepDuoc_GV(ObjectTietOnTap1, ca2, ngay2, tietSo2, idDonvi);
                    if (check)
                    {
                        updateTiet1 = UpdateTiet(ObjectTietOnTap1, ca2, ngay2, tietSo2);
                        if (!updateTiet1)
                        {
                            return (false, new ObjectTietOnTap_theoGVDto());
                        }
                    }
                    else
                    {
                        return (false, new ObjectTietOnTap_theoGVDto());
                    }
                }
                else
                {
                    var check_t1 = CheckViTriXepDuoc_GV(ObjectTietOnTap1, ca2, ngay2, tietSo2, idDonvi);
                    var check_t2 = CheckViTriXepDuoc_GV(ObjectTietOnTap2, ca1, ngay1, tietSo1, idDonvi);
                    if (check_t1 && check_t2 )
                    {
                        updateTiet1 = UpdateTiet(ObjectTietOnTap1, ca2, ngay2, tietSo2);
                        updateTiet2 = UpdateTiet(ObjectTietOnTap2, ca1, ngay1, tietSo1);
                        if (!updateTiet1 || !updateTiet2)
                        {
                            return (false, new ObjectTietOnTap_theoGVDto());
                        }
                    }
                    else
                    {
                        return (false, new ObjectTietOnTap_theoGVDto());
                    }

                }
                var ketQuaCheckViTri = TimViTriXepDuoc_byGV(tietDachon, idDonvi);
                return (true, ketQuaCheckViTri);

            }
            catch (Exception ex)
            {
                return (false, new ObjectTietOnTap_theoGVDto());
            }
        }
        public bool UpdateTietChuaXep(Object_TietOnTap tietDachon, int idDonvi)
        {
            try
            {

                bool updateTiet1 = UpdateTiet(tietDachon, tietDachon.Id_ca, tietDachon.Ngay, tietDachon.Tiet);

                if (!updateTiet1)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool HuyXep(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return false;
                }
                var tiet = _context.Chitiet_Lichontap.FirstOrDefault(c => c.Id == id);
                if (tiet != null)
                {
                    tiet.Id_ca = 0;
                    tiet.Ngay = 0;
                    tiet.Tiet = 0;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
