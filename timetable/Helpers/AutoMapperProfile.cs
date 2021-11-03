using AutoMapper;
using timetable.Models;

namespace timetable.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, User>();
            CreateMap<EmailRecovery, User>();

            //CreateMap<User, UserModel>();
            //CreateMap<UpdateModel, User>();
        }
    }
}