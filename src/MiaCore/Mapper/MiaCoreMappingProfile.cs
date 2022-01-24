using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using MiaCore.Features;
using MiaCore.Features.CreateUser;
using MiaCore.Features.Login;
using MiaCore.Features.NewsList;
using MiaCore.Features.Register;
using MiaCore.Features.SaveBillingInfo;
using MiaCore.Features.SaveCategory;
using MiaCore.Features.SaveNews;
using MiaCore.Features.UpdateProfile;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using MiaCore.Utils;

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
            CreateMap<SaveNewsRequest, News>()
                .ForMember(x => x.Content, a => a.MapFrom(f => JsonSerializer.Serialize(f.Content, options)));
            CreateMap<SaveBillingInfoRequest, MiaBillingInfo>();
            CreateMap(typeof(GenericListResponse<>), typeof(GenericListResponse<>));
            CreateMap<News, NewsDto>()
                .ForMember(x => x.Content, a => a.MapFrom(f => JsonSerializer.Deserialize<object>(f.Content, options)));
        }
    }
}