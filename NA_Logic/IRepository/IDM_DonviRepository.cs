using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NA_Entities.Entities.Danhmuc;

namespace NA_Logic.IRepository
{
    public interface IDM_DonviRepository
    {
        DM_Donvi getDonviById(int id);
    }
}