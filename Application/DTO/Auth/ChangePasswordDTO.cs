using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Auth
{
    public class ChangePasswordDTO
    {
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
        public string OTP { get; set; }
    }
}
