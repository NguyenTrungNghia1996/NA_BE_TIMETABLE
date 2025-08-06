using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;
using NetTopologySuite.Index.HPRtree;

namespace NA_Xepthoikhoabieu.Controllers
{
    [ApiController]
    [Route("api/object")]
    public class ObjectController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IObjectRepository _ob;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_GiaovienRepository _giaovien;
        private readonly IDM_MonhocRepository _monhoc;
        private readonly IDM_PhonghocRepository _phonghoc;
        private readonly ITo_hop_monRepository _thm;
        private readonly IDanhsach_ThoikhoabieuRepository _tkb;

        public ObjectController(IMapper mapper, IObjectRepository ob, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                                     IDM_GiaovienRepository giaovien, IDM_MonhocRepository monhoc, IDM_PhonghocRepository phonghoc, ITo_hop_monRepository thm, IDanhsach_ThoikhoabieuRepository tkb)
        {
            _mapper = mapper;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _ob = ob;
            _giaovien = giaovien;
            _monhoc = monhoc;
            _phonghoc = phonghoc;
            _thm = thm;
            _tkb = tkb;
        }
        [HttpGet("giaovien")]
        [RequireToken]
        public IActionResult Get_Oject_giaovien([FromQuery] int Idgv, [FromQuery] int Idtkb)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var check_tkb = _tkb.CheckId(Idtkb, idDonvi);
            var check_gv = _giaovien.CheckId(Idgv, idDonvi);
            if (Idgv <= 0 || !check_gv)
                return ApiResult.BadRequest($"Id giáo viên =  {Idgv} không hợp lệ, vui lòng kiểm tra lại");
            if (Idtkb <= 0 || !check_tkb)
                return ApiResult.BadRequest($"Id thời khoá biểu =  {Idtkb} không hợp lệ, vui lòng kiểm tra lại");
            // Lấy bản ghi từ db
            var detail = _ob.Object_giaovien(Idgv, Idtkb);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id giáo viên = {Idgv} và Id thời khoá biểu = {Idtkb}");
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpGet("phong")]
        [RequireToken]
        public IActionResult Get_Oject_Phonghoc([FromQuery] int Idph, [FromQuery] int Idtkb)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var check_tkb = _tkb.CheckId(Idtkb, idDonvi);
            var check_ph = _phonghoc.CheckId(Idph, idDonvi);
            if (Idph <= 0 || !check_ph)
                return ApiResult.BadRequest($"Id phòng học =  {Idph} không hợp lệ, vui lòng kiểm tra lại");
            if (Idtkb <= 0 || !check_tkb)
                return ApiResult.BadRequest($"Id thời khoá biểu =  {Idtkb} không hợp lệ, vui lòng kiểm tra lại");
            // Lấy bản ghi từ db
            var detail = _ob.Object_phonghoc(Idph, Idtkb);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id phòng học = {Idph} và Id thời khoá biểu = {Idtkb}");
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpGet("mon")]
        [RequireToken]
        public IActionResult Get_Oject_mon([FromQuery] int Idmon)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var check_mon = _monhoc.CheckId(Idmon, idDonvi);
            if (Idmon <= 0 || !check_mon)
                return ApiResult.BadRequest($"Id môn =  {Idmon} không hợp lệ, vui lòng kiểm tra lại");
            // Lấy bản ghi từ db
            var detail = _ob.Object_monhoc(Idmon, idDonvi);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id môn = {Idmon}");
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpGet("tohopmon")]
        [RequireToken]
        public IActionResult Get_Oject_Tohopmon([FromQuery] int Idthm)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var check_thm = _thm.CheckId(Idthm, idDonvi);
            if (Idthm <= 0 || !check_thm)
                return ApiResult.BadRequest($"Id tổ hợp môn =  {Idthm} không hợp lệ, vui lòng kiểm tra lại");

            // Lấy bản ghi từ db
            var detail = _ob.Object_tohopmon(Idthm, idDonvi);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id tổ hợp môn = {Idthm}");
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpGet("lophoc")]
        [RequireToken]
        public IActionResult Get_Oject_lophoc([FromQuery] int idlop, [FromQuery] int idtkb)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            

            // Lấy bản ghi từ db
            var detail = _ob.Object_lophoc(idlop, idtkb);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id lớp = {idlop}");
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpGet("monkhoi")]
        [RequireToken]
        public IActionResult Get_Oject_monkhoi([FromQuery] int idmon, [FromQuery] int idlop)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");


            // Lấy bản ghi từ db
            var detail = _ob.Object_monkhoi(idmon, idlop, idDonvi);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id môn = {idmon}");
            return ApiResult.Success(detail, "Thành công");
        }
        [HttpGet("checkgv")]
        [RequireToken]
        public IActionResult checkgv(int Ngay, int Tiet, int Ca, int id_giaovien, int id_tkb)
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");


            // Lấy bản ghi từ db
            var detail = _ob.Check_gv(Ngay, Tiet,  Ca,  id_giaovien, id_tkb);
            return ApiResult.Success(detail, "Thành công");
        }
    }
}
