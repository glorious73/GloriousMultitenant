using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Infrastructure.Result
{
    public interface IOperationResult<T>
    {
        bool Success { get; set; }
        T Result { get; set; }
    }
}
