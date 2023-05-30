using AutoMapper;
using Entity.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.Role
{
    public class RoleViewModel
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public string Created { get; set; }
    }
    public class RoleVMProfile : Profile
    {

        public RoleVMProfile()
        {
            CreateMap<Entity.Contracts.Role, RoleViewModel>()
                .ForMember(referencevm => referencevm.Name, option => option.MapFrom(src => src.Value))
                .ForMember(referencevm => referencevm.Created, option => option.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd")));
        }
    }
}
