using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IExportExcelRepository
    {
        byte[] ExportExcel_Class(int idtkb);
        byte[] ExportExcel_Teacher(int idtkb);
        byte[] ExportExcel_TKB(int idtkb);
    }
}
