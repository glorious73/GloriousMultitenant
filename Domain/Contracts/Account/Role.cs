using Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Contracts
{
    public class Role : ReferenceEntity
    {
        public bool IsSeeded { get; set; }
    }
}
