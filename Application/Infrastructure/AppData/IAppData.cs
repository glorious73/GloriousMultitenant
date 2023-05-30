using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Infrastructure.AppData
{
    public interface IAppData
    {
        RSA? EncryptionKey { get; set; }
        ECDsa? SigningKey { get; set; }
    }
}
