using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IExportExcelRepository
    {
        byte[] ExportExcel_Class(int idtkb, int show_room, int show_teacher);
        byte[] ExportExcel_Teacher(int idtkb, int show_room);
        byte[] ExportExcel_TKB(int idtkb, int show_room, int show_teacher);
        byte[] ExportAllDataToExcel(int idDonVi, int idTkb);
    }
}
