using Application.DTO.Role;
using Data.UnitOfWork;
using Entity.Contracts;
using Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logic
{
    public class RoleService : IRoleService
    {
        private IUnit _unit { get; set; }

        public RoleService(IUnit unit)
        {
            _unit = unit;
        }

        public Role Create(RoleDTO roleDTO, ApplicationUser currentUser)
        {
            if (currentUser == null)
                throw new ArgumentNullException("The role needs to be created by a user.");
            int roleCount = _unit.GetRepository<Role>().Count() + 1;
            bool isValidRole = IsValidRole(roleCount, roleDTO.Value, currentUser);
            // Create Role
            var role = new Role()
            {
                Code = roleCount,
                Value = roleDTO.Value
            };
            _unit.GetRepository<Role>().Insert(role);
            _unit.SaveChanges();
            // Done
            return role;
        }

        public Role Edit(RoleEditDTO roleDTO, ApplicationUser currentUser)
        {
            // Find role
            var role = GetByCode(roleDTO.Code);
            // Validate
            bool isValidEdit = IsValidEdit(roleDTO.Code, roleDTO.Value, role, currentUser);
            if (currentUser == null)
                throw new ArgumentNullException("The role needs to be edited by a user.");
            // Edit
            role.Value = roleDTO.Value;
            _unit.GetRepository<Role>().Update(role);
            _unit.SaveChanges();
            // Done
            return role;
        }

        public bool Delete(int roleId)
        {
            // Find role
            var role = GetById(roleId);
            bool isValid = isValidDelete(role);
            // Delete
            _unit.GetRepository<Role>().Delete(role);
            _unit.SaveChanges();
            // Done
            return true;
        }

        public Role GetById(int id)
        {
            Role role = _unit.GetRepository<Role>().GetById(id);
            if (role == null)
                throw new InvalidOperationException("Role was not found.");
            return role;
        }

        public Role GetByCode(int roleCode)
        {
            Role? role = _unit.GetRepository<Role>().Get(r => r.Code == roleCode).FirstOrDefault();
            if (role == null)
                throw new InvalidOperationException("Role was not found.");
            return role;
        }

        public Role GetByName(string roleName)
        {
            Role? role = _unit.GetRepository<Role>().Get(r => r.Value == roleName).FirstOrDefault();
            if (role == null)
                throw new InvalidOperationException("Role was not found.");
            return role;
        }

        public IEnumerable<Role> GetAll(FilterRoleDTO filterRoleDTO, ApplicationUser user)
        {
            Expression<Func<Role, bool>>? filter = FilterAll(filterRoleDTO, user);
            var roles = _unit.GetRepository<Role>()
                .Get(filter, "", filterRoleDTO.PageNumber, filterRoleDTO.PageSize).OrderBy(r => r.Code).ToList();
            return roles;
        }

        public int CountAll(FilterRoleDTO filterRoleDTO, ApplicationUser user)
        {
            Expression<Func<Role, bool>>? filter = FilterAll(filterRoleDTO, user);
            return _unit.GetRepository<Role>().Count(filter);
        }

        private Expression<Func<Role, bool>>? FilterAll(FilterRoleDTO filterRoleDTO, ApplicationUser user)
        {
            string? value = filterRoleDTO.Search;
            Expression<Func<Role, bool>>? filter =
                string.IsNullOrEmpty(value) ? r => true : r => r.Value.ToLower().Contains(value.ToLower());
            return FilterForUser(filter, user);
        }

        private Expression<Func<Role, bool>>? FilterForUser(Expression<Func<Role, bool>>? filter, ApplicationUser user)
        {
            if (!(user.Role.Code == (int)RoleEnums.Admin))
                throw new InvalidOperationException("Unauthorized.");
            return filter;
        }

        public bool AssignRoleToUser(int roleId, int userId)
        {
            // Find role
            var role = GetById(roleId);
            // Find user
            var user = _unit.GetRepository<ApplicationUser>().Get(u => u.Id == userId).FirstOrDefault();
            if (user == null)
                throw new InvalidOperationException("User was not found");
            // Assign role to user
            user.RoleId = roleId;
            _unit.GetRepository<ApplicationUser>().Update(user);
            _unit.SaveChanges();
            // Done
            return true;
        }

        private bool IsValidRole(int code, string value, ApplicationUser currentUser)
        {
            // User
            if (currentUser.Role.Code != (int)RoleEnums.Admin)
                throw new InvalidOperationException("Unauthorized.");
            // Role
            Role roleCode = _unit.GetRepository<Role>().Get(r => r.Code == code).FirstOrDefault();
            if (roleCode != null)
                throw new InvalidOperationException($"Another role with this code exists. Role: {roleCode.Code} - {roleCode.Value}");
            Role roleValue = _unit.GetRepository<Role>().Get(r => r.Value == value).FirstOrDefault();
            if (roleValue != null)
                throw new InvalidOperationException($"Another role with this value exists. Role: {roleValue.Code} - {roleValue.Value}");
            return true;
        }

        private bool IsValidEdit(int code, string value, Role role, ApplicationUser currentUser)
        {
            // User
            if (currentUser.Role.Code != (int)RoleEnums.Admin)
                throw new InvalidOperationException("Unauthorized.");
            // Role
            Role roleCode = _unit.GetRepository<Role>().Get(r => r.Code == code).FirstOrDefault(); ;
            if (roleCode != null && roleCode.Code != role.Code)
                throw new InvalidOperationException($"Another role with this code exists. Role: {roleCode.Code} - {roleCode.Value}");
            Role roleValue = _unit.GetRepository<Role>().Get(r => r.Value == value).FirstOrDefault();
            if (roleValue != null && roleValue.Value != role.Value)
                throw new InvalidOperationException($"Another role with this value exists. Role: {roleValue.Code} - {roleValue.Value}");
            return true;
        }

        private bool isValidDelete(Role role)
        {
            if (role.IsSeeded)
                throw new InvalidOperationException("Cannot delete a default role.");
            int numberOfUsers = _unit.GetRepository<ApplicationUser>().Count(u => u.RoleId == role.Id);
            if (numberOfUsers > 0)
                throw new InvalidOperationException($"Cannot delete the role since there are {numberOfUsers} users in it.");
            return true;
        }
    }
}
