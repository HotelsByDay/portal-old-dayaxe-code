using System;
using System.Collections.Generic;

namespace DayaxeDal.Repositories
{
    public interface IRepository<TEntity>
    {
        IEnumerable<TEntity> Get(Func<TEntity, bool> criteria);
        int Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void Delete(Func<TEntity, Boolean> predicate);
        TEntity GetById(long id);
        IEnumerable<TEntity> GetAll();
        TEntity Refresh(TEntity entity);
    }
}
