using System.Collections.Generic;
using System.Linq;

namespace DayaxeDal.Repositories
{
    public class MarketHotelRepository : BaseRepository
    {
        public int Add(MarketHotels entity)
        {
            DayaxeDbContext.MarketHotels.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        public void Update(MarketHotels products)
        {
            var entity = DayaxeDbContext.MarketHotels.FirstOrDefault(x => x.Id == products.Id);
            if (entity != null)
            {
                entity.MarketId = products.MarketId;
            }
            Commit();
        }

        public void Delete(List<MarketHotels> marketHotels)
        {
            if (marketHotels.Any())
            {
                var hotelIds = marketHotels.Select(x => x.HotelId).ToList();
                var removeList = DayaxeDbContext.MarketHotels
                    .Where(x => x.MarketId == marketHotels.First().MarketId
                        && hotelIds.Contains(x.HotelId));
                DayaxeDbContext.MarketHotels.DeleteAllOnSubmit(removeList);

                Commit();
            }
        }

        public IEnumerable<MarketHotels> GetAll()
        {
            var entities = CacheLayer.Get<List<MarketHotels>>(CacheKeys.MarketHotelsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.MarketHotels.ToList();
            CacheLayer.Add(entities, CacheKeys.MarketHotelsCacheKey);
            return entities.AsEnumerable();
        }

        public MarketHotels GetMarketByHotelId(int hotelId)
        {
            return GetAll().FirstOrDefault(x => x.HotelId == hotelId);
        }
    }
}
