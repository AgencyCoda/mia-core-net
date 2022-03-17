using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using MiaCore.Features;
using MiaCore.Features.CreateUser;
using MiaCore.Features.Login;
using MiaCore.Features.Register;
using MiaCore.Features.MiaBilling.SaveBillingInfo;
using MiaCore.Features.SaveCategory;
using MiaCore.Features.UpdateProfile;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;
using MiaCore.Features.MiaDevices.Register;

namespace MiaCore.Mapper
{
    internal class MiaCoreMappingProfile : Profile
    {
        public MiaCoreMappingProfile()
        {
            var options = new JsonSerializerOptions();
            var snakeCasePolicy = new SnakeCaseNamingPolicy();
            options.PropertyNamingPolicy = snakeCasePolicy;//JsonNamingPolicy.CamelCase;
            options.PropertyNameCaseInsensitive = true;
            options.Converters.Add(new JsonStringEnumConverter(snakeCasePolicy));

            CreateMap<MiaUser, LoginResponseDto>();
            CreateMap<RegisterRequest, MiaUser>();
            CreateMap<SaveUserRequest, MiaUser>();
            CreateMap<UpdateProfileRequest, MiaUser>();
            CreateMap<MiaUser, MiaUserDto>();
            CreateMap<SaveCategoryRequest, MiaCategory>();
            CreateMap<MiaCategory, CategoryDto>();
            CreateMap<SaveBillingInfoRequest, MiaBillingInfo>();
            CreateMap(typeof(GenericListResponse<>), typeof(GenericListResponse<>));
            CreateMap<RegisterMiaDeviceRequest, MiaDevice>()
                .ForMember(x => x.DeviceToken, x => x.MapFrom(a => a.Token))
                .ForMember(x => x.DeviceType, x => x.MapFrom(a => a.Type));
        }
    }
}