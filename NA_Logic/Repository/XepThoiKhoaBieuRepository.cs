using EFCore.BulkExtensions;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NetTopologySuite.Triangulate.Tri;
using NuGet.DependencyResolver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class XepThoiKhoaBieuRepository : IXepThoiKhoaBieuRepository
    {
        private readonly NA_DbContext _context;
        private Object_Monhoc _ObjectMon = new Object_Monhoc();
        private Object_Lophoc _ObjectLop = new Object_Lophoc();
        private Object_Giaovien _ObjectGiaovien = new Object_Giaovien();
        private Object_Phonghoc _ObjectPhong = new Object_Phonghoc();
        private Object_lop_mon _ObjectLopMon = new Object_lop_mon();
        private Object_MonKhoi _ObjectMonKhoi = new Object_MonKhoi();
        private List<Object_Tohopmon> _ObjectTohopmon = new List<Object_Tohopmon>();
        //private List<object_tiet_co_dinh> _ObjectTietcodinh;
        private List<Object_Tiet> _dsTietGoc = new List<Object_Tiet>();

        private List<Object_Monhoc> _dsObjectMon = new List<Object_Monhoc>();
        private List<Object_Lophoc> _dsObjectLop = new List<Object_Lophoc>();
        private List<Object_Giaovien> _dsObjectGiaovien = new List<Object_Giaovien>();
        private List<Object_Phonghoc> _dsObjectPhong= new List<Object_Phonghoc>();
        private List<Object_lop_mon> _dsObjectLopMon = new List<Object_lop_mon>();
        private List<Object_MonKhoi> _dsObjectMonKhoi = new List<Object_MonKhoi>();
        private List<Object_Tohopmon> _dsObjectTohopmon = new List<Object_Tohopmon>();
        private List<object_tiet_co_dinh> _dsObjectTietcodinh = new List<object_tiet_co_dinh>();
        private List<Object_ca> _dsCa;
        private int _soNgay;
        public XepThoiKhoaBieuRepository(NA_DbContext context)
        {
            _context = context;
        }
        //object tiết
        public List<Object_Tiet> List_Object_tiet(int idtkb)
        {
            try
            {
                var paramIdTkb = new SqlParameter("Id_tkb", SqlDbType.Int)
                {
                    Value = idtkb
                };
                var result = _context.Set<Chitiet_Thoikhoabieu_List>().FromSqlRaw("EXEC Get_Object  @Id_tkb = @Id_tkb", paramIdTkb)
                    .ToList();

                if (result == null) return null;
                var ds_tiet = new List<Object_Tiet>();
                foreach (var item in result)
                {
                    ds_tiet.Add(new Object_Tiet
                    {
                        Id = item.Id,
                        Id_don_vi = item.Id_don_vi,
                        Id_tkb = item.Id_tkb,
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
                        Khoa = item.Khoa,
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
        public void LoadObjectsFromTiet(Object_Tiet objectTiet, int idDonvi, List<Object_Tiet> ds_da_xep, List<Object_Tiet> ds_chua_xep)
        {
            try
            {
                if (objectTiet == null)
                {
                    return;
                }

                _ObjectMon = _dsObjectMon.FirstOrDefault(c => c.Id_mon == objectTiet.Id_mon);
                _ObjectLop = _dsObjectLop.FirstOrDefault(c => c.Id_lop == objectTiet.Id_lop);
                _ObjectGiaovien = _dsObjectGiaovien.FirstOrDefault(c => c.Id_giao_vien == objectTiet.Id_giao_vien);
                _ObjectGiaovien.ds_tiet_da_xep = ds_da_xep;
                _ObjectGiaovien.ds_tiet_chua_xep = ds_chua_xep;
                if (objectTiet.Id_phong == 0)
                {
                    _ObjectPhong = null;
                }
                else
                {
                    _ObjectPhong = _dsObjectPhong.FirstOrDefault(c => c.Id_phong == objectTiet.Id_phong);
                }
                _ObjectLopMon = _dsObjectLopMon.FirstOrDefault(c => c.Id_lop == objectTiet.Id_lop && c.Id_mon == objectTiet.Id_mon);
                _ObjectMonKhoi = _dsObjectMonKhoi.FirstOrDefault(c => c.Id_mon == objectTiet.Id_mon && c.Id_ban == _ObjectLop.Id_ban && c.Id_khoi == _ObjectLop.Id_khoi);
                _ObjectTohopmon = _dsObjectTohopmon.Where(c => c.Id_ban == _ObjectLop.Id_ban && c.Id_khoi == _ObjectLop.Id_khoi &&
                                                                                (c.Id_mon_1 == objectTiet.Id_mon || c.Id_mon_2 == objectTiet.Id_mon || c.Id_mon_3 == objectTiet.Id_mon)).ToList();
            }
            catch (Exception ex)
            {
                return;
            }
        }
        public void LoadObjectsFromTiet_TietBan(Object_Tiet objectTiet, int? idDonvi)
        {
            try
            {
                if (objectTiet == null)
                {
                    return;
                }

                _ObjectMon = _dsObjectMon.FirstOrDefault(c => c.Id_mon == objectTiet.Id_mon);
                _ObjectLop = _dsObjectLop.FirstOrDefault(c => c.Id_lop == objectTiet.Id_lop);
                _ObjectGiaovien = _dsObjectGiaovien.FirstOrDefault(c => c.Id_giao_vien == objectTiet.Id_giao_vien);
                if (objectTiet.Id_phong == 0)
                {
                    _ObjectPhong = null;
                }
                else
                {
                    _ObjectPhong = _dsObjectPhong.FirstOrDefault(c => c.Id_phong == objectTiet.Id_phong);
                }
                _ObjectLopMon = _dsObjectLopMon.FirstOrDefault(c => c.Id_lop == objectTiet.Id_lop && c.Id_mon == objectTiet.Id_mon);
                _ObjectMonKhoi = _dsObjectMonKhoi.FirstOrDefault(c => c.Id_mon == objectTiet.Id_mon && c.Id_ban == _ObjectLop.Id_ban && c.Id_khoi == _ObjectLop.Id_khoi);
                _ObjectTohopmon = _dsObjectTohopmon.Where(c => c.Id_ban == _ObjectLop.Id_ban && c.Id_khoi == _ObjectLop.Id_khoi &&
                                                                                (c.Id_mon_1 == objectTiet.Id_mon || c.Id_mon_2 == objectTiet.Id_mon || c.Id_mon_3 == objectTiet.Id_mon)).ToList();
            }
            catch (Exception)
            {
                return;
            }
        }
        public void LoadObjectsPhongFromTiet(Object_Tiet objectTiet)
        {
            try
            {
                if (objectTiet == null)
                {
                    return;
                }

                if (objectTiet.Id_phong == 0)
                {
                    _ObjectPhong = null;
                }
                else
                {
                    _ObjectPhong = _dsObjectPhong.FirstOrDefault(c => c.Id_phong == objectTiet.Id_phong);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        public void LoadObjectsMonFromTiet(Object_Tiet objectTiet)
        {
            try
            {
                if (objectTiet == null)
                {
                    return;
                }

                if (objectTiet.Id_mon == 0)
                {
                    _ObjectMon = null;
                }
                else
                {
                    _ObjectMon = _dsObjectMon.FirstOrDefault(c => c.Id_mon == objectTiet.Id_mon);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public void LoadAllInformation(int idTkb, int idDonvi)
        {
            try
            {
                var ob_tiet = new List<Object_Tiet>();
                var ob_giaovien = new List<Object_Giaovien>();
                var gv_tietnghi = new List<(int Id_giao_vien, int Id_ca, int Ngay, int Tiet)>();
                var ob_mon = new List<Object_Monhoc>();
                var mh_tietnghi = new List<(int Id_mon, int Id_ca, int Ngay, int Tiet)>();
                var ob_phong = new List<Object_Phonghoc>();
                var ph_tietnghi = new List<(int Id_phong, int Id_ca, int Thu, int Tiet)>();
                var ob_thm = new List<Object_Tohopmon>();
                var ob_lop = new List<Object_Lophoc>();
                var lh_tietnghi = new List<(int Id_lop, int Id_ca, int Thu, int Tiet)>();
                var ob_lopmon = new List<Object_lop_mon>();
                var lopmon_tietnghi = new List<(int Id_lop, int Id_mon, int Id_ca, int Thu, int Tiet)>();
                var ob_monkhoi = new List<Object_MonKhoi>();
                var mk_tietnghi = new List<(int Id_mon, int Id_khoi, int Id_ban, int Id_ca, int Thu, int Tiet)>();
                var tietcodinh = new List<object_tiet_co_dinh>();
                var ob_ca = new List<Object_ca>();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "GetAll_Information";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Thêm tham số
                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@idDonvi";
                    p1.Value = idDonvi;
                    cmd.Parameters.Add(p1);

                    var p2 = cmd.CreateParameter();
                    p2.ParameterName = "@idTkb";
                    p2.Value = idTkb;
                    cmd.Parameters.Add(p2);

                    _context.Database.OpenConnection();

                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            ob_tiet.Add(new Object_Tiet
                            {
                                Id = reader.GetInt32(0),
                                Id_don_vi = reader.GetInt32(11),
                                Id_tkb = reader.GetInt32(1),
                                Id_ca = reader.GetInt32(2),
                                Id_giao_vien = reader.GetInt32(3),
                                Ten_giao_vien = !reader.IsDBNull(4) ? reader.GetString(4) : "",
                                Id_lop = reader.GetInt32(5),
                                Ten_lop = !reader.IsDBNull(6) ? reader.GetString(6) : "",
                                Id_mon = reader.GetInt32(7),
                                Ten_mon = !reader.IsDBNull(8) ? reader.GetString(8) : "",
                                Id_phong = reader.GetInt32(9),
                                Ten_phong = !reader.IsDBNull(10) ? reader.GetString(10) : "",
                                Tiet_thu_may = reader.GetInt32(12),
                                Ngay = reader.GetInt32(13),
                                Tiet = reader.GetInt32(14),
                                Khoa = reader.GetBoolean(15)
                            });
                        }

                        reader.NextResult();
                        while (reader.Read())
                        {
                            ob_giaovien.Add(new Object_Giaovien
                            {
                                Id_giao_vien = reader.GetInt32(0),
                                Chi_day_mot_buoi = reader.GetBoolean(1),
                                So_tiet_toi_da = reader.GetInt32(2),
                            });
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            gv_tietnghi.Add((
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetInt32(2),
                                reader.GetInt32(3)
                            ));
                        }
                        reader.NextResult();
                        while (reader.Read()) {
                            ob_mon.Add(new Object_Monhoc
                            {
                                Id_mon = reader.GetInt32(0),
                                Hoc_cach_ngay = reader.GetBoolean(1),
                                Xep_thanh_cap = reader.GetBoolean(2),
                                So_tiet_toi_da_mot_ca = reader.GetInt32(3),
                                So_tiet_toi_da_hai_ca = reader.GetInt32(4),
                            });
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            mh_tietnghi.Add((
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetInt32(2),
                                reader.GetInt32(3)
                            ));
                        }
                        reader.NextResult();
                        while (reader.Read()) {
                            ob_phong.Add(new Object_Phonghoc
                            {
                                Id_phong = reader.GetInt32(0),
                                Id_loai_phong = reader.GetInt32(1),
                                Khong_kiem_tra_xung_dot = reader.GetBoolean(2)
                            });
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            ph_tietnghi.Add((
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetInt32(2),
                                reader.GetInt32(3)
                            ));
                        }
                        reader.NextResult();
                        while (reader.Read()) {
                            ob_thm.Add(new Object_Tohopmon
                            {
                                Id_to_hop_mon = reader.GetInt32(0),
                                Id_mon_1 = reader.GetInt32(2),
                                Id_mon_2 = reader.GetInt32(3),
                                Id_mon_3 = reader.GetInt32(4),
                                So_tiet_toi_da_1_ca = reader.GetInt32(5),
                                So_tiet_toi_da_2_ca = reader.GetInt32(6),
                                Id_ban = reader.GetInt32(7),
                                Id_khoi = reader.GetInt32(8)
                            });
                        }
                        reader.NextResult();
                        while (reader.Read()) {
                            ob_lop.Add(new Object_Lophoc
                            {
                                Id_lop = reader.GetInt32(0),
                                Id_ban = reader.GetInt32(1),
                                Id_khoi = reader.GetInt32(2)
                            });
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            lh_tietnghi.Add((
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetInt32(2),
                                reader.GetInt32(3)
                            ));
                        }
                        reader.NextResult();
                        while (reader.Read()) {
                            ob_lopmon.Add(new Object_lop_mon
                            {
                                Id_lop = reader.GetInt32(0),
                                Id_mon = reader.GetInt32(1)
                            });
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            lopmon_tietnghi.Add((
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetInt32(2),
                                reader.GetInt32(3),
                                reader.GetInt32(4)
                            ));
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            ob_monkhoi.Add(new Object_MonKhoi
                            {
                                Id_mon = reader.GetInt32(0),
                                Id_khoi = reader.GetInt32(1),
                                Id_ban = reader.GetInt32(2)
                            });
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            mk_tietnghi.Add((
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetInt32(2),
                                reader.GetInt32(3),
                                reader.GetInt32(4),
                                reader.GetInt32(5)
                            ));
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            tietcodinh.Add(new object_tiet_co_dinh
                            {
                                Id_mon = reader.GetInt32(0),
                                Id_ca = reader.GetInt32(1),
                                Ngay = reader.GetInt32(2),
                                Tiet = reader.GetInt32(3),
                                Id_lop = reader.GetInt32(4)
                            });
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            ob_ca.Add(new Object_ca
                            {
                                Id_ca = reader.GetInt32(1),
                                So_tiet = reader.GetInt32(3)

                            });
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            _soNgay = reader.GetInt32(0);
                        }
                    }
                }

                for (int i = 0; i < ob_giaovien.Count; i++)
                {
                    var gv = ob_giaovien[i];
                    gv.ds_tiet_tranh_xep = gv_tietnghi
                        .Where(t => t.Id_giao_vien == gv.Id_giao_vien)
                        .Select(t => new Ds_tiet_tranh_xep
                        {
                            Id_ca = t.Id_ca,
                            Ngay = t.Ngay,
                            Tiet = t.Tiet
                        })
                        .ToList();
                }

                for (int i = 0; i < ob_giaovien.Count; i++)
                {
                    var gv = ob_giaovien[i];
                    gv.ds_tiet_phan_cong = ob_tiet
                        .Where(t => t.Id_giao_vien == gv.Id_giao_vien)
                        .Select(t => new Ds_tiet_phan_cong
                        {
                            Id_mon = t.Id_mon,
                            Ten_mon = t.Ten_mon,
                            Id_lop = t.Id_lop,
                            Ten_lop = t.Ten_lop,
                            Id_phong = t.Id_phong,
                            Ten_phong = t.Ten_phong,
                            Id_ca = t.Id_ca,
                            Tiet = t.Tiet,
                            Ngay = t.Ngay
                        })
                        .ToList();
                }

                for (int i = 0; i < ob_mon.Count; i++)
                {
                    var mh = ob_mon[i];
                    mh.ds_tiet_tranh_xep = mh_tietnghi
                        .Where(t => t.Id_mon == mh.Id_mon)
                        .Select(t => new Ds_tiet_tranh_xep
                        {
                            Id_ca = t.Id_ca,
                            Ngay = t.Ngay,
                            Tiet = t.Tiet
                        })
                        .ToList();
                }

                for (int i = 0; i < ob_phong.Count; i++)
                {
                    var ph = ob_phong[i];
                    ph.ds_tiet_tranh_xep = ph_tietnghi
                        .Where(t => t.Id_phong == ph.Id_phong)
                        .Select(t => new Ds_tiet_tranh_xep
                        {
                            Id_ca = t.Id_ca,
                            Ngay = t.Thu,
                            Tiet = t.Tiet
                        })
                        .ToList();
                }

                for (int i = 0; i < ob_lop.Count; i++)
                {
                    var lh = ob_lop[i];
                    lh.ds_tiet_tranh_xep = lh_tietnghi
                        .Where(t => t.Id_lop == lh.Id_lop)
                        .Select(t => new Ds_tiet_tranh_xep
                        {
                            Id_ca = t.Id_ca,
                            Ngay = t.Thu,
                            Tiet = t.Tiet
                        })
                        .ToList();
                }
                for (int i = 0; i < ob_lopmon.Count; i++)
                {
                    var lm = ob_lopmon[i];
                    lm.ds_tiet_tranh_xep_lop_mon = lopmon_tietnghi
                        .Where(t => t.Id_mon == lm.Id_mon && t.Id_lop == lm.Id_lop)
                        .Select(t => new Ds_tiet_tranh_xep
                        {
                            Id_ca = t.Id_ca,
                            Ngay = t.Thu,
                            Tiet = t.Tiet
                        })
                        .ToList();
                }
                for (int i = 0; i < ob_monkhoi.Count; i++)
                {
                    var mk = ob_monkhoi[i];
                    mk.ds_tiet_tranh_xep_mon_khoi = mk_tietnghi
                        .Where(t => t.Id_mon == mk.Id_mon && t.Id_ban == mk.Id_ban && t.Id_khoi == mk.Id_khoi)
                        .Select(t => new Ds_tiet_tranh_xep
                        {
                            Id_ca = t.Id_ca,
                            Ngay = t.Thu,
                            Tiet = t.Tiet
                        })
                        .ToList();
                }
                _dsTietGoc = ob_tiet;
                _dsObjectGiaovien = ob_giaovien;
                _dsObjectMon = ob_mon;
                _dsObjectPhong = ob_phong;
                _dsObjectLop = ob_lop;
                _dsObjectLopMon = ob_lopmon;
                _dsObjectMonKhoi = ob_monkhoi;
                _dsObjectTohopmon = ob_thm;
                _dsObjectTietcodinh = tietcodinh;
                _dsCa = ob_ca;

            }
            catch (Exception)
            {
                return;
            }
        }


        public bool Check_gv(int Ngay, int Tiet, int Ca, int? id_giaovien, int id_tkb)
        {
            var object_gv = _ObjectGiaovien;
            if (object_gv == null)
            {
                return false;
            }
            
            bool check_chi_day_mot_buoi = false;
            bool check_so_tiet_toi_da = false;
            var tiet_dau_trong_ngay = object_gv.ds_tiet_da_xep.FirstOrDefault(t => t.Ngay == Ngay && t.Id_giao_vien == id_giaovien);
            if (object_gv.Chi_day_mot_buoi == true && tiet_dau_trong_ngay != null)
            {

                if (Ca == tiet_dau_trong_ngay.Id_ca) { check_chi_day_mot_buoi = false; }
                else { check_chi_day_mot_buoi = true; }
            }
            var count_tiet_trong_ngay = object_gv.ds_tiet_da_xep.Where(t => t.Ngay == Ngay && t.Id_giao_vien == id_giaovien).Count();

            if (object_gv.So_tiet_toi_da > 0)
            {
                if (count_tiet_trong_ngay + 1 > object_gv.So_tiet_toi_da)
                {
                    check_so_tiet_toi_da = true;
                }
            }

            if ( check_so_tiet_toi_da || check_chi_day_mot_buoi) { return true; }

            return false;
        }

        public bool Check_to_hop_mon(int Ngay, int Tiet, int? iddonvi, int? idmon, int? idlop, int? idgv, int id_tkb)
        {
            var list_thm = _ObjectTohopmon;
            var ds_da_xep = _ObjectGiaovien.ds_tiet_da_xep;
            var check = false;

            for(int i=0; i< list_thm.Count; i++)
            {
                int so_tiet_da_xep_2_ca = 0;
                var ds_ca = _dsCa;

                for(int j=0; j<ds_ca.Count;j++)
                {
                    int so_tiet_da_xep_1_ca = 0;

                    // Kiểm tra từng môn trong tổ hợp
                    var dsMonTrongToHop = new List<int>();
                    if (list_thm[i].Id_mon_1 > 0) dsMonTrongToHop.Add(list_thm[i].Id_mon_1);
                    if (list_thm[i].Id_mon_2 > 0) dsMonTrongToHop.Add(list_thm[i].Id_mon_2);
                    if (list_thm[i].Id_mon_3 > 0) dsMonTrongToHop.Add(list_thm[i].Id_mon_3);

                    for(int k =0; k< dsMonTrongToHop.Count; k++)
                    {
                        int so_tiet_1_mon_1_ca = ds_da_xep.Where(c =>
                            c.Id_lop == idlop &&
                            c.Id_mon == dsMonTrongToHop[k] &&
                            c.Id_ca == ds_ca[j].Id_ca &&
                            c.Ngay == Ngay).Count();

                        so_tiet_da_xep_1_ca += so_tiet_1_mon_1_ca ;
                    }

                    if (so_tiet_da_xep_1_ca +1 > list_thm[i].So_tiet_toi_da_1_ca)
                    {
                        check = true;
                    }

                    so_tiet_da_xep_2_ca += so_tiet_da_xep_1_ca;
                }

                if (list_thm[i].So_tiet_toi_da_1_ca == list_thm[i].So_tiet_toi_da_2_ca)
                {
                    return check;
                }

                if (list_thm[i].So_tiet_toi_da_1_ca < list_thm[i].So_tiet_toi_da_2_ca)
                {
                    if (so_tiet_da_xep_2_ca +1 > list_thm[i].So_tiet_toi_da_2_ca)
                    {
                        check = true;
                    }
                    return check;
                }
            }
            return check;
        }
        public void TimViTriXepDuoc(Object_Tiet objectTiet, int idDonvi, List<Object_Tiet> ds_da_xep, List<Object_Tiet> ds_chua_xep)
        {
            try
            {
                objectTiet.Ds_vi_tri_xep_duoc.Clear();
                LoadObjectsFromTiet(objectTiet, idDonvi, ds_da_xep, ds_chua_xep);

                if (_ObjectMon == null || _ObjectGiaovien == null)
                    return;

                var tietban = DsTietTranhXep(objectTiet);
                var dsCa = _dsCa;
                for(int i = 0; i< dsCa.Count; i++)
                {
                    
                    for (int ngay = 1; ngay <= _soNgay; ngay++)
                    {
                        for (int tiet = 1; tiet <= dsCa[i].So_tiet; tiet++)
                        {
                            var slotKey = $"{ngay}_{dsCa[i].Id_ca}_{tiet}";

                            if (tietban.Contains(slotKey))
                                continue;

                            if (CheckDieuKienConLai(ngay, tiet, dsCa[i].Id_ca, objectTiet, ds_da_xep))
                            {
                                objectTiet.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
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
                objectTiet.Ds_vi_tri_xep_duoc.Clear();
            }
        }
        //public void TimViTriXepDuoc_byMon(Object_Tiet objectTiet, List<Object_Tiet> ds_da_xep, List<Object_Tiet> ds_chua_xep)
        //{
        //    try
        //    {
        //        objectTiet.Ds_vi_tri_xep_duoc.Clear();
        //        LoadObjectsMonFromTiet(objectTiet);

        //        if (_ObjectMon == null)
        //            return;
        //        var tietTranhXep = new HashSet<string>();
        //        AddTietTranhXep(tietTranhXep, _ObjectMon?.ds_tiet_tranh_xep) ;
        //        var dsCa = _dsCa;
        //        for (int i = 0; i < ds_da_xep.Count; i++)
        //        {
        //            // Duyệt trực tiếp và check luôn - chỉ 1 lần duyệt
        //            for (int ngay = 1; ngay <= 7; ngay++)
        //            {
        //                for (int tiet = 1; tiet <= 5; tiet++)
        //                {
        //                    var slotKey = $"{ngay}_{dsCa[i].Id_ca}_{tiet}";

        //                    if (tietTranhXep.Contains(slotKey))
        //                        continue;

        //                    if (CheckMonHoc(ngay, tiet, dsCa[i].Id_ca, objectTiet, ds_da_xep))
        //                    {
        //                        objectTiet.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
        //                        {
        //                            Ca = dsCa[i].Id_ca,
        //                            Ngay = ngay,
        //                            Tiet = tiet,
        //                        });
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objectTiet.Ds_vi_tri_xep_duoc.Clear();
        //    }
        //}

        private HashSet<string> DsTietTranhXep(Object_Tiet objectTiet)
        {
            var tietTranhXep = new HashSet<string>();

            AddTietTranhXep(tietTranhXep, _ObjectGiaovien?.ds_tiet_tranh_xep);
            AddTietTranhXep(tietTranhXep, _ObjectMon?.ds_tiet_tranh_xep);
            AddTietTranhXep(tietTranhXep, _ObjectLop?.ds_tiet_tranh_xep);
            AddTietTranhXep(tietTranhXep, _ObjectLopMon?.ds_tiet_tranh_xep_lop_mon);
            AddTietTranhXep(tietTranhXep, _ObjectMonKhoi?.ds_tiet_tranh_xep_mon_khoi);
            if (_ObjectPhong != null)
            {
                if (_ObjectPhong.ds_tiet_tranh_xep != null && _ObjectPhong.Khong_kiem_tra_xung_dot == false)
                {
                    AddTietTranhXep(tietTranhXep, _ObjectPhong.ds_tiet_tranh_xep);
                }
            }

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

        private bool CheckDieuKienConLai(int ngay, int tiet, int idCa, Object_Tiet objectTiet, List<Object_Tiet> ds_da_xep)
        {
            bool check_trung_phong = ds_da_xep.Any(t => t.Id_phong == objectTiet.Id_phong && t.Id_ca == idCa && t.Ngay == ngay && t.Tiet == tiet);
            bool check_trung_gv = ds_da_xep.Any(t => t.Id_giao_vien == objectTiet.Id_giao_vien && t.Id_ca == idCa && t.Ngay == ngay && t.Tiet == tiet);
            bool check_trung_lop = ds_da_xep.Any(t => t.Id_lop == objectTiet.Id_lop && t.Id_ca == idCa && t.Ngay == ngay && t.Tiet == tiet);
            if (Check_gv(ngay, tiet, idCa, objectTiet.Id_giao_vien, objectTiet.Id_tkb))
            {
                return false;
            }

            if (Check_to_hop_mon(ngay, tiet, objectTiet.Id_don_vi, objectTiet.Id_mon, objectTiet.Id_lop, objectTiet.Id_giao_vien, objectTiet.Id_tkb))
            {
                return false;
            }

            if (!CheckMonHoc(ngay, tiet, idCa, objectTiet, ds_da_xep))
            {
                return false;
            }
            if (check_trung_phong || check_trung_gv || check_trung_lop) { return false; }

            return true;
        }

        private bool CheckHocCachNgay(int ngay, Object_Tiet objectTiet)
        {
            try
            {
                if (_ObjectMon == null || _ObjectMon.Hoc_cach_ngay == false)
                    return true;

                var dsDataXep = _ObjectGiaovien?.ds_tiet_da_xep?.Where(x =>
                    x.Id_lop == objectTiet.Id_lop &&
                    x.Id_mon == objectTiet.Id_mon).ToList();

                if (dsDataXep == null || dsDataXep.Count == 0)
                    return true;
                var ngayHomTruoc = ngay - 1;
                bool coTietHomTruoc = dsDataXep.Any(x => x.Ngay == ngayHomTruoc);

                return !coTietHomTruoc;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool CheckMonHoc(int ngay, int tiet, int idCa, Object_Tiet objectTiet, List<Object_Tiet> ds_da_xep)
        {
            if (_ObjectMon == null) return true;

            var dsDataXep = _ObjectGiaovien?.ds_tiet_da_xep?.Where(x =>
                x.Id_lop == objectTiet.Id_lop &&
                x.Id_mon == objectTiet.Id_mon).ToList();

            if (!CheckHocCachNgay(ngay, objectTiet))
            {
                return false;
            }

            // Kiểm tra số tiết tối đa mỗi ca
            var check = true;
            int so_tiet_da_xep_2_ca = 0;
            var ds_ca = _dsCa;

            for(int i=0; i<ds_ca.Count;i++)
            {
                int so_tiet_da_xep_1_ca = 0;
                int so_tiet_1_mon_1_ca = dsDataXep?.Where(c => c.Id_ca == ds_ca[i].Id_ca && c.Ngay == ngay).Count() ?? 0;
                so_tiet_da_xep_1_ca += so_tiet_1_mon_1_ca + 1;

                if (so_tiet_da_xep_1_ca > _ObjectMon.So_tiet_toi_da_mot_ca)
                {
                    check = false;
                }

                so_tiet_da_xep_2_ca += so_tiet_da_xep_1_ca;
            }

            if (_ObjectMon.So_tiet_toi_da_mot_ca == _ObjectMon.So_tiet_toi_da_hai_ca)
            {
                return check;
            }

            if (_ObjectMon.So_tiet_toi_da_mot_ca < _ObjectMon.So_tiet_toi_da_hai_ca)
            {
                if (so_tiet_da_xep_2_ca > _ObjectMon.So_tiet_toi_da_hai_ca)
                {
                    check = false;
                }
                return check;
            }

            return check;
        }
        private bool CheckXepCap(Object_Tiet tiet)
        {
            try
            {
                LoadObjectsMonFromTiet(tiet);
                if (_ObjectMon == null || _ObjectMon.Xep_thanh_cap == false)
                {
                    return false;
                }
                if (_ObjectGiaovien?.ds_tiet_da_xep != null)
                {
                    var capDaXep = _ObjectGiaovien.ds_tiet_da_xep.Any(x =>
                        x.Id_mon == tiet.Id_mon &&
                        x.Id_lop == tiet.Id_lop &&
                        x.Id_phong == tiet.Id_phong &&
                        x.Id_ca == tiet.Id_ca);

                    if (capDaXep)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in NeedPairScheduling: {ex.Message}");
                return false;
            }
        }

        private bool TryXepCap(Object_Tiet tietCanXep, List<Object_Tiet> dsTietChuaXep, List<Object_Tiet> dsTietDaXep, List<Object_Tiet> dsTietBoqua)
        {
            LoadObjectsMonFromTiet(tietCanXep);
            // tìm tiết đã xếp
            var tietDaXep = dsTietDaXep.FirstOrDefault(t =>
                t.Id_mon == tietCanXep.Id_mon && t.Id_lop == tietCanXep.Id_lop &&
                t.Id_phong == tietCanXep.Id_phong && t.Id_ca == tietCanXep.Id_ca &&
                t.Id_giao_vien == tietCanXep.Id_giao_vien);

            if (tietDaXep != null)
            {
                // Tìm vị trí liền kề với tiết đã xếp
                var vtLienKe = tietCanXep.Ds_vi_tri_xep_duoc.FirstOrDefault(vt => vt.Ca == tietDaXep.Id_ca &&
                    vt.Ngay == tietDaXep.Ngay && (vt.Tiet == tietDaXep.Tiet - 1 || vt.Tiet == tietDaXep.Tiet + 1));

                if (vtLienKe != null && UpdateTiet(tietCanXep, vtLienKe.Ca, vtLienKe.Ngay, vtLienKe.Tiet))
                {
                    tietCanXep.Id_ca = vtLienKe.Ca;
                    tietCanXep.Ngay = vtLienKe.Ngay;
                    tietCanXep.Tiet = vtLienKe.Tiet;
                    dsTietChuaXep.Remove(tietCanXep);
                    dsTietDaXep.Add(tietCanXep);
                    return true;
                }
            }

            // tìm tiết chưa xếp để ghép cặp
            var tietGhepCap = dsTietChuaXep.FirstOrDefault(t =>
                t.Id_mon == tietCanXep.Id_mon && t.Id_lop == tietCanXep.Id_lop &&
                t.Id_phong == tietCanXep.Id_phong &&
                t.Id_giao_vien == tietCanXep.Id_giao_vien && t != tietCanXep);

            // nếu không có tiết để ghép thì cho tiết hiện tại vào ds bỏ qua
            if (tietGhepCap == null)
            {
                dsTietChuaXep.Remove(tietCanXep);
                dsTietBoqua.Add(tietCanXep);
                return true;
            }

            // thử ghép cặp
            foreach (var vt in tietCanXep.Ds_vi_tri_xep_duoc)
            {
                if (tietGhepCap.Ds_vi_tri_xep_duoc.Any(v => v.Ca == vt.Ca && v.Ngay == vt.Ngay && v.Tiet == vt.Tiet + 1))
                {
                    if (UpdateTiet(tietCanXep,vt.Ca, vt.Ngay, vt.Tiet) && UpdateTiet(tietGhepCap, vt.Ca, vt.Ngay, vt.Tiet + 1))
                    {
                        tietCanXep.Id_ca = vt.Ca; tietGhepCap.Id_ca = vt.Ca;
                        tietCanXep.Ngay = vt.Ngay; tietCanXep.Tiet = vt.Tiet;
                        tietGhepCap.Ngay = vt.Ngay; tietGhepCap.Tiet = vt.Tiet + 1;

                        dsTietChuaXep.Remove(tietCanXep);
                        dsTietChuaXep.Remove(tietGhepCap);
                        dsTietDaXep.Add(tietCanXep);
                        dsTietDaXep.Add(tietGhepCap);
                        return true;
                    }
                }
            }

            // không thành công thì update tiết đang xét như tiết lẻ
            return false;
        }
        private bool UpdateListTiet(List<Object_Tiet> ds_tiet)
        {
            try
            {
                if (ds_tiet != null && ds_tiet.Any())
                {
                    var chitietlist = ds_tiet.Select(tiet => new Chitiet_Thoikhoabieu
                    {
                        Id = tiet.Id,
                        Id_tkb = tiet.Id_tkb,
                        Id_lop = tiet.Id_lop ?? 0,
                        Id_mon = tiet.Id_mon ?? 0,
                        Id_giao_vien = tiet.Id_giao_vien ?? 0,
                        Id_phong = tiet.Id_phong ?? 0,
                        Id_ca = tiet.Id_ca,
                        Tiet_thu_may = tiet.Tiet_thu_may,
                        Ngay = tiet.Ngay,
                        Tiet = tiet.Tiet,
                        Khoa = tiet.Khoa
                    }).ToList();
                    var chitiet = chitietlist.Where(c => c.Id_ca > 0).ToList();
                    if (chitiet != null)
                        _context.BulkUpdate(chitiet);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating database: {ex.Message}");
                return false;
            }
        }
        private bool UpdateTiet(Object_Tiet tiet, int Id_ca, int ngay, int tietSo)
        {
            try
            {
                var record = _context.Chitiet_Thoikhoabieu
                    .FirstOrDefault(x => x.Id_tkb == tiet.Id_tkb &&
                                         x.Id_lop == tiet.Id_lop &&
                                         x.Id_mon == tiet.Id_mon &&
                                         x.Id_giao_vien == tiet.Id_giao_vien &&
                                         x.Id_phong == tiet.Id_phong &&
                                         x.Tiet_thu_may == tiet.Tiet_thu_may);

                if (record != null)
                {
                    record.Ngay = ngay;
                    record.Tiet = tietSo;
                    record.Id_ca = Id_ca;
                    _context.SaveChanges();
                    return true;
                }

                Console.WriteLine("Không tìm thấy record");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating database: {ex.Message}");
                return false;
            }
        }
        public bool ProcessThoiKhoaBieu(int idtkb, int idDonvi)
        {
            try
            {
                // 1. Load tất cả tiết cần xếp
                LoadAllInformation(idtkb, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return false;
                }
                var dsTietChuaXep = _dsTietGoc.Where(c => c.Ngay <= 0).ToList();
                var dsTietDaXep = _dsTietGoc.Where(c => c.Id_ca > 0 && c.Ngay > 0 && c.Tiet > 0).ToList();
                var dsTietBoqua = new List<Object_Tiet>();

                // 2. Xử lý tiết cố định
                if(dsTietDaXep == null || dsTietDaXep.Count == 0)
                {
                    var dsTietCoDinh = _dsObjectTietcodinh;
                    for (int i = 0; i < _dsTietGoc.Count; i++)
                    {
                        var tietCoDinh = dsTietCoDinh.FirstOrDefault(tcd => tcd.Id_mon == _dsTietGoc[i].Id_mon && tcd.Id_lop == _dsTietGoc[i].Id_lop && tcd.Id_ca == _dsTietGoc[i].Id_ca && _dsTietGoc[i].Tiet_thu_may == 1);

                        if (tietCoDinh != null)
                        {
                            LoadObjectsPhongFromTiet(_dsTietGoc[i]);
                            if (_ObjectPhong == null || _ObjectPhong.Id_loai_phong == 1)
                            {
                                _dsTietGoc[i].Id_ca = tietCoDinh.Id_ca;
                                _dsTietGoc[i].Ngay = tietCoDinh.Ngay;
                                _dsTietGoc[i].Tiet = tietCoDinh.Tiet;
                                dsTietChuaXep.Remove(_dsTietGoc[i]);
                                dsTietDaXep.Add(_dsTietGoc[i]
                                    );
                            }

                        }
                    }
                }
                
                //3. lặp đến khi ds chưa xếp = 0
                int vongLap = 0;
                while (dsTietChuaXep.Count > 0)
                {
                    vongLap++;
                    //b1: Tìm vị trí xếp được cho tất cả tiết chưa xếp
                    for (int i = 0; i < dsTietChuaXep.Count; i++)
                    {
                        TimViTriXepDuoc(dsTietChuaXep[i], idDonvi, dsTietDaXep, dsTietChuaXep);
                    }
                    // b2: Lọc các tiết có thể xếp được (vị trí > 0), nếu vị trí = 0 thì thêm vào ds bỏ qua
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
                    // b3: Sắp xếp theo thứ tự số vị trí xếp được và lấy tiết đầu tiên
                    var dsTietSorted = dsTietCoTheXep.OrderBy(t => t.Ds_vi_tri_xep_duoc.Count).ToList();
                    var tietCanXep = dsTietSorted.First();
                    //b4: check xếp thành cặp
                    if (CheckXepCap(tietCanXep))
                    {
                        if (TryXepCap(tietCanXep, dsTietChuaXep, dsTietDaXep, dsTietBoqua))
                        {
                            continue;
                        }
                    }
                    // b5: Update tiết này vào database (chọn vị trí đầu tiên có thể xếp)
                    var viTriChon = tietCanXep.Ds_vi_tri_xep_duoc.Where(vt => vt.Ngay >= 1 && vt.Ngay <= _soNgay).FirstOrDefault();

                    if (viTriChon == null)
                    {
                        dsTietBoqua.Add(tietCanXep);
                        dsTietChuaXep.Remove(tietCanXep);
                        continue;
                    }
                    tietCanXep.Id_ca = viTriChon.Ca;
                    tietCanXep.Ngay = viTriChon.Ngay;
                    tietCanXep.Tiet = viTriChon.Tiet;
                    dsTietChuaXep.Remove(tietCanXep);
                    dsTietDaXep.Add(tietCanXep);
                    if (vongLap > 1000)
                    {
                        Console.WriteLine("Dừng thuật toán sau 1000 vòng lặp để tránh lặp vô hạn");
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
        public bool Xeptkb_byMon(List<int> idmon, int idtkb, int idDonvi)
        {
            try
            {
                // 1. Load tất cả tiết cần xếp
                LoadAllInformation(idtkb, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return false;
                }
                var dsTietChuaXep = new List<Object_Tiet>();
                var dsTietDaXep = new List<Object_Tiet>();
                var dsTietBoqua = new List<Object_Tiet>();
                for ( int i = 0; i< idmon.Count; i++)
                {
                    int id = idmon[i];
                    dsTietChuaXep.AddRange(_dsTietGoc.Where(c => c.Id_mon == id && c.Ngay == 0 && c.Tiet == 0));
                    dsTietDaXep.AddRange(_dsTietGoc.Where(c => c.Id_mon == id && c.Ngay > 0 && c.Tiet > 0));
                }
                // 2. Xử lý tiết cố định
                if (dsTietDaXep == null || dsTietDaXep.Count == 0)
                {
                    var dsTietCoDinh = _dsObjectTietcodinh;
                    for (int i = 0; i < _dsTietGoc.Count; i++)
                    {
                        var tietCoDinh = dsTietCoDinh.FirstOrDefault(tcd => tcd.Id_mon == _dsTietGoc[i].Id_mon && tcd.Id_lop == _dsTietGoc[i].Id_lop && _dsTietGoc[i].Tiet_thu_may == 1);

                        if (tietCoDinh != null)
                        {
                            LoadObjectsPhongFromTiet(_dsTietGoc[i]);
                            if (_ObjectPhong == null || _ObjectPhong.Id_loai_phong == 1)
                            {
                                _dsTietGoc[i].Id_ca = tietCoDinh.Id_ca;
                                _dsTietGoc[i].Ngay = tietCoDinh.Ngay;
                                _dsTietGoc[i].Tiet = tietCoDinh.Tiet;
                                dsTietChuaXep.Remove(_dsTietGoc[i]);
                                dsTietDaXep.Add(_dsTietGoc[i]
                                    );
                            }

                        }
                    }
                }
                //3. lặp đến khi ds chưa xếp = 0
                int vongLap = 0;
                while (dsTietChuaXep.Count > 0)
                {
                    vongLap++;

                    //b1: Tìm vị trí xếp được cho tất cả tiết chưa xếp
                    for(int i=0; i<dsTietChuaXep.Count; i++)
                    {
                        TimViTriXepDuoc(dsTietChuaXep[i], idDonvi, dsTietDaXep, dsTietChuaXep);
                    }
                    // b2: Lọc các tiết có thể xếp được (vị trí > 0), nếu vị trí = 0 thì thêm vào ds bỏ qua
                    var dsTietCoTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count > 0).ToList();
                    var dsTietKhongTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count == 0).ToList();

                    if (dsTietKhongTheXep != null && dsTietKhongTheXep.Count > 0)
                    {
                        for(int i=0; i<dsTietKhongTheXep.Count;i++)
                        {
                            dsTietBoqua.Add(dsTietKhongTheXep[i]);
                            dsTietChuaXep.Remove(dsTietKhongTheXep[i]);
                        }
                    }
                    if (dsTietCoTheXep.Count == 0)
                    {
                        break;
                    }
                    // b3: Sắp xếp theo thứ tự số vị trí xếp được và lấy tiết đầu tiên
                    var dsTietSorted = dsTietCoTheXep.OrderBy(t => t.Ds_vi_tri_xep_duoc.Count).ToList();
                    var tietCanXep = dsTietSorted.First();
                    //b4: check xếp thành cặp
                    if (CheckXepCap(tietCanXep))
                    {
                        if (TryXepCap(tietCanXep, dsTietChuaXep, dsTietDaXep, dsTietBoqua))
                        {
                            continue;
                        }
                    }
                    // b5: Update tiết này vào database (chọn vị trí đầu tiên có thể xếp)
                    var viTriChon = tietCanXep.Ds_vi_tri_xep_duoc.First();
                    tietCanXep.Id_ca = viTriChon.Ca;
                    tietCanXep.Ngay = viTriChon.Ngay;
                    tietCanXep.Tiet = viTriChon.Tiet;
                    dsTietChuaXep.Remove(tietCanXep);
                    dsTietDaXep.Add(tietCanXep);
                    if (vongLap > 1000)
                    {
                        Console.WriteLine("Dừng thuật toán sau 1000 vòng lặp để tránh lặp vô hạn");
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
        public bool Xeptkb_byGiaovien(List<int> idgv, int idtkb, int idDonvi)
        {
            try
            {
                // 1. Load tất cả tiết cần xếp
                LoadAllInformation(idtkb, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return false;
                }
                var dsTietChuaXep = new List<Object_Tiet>();
                var dsTietDaXep = new List<Object_Tiet>();
                var dsTietBoqua = new List<Object_Tiet>();
                for ( int i = 0; i< idgv.Count; i++)
                {
                    int id = idgv[i];
                    dsTietChuaXep.AddRange(_dsTietGoc.Where(c => c.Id_giao_vien == id && c.Ngay == 0 && c.Tiet == 0));
                    dsTietDaXep.AddRange(_dsTietGoc.Where(c => c.Id_giao_vien == id && c.Ngay > 0 && c.Tiet > 0));
                }
                // 2. Xử lý tiết cố định
                if (dsTietDaXep == null || dsTietDaXep.Count == 0)
                {
                    var dsTietCoDinh = _dsObjectTietcodinh;
                    for (int i = 0; i < _dsTietGoc.Count; i++)
                    {
                        var tietCoDinh = dsTietCoDinh.FirstOrDefault(tcd => tcd.Id_mon == _dsTietGoc[i].Id_mon && tcd.Id_lop == _dsTietGoc[i].Id_lop && _dsTietGoc[i].Tiet_thu_may == 1);

                        if (tietCoDinh != null)
                        {
                            LoadObjectsPhongFromTiet(_dsTietGoc[i]);
                            if (_ObjectPhong == null || _ObjectPhong.Id_loai_phong == 1)
                            {
                                _dsTietGoc[i].Id_ca = tietCoDinh.Id_ca;
                                _dsTietGoc[i].Ngay = tietCoDinh.Ngay;
                                _dsTietGoc[i].Tiet = tietCoDinh.Tiet;
                                dsTietChuaXep.Remove(_dsTietGoc[i]);
                                dsTietDaXep.Add(_dsTietGoc[i]
                                    );
                            }

                        }
                    }
                }
                //3. lặp đến khi ds chưa xếp = 0
                int vongLap = 0;
                while (dsTietChuaXep.Count > 0)
                {
                    vongLap++;

                    //b1: Tìm vị trí xếp được cho tất cả tiết chưa xếp
                    for(int i=0; i<dsTietChuaXep.Count; i++)
                    {
                        TimViTriXepDuoc(dsTietChuaXep[i], idDonvi, dsTietDaXep, dsTietChuaXep);
                    }
                    // b2: Lọc các tiết có thể xếp được (vị trí > 0), nếu vị trí = 0 thì thêm vào ds bỏ qua
                    var dsTietCoTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count > 0).ToList();
                    var dsTietKhongTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count == 0).ToList();

                    if (dsTietKhongTheXep != null && dsTietKhongTheXep.Count > 0)
                    {
                        for(int i=0; i<dsTietKhongTheXep.Count;i++)
                        {
                            dsTietBoqua.Add(dsTietKhongTheXep[i]);
                            dsTietChuaXep.Remove(dsTietKhongTheXep[i]);
                        }
                    }
                    if (dsTietCoTheXep.Count == 0)
                    {
                        break;
                    }
                    // b3: Sắp xếp theo thứ tự số vị trí xếp được và lấy tiết đầu tiên
                    var dsTietSorted = dsTietCoTheXep.OrderBy(t => t.Ds_vi_tri_xep_duoc.Count).ToList();
                    var tietCanXep = dsTietSorted.First();
                    //b4: check xếp thành cặp
                    if (CheckXepCap(tietCanXep))
                    {
                        if (TryXepCap(tietCanXep, dsTietChuaXep, dsTietDaXep, dsTietBoqua))
                        {
                            continue;
                        }
                    }
                    // b5: Update tiết này vào database (chọn vị trí đầu tiên có thể xếp)
                    var viTriChon = tietCanXep.Ds_vi_tri_xep_duoc.First();
                    tietCanXep.Id_ca = viTriChon.Ca;
                    tietCanXep.Ngay = viTriChon.Ngay;
                    tietCanXep.Tiet = viTriChon.Tiet;
                    dsTietChuaXep.Remove(tietCanXep);
                    dsTietDaXep.Add(tietCanXep);
                    if (vongLap > 1000)
                    {
                        Console.WriteLine("Dừng thuật toán sau 1000 vòng lặp để tránh lặp vô hạn");
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
        public bool Xeptkb_byLop(List<int> idlop, int idtkb, int idDonvi)
        {
            try
            {
                // 1. Load tất cả tiết cần xếp
                LoadAllInformation(idtkb, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return false;
                }
                var dsTietChuaXep = new List<Object_Tiet>();
                var dsTietDaXep = new List<Object_Tiet>();
                var dsTietBoqua = new List<Object_Tiet>();
                for ( int i = 0; i< idlop.Count; i++)
                {
                    int id = idlop[i];
                    dsTietChuaXep.AddRange(_dsTietGoc.Where(c => c.Id_lop == id && c.Ngay == 0 && c.Tiet == 0));
                    dsTietDaXep.AddRange(_dsTietGoc.Where(c => c.Id_lop == id && c.Ngay > 0 && c.Tiet > 0));
                }
                // 2. Xử lý tiết cố định
                if (dsTietDaXep == null || dsTietDaXep.Count == 0)
                {
                    var dsTietCoDinh = _dsObjectTietcodinh;
                    for (int i = 0; i < _dsTietGoc.Count; i++)
                    {
                        var tietCoDinh = dsTietCoDinh.FirstOrDefault(tcd => tcd.Id_mon == _dsTietGoc[i].Id_mon && tcd.Id_lop == _dsTietGoc[i].Id_lop && _dsTietGoc[i].Tiet_thu_may == 1);

                        if (tietCoDinh != null)
                        {
                            LoadObjectsPhongFromTiet(_dsTietGoc[i]);
                            if (_ObjectPhong == null || _ObjectPhong.Id_loai_phong == 1)
                            {
                                _dsTietGoc[i].Id_ca = tietCoDinh.Id_ca;
                                _dsTietGoc[i].Ngay = tietCoDinh.Ngay;
                                _dsTietGoc[i].Tiet = tietCoDinh.Tiet;
                                dsTietChuaXep.Remove(_dsTietGoc[i]);
                                dsTietDaXep.Add(_dsTietGoc[i]
                                    );
                            }

                        }
                    }
                }
                //3. lặp đến khi ds chưa xếp = 0
                int vongLap = 0;
                while (dsTietChuaXep.Count > 0)
                {
                    vongLap++;

                    //b1: Tìm vị trí xếp được cho tất cả tiết chưa xếp
                    for(int i=0; i<dsTietChuaXep.Count; i++)
                    {
                        TimViTriXepDuoc(dsTietChuaXep[i], idDonvi, dsTietDaXep, dsTietChuaXep);
                    }
                    // b2: Lọc các tiết có thể xếp được (vị trí > 0), nếu vị trí = 0 thì thêm vào ds bỏ qua
                    var dsTietCoTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count > 0).ToList();
                    var dsTietKhongTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count == 0).ToList();

                    if (dsTietKhongTheXep != null && dsTietKhongTheXep.Count > 0)
                    {
                        for(int i=0; i<dsTietKhongTheXep.Count;i++)
                        {
                            dsTietBoqua.Add(dsTietKhongTheXep[i]);
                            dsTietChuaXep.Remove(dsTietKhongTheXep[i]);
                        }
                    }
                    if (dsTietCoTheXep.Count == 0)
                    {
                        break;
                    }
                    // b3: Sắp xếp theo thứ tự số vị trí xếp được và lấy tiết đầu tiên
                    var dsTietSorted = dsTietCoTheXep.OrderBy(t => t.Ds_vi_tri_xep_duoc.Count).ToList();
                    var tietCanXep = dsTietSorted.First();
                    //b4: check xếp thành cặp
                    if (CheckXepCap(tietCanXep))
                    {
                        if (TryXepCap(tietCanXep, dsTietChuaXep, dsTietDaXep, dsTietBoqua))
                        {
                            continue;
                        }
                    }
                    // b5: Update tiết này vào database (chọn vị trí đầu tiên có thể xếp)
                    var viTriChon = tietCanXep.Ds_vi_tri_xep_duoc.First();
                    tietCanXep.Id_ca = viTriChon.Ca;
                    tietCanXep.Ngay = viTriChon.Ngay;
                    tietCanXep.Tiet = viTriChon.Tiet;
                    dsTietChuaXep.Remove(tietCanXep);
                    dsTietDaXep.Add(tietCanXep);
                    if (vongLap > 1000)
                    {
                        Console.WriteLine("Dừng thuật toán sau 1000 vòng lặp để tránh lặp vô hạn");
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
        public bool Xeptkb_byPhong(List<int> idphong, int idtkb, int idDonvi)
        {
            try
            {
                // 1. Load tất cả tiết cần xếp
                LoadAllInformation(idtkb, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return false;
                }
                var dsTietChuaXep = new List<Object_Tiet>();
                var dsTietDaXep = new List<Object_Tiet>();
                var dsTietBoqua = new List<Object_Tiet>();
                for ( int i = 0; i< idphong.Count; i++)
                {
                    int id = idphong[i];
                    dsTietChuaXep.AddRange(_dsTietGoc.Where(c => c.Id_phong == id && c.Ngay == 0 && c.Tiet == 0));
                    dsTietDaXep.AddRange(_dsTietGoc.Where(c => c.Id_phong == id && c.Ngay > 0 && c.Tiet > 0));
                }
                // 2. Xử lý tiết cố định
                if (dsTietDaXep == null || dsTietDaXep.Count == 0)
                {
                    var dsTietCoDinh = _dsObjectTietcodinh;
                    for (int i = 0; i < _dsTietGoc.Count; i++)
                    {
                        var tietCoDinh = dsTietCoDinh.FirstOrDefault(tcd => tcd.Id_mon == _dsTietGoc[i].Id_mon && tcd.Id_lop == _dsTietGoc[i].Id_lop && _dsTietGoc[i].Tiet_thu_may == 1);

                        if (tietCoDinh != null)
                        {
                            LoadObjectsPhongFromTiet(_dsTietGoc[i]);
                            if (_ObjectPhong == null || _ObjectPhong.Id_loai_phong == 1)
                            {
                                _dsTietGoc[i].Id_ca = tietCoDinh.Id_ca;
                                _dsTietGoc[i].Ngay = tietCoDinh.Ngay;
                                _dsTietGoc[i].Tiet = tietCoDinh.Tiet;
                                dsTietChuaXep.Remove(_dsTietGoc[i]);
                                dsTietDaXep.Add(_dsTietGoc[i]
                                    );
                            }

                        }
                    }
                }
                //3. lặp đến khi ds chưa xếp = 0
                int vongLap = 0;
                while (dsTietChuaXep.Count > 0)
                {
                    vongLap++;

                    //b1: Tìm vị trí xếp được cho tất cả tiết chưa xếp
                    for(int i=0; i<dsTietChuaXep.Count; i++)
                    {
                        TimViTriXepDuoc(dsTietChuaXep[i], idDonvi, dsTietDaXep, dsTietChuaXep);
                    }
                    // b2: Lọc các tiết có thể xếp được (vị trí > 0), nếu vị trí = 0 thì thêm vào ds bỏ qua
                    var dsTietCoTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count > 0).ToList();
                    var dsTietKhongTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count == 0).ToList();

                    if (dsTietKhongTheXep != null && dsTietKhongTheXep.Count > 0)
                    {
                        for(int i=0; i<dsTietKhongTheXep.Count;i++)
                        {
                            dsTietBoqua.Add(dsTietKhongTheXep[i]);
                            dsTietChuaXep.Remove(dsTietKhongTheXep[i]);
                        }
                    }
                    if (dsTietCoTheXep.Count == 0)
                    {
                        break;
                    }
                    // b3: Sắp xếp theo thứ tự số vị trí xếp được và lấy tiết đầu tiên
                    var dsTietSorted = dsTietCoTheXep.OrderBy(t => t.Ds_vi_tri_xep_duoc.Count).ToList();
                    var tietCanXep = dsTietSorted.First();
                    //b4: check xếp thành cặp
                    if (CheckXepCap(tietCanXep))
                    {
                        if (TryXepCap(tietCanXep, dsTietChuaXep, dsTietDaXep, dsTietBoqua))
                        {
                            continue;
                        }
                    }
                    // b5: Update tiết này vào database (chọn vị trí đầu tiên có thể xếp)
                    var viTriChon = tietCanXep.Ds_vi_tri_xep_duoc.First();
                    tietCanXep.Id_ca = viTriChon.Ca;
                    tietCanXep.Ngay = viTriChon.Ngay;
                    tietCanXep.Tiet = viTriChon.Tiet;
                    dsTietChuaXep.Remove(tietCanXep);
                    dsTietDaXep.Add(tietCanXep);
                    if (vongLap > 1000)
                    {
                        Console.WriteLine("Dừng thuật toán sau 1000 vòng lặp để tránh lặp vô hạn");
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
        public bool Xeptkb_byPhongCN( int idtkb, int idDonvi)
        {
            try
            {
                // 1. Load tất cả tiết cần xếp
                LoadAllInformation(idtkb, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return false;
                }
                var dsTietChuaXep = new List<Object_Tiet>();
                var dsTietDaXep = new List<Object_Tiet>();
                var dsTietBoqua = new List<Object_Tiet>();
                List<int> idphong = _context.DM_Phonghoc.Where(c => c.Id_Loai_phong_hoc == 2).Select(c => c.Id).ToList();
                for ( int i = 0; i< idphong.Count; i++)
                {
                    int id = idphong[i];
                    dsTietChuaXep.AddRange(_dsTietGoc.Where(c => c.Id_phong == id && c.Ngay == 0 && c.Tiet == 0));
                    dsTietDaXep.AddRange(_dsTietGoc.Where(c => c.Id_phong == id && c.Ngay > 0 && c.Tiet > 0));
                }
                // 2. Xử lý tiết cố định
                if (dsTietDaXep == null || dsTietDaXep.Count == 0)
                {
                    var dsTietCoDinh = _dsObjectTietcodinh;
                    for (int i = 0; i < _dsTietGoc.Count; i++)
                    {
                        var tietCoDinh = dsTietCoDinh.FirstOrDefault(tcd => tcd.Id_mon == _dsTietGoc[i].Id_mon && tcd.Id_lop == _dsTietGoc[i].Id_lop && _dsTietGoc[i].Tiet_thu_may == 1);

                        if (tietCoDinh != null)
                        {
                            LoadObjectsPhongFromTiet(_dsTietGoc[i]);
                            if (_ObjectPhong == null || _ObjectPhong.Id_loai_phong == 1)
                            {
                                _dsTietGoc[i].Id_ca = tietCoDinh.Id_ca;
                                _dsTietGoc[i].Ngay = tietCoDinh.Ngay;
                                _dsTietGoc[i].Tiet = tietCoDinh.Tiet;
                                dsTietChuaXep.Remove(_dsTietGoc[i]);
                                dsTietDaXep.Add(_dsTietGoc[i]
                                    );
                            }

                        }
                    }
                }
                //3. lặp đến khi ds chưa xếp = 0
                int vongLap = 0;
                while (dsTietChuaXep.Count > 0)
                {
                    vongLap++;

                    //b1: Tìm vị trí xếp được cho tất cả tiết chưa xếp
                    for(int i=0; i<dsTietChuaXep.Count; i++)
                    {
                        TimViTriXepDuoc(dsTietChuaXep[i], idDonvi, dsTietDaXep, dsTietChuaXep);
                    }
                    // b2: Lọc các tiết có thể xếp được (vị trí > 0), nếu vị trí = 0 thì thêm vào ds bỏ qua
                    var dsTietCoTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count > 0).ToList();
                    var dsTietKhongTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count == 0).ToList();

                    if (dsTietKhongTheXep != null && dsTietKhongTheXep.Count > 0)
                    {
                        for(int i=0; i<dsTietKhongTheXep.Count;i++)
                        {
                            dsTietBoqua.Add(dsTietKhongTheXep[i]);
                            dsTietChuaXep.Remove(dsTietKhongTheXep[i]);
                        }
                    }
                    if (dsTietCoTheXep.Count == 0)
                    {
                        break;
                    }
                    // b3: Sắp xếp theo thứ tự số vị trí xếp được và lấy tiết đầu tiên
                    var dsTietSorted = dsTietCoTheXep.OrderBy(t => t.Ds_vi_tri_xep_duoc.Count).ToList();
                    var tietCanXep = dsTietSorted.First();
                    //b4: check xếp thành cặp
                    if (CheckXepCap(tietCanXep))
                    {
                        if (TryXepCap(tietCanXep, dsTietChuaXep, dsTietDaXep, dsTietBoqua))
                        {
                            continue;
                        }
                    }
                    // b5: Update tiết này vào database (chọn vị trí đầu tiên có thể xếp)
                    var viTriChon = tietCanXep.Ds_vi_tri_xep_duoc.First();
                    tietCanXep.Id_ca = viTriChon.Ca;
                    tietCanXep.Ngay = viTriChon.Ngay;
                    tietCanXep.Tiet = viTriChon.Tiet;
                    dsTietChuaXep.Remove(tietCanXep);
                    dsTietDaXep.Add(tietCanXep);
                    if (vongLap > 1000)
                    {
                        Console.WriteLine("Dừng thuật toán sau 1000 vòng lặp để tránh lặp vô hạn");
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
        public bool Xeptkb_byGVCN( int idtkb, int idDonvi)
        {
            try
            {
                // 1. Load tất cả tiết cần xếp
                LoadAllInformation(idtkb, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return false;
                }
                var dsTietChuaXep = new List<Object_Tiet>();
                var dsTietDaXep = new List<Object_Tiet>();
                var dsTietBoqua = new List<Object_Tiet>();
                var idgvcn = _context.DM_Lophoc.Where(c=>c.Id_don_vi == idDonvi).Select(c => c.Id_gvcn).ToList();
                var mon_do_gvcn_day = _context.Dm_Monhoc.Where(c => c.Do_GVCN_phu_trach == true && c.Id_don_vi == idDonvi).Select(c => c.Id).ToList();
                dsTietChuaXep = _dsTietGoc.Where(c =>idgvcn.Contains(c.Id_giao_vien??0) &&mon_do_gvcn_day.Contains(c.Id_mon??0) &&c.Ngay == 0 && c.Tiet == 0).ToList();
                dsTietDaXep = _dsTietGoc.Where(c =>idgvcn.Contains(c.Id_giao_vien??0) &&mon_do_gvcn_day.Contains(c.Id_mon??0) &&c.Ngay > 0 && c.Tiet > 0).ToList();

                // 2. Xử lý tiết cố định
                if (dsTietDaXep == null || dsTietDaXep.Count == 0)
                {
                    var dsTietCoDinh = _dsObjectTietcodinh;
                    for (int i = 0; i < _dsTietGoc.Count; i++)
                    {
                        var tietCoDinh = dsTietCoDinh.FirstOrDefault(tcd => tcd.Id_mon == _dsTietGoc[i].Id_mon && tcd.Id_lop == _dsTietGoc[i].Id_lop && _dsTietGoc[i].Tiet_thu_may == 1);

                        if (tietCoDinh != null)
                        {
                            LoadObjectsPhongFromTiet(_dsTietGoc[i]);
                            if (_ObjectPhong == null || _ObjectPhong.Id_loai_phong == 1)
                            {
                                _dsTietGoc[i].Id_ca = tietCoDinh.Id_ca;
                                _dsTietGoc[i].Ngay = tietCoDinh.Ngay;
                                _dsTietGoc[i].Tiet = tietCoDinh.Tiet;
                                dsTietChuaXep.Remove(_dsTietGoc[i]);
                                dsTietDaXep.Add(_dsTietGoc[i]
                                    );
                            }

                        }
                    }
                }
                //3. lặp đến khi ds chưa xếp = 0
                int vongLap = 0;
                while (dsTietChuaXep.Count > 0)
                {
                    vongLap++;

                    //b1: Tìm vị trí xếp được cho tất cả tiết chưa xếp
                    for(int i=0; i<dsTietChuaXep.Count; i++)
                    {
                        TimViTriXepDuoc(dsTietChuaXep[i], idDonvi, dsTietDaXep, dsTietChuaXep);
                    }
                    // b2: Lọc các tiết có thể xếp được (vị trí > 0), nếu vị trí = 0 thì thêm vào ds bỏ qua
                    var dsTietCoTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count > 0).ToList();
                    var dsTietKhongTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count == 0).ToList();

                    if (dsTietKhongTheXep != null && dsTietKhongTheXep.Count > 0)
                    {
                        for(int i=0; i<dsTietKhongTheXep.Count;i++)
                        {
                            dsTietBoqua.Add(dsTietKhongTheXep[i]);
                            dsTietChuaXep.Remove(dsTietKhongTheXep[i]);
                        }
                    }
                    if (dsTietCoTheXep.Count == 0)
                    {
                        break;
                    }
                    // b3: Sắp xếp theo thứ tự số vị trí xếp được và lấy tiết đầu tiên
                    var dsTietSorted = dsTietCoTheXep.OrderBy(t => t.Ds_vi_tri_xep_duoc.Count).ToList();
                    var tietCanXep = dsTietSorted.First();
                    //b4: check xếp thành cặp
                    if (CheckXepCap(tietCanXep))
                    {
                        if (TryXepCap(tietCanXep, dsTietChuaXep, dsTietDaXep, dsTietBoqua))
                        {
                            continue;
                        }
                    }
                    // b5: Update tiết này vào database (chọn vị trí đầu tiên có thể xếp)
                    var viTriChon = tietCanXep.Ds_vi_tri_xep_duoc.First();
                    tietCanXep.Id_ca = viTriChon.Ca;
                    tietCanXep.Ngay = viTriChon.Ngay;
                    tietCanXep.Tiet = viTriChon.Tiet;
                    dsTietChuaXep.Remove(tietCanXep);
                    dsTietDaXep.Add(tietCanXep);
                    if (vongLap > 1000)
                    {
                        Console.WriteLine("Dừng thuật toán sau 1000 vòng lặp để tránh lặp vô hạn");
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
        public bool Xeptkb_byLopMon(List<Sotiet_LopMonDto> dsLopMon, int idtkb, int idDonvi)
        {
            try
            {
                // 1. Load tất cả tiết cần xếp
                LoadAllInformation(idtkb, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return false;
                }
                var dsTietChuaXep = new List<Object_Tiet>();
                var dsTietDaXep = new List<Object_Tiet>();
                var dsTietBoqua = new List<Object_Tiet>();

                foreach (var lopDto in dsLopMon)
                {
                    int idLop = lopDto.Id_lop;
                    foreach (var mon in lopDto.ds_mon)
                    {
                        int idMon = mon.Id_mon;
                        dsTietChuaXep.AddRange(_dsTietGoc.Where(c => c.Id_lop == idLop && c.Id_mon == idMon && c.Ngay == 0 && c.Tiet == 0));
                        dsTietDaXep.AddRange(_dsTietGoc.Where(c => c.Id_lop == idLop && c.Id_mon == idMon && c.Ngay > 0 && c.Tiet > 0));
                    }
                }
                // 2. Xử lý tiết cố định
                if (dsTietDaXep == null || dsTietDaXep.Count == 0)
                {
                    var dsTietCoDinh = _dsObjectTietcodinh;
                    for (int i = 0; i < _dsTietGoc.Count; i++)
                    {
                        var tietCoDinh = dsTietCoDinh.FirstOrDefault(tcd => tcd.Id_mon == _dsTietGoc[i].Id_mon && tcd.Id_lop == _dsTietGoc[i].Id_lop && _dsTietGoc[i].Tiet_thu_may == 1);

                        if (tietCoDinh != null)
                        {
                            LoadObjectsPhongFromTiet(_dsTietGoc[i]);
                            if (_ObjectPhong == null || _ObjectPhong.Id_loai_phong == 1)
                            {
                                _dsTietGoc[i].Ngay = tietCoDinh.Ngay;
                                _dsTietGoc[i].Tiet = tietCoDinh.Tiet;
                                dsTietChuaXep.Remove(_dsTietGoc[i]);
                                dsTietDaXep.Add(_dsTietGoc[i]
                                    );
                            }

                        }
                    }
                }
                //3. lặp đến khi ds chưa xếp = 0
                int vongLap = 0;
                while (dsTietChuaXep.Count > 0)
                {
                    vongLap++;

                    //b1: Tìm vị trí xếp được cho tất cả tiết chưa xếp
                    for(int i=0; i<dsTietChuaXep.Count; i++)
                    {
                        TimViTriXepDuoc(dsTietChuaXep[i], idDonvi, dsTietDaXep, dsTietChuaXep);
                    }
                    // b2: Lọc các tiết có thể xếp được (vị trí > 0), nếu vị trí = 0 thì thêm vào ds bỏ qua
                    var dsTietCoTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count > 0).ToList();
                    var dsTietKhongTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count == 0).ToList();

                    if (dsTietKhongTheXep != null && dsTietKhongTheXep.Count > 0)
                    {
                        for(int i=0; i<dsTietKhongTheXep.Count;i++)
                        {
                            dsTietBoqua.Add(dsTietKhongTheXep[i]);
                            dsTietChuaXep.Remove(dsTietKhongTheXep[i]);
                        }
                    }
                    if (dsTietCoTheXep.Count == 0)
                    {
                        break;
                    }
                    // b3: Sắp xếp theo thứ tự số vị trí xếp được và lấy tiết đầu tiên
                    var dsTietSorted = dsTietCoTheXep.OrderBy(t => t.Ds_vi_tri_xep_duoc.Count).ToList();
                    var tietCanXep = dsTietSorted.First();
                    //b4: check xếp thành cặp
                    if (CheckXepCap(tietCanXep))
                    {
                        if (TryXepCap(tietCanXep, dsTietChuaXep, dsTietDaXep, dsTietBoqua))
                        {
                            continue;
                        }
                    }
                    // b5: Update tiết này vào database (chọn vị trí đầu tiên có thể xếp)
                    var viTriChon = tietCanXep.Ds_vi_tri_xep_duoc.First();
                    tietCanXep.Id_ca = viTriChon.Ca;
                    tietCanXep.Ngay = viTriChon.Ngay;
                    tietCanXep.Tiet = viTriChon.Tiet;
                    dsTietChuaXep.Remove(tietCanXep);
                    dsTietDaXep.Add(tietCanXep);
                    if (vongLap > 1000)
                    {
                        Console.WriteLine("Dừng thuật toán sau 1000 vòng lặp để tránh lặp vô hạn");
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

        public ObjectTiet_theoLopDto GetTkbByLop(int id_lop, int idtkb, int idDonvi)
        {
            LoadAllInformation(idtkb, idDonvi);
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
            var result = new ObjectTiet_theoLopDto
            {
                Id_lop = id_lop,
                Ten_lop = firstTiet.Ten_lop,
                timetable = new List<tkb_theo_lop>(),
                ds_chua_xep = new List<tkb_chuaxep_lop>()
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

                        var tietItem = new tkb_theo_lop
                        {
                            Id_chitiet = tietHoc?.Id ?? 0,
                            Id_don_vi = idDonvi,
                            Id_tkb = idtkb,
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
                            tietItem.isLock = tietHoc.Khoa;
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
                            tietItem.isLock = false;
                            tietItem.isDrag = false;
                            tietItem.isRest = isBreak;
                            result.timetable.Add(tietItem);
                        }
                    }
                }
            }

            var tietChuaXep = tiet.Where(t => t.Id_lop == id_lop && t.Id_tkb == idtkb && t.Ngay <= 0 && t.Tiet <= 0).GroupBy(t => new { t.Id_mon, t.Id_phong, t.Id_giao_vien })
                                  .Select(g => new {
                                      FirstItem = g.First(),
                                      SoTiet = g.Count()
                                  }).ToList();
            foreach (var tietcx in tietChuaXep)
            {
                var t = tietcx.FirstItem;
                var tietItem = new tkb_chuaxep_lop
                {
                    Id_chitiet = t.Id,
                    Id_don_vi = t.Id_don_vi ?? 0,
                    Id_tkb = t.Id_tkb,
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
        public ObjectTiet_theoGVDto GetTkbByGiaovien(int id_gv, int idtkb, int idDonvi)
        {
            LoadAllInformation(idtkb, idDonvi);

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
            var result = new ObjectTiet_theoGVDto
            {
                Id_giao_vien = id_gv,
                Ten_giao_vien = firstTiet.Ten_giao_vien,
                Tong_so_tiet = tiet.Count(),
                So_tiet_da_xep = tiet.Where(c=>c.Ngay > 0 && c.Tiet > 0).Count(),
                timetable = new List<tkb_theo_giaovien>(),
                ds_chua_xep = new List<tkb_chuaxep_giaovien>()
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

                        var tietItem = new tkb_theo_giaovien
                        {
                            Id_chitiet = tietHoc?.Id ?? 0,
                            Id_don_vi = idDonvi,
                            Id_tkb = idtkb,
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
                            tietItem.isLock = tietHoc.Khoa;
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
                            tietItem.isLock = false;
                            tietItem.isDrag = false;
                            tietItem.isRest = isBreak;
                            result.timetable.Add(tietItem);
                        }
                    }
                }
            }

            var tietChuaXep = tiet.Where(t => t.Id_giao_vien == id_gv && t.Id_tkb == idtkb && t.Ngay <= 0 && t.Tiet <= 0).GroupBy(t => new { t.Id_mon, t.Id_lop, t.Id_phong})
                                  .Select(g => new {
                                      FirstItem = g.First(),
                                      SoTiet = g.Count()
                                  }).ToList();
            foreach (var tietcx in tietChuaXep)
            {
                var t = tietcx.FirstItem;
                var tietItem = new tkb_chuaxep_giaovien
                {
                    Id_chitiet = t.Id,
                    Id_don_vi = t.Id_don_vi ?? 0,
                    Id_tkb = t.Id_tkb,
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
        public List<Object_TietChuaXep> GetTietChuaXep( int idtkb)
        {
            _dsTietGoc = List_Object_tiet(idtkb);
            if (_dsTietGoc == null || _dsTietGoc.Count == 0)
            {
                return null;
            }
            var result = new List<Object_TietChuaXep>();
            var tietChuaXep = _dsTietGoc.Where(t => t.Id_tkb == idtkb && t.Ngay <= 0 && t.Tiet <= 0).GroupBy(t => new { t.Id_mon, t.Id_lop, t.Id_giao_vien, t.Id_phong })
                      .Select(g => new {
                          FirstItem = g.First(),
                          SoTiet = g.Count()
                      }).ToList();
            foreach (var tietcx in tietChuaXep)
            {
                var t = tietcx.FirstItem;
                var tietItem = new Object_TietChuaXep
                {
                    Id = t.Id,
                    Id_don_vi = t.Id_don_vi ?? 0,
                    Id_tkb = t.Id_tkb,
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

        public void TimViTriXepDuoc_Lop(Object_Tiet objectTiet, int idDonvi)
        {
            try
            {
                objectTiet.Ds_vi_tri_xep_duoc.Clear();
                LoadObjectsFromTiet_TietBan(objectTiet, idDonvi);
                var ds_da_xep = _dsTietGoc.Where(c => c.Ngay > 0 && c.Tiet > 0).ToList();
                var tietban = DsTietTranhXep(objectTiet);
                var dsCa = _dsCa;
                var ds_tiet_da_xep_gv = ds_da_xep.Where(t => t.Id_giao_vien == objectTiet.Id_giao_vien).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                var ds_tiet_da_xep_phong = ds_da_xep.Where(t => t.Id_phong != objectTiet.Id_phong).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                for (int i=0; i < dsCa.Count; i++)
                {
                    for (int ngay = 1; ngay <= _soNgay; ngay++)
                    {
                        for (int tiet = 1; tiet <= dsCa[i].So_tiet; tiet++)
                        {
                            var slotKey = $"{ngay}_{dsCa[i].Id_ca}_{tiet}";

                            if (tietban.Contains(slotKey) || ds_tiet_da_xep_gv.Contains(slotKey) || ds_tiet_da_xep_phong.Contains(slotKey))
                                continue;
                            else
                            {
                                objectTiet.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
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
                objectTiet.Ds_vi_tri_xep_duoc.Clear();
            }
        }
        public Object_Tiet TimViTriXepDuoc_Lop_Tietdaxep(tkb_theo_lop tietdaxep, int idlop, int idDonvi)
        {
            try
            {
                Object_Tiet objectTiet = new Object_Tiet
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
                LoadObjectsFromTiet_TietBan(objectTiet, idDonvi);
                var ds_da_xep = _dsTietGoc.Where(c => c.Ngay > 0 && c.Tiet > 0).ToList();
                var tietban = DsTietTranhXep(objectTiet);
                var dsCa = _dsCa;
                var ds_tiet_da_xep_gv = ds_da_xep
                    .Where(t => t.Id_giao_vien == objectTiet.Id_giao_vien)
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
                                objectTiet.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
                                {
                                    Ca = dsCa[i].Id_ca,
                                    Ngay = ngay,
                                    Tiet = tiet,
                                });
                            }
                        }
                    }
                }
                return objectTiet;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void TimViTriXepDuoc_TietChuaXep(Object_Tiet objectTiet, int idDonvi)
        {
            try
            {
                objectTiet.Ds_vi_tri_xep_duoc.Clear();
                LoadObjectsFromTiet_TietBan(objectTiet, idDonvi);
                var ds_da_xep = _dsTietGoc.Where(c => c.Ngay > 0 && c.Tiet > 0).ToList();
                var tietban = DsTietTranhXep(objectTiet);
                var dsCa = _dsCa;
                var ds_tiet_da_xep_gv = ds_da_xep.Where(t => t.Id_giao_vien == objectTiet.Id_giao_vien).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                var ds_tiet_da_xep_phong = ds_da_xep.Where(t => t.Id_phong == objectTiet.Id_phong).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                var ds_tiet_da_xep_lop = ds_da_xep.Where(t => t.Id_lop == objectTiet.Id_lop).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
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
                                objectTiet.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
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
                objectTiet.Ds_vi_tri_xep_duoc.Clear();
            }
        }
        public bool CheckViTriXepDuoc_Lop(Object_Tiet objectTiet, int Ca, int Ngay, int Tiet, int idDonvi)
        {
            try
            {
                var tietban = DsTietTranhXep(objectTiet); 
                LoadObjectsFromTiet_TietBan(objectTiet, idDonvi);
                var ds_da_xep = _dsTietGoc.Where(c => c.Ngay > 0 && c.Tiet > 0).ToList();
                var ds_tiet_da_xep_gv = ds_da_xep.Where(t => t.Id_giao_vien == objectTiet.Id_giao_vien).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();

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
        public ObjectTiet_theoLopDto TimViTriXepDuoc_byLop(ObjectTiet_theoLopDto tietDachon, int idDonvi)
        {
            try
            {
                int idLop = tietDachon.Id_lop;
                var tiet = tietDachon.timetable?[0];
                if (tiet == null)
                {
                    return new ObjectTiet_theoLopDto();
                }

                int idChitiet = tiet.Id_chitiet;
                int idTkb = tiet.Id_tkb;
                int idCa = tiet.Id_ca;
                int ngay = tiet.Ngay;
                int tietSo = tiet.Tiet;
                int idMon = tiet.Id_mon;
                int idGiaoVien = tiet.Id_giao_vien;
                int idPhong = tiet.Id_phong;
                bool isRest = tiet.isRest;
                bool isLock = tiet.isLock;
                LoadAllInformation(idTkb, idDonvi);

                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return new ObjectTiet_theoLopDto();
                }
                var tkbBase = GetTkbByLop(idLop, idTkb, idDonvi);
                if (tkbBase == null)
                {
                    return new ObjectTiet_theoLopDto();
                }
                if (isLock || isRest || tiet.isError)
                {
                    return tkbBase;
                }

                var dsViTriXepDuoc = new List<(int Ca, int Ngay, int Tiet)>();

                // TH1: objectTiet_DaChon có đủ thông tin
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
                                dsViTriXepDuoc.Add((viTri.Ca,viTri.Ngay, viTri.Tiet));
                            }
                        }
                    }
                }
                // TH2: objectTiet_DaChon chỉ có thông tin cơ bản
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
                dsViTriXepDuoc.Add((idCa,ngay, tietSo));
                // Cập nhật isDrag cho các tiết trong tkbBase
                foreach (var tietdaxep in tkbBase.timetable)
                {
                    bool isDrag = false;
                    if (tietdaxep.Id_mon > 0)
                    {
                        var dsvitri_tietdaxep = new List<(int Ca, int Ngay, int Tiet)>();
                        var objectTiet = TimViTriXepDuoc_Lop_Tietdaxep(tietdaxep, idLop, idDonvi);

                        if (objectTiet?.Ds_vi_tri_xep_duoc != null)
                        {
                            foreach (var viTri in objectTiet.Ds_vi_tri_xep_duoc)
                            {
                                dsvitri_tietdaxep.Add((viTri.Ca, viTri.Ngay, viTri.Tiet));
                            }
                        }


                        if (!tietdaxep.isLock)
                        {
                            var vitri = (tietdaxep.Id_ca, tietdaxep.Ngay, tietdaxep.Tiet);

                            // Kiểm tra tiết gốc có trong ds vị trí xếp được của tiết đã xếp không
                            bool check_tietgoc = dsvitri_tietdaxep.Any(vt =>
                                vt.Ca == idCa &&
                                vt.Ngay == ngay &&
                                vt.Tiet == tietSo);

                            // Kiểm tra tiết đã xếp có trong dsViTriXepDuoc không
                            bool check_tietdaxep = dsViTriXepDuoc.Any(vt =>
                                vt.Ca == vitri.Id_ca &&
                                vt.Ngay == vitri.Ngay &&
                                vt.Tiet == vitri.Tiet);

                            isDrag = check_tietdaxep && check_tietgoc;
                        }
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
                return tkbBase;
            }
            catch (Exception)
            {
                return new ObjectTiet_theoLopDto();
            }
        }
        public ObjectTiet_theoLopDto TimViTriXepDuoc_TietChuaXep_byLop(ObjectTiet_theoLopDto tietDachon, int idDonvi)
        {
            try
            {
                int idLop = tietDachon.Id_lop;
                var tiet = tietDachon.ds_chua_xep?[0];
                if (tiet == null)
                {
                    return new ObjectTiet_theoLopDto();
                }

                int idChitiet = tiet.Id_chitiet;
                int idTkb = tiet.Id_tkb;

                LoadAllInformation(idTkb, idDonvi);

                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return new ObjectTiet_theoLopDto();
                }
                var tkbBase = GetTkbByLop(idLop, idTkb, idDonvi);
                if (tkbBase == null)
                {
                    return new ObjectTiet_theoLopDto();
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
                            dsViTriXepDuoc.Add((viTri.Ca,viTri.Ngay, viTri.Tiet));
                        }
                    }
                }
                // Cập nhật isDrag cho các tiết trong tkbBase
                foreach (var tietdaxep in tkbBase.timetable)
                {
                    bool isDrag =  dsViTriXepDuoc.Any(vt =>
                                          vt.Ca == tietdaxep.Id_ca &&
                                          vt.Ngay == tietdaxep.Ngay &&
                                          vt.Tiet == tietdaxep.Tiet);
                    tietdaxep.isDrag = isDrag;
                }
                return tkbBase;
            }
            catch (Exception)
            {
                return new ObjectTiet_theoLopDto();
            }
        }
        public ObjectTiet_theoGVDto TimViTriXepDuoc_TietChuaXep_byGV(ObjectTiet_theoGVDto tietDachon, int idDonvi)
        {
            try
            {
                int idgv = tietDachon.Id_giao_vien;
                var tiet = tietDachon.ds_chua_xep?[0];
                if (tiet == null)
                {
                    return new ObjectTiet_theoGVDto();
                }

                int idChitiet = tiet.Id_chitiet;
                int idTkb = tiet.Id_tkb;

                LoadAllInformation(idTkb, idDonvi);

                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return new ObjectTiet_theoGVDto();
                }
                var tkbBase = GetTkbByGiaovien(idgv, idTkb, idDonvi);
                if (tkbBase == null)
                {
                    return new ObjectTiet_theoGVDto();
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
                // Cập nhật isDrag cho các tiết trong tkbBase
                foreach (var tietdaxep in tkbBase.timetable)
                {
                    bool isDrag =  dsViTriXepDuoc.Any(vt =>
                                        vt.Ca == tietdaxep.Id_ca &&
                                        vt.Ngay == tietdaxep.Ngay &&
                                        vt.Tiet == tietdaxep.Tiet);

                    tietdaxep.isDrag = isDrag;
                }
                return tkbBase;
            }
            catch (Exception)
            {
                return new ObjectTiet_theoGVDto();
            }
        }
        public List<Object_Tiet> TimTietXepDuoc_byLop(ObjectTiet_theoLopDto tietDachon, int idDonvi)
        {
            try
            {
                int idLop = tietDachon.Id_lop;
                var tiet = tietDachon.timetable?[0];
                if (tiet == null)
                {
                    return new List<Object_Tiet>();
                }

                int idChitiet = tiet.Id_chitiet;
                int idTkb = tiet.Id_tkb;
                int idCa = tiet.Id_ca;
                int ngay = tiet.Ngay;
                int tietSo = tiet.Tiet;

                LoadAllInformation(idTkb, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return new List<Object_Tiet>();
                }

                var tkbBase = GetTkbByLop(idLop, idTkb, idDonvi);
                if (tkbBase == null)
                {
                    return new List<Object_Tiet>();
                }

                var dsViTriXepDuoc = new List<(int Ca,int Ngay, int Tiet)>();
                var tietChuaXep = new List<Object_Tiet>();

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
                            var tietMoi = new Object_Tiet
                            {
                                Id = tietGoc.Id,
                                Id_don_vi = tietGoc.Id_don_vi ?? 0,
                                Id_tkb = tietGoc.Id_tkb,
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
                return new List<Object_Tiet>();
            }
        }
        public void TimViTriXepDuoc_GV(Object_Tiet objectTiet, int idDonvi)
        {
            try
            {
                objectTiet.Ds_vi_tri_xep_duoc.Clear();
                var ds_da_xep = _dsTietGoc.Where(c => c.Ngay > 0 && c.Tiet > 0).ToList();
                LoadObjectsFromTiet_TietBan(objectTiet, idDonvi);
                var tietban = DsTietTranhXep(objectTiet);
                var dsCa = _dsCa;
                var ds_tiet_da_xep_phong = ds_da_xep.Where(t => t.Id_phong != objectTiet.Id_phong).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                for (int i = 0; i<dsCa.Count; i++)
                {
                    for (int ngay = 1; ngay <= _soNgay; ngay++)
                    {
                        for (int tiet = 1; tiet <= dsCa[i].So_tiet; tiet++)
                        {
                            var slotKey = $"{ngay}_{dsCa[i].Id_ca}_{tiet}";

                            if (tietban.Contains(slotKey) || ds_tiet_da_xep_phong.Contains(slotKey))
                                continue;
                            else
                            {
                                objectTiet.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
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
                objectTiet.Ds_vi_tri_xep_duoc.Clear();
            }
        }
        public Object_Tiet TimViTriXepDuoc_GV_Tietdaxep(tkb_theo_giaovien tietdaxep, int idgiaovien, int idDonvi)
        {
            try
            {
                Object_Tiet objectTiet = new Object_Tiet
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
                LoadObjectsFromTiet_TietBan(objectTiet, idDonvi);
                var ds_da_xep = _dsTietGoc.Where(c => c.Ngay > 0 && c.Tiet > 0).ToList();
                var tietban = DsTietTranhXep(objectTiet);
                var dsCa = _dsCa;
                var ds_tiet_da_xep_phong = ds_da_xep.Where(t => t.Id_phong == objectTiet.Id_phong).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                var ds_tiet_da_xep_lop = ds_da_xep.Where(t => t.Id_lop == objectTiet.Id_lop).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
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
                                objectTiet.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
                                {
                                    Ca = dsCa[i].Id_ca,
                                    Ngay = ngay,
                                    Tiet = tiet,
                                });
                            }
                        }
                    }
                }
                return objectTiet;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool CheckViTriXepDuoc_GV(Object_Tiet objectTiet, int Ca, int Ngay, int Tiet, int idDonvi)
        {
            try
            {
                LoadObjectsFromTiet_TietBan(objectTiet, idDonvi);
                var tietban = DsTietTranhXep(objectTiet);
                var ds_da_xep = _dsTietGoc.Where(c => c.Ngay > 0 && c.Tiet > 0).ToList();
                var ds_tiet_da_xep_phong = ds_da_xep.Where(t => t.Id_phong == objectTiet.Id_phong).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
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
        public ObjectTiet_theoGVDto TimViTriXepDuoc_byGV(ObjectTiet_theoGVDto tietDachon, int idDonvi)
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
                int idTkb = tiet.Id_tkb;
                int idCa = tiet.Id_ca;
                int ngay = tiet.Ngay;
                int tietSo = tiet.Tiet;
                int idMon = tiet.Id_mon ?? 0;
                int idlop = tiet.Id_lop ?? 0;
                int idPhong = tiet.Id_phong ?? 0;
                bool isRest = tiet.isRest;
                bool isLock = tiet.isLock;

                LoadAllInformation(idTkb, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return new ObjectTiet_theoGVDto();
                }

                var tkbBase = GetTkbByGiaovien(idGV, idTkb, idDonvi);
                if (tkbBase == null)
                {
                    return new ObjectTiet_theoGVDto();
                }
                if (isLock || isRest || tiet.isError)
                {
                    return tkbBase;
                }
                var dsViTriXepDuoc = new List<(int Ca,int Ngay, int Tiet)>();

                // TH1: objectTiet_DaChon có đủ thông tin
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
                // TH2: objectTiet_DaChon chỉ có thông tin cơ bản
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
                dsViTriXepDuoc.Add((idCa,ngay, tietSo));
                // Cập nhật isDrag cho các tiết trong tkbBase
                foreach (var tietdaxep in tkbBase.timetable)
                {
                    bool isDrag = false;
                    if (tietdaxep.Id_mon > 0)
                    {
                        var dsvitri_tietdaxep = new List<(int Ca, int Ngay, int Tiet)>();
                        var objectTiet = TimViTriXepDuoc_GV_Tietdaxep(tietdaxep, idGV, idDonvi);

                        if (objectTiet?.Ds_vi_tri_xep_duoc != null)
                        {
                            foreach (var viTri in objectTiet.Ds_vi_tri_xep_duoc)
                            {
                                dsvitri_tietdaxep.Add((viTri.Ca, viTri.Ngay, viTri.Tiet));
                            }
                        }

                        
                        if (!tietdaxep.isLock)
                        {
                            var vitri = (tietdaxep.Id_ca, tietdaxep.Ngay, tietdaxep.Tiet);

                            // Kiểm tra vị trí của tiết gốc có trong ds xếp được của tiết đang xét không
                            bool check_tietgoc = dsvitri_tietdaxep.Any(vt =>
                                vt.Ca == idCa &&
                                vt.Ngay == ngay &&
                                vt.Tiet == tietSo);

                            // Kiểm tra vị trí hiện tại có trong dsViTriXepDuoc không
                            bool check_tietdaxep = dsViTriXepDuoc.Any(vt =>
                                vt.Ca == vitri.Id_ca &&
                                vt.Ngay == vitri.Ngay &&
                                vt.Tiet == vitri.Tiet);

                            isDrag = check_tietdaxep && check_tietgoc;
                        }
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
                return tkbBase;
            }
            catch (Exception)
            {
                return new ObjectTiet_theoGVDto();
            }
        }
        public List<Object_Tiet> TimTietXepDuoc_byGV(ObjectTiet_theoGVDto tietDachon, int idDonvi)
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
                int idTkb = tiet.Id_tkb;
                int idCa = tiet.Id_ca;
                int ngay = tiet.Ngay;
                int tietSo = tiet.Tiet;
                int idMon = tiet.Id_mon ?? 0;
                int idlop = tiet.Id_lop ?? 0;
                int idPhong = tiet.Id_phong ?? 0;

                LoadAllInformation(idTkb, idDonvi);
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return new List<Object_Tiet>();
                }

                var tkbBase = GetTkbByGiaovien(idGV, idTkb, idDonvi);
                if (tkbBase == null)
                {
                    return new List<Object_Tiet>();
                }

                var dsViTriXepDuoc = new List<(int Ca, int Ngay, int Tiet)>();
                var tietChuaXep = new List<Object_Tiet>();

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
                            var tietMoi = new Object_Tiet
                            {
                                Id = tietGoc.Id,
                                Id_don_vi = tietGoc.Id_don_vi ?? 0,
                                Id_tkb = tietGoc.Id_tkb,
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
                return new List<Object_Tiet>();
            }
        }
        public (bool success, ObjectTiet_theoLopDto result) DoiChoHaiTiet_Lop(ObjectTiet_theoLopDto tietDachon, int idDonvi)
        {
            try
            {
                if (tietDachon?.timetable == null || tietDachon.timetable.Count < 2)
                {
                    return (false, new ObjectTiet_theoLopDto());
                }

                var tiet1 = tietDachon.timetable[0];
                var tiet2 = tietDachon.timetable[1];

                if (tiet1 == null || tiet2 == null)
                {
                    return (false, new ObjectTiet_theoLopDto());
                }

                int ngay1 = tiet1.Ngay;
                int tietSo1 = tiet1.Tiet;
                int ca1 = tiet1.Id_ca;
                int ngay2 = tiet2.Ngay;
                int tietSo2 = tiet2.Tiet;
                int ca2 = tiet2.Id_ca;
                bool lock1 = tiet1.isLock;
                bool lock2 = tiet2.isLock;
                // Tạo Object_Tiet từ tiết 1
                var objectTiet1 = new Object_Tiet
                {
                    Id_tkb = tiet1.Id_tkb,
                    Id_lop = tietDachon.Id_lop, 
                    Id_mon = tiet1.Id_mon,
                    Id_giao_vien = tiet1.Id_giao_vien,
                    Id_phong = tiet1.Id_phong,
                    Id_ca = tiet1.Id_ca,
                    Tiet_thu_may = tiet1.Tiet_thu_may,
                };

                // Tạo Object_Tiet từ tiết 2  
                var objectTiet2 = new Object_Tiet
                {
                    Id_tkb = tiet2.Id_tkb,
                    Id_lop = tietDachon.Id_lop,
                    Id_mon = tiet2.Id_mon,
                    Id_giao_vien = tiet2.Id_giao_vien,
                    Id_phong = tiet2.Id_phong,
                    Id_ca = tiet2.Id_ca,
                    Tiet_thu_may = tiet2.Tiet_thu_may
                };
                LoadAllInformation(objectTiet1.Id_tkb, idDonvi);
                bool check = true;
                bool updateTiet1 = false;
                bool updateTiet2 = false;
                if( objectTiet1.Id_mon == 0)
                {
                    check = CheckViTriXepDuoc_Lop(objectTiet2, ca1, ngay1, tietSo1, idDonvi);

                    if (check)
                    {
                        updateTiet2 = UpdateTiet(objectTiet2,ca1, ngay1, tietSo1);
                        if (!updateTiet2)
                        {
                            return (false, new ObjectTiet_theoLopDto());
                        }
                    }
                    else
                    {
                        return (false, new ObjectTiet_theoLopDto());
                    }
                }
                else if(objectTiet2.Id_mon == 0)
                {
                    check = CheckViTriXepDuoc_Lop(objectTiet1, ca2, ngay2, tietSo2, idDonvi);
                    if (check)
                    {
                        updateTiet1 = UpdateTiet(objectTiet1, ca2, ngay2, tietSo2);
                        if (!updateTiet1)
                        {
                            return (false, new ObjectTiet_theoLopDto());
                        }
                    }
                    else
                    {
                        return (false, new ObjectTiet_theoLopDto());
                    }
                }
                else
                {
                    var check_t1 = CheckViTriXepDuoc_Lop(objectTiet1, ca2, ngay2, tietSo2, idDonvi);
                    var check_t2 = CheckViTriXepDuoc_Lop(objectTiet2, ca1, ngay1, tietSo1, idDonvi);
                    if(check_t1 && check_t2 && !lock1 && !lock2)
                    {
                        updateTiet1 = UpdateTiet(objectTiet1, ca2, ngay2, tietSo2);
                        updateTiet2 = UpdateTiet(objectTiet2, ca1, ngay1, tietSo1);
                        if (!updateTiet1 || !updateTiet2)
                        {
                            return (false, new ObjectTiet_theoLopDto());
                        }
                    }
                    else
                    {
                        return (false, new ObjectTiet_theoLopDto());
                    }
                    
                }

                var ketQuaCheckViTri = TimViTriXepDuoc_byLop(tietDachon, idDonvi);
                return (true, ketQuaCheckViTri);
                
            }
            catch (Exception ex)
            {
                return (false, new ObjectTiet_theoLopDto());
            }
        }
        public (bool success, ObjectTiet_theoGVDto result) DoiChoHaiTiet_GV(ObjectTiet_theoGVDto tietDachon, int idDonvi)
        {
            try
            {
                if (tietDachon?.timetable == null || tietDachon.timetable.Count < 2)
                {
                    return (false, new ObjectTiet_theoGVDto());
                }

                var tiet1 = tietDachon.timetable[0];
                var tiet2 = tietDachon.timetable[1];

                if (tiet1 == null || tiet2 == null)
                {
                    return (false, new ObjectTiet_theoGVDto());
                }

                int ngay1 = tiet1.Ngay;
                int tietSo1 = tiet1.Tiet;
                int ca1 = tiet1.Id_ca;
                int ngay2 = tiet2.Ngay;
                int tietSo2 = tiet2.Tiet;
                int ca2 = tiet2.Id_ca;
                bool lock1 = tiet1.isLock;
                bool lock2 = tiet2.isLock;
                // Tạo Object_Tiet từ tiết 1
                var objectTiet1 = new Object_Tiet
                {
                    Id_tkb = tiet1.Id_tkb,
                    Id_lop = tiet1.Id_lop, 
                    Id_mon = tiet1.Id_mon,
                    Id_giao_vien = tietDachon.Id_giao_vien,
                    Id_phong = tiet1.Id_phong,
                    Id_ca = tiet1.Id_ca,
                    Tiet_thu_may = tiet1.Tiet_thu_may
                };

                // Tạo Object_Tiet từ tiết 2  
                var objectTiet2 = new Object_Tiet
                {
                    Id_tkb = tiet2.Id_tkb,
                    Id_lop = tiet2.Id_lop,
                    Id_mon = tiet2.Id_mon,
                    Id_giao_vien = tietDachon.Id_giao_vien,
                    Id_phong = tiet2.Id_phong,
                    Id_ca = tiet2.Id_ca,
                    Tiet_thu_may = tiet2.Tiet_thu_may
                };
                LoadAllInformation(objectTiet1.Id_tkb, idDonvi);
                bool check = true;
                bool updateTiet1 = false;
                bool updateTiet2 = false;
                if (objectTiet1.Id_mon == 0)
                {
                    check = CheckViTriXepDuoc_GV(objectTiet2, ca1, ngay1, tietSo1, idDonvi);
                    if (check)
                    {
                        updateTiet2 = UpdateTiet(objectTiet2, ca1, ngay1, tietSo1);
                        if (!updateTiet2)
                        {
                            return (false, new ObjectTiet_theoGVDto());
                        }
                    }
                    else
                    {
                        return (false, new ObjectTiet_theoGVDto());
                    }
                }
                else if (objectTiet2.Id_mon == 0)
                {
                    check = CheckViTriXepDuoc_GV(objectTiet1, ca2, ngay2, tietSo2, idDonvi);
                    if (check)
                    {
                        updateTiet1 = UpdateTiet(objectTiet1, ca2, ngay2, tietSo2);
                        if (!updateTiet1)
                        {
                            return (false, new ObjectTiet_theoGVDto());
                        }
                    }
                    else
                    {
                        return (false, new ObjectTiet_theoGVDto());
                    }
                }
                else
                {
                    var check_t1 = CheckViTriXepDuoc_GV(objectTiet1, ca2, ngay2, tietSo2, idDonvi);
                    var check_t2 = CheckViTriXepDuoc_GV(objectTiet2, ca1, ngay1, tietSo1, idDonvi);
                    if (check_t1 && check_t2 && !lock1&&!lock2)
                    {
                        updateTiet1 = UpdateTiet(objectTiet1, ca2, ngay2, tietSo2);
                        updateTiet2 = UpdateTiet(objectTiet2, ca1, ngay1, tietSo1);
                        if (!updateTiet1 || !updateTiet2)
                        {
                            return (false, new ObjectTiet_theoGVDto());
                        }
                    }
                    else
                    {
                        return (false, new ObjectTiet_theoGVDto());
                    }
                    
                }
                var ketQuaCheckViTri = TimViTriXepDuoc_byGV(tietDachon, idDonvi);
                return (true, ketQuaCheckViTri);
              
            }
            catch (Exception ex)
            {
                return (false, new ObjectTiet_theoGVDto());
            }
        }
        public bool UpdateTietChuaXep(Object_Tiet tietDachon, int idDonvi)
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
        public bool KhoaTiet_Mon(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return false;
                }
                var tiet = _context.Chitiet_Thoikhoabieu.FirstOrDefault(c => c.Id == id);
                var ds_tiet = _context.Chitiet_Thoikhoabieu.Where(c => c.Id_mon == tiet.Id_mon).ToList();
                if (ds_tiet != null && ds_tiet.Any())
                {
                    foreach (var item in ds_tiet)
                    {
                        item.Khoa = true;
                    }

                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public bool HuyKhoa_Mon(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return false;
                }
                var tiet = _context.Chitiet_Thoikhoabieu.FirstOrDefault(c => c.Id == id);
                var ds_tiet = _context.Chitiet_Thoikhoabieu.Where(c => c.Id_mon == tiet.Id_mon).ToList();
                if (ds_tiet != null && ds_tiet.Any())
                {
                    foreach (var item in ds_tiet)
                    {
                        item.Khoa = false;
                    }

                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public bool KhoaTiet_GV(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return false;
                }
                var tiet = _context.Chitiet_Thoikhoabieu.FirstOrDefault(c => c.Id == id);
                var ds_tiet = _context.Chitiet_Thoikhoabieu.Where(c => c.Id_giao_vien == tiet.Id_giao_vien).ToList();
                if (ds_tiet != null && ds_tiet.Any())
                {
                    foreach (var item in ds_tiet)
                    {
                        item.Khoa = true;
                    }

                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public bool HuyKhoa_GV(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return false;
                }
                var tiet = _context.Chitiet_Thoikhoabieu.FirstOrDefault(c => c.Id == id);
                var ds_tiet = _context.Chitiet_Thoikhoabieu.Where(c => c.Id_giao_vien == tiet.Id_giao_vien).ToList();
                if (ds_tiet != null && ds_tiet.Any())
                {
                    foreach (var item in ds_tiet)
                    {
                        item.Khoa = false;
                    }

                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public bool KhoaTiet(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return false;
                }
                var tiet = _context.Chitiet_Thoikhoabieu.FirstOrDefault(c => c.Id == id);
                if (tiet != null )
                {
                    tiet.Khoa = true;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public bool HuyKhoa(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return false;
                }
                var tiet = _context.Chitiet_Thoikhoabieu.FirstOrDefault(c => c.Id == id);
                if (tiet != null)
                {
                    tiet.Khoa = false;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch(Exception)
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
                var tiet = _context.Chitiet_Thoikhoabieu.FirstOrDefault(c => c.Id == id);
                if(tiet != null)
                {
                    tiet.Id_ca = 0;
                    tiet.Ngay = 0;
                    tiet.Tiet = 0;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public bool HuyTietNghiLop(int idLop)
        {
            try
            {
                var del = _context.Lophoc_Tietnghi.FirstOrDefault(c => c.Id_lop == idLop);
                if (del != null)
                {
                    _context.Lophoc_Tietnghi.Remove(del);
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
        public bool HuyTietNghiGV(int idgv)
        {
            try
            {
                var del = _context.Giaovien_Tiettranhxep.FirstOrDefault(c => c.Id_giao_vien == idgv);
                if (del != null)
                {
                    _context.Giaovien_Tiettranhxep.Remove(del);
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
 