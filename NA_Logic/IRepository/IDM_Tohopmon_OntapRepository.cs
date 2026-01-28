using NA_Entities.Entities.Danh_muc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Logic.IRepository
{
    public interface IDM_Tohopmon_OntapRepository
    {
        List<DM_Tohopmon_Ontap_List> GetList_Paging(int PageIndex, int PageSize, string search, int idDonvi);
        DM_Tohopmon_Ontap GetDetailById(int id, int idDonvi);
        List<int> GetlistMon(int id);
        bool Add(DM_Tohopmon_Ontap thm);
        bool CheckMa(string Ma, int idDonvi, int? Id);
        bool AddMon(int Id, List<int> monId);
        bool Update(DM_Tohopmon_Ontap thm);
        bool UpdateMon(int Id, List<int> monId);
        bool Delete(int Id);
        bool DeleteMon(int Id);
    }
}
