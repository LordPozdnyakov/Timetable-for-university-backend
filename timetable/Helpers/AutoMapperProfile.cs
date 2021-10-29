using AutoMapper;
using timetable.Model;
using timetable.Models;

namespace timetable.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            //CreateMap<UpdateModel, User>();
        }
    }
}