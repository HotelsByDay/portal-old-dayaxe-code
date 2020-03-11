using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using DayaxeDal.Custom;
using DayaxeDal.Parameters;

namespace DayaxeDal.Repositories
{
    public class SubscriptionRepository : BaseRepository, IRepository<Subscriptions>
    {
        public IEnumerable<Subscriptions> Get(Func<Subscriptions, bool> criteria)
        {
            return GetAll().Where(criteria);
        }

        public int Add(Subscriptions entity)
        {
            DayaxeDbContext.Subscriptions.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        public void Update(Subscriptions entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Subscriptions entity, List<SubscriptionImages> photoses)
        {
            using (var transaction = new TransactionScope())
            {
                var entityUpdate = DayaxeDbContext.Subscriptions.SingleOrDefault(x => x.Id == entity.Id);
                if (entityUpdate != null)
                {
                    if (entity.MaxPurchases != entityUpdate.MaxPurchases)
                    {
                        var discountSubscriptions = (from d in DayaxeDbContext.Discounts
                            join sbd in DayaxeDbContext.SubsciptionDiscountUseds on d.Id equals sbd.DiscountId
                            join sb in DayaxeDbContext.SubscriptionBookings on sbd.SubscriptionBookingId equals sb.Id
                            where d.PromoType == 2 && sb.SubscriptionId == entity.Id
                            select d).ToList();

                        discountSubscriptions.ForEach(discount =>
                        {
                            discount.MaxPurchases = (byte)entity.MaxPurchases;
                        });
                    }

                    entityUpdate.SubscriptionType = entity.SubscriptionType;
                    entityUpdate.Name = entity.Name;
                    entityUpdate.MaxGuest = entity.MaxGuest;
                    entityUpdate.Price = entity.Price;
                    entityUpdate.MaxPurchases = entity.MaxPurchases;
                    entityUpdate.ProductHighlight = entity.ProductHighlight;
                    entityUpdate.WhatYouGet = entity.WhatYouGet;
                    entityUpdate.MetaDescription = entity.MetaDescription;
                    entityUpdate.MetaKeyword = entity.MetaKeyword;

                    entityUpdate.IsActive = entity.IsActive;
                    entityUpdate.IsDelete = entity.IsDelete;
                }

                var subscriptionImages = DayaxeDbContext.SubscriptionImages
                    .Where(x => photoses.Select(y => y.Id).Contains(x.Id)).ToList();
                if (subscriptionImages.Any())
                {
                    subscriptionImages.ForEach(item =>
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
                transaction.Complete();
            }
        }

        public void Delete(Subscriptions entity)
        {
            var updateEntity = DayaxeDbContext.Subscriptions.FirstOrDefault(x => x.Id == entity.Id);
            if (updateEntity != null)
            {
                updateEntity.IsDelete = true;
                Commit();
            }
        }

        public void Delete(Func<Subscriptions, bool> predicate)
        {
            var entities = DayaxeDbContext.Subscriptions.Where(predicate).AsEnumerable();
            entities.ToList().ForEach(hotels =>
            {
                hotels.IsDelete = true;
            });
            Commit();
        }

        public Subscriptions GetById(long id)
        {
            return GetAll().FirstOrDefault(h => h.Id == id);
        }

        public Subscriptions GetById(string id)
        {
            return GetAll().FirstOrDefault(h => h.StripePlanId == id);
        }

        public IEnumerable<Subscriptions> GetAll()
        {
            var entities = CacheLayer.Get<List<Subscriptions>>(CacheKeys.SubscriptionsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.Subscriptions.ToList();
            CacheLayer.Add(entities, CacheKeys.SubscriptionsCacheKey);
            return entities.AsEnumerable();
        }

        public Subscriptions Refresh(Subscriptions entity)
        {
            throw new NotImplementedException();
        }

        #region Subscription Image

        public int AddImage(SubscriptionImages entity)
        {
            using (var transaction = new TransactionScope())
            {
                if (entity.IsCover)
                {
                    var images = DayaxeDbContext.SubscriptionImages
                        .Where(i => i.SubscriptionId == entity.SubscriptionId && i.IsActive)
                        .ToList();

                    images.ForEach(image =>
                    {
                        image.IsCover = false;
                        image.Order = 2;
                    });
                }
                DayaxeDbContext.SubscriptionImages.InsertOnSubmit(entity);
                
                Commit();
                transaction.Complete();
                return entity.Id;
            }
        }

        public SubscriptionImages GetImageById(long id)
        {
            return SubscriptionImagesList.FirstOrDefault(h => h.Id == id);
        }
        
        public void DeleteImage(long id)
        {
            var updateEntity = DayaxeDbContext.SubscriptionImages.FirstOrDefault(x => x.Id == id);
            if (updateEntity != null)
            {
                updateEntity.IsActive = false;
                Commit();
            }
        }

        #endregion

        #region Customer Credits

        public CustomerCredits GetCustomerCreditsByStripeCustomerId(string customerId)
        {
            return (from ci in CustomerInfoList
                join cc in CustomerCreditList on ci.CustomerId equals cc.CustomerId
                where !string.IsNullOrEmpty(ci.StripeCustomerId) && ci.StripeCustomerId.Equals(customerId, StringComparison.OrdinalIgnoreCase)
                select cc).FirstOrDefault();
        }

        #endregion

        #region Subscription Cycle

        public void AddSubscriptionCycle(AddSubscriptionCycleParams param)
        {
            using (var transaction = new TransactionScope())
            {
                var subscriptionBookings = DayaxeDbContext.SubscriptionBookings
                    .FirstOrDefault(sb => sb.Id == param.SubscriptionCyclesObject.SubscriptionBookingId);

                if (subscriptionBookings != null)
                {
                    subscriptionBookings.CancelDate = param.CanceledDate;
                }

                DayaxeDbContext.SubscriptionCycles.InsertOnSubmit(param.SubscriptionCyclesObject);
                Commit();

                if (param.SubscriptionInvoices.Any())
                {
                    param.SubscriptionInvoices.ForEach(subscriptionInvoice =>
                    {
                        subscriptionInvoice.SubscriptionCyclesId = param.SubscriptionCyclesObject.Id;
                    });
                }

                DayaxeDbContext.SubscriptionInvoices.InsertAllOnSubmit(param.SubscriptionInvoices);

                Commit();
                transaction.Complete();
            }
        }

        public void UpdateSubscriptionCycle(AddSubscriptionCycleParams param)
        {
            using (var transaction = new TransactionScope())
            {
                var subscriptionBookings = DayaxeDbContext.SubscriptionCycles
                    .FirstOrDefault(sb => sb.Id == param.SubscriptionCyclesObject.Id);

                if (subscriptionBookings != null)
                {
                    subscriptionBookings.StartDate = param.SubscriptionCyclesObject.StartDate;
                    subscriptionBookings.EndDate = param.SubscriptionCyclesObject.EndDate;
                    subscriptionBookings.LastUpdatedDate = param.SubscriptionCyclesObject.LastUpdatedDate;
                    subscriptionBookings.Price = param.SubscriptionCyclesObject.Price;
                    subscriptionBookings.StripeInvoiceId = param.SubscriptionCyclesObject.StripeInvoiceId;
                    subscriptionBookings.StripeChargeId = param.SubscriptionCyclesObject.StripeChargeId;
                    subscriptionBookings.StripeCouponId = param.SubscriptionCyclesObject.StripeCouponId;
                    subscriptionBookings.CancelDate = param.CanceledDate;
                    subscriptionBookings.CancelDate = param.CanceledDate;
                    
                    if (param.SubscriptionInvoices.Any())
                    {
                        param.SubscriptionInvoices.ForEach(subscriptionInvoice =>
                        {
                            subscriptionInvoice.SubscriptionCyclesId = param.SubscriptionCyclesObject.Id;
                        });

                        DayaxeDbContext.SubscriptionInvoices.InsertAllOnSubmit(param.SubscriptionInvoices);
                    }
                }

                Commit();
                transaction.Complete();
            }
        }

        public void UpdateSubscriptionCycleFailed(StripePaymentFailedInsertOrUpdate param)
        {
            using (var transaction = new TransactionScope())
            {
                var subscriptionBookings = DayaxeDbContext.SubscriptionCycles
                    .FirstOrDefault(sb => sb.Id == param.CurrentSubscriptionBookings.Id);

                if (subscriptionBookings != null)
                {
                    subscriptionBookings.Status = param.CurrentSubscriptionBookings.Status;
                    subscriptionBookings.LastUpdatedDate = param.CurrentSubscriptionBookings.LastUpdatedDate;
                    subscriptionBookings.LastUpdatedBy = param.CurrentSubscriptionBookings.LastUpdatedBy;

                    if (param.CurrentSubscriptionCycles != null)
                    {
                        if (param.CurrentSubscriptionCycles.Id > 0)
                        {
                            var cycles = DayaxeDbContext.SubscriptionCycles
                                .FirstOrDefault(sc => sc.Id == param.CurrentSubscriptionCycles.Id);
                            if (cycles != null)
                            {
                                cycles.Status = param.CurrentSubscriptionCycles.Status;
                                cycles.LastUpdatedDate = param.CurrentSubscriptionCycles.LastUpdatedDate;
                                cycles.LastUpdatedBy = param.CurrentSubscriptionCycles.LastUpdatedBy;
                            }
                        }
                        else
                        {
                            DayaxeDbContext.SubscriptionCycles.InsertOnSubmit(param.CurrentSubscriptionCycles);
                        }
                    }
                }

                Commit();
                transaction.Complete();
            }
        }

        #endregion

        #region Custom
        public Discounts GetDiscountsBySubscriptionBookingId(int subscriptionBookingId)
        {
            return (from d in DiscountList
                join db in SubscriptionBookingDiscountsList on d.Id equals db.DiscountId
                where db.SubscriptionBookingId == subscriptionBookingId &&
                      d.PromoType != (int)Enums.PromoType.SubscriptionPromo
                select d).FirstOrDefault();
        }

        #endregion Custom
    }
}
