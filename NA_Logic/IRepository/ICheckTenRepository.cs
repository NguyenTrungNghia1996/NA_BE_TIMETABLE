using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface ICheckTenRepository
    {
        bool CheckTrungTen<T>(string name, int? excludeId = null) where T : class;
    }
}
