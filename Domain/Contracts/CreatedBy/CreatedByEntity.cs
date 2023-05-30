using Entity.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Contracts.CreatedBy
{
    public class CreatedByEntity : Entity.Base.Entity
    {
        [ForeignKey("CreatedBy")]
        public int? CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
    }
}
