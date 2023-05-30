using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Base
{
    public class ReferenceEntity : Base.Entity
    {
        public int Code { get; set; }
        public string Value { get; set; }
    }
}
