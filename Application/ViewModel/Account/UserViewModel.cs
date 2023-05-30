using AutoMapper;
using Entity.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.Account
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public string LastLogin { get; set; }
        public string Created { get; set; }
    }
    public class ApplicationUserVMProfile : Profile
    {
        public ApplicationUserVMProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>()
                .ForMember(uservm => uservm.Role, option => option.MapFrom(src => src.Role.Value))
                .ForMember(uservm => uservm.LastLogin, option => option.MapFrom(src => src.LastLogin.ToString("yyyy-MM-dd")))
                .ForMember(uservm => uservm.Created, option => option.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd")));
        }
    }
}
