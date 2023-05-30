using Entity.Contracts.CreatedBy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Contracts
{
    public class License : CreatedByEntity
    {
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        [ForeignKey("LicenseType")]
        public int LicenseTypeId { get; set; }
        public virtual LicenseType LicenseType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
