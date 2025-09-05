using Microsoft.EntityFrameworkCore;
using NA_Entities.DBContext;
using NA_Logic.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.Repository
{
    public class ValidateRepository : IValidateRepository
    {
        private readonly NA_DbContext _context;

        public ValidateRepository(NA_DbContext context)
        {
            _context = context;
        }

        public bool CheckTrungTen<T>(string name, int? excludeId = null) where T : class
        {
            var normalizedInputName = name?.Trim().ToLower().Replace(" ", "") ?? "";
            var tableName = _context.Model.FindEntityType(typeof(T)).GetTableName();

            var sql = excludeId == null
                ? $"SELECT Ten FROM {tableName}"
                : $"SELECT Ten FROM {tableName} WHERE Id != {excludeId}";

            var names = _context.Database.SqlQueryRaw<string>(sql).ToList();

            return names.Any(existingName =>
            {
                var normalizedExistingName = existingName?.Trim().ToLower().Replace(" ", "") ?? "";
                return normalizedInputName == normalizedExistingName;
            });
        }
        public bool CheckTrungTen_byDonvi<T>(int idDonvi, string name, int? excludeId = null) where T : class
        {
            var normalizedInputName = name?.Trim().ToLower().Replace(" ", "") ?? "";
            var tableName = _context.Model.FindEntityType(typeof(T)).GetTableName();

            var sql = excludeId == null
                ? $"SELECT Ten FROM {tableName} where Id_don_vi = {idDonvi}"
                : $"SELECT Ten FROM {tableName} WHERE  Id_don_vi = {idDonvi} and Id != {excludeId}";

            var names = _context.Database.SqlQueryRaw<string>(sql).ToList();

            return names.Any(existingName =>
            {
                var normalizedExistingName = existingName?.Trim().ToLower().Replace(" ", "") ?? "";
                return normalizedInputName == normalizedExistingName;
            });
        }
    }
}
