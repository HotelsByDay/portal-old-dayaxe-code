using System;
using System.Collections.Generic;
using System.Linq;

namespace DayaxeDal.Repositories
{
    public class LogRepository: BaseRepository, IRepository<Logs>
    {
        public IEnumerable<Logs> Get(Func<Logs, bool> criteria)
        {
            return DayaxeDbContext.Logs.Where(criteria);
        }

        public int Add(Logs entity)
        {
            DayaxeDbContext.Logs.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        public void Update(Logs entity)
        {
            //DayaxeDbContext.Logs.Attach(entity, true);
            //Commit();
        }

        public void Delete(Logs entity)
        {
            DayaxeDbContext.Logs.DeleteOnSubmit(entity);
            Commit();
        }

        public void Delete(Func<Logs, bool> predicate)
        {
            IEnumerable<Logs> listHotels = DayaxeDbContext.Logs.Where(predicate).AsEnumerable();
            DayaxeDbContext.Logs.DeleteAllOnSubmit(listHotels);
            Commit();
        }

        public Logs GetById(long id)
        {
            return DayaxeDbContext.Logs.FirstOrDefault(h => h.Id == id);
        }

        public IEnumerable<Logs> GetAll()
        {
            var entities = CacheLayer.Get<List<Logs>>(CacheKeys.LogsCacheKey);
            if (entities != null)
            {
                return entities.OrderByDescending(l => l.Id).AsEnumerable();
            }
            entities = DayaxeDbContext.Logs.OrderByDescending(l => l.Id).ToList();
            CacheLayer.Add(entities, CacheKeys.LogsCacheKey, 15);
            return entities.AsEnumerable();
        }

        public Logs Refresh(Logs entity)
        {
            throw new NotImplementedException();
        }
    }
}
