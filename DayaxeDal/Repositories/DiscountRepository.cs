using System;
using System.Collections.Generic;
using System.Linq;
using DayaxeDal.Extensions;
using DayaxeDal.Parameters;

namespace DayaxeDal.Repositories
{
    public class DiscountRepository : BaseRepository, IRepository<Discounts>
    {
        public IEnumerable<Discounts> Get(Func<Discounts, bool> criteria)
        {
            return GetAll().Where(criteria);
        }

        public int Add(Discounts entity)
        {
            DayaxeDbContext.Discounts.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        public void Add(List<Discounts> entity)
        {
            DayaxeDbContext.Discounts.InsertAllOnSubmit(entity);
            Commit();
        }

        public void Update(Discounts entity)
        {
            var update = DayaxeDbContext.Discounts.SingleOrDefault(x => x.Id == entity.Id);
            if (update != null)
            {
                update.DiscountName = entity.DiscountName;
                update.StartDate = entity.StartDate;
                update.EndDate = entity.EndDate;
                update.Code = entity.Code;
                update.PercentOff = entity.PercentOff;
                update.CodeRequired = entity.CodeRequired;
                update.PromoType = entity.PromoType;
                update.MinAmount = entity.MinAmount;
                update.IsAllProducts = entity.IsAllProducts;
                update.MaxPurchases = entity.MaxPurchases;
                update.FinePrint = entity.FinePrint;
                update.BillingCycleNumber = entity.BillingCycleNumber;
            }
            Commit();
        }

        public void Delete(Discounts entity)
        {
            var customer = DayaxeDbContext.Discounts.FirstOrDefault(x => x.Id == entity.Id);
            if (customer != null)
            {
                customer.IsDelete = true;
            }
            Commit();
        }

        public void Delete(Func<Discounts, bool> predicate)
        {
            IEnumerable<Discounts> listHotels = DayaxeDbContext.Discounts.Where(predicate).AsEnumerable();
            listHotels.ToList().ForEach(hotels =>
            {
                hotels.IsDelete = true;
            });
            Commit();
        }

        public Discounts GetById(long id)
        {
            return GetAll().FirstOrDefault(h => h.Id == id);
        }

        public IEnumerable<Discounts> GetAll()
        {
            var entities = CacheLayer.Get<List<Discounts>>(CacheKeys.DiscountsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.Discounts.Where(d => !d.IsDelete && d.PromoType != (int)Enums.PromoType.SubscriptionPromo).ToList();

            // Calculate Status and Count Uses
            entities.ForEach(item =>
            {
                int discountUsed;
                if (item.IsAllProducts)
                {
                    discountUsed = DiscountBookingList.Count(x => x.DiscountId == item.Id);
                }
                else
                {
                    var discountHotels = DiscountProductList.Where(dh => dh.DiscountId == item.Id).ToList();
                    var uses = (from p in discountHotels
                        join p1 in DiscountBookingList
                        on new { p.DiscountId, p.ProductId } equals new { p1.DiscountId, p1.ProductId }
                        select p1);
                    discountUsed = uses.Count();
                }
                item.DiscountUses = discountUsed;
                item.Status = Enums.DiscountStatus.Scheduled;

                if (item.StartDate.HasValue && item.EndDate.HasValue)
                {
                    DateTime dateNow = DateTime.UtcNow.ToLosAngerlesTime().Date;
                    if (item.StartDate.Value.Date <= dateNow.Date && item.EndDate.Value.Date >= dateNow.Date)
                    {
                        item.Status = Enums.DiscountStatus.Active;
                    }
                    if (item.StartDate.Value.Date < dateNow.Date && item.EndDate.Value.Date < dateNow.Date)
                    {
                        item.Status = Enums.DiscountStatus.Ended;
                    }
                }
            });

            CacheLayer.Add(entities, CacheKeys.DiscountsCacheKey);
            return entities.AsEnumerable();
        }

        public Discounts Refresh(Discounts entity)
        {
            throw new NotImplementedException();
        }

        #region Custom

        public void Delete(int discountId)
        {
            var customer = DayaxeDbContext.Discounts.FirstOrDefault(x => x.Id == discountId);
            if (customer != null)
            {
                customer.IsDelete = true;
            }
            Commit();
        }

        public Discounts GetDiscountValidByCode(GetDiscountValidByCodeParams param, out bool isLimit)
        {
            isLimit = false;

            // Products
            var discount = (from p1 in DiscountProductList
                            join p in DiscountList on p1.DiscountId equals p.Id
                            where p.Code.ToLower() == param.Code.Trim().ToLower()
                                  && !p.IsDelete
                                  && p1.ProductId == param.ProductId
                                  && p.StartDate.HasValue
                                  && p.EndDate.HasValue
                                  && (param.IsAdmin || (p.StartDate.Value.Date <= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(param.TimezoneId).Date
                                    && p.EndDate.Value.Date >= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(param.TimezoneId).Date))
                            select p).FirstOrDefault();

            // Subscription
            if (param.SubscriptionId > 0)
            {
                discount = (from ds in DiscountSubscriptionList
                            join d in DiscountOfSubscriptionList on ds.DiscountId equals d.Id
                            where d.Code.ToLower() == param.Code.Trim().ToLower()
                                  && !d.IsDelete
                                  && ds.SubscriptionId == param.SubscriptionId
                                  && d.StartDate.HasValue
                                  && d.EndDate.HasValue
                                  && d.StartDate.Value.Date <= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(param.TimezoneId).Date
                                    && d.EndDate.Value.Date >= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(param.TimezoneId).Date
                            select d).FirstOrDefault();

                if (discount == null)
                {
                    discount = (from ds in DiscountSubscriptionList
                        join d in DiscountList on ds.DiscountId equals d.Id
                        where d.Code.ToLower() == param.Code.Trim().ToLower()
                              && !d.IsDelete
                              && ds.SubscriptionId == param.SubscriptionId
                              && d.StartDate.HasValue
                              && d.EndDate.HasValue
                              && d.StartDate.Value.Date <= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(param.TimezoneId).Date
                              && d.EndDate.Value.Date >= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(param.TimezoneId).Date
                        select d).FirstOrDefault();
                }
            }

            // Discount AllProduct with Code Required
            if (discount == null)
            {
                discount = DiscountList.FirstOrDefault(x => x.IsAllProducts
                    && x.Code.ToLower() == param.Code.Trim().ToLower()
                    && x.CodeRequired
                    && !x.IsDelete
                    && x.StartDate.HasValue
                    && x.EndDate.HasValue
                    && (param.IsAdmin || (x.StartDate.Value.Date <= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(param.TimezoneId).Date
                        && x.EndDate.Value.Date >= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(param.TimezoneId).Date)));
            }

            // Discount AllProduct with Code Not Required
            if (discount == null)
            {
                discount = DiscountList.FirstOrDefault(x => x.IsAllProducts
                    && x.Code.ToLower() == param.Code.Trim().ToLower()
                    && !x.IsDelete
                    && x.StartDate.HasValue
                    && x.EndDate.HasValue
                    && (param.IsAdmin || (x.StartDate.Value.Date <= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(param.TimezoneId).Date
                        && x.EndDate.Value.Date >= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(param.TimezoneId).Date)));
            }

            if (discount != null && param.CustomerId > 0)
            {
                var bookings = (from b in BookingList
                                join db in DiscountBookingList on b.BookingId equals db.BookingId
                                join ci in CustomerInfoList on b.CustomerId equals ci.CustomerId
                                where ci.CustomerId == param.CustomerId && db.DiscountId == discount.Id &&
                                    b.PassStatus != (int)Enums.BookingStatus.Refunded &&
                                    b.BookingId != param.BookingId
                                select b).Count();

                if (discount.MaxPurchases != 0 && discount.MaxPurchases > bookings)
                {
                    return discount;
                }
                isLimit = true;
                return null;
            }

            return discount;
        }

        public Discounts VerifyDiscounts(Discounts discounts, int customerId)
        {
            var bookings = (from b in BookingList
                            join db in DiscountBookingList on b.BookingId equals db.BookingId
                            join ci in CustomerInfoList on b.CustomerId equals ci.CustomerId
                            where ci.CustomerId == customerId && db.DiscountId == discounts.Id && b.PassStatus != (int)Enums.BookingStatus.Refunded
                            select b).Count();
            if (discounts.MaxPurchases != 0 && discounts.MaxPurchases <= bookings)
            {
                return null;
            }
            return discounts;
        }

        public Discounts VerifyDiscountsSubscription(Discounts discounts, int customerId, int subscriptionId)
        {
            var bookings = (from sb in SubscriptionBookingsList
                            join sbd in SubscriptionBookingDiscountsList on sb.Id equals sbd.SubscriptionBookingId
                            join ci in CustomerInfoList on sb.CustomerId equals ci.CustomerId
                            where ci.CustomerId == customerId && sbd.DiscountId == discounts.Id && sb.Status != (int)Enums.SubscriptionBookingStatus.End
                            select sb).Count();

            var exists = DiscountSubscriptionList
                .FirstOrDefault(ds => ds.DiscountId == discounts.Id && ds.SubscriptionId == subscriptionId);
            if (exists == null)
            {
                return null;
            }

            if (discounts.MaxPurchases != 0 && discounts.MaxPurchases <= bookings)
            {
                return null;
            }
            return discounts;
        }

        public Discounts GetDiscountUsedWithSubscription(string code, int subscriptionId, string timezoneId)
        {
            var discount = (from ds in DiscountSubscriptionList
                join d in DiscountList on ds.DiscountId equals d.Id
                where d.Code.ToLower() == code.Trim().ToLower()
                      && !d.IsDelete
                      && ds.SubscriptionId == subscriptionId
                      && d.StartDate.HasValue
                      && d.EndDate.HasValue
                      && d.StartDate.Value.Date <= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(timezoneId).Date
                      && d.EndDate.Value.Date >= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(timezoneId).Date
                select d);

            return discount.FirstOrDefault();
        }

        #endregion

        #region Discount Products

        public void AddDiscountProducts(DiscountProducts entity)
        {
            DayaxeDbContext.DiscountProducts.InsertOnSubmit(entity);

            Commit();
        }

        public void AddAllDiscountProducts(List<DiscountProducts> entities)
        {
            DayaxeDbContext.DiscountProducts.InsertAllOnSubmit(entities);

            Commit();
        }

        public void RemoveDiscountProducts(List<DiscountProducts> entities)
        {
            var removeList = DayaxeDbContext.DiscountProducts.Where(x => entities.Contains(x)).ToList();
            DayaxeDbContext.DiscountProducts.DeleteAllOnSubmit(removeList);

            Commit();
        }

        public List<Products> SearchProductsByDiscountId(int id)
        {
            var queryProduct = from p in ProductList
                join p1 in DiscountProductList on p.ProductId equals p1.ProductId
                join p2 in DiscountList on p1.DiscountId equals p2.Id
                where p2.Id == id && !p.IsDelete
                select p;

            return queryProduct.ToList();
        }

        public List<DiscountProducts> GetDiscountsProductsById(int id)
        {
            var discountHotels = (from p in DiscountProductList
                join p1 in ProductList
                on p.ProductId equals p1.ProductId
                where !p1.IsDelete && p.DiscountId == id
                select p).ToList();
            return discountHotels;
        }

        #endregion

        #region Discount Subscriptions

        public void AddDiscountSubscriptions(DiscountSubscriptions entity)
        {
            DayaxeDbContext.DiscountSubscriptions.InsertOnSubmit(entity);

            Commit();
        }

        public void AddAllDiscountSubscriptions(List<DiscountSubscriptions> entities)
        {
            DayaxeDbContext.DiscountSubscriptions.InsertAllOnSubmit(entities);

            Commit();
        }

        public void RemoveDiscountSubscriptions(List<DiscountSubscriptions> entities)
        {
            var removeList = DayaxeDbContext.DiscountSubscriptions.Where(x => entities.Contains(x)).ToList();
            DayaxeDbContext.DiscountSubscriptions.DeleteAllOnSubmit(removeList);

            Commit();
        }

        public List<Subscriptions> SearchSubscriptionsByDiscountId(int id)
        {
            var queryProduct = from p in SubscriptionsList
                join p1 in DiscountSubscriptionList on p.Id equals p1.SubscriptionId
                join p2 in DiscountList on p1.DiscountId equals p2.Id
                where p2.Id == id && !p.IsDelete
                select p;

            return queryProduct.ToList();
        }

        public List<DiscountSubscriptions> GetDiscountSubscriptionsById(int id)
        {
            var discountHotels = (from p in DiscountSubscriptionList
                join p1 in SubscriptionsList
                on p.SubscriptionId equals p1.Id
                where !p1.IsDelete && p.DiscountId == id
                select p).ToList();
            return discountHotels;
        }

        #endregion
    }
}
