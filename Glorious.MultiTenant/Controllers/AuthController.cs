using Application.DTO.Auth;
using Application.Infrastructure.Result;
using Application.Logic.Account;
using Application.Logic.Auth;
using Application.Logic.Email;
using Application.ViewModel.Auth;
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
    public class AuthController : Controller
    {
        private IAuthService _authService;
        private IAccountService _accountService;
        private IEmailService _emailService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IAccountService accountService, IEmailService emailService, IMapper mapper)
        {
            _authService = authService;
            _accountService = accountService;
            _emailService = emailService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO requestUser)
        {
            ApplicationUser user = _authService.Authenticate(requestUser);
            return Ok(new OperationResult() { Success = true, Result = new { user = _mapper.Map<LoginViewModel>(user) } });
        }

        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            ApplicationUser currentUser = _accountService.GetByEmail(forgotPasswordDTO.Email);
            string passwordResetToken = _accountService.CreatePasswordResetToken(currentUser);
            bool result = await _emailService.SendResetPasswordEmail(currentUser);
            return Ok(new OperationResult() { Success = true, Result = new { message = $"Reset password email was sent to {currentUser.Email}." } });

        }

        [AllowAnonymous]
        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            ApplicationUser currentUser = _accountService.GetByPasswordResetToken(resetPasswordDTO.PasswordResetToken);
            bool result = _authService.ResetPassword(resetPasswordDTO, currentUser);
            return Ok(new OperationResult() { Success = true, Result = new { message = $"Password was reset successfully." } });
        }

        [HttpPut("ChangePasswordOTP")]
        public async Task<IActionResult> ChangePasswordOTP()
        {
            ApplicationUser currentUser = _accountService.GetCurrentUser(this.User);
            string otp = _accountService.CreateChangePasswordOTP(currentUser);
            bool result = await _emailService.SendChangePasswordOTPEmail(currentUser);
            return Ok(new OperationResult() { Success = true, Result = new { message = $"OTP was sent to {currentUser.Email}." } });
        }

        [HttpPut("ChangePassword")]
        public IActionResult ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            ApplicationUser currentUser = _accountService.GetCurrentUser(this.User);
            bool result = _authService.ChangePassword(changePasswordDTO, currentUser);
            return Ok(new OperationResult() { Success = true, Result = new { message = $"Password was changed successfully." } });
        }
    }
}
