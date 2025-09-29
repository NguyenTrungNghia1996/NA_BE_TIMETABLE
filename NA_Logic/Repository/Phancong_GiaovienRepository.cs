using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
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
using System.IO.Pipelines;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Phancong_GiaovienRepository : IPhancong_GiaovienRepository
    {
        private readonly NA_DbContext _dbContext;
        public Phancong_GiaovienRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<PhancongGVDto> GetList_Paging( int idgv, int idDonvi,  int type)
        {
            try
            {

                var paramIdDonvi = new SqlParameter("Id_don_vi", SqlDbType.Int)
                {
                    Value = idDonvi
                };
                var paramIdgv = new SqlParameter("Id_giao_vien", SqlDbType.Int)
                {
                    Value = idgv
                };
                var paramType = new SqlParameter("Loai_hien_thi", SqlDbType.Int)
                {
                    Value = type
                };


                var raw = _dbContext.Set<PhancongGV>().FromSqlRaw("EXEC Getlist_PhancongGiaovien @Id_giao_vien, @Id_don_vi, @Loai_hien_thi",
                    paramIdgv, paramIdDonvi, paramType)
                    .ToList();
                if (raw == null) raw = new List<PhancongGV>();
                var result = raw.Select(item => new PhancongGVDto
                {
                    STT = item.STT,
                    Id_giao_vien = item.Id_giao_vien,
                    Id_mon = item.Id_mon,
                    Ten_mon = item.Ten_mon ?? string.Empty,
                    Id_don_vi = item.Id_don_vi,
                    Ten_lop = item.Ten_lop ?? string.Empty,
                    Id_lop = item.Id_lop?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?.Select(int.Parse) ?.ToList() ?? []
                }).ToList();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<DsLop_ByGVandMon> GetList_Lop_ByGvAndMon( int idgv, int idDonvi,  int idMon)
        {
            try
            {

                var paramIdDonvi = new SqlParameter("Id_don_vi", SqlDbType.Int)
                {
                    Value = idDonvi
                };
                var paramIdgv = new SqlParameter("Id_giao_vien", SqlDbType.Int)
                {
                    Value = idgv
                };
                var paramIdMon = new SqlParameter("Id_mon", SqlDbType.Int)
                {
                    Value = idMon
                };


                var result = _dbContext.Set<DsLop_ByGVandMon>().FromSqlRaw("EXEC GetList_LopByMonAndGV @Id_giao_vien, @Id_mon, @Id_don_vi",
                    paramIdgv,paramIdMon, paramIdDonvi)
                    .ToList();
                if (result == null) result = new List<DsLop_ByGVandMon>();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool Add(List<PhancongGVDto> phancongList)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                // Group theo gv và môn
                var grouped = phancongList.GroupBy(x => new { x.Id_giao_vien, x.Id_mon });

                var ListDelete = new List<Lophoc_Monhoc>();
                var ListAdd = new List<Lophoc_Monhoc>();
                // xử lý từng group
                foreach (var group in grouped)
                {
                    var idgv = group.Key.Id_giao_vien;
                    var idmon = group.Key.Id_mon;
                    var newLopIds = group.SelectMany(x => x.Id_lop).Distinct().ToList();
                    
                    // Tìm bản ghi đã tồn tại
                    var exist = _dbContext.Lophoc_Monhoc.Where(x => x.Id_giao_vien == idgv && x.Id_mon == idmon).ToList();
                    if (newLopIds == null || newLopIds.Count == 0)
                    {
                        if (exist.Count > 0)
                        {
                            ListDelete.AddRange(exist);
                        }
                        continue;
                    }
                    // Tìm bản ghi có trong lớp môn nhưng không có ở phancongList để xoá
                    var toDeleteForGroup = exist.Where(e => !newLopIds.Contains(e.Id_lop)).ToList();
                    ListDelete.AddRange(toDeleteForGroup);

                    // Tìm bản ghi có trong phancongList nhưng không có ở Lớp môn để thêm
                    var existingLopIds = exist.Select(x => x.Id_lop).ToList();
                    var toAddForGroup = newLopIds.Where(lopId => !existingLopIds.Contains(lopId))
                        .Select(lopId => new Lophoc_Monhoc
                        {
                            Id_giao_vien = idgv,
                            Id_mon = idmon,
                            Id_lop = lopId
                        }).ToList();
                    ListAdd.AddRange(toAddForGroup);
                }

                if (ListDelete.Any())
                    _dbContext.BulkDelete(ListDelete);

                if (ListAdd.Any())
                    _dbContext.BulkInsert(ListAdd);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}
