using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Email
{
    public interface IEmailUtility
    {
        string From { get; set; }
        Task<bool> SendMailJetEmail(string to, string subject, string body);
    }
}
