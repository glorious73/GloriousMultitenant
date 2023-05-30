using Application.DTO.Account;
using Application.DTO.Auth;
using Data.UnitOfWork;
using Entity.Contracts;
using Entity.Enums;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utility;

namespace Application.Logic.Account
{
    public class AccountService : IAccountService
    {
        private IConfiguration _config { get; }
        private IUnit _unit { get; set; }
        private IRoleService _roleService { get; set; }
        private IExpressionUtility _expressionUtility;

        public AccountService(IConfiguration config, IUnit unit, IRoleService roleService, IExpressionUtility expressionUtility)
        {
            _unit = unit;
            _config = config;
            _roleService = roleService;
            _expressionUtility = expressionUtility;
        }

        public ApplicationUser Create(UserDTO userDTO, string RandomPassword, ApplicationUser currentUser)
        {
            bool isValidUser = IsValidUser(userDTO, currentUser);
            if (!isValidUser)
                throw new Exception("User data is not valid. Kindly correct the errors in the form.");
            // Create User
            var role = _roleService.GetByCode(userDTO.RoleCode);
            var user = new ApplicationUser()
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email.ToLower(),
                PhoneNumber = userDTO.PhoneNumber,
                Username = (!string.IsNullOrEmpty(userDTO.Username)) ? userDTO.Username.ToLower() : userDTO.Email,
                RoleId = role.Id
            };
            // Hash password
            HashPasswordDTO hashResult = HashPassword(RandomPassword);
            user.PasswordHash = hashResult.Hash;
            user.PasswordSalt = Convert.ToBase64String(hashResult.Salt);
            // Regitser user
            _unit.GetRepository<ApplicationUser>().Insert(user);
            _unit.SaveChanges();
            // Done
            return user;
        }

        public ApplicationUser Edit(int accountId, UserEditDTO userDTO, ApplicationUser currentUser)
        {
            // Find
            ApplicationUser user = GetById(accountId);
            // Validate
            bool result = IsValidEdit(userDTO, user);
            // Edit
            user.FirstName = userDTO.FirstName;
            user.LastName = userDTO.LastName;
            user.Username = (!string.IsNullOrEmpty(userDTO.Username)) ? userDTO.Username.ToLower() : userDTO.Email;
            user.Email = userDTO.Email;
            user.PhoneNumber = userDTO.PhoneNumber;
            user.RoleId = _roleService.GetByCode(userDTO.RoleCode).Id;
            user.UpdatedAt = DateTime.UtcNow;
            // Update
            _unit.GetRepository<ApplicationUser>().Update(user);
            _unit.SaveChanges();
            // Done
            return user;
        }

        public bool Delete(int accountId)
        {
            var account = GetById(accountId);
            bool isValid = isValidDelete(account);
            account.isDeleted = true;
            _unit.SaveChanges();
            // Done
            return true;
        }

        public IEnumerable<ApplicationUser> GetAll(FilterUserDTO filterUserDTO, ApplicationUser user)
        {
            Expression<Func<ApplicationUser, bool>>? filter = FilterAll(filterUserDTO, user);
            // Get users
            var users = _unit.GetRepository<ApplicationUser>()
                    .Get(filter, nameof(Role), filterUserDTO.PageNumber, filterUserDTO.PageSize).ToList();
            // return users
            return users;
        }

        public int CountAll(FilterUserDTO filterUserDTO, ApplicationUser user)
        {
            Expression<Func<ApplicationUser, bool>>? filter = FilterAll(filterUserDTO, user);
            return _unit.GetRepository<ApplicationUser>().Count(filter);
        }

        private Expression<Func<ApplicationUser, bool>>? FilterAll(FilterUserDTO filterUserDTO, ApplicationUser user)
        {
            // Get filter values
            string? email = filterUserDTO.Search;
            // Build Expressions
            Expression<Func<ApplicationUser, bool>>? filterEmail =
                (string.IsNullOrEmpty(email)) ? u => true : u => u.Email.ToLower().Contains(email.ToLower());
            Expression<Func<ApplicationUser, bool>>? filterRole =
                (filterUserDTO.RoleId == 0) ? u => true : u => u.RoleId == filterUserDTO.RoleId;
            // Return
            Expression<Func<ApplicationUser, bool>>? filter = _expressionUtility.AndAlso<ApplicationUser>(filterEmail, filterRole);
            return FilterForUser(filter, user);
        }

        private Expression<Func<ApplicationUser, bool>>? FilterForUser(Expression<Func<ApplicationUser, bool>>? filter, ApplicationUser user)
        {
            if (!(user.Role.Code == (int)RoleEnums.Admin))
                throw new Exception("Unauthorized.");
            return filter;
        }

        public ApplicationUser GetById(int Id)
        {
            var user = _unit.GetRepository<ApplicationUser>().Get(u => u.Id == Id, nameof(Role)).FirstOrDefault();
            if (user == null)
                throw new InvalidOperationException("User was not found.");
            // Done
            return user;
        }

        public ApplicationUser GetByEmail(string Email)
        {
            var user = _unit.GetRepository<ApplicationUser>().Get(u => u.Email == Email, nameof(Role)).FirstOrDefault();
            if (user == null)
                throw new InvalidOperationException("User was not found.");
            // Done
            return user;
        }

        public ApplicationUser GetByPasswordResetToken(string token)
        {
            var user = _unit.GetRepository<ApplicationUser>().Get(u => u.PasswordResetToken == token).FirstOrDefault();
            if (user == null)
                throw new InvalidOperationException("User was not found.");
            // Done
            return user;
        }

        public ApplicationUser GetCurrentUser(ClaimsPrincipal claimsPrincipal)
        {
            ApplicationUser currentUser = null;
            var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.Name);
            if (claim == null)
                throw new Exception("Could not get current user. Please contact administrator.");
            int userId = Convert.ToInt32(claim.Value);
            currentUser = GetById(userId);
            return currentUser;
        }

        public string CreatePasswordResetToken(ApplicationUser user)
        {
            // Generate
            user.PasswordResetToken = Guid.NewGuid().ToString();
            // Save
            _unit.GetRepository<ApplicationUser>().Update(user);
            _unit.SaveChanges();
            // Done
            return user.PasswordResetToken;
        }

        public string CreateChangePasswordOTP(ApplicationUser user)
        {
            // Generate
            var chars = "0123456789";
            string otp = new string(chars.Select(c => chars[new Random().Next(chars.Length)]).Take(6).ToArray());
            user.PasswordOTP = otp;
            // Save
            _unit.GetRepository<ApplicationUser>().Update(user);
            _unit.SaveChanges();
            // Done
            return user.PasswordOTP;
        }

        public HashPasswordDTO HashPassword(string password, byte[] salt = null)
        {
            // generate a 128-bit salt using a secure PRNG
            if (salt == null)
            {
                salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
            }
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            // Done
            return new HashPasswordDTO() { Hash = hashed, Salt = salt };
        }

        private bool IsValidUser(UserDTO userDTO, ApplicationUser currentUser)
        {
            // Validate Email
            try
            {
                MailAddress mailAddress = new MailAddress(userDTO.Email);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Please enter a valid email address.");
            }
            // Validate Phone number (Optional)
            if (userDTO.PhoneNumber != null)
                if (!Regex.IsMatch(userDTO.PhoneNumber, "^966[0-9]{9}$"))
                    throw new InvalidOperationException("Phone number must start with a \"966\" followed by 9 digits.");
            // Validate Username
            if (!string.IsNullOrEmpty(userDTO.Username) && userDTO.Username.Contains(" "))
                throw new InvalidOperationException("Username cannot contain spaces.");
            // Validate Role
            if (userDTO.RoleCode == 0)
                throw new InvalidOperationException("Please choose a valid role code.");
            if (userDTO.RoleCode == (int)RoleEnums.Admin && currentUser.Role.Code != (int)RoleEnums.Admin)
                throw new InvalidOperationException("New admin user requires authorization.");
            // Validate duplicates
            var existentUserByEmail = _unit.GetRepository<ApplicationUser>().Get(u => u.Email == userDTO.Email).FirstOrDefault();
            var existentUserByUsername = _unit.GetRepository<ApplicationUser>().Get(u => u.Username == userDTO.Username).FirstOrDefault();
            if (existentUserByEmail != null)
                throw new InvalidOperationException("Email already exists.");
            if (existentUserByUsername != null)
                throw new InvalidOperationException("Username already exists.");
            return true;
        }

        private bool IsValidEdit(UserEditDTO userDTO, ApplicationUser user)
        {
            if (user.Username == "admin")
                throw new InvalidOperationException("Cannot edit the information of the default user.");
            // Validate Email
            try
            {
                MailAddress mailAddress = new MailAddress(userDTO.Email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excption with entered email from user. {ex.Message}");
                throw new InvalidOperationException($"Please enter a valid email.");
            }
            // Validate Phone number (Optional)
            if (userDTO.PhoneNumber != null)
                if (!Regex.IsMatch(userDTO.PhoneNumber, "^966[0-9]{9}$"))
                    throw new InvalidOperationException("Phone number must start with a \"966\" followed by 9 digits.");
            // Validate Username
            if (!string.IsNullOrEmpty(userDTO.Username) && userDTO.Username.Contains(" "))
                throw new InvalidOperationException("Username cannot contain spaces.");
            // Validate Role
            if (userDTO.RoleCode == 0)
                throw new InvalidOperationException("Please choose a valid role.");
            // Validate duplicates
            var existentByEmail = _unit.GetRepository<ApplicationUser>().Get(u => u.Email == userDTO.Email).FirstOrDefault();
            var existentByUsername = _unit.GetRepository<ApplicationUser>().Get(u => u.Username == userDTO.Username).FirstOrDefault();
            if (existentByEmail != null && existentByEmail.Email != userDTO.Email)
                throw new InvalidOperationException("Email already exists.");
            if (existentByUsername != null && existentByUsername.Username != userDTO.Username)
                throw new InvalidOperationException("Username already exists.");
            return true;
        }

        private bool isValidDelete(ApplicationUser user)
        {
            // Admin
            if (user.Username == "admin")
                throw new InvalidOperationException("Cannot delete default account.");
            if (user.Role.Code == (int)RoleEnums.Admin)
                return true;
            return true;
        }
    }
}
