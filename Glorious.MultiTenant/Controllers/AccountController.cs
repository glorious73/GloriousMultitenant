using Application.DTO.Account;
using Application.Infrastructure.Result;
using Application.Logic.Account;
using Application.Logic.Email;
using Application.ViewModel.Account;
using AutoMapper;
using Entity.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private IAccountService _accountService;
        private IEmailService _emailService;
        private readonly IMapper _mapper;

        public AccountController(IAccountService accountService, IEmailService emailService, IMapper mapper)
        {
            _accountService = accountService;
            _emailService = emailService;
            _mapper = mapper;

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDTO requestUser)
        {
            // Get current user
            ApplicationUser currentUser = _accountService.GetCurrentUser(this.User);
            // Generate password
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            string userPassword = new string(chars.Select(c => chars[new Random().Next(chars.Length)]).Take(8).ToArray());
            // Register new user
            ApplicationUser user = _accountService.Create(requestUser, userPassword, currentUser);
            // Send Registration Email (to become async)
            try
            {
                bool result = await _emailService.SendRegistrationEmail(requestUser, userPassword);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send email. {ex.Message}");
            }
            // Done
            return Created("", new OperationResult() { Success = true, Result = new { user = _mapper.Map<UserViewModel>(user) } });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UserEditDTO user)
        {
            // Get current user
            ApplicationUser currentUser = _accountService.GetCurrentUser(this.User);
            var editedUser = _accountService.Edit(id, user, currentUser);
            return Ok(new OperationResult() { Success = true, Result = new { user = _mapper.Map<UserViewModel>(editedUser) } });
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool result = _accountService.Delete(id);
            return Ok(new OperationResult() { Success = true, Result = new { message = "user account was deleted." } });
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            // 1. Get current user
            ApplicationUser currentUser = _accountService.GetCurrentUser(this.User);
            // 2. Get user data
            var user = _accountService.GetById(id);
            return Ok(new OperationResult() { Success = true, Result = new { user = _mapper.Map<UserViewModel>(user) } });
        }


        [HttpGet]
        public IActionResult GetAll([FromQuery] FilterUserDTO filterUserDTO)
        {
            // 1. Get Current user
            ApplicationUser currentUser = _accountService.GetCurrentUser(this.User);
            // 2. Get available users
            var users = _accountService.GetAll(filterUserDTO, currentUser);
            // 3. Prepare Result
            int totalItems = _accountService.CountAll(filterUserDTO, currentUser);
            var result = new
            {
                users = _mapper.Map<List<UserViewModel>>(users.ToList()),
                pageNumber = filterUserDTO.PageNumber,
                pageSize = filterUserDTO.PageSize,
                totalItems = totalItems,
                totalPages = (int)Math.Ceiling(totalItems / (double)filterUserDTO.PageSize)
            };
            return Ok(new OperationResult() { Success = true, Result = result });
        }
    }
}
