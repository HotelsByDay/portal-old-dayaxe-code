using System;
using System.Collections.Generic;
using System.Linq;

namespace DayaxeDal.Repositories
{
    public class MarketRepository : BaseRepository, IRepository<Markets>
    {
        public IEnumerable<Markets> Get(Func<Markets, bool> criteria)
        {
            return GetAll().Where(criteria);
        }

        public int Add(Markets entity)
        {
            DayaxeDbContext.Markets.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        public void Update(Markets entity)
        {
            var update = DayaxeDbContext.Markets.SingleOrDefault(x => x.Id == entity.Id);
            if (update != null)
            {
                update.IsActive = entity.IsActive;
                update.LocationName = entity.LocationName;
                update.MarketCode = entity.MarketCode;
                update.Permalink = entity.Permalink;
                update.State = entity.State;
                update.Latitude = entity.Latitude;
                update.Longitude = entity.Longitude;

                update.PublicId = entity.PublicId;
                update.Format = entity.Format;
                update.Version = entity.Version;
                update.ImageUrl = entity.ImageUrl;

                update.IsCalculateTax = entity.IsCalculateTax;
            }
            Commit();
        }

        public void Delete(Markets entity)
        {
            var customer = DayaxeDbContext.Markets.FirstOrDefault(x => x.Id == entity.Id);
            if (customer != null)
            {
                customer.IsActive = true;
            }
            Commit();
        }

        public void Delete(Func<Markets, bool> predicate)
        {
            IEnumerable<Markets> listHotels = DayaxeDbContext.Markets.Where(predicate).AsEnumerable();
            listHotels.ToList().ForEach(hotels =>
            {
                hotels.IsActive = true;
            });
            Commit();
        }

        public Markets GetById(long id)
        {
            return GetAll().FirstOrDefault(h => h.Id == id);
        }

        public IEnumerable<Markets> GetAll()
        {
            var entities = CacheLayer.Get<List<Markets>>(CacheKeys.MarketsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.Markets.ToList();
            CacheLayer.Add(entities, CacheKeys.MarketsCacheKey);
            return entities.AsEnumerable();
        }

        public Markets Refresh(Markets entity)
        {
            throw new NotImplementedException();
        }

        #region Custom

        public void Delete(int marketId)
        {
            var markets = DayaxeDbContext.Markets.FirstOrDefault(x => x.Id == marketId);
            if (markets != null)
            {
                var marketHotels = DayaxeDbContext.MarketHotels.Where(x => x.MarketId == marketId);
                DayaxeDbContext.MarketHotels.DeleteAllOnSubmit(marketHotels);

                DayaxeDbContext.Markets.DeleteOnSubmit(markets);

                Commit();
            }
        }

        #endregion 
    }
}
