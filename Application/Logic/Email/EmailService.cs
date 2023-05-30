using Application.DTO.Account;
using Data.UnitOfWork;
using Entity.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Email;

namespace Application.Logic.Email
{
    public class EmailService : IEmailService
    {
        private IUnit _unit { get; set; }
        private IConfiguration _configuration { get; set; }
        private IEmailUtility _emailUtility { get; set; }

        public EmailService(IUnit unit, IConfiguration configuration, IEmailUtility emailUtility)
        {
            _unit = unit;
            _configuration = configuration;
            _emailUtility = emailUtility;
        }

        public async Task<bool> SendRegistrationEmail(UserDTO user, string password)
        {
            string emailBody = $"<div>Dear {user.FirstName} {user.LastName} <br/><br/>Welcome to Glorious Multitenant. Please find your login information below:<br/>Email: {user.Email} <br/>Username: {user.Username} <br/>Password: {password} <br/><br/>To access the website, please click <a href='{_configuration.GetSection("WebLink").Get<string>()}'>here</a>.<br/><br/>Glorious Multitenant</div>";
            bool result = await _emailUtility.SendMailJetEmail(user.Email, "Account Created", emailBody);
            // Done
            return result;
        }

        public async Task<bool> SendResetPasswordEmail(ApplicationUser user)
        {
            string emailBody = $"<div>Dear user,<br/>To reset your password, please <a href='{_configuration.GetSection("WebLink").Get<string>()}{_configuration.GetSection("ResetPasswordPath").Get<string>()}?token={user.PasswordResetToken}'>click here</a>.<br/><br/>If you haven't requested to reset your password, you can safely ignore this email.<br/><br/>Glorious Multitenant</div>";
            bool result = await _emailUtility.SendMailJetEmail(user.Email, "Reset Password", emailBody);
            return result;
        }

        public async Task<bool> SendChangePasswordOTPEmail(ApplicationUser user)
        {
            string emailBody = $"<div>Dear user,<br/>To change your password, please use the OTP {user.PasswordOTP}.<br/><br/>If you haven't requested to change your password, you can safely ignore this email.<br/><br/>Glorious Multitenant</div>";
            bool result = await _emailUtility.SendMailJetEmail(user.Email, "Change Password", emailBody);
            return result;
        }
    }
}
