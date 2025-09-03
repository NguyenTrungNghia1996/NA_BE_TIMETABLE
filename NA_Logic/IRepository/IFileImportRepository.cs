using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IFileImportRepository
    {
        string ConvertExcelToJson(Stream stream);
    }
}
