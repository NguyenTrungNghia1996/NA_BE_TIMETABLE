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
        private List<object_tiet_co_dinh> _ObjectTietcodinh;
        private List<Object_Tiet> _dsTietGoc;

        private List<Object_Monhoc> _dsObjectMon;
        private List<Object_Lophoc> _dsObjectLop;
        private List<Object_Giaovien> _dsObjectGiaovien;
        private List<Object_Phonghoc> _dsObjectPhong;
        private List<Object_lop_mon> _dsObjectLopMon;
        private List<Object_MonKhoi> _dsObjectMonKhoi;
        private List<Object_Tohopmon> _dsObjectTohopmon;
        private List<object_tiet_co_dinh> _dsObjectTietcodinh;

        public ObjectRepository(NA_DbContext context)
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
        public Object_Tiet Object_tiet(int idtkb)
        {
            try
            {
                var paramIdTkb = new SqlParameter("@Id_tkb", SqlDbType.Int) { Value = idtkb };

                var item = _context.Set<Chitiet_Thoikhoabieu_List>()
                    .FromSqlRaw("EXEC Get_Object @Id_tkb", paramIdTkb)
                    .AsEnumerable()   
                    .FirstOrDefault();

                if (item == null) return null;

                return new Object_Tiet
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
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        //object giáo viên
        public Object_Giaovien Object_giaovien(int? idgv, int idtkb)
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
                //var tietban = _context.Giaovien_Tiettranhxep.Where(c => c.Id_giao_vien == idgv).ToList();
                if (result == null) return null;
                var teacher = new Object_Giaovien
                {
                    Id_giao_vien = result[0].Id_giao_vien ?? 0,
                    Chi_day_mot_buoi = giaovien?.Chi_day_mot_buoi ?? false,
                    So_tiet_toi_da = giaovien?.So_tiet_toi_da ?? 0,
                    //ds_tiet_phan_cong = new List<Ds_tiet_phan_cong>(),
                    ds_tiet_da_xep = new List<Ds_tiet_da_xep>(),
                    ds_tiet_chua_xep = new List<Ds_chua_xep>(),
                    //ds_tiet_tranh_xep = new List<Ds_tiet_tranh_xep>()
                };
                foreach (var r in result)
                {
                    //teacher.ds_tiet_phan_cong.Add(new Ds_tiet_phan_cong
                    //{
                    //    Id_mon = r.Id_mon,
                    //    Ten_mon = r.Ten_mon,
                    //    Id_lop = r.Id_lop,
                    //    Ten_lop = r.Ten_lop,
                    //    Id_phong = r.Id_phong,
                    //    Ten_phong = r.Ten_phong,
                    //    Id_ca = r.Id_ca,
                    //    Tiet = r.Tiet,
                    //    Ngay = r.Ngay
                    //});

                    if (r.Tiet > 0 && r.Ngay > 0)
                    {
                        teacher.ds_tiet_da_xep.Add(new Ds_tiet_da_xep
                        {
                            Id_giao_vien = r.Id_giao_vien ?? 0,
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
                //foreach (var item in tietban)
                //{
                //    teacher.ds_tiet_tranh_xep.Add(new Ds_tiet_tranh_xep
                //    {

                //        Id_ca = item.Id_ca,
                //        Tiet = item.Tiet,
                //        Ngay = item.Ngay
                //    });
                //}

                return teacher;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //object phòng học
        //public Object_Phonghoc Object_phonghoc(int? idph, int idtkb)
        //{
        //    try
        //    {
        //        var paramIdphong = new SqlParameter("Id_phong", SqlDbType.Int)
        //        {
        //            Value = idph
        //        };
        //        var paramIdTkb = new SqlParameter("Id_tkb", SqlDbType.Int)
        //        {
        //            Value = idtkb
        //        };
        //        var result = _context.Set<Chitiet_Thoikhoabieu_List>().FromSqlRaw("EXEC Get_Object @Id_phong = @Id_phong, @Id_tkb = @Id_tkb",
        //              paramIdphong, paramIdTkb)
        //            .ToList();
        //        var phonghoc  = _context.DM_Phonghoc.FirstOrDefault(ph => ph.Id == idph); 
        //        var tietban = _context.Tiet_ban.Where(c => c.Id_phong == idph).ToList();
        //        if (result == null) return null;
        //        var room = new Object_Phonghoc
        //        {
        //            Id_don_vi = result[0].Id_don_vi ?? 0,
        //            Ten_don_vi = result[0].Ten_don_vi,
        //            Id_phong = result[0].Id_phong,  
        //            Ten_phong = result[0].Ten_phong,
        //            Id_loai_phong = phonghoc.Id_Loai_phong_hoc,
        //            Khong_kiem_tra_xung_dot = phonghoc.Khong_kiem_tra_xung_dot,
        //            ds_mon_tai_phong = new List<Ds_mon>(),
        //            ds_tiet_tranh_xep = new List<Ds_tiet_tranh_xep>(),
        //            //ds_tiet_da_xep = new List<Ds_tiet_da_xep>()
        //        };
        //        room.ds_mon_tai_phong = result.Select(r => new Ds_mon
        //                                 {
        //                                     Id_mon = r.Id_mon,
        //                                     Ten_mon = r.Ten_mon
        //                                 }).GroupBy(m => m.Id_mon).Select(g => g.First()).ToList();
        //        foreach (var item in tietban)
        //        {
        //            room.ds_tiet_tranh_xep.Add(new Ds_tiet_tranh_xep
        //            {

        //                Id_ca = item.Id_ca,
        //                Tiet = item.Tiet,
        //                Ngay = item.Thu
        //            });
        //        }
        //        return room;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        //object môn học
        public Object_Monhoc Object_monhoc(int? idmon, int idDonvi)
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
                //var tietcodinh = _context.Tiet_co_dinh.Where(c => c.Id_mon == idmon).ToList();
                //var tietban = _context.Tiet_Tranh_Xep.Where(c => c.Id_mon == idmon).ToList();

                var subject = new Object_Monhoc
                {
                    Id_mon = monhoc.Id_mon,
                    Hoc_cach_ngay = monhoc.Hoc_cach_ngay,
                    Xep_thanh_cap = monhoc.Xep_thanh_cap,
                    So_tiet_toi_da_mot_ca = monhoc.So_tiet_toi_da_1_ca,
                    So_tiet_toi_da_hai_ca = monhoc.So_tiet_toi_da_2_ca,
                    //ds_tiet_co_dinh = new List<Ds_tiet_co_dinh>(),
                    //ds_tiet_tranh_xep = new List<Ds_tiet_tranh_xep>()
                };
                //foreach (var r in tietcodinh)
                //{
                //    subject.ds_tiet_co_dinh.Add(new Ds_tiet_co_dinh
                //    {
                //        Id_khoi = r.Id_khoi_lop,
                //        Id_ca = r.Id_ca,
                //        Ngay = r.Ngay,
                //        Tiet = r.Tiet
                //    });
                //}
                //foreach (var item in tietban)
                //{
                //    subject.ds_tiet_tranh_xep.Add(new Ds_tiet_tranh_xep
                //    {

                //        Id_ca = item.Id_ca,
                //        Tiet = item.Tiet,
                //        Ngay = item.Thu
                //    });
                //}

                return subject;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //public List<Object_Tohopmon> Object_tohopmon(int? idmon, int? idlop, int? idDonvi)
        //{
        //    try
        //    {
        //        var lop = _context.DM_Lophoc.FirstOrDefault(c=>c.Id == idlop);
        //        int idkhoi = lop.Id_khoi;
        //        int idban = lop.Id_ban;
        //        var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int)
        //        {
        //            Value = idDonvi 
        //        };
        //        var paramIdMon = new SqlParameter("id_mon", SqlDbType.Int)
        //        {
        //            Value = idmon 
        //        };
        //        var paramIdkhoi = new SqlParameter("id_khoi", SqlDbType.Int)
        //        {
        //            Value = idkhoi 
        //        };
        //        var paramIdban = new SqlParameter("id_ban", SqlDbType.Int)
        //        {
        //            Value = idban 
        //        };
        //        var result = _context.Set<Monhoc_Tohopmon_List>().FromSqlRaw("EXEC MonTohop_GetList_Paging  @idDonvi = @idDonvi, @id_mon = @id_mon, @id_khoi = @id_khoi, @id_ban = @id_ban",
        //                        paramIdDonvi, paramIdMon, paramIdkhoi, paramIdban).ToList();
        
        //        if (result == null || result.Count == 0) 
        //            return new List<Object_Tohopmon>();

        //        var listTohopmon = new List<Object_Tohopmon>();

        //        var groupedResult = result.GroupBy(r => r.Id);

        //        foreach (var group in groupedResult)
        //        {
        //            var firstItem = group.First();
        //            var tohopmon = new Object_Tohopmon
        //            {
        //                Id_to_hop_mon = firstItem.Id,
        //                ds_mon = new List<Ds_mon>(),
        //                So_tiet_toi_da_1_ca = firstItem.So_tiet_toi_da_1_ca,
        //                So_tiet_toi_da_2_ca = firstItem.So_tiet_toi_da_2_ca
        //            };

        //            foreach (var r in group)
        //            {
        //                if (r.Id_mon_1 > 0 && !string.IsNullOrEmpty(r.Ten_mon_hoc_1))
        //                {
        //                    if (!tohopmon.ds_mon.Any(m => m.Id_mon == r.Id_mon_1))
        //                    {
        //                        tohopmon.ds_mon.Add(new Ds_mon
        //                        {
        //                            Id_mon = r.Id_mon_1,
        //                            Ten_mon = r.Ten_mon_hoc_1
        //                        });
        //                    }
        //                }

        //                if (r.Id_mon_2 > 0 && !string.IsNullOrEmpty(r.Ten_mon_hoc_2))
        //                {
        //                    if (!tohopmon.ds_mon.Any(m => m.Id_mon == r.Id_mon_2))
        //                    {
        //                        tohopmon.ds_mon.Add(new Ds_mon
        //                        {
        //                            Id_mon = r.Id_mon_2,
        //                            Ten_mon = r.Ten_mon_hoc_2
        //                        });
        //                    }
        //                }

        //                if (r.Id_mon_3 > 0 && !string.IsNullOrEmpty(r.Ten_mon_hoc_3))
        //                {
        //                    if (!tohopmon.ds_mon.Any(m => m.Id_mon == r.Id_mon_3))
        //                    {
        //                        tohopmon.ds_mon.Add(new Ds_mon
        //                        {
        //                            Id_mon = r.Id_mon_3,
        //                            Ten_mon = r.Ten_mon_hoc_3
        //                        });
        //                    }
        //                }
        //            }

        //            listTohopmon.Add(tohopmon);
        //        }

        //        return listTohopmon;
        //    }
        //    catch (Exception)
        //    {
        //        return new List<Object_Tohopmon>();
        //    }
        //}
        //object lớp học
        //public Object_Lophoc Object_lophoc(int? idlop, int idtkb)
        //{
        //    try
        //    {
        //        var paramIdLop = new SqlParameter("Id_lop", SqlDbType.Int)
        //        {
        //            Value = idlop
        //        };
        //        var paramIdTkb = new SqlParameter("Id_tkb", SqlDbType.Int)
        //        {
        //            Value = idtkb
        //        };


        //        var result = _context.Set<Chitiet_Thoikhoabieu_List>().FromSqlRaw("EXEC Get_Object @Id_lop = @Id_lop, @Id_tkb = @Id_tkb",
        //              paramIdLop, paramIdTkb)
        //            .ToList();
        //        var tietban = _context.Lophoc_Tietnghi.Where(c => c.Id_lop == idlop).ToList();
        //        var tietban_lopmon = _context.Lophoc_Monhoc_Tiettranhxep.Where(c => c.Id_lop == idlop).ToList();
        //        if (result == null) return null;
        //        var lop = new Object_Lophoc
        //        {
        //            Id_don_vi = result[0].Id_don_vi ?? 0,
        //            Ten_don_vi = result[0].Ten_don_vi,
        //            Id_lop = result[0].Id_lop,
        //            Ten_lop = result[0].Ten_lop,
        //            ds_lop_mon = new List<Ds_tiet_phan_cong>(),
        //            ds_tiet_tranh_xep = new List<Ds_tiet_tranh_xep>(),
        //            //ds_tiet_tranh_xep_lop_mon = new List<Ds_tiet_tranh_xep_lop_mon>()
        //        };
        //        foreach (var r in result)
        //        {
        //            lop.ds_lop_mon.Add(new Ds_tiet_phan_cong
        //            {
        //                Id_mon = r.Id_mon,
        //                Ten_mon = r.Ten_mon,
        //                Id_lop = r.Id_lop,
        //                Ten_lop = r.Ten_lop,
        //                Id_phong = r.Id_phong,
        //                Ten_phong = r.Ten_phong
        //            });
        //        }
        //        foreach (var item in tietban)
        //        {
        //            lop.ds_tiet_tranh_xep.Add(new Ds_tiet_tranh_xep
        //            {

        //                Id_ca = item.Id_ca,
        //                Tiet = item.Tiet,
        //                Ngay = item.Ngay
        //            });
        //        }
        //        //lop.ds_tiet_tranh_xep_lop_mon = tietban_lopmon.GroupBy(x => x.Id_mon).Select(g => new Ds_tiet_tranh_xep_lop_mon
        //        //                                {
        //        //                                    Id_mon = g.Key,
        //        //                                    Ds_tiet_tranh_xep_monlop = g.Select(item => new Ds_tiet_tranh_xep
        //        //                                    {
        //        //                                        Id_ca = item.Id_ca,
        //        //                                        Tiet = item.Tiet,
        //        //                                        Ngay = item.Ngay
        //        //                                    }).ToList()
        //        //                                }).ToList();

        //        return lop;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        //object môn khối
        //public Object_MonKhoi Object_monkhoi(int? idmon, int? idlop, int? idDonvi)
        //{
        //    try
        //    {
        //        var monhoc = _context.Dm_Monhoc.Join(_context.DM_Donvi, mh => mh.Id_don_vi, dv => dv.Id,
        //                     (mh, dv) => new { Id_mon = mh.Id, Ten_mon = mh.Ten, Id_don_vi = dv.Id, Ten_don_vi = dv.TenDonvi })
        //                     .FirstOrDefault(mh => mh.Id_mon == idmon && mh.Id_don_vi == idDonvi);
        //        var khoi = _context.DM_Lophoc.Join(_context.DM_Khoilop, lh => lh.Id_khoi, kl => kl.Id,
        //                    (lh, kl) => new { Id_khoi = kl.Id, Ten_khoi = kl.Ten, Id_lop = lh.Id, Id_ban = lh.Id_ban }).FirstOrDefault(x => x.Id_lop == idlop);
        //        var tietban = _context.Monhoc_Khoilop_Tiettranhxep.Where(c => c.Id_mon == idmon && c.Id_ban == khoi.Id_ban && c.Id_khoi == khoi.Id_khoi).ToList();

        //        var monkhoi = new Object_MonKhoi
        //        {
        //            Id_don_vi = monhoc.Id_don_vi,
        //            Ten_don_vi = monhoc.Ten_don_vi,
        //            Id_mon = monhoc.Id_mon,
        //            Ten_mon = monhoc.Ten_mon,
        //            Id_khoi = khoi.Id_khoi,
        //            Ten_khoi = khoi.Ten_khoi,
        //            ds_tiet_tranh_xep_mon_khoi = new List<Ds_tiet_tranh_xep_mon_khoi>()
        //        };
                
        //        foreach (var item in tietban)
        //        {
        //            monkhoi.ds_tiet_tranh_xep_mon_khoi.Add(new Ds_tiet_tranh_xep_mon_khoi
        //            {

        //                Id_ca = item.Id_ca,
        //                Tiet = item.Tiet,
        //                Ngay = item.Ngay
        //            });
        //        }

        //        return monkhoi;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        //public Object_lop_mon Object_lopmon(int? idmon, int? idlop, int? idDonvi)
        //{
        //    try
        //    {
        //        var monhoc = _context.Dm_Monhoc.Join(_context.DM_Donvi, mh => mh.Id_don_vi, dv => dv.Id,
        //                     (mh, dv) => new { Id_mon = mh.Id, Ten_mon = mh.Ten, Id_don_vi = dv.Id, Ten_don_vi = dv.TenDonvi })
        //                     .FirstOrDefault(mh => mh.Id_mon == idmon && mh.Id_don_vi == idDonvi);
        //        var lop = _context.DM_Lophoc.FirstOrDefault(x => x.Id == idlop);
        //        var tietban = _context.Lophoc_Monhoc_Tiettranhxep.Where(c => c.Id_mon == idmon && c.Id_lop == idlop).ToList();

        //        var lopmon = new Object_lop_mon
        //        {
        //            Id_don_vi = monhoc.Id_don_vi,
        //            Ten_don_vi = monhoc.Ten_don_vi,
        //            Id_mon = monhoc.Id_mon,
        //            Ten_mon = monhoc.Ten_mon,
        //            Id_lop = lop.Id,
        //            Ten_lop = lop.Ten,
        //            ds_tiet_tranh_xep_lop_mon = new List<Ds_tiet_tranh_xep>()
        //        };
                
        //        foreach (var item in tietban)
        //        {
        //            lopmon.ds_tiet_tranh_xep_lop_mon.Add(new Ds_tiet_tranh_xep
        //            {
        //                Id_ca = item.Id_ca,
        //                Tiet = item.Tiet,
        //                Ngay = item.Ngay
        //            });
        //        }

        //        return lopmon;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        public void LoadObjectsFromTiet(Object_Tiet objectTiet, int idDonvi)
        {
            try
            {
                if (objectTiet == null)
                {
                    return;
                }

                //var donvi = _context.DM_Lophoc
                //    .Join(_context.DM_Donvi, lh => lh.Id_don_vi, dv => dv.Id,
                //          (lh, dv) => new { Id_don_vi = dv.Id, Id_lop = lh.Id })
                //    .FirstOrDefault(x => x.Id_lop == objectTiet.Id_lop);

                //if (donvi == null)
                //{
                //    return;
                //}

                _ObjectMon = _dsObjectMon.FirstOrDefault(c=> c.Id_mon == objectTiet.Id_mon);
                _ObjectLop = _dsObjectLop.FirstOrDefault(c=> c.Id_lop == objectTiet.Id_lop);
                _ObjectGiaovien = _dsObjectGiaovien.FirstOrDefault(c => c.Id_giao_vien == objectTiet.Id_giao_vien);
                if (objectTiet.Id_phong == 0)
                {
                    _ObjectPhong = null;
                }
                else
                {
                    _ObjectPhong = _dsObjectPhong.FirstOrDefault(c=>c.Id_phong == objectTiet.Id_phong);
                }
                _ObjectLopMon = _dsObjectLopMon.FirstOrDefault(c=>c.Id_lop == objectTiet.Id_lop && c.Id_mon == objectTiet.Id_mon);
                //_ObjectMonKhoi = _dsObjectMonKhoi.FirstOrDefault(c=>c.Id_mon = objectTiet.Id_mon && c.Id_ban == _);
                //_ObjectTohopmon = Object_tohopmon(objectTiet.Id_mon, objectTiet.Id_lop, objectTiet.Id_don_vi);
            }
            catch (Exception ex)
            {
                return;
            }
        }
        public void LoadObjectsFromTiet_Test(int idTkb, int idDonvi)
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

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "Test";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Thêm tham số
                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@idDonvi";
                    p1.Value = idDonvi;
                    cmd.Parameters.Add(p1);

                    var p2 = cmd.CreateParameter();
                    p2.ParameterName = "@@idTkb";
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
                                Id_tkb = reader.GetInt32(2),
                                Id_ca = reader.GetInt32(2),
                                Id_giao_vien = reader.GetInt32(4),
                                Ten_giao_vien = !reader.IsDBNull(5) ? reader.GetString(5) : "",
                                Id_lop = reader.GetInt32(6),
                                Ten_lop = !reader.IsDBNull(7)?reader.GetString(7) : "",
                                Id_mon = reader.GetInt32(0),
                                Ten_mon = !reader.IsDBNull (8)?reader.GetString(8) :"",
                                Id_phong = reader.GetInt32(9),
                                Ten_phong = !reader.IsDBNull(10)? reader.GetString(10) :"",
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
                for (int i = 0; i < ob_giaovien.Count; i++)
                {
                    var gv = ob_giaovien[i];
                    gv.ds_tiet_da_xep = ob_tiet
                        .Where(t=> t.Ngay > 0 && t.Tiet > 0)
                        .Select(t => new Ds_tiet_da_xep
                        {
                            Id_giao_vien = t.Id_giao_vien,
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
                for (int i = 0; i < ob_giaovien.Count; i++)
                {
                    var gv = ob_giaovien[i];
                    gv.ds_tiet_chua_xep = ob_tiet
                        .Where(t=> t.Ngay > 0 && t.Tiet > 0)
                        .Select(t => new Ds_chua_xep
                        {
                            Id_giao_vien = t.Id_giao_vien,
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

            }
            catch (Exception ex)
            {
                return;
            }
        }
       

        public bool Check_gv(int Ngay, int Tiet, int Ca, int? id_giaovien, int? idphong, int id_tkb)
        {
            var object_gv = _ObjectGiaovien;
            if (object_gv == null)
            {
                return false;
            }
            bool check_trung_gv = object_gv.ds_tiet_da_xep.Any(t => t.Id_phong == idphong && t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);
            bool check_trung_phong = object_gv.ds_tiet_da_xep.Any(t => t.Id_giao_vien == id_giaovien && t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);
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
                if (count_tiet_trong_ngay > object_gv.So_tiet_toi_da)
                {
                    check_so_tiet_toi_da = true;
                }
            }

            if (check_trung_gv|| check_trung_phong || check_so_tiet_toi_da || check_chi_day_mot_buoi)  { return true; }    

            return false;
        }
        //public bool Check_phong(int Ngay, int Tiet, int Ca, int idphong, int id_tkb)
        //{
        //    var object_phong = Object_phonghoc(idphong, id_tkb);
        //    if (object_phong == null)
        //    {
        //        return false;
        //    }

        //    bool check_tietban = false;
        //    if(object_phong.Khong_kiem_tra_xung_dot == true)
        //    {
        //        check_tietban = object_phong.ds_tiet_tranh_xep.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);
        //    }

        //    return check_tietban;
        //}
        //public bool Check_mon(int Ngay, int Tiet, int Ca, int idmon, int id_tkb)
        //{
        //    var object_mon = Object_monhoc(idmon, id_tkb);
        //    if (object_mon == null)
        //    {
        //        return false;
        //    }

        //    bool check_tietban = object_mon.ds_tiet_tranh_xep.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);

        //    return check_tietban;
        //}
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


        public bool Check_to_hop_mon(int Ngay, int Tiet, int? iddonvi, int? idmon,int? idlop, int? idgv,  int id_tkb)
        {
            var list_thm = _ObjectTohopmon;
            var ds_da_xep = _ObjectGiaovien.ds_tiet_da_xep;
            var check = false;
            foreach (var ob_thm in list_thm)
            {
                //var dsmon = ob_thm.ds_mon;
                int so_tiet_da_xep_2_ca = 0;

                var ds_ca = _context.Ca_Donvi.Where(c => c.Id_don_vi == iddonvi).ToList();

                foreach (var ca in ds_ca)
                {
                    int so_tiet_da_xep_1_ca = 0;
                    //foreach (var mon in dsmon)
                    //{
                    //     int so_tiet_1_mon_1_ca = ds_da_xep.Where(c => c.Id_lop == idlop && c.Id_mon == mon.Id_mon && c.Id_ca == ca.Id_ca_hoc && c.Ngay == Ngay).Count();
                    //    so_tiet_da_xep_1_ca += so_tiet_1_mon_1_ca+1;
                    //}
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
        //public bool Check_lop(int Ngay, int Tiet, int Ca, int idlop, int idmon, int id_tkb)
        //{
        //    var object_lop = Object_lophoc(idlop, id_tkb);
        //    var object_lopmon = Object_lopmon(idlop, idmon, id_tkb);
        //    if (object_lop == null)
        //    {
        //        return false;
        //    }

        //    bool check_tietban = object_lop.ds_tiet_tranh_xep.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);
        //    bool check_tietban_lopmon = object_lopmon.ds_tiet_tranh_xep_lop_mon.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);
        //    if (check_tietban || check_tietban_lopmon)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //public bool Check_mon_khoi(int Ngay, int Tiet, int Ca, int idmon, int idlop, int idDonvi)
        //{
        //    var object_monkhoi = Object_monkhoi(idmon, idlop, idDonvi);
        //    if (object_monkhoi == null)
        //    {
        //        return false;
        //    }

        //    bool check_tietban = object_monkhoi.ds_tiet_tranh_xep_mon_khoi.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);

        //    return check_tietban;
        //}
        public void TimViTriXepDuoc(Object_Tiet objectTiet, int idDonvi)
        {
            try
            {
                objectTiet.Ds_vi_tri_xep_duoc.Clear();
                LoadObjectsFromTiet(objectTiet, idDonvi);

                if (_ObjectMon == null || _ObjectGiaovien == null)
                    return;

                var tietban = DsTietTranhXep(objectTiet);
                int caTietHoc = objectTiet.Id_ca;

                // Duyệt trực tiếp và check luôn - chỉ 1 lần duyệt
                for (int ngay = 1; ngay <= 7; ngay++)
                {
                    for (int tiet = 1; tiet <= 5; tiet++)
                    {
                        var slotKey = $"{ngay}_{caTietHoc}_{tiet}";

                        if (tietban.Contains(slotKey))
                            continue;

                        if (CheckDieuKienConLai(ngay, tiet, caTietHoc, objectTiet, idDonvi))
                        {
                            objectTiet.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
                            {
                                Ngay = ngay,
                                Tiet = tiet,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objectTiet.Ds_vi_tri_xep_duoc.Clear();
            }
        }
        public void TimViTriXepDuoc_Lop(Object_Tiet objectTiet, int idDonvi)
        {
            try
            {
                objectTiet.Ds_vi_tri_xep_duoc.Clear();
                LoadObjectsFromTiet(objectTiet, idDonvi);

                if (_ObjectMon == null || _ObjectGiaovien == null)
                    return;

                var tietban = DsTietTranhXep(objectTiet);
                int caTietHoc = objectTiet.Id_ca;
                var ds_tiet_da_xep_gv = _ObjectGiaovien.ds_tiet_da_xep.Where(t => t.Id_giao_vien == objectTiet.Id_giao_vien).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();

                for (int ngay = 1; ngay <= 7; ngay++)
                {
                    for (int tiet = 1; tiet <= 5; tiet++)
                    {
                        var slotKey = $"{ngay}_{caTietHoc}_{tiet}";

                        if (tietban.Contains(slotKey) || ds_tiet_da_xep_gv.Contains(slotKey))
                            continue;
                        else
                        {
                            objectTiet.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
                            {
                                Ngay = ngay,
                                Tiet = tiet,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objectTiet.Ds_vi_tri_xep_duoc.Clear();
            }
        }
        public bool CheckViTriXepDuoc_Lop(Object_Tiet objectTiet, int Ca, int Ngay, int Tiet)
        {
            try
            {
                Object_Giaovien ob_gv = Object_giaovien(objectTiet.Id_giao_vien, objectTiet.Id_tkb);
                var tietban = DsTietTranhXep(objectTiet);
                var ds_tiet_da_xep_gv = ob_gv.ds_tiet_da_xep.Where(t => t.Id_giao_vien == objectTiet.Id_giao_vien).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();

                var slotKey = $"{Ngay}_{Ca}_{Tiet}";

                if (tietban.Contains(slotKey) || ds_tiet_da_xep_gv.Contains(slotKey))
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void TimViTriXepDuoc_GV(Object_Tiet objectTiet, int idDonvi)
        {
            try
            {
                objectTiet.Ds_vi_tri_xep_duoc.Clear();
                LoadObjectsFromTiet(objectTiet, idDonvi);

                if (_ObjectMon == null || _ObjectGiaovien == null)
                    return;

                var tietban = DsTietTranhXep(objectTiet);
                int caTietHoc = objectTiet.Id_ca;
                var ds_tiet_da_xep_phong = _ObjectGiaovien.ds_tiet_da_xep.Where(t => t.Id_phong == objectTiet.Id_phong).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();

                for (int ngay = 1; ngay <= 7; ngay++)
                {
                    for (int tiet = 1; tiet <= 5; tiet++)
                    {
                        var slotKey = $"{ngay}_{caTietHoc}_{tiet}";

                        if (tietban.Contains(slotKey) || ds_tiet_da_xep_phong.Contains(slotKey))
                            continue;
                        else
                        {
                            objectTiet.Ds_vi_tri_xep_duoc.Add(new Ds_vi_tri_xep_duoc
                            {
                                Ngay = ngay,
                                Tiet = tiet,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objectTiet.Ds_vi_tri_xep_duoc.Clear();
            }
        }
        public bool CheckViTriXepDuoc_GV(Object_Tiet objectTiet, int Ca, int Ngay, int Tiet)
        {
            try
            {
                Object_Giaovien ob_gv = Object_giaovien(objectTiet.Id_giao_vien, objectTiet.Id_tkb);
                var tietban = DsTietTranhXep(objectTiet);
                var ds_tiet_da_xep_phong = _ObjectGiaovien.ds_tiet_da_xep.Where(t => t.Id_phong == objectTiet.Id_phong).Select(c => $"{c.Ngay}_{c.Id_ca}_{c.Tiet}").ToList();
                var slotKey = $"{Ngay}_{Ca}_{Tiet}";
                if (tietban.Contains(slotKey) || ds_tiet_da_xep_phong.Contains(slotKey))
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private HashSet<string> DsTietTranhXep(Object_Tiet objectTiet)
        {
            //var tietTranhXep = new HashSet<string>();
            var paramIdMon = new SqlParameter("Id_mon", SqlDbType.Int)
            {
                Value = objectTiet.Id_mon
            };
            var paramIdgv = new SqlParameter("Id_gv", SqlDbType.Int)
            {
                Value = objectTiet.Id_giao_vien
            };
            var paramIdlop = new SqlParameter("Id_lop", SqlDbType.Int)
            {
                Value = objectTiet.Id_lop
            };
            var paramIdphong = new SqlParameter("Id_phong", SqlDbType.Int)
            {
                Value = objectTiet.Id_phong
            };
            var result = _context.Set<Ds_tiet_tranh_xep>().FromSqlRaw("EXEC GetList_TietTranhXep  @Id_mon, @Id_gv, @Id_lop, @Id_phong",
                               paramIdMon, paramIdgv, paramIdlop, paramIdphong).ToList();
            var hashSet = new HashSet<string>();
            foreach (var item in result)
            {
                string key = $"{item.Ngay}_{item.Id_ca}_{item.Tiet}";
                hashSet.Add(key);
            }
            return hashSet;
        }


        private bool CheckDieuKienConLai(int ngay, int tiet, int idCa, Object_Tiet objectTiet, int idDonvi)
        {
            if (Check_gv(ngay, tiet, idCa, objectTiet.Id_giao_vien, objectTiet.Id_phong, objectTiet.Id_tkb))
            {
                return false;
            }

            if (Check_to_hop_mon(ngay, tiet, objectTiet.Id_don_vi, objectTiet.Id_mon, objectTiet.Id_lop, objectTiet.Id_giao_vien, objectTiet.Id_tkb))
            {
                return false;
            }

            if (!CheckMonHoc(ngay, tiet, idCa, objectTiet, idDonvi))
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
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool CheckMonHoc(int ngay, int tiet, int idCa, Object_Tiet objectTiet, int iddonvi)
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
            var ds_ca = _context.Ca_Donvi.Where(c => c.Id_don_vi == iddonvi).ToList();

            foreach (var ca in ds_ca)
            {
                int so_tiet_da_xep_1_ca = 0;
                int so_tiet_1_mon_1_ca = dsDataXep?.Where(c => c.Id_ca == ca.Id_ca_hoc && c.Ngay == ngay).Count() ?? 0;
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
                if (_ObjectMon == null || !_ObjectMon.Xep_thanh_cap)
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
            // tìm tiết đã xếp
            var tietDaXep = dsTietDaXep.FirstOrDefault(t =>
                t.Id_mon == tietCanXep.Id_mon && t.Id_lop == tietCanXep.Id_lop &&
                t.Id_phong == tietCanXep.Id_phong && t.Id_ca == tietCanXep.Id_ca &&
                t.Id_giao_vien == tietCanXep.Id_giao_vien);

            if (tietDaXep != null)
            {
                // Tìm vị trí liền kề với tiết đã xếp
                var vtLienKe = tietCanXep.Ds_vi_tri_xep_duoc.FirstOrDefault(vt =>
                    vt.Ngay == tietDaXep.Ngay && (vt.Tiet == tietDaXep.Tiet - 1 || vt.Tiet == tietDaXep.Tiet + 1));

                if (vtLienKe != null && UpdateTiet(tietCanXep, vtLienKe.Ngay, vtLienKe.Tiet))
                {
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
                t.Id_phong == tietCanXep.Id_phong && t.Id_ca == tietCanXep.Id_ca &&
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
                if (tietGhepCap.Ds_vi_tri_xep_duoc.Any(v => v.Ngay == vt.Ngay && v.Tiet == vt.Tiet + 1))
                {
                    if (UpdateTiet(tietCanXep, vt.Ngay, vt.Tiet) && UpdateTiet(tietGhepCap, vt.Ngay, vt.Tiet + 1))
                    {
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
        public bool ProcessThoiKhoaBieu(int idtkb, int idDonvi)
        {
            try
            {
                // 1. Load tất cả tiết cần xếp
                _dsTietGoc = List_Object_tiet(idtkb);
                var dsTietChuaXep = new List<Object_Tiet>(_dsTietGoc);
                
                var dsTietDaXep = new List<Object_Tiet>();
                var dsTietBoqua = new List<Object_Tiet>();
                if (_dsTietGoc == null || _dsTietGoc.Count == 0)
                {
                    return false;
                }

                // 2. Xử lý tiết cố định trước
                var dsTietCoDinh = GetTietCoDinh(idDonvi);
                foreach (var tiet in _dsTietGoc)
                {
                    var tietCoDinh = dsTietCoDinh.FirstOrDefault(tcd => tcd.Id_mon == tiet.Id_mon && tcd.Id_lop == tiet.Id_lop && tcd.Id_ca == tiet.Id_ca && tiet.Tiet_thu_may == 1);
                    
                    if (tietCoDinh != null)
                    {
                        LoadObjectsFromTiet(tiet, idDonvi);
                        if (_ObjectPhong == null || _ObjectPhong.Id_loai_phong == 1)
                        {
                            bool updateSuccess = UpdateTiet(tiet, tietCoDinh.Ngay, tietCoDinh.Tiet);

                            if (updateSuccess)
                            {
                                tiet.Ngay = tietCoDinh.Ngay;
                                tiet.Tiet = tietCoDinh.Tiet;
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
                }

                //3. lặp đến khi ds chưa xếp = 0
                int vongLap = 0;
                while (dsTietChuaXep.Count > 0)
                {
                    vongLap++;

                    //b1: Tìm vị trí xếp được cho tất cả tiết chưa xếp
                    foreach (var tiet in dsTietChuaXep)
                    {
                        TimViTriXepDuoc(tiet, idDonvi);
                    }
                    // b2: Lọc các tiết có thể xếp được (vị trí > 0), nếu vị trí = 0 thì thêm vào ds bỏ qua
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
                    bool updateSuccess = UpdateTiet(tietCanXep, viTriChon.Ngay, viTriChon.Tiet);

                    if (updateSuccess)
                    {
                        tietCanXep.Ngay = viTriChon.Ngay;
                        tietCanXep.Tiet = viTriChon.Tiet;
                        dsTietChuaXep.Remove(tietCanXep);
                        dsTietDaXep.Add(tietCanXep);
                    }
                    else
                    {
                        Console.WriteLine($"Lỗi khi update tiết vào database");
                        dsTietChuaXep.Remove(tietCanXep);
                    }
                    if (vongLap > 1000)
                    {
                        Console.WriteLine("Dừng thuật toán sau 1000 vòng lặp để tránh lặp vô hạn");
                        break;
                    }
                }
                // Kết hợp kết quả cuối cùng
                //var ketQua = new List<Object_Tiet>();
                //ketQua.AddRange(dsTietDaXep);
                //ketQua.AddRange(dsTietBoqua);
               
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessThoiKhoaBieu: {ex.Message}");
                return false;
            }
        }

        public ObjectTiet_theoLopDto GetTkbByLop(int id_lop, int idtkb)
        {
            _dsTietGoc = List_Object_tiet(idtkb);
            if (_dsTietGoc == null || _dsTietGoc.Count == 0)
            {
                return null;
            }

            var tiet = _dsTietGoc.Where(t => t.Id_lop == id_lop).ToList();

            if (!tiet.Any())
            {
                return null;
            }

            var firstTiet = tiet.First();
            int idDonvi = firstTiet.Id_don_vi ?? 0;

            var dsCa = _context.Ca_Donvi.Where(cd => cd.Id_don_vi == 1)
                             .Join(_context.DM_Cahoc,
                                   cd => cd.Id_ca_hoc,
                                   ca => ca.Id,
                                   (cd, ca) => new
                                   {
                                       Id = ca.Id,
                                       Ten = ca.Ten
                                   }).ToList();

            var tietTranhXep = _context.Lophoc_Tietnghi.Where(c => c.Id_lop == id_lop).ToList();

            var dsNgay = Enum.GetValues<Ngay>().ToList();
            var dsTietEnum = Enum.GetValues<Tiet>().ToList();

            var result = new ObjectTiet_theoLopDto
            {
                Id_lop = id_lop,
                Ten_lop = firstTiet.Ten_lop,
                timetable = new List<tkb_theo_lop>(),
                ds_chua_xep = new List<tkb_theo_lop>()
            };

            foreach (var ca in dsCa)
            {
                foreach (var ngay in dsNgay)
                {
                    foreach (var tietEnum in dsTietEnum)
                    {
                        // Tìm tiết học thực tế
                        var tietHoc = tiet.FirstOrDefault(t =>
                            t.Id_ca == ca.Id &&
                            t.Ngay == (int)ngay &&
                            t.Tiet == (int)tietEnum);

                        // Kiểm tra có trong danh sách tránh xếp không
                        bool isBreak = tietTranhXep.Any(tx =>
                            tx.Id_ca == ca.Id &&
                            tx.Ngay == (int)ngay &&
                            tx.Tiet == (int)tietEnum);

                        // Tạo tkb_theo_lop item
                        var tietItem = new tkb_theo_lop
                        {
                            Id_chitiet = tietHoc?.Id ?? 0,
                            Id_don_vi = idDonvi,
                            Id_tkb = idtkb,
                            Id_ca = ca.Id,
                            Ngay = (int)ngay,
                            Tiet = (int)tietEnum,
                            Tiet_thu_may = tietHoc?.Tiet_thu_may ?? 0
                        };

                        if (isBreak)
                        {
                            // Tiết tránh xếp - để trống thông tin môn học
                            tietItem.Id_mon = 0;
                            tietItem.Ten_mon = "";
                            tietItem.Id_giao_vien = 0;
                            tietItem.Ten_giao_vien = "";
                            tietItem.Id_phong = 0;
                            tietItem.Ten_phong = "";
                            tietItem.isLock = false;
                            tietItem.isDrag = false;
                            tietItem.isRest = true;

                            result.timetable.Add(tietItem);
                        }
                        else if (tietHoc != null)
                        {
                            // Tiết có môn học
                            tietItem.Id_mon = tietHoc.Id_mon ?? 0;
                            tietItem.Ten_mon = tietHoc.Ten_mon ?? "";
                            tietItem.Id_giao_vien = tietHoc.Id_giao_vien ?? 0;
                            tietItem.Ten_giao_vien = tietHoc.Ten_giao_vien ?? "";
                            tietItem.Id_phong = tietHoc.Id_phong ?? 0;
                            tietItem.Ten_phong = tietHoc.Ten_phong ?? "Không cần phòng";
                            tietItem.isLock = tietHoc.Khoa;
                            tietItem.isDrag = false;
                            tietItem.isRest = false;

                            result.timetable.Add(tietItem);
                        }
                        else
                        {
                            // Tiết trống - chỉ thêm nếu cần hiển thị full grid
                            tietItem.Id_mon = 0;
                            tietItem.Ten_mon = "";
                            tietItem.Id_giao_vien = 0;
                            tietItem.Ten_giao_vien = "";
                            tietItem.Id_phong = 0;
                            tietItem.Ten_phong = "";
                            tietItem.isLock = false;
                            tietItem.isDrag = false;
                            tietItem.isRest = false;
                            result.timetable.Add(tietItem);
                        }
                    }
                }
            }

            // Xử lý ds_chua_xep (các tiết chưa có thời gian cụ thể)
            var tietChuaXep = tiet.Where(t => t.Ngay <= 0 || t.Tiet <= 0).ToList();
            foreach (var t in tietChuaXep)
            {
                var tietItem = new tkb_theo_lop
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
                    Tiet_thu_may = t.Tiet_thu_may,
                    Id_ca = t.Id_ca,
                    Ngay = t.Ngay,
                    Tiet = t.Tiet,
                    isLock = false,
                    isDrag = false,
                    isRest = false
                };

                result.ds_chua_xep.Add(tietItem);
            }

            return result;
        }
        public ObjectTiet_theoGVDto GetTkbByGiaovien(int id_gv, int idtkb)
        {
            _dsTietGoc = List_Object_tiet(idtkb);
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
            int idDonvi = firstTiet.Id_don_vi ?? 0;

            var dsCa = _context.Ca_Donvi.Where(cd => cd.Id_don_vi == 1)
                             .Join(_context.DM_Cahoc,
                                   cd => cd.Id_ca_hoc,
                                   ca => ca.Id,
                                   (cd, ca) => new
                                   {
                                       Id = ca.Id,
                                       Ten = ca.Ten
                                   }).ToList();

            var tietTranhXep = _context.Giaovien_Tiettranhxep.Where(c => c.Id_giao_vien == id_gv).ToList();

            var dsNgay = Enum.GetValues<Ngay>().ToList();
            var dsTietEnum = Enum.GetValues<Tiet>().ToList();

            var result = new ObjectTiet_theoGVDto
            {
                Id_giao_vien = id_gv,
                Ten_giao_vien = firstTiet.Ten_giao_vien,
                timetable = new List<tkb_theo_giaovien>(),
                ds_chua_xep = new List<tkb_theo_giaovien>()
            };

            foreach (var ca in dsCa)
            {
                foreach (var ngay in dsNgay)
                {
                    foreach (var tietEnum in dsTietEnum)
                    {
                        // Tìm tiết học thực tế
                        var tietHoc = tiet.FirstOrDefault(t =>
                            t.Id_ca == ca.Id &&
                            t.Ngay == (int)ngay &&
                            t.Tiet == (int)tietEnum);

                        // Kiểm tra có trong danh sách tránh xếp không
                        bool isBreak = tietTranhXep.Any(tx =>
                            tx.Id_ca == ca.Id &&
                            tx.Ngay == (int)ngay &&
                            tx.Tiet == (int)tietEnum);

                        // Tạo tkb_theo_lop item
                        var tietItem = new tkb_theo_giaovien
                        {
                            Id_chitiet = tietHoc?.Id ?? 0,
                            Id_don_vi = idDonvi,
                            Id_tkb = idtkb,
                            Id_ca = ca.Id,
                            Ngay = (int)ngay,
                            Tiet = (int)tietEnum,
                            Tiet_thu_may = tietHoc?.Tiet_thu_may ?? 0
                        };

                        if (isBreak)
                        {
                            // Tiết tránh xếp - để trống thông tin môn học
                            tietItem.Id_mon = 0;
                            tietItem.Ten_mon = "";
                            tietItem.Id_lop = 0;
                            tietItem.Ten_lop = "";
                            tietItem.Id_phong = 0;
                            tietItem.Ten_phong = "";
                            tietItem.isLock = false;
                            tietItem.isDrag = false;
                            tietItem.isRest = true;

                            result.timetable.Add(tietItem);
                        }
                        else if (tietHoc != null)
                        {
                            // Tiết có môn học
                            tietItem.Id_mon = tietHoc.Id_mon ?? 0;
                            tietItem.Ten_mon = tietHoc.Ten_mon ?? "";
                            tietItem.Id_lop = tietHoc.Id_lop ?? 0;
                            tietItem.Ten_lop = tietHoc.Ten_lop ?? "";
                            tietItem.Id_phong = tietHoc.Id_phong ?? 0;
                            tietItem.Ten_phong = tietHoc.Ten_phong ?? "Không cần phòng";
                            tietItem.isLock = tietHoc.Khoa;
                            tietItem.isDrag = false;
                            tietItem.isRest = false;

                            result.timetable.Add(tietItem);
                        }
                        else
                        {
                            // Tiết trống - chỉ thêm nếu cần hiển thị full grid
                            tietItem.Id_mon = 0;
                            tietItem.Ten_mon = "";
                            tietItem.Id_lop = 0;
                            tietItem.Ten_lop = "";
                            tietItem.Id_phong = 0;
                            tietItem.Ten_phong = "";
                            tietItem.isLock = false;
                            tietItem.isDrag = false;
                            tietItem.isRest = false;
                            result.timetable.Add(tietItem);
                        }
                    }
                }
            }

            // Xử lý ds_chua_xep (các tiết chưa có thời gian cụ thể)
            var tietChuaXep = tiet.Where(t => t.Ngay <= 0 || t.Tiet <= 0).ToList();
            foreach (var t in tietChuaXep)
            {
                var tietItem = new tkb_theo_giaovien
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
                    Tiet_thu_may = t.Tiet_thu_may,
                    Id_ca = t.Id_ca,
                    Ngay = t.Ngay,
                    Tiet = t.Tiet,
                    isLock = false,
                    isDrag = false,
                    isRest = false
                };

                result.ds_chua_xep.Add(tietItem);
            }

            return result;
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

                var dsTietGoc = List_Object_tiet(idTkb);
                if (dsTietGoc == null || dsTietGoc.Count == 0)
                {
                    return new ObjectTiet_theoLopDto();
                }

                var tkbBase = GetTkbByLop(idLop, idTkb);
                if (tkbBase == null)
                {
                    return new ObjectTiet_theoLopDto();
                }

                var dsViTriXepDuoc = new List<(int Ngay, int Tiet)>();

                // TH1: objectTiet_DaChon có đủ thông tin
                if (idMon > 0)
                {
                    var tietGoc = dsTietGoc.FirstOrDefault(t => t.Id == idChitiet);
                    if (tietGoc != null)
                    {
                        TimViTriXepDuoc_Lop(tietGoc, idDonvi);
                        if (tietGoc.Ds_vi_tri_xep_duoc != null)
                        {
                            foreach (var viTri in tietGoc.Ds_vi_tri_xep_duoc)
                            {
                                dsViTriXepDuoc.Add((viTri.Ngay, viTri.Tiet));
                            }
                        }
                    }
                }
                // TH2: objectTiet_DaChon chỉ có thông tin cơ bản
                else
                {
                    var cacTietCuaLop = dsTietGoc.Where(t => t.Id_lop == idLop && t.Id_ca == idCa).ToList();
                    foreach (var tietGoc in cacTietCuaLop)
                    {
                        TimViTriXepDuoc_Lop(tietGoc, idDonvi);
                        if (tietGoc.Ds_vi_tri_xep_duoc != null && tietGoc.Ds_vi_tri_xep_duoc.Count > 0)
                        {
                            bool coViTriTrung = tietGoc.Ds_vi_tri_xep_duoc.Any(viTri =>
                                viTri.Ngay == ngay && viTri.Tiet == tietSo);

                            if (coViTriTrung)
                            {
                                dsViTriXepDuoc.Add((tietGoc.Ngay, tietGoc.Tiet));
                            }
                        }
                    }
                }

                // Cập nhật isDrag cho các tiết trong tkbBase
                foreach (var tietInTimetable in tkbBase.timetable)
                {
                    bool isDragable = tietInTimetable.Id_ca == idCa &&
                                      dsViTriXepDuoc.Any(vt =>
                                          vt.Ngay == tietInTimetable.Ngay &&
                                          vt.Tiet == tietInTimetable.Tiet);

                    bool isSelectedTiet = tietInTimetable.Id_chitiet == idChitiet &&
                                          tietInTimetable.Ngay == ngay &&
                                          tietInTimetable.Tiet == tietSo;

                    tietInTimetable.isDrag = isDragable || isSelectedTiet;
                }

                return tkbBase;
            }
            catch (Exception ex)
            {
                return new ObjectTiet_theoLopDto();
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

                var dsTietGoc = List_Object_tiet(idTkb);
                if (dsTietGoc == null || dsTietGoc.Count == 0)
                {
                    return new ObjectTiet_theoGVDto();
                }

                var tkbBase = GetTkbByGiaovien(idGV, idTkb);
                if (tkbBase == null)
                {
                    return new ObjectTiet_theoGVDto();
                }

                var dsViTriXepDuoc = new List<(int Ngay, int Tiet)>();

                // TH1: objectTiet_DaChon có đủ thông tin
                if (idMon > 0)
                {
                    var tietGoc = dsTietGoc.FirstOrDefault(t => t.Id == idChitiet);
                    if (tietGoc != null)
                    {
                        TimViTriXepDuoc_GV(tietGoc, idDonvi);
                        if (tietGoc.Ds_vi_tri_xep_duoc != null && tietGoc.Ds_vi_tri_xep_duoc.Count > 0)
                        {
                            foreach (var viTri in tietGoc.Ds_vi_tri_xep_duoc)
                            {
                                dsViTriXepDuoc.Add((viTri.Ngay, viTri.Tiet));
                            }
                        }
                    }
                }
                // TH2: objectTiet_DaChon chỉ có thông tin cơ bản
                else
                {
                    var cacTietCuaGV = dsTietGoc.Where(t => t.Id_giao_vien == idGV && t.Id_ca == idCa).ToList();
                    foreach (var tietGoc in cacTietCuaGV)
                    {
                        TimViTriXepDuoc_GV(tietGoc, idDonvi);
                        if (tietGoc.Ds_vi_tri_xep_duoc != null)
                        {
                            foreach (var viTri in tietGoc.Ds_vi_tri_xep_duoc)
                            {
                                if (viTri.Ngay == ngay && viTri.Tiet == tietSo)
                                {
                                    dsViTriXepDuoc.Add((viTri.Ngay, viTri.Tiet));
                                }
                            }
                        }
                    }
                }

                // Cập nhật isDrag cho các tiết trong tkbBase
                foreach (var tietInTimetable in tkbBase.timetable)
                {
                    bool isDragable = tietInTimetable.Id_ca == idCa &&
                                      dsViTriXepDuoc.Any(vt =>
                                          vt.Ngay == tietInTimetable.Ngay &&
                                          vt.Tiet == tietInTimetable.Tiet);

                    bool isSelectedTiet = tietInTimetable.Id_chitiet == idChitiet &&
                                          tietInTimetable.Ngay == ngay &&
                                          tietInTimetable.Tiet == tietSo;

                    tietInTimetable.isDrag = isDragable || isSelectedTiet;
                }

                return tkbBase;
            }
            catch (Exception ex)
            {
                return new ObjectTiet_theoGVDto();
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
                int ngay2 = tiet2.Ngay;
                int tietSo2 = tiet2.Tiet;

                // Tạo Object_Tiet từ tiết 1
                var objectTiet1 = new Object_Tiet
                {
                    Id_tkb = tiet1.Id_tkb,
                    Id_lop = tietDachon.Id_lop, 
                    Id_mon = tiet1.Id_mon,
                    Id_giao_vien = tiet1.Id_giao_vien,
                    Id_phong = tiet1.Id_phong,
                    Id_ca = tiet1.Id_ca,
                    Tiet_thu_may = tiet1.Tiet_thu_may
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
                bool check = true;
                if( objectTiet1.Id_mon == 0)
                {
                    check = CheckViTriXepDuoc_Lop(objectTiet1, objectTiet2.Id_ca, objectTiet2.Ngay, objectTiet2.Tiet);
                }
                else if(objectTiet2.Id_mon == 0)
                {
                    check = CheckViTriXepDuoc_Lop(objectTiet2, objectTiet1.Id_ca, objectTiet1.Ngay, objectTiet1.Tiet);
                }
                else
                {
                    var check_t1 = CheckViTriXepDuoc_Lop(objectTiet1, objectTiet2.Id_ca, objectTiet2.Ngay, objectTiet2.Tiet);
                    var check_t2 = CheckViTriXepDuoc_Lop(objectTiet2, objectTiet1.Id_ca, objectTiet1.Ngay, objectTiet1.Tiet);
                    if(check_t1 && check_t2)
                    {
                        check = true;
                    }
                    else
                    {
                        check = false;
                    }    
                }

                if (check)
                {
                    // Đổi chỗ
                    bool updateTiet1 = UpdateTiet(objectTiet1, ngay2, tietSo2);
                    bool updateTiet2 = UpdateTiet(objectTiet2, ngay1, tietSo1);

                    if (!updateTiet1 || !updateTiet2)
                    {
                        return (false, new ObjectTiet_theoLopDto());
                    }

                    var ketQuaCheckViTri = TimViTriXepDuoc_byLop(tietDachon, idDonvi);
                    return (true, ketQuaCheckViTri);
                }

                return (false, new ObjectTiet_theoLopDto());
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
                int ngay2 = tiet2.Ngay;
                int tietSo2 = tiet2.Tiet;

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

                bool check = true;
                if (objectTiet1.Id_mon == 0)
                {
                    check = CheckViTriXepDuoc_GV(objectTiet1, objectTiet2.Id_ca, objectTiet2.Ngay, objectTiet2.Tiet);
                }
                else if (objectTiet2.Id_mon == 0)
                {
                    check = CheckViTriXepDuoc_GV(objectTiet2, objectTiet1.Id_ca, objectTiet1.Ngay, objectTiet1.Tiet);
                }
                else
                {
                    var check_t1 = CheckViTriXepDuoc_GV(objectTiet1, objectTiet2.Id_ca, objectTiet2.Ngay, objectTiet2.Tiet);
                    var check_t2 = CheckViTriXepDuoc_GV(objectTiet2, objectTiet1.Id_ca, objectTiet1.Ngay, objectTiet1.Tiet);
                    if (check_t1 && check_t2)
                    {
                        check = true;
                    }
                    else
                    {
                        check = false;
                    }
                }

                if (check)
                {
                    // Đổi chỗ
                    bool updateTiet1 = UpdateTiet(objectTiet1, ngay2, tietSo2);
                    bool updateTiet2 = UpdateTiet(objectTiet2, ngay1, tietSo1);

                    if (!updateTiet1 || !updateTiet2)
                    {
                        return (false, new ObjectTiet_theoGVDto());
                    }

                    var ketQuaCheckViTri = TimViTriXepDuoc_byGV(tietDachon, idDonvi);
                    return (true, ketQuaCheckViTri);
                }

                return (false, new ObjectTiet_theoGVDto());
            }
            catch (Exception ex)
            {
                return (false, new ObjectTiet_theoGVDto());
            }
        }
    }
}
 