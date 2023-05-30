using Data.BaseRepository;
using Entity.Base;
using Entity.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.UnitOfWork
{
    public class Unit : IUnit
    {
        private ApplicationDbContext DbContext;
        public Unit(ApplicationDbContext applicationDbContext)
        {
            DbContext = applicationDbContext;
        }
        public Repository<T> GetRepository<T>() where T : class, IEntity
        {
            return new Repository<T>(DbContext);
        }
        public void Dispose()
        {
            DbContext.Dispose();
        }
        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }
    }
}
