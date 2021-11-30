using AutoMapper;
using MiaCore.Features;
using MiaCore.Features.Login;
using MiaCore.Features.Register;
using MiaCore.Models;

namespace MiaCore.Mapper
{
    internal class MiaCoreMappingProfile : Profile
    {
        public MiaCoreMappingProfile()
        {
            CreateMap<MiaUser, LoginResponseDto>();
            CreateMap<RegisterRequest, MiaUser>();
            CreateMap<MiaUser, MiaUserDto>();
        }
    }
}