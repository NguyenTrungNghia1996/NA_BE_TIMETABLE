using EFCore.BulkExtensions;
using Humanizer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
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
    public class ObjectRepository : IObjectRepository
    {
        private readonly NA_DbContext _context;
        private Object_Monhoc _ObjectMon;
        private Object_Lophoc _ObjectLop;
        private Object_Giaovien _ObjectGiaovien;
        private Object_Phonghoc _ObjectPhong;
        private Object_lop_mon _ObjectLopMon;
        private Object_MonKhoi _ObjectMonKhoi;
        private List<Object_Tohopmon> _ObjectTohopmon;
        public ObjectRepository(NA_DbContext context)
        {
            _context = context;
        }
        //object tiết
        public List<Object_Tiet> Object_tiet(int idtkb)
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
                        Id_don_vi = item.Id_don_vi,
                        Id_tkb = item.Id_tkb,
                        Id_lop = item.Id_lop,
                        Id_mon = item.Id_mon,
                        Id_giao_vien = item.Id_giao_vien,
                        Id_phong = item.Id_phong,
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
        //object giáo viên
        public Object_Giaovien Object_giaovien(int idgv, int idtkb)
        {
            try
            {
                var paramIdTkb = new SqlParameter("Id_tkb", SqlDbType.Int)
                {
                    Value = idtkb
                };
                var result = _context.Set<Chitiet_Thoikhoabieu_List>().FromSqlRaw("EXEC Get_Object @Id_tkb = @Id_tkb", paramIdTkb)
                    .ToList();
                var giaovien = _context.Giaovien_Buoiday.FirstOrDefault(c => c.Id == idgv);
                var tietban = _context.Giaovien_Tiettranhxep.Where(c => c.Id_giao_vien == idgv).ToList();
                if (result == null) return null;
                var teacher = new Object_Giaovien
                {
                    Id_don_vi = result[0].Id_don_vi,
                    Ten_don_vi = result[0].Ten_don_vi,
                    Id_giao_vien = result[0].Id_giao_vien,
                    Ten_giao_vien = result[0].Ten_giao_vien,
                    Chi_day_mot_buoi = giaovien.Chi_day_mot_buoi,
                    So_tiet_toi_da = giaovien.So_tiet_toi_da,
                    ds_tiet_phan_cong = new List<Ds_tiet_phan_cong>(),
                    ds_tiet_da_xep = new List<Ds_tiet_da_xep>(),
                    ds_tiet_chua_xep = new List<Ds_chua_xep>(),
                    ds_tiet_tranh_xep = new List<Ds_tiet_tranh_xep>()
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
                foreach (var item in tietban)
                {
                    teacher.ds_tiet_tranh_xep.Add(new Ds_tiet_tranh_xep
                    {

                        Id_ca = item.Id_ca,
                        Tiet = item.Tiet,
                        Ngay = item.Ngay
                    });
                }

                return teacher;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //object phòng học
        public Object_Phonghoc Object_phonghoc(int idph, int idtkb)
        {
            try
            {
                var paramIdphong = new SqlParameter("Id_phong", SqlDbType.Int)
                {
                    Value = idph
                };
                var paramIdTkb = new SqlParameter("Id_tkb", SqlDbType.Int)
                {
                    Value = idtkb
                };
                var result = _context.Set<Chitiet_Thoikhoabieu_List>().FromSqlRaw("EXEC Get_Object @Id_phong = @Id_phong, @Id_tkb = @Id_tkb",
                      paramIdphong, paramIdTkb)
                    .ToList();
                var phonghoc  = _context.DM_Phonghoc.FirstOrDefault(ph => ph.Id == idph); 
                var tietban = _context.Tiet_ban.Where(c => c.Id_phong == idph).ToList();
                if (result == null) return null;
                var room = new Object_Phonghoc
                {
                    Id_don_vi = result[0].Id_don_vi,
                    Ten_don_vi = result[0].Ten_don_vi,
                    Id_phong = result[0].Id_phong,  
                    Ten_phong = result[0].Ten_phong,
                    Khong_kiem_tra_xung_dot = phonghoc.Khong_kiem_tra_xung_dot,
                    ds_mon_tai_phong = new List<Ds_mon>(),
                    ds_tiet_tranh_xep = new List<Ds_tiet_tranh_xep>(),
                    ds_tiet_da_xep = new List<Ds_tiet_da_xep>()
                };
                room.ds_mon_tai_phong = result.Select(r => new Ds_mon
                                         {
                                             Id_mon = r.Id_mon,
                                             Ten_mon = r.Ten_mon
                                         }).GroupBy(m => m.Id_mon).Select(g => g.First()).ToList();
                foreach (var item in tietban)
                {
                    room.ds_tiet_tranh_xep.Add(new Ds_tiet_tranh_xep
                    {

                        Id_ca = item.Id_ca,
                        Tiet = item.Tiet,
                        Ngay = item.Thu
                    });
                }
                foreach (var r in result)
                {

                    if (r.Tiet > 0 && r.Ngay > 0)
                    {
                        room.ds_tiet_da_xep.Add(new Ds_tiet_da_xep
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
                return room;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //object môn học
        public Object_Monhoc Object_monhoc(int idmon, int idDonvi)
        {
            try
            {
                var monhoc = _context.Dm_Monhoc.Join(_context.DM_Donvi, mh => mh.Id_don_vi, dv => dv.Id,
                             (mh, dv) => new {   Id_mon = mh.Id, 
                                                 Ten_mon = mh.Ten, 
                                                 Id_don_vi = dv.Id, 
                                                 Ten_don_vi = dv.TenDonvi,
                                                 Hoc_cach_ngay = mh.Hoc_cach_ngay,
                                                 Xep_thanh_cap = mh.Xep_thanh_cap,
                                                 So_tiet_toi_da_1_ca = mh.So_tiet_toi_da_mot_ca,
                                                 So_tiet_toi_da_2_ca = mh.So_tiet_toi_da_hai_ca})
                             .FirstOrDefault(mh => mh.Id_mon == idmon && mh.Id_don_vi == idDonvi);
                var tietcodinh = _context.Tiet_co_dinh.Where(c => c.Id_mon == idmon).ToList();
                var tietban = _context.Tiet_Tranh_Xep.Where(c => c.Id_mon == idmon).ToList();

                var subject = new Object_Monhoc
                {
                    Id_don_vi = monhoc.Id_don_vi,
                    Ten_don_vi = monhoc.Ten_don_vi,
                    Id_mon = monhoc.Id_mon,
                    Ten_mon = monhoc.Ten_mon,
                    Hoc_cach_ngay = monhoc.Hoc_cach_ngay,
                    Xep_thanh_cap = monhoc.Xep_thanh_cap,
                    So_tiet_toi_da_mot_ca = monhoc.So_tiet_toi_da_1_ca,
                    So_tiet_toi_da_hai_ca = monhoc.So_tiet_toi_da_2_ca,
                    ds_tiet_co_dinh = new List<Ds_tiet_co_dinh>(),
                    ds_tiet_tranh_xep = new List<Ds_tiet_tranh_xep>()
                };
                foreach (var r in tietcodinh)
                {
                    subject.ds_tiet_co_dinh.Add(new Ds_tiet_co_dinh
                    {
                        Id_khoi = r.Id_khoi_lop,
                        Id_ca = r.Id_ca,
                        Ngay = r.Ngay,
                        Tiet = r.Tiet
                    });
                }
                foreach (var item in tietban)
                {
                    subject.ds_tiet_tranh_xep.Add(new Ds_tiet_tranh_xep
                    {

                        Id_ca = item.Id_ca,
                        Tiet = item.Tiet,
                        Ngay = item.Thu
                    });
                }

                return subject;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<Object_Tohopmon> Object_tohopmon(int idmon, int idlop, int idDonvi)
        {
            try
            {
                var lop = _context.DM_Lophoc.FirstOrDefault(c=>c.Id == idlop);
                int idkhoi = lop.Id_khoi;
                int idban = lop.Id_ban;
                var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int)
                {
                    Value = idDonvi 
                };
                var paramIdMon = new SqlParameter("id_mon", SqlDbType.Int)
                {
                    Value = idmon 
                };
                var paramIdkhoi = new SqlParameter("id_khoi", SqlDbType.Int)
                {
                    Value = idkhoi 
                };
                var paramIdban = new SqlParameter("id_ban", SqlDbType.Int)
                {
                    Value = idban 
                };
                var result = _context.Set<Monhoc_Tohopmon_List>().FromSqlRaw("EXEC MonTohop_GetList_Paging  @idDonvi = @idDonvi, @id_mon = @id_mon, @id_khoi = @id_khoi, @id_ban = @id_ban",
                                paramIdDonvi, paramIdMon, paramIdkhoi, paramIdban).ToList();
        
                if (result == null || result.Count == 0) 
                    return new List<Object_Tohopmon>();

                var listTohopmon = new List<Object_Tohopmon>();

                var groupedResult = result.GroupBy(r => r.Id);

                foreach (var group in groupedResult)
                {
                    var firstItem = group.First();
                    var tohopmon = new Object_Tohopmon
                    {
                        Id_don_vi = firstItem.Id_don_vi,
                        Id_to_hop_mon = firstItem.Id,
                        ds_mon = new List<Ds_mon>(),
                        So_tiet_toi_da_1_ca = firstItem.So_tiet_toi_da_1_ca,
                        So_tiet_toi_da_2_ca = firstItem.So_tiet_toi_da_2_ca
                    };

                    foreach (var r in group)
                    {
                        if (r.Id_mon_1 > 0 && !string.IsNullOrEmpty(r.Ten_mon_hoc_1))
                        {
                            if (!tohopmon.ds_mon.Any(m => m.Id_mon == r.Id_mon_1))
                            {
                                tohopmon.ds_mon.Add(new Ds_mon
                                {
                                    Id_mon = r.Id_mon_1,
                                    Ten_mon = r.Ten_mon_hoc_1
                                });
                            }
                        }

                        if (r.Id_mon_2 > 0 && !string.IsNullOrEmpty(r.Ten_mon_hoc_2))
                        {
                            if (!tohopmon.ds_mon.Any(m => m.Id_mon == r.Id_mon_2))
                            {
                                tohopmon.ds_mon.Add(new Ds_mon
                                {
                                    Id_mon = r.Id_mon_2,
                                    Ten_mon = r.Ten_mon_hoc_2
                                });
                            }
                        }

                        if (r.Id_mon_3 > 0 && !string.IsNullOrEmpty(r.Ten_mon_hoc_3))
                        {
                            if (!tohopmon.ds_mon.Any(m => m.Id_mon == r.Id_mon_3))
                            {
                                tohopmon.ds_mon.Add(new Ds_mon
                                {
                                    Id_mon = r.Id_mon_3,
                                    Ten_mon = r.Ten_mon_hoc_3
                                });
                            }
                        }
                    }

                    listTohopmon.Add(tohopmon);
                }

                return listTohopmon;
            }
            catch (Exception)
            {
                return new List<Object_Tohopmon>();
            }
        }
        //object lớp học
        public Object_Lophoc Object_lophoc(int idlop, int idtkb)
        {
            try
            {
                var paramIdLop = new SqlParameter("Id_lop", SqlDbType.Int)
                {
                    Value = idlop
                };
                var paramIdTkb = new SqlParameter("Id_tkb", SqlDbType.Int)
                {
                    Value = idtkb
                };


                var result = _context.Set<Chitiet_Thoikhoabieu_List>().FromSqlRaw("EXEC Get_Object @Id_lop = @Id_lop, @Id_tkb = @Id_tkb",
                      paramIdLop, paramIdTkb)
                    .ToList();
                var tietban = _context.Lophoc_Tietnghi.Where(c => c.Id_lop == idlop).ToList();
                var tietban_lopmon = _context.Lophoc_Monhoc_Tiettranhxep.Where(c => c.Id_lop == idlop).ToList();
                if (result == null) return null;
                var lop = new Object_Lophoc
                {
                    Id_don_vi = result[0].Id_don_vi,
                    Ten_don_vi = result[0].Ten_don_vi,
                    Id_lop = result[0].Id_lop,
                    Ten_lop = result[0].Ten_lop,
                    ds_lop_mon = new List<Ds_tiet_phan_cong>(),
                    ds_tiet_tranh_xep = new List<Ds_tiet_tranh_xep>(),
                    //ds_tiet_tranh_xep_lop_mon = new List<Ds_tiet_tranh_xep_lop_mon>()
                };
                foreach (var r in result)
                {
                    lop.ds_lop_mon.Add(new Ds_tiet_phan_cong
                    {
                        Id_mon = r.Id_mon,
                        Ten_mon = r.Ten_mon,
                        Id_lop = r.Id_lop,
                        Ten_lop = r.Ten_lop,
                        Id_phong = r.Id_phong,
                        Ten_phong = r.Ten_phong
                    });
                }
                foreach (var item in tietban)
                {
                    lop.ds_tiet_tranh_xep.Add(new Ds_tiet_tranh_xep
                    {

                        Id_ca = item.Id_ca,
                        Tiet = item.Tiet,
                        Ngay = item.Ngay
                    });
                }
                //lop.ds_tiet_tranh_xep_lop_mon = tietban_lopmon.GroupBy(x => x.Id_mon).Select(g => new Ds_tiet_tranh_xep_lop_mon
                //                                {
                //                                    Id_mon = g.Key,
                //                                    Ds_tiet_tranh_xep_monlop = g.Select(item => new Ds_tiet_tranh_xep
                //                                    {
                //                                        Id_ca = item.Id_ca,
                //                                        Tiet = item.Tiet,
                //                                        Ngay = item.Ngay
                //                                    }).ToList()
                //                                }).ToList();

                return lop;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //object môn khối
        public Object_MonKhoi Object_monkhoi(int idmon, int idlop, int idDonvi)
        {
            try
            {
                var monhoc = _context.Dm_Monhoc.Join(_context.DM_Donvi, mh => mh.Id_don_vi, dv => dv.Id,
                             (mh, dv) => new { Id_mon = mh.Id, Ten_mon = mh.Ten, Id_don_vi = dv.Id, Ten_don_vi = dv.TenDonvi })
                             .FirstOrDefault(mh => mh.Id_mon == idmon && mh.Id_don_vi == idDonvi);
                var khoi = _context.DM_Lophoc.Join(_context.DM_Khoilop, lh => lh.Id_khoi, kl => kl.Id,
                            (lh, kl) => new { Id_khoi = kl.Id, Ten_khoi = kl.Ten, Id_lop = lh.Id, Id_ban = lh.Id_ban }).FirstOrDefault(x => x.Id_lop == idlop);
                var tietban = _context.Monhoc_Khoilop_Tiettranhxep.Where(c => c.Id_mon == idmon && c.Id_ban == khoi.Id_ban && c.Id_khoi == khoi.Id_khoi).ToList();

                var monkhoi = new Object_MonKhoi
                {
                    Id_don_vi = monhoc.Id_don_vi,
                    Ten_don_vi = monhoc.Ten_don_vi,
                    Id_mon = monhoc.Id_mon,
                    Ten_mon = monhoc.Ten_mon,
                    Id_khoi = khoi.Id_khoi,
                    Ten_khoi = khoi.Ten_khoi,
                    ds_tiet_tranh_xep_mon_khoi = new List<Ds_tiet_tranh_xep_mon_khoi>()
                };
                
                foreach (var item in tietban)
                {
                    monkhoi.ds_tiet_tranh_xep_mon_khoi.Add(new Ds_tiet_tranh_xep_mon_khoi
                    {

                        Id_ca = item.Id_ca,
                        Tiet = item.Tiet,
                        Ngay = item.Ngay
                    });
                }

                return monkhoi;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Object_lop_mon Object_lopmon(int idmon, int idlop, int idDonvi)
        {
            try
            {
                var monhoc = _context.Dm_Monhoc.Join(_context.DM_Donvi, mh => mh.Id_don_vi, dv => dv.Id,
                             (mh, dv) => new { Id_mon = mh.Id, Ten_mon = mh.Ten, Id_don_vi = dv.Id, Ten_don_vi = dv.TenDonvi })
                             .FirstOrDefault(mh => mh.Id_mon == idmon && mh.Id_don_vi == idDonvi);
                var lop = _context.DM_Lophoc.FirstOrDefault(x => x.Id == idlop);
                var tietban = _context.Lophoc_Monhoc_Tiettranhxep.Where(c => c.Id_mon == idmon && c.Id_lop == idlop).ToList();

                var lopmon = new Object_lop_mon
                {
                    Id_don_vi = monhoc.Id_don_vi,
                    Ten_don_vi = monhoc.Ten_don_vi,
                    Id_mon = monhoc.Id_mon,
                    Ten_mon = monhoc.Ten_mon,
                    Id_lop = lop.Id,
                    Ten_lop = lop.Ten,
                    ds_tiet_tranh_xep_lop_mon = new List<Ds_tiet_tranh_xep>()
                };
                
                foreach (var item in tietban)
                {
                    lopmon.ds_tiet_tranh_xep_lop_mon.Add(new Ds_tiet_tranh_xep
                    {
                        Id_ca = item.Id_ca,
                        Tiet = item.Tiet,
                        Ngay = item.Ngay
                    });
                }

                return lopmon;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void LoadObjectsFromTiet(Object_Tiet objectTiet)
        {
            try
            {
                if (objectTiet == null)
                {
                    return;
                }

                var donvi = _context.DM_Lophoc
                    .Join(_context.DM_Donvi, lh => lh.Id_don_vi, dv => dv.Id,
                          (lh, dv) => new { Id_don_vi = dv.Id, Id_lop = lh.Id })
                    .FirstOrDefault(x => x.Id_lop == objectTiet.Id_lop);

                if (donvi == null)
                {
                    return;
                }

                _ObjectMon = Object_monhoc(objectTiet.Id_mon, donvi.Id_don_vi);
                _ObjectLop = Object_lophoc(objectTiet.Id_lop, objectTiet.Id_tkb);
                _ObjectGiaovien = Object_giaovien(objectTiet.Id_giao_vien, objectTiet.Id_tkb);
                _ObjectPhong = Object_phonghoc(objectTiet.Id_phong, objectTiet.Id_tkb);
                _ObjectLopMon = Object_lopmon(objectTiet.Id_mon, objectTiet.Id_lop, objectTiet.Id_tkb);
                _ObjectMonKhoi = Object_monkhoi(objectTiet.Id_mon, objectTiet.Id_lop, objectTiet.Id_don_vi);
                _ObjectTohopmon = Object_tohopmon(objectTiet.Id_mon, objectTiet.Id_lop, objectTiet.Id_don_vi);
            }
            catch (Exception ex)
            {
                return;
            }
        }
        public bool Check_gv(int Ngay, int Tiet, int Ca, int id_giaovien, int idphong, int id_tkb)
        {
            var object_gv = Object_giaovien(id_giaovien, id_tkb);
            var object_phong = Object_phonghoc(idphong, id_tkb);
            if (object_gv == null)
            {
                return false;
            }
            bool check_trung_giao_vien = object_gv.ds_tiet_da_xep.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);
            bool check_trung_phong = object_phong.ds_tiet_da_xep.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);
            bool check_tietban = object_gv.ds_tiet_tranh_xep.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);
            bool check_chi_day_mot_buoi = false;
            bool check_so_tiet_toi_da = false;
            var tiet_dau_trong_ngay = object_gv.ds_tiet_da_xep.FirstOrDefault(t => t.Ngay == Ngay);
            if (object_gv.Chi_day_mot_buoi == true && tiet_dau_trong_ngay != null)
            {
                
                if (Ca == tiet_dau_trong_ngay.Id_ca) { check_chi_day_mot_buoi = false; }
                else { check_chi_day_mot_buoi = true; }
            }
            var count_tiet_trong_ngay = object_gv.ds_tiet_da_xep.Where(t => t.Ngay == Ngay).Count();
            
            if (count_tiet_trong_ngay > object_gv.So_tiet_toi_da)
            {
                check_so_tiet_toi_da = true;
            }
            if (check_tietban || check_trung_giao_vien || check_trung_phong || check_so_tiet_toi_da || check_chi_day_mot_buoi) { return true; }    

            return false;
        }
        public bool Check_phong(int Ngay, int Tiet, int Ca, int idphong, int id_tkb)
        {
            var object_phong = Object_phonghoc(idphong, id_tkb);
            if (object_phong == null)
            {
                return false;
            }

            bool check_tietban = false;
            if(object_phong.Khong_kiem_tra_xung_dot == true)
            {
                check_tietban = object_phong.ds_tiet_tranh_xep.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);
            }

            return check_tietban;
        }
        public bool Check_mon(int Ngay, int Tiet, int Ca, int idmon, int id_tkb)
        {
            var object_mon = Object_monhoc(idmon, id_tkb);
            if (object_mon == null)
            {
                return false;
            }

            bool check_tietban = object_mon.ds_tiet_tranh_xep.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);

            return check_tietban;
        }
        public List<object_tiet_co_dinh> GetTietCoDinh(int id_don_vi)
        {
            var result = (from tcd in _context.Tiet_co_dinh
                          join kl in _context.DM_Khoilop on tcd.Id_khoi_lop equals kl.Id
                          join lh in _context.DM_Lophoc on kl.Id equals lh.Id_khoi
                          join lhm in _context.Lophoc_Monhoc on new { lop = lh.Id, mon = tcd.Id_mon } equals new { lop = lhm.Id_lop, mon = lhm.Id_mon }
                          join mh in _context.Dm_Monhoc on tcd.Id_mon equals mh.Id
                          where mh.Id_don_vi == id_don_vi && lh.Id_don_vi == id_don_vi
                          select new object_tiet_co_dinh
                          {
                              Id_mon = tcd.Id_mon,
                              Id_ca = tcd.Id_ca,
                              Id_lop = lh.Id,
                              Ngay = tcd.Ngay,
                              Tiet = tcd.Tiet
                          }).ToList();

            return result;
        }


        public bool Check_to_hop_mon(int Ngay, int Tiet, int iddonvi, int idmon,int idlop, int idgv,  int id_tkb)
        {
            var list_thm = Object_tohopmon(idmon,idlop, id_tkb);
            var ds_da_xep = Object_giaovien(idgv, id_tkb).ds_tiet_da_xep;
            var check = false;
            foreach (var ob_thm in list_thm)
            {
                var dsmon = ob_thm.ds_mon;
                int so_tiet_da_xep_2_ca = 0;

                var ds_ca = _context.Ca_Donvi.Where(c => c.Id_don_vi == iddonvi).ToList();

                foreach (var ca in ds_ca)
                {
                    int so_tiet_da_xep_1_ca = 0;
                    foreach (var mon in dsmon)
                    {
                         int so_tiet_1_mon_1_ca = ds_da_xep.Where(c => c.Id_lop == idlop && c.Id_mon == mon.Id_mon && c.Id_ca == ca.Id_ca_hoc && c.Ngay == Ngay).Count();
                        so_tiet_da_xep_1_ca += so_tiet_1_mon_1_ca;
                    }
                    if (so_tiet_da_xep_1_ca > ob_thm.So_tiet_toi_da_1_ca)
                    {
                        check = true;
                    }
                    so_tiet_da_xep_2_ca += so_tiet_da_xep_1_ca;
                    Console.WriteLine($"Ca {ca.Id}: {so_tiet_da_xep_1_ca} tiết");
                }
                if(ob_thm.So_tiet_toi_da_1_ca == ob_thm.So_tiet_toi_da_2_ca)
                {
                    return check;
                }
                if (ob_thm.So_tiet_toi_da_1_ca < ob_thm.So_tiet_toi_da_2_ca)
                {
                    if(so_tiet_da_xep_2_ca > ob_thm.So_tiet_toi_da_2_ca)
                    {
                        check = true;
                    }
                    return check;
                }
            }
            return check;
        }
        public bool Check_lop(int Ngay, int Tiet, int Ca, int idlop, int idmon, int id_tkb)
        {
            var object_lop = Object_lophoc(idlop, id_tkb);
            var object_lopmon = Object_lopmon(idlop, idmon, id_tkb);
            if (object_lop == null)
            {
                return false;
            }

            bool check_tietban = object_lop.ds_tiet_tranh_xep.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);
            bool check_tietban_lopmon = object_lopmon.ds_tiet_tranh_xep_lop_mon.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);
            if (check_tietban || check_tietban_lopmon)
            {
                return true;
            }
            return false;
        }
        public bool Check_mon_khoi(int Ngay, int Tiet, int Ca, int idmon, int idlop, int idDonvi)
        {
            var object_monkhoi = Object_monkhoi(idmon, idlop, idDonvi);
            if (object_monkhoi == null)
            {
                return false;
            }

            bool check_tietban = object_monkhoi.ds_tiet_tranh_xep_mon_khoi.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);

            return check_tietban;
        }
        private List<(int Ngay, int Tiet, int Id_ca)> CreateAvailableSlots(Object_Tiet objectTiet)
        {
            var availableSlots = new List<(int Ngay, int Tiet, int Id_ca)>();
            var tietTranhXep = CreateTietTranhXepHashSet(objectTiet);

            int caTietHoc = objectTiet.Id_ca;

            // Chỉ duyệt những slot không nằm trong tiết tránh xếp
            for (int ngay = 1; ngay <= 7; ngay++)
            {
                for (int tiet = 1; tiet <= 5; tiet++)
                {
                    var slotKey = $"{ngay}_{caTietHoc}_{tiet}";

                    // Chỉ thêm vào danh sách nếu KHÔNG nằm trong tiết tránh xếp
                    if (!tietTranhXep.Contains(slotKey))
                    {
                        availableSlots.Add((ngay, tiet, caTietHoc));
                    }
                }
            }

            return availableSlots;
        }
        public void FindValidPositions(Object_Tiet objectTiet, int idDonvi)
        {
            try
            {

                objectTiet.Ds_vi_tri_xep_duoc.Clear();
                LoadObjectsFromTiet(objectTiet);

                if (_ObjectMon == null || _ObjectLop == null || _ObjectGiaovien == null || _ObjectPhong == null)
                {
                    return;
                }

                // Bước 1: Tạo danh sách slot có thể xếp được (loại bỏ tiết tránh xếp)
                var availableSlots = CreateAvailableSlots(objectTiet);

                // Bước 2: Duyệt chỉ những slot có thể xếp được
                foreach (var slot in availableSlots)
                {
                    if (CheckSoftConstraints(slot.Ngay, slot.Tiet, objectTiet.Id_ca, objectTiet, idDonvi))
                    {
                        objectTiet.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
                        {
                            Ngay = slot.Ngay,
                            Tiet = slot.Tiet,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                objectTiet.Ds_vi_tri_xep_duoc.Clear();
            }
        }



        private HashSet<string> CreateTietTranhXepHashSet(Object_Tiet objectTiet)
        {
            var tietTranhXep = new HashSet<string>();

            // Tiết tránh xếp của giáo viên
            if (_ObjectGiaovien?.ds_tiet_tranh_xep != null)
            {
                foreach (var tiet in _ObjectGiaovien.ds_tiet_tranh_xep)
                {
                    tietTranhXep.Add($"{tiet.Ngay}_{tiet.Id_ca}_{tiet.Tiet}");
                }
            }

            // Tiết tránh xếp của phòng học
            if (_ObjectPhong?.ds_tiet_tranh_xep != null && _ObjectPhong.Khong_kiem_tra_xung_dot == false)
            {
                foreach (var tiet in _ObjectPhong.ds_tiet_tranh_xep)
                {
                    tietTranhXep.Add($"{tiet.Ngay}_{tiet.Id_ca}_{tiet.Tiet}");
                }
            }

            // Tiết tránh xếp của môn học
            if (_ObjectMon?.ds_tiet_tranh_xep != null)
            {
                foreach (var tiet in _ObjectMon.ds_tiet_tranh_xep)
                {
                    tietTranhXep.Add($"{tiet.Ngay}_{tiet.Id_ca}_{tiet.Tiet}");
                }
            }

            // Tiết tránh xếp của lớp
            if (_ObjectLop?.ds_tiet_tranh_xep != null)
            {
                foreach (var tiet in _ObjectLop.ds_tiet_tranh_xep)
                {
                    tietTranhXep.Add($"{tiet.Ngay}_{tiet.Id_ca}_{tiet.Tiet}");
                }
            }

            // Tiết tránh xếp của lớp-môn
            if (_ObjectLopMon?.ds_tiet_tranh_xep_lop_mon != null)
            {
                foreach (var tiet in _ObjectLopMon.ds_tiet_tranh_xep_lop_mon)
                {
                    tietTranhXep.Add($"{tiet.Ngay}_{tiet.Id_ca}_{tiet.Tiet}");
                }
            }

            // Tiết tránh xếp của môn-khối
            if (_ObjectMonKhoi?.ds_tiet_tranh_xep_mon_khoi != null)
            {
                foreach (var tiet in _ObjectMonKhoi.ds_tiet_tranh_xep_mon_khoi)
                {
                    tietTranhXep.Add($"{tiet.Ngay}_{tiet.Id_ca}_{tiet.Tiet}");
                }
            }

            return tietTranhXep;
        }

        private bool CheckSoftConstraints(int ngay, int tiet, int idCa, Object_Tiet objectTiet, int idDonvi)
        {
            if (Check_gv(ngay, tiet, idCa, objectTiet.Id_giao_vien, objectTiet.Id_phong, objectTiet.Id_tkb))
            {
                return false;
            }

            if (Check_to_hop_mon(ngay, tiet, objectTiet.Id_don_vi, objectTiet.Id_mon, objectTiet.Id_lop, objectTiet.Id_giao_vien, objectTiet.Id_tkb))
            {
                return false;
            }

            if (!CheckMonHocConstraints(ngay, tiet, idCa, objectTiet, idDonvi))
            {
                return false;
            }

            return true;
        }

        private bool CheckHocCachNgay(int ngay, Object_Tiet objectTiet)
        {
            try
            {
                if (_ObjectMon == null || !_ObjectMon.Hoc_cach_ngay)
                    return true; // Không có ràng buộc học cách ngày

                var dsDataXep = _ObjectGiaovien?.ds_tiet_da_xep?.Where(x =>
                    x.Id_lop == objectTiet.Id_lop &&
                    x.Id_mon == objectTiet.Id_mon).ToList();

                if (dsDataXep == null || dsDataXep.Count == 0)
                    return true; // Chưa có tiết nào được xếp

                // Kiểm tra ngày hôm trước có tiết cùng môn, cùng lớp không
                var ngayHomTruoc = ngay - 1;
                bool coTietHomTruoc = dsDataXep.Any(x => x.Ngay == ngayHomTruoc);

                return !coTietHomTruoc; // True nếu hôm trước KHÔNG có tiết, False nếu có
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CheckHocCachNgay: {ex.Message}");
                return false;
            }
        }

        private bool CheckMonHocConstraints(int ngay, int tiet, int idCa, Object_Tiet objectTiet, int iddonvi)
        {
            if (_ObjectMon == null) return true;

            var dsDataXep = _ObjectGiaovien?.ds_tiet_da_xep?.Where(x =>
                x.Id_lop == objectTiet.Id_lop &&
                x.Id_mon == objectTiet.Id_mon).ToList();

            // Kiểm tra học cách ngày
            if (!CheckHocCachNgay(ngay, objectTiet))
            {
                return false;
            }

            // Kiểm tra số tiết tối đa mỗi ca
            var check = true;
            int so_tiet_da_xep_2_ca = 0;
            var ds_ca = _context.Ca_Donvi.Where(c => c.Id_don_vi == iddonvi).ToList();

            foreach (var ca in ds_ca)
            {
                int so_tiet_da_xep_1_ca = 0;
                int so_tiet_1_mon_1_ca = dsDataXep?.Where(c => c.Id_ca == ca.Id_ca_hoc && c.Ngay == ngay).Count() ?? 0;
                so_tiet_da_xep_1_ca += so_tiet_1_mon_1_ca;

                if (so_tiet_da_xep_1_ca > _ObjectMon.So_tiet_toi_da_mot_ca)
                {
                    check = false;
                }

                so_tiet_da_xep_2_ca += so_tiet_da_xep_1_ca;
                Console.WriteLine($"Ca {ca.Id}: {so_tiet_da_xep_1_ca} tiết");
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
        private bool CheckXepThanhCap(Object_Tiet tiet, List<Object_Tiet> dsTietChuaXep)
        {
            try
            {
                // Load thông tin các object liên quan
                LoadObjectsFromTiet(tiet);

                // Kiểm tra điều kiện tiên quyết - môn có cần xếp thành cặp không
                if (_ObjectMon == null || !_ObjectMon.Xep_thanh_cap)
                {
                    return false; // Không cần xếp cặp
                }

                // Kiểm tra xem môn này ở lớp này đã có cặp tiết nào được xếp chưa
                if (_ObjectGiaovien?.ds_tiet_da_xep != null)
                {
                    var capDaXep = _ObjectGiaovien.ds_tiet_da_xep.Any(x =>
                        x.Id_mon == tiet.Id_mon &&
                        x.Id_lop == tiet.Id_lop &&
                        x.Id_phong == tiet.Id_phong &&
                        x.Id_ca == tiet.Id_ca);

                    if (capDaXep)
                    {
                        return false; // Đã có cặp rồi, xếp như tiết lẻ bình thường
                    }
                }

                // Tìm các tiết còn lại cùng môn, lớp, phòng, ca, giáo viên mà chưa được xếp
                var dsTietCungNhom = dsTietChuaXep.Where(t =>
                    t.Id_mon == tiet.Id_mon &&
                    t.Id_lop == tiet.Id_lop &&
                    t.Id_phong == tiet.Id_phong &&
                    t.Id_ca == tiet.Id_ca &&
                    t.Id_giao_vien == tiet.Id_giao_vien &&
                    t != tiet // Loại trừ chính tiết đang xét
                ).ToList();

                // Nếu không còn tiết nào để ghép cặp
                if (dsTietCungNhom.Count == 0)
                {
                    return true; 
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CheckXepThanhCap: {ex.Message}");
                return false;
            }
        }
        private bool UpdateTiet(Object_Tiet tiet, int ngay, int tietSo)
        {
            try
            {
                var record = _context.Chitiet_Thoikhoabieu
                    .FirstOrDefault(x => x.Id_tkb == tiet.Id_tkb &&
                                         x.Id_lop == tiet.Id_lop &&
                                         x.Id_mon == tiet.Id_mon &&
                                         x.Id_giao_vien == tiet.Id_giao_vien &&
                                         x.Id_phong == tiet.Id_phong &&
                                         x.Id_ca == tiet.Id_ca &&
                                         x.Tiet_thu_may == tiet.Tiet_thu_may);

                if (record != null)
                {
                    record.Ngay = ngay;
                    record.Tiet = tietSo;
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
        public List<Object_Tiet> ProcessThoiKhoaBieu(int idtkb, int idDonvi)
        {
            try
            {
                // 1. Load tất cả tiết cần xếp
                var dsTietGoc = Object_tiet(idtkb);
                var dsTietChuaXep = new List<Object_Tiet>(dsTietGoc);
                var dsTietDaXep = new List<Object_Tiet>();
                var dsTietBoqua = new List<Object_Tiet>();
                if (dsTietGoc == null || dsTietGoc.Count == 0)
                {
                    return new List<Object_Tiet>();
                }

                // 2. Xử lý tiết cố định trước
                var dsTietCoDinh = GetTietCoDinh(idDonvi);
                foreach (var tiet in dsTietGoc)
                {
                    var tietCoDinh = dsTietCoDinh.FirstOrDefault(tcd => tcd.Id_mon == tiet.Id_mon && tcd.Id_lop == tiet.Id_lop && tcd.Id_ca == tiet.Id_ca && tiet.Tiet_thu_may == 1);

                    if (tietCoDinh != null)
                    {
                        bool updateSuccess = UpdateTiet(tiet, tietCoDinh.Ngay, tietCoDinh.Tiet);

                        if (updateSuccess)
                        {
                            dsTietChuaXep.Remove(tiet);
                            dsTietDaXep.Add(tiet);
                        }
                        else
                        {
                            dsTietBoqua.Add(tiet);
                            dsTietChuaXep.Remove(tiet);
                        }
                    }
                }

                int vongLap = 0;
                while (dsTietChuaXep.Count > 0)
                {
                    vongLap++;

                    // BƯỚC 1: Tìm vị trí xếp được cho tất cả tiết chưa xếp
                    foreach (var tiet in dsTietChuaXep)
                    {
                        FindValidPositions(tiet, idDonvi);
                    }


                    // BƯỚC 2: Lọc các tiết có thể xếp được (vị trí > 0)
                    var dsTietCoTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count > 0).ToList();
                    var dsTietKhongTheXep = dsTietChuaXep.Where(t => t.Ds_vi_tri_xep_duoc.Count == 0).ToList();

                    if (dsTietKhongTheXep != null && dsTietKhongTheXep.Count > 0)
                    {
                        foreach (var kx in dsTietKhongTheXep)
                        {
                            dsTietBoqua.Add(kx);
                            dsTietChuaXep.Remove(kx);
                        }
                    }

                    // Nếu không còn tiết nào xếp được thì dừng
                    if (dsTietCoTheXep.Count == 0)
                    {
                        break;
                    }

                    
                    // BƯỚC 3: Sắp xếp theo thứ tự ưu tiên (tiết có ít vị trí xếp được nhất trước)
                    var dsTietSorted = dsTietCoTheXep.OrderBy(t => t.Ds_vi_tri_xep_duoc.Count).ToList();

                    // BƯỚC 4: Chọn tiết có ít vị trí xếp được nhất để xếp
                    var tietCanXep = dsTietSorted.First();

                    // BƯỚC 5: Check và xử lý các tiết cần xếp cặp TRƯỚC KHI lọc
                        if (CheckXepThanhCap(tietCanXep, dsTietChuaXep))
                        {
                            // Tiết này là tiết cuối cùng của tổ hợp cần xếp cặp → bỏ qua
                            Console.WriteLine($"Bỏ qua tiết cuối cùng không thể xếp cặp: Môn {tietCanXep.Id_mon}, Lớp {tietCanXep.Id_lop}");
                            dsTietBoqua.Add(tietCanXep);
                            dsTietChuaXep.Remove(tietCanXep);
                            continue;
                        }

                        // Nếu là tiết cần xếp cặp và còn tiết để ghép
                        if (_ObjectMon != null && _ObjectMon.Xep_thanh_cap)
                        {
                            // Kiểm tra xem đã có cặp nào được xếp chưa
                            bool capDaXep = false;
                            if (_ObjectGiaovien?.ds_tiet_da_xep != null)
                            {
                                capDaXep = _ObjectGiaovien.ds_tiet_da_xep.Any(x =>
                                    x.Id_mon == tietCanXep.Id_mon &&
                                    x.Id_lop == tietCanXep.Id_lop &&
                                    x.Id_phong == tietCanXep.Id_phong &&
                                    x.Id_ca == tietCanXep.Id_ca);
                            }

                            if (!capDaXep) // Chưa có cặp nào được xếp
                            {
                                // Tìm tiết để ghép cặp
                                var tietGhepCap = dsTietChuaXep.FirstOrDefault(t =>
                                    t.Id_mon == tietCanXep.Id_mon &&
                                    t.Id_lop == tietCanXep.Id_lop &&
                                    t.Id_phong == tietCanXep.Id_phong &&
                                    t.Id_ca == tietCanXep.Id_ca &&
                                    t.Id_giao_vien == tietCanXep.Id_giao_vien &&
                                    t != tietCanXep);

                                if (tietGhepCap != null)
                                {
                                    // Tìm vị trí có thể xếp cả 2 tiết liền kề
                                    bool daNepCap = false;
                                    foreach (var viTri1 in tietCanXep.Ds_vi_tri_xep_duoc)
                                    {
                                        // Kiểm tra vị trí tiết tiếp theo (cùng ngày, tiết kế tiếp)
                                        if (tietGhepCap.Ds_vi_tri_xep_duoc.Any(v => v.Ngay == viTri1.Ngay && v.Tiet == viTri1.Tiet + 1))
                                        {
                                            // Có thể xếp cặp → Update cả 2 tiết
                                            bool updateTiet1 = UpdateTiet(tietCanXep, viTri1.Ngay, viTri1.Tiet);
                                            bool updateTiet2 = UpdateTiet(tietGhepCap, viTri1.Ngay, viTri1.Tiet + 1);

                                            if (updateTiet1 && updateTiet2)
                                            {
                                                Console.WriteLine($"✓ Đã xếp cặp tiết: Ngày {viTri1.Ngay} Tiết {viTri1.Tiet}-{viTri1.Tiet + 1}");

                                                // Chuyển cả 2 tiết sang danh sách đã xếp
                                                dsTietChuaXep.Remove(tietCanXep);
                                                dsTietChuaXep.Remove(tietGhepCap);
                                                dsTietDaXep.Add(tietCanXep);
                                                dsTietDaXep.Add(tietGhepCap);
                                                daNepCap = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (daNepCap) continue;
                                }
                            }
                        }
                    // BƯỚC 6: Update tiết này vào database (chọn vị trí đầu tiên có thể xếp)
                    var viTriChon = tietCanXep.Ds_vi_tri_xep_duoc.First();
                    bool updateSuccess = UpdateTiet(tietCanXep, viTriChon.Ngay, viTriChon.Tiet);

                    if (updateSuccess)
                    {
                        Console.WriteLine($"✓ Đã xếp tiết lẻ vào Ngày {viTriChon.Ngay}, Tiết {viTriChon.Tiet}");

                        // Chuyển tiết từ danh sách chưa xếp sang đã xếp
                        dsTietChuaXep.Remove(tietCanXep);
                        dsTietDaXep.Add(tietCanXep);
                    }
                    else
                    {
                        Console.WriteLine($"✗ Lỗi khi update tiết vào database");
                        dsTietChuaXep.Remove(tietCanXep); // Loại bỏ để tránh lặp vô hạn
                    }

                    // Tránh lặp vô hạn
                    if (vongLap > 1000)
                    {
                        Console.WriteLine("Dừng thuật toán sau 1000 vòng lặp để tránh lặp vô hạn");
                        break;
                    }
                }

                // Kết hợp kết quả cuối cùng
                var ketQua = new List<Object_Tiet>();
                ketQua.AddRange(dsTietDaXep);
                ketQua.AddRange(dsTietChuaXep);
                ketQua.AddRange(dsTietBoqua);

                return ketQua;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessThoiKhoaBieu: {ex.Message}");
                return new List<Object_Tiet>();
            }
        }
    }
}
 