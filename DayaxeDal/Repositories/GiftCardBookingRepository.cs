using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace DayaxeDal.Repositories
{
    public class GiftCardBookingRepository : BaseRepository, IRepository<GiftCardBookings>
    {
        public IEnumerable<GiftCardBookings> Get(Func<GiftCardBookings, bool> criteria)
        {
            return GetAll().Where(criteria);
        }

        public int Add(GiftCardBookings entity)
        {
            using (var transaction = new TransactionScope())
            {
                var customerInfos = CustomerInfoList.FirstOrDefault(ci => ci.CustomerId == entity.CustomerId);

                var giftCard = new GiftCards
                {
                    Amount = entity.TotalPrice,
                    Code = string.Format(Constant.GiftCardFormat, 
                        entity.CustomerId, 
                        Helper.RandomString(7)),
                    Name = string.Format(Constant.GiftCardNameFormat, 
                        customerInfos != null ? customerInfos.EmailAddress : entity.CustomerId.ToString(), 
                        entity.RecipientName, 
                        entity.RecipientEmail),
                    IsDelete = false,
                    Status = (byte)Enums.GiftCardType.Available
                };

                DayaxeDbContext.GiftCards.InsertOnSubmit(giftCard);
                Commit();

                entity.GiftCardId = giftCard.Id;
                DayaxeDbContext.GiftCardBookings.InsertOnSubmit(entity);
                Commit();

                if (entity.PayByCredit > 0)
                {
                    var creditLogs = new CustomerCreditLogs
                    {
                        Amount = entity.PayByCredit,
                        CreatedBy = entity.CustomerId,
                        CreatedDate = DateTime.UtcNow,
                        CreditType = (byte)Enums.CreditType.Charge,
                        Description = entity.Description,
                        CustomerId = entity.CustomerId,
                        Status = true,
                        GiftCardId = entity.GiftCardId
                    };

                    DayaxeDbContext.CustomerCreditLogs.InsertOnSubmit(creditLogs);

                    var cusCredits = DayaxeDbContext.CustomerCredits
                        .SingleOrDefault(cc => cc.CustomerId == entity.CustomerId);

                    if (cusCredits != null)
                    {
                        cusCredits.Amount -= entity.PayByCredit;
                    }
                }

                var schedulesAddOn = new Schedules
                {
                    ScheduleSendType = (int)Enums.ScheduleSendType.IsEmailGiftCardConfirmation,
                    Name = "Send eGift Card Confirmation",
                    Status = (int)Enums.ScheduleType.NotRun,
                    GiftCardBookingId = entity.Id
                };
                DayaxeDbContext.Schedules.InsertOnSubmit(schedulesAddOn);

                Commit();
                transaction.Complete();

                return entity.Id;
            }
        }

        public void Update(GiftCardBookings entity)
        {
            var update = DayaxeDbContext.GiftCardBookings.SingleOrDefault(x => x.Id == entity.Id);
            if (update != null)
            {
                update.IsSend = entity.IsSend;
                update.SendDate = entity.SendDate;
                update.LastUpdatedDate = entity.LastUpdatedDate;
                update.LastUpdatedBy = entity.LastUpdatedBy;
                Commit();
            }
        }

        public void Update(List<GiftCardBookings> entities)
        {
            using (var transaction = new TransactionScope())
            {
                entities.ForEach(entity =>
                {
                    var update = DayaxeDbContext.GiftCardBookings.SingleOrDefault(x => x.Id == entity.Id);
                    if (update != null)
                    {
                        update.IsSend = entity.IsSend;
                        update.SendDate = entity.SendDate;
                        update.LastUpdatedDate = entity.LastUpdatedDate;
                        update.LastUpdatedBy = entity.LastUpdatedBy;
                    }
                });
                Commit();
                transaction.Complete();
            }
        }

        public void Delete(GiftCardBookings entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Func<GiftCardBookings, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public GiftCardBookings GetById(long id)
        {
            return GetAll().FirstOrDefault(h => h.Id == id);
        }

        public IEnumerable<GiftCardBookings> GetAll()
        {
            var entities = CacheLayer.Get<List<GiftCardBookings>>(CacheKeys.GiftCardBookingCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.GiftCardBookings.ToList();
            CacheLayer.Add(entities, CacheKeys.GiftCardBookingCacheKey);
            return entities.AsEnumerable();
        }

        public GiftCardBookings Refresh(GiftCardBookings entity)
        {
            throw new NotImplementedException();
        }

        #region Custom

        public IEnumerable<GiftCardBookings> GetGiftCardBookingsDelivery()
        {
            return DayaxeDbContext.GiftCardBookings.Where(g => g.DeliveryDate < DateTime.UtcNow && 
                !g.SendDate.HasValue && 
                !g.IsSend);
        }

        #endregion
    }
}
