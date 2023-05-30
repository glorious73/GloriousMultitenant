using Application.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Account
{
    public class FilterUserDTO : FilterDTO
    {
        public int RoleId { get; set; } = 0;

    }
}
