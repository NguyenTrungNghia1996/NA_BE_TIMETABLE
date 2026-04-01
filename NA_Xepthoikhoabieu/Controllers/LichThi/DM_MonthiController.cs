using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
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
    [Route("api/monthi")]
    [ApiController]
    public class DM_MonthiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_MonthiRepository _monthi;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_DonviRepository _donvi;
        private readonly IDM_HoidongthiRepository _hoidong;
        private readonly IDM_MonhocRepository _monhoc;
        public DM_MonthiController(IMapper mapper, IDM_MonthiRepository monthi, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                       IDM_HoidongthiRepository hoidong, IDM_DonviRepository donvi, IDM_MonhocRepository monhoc)
        {
            _mapper = mapper;
            _monthi = monthi;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _hoidong = hoidong;
            _donvi = donvi;
            _monhoc = monhoc;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var list = _monthi.GetList_Paging(PageIndex, PageSize, search, idDonvi);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<DM_Monthi_ListDto>>(list);
            int totalrecord = list.First().Total;
            return ApiResult.Success(new
            {
                items = listDto,
                totalrecord = totalrecord
            },
            "Thành công");
        }
        [HttpGet("cha")]
        [RequireToken]
        public IActionResult GetListCha([FromQuery] int idHoiDong, [FromQuery] string search = "")
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if (idHoiDong == 0 || idHoiDong == null)
            {
                return ApiResult.BadRequest("Vui lòng chọn hội đồng");
            }
            bool checkHoiDong = _hoidong.CheckId(idHoiDong, idDonvi);
            if (!checkHoiDong)
            {
                return ApiResult.BadRequest("Id hội đồng không hợp lệ, vui lòng kiểm tra lại");
            }
            var list = _monthi.GetlistCha(search, idHoiDong);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();

            return ApiResult.Success(new
            {
                items = list
            },
            "Thành công");
        }
        [HttpGet("tuchon")]
        [RequireToken]
        public IActionResult GetListTuChon([FromQuery] int idHoiDong, [FromQuery] string search = "")
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if (idHoiDong == 0 || idHoiDong == null)
            {
                return ApiResult.BadRequest("Vui lòng chọn hội đồng");
            }
            bool checkHoiDong = _hoidong.CheckId(idHoiDong, idDonvi);
            if (!checkHoiDong)
            {
                return ApiResult.BadRequest("Id hội đồng không hợp lệ, vui lòng kiểm tra lại");
            }
            var list = _monthi.GetlistMonTuChon(search, idHoiDong);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();

            return ApiResult.Success(new
            {
                items = list
            },
            "Thành công");
        }
        [HttpGet("detail")]
        [RequireToken]
        public IActionResult GetDetailByID([FromQuery] int Id)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            // Lấy bản ghi từ db
            var detailmonthi = _monthi.GetDetailById(Id, idDonvi);
            if (detailmonthi == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");

            return ApiResult.Success(detailmonthi, "Thành công");
        }
        [HttpGet("list")]
        [RequireToken]
        public IActionResult GetListMon([FromQuery] int idHoiDong)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            bool checkhoidong = _hoidong.CheckId(idHoiDong, idDonvi);
            if (!checkhoidong)
            {
                return ApiResult.BadRequest("Id hội đồng không hợp lệ, vui lòng kiểm tra lại");
            }
            // Lấy bản ghi từ db
            var detailhocsinh = _monthi.GetMonByIdHoiDong(idHoiDong, idDonvi);
            if (detailhocsinh == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {idHoiDong}");

            return ApiResult.Success(detailhocsinh, "Thành công");
        }
        [HttpPost("list")]
        [RequireToken]
        public IActionResult CreateList([FromBody] DM_Monthi_Multi data)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if (data.Id_hoi_dong == 0 || data.Id_hoi_dong == null)
            {
                return ApiResult.BadRequest("Vui lòng chọn hội đồng");
            }
            if (data.Id_mon.Count == 0 || data.Id_mon == null)
            {
                return ApiResult.BadRequest("Vui lòng chọn môn học");
            }
            bool checkhoidong = _hoidong.CheckId(data.Id_hoi_dong, idDonvi);
            if (!checkhoidong)
            {
                return ApiResult.BadRequest("Id hội đồng không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkmonhoc = _monhoc.CheckIds(data.Id_mon.OfType<int>(), idDonvi);
            if (!checkmonhoc )
            {
                return ApiResult.BadRequest("Id môn học không hợp lệ, vui lòng kiểm tra lại");
            }
            
            // add 
            bool add = _monthi.AddList(data);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");

            return ApiResult.Success("Thêm mới thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_MonthiDto monthi)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            monthi.Id = 0;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var item = _mapper.Map<DM_Monthi>(monthi);
            item.Id = 0;
            bool checkma = _monthi.CheckMa(monthi.Ma, idDonvi, monthi.Id);
            if (checkma)
            {
                return ApiResult.BadRequest("Mã môn thi đã tồn tại");
            }
            bool checkten = _monthi.CheckTrungTen(monthi.Id_hoi_dong, monthi.Ten, null);
            if (checkten)
            {
                return ApiResult.BadRequest("Tên môn thi đã tồn tại");
            }

            if (monthi.Id_hoi_dong == 0 || monthi.Id_hoi_dong == null)
            {
                return ApiResult.BadRequest("Vui lòng chọn hội đồng");
            }

            bool checkhoidong = _hoidong.CheckId(monthi.Id_hoi_dong, idDonvi);
            if (!checkhoidong)
            {
                return ApiResult.BadRequest("Id hội đồng không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkcha = _monthi.CheckIdCha(monthi.Id_cha, idDonvi);
            if (!checkcha)
            {
                return ApiResult.BadRequest("Id cha không hợp lệ, vui lòng kiểm tra lại");
            }

            bool add = _monthi.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");

            return ApiResult.Success(new
            {
                item = monthi
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_Monthi monthi)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            var item = _mapper.Map<DM_Monthi>(monthi);
            var monthidb = _monthi.GetDetailById(monthi.Id, idDonvi);
            if (monthidb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");

            bool checkma = _monthi.CheckMa(monthi.Ma, monthi.Id_hoi_dong, monthi.Id);
            if (checkma)
            {
                return ApiResult.BadRequest("Mã môn thi đã tồn tại");
            }
            bool checkten = _monthi.CheckTrungTen(monthi.Id_hoi_dong, monthi.Ten, monthi.Id);
            if (checkten)
            {
                return ApiResult.BadRequest("Tên môn thi đã tồn tại");
            }
            if (monthi.Id_hoi_dong == 0 || monthi.Id_hoi_dong == null)
            {
                return ApiResult.BadRequest("Vui lòng chọn hội đồng");
            }

            bool checkhoidong = _hoidong.CheckId(monthi.Id_hoi_dong, idDonvi);
            if (!checkhoidong)
            {
                return ApiResult.BadRequest("Id hội đồng không hợp lệ, vui lòng kiểm tra lại");
            }
            bool checkcha = _monthi.CheckIdCha(monthi.Id_cha, idDonvi);
            if (!checkcha)
            {
                return ApiResult.BadRequest("Id cha không hợp lệ, vui lòng kiểm tra lại");
            }
            bool add = _monthi.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            return ApiResult.Success(new
            {
                item = monthi
            },
            "Cập nhật thành công");
        }
        [HttpDelete]
        [RequireToken]
        public IActionResult Delete([FromQuery] int id)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var monthidb = _monthi.GetDetailById(id, idDonvi);
            if (monthidb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            //check ràng buộc
            //bool check = _monthi.CheckContraint(id, idDonvi);
            //if (check)
            //{
            //    return ApiResult.BadRequest("Học sinh đã có ràng buộc, không thể xoá");
            //}

            bool request = _monthi.Delete(id, idDonvi);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }

    }
}
