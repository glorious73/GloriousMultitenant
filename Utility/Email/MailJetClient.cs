using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Email
{
    public class MailJetClient
    {
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public string MJ_APIKEY_PUBLIC { get; set; }
        public string MJ_APIKEY_PRIVATE { get; set; }
    }
}
