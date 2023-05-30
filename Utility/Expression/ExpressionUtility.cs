using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class ExpressionUtility : IExpressionUtility
    {
        public ExpressionUtility() { }

        public Expression<Func<T, bool>> AndAlso<T>(Expression<Func<T, bool>> source, Expression<Func<T, bool>> expression)
            where T : class
        {
            // Combine
            var parameter = Expression.Parameter(typeof(T));
            var filterbody = Expression.AndAlso(
                Expression.Invoke(source, parameter),
                Expression.Invoke(expression, parameter));
            var filter = Expression.Lambda<Func<T, bool>>(filterbody, parameter);
            // Return
            return filter;
        }
    }
}
