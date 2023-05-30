using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Infrastructure.AppData
{
    public class AppData : IAppData
    {
        public RSA? EncryptionKey { get; set; }
        public ECDsa? SigningKey { get; set; }
    }
}
