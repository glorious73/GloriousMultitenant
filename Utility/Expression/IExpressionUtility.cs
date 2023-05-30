using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public interface IExpressionUtility
    {
        Expression<Func<T, bool>> AndAlso<T>(Expression<Func<T, bool>> source, Expression<Func<T, bool>> expression)
            where T : class;
    }
}
