using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class Phieu_BaogiangRepository : IPhieu_BaogiangRepository
    {
        private readonly NA_DbContext _dbContext;
        public Phieu_BaogiangRepository(NA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Phieu_Baogiang_List> GetList_Paging(int PageIndex, int PageSize, int idlbg,  string search, ref int totalrecord)
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
                var paramIdLbg = new SqlParameter("IdLBG", SqlDbType.Int)
                {
                    Value = idlbg
                };
                var paramSearch = new SqlParameter("search", SqlDbType.NVarChar)
                {
                    Value = search ?? string.Empty
                };
                var paramTotal = new SqlParameter("total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var result = _dbContext.Set<Phieu_Baogiang_List>().FromSqlRaw("EXEC Phieu_Baogiang_GetList_Paging @pageIndex, @pageSize, @search, @IdLBG,  @total OUTPUT",
                    paramPageIndex, paramPageSize, paramSearch,paramIdLbg, paramTotal)
                    .ToList();
                if (result == null) result = new List<Phieu_Baogiang_List>();
                totalrecord = (int)paramTotal.Value;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Phieu_Baogiang GetDetailById(int Id)
        {
            try
            {
                var namhoc = _dbContext.Phieu_Baogiang.FirstOrDefault(c => c.Id == Id);
                return namhoc;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool Add(int idLgb, int idtkb)
        {
            try
            {
                var listIdgv = _dbContext.Chitiet_Thoikhoabieu.Where(c => c.Id_tkb == idtkb).Select(c => c.Id_giao_vien).Distinct().ToList();
                var listPBG = new List<Phieu_Baogiang>();
                for (int i = 0; i < listIdgv.Count; i++)
                {
                    var itemPBG = new Phieu_Baogiang
                    {
                        Id_lich_bao_giang = idLgb,
                        Id_giao_vien = listIdgv[i]
                    };
                    listPBG.Add(itemPBG);
                }
                _dbContext.BulkInsert(listPBG);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Delete(int idlgb)
        {
            try
            {
                var listPBG = _dbContext.Phieu_Baogiang.Where(c => c.Id_lich_bao_giang == idlgb).ToList();
                _dbContext.BulkDelete(listPBG);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
