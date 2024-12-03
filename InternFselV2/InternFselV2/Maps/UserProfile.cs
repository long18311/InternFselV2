using AutoMapper;
using InternFselV2.Entities;
using InternFselV2.Model.CommandModel.UserCmd;
using InternFselV2.Model.EnityModel;

namespace InternFselV2.Maps
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserCommandModel, User>();
            CreateMap<UpdateUserCommandModel, User>();
            CreateMap<User, UserModel>().MaxDepth(1);
        }
    }
}
