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
    [Route("api/dantoc")]
    [ApiController]
    public class DM_DantocController : ControllerBase
    {
        private readonly IDM_DantocRepository _Dantoc;

        public DM_DantocController( IDM_DantocRepository Dantoc)
        {
            _Dantoc = Dantoc;

        }
        [HttpGet]
        public IActionResult GetList_Paging( [FromQuery] string search = "")
        {

            var list = _Dantoc.GetList_Paging(search);
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
