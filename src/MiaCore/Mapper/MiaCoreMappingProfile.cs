using AutoMapper;
using MiaCore.Authentication;

namespace MiaCore.MApper
{
    internal class MiaCoreMappingProfile : Profile
    {
        public MiaCoreMappingProfile()
        {
            CreateMap<MiaUser, LoginResponse>();
        }
    }
}