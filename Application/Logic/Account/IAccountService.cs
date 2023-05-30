using Application.DTO.Account;
using Application.DTO.Auth;
using Entity.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logic.Account
{
    public interface IAccountService
    {
        ApplicationUser Create(UserDTO RequestUser, string RandomPassword, ApplicationUser currentUser);
        ApplicationUser Edit(int accountId, UserEditDTO userToEdit, ApplicationUser user);
        ApplicationUser GetById(int Id);
        ApplicationUser GetByEmail(string Email);
        ApplicationUser GetByPasswordResetToken(string token);
        ApplicationUser GetCurrentUser(ClaimsPrincipal claimsPrincipal);
        IEnumerable<ApplicationUser> GetAll(FilterUserDTO filterUserDTO, ApplicationUser user);
        int CountAll(FilterUserDTO filterUserDTO, ApplicationUser user);
        bool Delete(int accountId);
        string CreatePasswordResetToken(ApplicationUser user);
        string CreateChangePasswordOTP(ApplicationUser user);
        HashPasswordDTO HashPassword(string password, byte[] salt = null);
    }
}
