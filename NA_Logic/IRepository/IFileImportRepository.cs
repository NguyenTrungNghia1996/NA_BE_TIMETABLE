using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IFileImportRepository
    {
        string ImportAccessConvertExcelToJson(Stream stream);
        bool ImportExcelToDb(Stream stream, int idDonvi);
        string ImportBackUpConvertExcelToJson(Stream stream);
        bool ImportBackUpToDb(Stream stream, int idDonvi);
    }
}
