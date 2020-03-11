using System.Collections.Generic;
using System.Linq;

namespace DayaxeDal.Repositories
{
    public class ProductImageRepository: BaseRepository
    {
        public void Add(ProductImages image)
        {
            DayaxeDbContext.ProductImages.InsertOnSubmit(image);
            Commit();
        }

        public ProductImages GetById(int id)
        {
            return GetAll().FirstOrDefault(x => x.Id == id);
        }

        public void Update(List<ProductImages> photoses)
        {
            var productImages = DayaxeDbContext.ProductImages.Where(x => photoses.Select(y => y.Id).Contains(x.Id)).ToList();
            if (productImages.Any())
            {
                productImages.ForEach(item =>
                {
                    var newItem = photoses.FirstOrDefault(x => x.Id == item.Id);
                    if (newItem != null)
                    {
                        item.Order = newItem.Order;
                        item.IsCover = newItem.IsCover;
                    }
                });
            }
            Commit();
        }

        public void Delete(int photoId)
        {
            var photo = DayaxeDbContext.ProductImages.FirstOrDefault(x => x.Id == photoId);
            if (photo != null)
            {
                photo.IsActive = false;
            }
            Commit();
        }

        public IEnumerable<ProductImages> GetAll()
        {
            var entities = CacheLayer.Get<List<ProductImages>>(CacheKeys.ProductImagesCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.ProductImages.ToList();
            CacheLayer.Add(entities, CacheKeys.ProductImagesCacheKey);
            return entities.AsEnumerable();
        }
    }
}
