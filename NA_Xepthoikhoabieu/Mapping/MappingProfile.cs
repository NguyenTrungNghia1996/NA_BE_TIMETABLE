using AutoMapper;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Danh_muc;
using NA_Entities.Entities.Danhmuc;
using NA_Entities.Entities.Dtos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NA_Xepthoikhoabieu.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Auth_Users, UserDto>();
            CreateMap<DM_Caphoc_List, DM_Caphoc_ListDto>();
            CreateMap<DM_Caphoc, DM_Caphoc_Dto>();
            CreateMap<DM_Caphoc_Dto, DM_Caphoc>();
            CreateMap<Auth_Users, Auth_UsersDto>();
            CreateMap<Auth_UsersDto, Auth_Users>();
            CreateMap<Auth_Users, Auth_UsersDto>();
            CreateMap<Auth_Users_UpdateDto, Auth_Users>();

            CreateMap<DM_Cahoc, DM_CahocDto>();
            CreateMap<DM_Cahoc_List, DM_Cahoc_ListDto>();
            CreateMap<DM_CahocDto, DM_Cahoc>();
            CreateMap<DM_Donvi, DM_DonviDto>();
            CreateMap<DM_Donvi_List, DM_Donvi_List_Dto>();
            CreateMap<DM_DonviDto, DM_Donvi>();
            CreateMap<DM_Donvi_updateDto,DM_Donvi>();
            CreateMap<DM_Diemtruong, DM_DiemtruongDto>();
            CreateMap<DM_Diemtruong_List, DM_Diemtruong_ListDto>();
            CreateMap<DM_DiemtruongDto, DM_Diemtruong>();
            CreateMap<DM_Loaiphonghoc, DM_LoaiphonghocDto>();
            CreateMap<DM_Loaiphonghoc_List,DM_Loaiphonghoc_ListDto>();
            CreateMap<DM_LoaiphonghocDto, DM_Loaiphonghoc>();
            CreateMap<DM_Khoikienthuc, DM_KhoikienthucDto>();
            CreateMap<DM_Khoikienthuc_List,DM_Khoikienthuc_ListDto>();
            CreateMap<DM_KhoikienthucDto, DM_Khoikienthuc>();
            CreateMap<DM_Tochuyenmon,DM_TochuyenmonDto>();
            CreateMap<DM_TochuyenmonDto,DM_Tochuyenmon>();
            CreateMap<DM_Tochuyenmon_List,DM_Tochuyenmon_ListDto>();
            CreateMap<DM_Khoilop,DM_KhoilopDto>();
            CreateMap<DM_KhoilopDto,DM_Khoilop>();
            CreateMap<DM_Khoilop_List,DM_Khoilop_ListDto>();
            CreateMap<DM_Phonghoc,DM_PhonghocDto>();
            CreateMap<DM_Phonghoc_list,DM_Phonghoc_listDto>();
            CreateMap<DM_PhonghocDto,DM_Phonghoc>();
            CreateMap<DM_Tiethoc,DM_TiethocDto>();
            CreateMap<DM_TiethocDto,DM_Tiethoc>();
            CreateMap<DM_Tiethoc_List,DM_Tiethoc_ListDto>();
            CreateMap<DM_Tiethoc_updateDto, DM_Tiethoc>();
            CreateMap<DM_Ngayhoc_List,DM_Ngayhoc_ListDto>();
            CreateMap<DM_NgayhocDto,DM_Ngayhoc>();
            CreateMap<DM_Ngayhoc,DM_NgayhocDto>();
            CreateMap<DM_Monhoc,DM_MonhocDto>();
            CreateMap<DM_MonhocDto,DM_Monhoc>();
            CreateMap<DM_Monhoc_List, DM_Monhoc_ListDto>();
            CreateMap<Tiet_co_dinh_List, Tiet_co_dinh_ListDto>();
            CreateMap<DM_Banhoc, DM_BanhocDto>();
            CreateMap<DM_Banhoc_List, DM_Banhoc_ListDto>();
            CreateMap<DM_BanhocDto, DM_Banhoc>();
            CreateMap<Monhoc_Tohopmon, Monhoc_TohopmonDto>();
            CreateMap<Monhoc_Tohopmon_List, Monhoc_Tohopmon_ListDto>();
            CreateMap<Monhoc_TohopmonDto, Monhoc_Tohopmon>();
            CreateMap<DM_Giaovien, DM_GiaovienDto>();
            CreateMap<DM_Giaovien_List, DM_Giaovien_ListDto>();
            CreateMap<DM_GiaovienDto, DM_Giaovien>();

            CreateMap<Auth_Roles, Auth_RolesDto>();
            CreateMap<Auth_RolesDto, Auth_Roles>();
            CreateMap<Auth_Roles_PermissionDto, Auth_Roles_Permissions>();
            CreateMap<Auth_Roles_Permissions, Auth_Roles_PermissionDto>();
        }
    }
}
