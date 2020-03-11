using System.Collections.Generic;
using System.Linq;

namespace DayaxeDal.Repositories
{
    public class PhotoRepository : BaseRepository
    {
        public int Add(Photos entity)
        {
            DayaxeDbContext.Photos.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        public void Update(List<Photos> entities)
        {
            var updates = DayaxeDbContext.Photos.Where(x => entities.Select(y => y.Id).Contains(x.Id)).ToList();
            if (updates.Any())
            {
                updates.ForEach(item =>
                {
                    var newItem = entities.FirstOrDefault(x => x.Id == item.Id);
                    if (newItem != null)
                    {
                        item.Order = newItem.Order;
                    }
                });
            }
            Commit();
        }

        public void Delete(int photoId)
        {
            var photo = DayaxeDbContext.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo != null)
            {
                photo.IsActive = false;
            }
            Commit();
        }

        public Photos GetById(long id)
        {
            return GetAll().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Photos> GetAll()
        {
            var entities = CacheLayer.Get<List<Photos>>(CacheKeys.PhotosCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.Photos.ToList();
            CacheLayer.Add(entities, CacheKeys.PhotosCacheKey);
            return entities.AsEnumerable();
        }
    }
}
