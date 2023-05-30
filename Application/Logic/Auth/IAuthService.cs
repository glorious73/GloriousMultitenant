using Application.DTO.Auth;
using Entity.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logic.Auth
{
    public interface IAuthService
    {
        ApplicationUser Authenticate(LoginDTO loginDTO);
        bool ResetPassword(ResetPasswordDTO resetPasswordDTO, ApplicationUser user);
        bool ChangePassword(ChangePasswordDTO changePasswordDTO, ApplicationUser user);
    }
}
