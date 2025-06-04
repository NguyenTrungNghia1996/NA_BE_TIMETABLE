using AutoMapper;
using NA_Entities.Entities.Auth;
using NA_Entities.Entities.Danh_muc;
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
            
            CreateMap<DM_Cahoc,DM_CahocDto>();
            CreateMap<DM_Cahoc_List, DM_Cahoc_ListDto>();
            CreateMap<DM_CahocDto, DM_Cahoc>();
        }
    }
}
