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
            CreateMap<DM_Lophoc, DM_LophocDto>();
            CreateMap<DM_LophocDto, DM_Lophoc>();
            CreateMap<DM_Lophoc_List, DM_Lophoc_ListDto>();
            CreateMap<Danhsach_Thoikhoabieu,Danhsach_ThoikhoabieuDto>();
            CreateMap<DM_Donvi, Thongtin_Donvi_updateDto>();
            CreateMap<Thongtin_Donvi_updateDto, DM_Donvi>();
            CreateMap<Ca_DonviDto, Ca_Donvi>();

            CreateMap<DM_Lopontap, DM_LopontapDto>();
            CreateMap<DM_LopontapDto, DM_Lopontap>();
            CreateMap<DM_Lopontap_List, DM_Lopontap_ListDto>();
            CreateMap<DM_Hocsinh, DM_HocsinhDto>();
            CreateMap<DM_HocsinhDto, DM_Hocsinh>();
            CreateMap<DM_Hocsinh_List, DM_Hocsinh_ListDto>();
            CreateMap<DM_Loaikiemtra, DM_LoaikiemtraDto>();
            CreateMap<DM_LoaikiemtraDto, DM_Loaikiemtra>();
            CreateMap<DM_Loaikiemtra_List, DM_Loaikiemtra_ListDto>();
            CreateMap<DM_Tohopmon_Ontap, DM_Tohopmon_OntapDto>();
            CreateMap<DM_Tohopmon_OntapDto, DM_Tohopmon_Ontap>();
            CreateMap<DM_Tohopmon_Ontap_List, DM_Tohopmon_Ontap_ListDto>();
            CreateMap<Hocsinh_Lopon_List, Hocsinh_Lopon_ListDto>();
            CreateMap<DM_Baikiemtra_List, DM_Baikiemtra_ListDto>();

            CreateMap<DM_Hoidongthi, DM_HoidongthiDto>();
            CreateMap<DM_HoidongthiDto, DM_Hoidongthi>();
            CreateMap<DM_Hoidongthi_List, DM_Hoidongthi_ListDto>();
            CreateMap<DM_Diemthi_List, DM_Diemthi_ListDto>();
            CreateMap<DM_Monthi, DM_MonthiDto>();
            CreateMap<DM_MonthiDto, DM_Monthi>();
            CreateMap<DM_Monthi_List, DM_Monthi_ListDto>();
            CreateMap<DM_Giamthi, DM_GiamthiDto>();
            CreateMap<DM_GiamthiDto, DM_Giamthi>();
            CreateMap<DM_Giamthi_List, DM_Giamthi_ListDto>(); 
            CreateMap<DM_Phongthi, DM_PhongthiDto>();
            CreateMap<DM_PhongthiDto, DM_Phongthi>();
            CreateMap<DM_Phongthi_List, DM_Phongthi_ListDto>(); 
            CreateMap<DM_Thisinh, DM_ThisinhDto>();
            CreateMap<DM_ThisinhDto, DM_Thisinh>();
            CreateMap<DM_Thisinh_List, DM_Thisinh_ListDto>(); 

            CreateMap<Auth_Roles, Auth_RolesDto>();
            CreateMap<Auth_RolesDto, Auth_Roles>();
            CreateMap<Auth_Roles_PermissionDto, Auth_Roles_Permissions>();
            CreateMap<Auth_Roles_Permissions, Auth_Roles_PermissionDto>();
        }
    }
}
