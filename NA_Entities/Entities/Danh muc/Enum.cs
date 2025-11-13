using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Danh_muc
{
    public enum Ngay
    {
        [Display(Name = "Hai")]
        thu_hai = 1,       
        [Display(Name = "Ba")]
        thu_ba = 2,       
        [Display(Name = "Tư")]
        thu_tu = 3,       
        [Display(Name = "Năm")]
        thu_nam = 4,       
        [Display(Name = "Sáu")]
        thu_sau = 5,       
        [Display(Name = "Bảy")]
        thu_bay = 6,       
        [Display(Name = "Chủ nhật")]
        chu_nhat=7
    }
    public enum Tiet
    {
        [Display(Name = "Tiết một")]
        tiet_mot = 1,       
        [Display(Name = "Tiết hai")]
        tiet_hai = 2,       
        [Display(Name = "Tiết ba")]
        tiet_ba = 3,       
        [Display(Name = "Tiết bốn")]
        tiet_tu = 4,       
        [Display(Name = "Tiết năm")]
        tiet_nam = 5,       
    }
}
