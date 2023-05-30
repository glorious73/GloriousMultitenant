using Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Contracts
{
    public class ApplicationUser : Base.Entity
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [ForeignKey("Role")]
        public int? RoleId { get; set; }
        public virtual Role Role { get; set; }
        public string? Token { get; set; } // Json Web Token
        public string? PasswordResetToken { get; set; }
        public string? PasswordOTP { get; set; }
        public DateTime LastLogin { get; set; }
        public int NumberOfLogins { get; set; }
        public bool IsSeeded { get; set; }
    }
}
