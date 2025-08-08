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
using System.Drawing.Printing;
using System.Linq;
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

                // Group by Id_to_hop_mon để tạo từng Object_Tohopmon riêng biệt
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

                    // Lấy tất cả môn trong tổ hợp này
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
            if (object_gv.Chi_day_mot_buoi == true)
            {
                var tiet_dau_trong_ngay = object_gv.ds_tiet_da_xep.FirstOrDefault(t => t.Ngay == Ngay);
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
        public bool Tiet_co_dinh(int Ngay, int Tiet, int Ca, int idmon, int idlop, int id_tkb)
        {
            var khoi = _context.DM_Lophoc.Join(_context.DM_Khoilop, lh => lh.Id_khoi, kl => kl.Id,
                            (lh, kl) => new { Id_khoi = kl.Id, Ten_khoi = kl.Ten, Id_lop = lh.Id, Id_ban = lh.Id_ban }).FirstOrDefault(x => x.Id_lop == idlop);
            var object_mon = Object_monhoc(idmon, id_tkb);
            bool check_tiet_co_dinh = object_mon.ds_tiet_co_dinh.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet && t.Id_khoi == khoi.Id_khoi);
            return check_tiet_co_dinh;
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
                        int so_tiet_1_mon_1_ca = ds_da_xep.Where(c => c.Id_lop == idlop && c.Id_mon == mon.Id_mon && c.Id_ca == ca.Id_ca_hoc).Count();
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
    }
}
 