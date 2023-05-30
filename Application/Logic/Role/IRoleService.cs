using Application.DTO.Role;
using Entity.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logic
{
    public interface IRoleService
    {
        Role Create(RoleDTO roleDTO, ApplicationUser currentUser);
        Role Edit(RoleEditDTO roleDTO, ApplicationUser currentUser);
        IEnumerable<Role> GetAll(FilterRoleDTO filterRoleDTO, ApplicationUser user);
        int CountAll(FilterRoleDTO filterRoleDTO, ApplicationUser user);
        Role GetById(int id);
        Role GetByCode(int roleCode);
        Role GetByName(string roleName);
        bool Delete(int roleId);
        bool AssignRoleToUser(int roleId, int userId);
    }
}
