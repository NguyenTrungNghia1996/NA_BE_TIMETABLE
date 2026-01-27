using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/lienhe")]
    [ApiController]
    public class Thongtin_LienheController: ControllerBase
    {
        private readonly IThongtin_LienheRepository _thongtin;
        public Thongtin_LienheController(IThongtin_LienheRepository thongtin)
        {
            _thongtin = thongtin;
        }
        [HttpPost]
        public IActionResult Create([FromBody] Thongtin_Lienhe tt)
        {
            tt.Id = 0;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            // add 
            bool add = _thongtin.Add(tt);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            
            bool send = _thongtin.SendMail(tt);
            if (!send)
                return ApiResult.NotFound("Gửi thông tin thất bại");
            return ApiResult.Success("Gửi thông tin thành công");
        }
    }
}
