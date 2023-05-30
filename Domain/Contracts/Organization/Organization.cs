using Entity.Contracts.CreatedBy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Contracts
{
    public class Organization : CreatedByEntity
    {
        public string Name { get; set; }
        public string CommercialRegistration { get; set; }

    }
}
