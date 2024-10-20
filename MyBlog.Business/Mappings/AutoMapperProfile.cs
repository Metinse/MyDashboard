using AutoMapper;
using MyBlog.Entities;
using MyBlog.DTOs;
using MyBlog.Entities.DTOs;

namespace MyBlog.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Entity => DTO dönüşümü
            CreateMap<User, UserDto>();

            // DTO => Entity dönüşümü (kullanıcı oluşturma veya güncelleme senaryoları için)
            CreateMap<UserDto, User>();

            // Login için DTO dönüşümü
            CreateMap<LoginDto, User>();
        }
    }
}

