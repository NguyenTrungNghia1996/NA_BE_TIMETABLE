using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;

namespace NA_Xepthoikhoabieu.Controllers
{
    [Route("api/tohopmon")]
    [ApiController]
    public class Monhoc_TohopmonController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITo_hop_monRepository _tohopmon;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_BanhocRepository _banhoc;
        private readonly IDM_KhoilopRepository _khoilop;
        private readonly IDM_MonhocRepository _monhoc;
        public Monhoc_TohopmonController(IMapper mapper, ITo_hop_monRepository tohopmon, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                         IDM_BanhocRepository banhoc, IDM_KhoilopRepository khoilop, IDM_MonhocRepository monhoc)
        {
            _mapper = mapper;
            _tohopmon = tohopmon;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _banhoc = banhoc;
            _khoilop = khoilop;
            _monhoc = monhoc;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize)
        {

            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            // Lấy danh sách dữ liệu
            int totalrecord = 0;
            var list = _tohopmon.GetList_Paging(PageIndex, PageSize, idDonvi, ref totalrecord);
            if (list == null || list.Count == 0)
                return ApiResult.NotFound("Không tồn tại bản ghi hợp lệ nào");
            var listDto = _mapper.Map<List<Monhoc_Tohopmon_ListDto>>(list);
            return ApiResult.Success(new
            {
                items = listDto,
                totalrecord = totalrecord
            },
            "Thành công");
        }
        [HttpGet("detail")]
        [RequireToken]
        public IActionResult GetDetailByID([FromQuery] int Id)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            // Lấy bản ghi từ db
            var detailtohopmon = _tohopmon.GetDetailById(Id, idDonvi);
            if (detailtohopmon == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<Monhoc_TohopmonDto>(detailtohopmon);
            return ApiResult.Success(detailDto, "Thành công");
        }
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] Monhoc_TohopmonDto tohopmon)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());

            // mapper data 
            var item = _mapper.Map<Monhoc_Tohopmon>(tohopmon);
            item.Id = 0;
            //check trùng tên
            var check_ten = _tohopmon.CheckTrungTen(idDonvi, tohopmon.Ten);
            if (check_ten)
                return ApiResult.BadRequest("Tên tổ hợp môn đã trùng, vui lòng kiểm tra lại");
            //check các id
            var check_ban = _banhoc.CheckId(tohopmon.Id_ban, idDonvi);
            var check_khoi = _khoilop.CheckKhoilopByDonvi(tohopmon.Id_khoi, idDonvi);
            var check_mon_1 = _monhoc.CheckId(tohopmon.Id_mon_1, idDonvi);
            var check_mon_2 = _monhoc.CheckId(tohopmon.Id_mon_2, idDonvi);
            var check_mon_3 = _monhoc.CheckId(tohopmon.Id_mon_3, idDonvi);
            if (!check_ban)
                ModelState.AddModelError("Id_ban", "Id ban không hợp lệ, vui lòng kiểm tra lại");
            if (!check_khoi)
                ModelState.AddModelError("Id_khoi", "Id khối không hợp lệ, vui lòng kiểm tra lại");
            if (!check_mon_1)
                ModelState.AddModelError("Id_mon_1", "Id môn 1 không hợp lệ, vui lòng kiểm tra lại");
            if (!check_mon_2)
                ModelState.AddModelError("Id_mon_2", "Id môn 2 không hợp lệ, vui lòng kiểm tra lại");
            if (!check_mon_3 && tohopmon.Id_mon_3>0)
                ModelState.AddModelError("Id_mon_3", "Id môn 3 không hợp lệ, vui lòng kiểm tra lại");
            if(tohopmon.So_tiet_toi_da_1_ca <=0)
                ModelState.AddModelError("So_tiet_toi_da_1_ca", "Số tiết tối đa 1 ca không hợp lệ, vui lòng kiểm tra lại");
            if (tohopmon.So_tiet_toi_da_2_ca > 0 && tohopmon.So_tiet_toi_da_2_ca >10)
                ApiResult.BadRequest( "Số tiết tối đa 2 ca phải nhỏ hơn 10");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (tohopmon.So_tiet_toi_da_2_ca>0 && tohopmon.So_tiet_toi_da_1_ca > tohopmon.So_tiet_toi_da_2_ca)
            {
                return ApiResult.BadRequest("Số tiết tối đa 2 ca phải lớn hơn hoặc bằng số tiết tối đa 1 ca");
            }
            var check = _tohopmon.CheckTrung(item, idDonvi);
            if (check)
                return ApiResult.BadRequest("Tổ hợp môn đã tồn tại");
            // add 
            bool add = _tohopmon.Add(item);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            // mapper data trả về view
            var itemDto = _mapper.Map<Monhoc_TohopmonDto>(item);
            return ApiResult.Success(new
            {
                item = itemDto
            },
            "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] Monhoc_TohopmonDto tohopmon)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
  
            // Kiểm tra bản ghi hợp lệ
            var tohopmondb = _tohopmon.GetDetailById(tohopmon.Id, idDonvi);
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (tohopmondb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            //check trùng tên
            var check_ten = _tohopmon.CheckTrungTen(idDonvi, tohopmon.Ten, tohopmon.Id);
            if (check_ten)
                return ApiResult.BadRequest("Tên tổ hợp môn đã trùng, vui lòng kiểm tra lại");
            //check các id
            var check_ban = _banhoc.CheckId(tohopmon.Id_ban, idDonvi);
            var check_khoi = _khoilop.CheckKhoilopByDonvi(tohopmon.Id_khoi, idDonvi);
            var check_mon_1 = _monhoc.CheckId(tohopmon.Id_mon_1, idDonvi);
            var check_mon_2 = _monhoc.CheckId(tohopmon.Id_mon_2, idDonvi);
            var check_mon_3 = _monhoc.CheckId(tohopmon.Id_mon_3, idDonvi);
            if (!check_ban)
                ModelState.AddModelError("Id_ban", "Id ban không hợp lệ, vui lòng kiểm tra lại");
            if (!check_khoi)
                ModelState.AddModelError("Id_khoi", "Id khối không hợp lệ, vui lòng kiểm tra lại");
            if (!check_mon_1)
                ModelState.AddModelError("Id_mon_1", "Id môn 1 không hợp lệ, vui lòng kiểm tra lại");
            if (!check_mon_2)
                ModelState.AddModelError("Id_mon_2", "Id môn 2 không hợp lệ, vui lòng kiểm tra lại");
            if (!check_mon_3 && tohopmon.Id_mon_3 > 0)
                ModelState.AddModelError("Id_mon_3", "Id môn 3 không hợp lệ, vui lòng kiểm tra lại");
            if (tohopmon.So_tiet_toi_da_1_ca <= 0)
                ModelState.AddModelError("So_tiet_toi_da_1_ca", "Số tiết tối đa 1 ca không hợp lệ, vui lòng kiểm tra lại");
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (tohopmon.So_tiet_toi_da_1_ca > tohopmon.So_tiet_toi_da_2_ca)
            {
                return ApiResult.BadRequest("Số tiết tối đa 2 ca phải lớn hơn hoặc bằng số tiết tối đa 1 ca");
            }

            var item = _mapper.Map<Monhoc_Tohopmon>(tohopmon);
            var check = _tohopmon.CheckTrung(item, idDonvi);
            if (check)
                return ApiResult.BadRequest("Tổ hợp môn đã tồn tại");
            bool add = _tohopmon.Update(item);
            if (!add)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var itemDto = _mapper.Map<Monhoc_TohopmonDto>(item);
            return ApiResult.Success(new
            {
                item = itemDto
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

            var tohopmondb = _tohopmon.GetDetailById(id, idDonvi);
            if (tohopmondb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");
            var request = _tohopmon.Delete(id);
            if (!request)
                return ApiResult.NotFound("Xóa thất bại");
            return ApiResult.Ok("Xóa thành công");
        }
    }
}
