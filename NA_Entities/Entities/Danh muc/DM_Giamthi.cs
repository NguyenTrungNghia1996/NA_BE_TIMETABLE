using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public class DM_Giamthi
    {
        public int Id { get; set; }
        public string Ma { get; set; }
        public string Ho_va_ten { get; set; }
        public int? Id_giao_vien { get; set; }
        public int Id_diem_thi { get; set; }
    }
    public class DM_Giamthi_Multi
    {
        public int Id_diem_thi { get; set; }
        public List<int?> Id_giao_vien { get; set; }
    }
    public class DM_Giamthi_List
    {
        public int Id { get; set; }
        public string Ma { get; set; }
        public string Ho_va_ten { get; set; }
        public int? Id_giao_vien { get; set; }
        public int Id_diem_thi { get; set; }
        public int Total {  get; set; }
    }
}
