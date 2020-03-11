using System.Collections.Generic;
using System.Linq;

namespace DayaxeDal.Repositories
{
    public class AmentyListRepository : BaseRepository
    {
        public int Add(AmentyLists entity)
        {
            DayaxeDbContext.AmentyLists.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }
        public int Add(List<AmentyLists> entity)
        {
            DayaxeDbContext.AmentyLists.InsertAllOnSubmit(entity);
            Commit();
            return 1;
        }

        public void Update(List<AmentyLists> amentyLists)
        {
            var amentyUpdate = DayaxeDbContext.AmentyLists.Where(x => amentyLists.Select(y => y.Id).Contains(x.Id)).ToList();
            if (amentyUpdate.Any())
            {
                amentyUpdate.ForEach(item =>
                {
                    var newItem = amentyLists.FirstOrDefault(x => x.Id == item.Id);
                    if (newItem != null)
                    {
                        item.IsActive = newItem.IsActive;
                        item.IsAmenty = newItem.IsAmenty;
                        item.Name = newItem.Name;
                        item.AmentyTypeId = newItem.AmentyTypeId;
                        item.AmentyOrder = newItem.AmentyOrder;
                    }
                });
            }
            Commit();
        }

        public void Delete(List<int> amentyInts)
        {
            var amenties = DayaxeDbContext.AmentyLists.Where(x => amentyInts.Contains(x.Id)).ToList();
            amenties.ForEach(item =>
            {
                item.IsActive = false;
            });
            Commit();
        }

        public AmentyLists GetById(long id)
        {
            var products = GetAll().FirstOrDefault(x => x.Id == id);

            return products;
        }

        private IEnumerable<AmentyLists> GetAll()
        {
            var entities = CacheLayer.Get<List<AmentyLists>>(CacheKeys.AmentyListsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.AmentyLists.ToList();
            CacheLayer.Add(entities, CacheKeys.AmentyListsCacheKey);
            return entities.AsEnumerable();
        }
    }
}
