using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Auth
{
    public class HashPasswordDTO
    {
        public string Hash { get; set; }
        public byte[] Salt { get; set; }
    }
}
