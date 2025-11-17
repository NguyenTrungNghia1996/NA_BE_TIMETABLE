using NA_Entities.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IExportWordRepository
    {
        PhieubaogiangDto GetList_Chitiet(int idpbg, int idDonvi);
        byte[] FillTemplate(string templatePath, PhieubaogiangDto data);
        byte[] FillMultipleAndZip(string templatePath, int idlbg, int idDonvi);
    }
}
