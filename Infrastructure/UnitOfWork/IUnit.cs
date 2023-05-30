using Data.BaseRepository;
using Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.UnitOfWork
{
    public interface IUnit
    {
        Repository<T> GetRepository<T>() where T : class, IEntity;
        int SaveChanges();
        void Dispose();
    }
}
