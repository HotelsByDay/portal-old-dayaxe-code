using System;
using System.Collections.Generic;
using System.Linq;

namespace DayaxeDal.Repositories
{
    public class GiftCardRepository: BaseRepository, IRepository<GiftCards>
    {
        public IEnumerable<GiftCards> Get(Func<GiftCards, bool> criteria)
        {
            return DayaxeDbContext.GiftCards.Where(criteria);
        }

        public int Add(GiftCards entity)
        {
            DayaxeDbContext.GiftCards.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        public void Update(GiftCards entity)
        {
            var update = DayaxeDbContext.GiftCards.SingleOrDefault(x => x.Id == entity.Id);
            if (update != null)
            {
                update.Code = entity.Code;
                update.Name = entity.Name;
                update.Amount = entity.Amount;
                update.IsDelete = entity.IsDelete;
                Commit();
            }
        }

        public void Delete(GiftCards entity)
        {
            var giftCard = DayaxeDbContext.GiftCards.FirstOrDefault(g => g.Id == entity.Id);
            if (giftCard != null)
            {
                giftCard.IsDelete = true;
                Commit();
            }
        }

        public void Delete(Func<GiftCards, bool> predicate)
        {
            var entities = DayaxeDbContext.GiftCards.Where(predicate).ToList();
            if (entities.Any())
            {
                entities.ForEach(item =>
                {
                    item.IsDelete = true;
                });
                Commit();
            }
        }

        public GiftCards GetById(long id)
        {
            return GetAll().FirstOrDefault(h => h.Id == id);
        }

        public IEnumerable<GiftCards> GetAll()
        {
            var entities = CacheLayer.Get<List<GiftCards>>(CacheKeys.GiftCardCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.GiftCards.ToList();
            CacheLayer.Add(entities, CacheKeys.GiftCardCacheKey);
            return entities.AsEnumerable();
        }

        public GiftCards Refresh(GiftCards entity)
        {
            throw new NotImplementedException();
        }

        public bool IsCodeExists(string surveyCode)
        {
            var booking = GiftCardList.FirstOrDefault(x => x.Code == surveyCode.ToUpper());
            if (booking != null)
            {
                return true;
            }
            return false;
        }
    }
}
