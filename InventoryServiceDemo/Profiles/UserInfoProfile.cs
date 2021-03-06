using AutoMapper;
using InventoryServiceDemo.DTOs.Requests;
using InventoryServiceDemo.DTOs.Responses;
using InventoryServiceDemo.Models;

namespace InventoryServiceDemo.Profiles
{
    public class UserInfoProfile : Profile
    {
        public UserInfoProfile()
        {
            CreateMap<UserInfo, UserInfoReadDto>();
            CreateMap<UserRegisterDto, UserInfo>();
        }
    }
}