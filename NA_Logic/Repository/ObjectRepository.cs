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
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class ObjectRepository : IObjectRepository
    {
        private readonly NA_DbContext _context;
        public ObjectRepository(NA_DbContext context)
        {
            _context = context;
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


                var result = _context.Set<Chitiet_Thoikhoabieu_List>().FromSqlRaw("EXEC Get_Object @Id_giao_vien = @Id_giao_vien, @Id_tkb = @Id_tkb",
                      paramIdGiaovien, paramIdTkb)
                    .ToList();
                var tietban = _context.Giaovien_Tiettranhxep.Where(c => c.Id_giao_vien == idgv).ToList();
                if (result == null) return null;
                var teacher = new Object_Giaovien
                {
                    Id_don_vi = result[0].Id_don_vi,
                    Ten_don_vi = result[0].Ten_don_vi,
                    Id_giao_vien = result[0].Id_giao_vien,
                    Ten_giao_vien = result[0].Ten_giao_vien,
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
                var tietban = _context.Tiet_ban.Where(c => c.Id_phong == idph).ToList();
                if (result == null) return null;
                var room = new Object_Phonghoc
                {
                    Id_don_vi = result[0].Id_don_vi,
                    Ten_don_vi = result[0].Ten_don_vi,
                    Id_phong = result[0].Id_phong,  
                    Ten_phong = result[0].Ten_phong,
                    ds_mon_tai_phong = new List<Ds_mon>(),
                    ds_tiet_tranh_xep = new List<Ds_tiet_tranh_xep>()
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
                             (mh, dv) => new { Id_mon = mh.Id, Ten_mon = mh.Ten, Id_don_vi = dv.Id, Ten_don_vi = dv.TenDonvi })
                             .FirstOrDefault(mh => mh.Id_mon == idmon && mh.Id_don_vi == idDonvi);
                var tietcodinh = _context.Tiet_co_dinh.Where(c => c.Id_mon == idmon).ToList();
                var tietban = _context.Tiet_Tranh_Xep.Where(c => c.Id_mon == idmon).ToList();

                var subject = new Object_Monhoc
                {
                    Id_don_vi = monhoc.Id_don_vi,
                    Ten_don_vi = monhoc.Ten_don_vi,
                    Id_mon = monhoc.Id_mon,
                    Ten_mon = monhoc.Ten_mon,
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
        public Object_Tohopmon Object_tohopmon(int idthm, int idDonvi)
        {
            try
            {
                var paramIdDonvi = new SqlParameter("idDonvi", SqlDbType.Int)
                {
                    Value = idDonvi
                };
                var paramIdTohopmon = new SqlParameter("id_to_hop_mon", SqlDbType.Int)
                {
                    Value = idthm
                };
                var result = _context.Set<Monhoc_Tohopmon_List>().FromSqlRaw("EXEC MonTohop_GetList_Paging  @idDonvi = @idDonvi, @id_to_hop_mon = @id_to_hop_mon",
                                paramIdDonvi, paramIdTohopmon).ToList();
                if (result == null) result = new List<Monhoc_Tohopmon_List>();

                var tohopmon = new Object_Tohopmon
                {
                    Id_don_vi = result[0].Id_don_vi,
                    ds_mon = new List<Ds_mon>(),
                    So_tiet_toi_da_1_ca = result[0].So_tiet_toi_da_1_ca,
                    So_tiet_toi_da_2_ca = result[0].So_tiet_toi_da_2_ca
                };
                foreach (var r in result)
                {
                    if (r.Id_mon_1 > 0 && !string.IsNullOrEmpty(r.Ten_mon_hoc_1))
                    {
                        tohopmon.ds_mon.Add(new Ds_mon
                        {
                            Id_mon = r.Id_mon_1,
                            Ten_mon = r.Ten_mon_hoc_1
                        });
                    }

                    if (r.Id_mon_2 > 0 && !string.IsNullOrEmpty(r.Ten_mon_hoc_2))
                    {
                        tohopmon.ds_mon.Add(new Ds_mon
                        {
                            Id_mon = r.Id_mon_2,
                            Ten_mon = r.Ten_mon_hoc_2
                        });
                    }

                    if (r.Id_mon_3 > 0 && !string.IsNullOrEmpty(r.Ten_mon_hoc_3))
                    {
                        tohopmon.ds_mon.Add(new Ds_mon
                        {
                            Id_mon = r.Id_mon_3,
                            Ten_mon = r.Ten_mon_hoc_3
                        });
                    }
                }
                ;
                return tohopmon;

            }
            catch (Exception)
            {
                return null;
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
                    ds_tiet_tranh_xep_lop_mon = new List<Ds_tiet_tranh_xep_lop_mon>()
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
                lop.ds_tiet_tranh_xep_lop_mon = tietban_lopmon.GroupBy(x => x.Id_mon).Select(g => new Ds_tiet_tranh_xep_lop_mon
                                                {
                                                    Id_mon = g.Key,
                                                    Ds_tiet_tranh_xep_monlop = g.Select(item => new Ds_tiet_tranh_xep
                                                    {
                                                        Id_ca = item.Id_ca,
                                                        Tiet = item.Tiet,
                                                        Ngay = item.Ngay
                                                    }).ToList()
                                                }).ToList();

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
        public bool Check_gv(int Ngay, int Tiet, int Ca, int id_giaovien, int id_tkb)
        {
            var object_gv = Object_giaovien(id_giaovien, id_tkb);
            if (object_gv == null)
            {
                return false;
            }

            bool check_tietban = object_gv.ds_tiet_tranh_xep.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);

            return check_tietban;
        }
        public bool Check_phong(int Ngay, int Tiet, int Ca, int idphong, int id_tkb)
        {
            var object_phong = Object_phonghoc(idphong, id_tkb);
            if (object_phong == null)
            {
                return false;
            }

            bool check_tietban = object_phong.ds_tiet_tranh_xep.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);

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
        public bool Check_lop(int Ngay, int Tiet, int Ca, int idlop, int id_tkb)
        {
            var object_lop = Object_lophoc(idlop, id_tkb);
            if (object_lop == null)
            {
                return false;
            }

            bool check_tietban = object_lop.ds_tiet_tranh_xep.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet);
            bool check_tietban_lopmon = object_lop.ds_tiet_tranh_xep_lop_mon.Any(x => x.Ds_tiet_tranh_xep_monlop.Any(t => t.Id_ca == Ca && t.Ngay == Ngay && t.Tiet == Tiet));
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
 