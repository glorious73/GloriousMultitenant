using Entity.Contracts.CreatedBy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Contracts
{
    public class Transaction : CreatedByEntity
    {
        public double Amount { get; set; }
        public string Currency { get; set; }
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        [ForeignKey("TransactionType")]
        public int TransactionTypeId { get; set; }
        public virtual TransactionType TransactionType { get; set; }
    }
}
