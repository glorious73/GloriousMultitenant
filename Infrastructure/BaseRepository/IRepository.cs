using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.BaseRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, string includeProperties = "", int pageNumber = 0, int pageSize = 20, bool orderByDate = false);

        T GetById(object id);
        void Insert(T entity);
        void Insert(List<T> entities);

        void Delete(object id);

        void Delete(T entityToDelete);

        void Update(T entityToUpdate);
        void Update(List<T> entitiesToUpdate);
        bool Any(Expression<Func<T, bool>> filter);
        int Count(Expression<Func<T, bool>> filter = null);
    }
}
