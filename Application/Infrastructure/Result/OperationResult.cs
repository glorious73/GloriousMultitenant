using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Infrastructure.Result
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public object? Result { get; set; }
    }
}
