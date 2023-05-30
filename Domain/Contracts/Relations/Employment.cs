using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Contracts
{
    public class Employment : Base.Entity
    {
        [ForeignKey("Organization")]
        public int? OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        [ForeignKey("ApplicationUser")]
        public int? ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
