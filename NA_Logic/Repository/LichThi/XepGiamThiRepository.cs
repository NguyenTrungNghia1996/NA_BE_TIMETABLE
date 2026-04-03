using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.IRepository.LichThi;
using NA_Logic.IRepository.XepGiamThi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class XepGiamThiRepository : IXepGiamThiRepository
    {
        private readonly NA_DbContext _dbContext;
        private readonly List<DM_Giamthi> _dsGiamThi = new List<DM_Giamthi>();
        private readonly List<DM_Phongthi> _dsPhongThi = new List<DM_Phongthi>();
        private readonly List<Phongthi_Thisinh> _dsPhongThiThiSinh = new List<Phongthi_Thisinh>();
        private readonly List<Giaovien_Monhoc> _dsPhanCongGV = new List<Giaovien_Monhoc>();

        public XepGiamThiRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void LoadAllInformation(int idLich)
        {
            try
            {

                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "GetAll_Infomation_ToXepLichThi";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Thêm tham số
                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@idLich";
                    p1.Value = idLich;
                    cmd.Parameters.Add(p1);

                    _dbContext.Database.OpenConnection();

                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            _dsPhongThi.Add(new DM_Phongthi
                            {
                                Id = (int)reader["Id"],
                                Id_diem_thi = (int)reader["Id_diem_thi"],
                                Toa = reader["Toa"] != DBNull.Value ? (string)reader["Toa"] : "",
                                Tang = (int)reader["Tang"],
                                So_phong = (int)reader["So_phong"],
                            });
                        }

                        reader.NextResult();
                        while (reader.Read())
                        {
                            _dsGiamThi.Add(new DM_Giamthi
                            {
                                Id = (int)reader["Id"],
                                Ma = reader["Ma"] != DBNull.Value ? (string)reader["Ma"] : "",
                                Ho_va_ten = reader["Ho_va_ten"] != DBNull.Value ? (string)reader["Ho_va_ten"] : "",
                                Id_diem_thi = (int)reader["Id_diem_thi"],
                                Id_giao_vien = reader["Id_giao_vien"] != DBNull.Value ? (int?)reader["Id_giao_vien"] : null,
                            });
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            _dsPhongThiThiSinh.Add(new Phongthi_Thisinh
                            {
                                Id = (int)reader["Id"],
                                Id_phong = (int)reader["Id_phong"],
                                Id_thi_sinh = (int)reader["Id_thi_sinh"],
                                Mon_1 = reader["Mon_1"] != DBNull.Value ? (int?)reader["Mon_1"] : null,
                                Mon_2 = reader["Mon_2"] != DBNull.Value ? (int?)reader["Mon_2"] : null,
                            });
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            _dsPhanCongGV.Add(new Giaovien_Monhoc
                            {
                                Id = (int)reader["Id"],
                                Id_giao_vien = (int)reader["Id_giao_vien"],
                                Id_mon = (int)reader["Id_mon"],
                            });
                        }
                    }
                }

            }
            catch (Exception)
            {
                return;
            }
        }

        public bool XepGiamThi(int idLich)
        {
            try
            {
                var lich = _dbContext.DM_Lichthi.FirstOrDefault(x => x.Id == idLich);
                if (lich == null)
                    return false;

                var diemThi = _dbContext.DM_Diemthi.FirstOrDefault(x => x.Id == lich.Id_diem_thi);
                if (diemThi == null)
                    return false;

                int soGiamThiPhong = diemThi.So_giam_thi_1_phong;
                int? soPhongGiamSat = diemThi.So_phong_giam_sat_toi_da;

                LoadAllInformation(idLich);

                var nhomPhongGiamSat = new List<List<DM_Phongthi>>();

                var groupByTang = _dsPhongThi.GroupBy(x => new { x.Toa, x.Tang });
                if (diemThi.Co_giam_sat && soPhongGiamSat != null && soPhongGiamSat > 0)
                {
                    foreach (var tang in groupByTang)
                    {
                        var danhSachPhongTrongTang = tang.ToList();

                        for (int i = 0; i < danhSachPhongTrongTang.Count; i += soPhongGiamSat ?? 0)
                        {
                            var nhom = danhSachPhongTrongTang.Skip(i).Take(soPhongGiamSat ?? 0).ToList();
                            nhomPhongGiamSat.Add(nhom);
                        }
                    }
                }

                int tongGiamSatCan = diemThi.Co_giam_sat ? nhomPhongGiamSat.Count : 0;
                int tongGiamThiThuongCan = _dsPhongThi.Count * soGiamThiPhong;
                int tongCan = tongGiamSatCan + tongGiamThiThuongCan;

                int soGiamThiPhongAo = _dsGiamThi.Count - tongCan;
                bool coPhongAo = _dsGiamThi.Count > tongCan;
                var phongAo = coPhongAo ? new DM_Phongthi { Id = -1, Toa = "AO", Tang = -1 } : null;

                var monTheoPhong = new Dictionary<int, List<int>>();
                foreach (var phong in _dsPhongThi)
                {
                    List<int> danhSachMon;
                    if (lich.Bai_thi_tu_chon)
                    {
                        danhSachMon = _dsPhongThiThiSinh.Where(x => x.Id_phong == phong.Id).SelectMany(x => new[] { x.Mon_1, x.Mon_2 })
                                .Where(x => x != null && x != 0).Select(x => x!.Value).Distinct().ToList();
                    }
                    else
                    {
                        danhSachMon = lich.Id_mon != null
                            ? new List<int> { lich.Id_mon.Value }
                            : new List<int>();
                    }
                    monTheoPhong[phong.Id] = danhSachMon;
                }

                var phanCong = _dsPhanCongGV.GroupBy(x => x.Id_giao_vien).ToDictionary(x => x.Key, x => x.Select(p => p.Id_mon).ToList());

                var random = new Random();
                var ketQua = new List<Chitiet_Lichthi>();
                var giamThiConLai = _dsGiamThi.ToList();
                var phongConLai = _dsPhongThi.ToList();
                var soGiamThiTheoPhong = _dsPhongThi.ToDictionary(x => x.Id, x => 0);
                var nhomCoGiamSat = new HashSet<int>();
                if (coPhongAo)
                {
                    phongConLai.Add(phongAo);
                    soGiamThiTheoPhong[-1] = 0;
                }
                while (giamThiConLai.Any())
                {
                    var phongTheoGiamThi = new List<PhongTheoGiamThi>();

                    foreach (var gt in giamThiConLai)
                    {
                        var monGiaoVien = new List<int>();
                        if (gt.Id_giao_vien != null && phanCong.ContainsKey((int)gt.Id_giao_vien))
                            monGiaoVien = phanCong[(int)gt.Id_giao_vien];

                        var phongXepDuoc = new List<DM_Phongthi>();
                        foreach (var phong in phongConLai)
                        {
                            if (!monTheoPhong.ContainsKey(phong.Id))
                            {
                                phongXepDuoc.Add(phong);
                                continue;
                            }

                            var monPhong = monTheoPhong[phong.Id];
                            bool coTrungMon = monPhong.Any(m => monGiaoVien.Contains(m));
                            if (!coTrungMon)
                                phongXepDuoc.Add(phong);
                        }

                        phongTheoGiamThi.Add(new PhongTheoGiamThi
                        {
                            GiamThi = gt,
                            PhongCoThe = phongXepDuoc
                        });
                    }

                    int soItNhat = phongTheoGiamThi.Min(x => x.PhongCoThe.Count);

                    var ungVien = phongTheoGiamThi.Where(x => x.PhongCoThe.Count == soItNhat).ToList();

                    var chon = ungVien[random.Next(ungVien.Count)];
                    var giamThiDuocChon = chon.GiamThi;
                    var phongCoThe = chon.PhongCoThe;

                    int loai;
                    if (diemThi.Co_giam_sat || tongGiamSatCan == 0)
                        loai = random.Next(1, soGiamThiPhong + 2);
                    else
                        loai = random.Next(2, soGiamThiPhong + 2);

                    if (loai == 1)
                    {
                        var nhomChuaCoGiamSat = nhomPhongGiamSat.Select((nhom, index) => new { nhom, index }).Where(x => !nhomCoGiamSat.Contains(x.index)).ToList();

                        if (nhomChuaCoGiamSat.Any())
                        {
                            var nhomChon = nhomChuaCoGiamSat[random.Next(nhomChuaCoGiamSat.Count)];
                            nhomCoGiamSat.Add(nhomChon.index);
                            foreach (var phong in nhomChon.nhom)
                            {
                                ketQua.Add(new Chitiet_Lichthi
                                {
                                    Id_lich = idLich,
                                    Id_phong = phong.Id,
                                    Id_giam_thi = giamThiDuocChon.Id,
                                    Loai_giam_thi = loai,
                                    La_phong_cho = false
                                });
                            }
                        }
                        else
                            loai = random.Next(2, soGiamThiPhong + 2);
                    }

                    if (loai != 1)
                    {
                        DM_Phongthi phongDuocChon = null;

                        if (phongCoThe.Any())
                            phongDuocChon = phongCoThe[random.Next(phongCoThe.Count)];
                        if (phongDuocChon != null)
                        {
                            if (phongDuocChon.Id == -1)
                            {
                                ketQua.Add(new Chitiet_Lichthi
                                {
                                    Id_lich = idLich,
                                    Id_phong = null,
                                    Id_giam_thi = giamThiDuocChon.Id,
                                    Loai_giam_thi = loai,
                                    La_phong_cho = true
                                });
                                soGiamThiTheoPhong[-1]++;
                                if (soGiamThiTheoPhong[-1] >= soGiamThiPhongAo)
                                    phongConLai.Remove(phongAo);
                            }
                            else
                            {
                                ketQua.Add(new Chitiet_Lichthi
                                {
                                    Id_lich = idLich,
                                    Id_phong = phongDuocChon.Id,
                                    Id_giam_thi = giamThiDuocChon.Id,
                                    Loai_giam_thi = loai,
                                    La_phong_cho = false
                                });

                                soGiamThiTheoPhong[phongDuocChon.Id]++;

                                if (soGiamThiTheoPhong[phongDuocChon.Id] >= soGiamThiPhong)
                                    phongConLai.Remove(phongDuocChon);
                            }
                        }
                    }

                    giamThiConLai.Remove(giamThiDuocChon);
                }

                var dataCu = _dbContext.Chitiet_Lichthi.Where(x => x.Id_lich == idLich).ToList();
                _dbContext.BulkDelete(dataCu);
                _dbContext.BulkInsert(ketQua);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool HuyKetQua(int idLich)
        {
            try
            {
                var dataCu = _dbContext.Chitiet_Lichthi.Where(x => x.Id_lich == idLich).ToList();
                _dbContext.BulkDelete(dataCu);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
