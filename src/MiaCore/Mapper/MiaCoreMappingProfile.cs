using AutoMapper;
using MiaCore.Features;
using MiaCore.Features.CreateUser;
using MiaCore.Features.Login;
using MiaCore.Features.Register;
using MiaCore.Features.SaveCategory;
using MiaCore.Features.UpdateProfile;
using MiaCore.Models;

namespace MiaCore.Mapper
{
    internal class MiaCoreMappingProfile : Profile
    {
        public MiaCoreMappingProfile()
        {
            CreateMap<MiaUser, LoginResponseDto>();
            CreateMap<RegisterRequest, MiaUser>();
            CreateMap<SaveUserRequest, MiaUser>();
            CreateMap<UpdateProfileRequest, MiaUser>();
            CreateMap<MiaUser, MiaUserDto>();
            CreateMap<SaveCategoryRequest, MiaCategory>();
            CreateMap<MiaCategory, CategoryDto>();
        }
    }
}