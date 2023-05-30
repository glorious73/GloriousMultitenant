using Application.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Role
{
    public class FilterRoleDTO : FilterDTO
    {
        public int CreatedById { get; set; } = 0;
    }
}
