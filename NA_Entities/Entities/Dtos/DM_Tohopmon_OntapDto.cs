using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA_Entities.Entities.Dtos
{
    public class DM_Tohopmon_OntapDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "Mã tổ hợp không được để trống")]
        [StringLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        public string Ma { get; set; } = string.Empty;
        [Required(ErrorMessage = "Tên tổ hợp không được để trống")]
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Ten { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn ít nhất 1 môn")]
        public List<int> Ds_mon { get; set; } = new List<int>();
    }
    public class DM_Tohopmon_Ontap_ListDto
    {
        public int Id { get; set; } = 0;
        public string Ma { get; set; } = string.Empty;
        public string Ten { get; set; } = string.Empty;
        public string Ten_mon { get; set; } = string.Empty;
    }
}
