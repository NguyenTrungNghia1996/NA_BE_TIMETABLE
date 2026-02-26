using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using NA_Logic.IRepository;
using NA_Logic.Repository;
using NA_Xepthoikhoabieu.Helpers;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NA_Xepthoikhoabieu.Controllers
{
    [ApiController]
    [Route("api/thm/on")]
    public class DM_Tohopmon_OntapController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDM_Tohopmon_OntapRepository _thm;
        private readonly IClaimHelperRepository _claimHelperRepository;
        private readonly IAuthRepository _auth;
        private readonly IDM_MonhocRepository _mon;

        public DM_Tohopmon_OntapController(IMapper mapper, IDM_Tohopmon_OntapRepository thm, IClaimHelperRepository claimHelperRepository, IAuthRepository auth,
                 IDM_MonhocRepository mon)
        {
            _mapper = mapper;
            _thm = thm;
            _claimHelperRepository = claimHelperRepository;
            _auth = auth;
            _mon = mon;
        }
        [HttpGet]
        [RequireToken]
        public IActionResult GetList_Paging([FromQuery] int PageIndex, [FromQuery] int PageSize, [FromQuery] string search = "")
        {
            // Kiểm tra tồn tại Id_Donvi và lấy Id_Donvi từ token
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy danh sách dữ liệu

            var list = _thm.GetList_Paging(PageIndex, PageSize, search, idDonvi);
            if (list == null || list.Count == 0)
                return ApiResult.Ok();
            var listDto = _mapper.Map<List<DM_Tohopmon_Ontap_ListDto>>(list);
            int totalrecord = list.First().Total;
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
            if (Id <= 0)
                return ApiResult.BadRequest($"Id {Id} không hợp lệ, vui lòng kiểm tra lại");
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            // Lấy bản ghi từ db
            var detail = _thm.GetDetailById(Id, idDonvi);
            if (detail == null)
                return ApiResult.NotFound($"Không tìm thấy bản ghi nào cho Id= {Id}");
            var detailDto = _mapper.Map<DM_Tohopmon_OntapDto>(detail);
            detailDto.Ds_mon = _thm.GetlistMon(Id);
            return ApiResult.Success(detailDto, "Thành công");
        }
        
        [HttpPost]
        [RequireToken]
        public IActionResult Create([FromBody] DM_Tohopmon_OntapDto thm)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var addthm = _mapper.Map<DM_Tohopmon_Ontap>(thm);
            addthm.Id = 0;
            addthm.Id_don_vi = idDonvi;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!Regex.IsMatch(thm.Ma, @"^[A-Za-z0-9\s._-]+$"))
            {
                return BadRequest(new { message = "Mã tổ hợp món chỉ được chứa chữ cái, số, khoảng trắng và các ký tự . - _" });
            }
            if (thm.Ds_mon == null || thm.Ds_mon.Count == 0)
            {
                return ApiResult.BadRequest("Vui lòng chọn ít nhất 1 môn học");
            }
            
            var checkmon = _mon.CheckIds(thm.Ds_mon, idDonvi);
            if (!checkmon)
                return ApiResult.BadRequest("Id môn học không hợp lệ, vui lòng kiểm tra lại");
            bool checkma = _thm.CheckMa(thm.Ma, idDonvi, addthm.Id);
            if (checkma)
            {
                return ApiResult.BadRequest("Mã tổ hợp đã tồn tại");
            }
            //add
            bool add = _thm.Add(addthm);
            if (!add)
                return ApiResult.NotFound("Thêm mới thất bại, lưu dữ liệu không thành công");
            thm.Id = addthm.Id;
            var addmon = _thm.AddMon(thm.Id, thm.Ds_mon);
            if (!addmon)
                return ApiResult.Success(new
                {
                    item = thm
                },
                "Thêm mới thất bại");
            return ApiResult.Success(new
            {
                item = thm
            }, "Thêm mới thành công");
        }
        [HttpPut]
        [RequireToken]
        public IActionResult Update([FromBody] DM_Tohopmon_OntapDto thm)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");
            var item = _mapper.Map<DM_Tohopmon_Ontap>(thm);
            item.Id_don_vi = idDonvi;
            var thmdb = _thm.GetDetailById(thm.Id, idDonvi);
            
            if (thmdb == null)
                return ApiResult.NotFound("Bản ghi không tồn tại, vui lòng kiểm tra lại Id");
            if (!ModelState.IsValid)
                return ApiResult.BadRequest(ModelState.GetErrorsAsString());
            if (!Regex.IsMatch(thm.Ma, @"^[A-Za-z0-9\s._-]+$"))
            {
                return BadRequest(new { message = "Mã tổ hợp món chỉ được chứa chữ cái, số, khoảng trắng và các ký tự . - _" });
            }
            if (thm.Ds_mon == null || thm.Ds_mon.Count == 0)
            {
                return ApiResult.BadRequest("Vui lòng chọn ít nhất 1 môn học");
            }

            var checkmon = _mon.CheckIds(thm.Ds_mon, idDonvi);
            if (!checkmon)
                return ApiResult.BadRequest("Id môn học không hợp lệ, vui lòng kiểm tra lại");
            bool checkma = _thm.CheckMa(thm.Ma, idDonvi, thm.Id);
            if (checkma)
            {
                return ApiResult.BadRequest("Mã tổ hợp đã tồn tại");
            }
            //update
            bool update = _thm.Update(item);
            if (!update)
                return ApiResult.NotFound("Cập nhật thất bại, lưu dữ liệu không thành công");
            var editMon = _thm.UpdateMon(thm.Id, thm.Ds_mon);
            
            if (!editMon)
                return ApiResult.Success(new
                {
                    item = thm
                },
                "Cập nhật thất bại");

            return ApiResult.Success(new
            {
                item = thm
            }, "Cập nhật thành công");
        }
        [HttpDelete]
        [RequireToken]
        public IActionResult Delete([FromQuery] int id)
        {
            int idDonvi = _claimHelperRepository.GetIdDonvi(User);
            if (idDonvi == 0) return ApiResult.Unauthorized("Thông tin đơn vị không hợp lệ, vui lòng kiểm tra lại hoặc liên hệ admin để biết thêm chi tiết");

            var thmdb = _thm.GetDetailById(id, idDonvi);
            if (thmdb == null)
                return ApiResult.NotFound($"Bản ghi có Id= {id} không tồn tại, vui lòng kiểm tra lại");

            //check ràng buộc
            bool check = _thm.CheckContraint(id, idDonvi);
            if (check)
            {
                return ApiResult.BadRequest("Tổ hợp môn đã có ràng buộc, không thể xoá");
            }

            var deleteMon = _thm.DeleteMon(id);
            if (!deleteMon)
                return ApiResult.BadRequest("Xóa các môn học thất bại");
            var delete = _thm.Delete(id);
            if (!delete)
                return ApiResult.BadRequest("Xóa tổ hợp môn thất bại");
            return ApiResult.Success("Xóa tổ hợp môn thành công");
        }

    }
}