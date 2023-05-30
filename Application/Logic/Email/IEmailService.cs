using Application.DTO.Account;
using Entity.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logic.Email
{
    public interface IEmailService
    {
        Task<bool> SendRegistrationEmail(UserDTO user, string password);
        Task<bool> SendResetPasswordEmail(ApplicationUser user);
        Task<bool> SendChangePasswordOTPEmail(ApplicationUser user);
    }
}
