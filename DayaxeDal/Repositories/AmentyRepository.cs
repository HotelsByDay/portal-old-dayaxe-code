using System.Collections.Generic;
using System.Linq;

namespace DayaxeDal.Repositories
{
    public class AmentyRepository : BaseRepository
    {
        public int Add(Amenties entity)
        {
            DayaxeDbContext.Amenties.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        public void Update(Amenties amenties)
        {
            var update = DayaxeDbContext.Amenties.FirstOrDefault(x => x.Id == amenties.Id);
            if (update != null)
            {
                update.PoolActive = amenties.PoolActive;
                update.PoolHours = amenties.PoolHours;
                update.GymActive = amenties.GymActive;
                update.GymHours = amenties.GymHours;
                update.SpaActive = amenties.SpaActive;
                update.SpaHours = amenties.SpaHours;
                update.BusinessActive = amenties.BusinessActive;
                update.BusinessCenterHours = amenties.BusinessCenterHours;
                update.DinningActive = amenties.DinningActive;
                update.DinningHours = amenties.DinningHours;
                update.EventActive = amenties.EventActive;
                update.EventHours = amenties.EventHours;
            }
            Commit();
        }

        public IEnumerable<Amenties> GetAll()
        {
            var entities = CacheLayer.Get<List<Amenties>>(CacheKeys.AmentiesCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.Amenties.ToList();
            CacheLayer.Add(entities, CacheKeys.AmentiesCacheKey);
            return entities.AsEnumerable();
        }
    }
}
