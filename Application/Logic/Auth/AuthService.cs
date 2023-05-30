using Application.DTO.Auth;
using Application.Infrastructure.AppData;
using Application.Logic.Account;
using Data.UnitOfWork;
using Entity.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logic.Auth
{
    public class AuthService : IAuthService
    {
        private IConfiguration _config;
        private IAppData _appData;
        private IUnit _unit;
        private IAccountService _accountService;

        public AuthService(IConfiguration config, IAppData appData, IUnit unit, IAccountService accountService)
        {
            _config = config;
            _appData = appData;
            _unit = unit;
            _accountService = accountService;
        }

        public ApplicationUser Authenticate(LoginDTO loginDTO)
        {
            var user = GetUserByLoginCredentials(loginDTO);
            if (!VerifyPassword(loginDTO.Password, user.PasswordHash, Convert.FromBase64String(user.PasswordSalt)))
                throw new InvalidOperationException("Email address or Password is incorrect.");
            bool result = UpdateUserRecord(user);
            user.Token = GenerateJSONWebToken(user);
            // Done
            return user;
        }

        public bool ResetPassword(ResetPasswordDTO resetPasswordDTO, ApplicationUser user)
        {
            // 1. Validate code
            if (user.PasswordResetToken != resetPasswordDTO.PasswordResetToken)
                throw new InvalidOperationException("An error occured. Please click on the link in your email again.");
            // 2. Match passwords
            if (resetPasswordDTO.NewPassword != resetPasswordDTO.ConfirmNewPassword)
                throw new InvalidOperationException("Passwords don't match.");
            // 3. Hash user password
            HashPasswordDTO hashPasswordDTO = _accountService.HashPassword(resetPasswordDTO.NewPassword);
            // 4. Update Password Hash
            user.PasswordHash = hashPasswordDTO.Hash;
            user.PasswordSalt = Convert.ToBase64String(hashPasswordDTO.Salt);
            user.PasswordResetToken = null;
            // 5. Update DB
            _unit.GetRepository<ApplicationUser>().Update(user);
            _unit.SaveChanges();
            // Done
            return true;
        }

        public bool ChangePassword(ChangePasswordDTO changePasswordDTO, ApplicationUser user)
        {
            // 1. Validate code
            if (user.PasswordOTP != changePasswordDTO.OTP)
                throw new InvalidOperationException("An error occured. Please double check the OTP.");
            // 2. Match passwords
            if (changePasswordDTO.NewPassword != changePasswordDTO.ConfirmNewPassword)
                throw new InvalidOperationException("Passwords don't match.");
            // 3. Hash user password
            HashPasswordDTO hashPasswordDTO = _accountService.HashPassword(changePasswordDTO.NewPassword);
            // 4. Update Password Hash
            user.PasswordHash = hashPasswordDTO.Hash;
            user.PasswordSalt = Convert.ToBase64String(hashPasswordDTO.Salt);
            user.PasswordOTP = null;
            // 5. Update DB
            _unit.GetRepository<ApplicationUser>().Update(user);
            _unit.SaveChanges();
            // Done
            return true;
        }

        private ApplicationUser GetUserByLoginCredentials(LoginDTO loginDTO)
        {
            string email = loginDTO.Email.ToLower();
            string username = loginDTO.Username.ToLower();
            var repository = _unit.GetRepository<ApplicationUser>();
            var user = repository.Get(u => u.Email == email, nameof(Role)).FirstOrDefault();
            if (user == null)
                user = repository.Get(u => u.Username == username, nameof(Role)).FirstOrDefault();
            // User not found (Don't show message for security reasons)
            if (user == null)
                throw new InvalidOperationException("Email address or Password is incorrect.");
            return user;
        }

        private bool VerifyPassword(string password, string passwordHash, byte[] passwordSalt)
        {
            string testPasswordHash = _accountService.HashPassword(password, passwordSalt).Hash;
            return (testPasswordHash == passwordHash);
        }

        private bool UpdateUserRecord(ApplicationUser user)
        {
            user.LastLogin = DateTime.UtcNow;
            user.NumberOfLogins += 1;
            _unit.GetRepository<ApplicationUser>().Update(user);
            _unit.SaveChanges();
            return true;
        }

        private string GenerateJSONWebToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // Signing & Encryption
            var encryptionKey = _appData.EncryptionKey;
            var signingKey = _appData.SigningKey;
            var secretKid = _config.GetSection("Jwt:SecretKeyId").Get<string>();
            var encryptionKid = _config.GetSection("Jwt:EncryptionKeyId").Get<string>();

            var publicEncryptionKey = new RsaSecurityKey(encryptionKey.ExportParameters(false)) { KeyId = encryptionKid };
            var privateSigningKey = new ECDsaSecurityKey(signingKey) { KeyId = secretKid };
            // claims
            var claims = new Dictionary<string, object>()
            {
                {  ClaimTypes.Name, user.Id.ToString() },
                {  ClaimTypes.Email, user.Email },
                {  ClaimTypes.Role, user.Role.Value }
            };
            // Token
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Issuer = _config.GetSection("JWT:Issuer").Get<string>(),
                Audience = null,
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    privateSigningKey, SecurityAlgorithms.EcdsaSha256),
                EncryptingCredentials = new EncryptingCredentials(
                    publicEncryptionKey, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512),
                Claims = claims
            });
            return tokenHandler.WriteToken(token);
        }
    }
}
