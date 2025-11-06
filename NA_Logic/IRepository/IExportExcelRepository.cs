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
        byte[] ExportExcel_MaTranToanTruong(int idtkb);
        byte[] ExportExcel_MaTranKhoi(int idtkb);
        byte[] ExportExcel_MaTranGiaoVien(int idtkb);
        byte[] ExportExcel_MaTranToHopMon(int idtkb, int idDonvi);
        byte[] ExportExcel_MaTranLop(int idtkb);
    }
}
