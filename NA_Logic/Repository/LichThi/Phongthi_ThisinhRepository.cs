using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository.LichThi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository.LichThi
{
    public class Phongthi_ThisinhRepository: IPhongthi_ThisinhRepository
    {
        private readonly NA_DbContext _context;
        public Phongthi_ThisinhRepository(NA_DbContext context) { 
            _context = context;
        }
        public (bool isValid, string message) ValidateSoPhongTheoDiemThi(int idDiemThi)
        {
            var diemThi = _context.DM_Diemthi.FirstOrDefault(x => x.Id == idDiemThi);
            if (diemThi == null)
                return (false, "Điểm thi không tồn tại");

            int sucChua = (int)diemThi.So_thi_sinh_1_phong;

            var danhSachThisinh = _context.DM_Thisinh.Where(x => x.Id_diem_thi == idDiemThi && x.So_bao_danh != null).ToList();

            if (!danhSachThisinh.Any())
                return (false, "Chưa có thí sinh nào");

            var culture = new CultureInfo("vi-VN");
            var comparer = StringComparer.Create(culture, ignoreCase: true);

            int phongIndex = 0;

            var nhom2Mon = danhSachThisinh.Where(x => x.Mon_thi_1 != null && x.Mon_thi_2 != null).GroupBy(x => (x.Mon_thi_1, x.Mon_thi_2))
                .OrderByDescending(x => x.Count()).ToList();

            foreach (var nhom in nhom2Mon)
            {
                int i = 0;
                while (i < nhom.Count())
                {
                    phongIndex++;
                    i += sucChua;
                }
            }

            var nhom1Mon = danhSachThisinh.Where(x => x.Mon_thi_1 == null || x.Mon_thi_2 == null)
                .GroupBy(x => x.Mon_thi_1 ?? x.Mon_thi_2).Select(x => new { Mon = x.Key, Count = x.Count() }).ToList();

            var duSauBuoc1 = new List<(int? Mon, int Count)>();

            foreach (var nhom in nhom1Mon)
            {
                int i = 0;
                while (nhom.Count - i >= sucChua)
                {
                    phongIndex++;
                    i += sucChua;
                }
                int du = nhom.Count - i;
                if (du > 0)
                    duSauBuoc1.Add((nhom.Mon, du));
            }

            duSauBuoc1 = duSauBuoc1.OrderByDescending(x => x.Count).ToList();

            while (duSauBuoc1.Count > 0)
            {
                var mon1 = duSauBuoc1[0];
                duSauBuoc1.RemoveAt(0);

                if (duSauBuoc1.Count > 0)
                {
                    var mon2 = duSauBuoc1[0];
                    duSauBuoc1.RemoveAt(0);

                    int layMon1 = Math.Min(mon1.Count, sucChua);
                    int layMon2 = Math.Min(mon2.Count, sucChua - layMon1);

                    phongIndex++;

                    int duMon2 = mon2.Count - layMon2;
                    if (duMon2 > 0)
                    {
                        duSauBuoc1.Add((mon2.Mon, duMon2));
                        duSauBuoc1 = duSauBuoc1.OrderByDescending(x => x.Count).ToList();
                    }
                }
                else
                {
                    int i = 0;
                    while (i < mon1.Count)
                    {
                        phongIndex++;
                        i += sucChua;
                    }
                }
            }

            int soPhongHienCo = _context.DM_Phongthi.Count(x => x.Id_diem_thi == idDiemThi);

            if (soPhongHienCo < phongIndex)
                return (false, $"Không đủ phòng thi, cần {phongIndex} phòng nhưng hiện chỉ có {soPhongHienCo} phòng");

            return (true, "Đủ phòng thi");
        }
        public (bool isValid, string message) ValidateSoPhongTheoHoiDong(int idHoiDong)
        {
            var diemThiList = _context.DM_Diemthi.Where(x => x.Id_hoi_dong == idHoiDong).ToList();

            if (!diemThiList.Any())
                return (false, "Hội đồng không có điểm thi nào");

            foreach (var diemThi in diemThiList)
            {
                var (isValid, message) = ValidateSoPhongTheoDiemThi(diemThi.Id);
                if (!isValid)
                    return (false, $"Điểm thi {diemThi.Ten}: {message}");
            }

            return (true, "Đủ phòng thi");
        }
        public bool XepPhongTheoHoiDong(int idHoiDong)
        {
            var diemThiList = _context.DM_Diemthi.Where(x => x.Id_hoi_dong == idHoiDong).Select(x => x.Id).ToList();

            foreach (var idDiemThi in diemThiList)
                XepPhongTheoDiemThi(idDiemThi);

            return true;
        }

        public bool XepPhongTheoDiemThi(int idDiemThi)
        {
            try
            {
                var culture = new CultureInfo("vi-VN");
                var comparer = StringComparer.Create(culture, ignoreCase: true);

                var diemThi = _context.DM_Diemthi.FirstOrDefault(x => x.Id == idDiemThi);
                if (diemThi == null) 
                    return false;

                var danhSachPhong = _context.DM_Phongthi.Where(x => x.Id_diem_thi == idDiemThi).OrderBy(x => x.So_phong).ToList();

                if (!danhSachPhong.Any()) 
                    return false;

                var danhSachThisinh = _context.DM_Thisinh.Where(x => x.Id_diem_thi == idDiemThi && x.So_bao_danh != null).ToList();

                if (!danhSachThisinh.Any()) 
                    return false;

                var ketQua = new List<Phongthi_Thisinh>();
                int phongIndex = 0;
                int sucChua = (int)diemThi.So_thi_sinh_1_phong;

                var nhom2Mon = danhSachThisinh.Where(x => x.Mon_thi_1 != null && x.Mon_thi_2 != null).GroupBy(x => (x.Mon_thi_1, x.Mon_thi_2))
                    .OrderByDescending(x => x.Count()).ToList();

                foreach (var nhom in nhom2Mon)
                {
                    var sorted = nhom.OrderBy(x => x.Ho_va_ten.Split(' ').Last(), comparer)
                        .ThenBy(x => string.Join(" ", x.Ho_va_ten.Split(' ').SkipLast(1)), comparer).ToList();

                    int i = 0;
                    while (i < sorted.Count)
                    {
                        if (phongIndex >= danhSachPhong.Count) break;

                        var batch = sorted.Skip(i).Take(sucChua).ToList();
                        foreach (var ts in batch)
                        {
                            ketQua.Add(new Phongthi_Thisinh
                            {
                                Id_thi_sinh = ts.Id,
                                Id_phong = danhSachPhong[phongIndex].Id,
                                Mon_1 = ts.Mon_thi_1,
                                Mon_2 = ts.Mon_thi_2
                            });
                        }
                        phongIndex++;
                        i += sucChua;
                    }
                }

                var nhom1Mon = danhSachThisinh.Where(x => x.Mon_thi_1 == null || x.Mon_thi_2 == null).GroupBy(x => x.Mon_thi_1 ?? x.Mon_thi_2)
                    .Select(x => new
                    {
                        Mon = x.Key,
                        DanhSach = x.OrderBy(t => t.Ho_va_ten.Split(' ').Last(), comparer)
                                    .ThenBy(t => string.Join(" ", t.Ho_va_ten.Split(' ').SkipLast(1)), comparer)
                                    .ToList()
                    }).ToList();

                var duSauBuoc1 = new List<(int? Mon, List<DM_Thisinh> DanhSach)>();

                foreach (var nhom in nhom1Mon)
                {
                    var list = nhom.DanhSach.ToList();

                    int i = 0;
                    while (list.Count - i >= sucChua && phongIndex < danhSachPhong.Count)
                    {
                        var batch = list.Skip(i).Take(sucChua).ToList();
                        foreach (var ts in batch)
                            ketQua.Add(new Phongthi_Thisinh
                            {
                                Id_thi_sinh = ts.Id,
                                Id_phong = danhSachPhong[phongIndex].Id,
                                Mon_1 = ts.Mon_thi_1,
                                Mon_2 = ts.Mon_thi_2
                            });
                        phongIndex++;
                        i += sucChua;
                    }
                    var du = list.Skip(i).ToList();
                    if (du.Any())
                        duSauBuoc1.Add((nhom.Mon, du));
                }

                duSauBuoc1 = duSauBuoc1.OrderByDescending(x => x.DanhSach.Count).ToList();

                while (duSauBuoc1.Count > 0 && phongIndex < danhSachPhong.Count)
                {
                    var mon1 = duSauBuoc1[0];
                    duSauBuoc1.RemoveAt(0);

                    if (duSauBuoc1.Count > 0)
                    {
                        var mon2 = duSauBuoc1[0];
                        duSauBuoc1.RemoveAt(0);
                        int layMon1 = Math.Min(mon1.DanhSach.Count, sucChua);
                        int layMon2 = Math.Min(mon2.DanhSach.Count, sucChua - layMon1);

                        foreach (var ts in mon1.DanhSach.Take(layMon1))
                            ketQua.Add(new Phongthi_Thisinh
                            {
                                Id_thi_sinh = ts.Id,
                                Id_phong = danhSachPhong[phongIndex].Id,
                                Mon_1 = mon1.Mon,
                                Mon_2 = null
                            });

                        foreach (var ts in mon2.DanhSach.Take(layMon2))
                            ketQua.Add(new Phongthi_Thisinh
                            {
                                Id_thi_sinh = ts.Id,
                                Id_phong = danhSachPhong[phongIndex].Id,
                                Mon_1 = null,
                                Mon_2 = mon2.Mon,
                            });
                        phongIndex++;

                        var duMon2 = mon2.DanhSach.Skip(layMon2).ToList();
                        if (duMon2.Any())
                        {
                            duSauBuoc1.Add((mon2.Mon, duMon2));
                            duSauBuoc1 = duSauBuoc1.OrderByDescending(x => x.DanhSach.Count).ToList();
                        }
                    }
                    else
                    {
                        int i = 0;
                        while (i < mon1.DanhSach.Count && phongIndex < danhSachPhong.Count)
                        {
                            var batch = mon1.DanhSach.Skip(i).Take(sucChua).ToList();
                            foreach (var ts in batch)
                                ketQua.Add(new Phongthi_Thisinh { Id_thi_sinh = ts.Id, Id_phong = danhSachPhong[phongIndex].Id, Mon_1 = ts.Mon_thi_1, Mon_2 = ts.Mon_thi_2 });
                            phongIndex++;
                            i += sucChua;
                        }
                    }
                }

                var idPhongList = danhSachPhong.Select(x => x.Id).ToList();
                var dataCu = _context.Phongthi_Thisinh.Where(x => idPhongList.Contains(x.Id_phong)).ToList();
                _context.BulkDelete(dataCu);
                _context.BulkInsert(ketQua);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckPhongCoThiSinh(int idDiemThi)
        {
            return _context.DM_Phongthi
                .Any(p => p.Id_diem_thi == idDiemThi
                    && _context.Phongthi_Thisinh.Any(pt => pt.Id_phong == p.Id));
        }
        public bool Delete(int id)
        {
            try
            {
                var data = _context.Phongthi_Thisinh.FirstOrDefault(c => c.Id == id);
                if(data == null)
                    return false;
                _context.Phongthi_Thisinh.Remove(data);
                _context.SaveChanges();
                return true;

            }
            catch (Exception) { 
                return false;
            }
        }
    }
}
