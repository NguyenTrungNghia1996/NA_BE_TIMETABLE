using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Monthi
    {
        public int Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int Id_hoi_dong { get; set; }
        public int? Id_mon { get; set; }
    }
    public class DM_Monthi_Detail
    {
        public int Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int Id_hoi_dong { get; set; }
        public int? Id_mon { get; set; }
        public int Id_nam { get; set; }
    }
    public class DM_Monthi_Multi
    {
        public int Id_hoi_dong { get; set; }
        public List<int?> Id_mon { get; set; } 
    }
    public class DM_Monthi_List
    {
        public int Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string Ten_hoi_dong { get;set; }
        public string Ten_nam { get; set; }
        public int Id_hoi_dong { get; set; }
        public int? Id_mon { get; set; }
        public int Total { get; set; }
    }
}
