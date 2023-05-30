using Entity.Base;
using Entity.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.BaseRepository
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        public ApplicationDbContext context;
        public DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        /// <summary>
        /// get a list
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Get(
          Expression<Func<T, bool>> filter = null,
          string includeProperties = "", int pageNumber = 0, int pageSize = 20, bool orderByDate = false)
        {
            IQueryable<T> query = dbSet;
            // Filter
            if (filter != null)
                query = query.Where(filter);
            // Include
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);
            // Paginate
            if (pageNumber > 0)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            // Order By
            return (orderByDate) ? query.OrderBy(t => t.CreatedAt).AsEnumerable() : query.AsEnumerable();

        }

        public virtual T GetById(object id)
        {
            return dbSet.Find(id);
        }

        /// <summary>
        /// count a list
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>count of items in the query</returns>
        public virtual int Count(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
                return query.Count(filter);

            return query.Count();
        }

        public virtual void Insert(T entity)
        {
            dbSet.Add(entity);
        }
        public virtual void Insert(List<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public virtual void Delete(object id)
        {
            T entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(T entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(T entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual void Update(List<T> entitiesToUpdate)
        {
            context.UpdateRange(entitiesToUpdate);
        }

        public virtual bool Any(Expression<Func<T, bool>> filter)
        {
            return dbSet.Any(filter);
        }
    }
}
