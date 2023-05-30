using Application.Logic.Account;
using Application.Logic;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTO.Role;
using Application.Infrastructure.Result;
using Application.ViewModel.Role;
using Entity.Contracts;

namespace WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : Controller
    {
        private IRoleService _roleService;
        private IAccountService _accountService;
        private IMapper _mapper;

        public RoleController(IRoleService roleService, IAccountService accountService, IMapper mapper)
        {
            _roleService = roleService;
            _accountService = accountService;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult Create([FromBody] RoleDTO requestRole)
        {
            // Get current user
            ApplicationUser currentUser = _accountService.GetCurrentUser(this.User);
            // Create new role
            Role role = _roleService.Create(requestRole, currentUser);
            // Done
            return Created("", new OperationResult() { Success = true, Result = new { role = _mapper.Map<RoleViewModel>(role) } });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] RoleEditDTO requestRole)
        {
            // Get current user
            ApplicationUser currentUser = _accountService.GetCurrentUser(this.User);
            // Edit role
            Role role = _roleService.Edit(requestRole, currentUser);
            // Done
            return Ok(new OperationResult() { Success = true, Result = new { role = _mapper.Map<RoleViewModel>(role) } });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool result = _roleService.Delete(id);
            return Ok(new OperationResult() { Success = true, Result = new { message = "role was deleted." } });
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            // 1. Get current user
            ApplicationUser currentUser = _accountService.GetCurrentUser(this.User);
            // 2. Get role data
            var role = _roleService.GetById(id);
            return Ok(new OperationResult() { Success = true, Result = new { role = _mapper.Map<RoleViewModel>(role) } });
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll([FromQuery] FilterRoleDTO filterRoleDTO)
        {
            // 1. Get Current user
            ApplicationUser currentUser = _accountService.GetCurrentUser(this.User);
            // 2. Get available roles
            var roles = _roleService.GetAll(filterRoleDTO, currentUser);
            // 3. Prepare Result
            int totalItems = _roleService.CountAll(filterRoleDTO, currentUser);
            var result = new
            {
                roles = _mapper.Map<List<RoleViewModel>>(roles.ToList()),
                pageNumber = filterRoleDTO.PageNumber,
                pageSize = filterRoleDTO.PageSize,
                totalItems = totalItems,
                totalPages = (int)Math.Ceiling(totalItems / (double)filterRoleDTO.PageSize)
            };
            return Ok(new OperationResult() { Success = true, Result = result });
        }

        [HttpPut("{roleId}/user/{userId}")]
        public IActionResult Assign(int roleId, int userId)
        {
            // Get current user
            ApplicationUser currentUser = _accountService.GetCurrentUser(this.User);
            // Assign role to user
            bool result = _roleService.AssignRoleToUser(roleId, userId);
            // Done
            return Ok(new OperationResult() { Success = true, Result = new { message = "Role was assigned to user." } });
        }
    }
}
