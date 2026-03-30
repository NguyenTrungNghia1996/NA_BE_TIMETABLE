using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.IRepository.Auth;
using NA_Logic.IRepository.LichOnTap;
using NA_Logic.IRepository.LichThi;
using NA_Logic.IRepository.XepGiamThi;
using NA_Logic.IRepository.XepThoiKhoaBieu;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/hanhchinh")]
    [ApiController]
    public class DM_DonviHanhchinhController : ControllerBase
    {
        private readonly IDM_DonviHanhchinhRepository _Dantoc;

        public DM_DonviHanhchinhController(IDM_DonviHanhchinhRepository Dantoc)
        {
            _Dantoc = Dantoc;

        }
        [HttpGet("tinh")]
        public IActionResult GetList_Tinh_Paging([FromQuery] string search = "")
        {

            var list = _Dantoc.GetList_Tinh_Paging(search);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();

            return ApiResult.Success(new
            {
                items = list,
            },
            "Thành công");
        }
        [HttpGet("xa")]
        public IActionResult GetList_Xa_Paging([FromQuery] string search = "", [FromQuery] int idTinh = 0)
        {

            var list = _Dantoc.GetList_Xa_Paging(search, idTinh);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();

            return ApiResult.Success(new
            {
                items = list,
            },
            "Thành công");
        }

    }
}
