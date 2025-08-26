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
    public class CheckTenRepository : ICheckTenRepository
    {
        private readonly NA_DbContext _context;

        public CheckTenRepository(NA_DbContext context)
        {
            _context = context;
        }

        public bool CheckTrungTen<T>(string name, int? excludeId = null) where T : class
        {
            // Chuẩn hóa
            var normalizedInputName = name?.Trim().ToLower().Replace(" ", "") ?? "";

            // Lấy danh sách tên hiện có
            var existingNames = _context.Set<T>()
                .Where(x => excludeId == null || EF.Property<int>(x, "Id") != excludeId)
                .Select(x => EF.Property<string>(x, "Name"))
                .ToList();

            // So sánh với từng tên đã chuẩn hóa
            foreach (var existingName in existingNames)
            {
                var normalizedExistingName = existingName?.Trim().ToLower().Replace(" ", "") ?? "";
                if (normalizedInputName == normalizedExistingName)
                {
                    return true; 
                }
            }

            return false;
        }
    }
}
