using System;
using System.Collections.Generic;
using System.Linq;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;

namespace DayaxeDal
{
    public partial class BaseRepository
    {
        #region Private Function

        private IEnumerable<Taxes> GetAllTaxes()
        {
            var entities = CacheLayer.Get<List<Taxes>>(CacheKeys.TaxesCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.Taxes.ToList();
            CacheLayer.Add(entities, CacheKeys.TaxesCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<RevShares> GetAllRevShares()
        {
            var entities = CacheLayer.Get<List<RevShares>>(CacheKeys.RevSharesCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.RevShares.ToList();
            CacheLayer.Add(entities, CacheKeys.RevSharesCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<Photos> GetAllPhotos()
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

        private IEnumerable<AmentyLists> GetAllAmentyList()
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

        private IEnumerable<Hotels> GetAllHotels()
        {
            var entities = CacheLayer.Get<List<Hotels>>(CacheKeys.HotelsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.Hotels.ToList();
            SetTotalTickets(ref entities);
            CacheLayer.Add(entities, CacheKeys.HotelsCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<Amenties> GetAllAmenties()
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

        private IEnumerable<Products> GetAllProducts()
        {
            var entities = CacheLayer.Get<List<Products>>(CacheKeys.ProductsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.Products.ToList();
            SetImagePathForProducts(ref entities);
            CacheLayer.Add(entities, CacheKeys.ProductsCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<CustomerInfos> GetAllCustomerInfos()
        {
            var entities = CacheLayer.Get<List<CustomerInfos>>(CacheKeys.CustomerInfosCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.CustomerInfos.ToList();
            CacheLayer.Add(entities, CacheKeys.CustomerInfosCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<CustomerInfoSearchCriteria> GetAllCustomerInfoSearchCriterias()
        {
            var entities = CacheLayer.Get<List<CustomerInfoSearchCriteria>>(CacheKeys.CustomerInfosSearchCriteriaCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.CustomerInfoSearchCriteria.ToList();
            CacheLayer.Add(entities, CacheKeys.CustomerInfosSearchCriteriaCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<MarketHotels> GetAllMarketHotels()
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

        //private IEnumerable<BlockedDates> GetAllBlockedDates()
        //{
        //    var entities = CacheLayer.Get<List<BlockedDates>>(CacheKeys.BlockedDatesCacheKey);
        //    if (entities != null)
        //    {
        //        return entities.AsEnumerable();
        //    }
        //    entities = DayaxeDbContext.BlockedDates.ToList();
        //    CacheLayer.Add(entities, CacheKeys.BlockedDatesCacheKey);
        //    return entities.AsEnumerable();
        //}

        private IEnumerable<ProductUpgrades> GetAllProductUpgrades()
        {
            var entities = CacheLayer.Get<List<ProductUpgrades>>(CacheKeys.ProductUpgradesCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.ProductUpgrades.ToList();
            CacheLayer.Add(entities, CacheKeys.ProductUpgradesCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<CustomerInfosHotels> GetAllUserHotels()
        {
            var entities = CacheLayer.Get<List<CustomerInfosHotels>>(CacheKeys.CustomerInfosHotelsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.CustomerInfosHotels.ToList();
            CacheLayer.Add(entities, CacheKeys.CustomerInfosHotelsCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<Discounts> GetAllDiscounts()
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

        private IEnumerable<DiscountProducts> GetAllDiscountProducts()
        {
            var entities = CacheLayer.Get<List<DiscountProducts>>(CacheKeys.DiscountProductsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.DiscountProducts.ToList();
            CacheLayer.Add(entities, CacheKeys.DiscountProductsCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<Bookings> GetAllBookings()
        {
            var entities = CacheLayer.Get<List<Bookings>>(CacheKeys.BookingsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.Bookings.ToList();
            CacheLayer.Add(entities, CacheKeys.BookingsCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<Surveys> GetAllSurveys()
        {
            var entities = CacheLayer.Get<List<Surveys>>(CacheKeys.SurveysCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.Surveys.ToList();
            CacheLayer.Add(entities, CacheKeys.SurveysCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<ProductImages> GetAllProductImages()
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

        private IEnumerable<BlockedDatesCustomPrice> GetAllBlockedDatesCustomPrice()
        {
            var entities = CacheLayer.Get<List<BlockedDatesCustomPrice>>(CacheKeys.BlockedDatesCustomPricesCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.BlockedDatesCustomPrice.ToList();
            CacheLayer.Add(entities, CacheKeys.BlockedDatesCustomPricesCacheKey);
            return entities.AsEnumerable();
        }

        //private IEnumerable<Invoices> GetAllInvoice()
        //{
        //    var entities = CacheLayer.Get<List<Invoices>>(CacheKeys.InvoicesCacheKey);
        //    if (entities != null)
        //    {
        //        return entities.AsEnumerable();
        //    }
        //    entities = DayaxeDbContext.Invoices.ToList();
        //    CacheLayer.Add(entities, CacheKeys.InvoicesCacheKey);
        //    return entities.AsEnumerable();
        //}

        private IEnumerable<DiscountBookings> GetAllDiscountBooking()
        {
            var entities = CacheLayer.Get<List<DiscountBookings>>(CacheKeys.DiscountBookingsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.DiscountBookings.ToList();
            CacheLayer.Add(entities, CacheKeys.DiscountBookingsCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<Markets> GetAllMarket()
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

        private IEnumerable<DefaultPrices> GetAllDefaultPriceList()
        {
            var entities = CacheLayer.Get<List<DefaultPrices>>(CacheKeys.DefaultPricesCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.DefaultPrices.ToList();
            CacheLayer.Add(entities, CacheKeys.DefaultPricesCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<ProductAddOns> GetAllProductAddOns()
        {
            var entities = CacheLayer.Get<List<ProductAddOns>>(CacheKeys.ProductAddOnsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.ProductAddOns.ToList();
            CacheLayer.Add(entities, CacheKeys.ProductAddOnsCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<CustomerCredits> GetAllCustomerCredits()
        {
            var entities = CacheLayer.Get<List<CustomerCredits>>(CacheKeys.CustomerCreditsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.CustomerCredits.ToList();
            CacheLayer.Add(entities, CacheKeys.CustomerCreditsCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<CustomerCreditLogs> GetAllCustomerCreditLogs()
        {
            var entities = CacheLayer.Get<List<CustomerCreditLogs>>(CacheKeys.CustomerCreditLogsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.CustomerCreditLogs.ToList();
            CacheLayer.Add(entities, CacheKeys.CustomerCreditLogsCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<GiftCards> GetAllGiftCards()
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

        private IEnumerable<GiftCardBookings> GetAllGiftCardBookings()
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

        private IEnumerable<BookingHistories> GetAllBookingHistories()
        {
            var entities = CacheLayer.Get<List<BookingHistories>>(CacheKeys.BookingHistoriesCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.BookingHistories.ToList();
            CacheLayer.Add(entities, CacheKeys.BookingHistoriesCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<Subscriptions> GetAllSubscriptions()
        {
            var entities = CacheLayer.Get<List<Subscriptions>>(CacheKeys.SubscriptionsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.Subscriptions.ToList();
            SetImagePathForSubscription(ref entities);
            CacheLayer.Add(entities, CacheKeys.SubscriptionsCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<SubscriptionImages> GetAllSubscriptionImages()
        {
            var entities = CacheLayer.Get<List<SubscriptionImages>>(CacheKeys.SubscriptionImagesCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.SubscriptionImages.ToList();
            CacheLayer.Add(entities, CacheKeys.SubscriptionImagesCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<SubscriptionBookings> GetAllSubscriptionBookings()
        {
            var entities = CacheLayer.Get<List<SubscriptionBookings>>(CacheKeys.SubscriptionBookingsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.SubscriptionBookings.ToList();
            CacheLayer.Add(entities, CacheKeys.SubscriptionBookingsCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<SubscriptionCycles> GetAllSubscriptionCycles()
        {
            var entities = CacheLayer.Get<List<SubscriptionCycles>>(CacheKeys.SubscriptionCyclesCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.SubscriptionCycles.ToList();
            CacheLayer.Add(entities, CacheKeys.SubscriptionCyclesCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<DiscountSubscriptions> GetAllDiscountSubscriptions()
        {
            var entities = CacheLayer.Get<List<DiscountSubscriptions>>(CacheKeys.DiscountSubscriptionsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.DiscountSubscriptions.ToList();
            CacheLayer.Add(entities, CacheKeys.DiscountSubscriptionsCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<SubscriptionBookingDiscounts> GetAllSubscriptionBookingDiscounts()
        {
            var entities = CacheLayer.Get<List<SubscriptionBookingDiscounts>>(CacheKeys.SubscriptionBookingDiscountsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.SubscriptionBookingDiscounts.ToList();
            CacheLayer.Add(entities, CacheKeys.SubscriptionBookingDiscountsCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<SubsciptionDiscountUseds> GetAllSubscriptionDiscountUsed()
        {
            var entities = CacheLayer.Get<List<SubsciptionDiscountUseds>>(CacheKeys.SubsciptionDiscountUsedCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.SubsciptionDiscountUseds.ToList();
            CacheLayer.Add(entities, CacheKeys.SubsciptionDiscountUsedCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<Discounts> GetAllSubscriptionDiscounts()
        {
            var entities = CacheLayer.Get<List<Discounts>>(CacheKeys.SubscriptionDiscountsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }

            entities = DayaxeDbContext.Discounts.Where(d => !d.IsDelete && d.PromoType == (int)Enums.PromoType.SubscriptionPromo).ToList();

            // Calculate Status and Count Uses
            entities.ForEach(item =>
            {
                item.DiscountUses = SubscriptionDiscountUsedList.Count(x => x.DiscountId == item.Id);
                item.Status = Enums.DiscountStatus.Scheduled;

                if (item.StartDate.HasValue && item.EndDate.HasValue)
                {
                    DateTime dateNow = DateTime.UtcNow;
                    if (item.StartDate.Value <= dateNow.AddMinutes(5) && item.EndDate.Value >= dateNow.AddMinutes(-5))
                    {
                        item.Status = Enums.DiscountStatus.Active;
                    }
                    if (item.StartDate.Value < dateNow.AddMinutes(5) && item.EndDate.Value < dateNow.AddMinutes(-5))
                    {
                        item.Status = Enums.DiscountStatus.Ended;
                    }
                }
            });

            CacheLayer.Add(entities, CacheKeys.SubscriptionDiscountsCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<Policies> GetAllPolicies()
        {
            var entities = CacheLayer.Get<List<Policies>>(CacheKeys.PoliciesCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }

            entities = DayaxeDbContext.Policies.Where(d => !d.IsDelete).ToList();
            CacheLayer.Add(entities, CacheKeys.PoliciesCacheKey);
            return entities.AsEnumerable();
        }

        private IEnumerable<HotelPolicies> GetAllHotelPolicies()
        {
            var entities = CacheLayer.Get<List<HotelPolicies>>(CacheKeys.HotelPoliciesCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }

            entities = DayaxeDbContext.HotelPolicies.ToList();
            CacheLayer.Add(entities, CacheKeys.HotelPoliciesCacheKey);
            return entities.AsEnumerable();
        }

        #endregion

        #region Private Calculate Data on Reset

        protected void SetTotalTickets(ref List<Hotels> hotels)
        {
            hotels.ForEach(hotel =>
            {
                var totalReviews = (from s in SurveyList
                                    join b in BookingList on s.BookingId equals b.BookingId
                                    join p in ProductList on b.ProductId equals p.ProductId
                                    where p.HotelId == hotel.HotelId && s.IsFinish
                                    select s).Count();

                hotel.TotalReviews = string.Format(Constant.TotalReviews, totalReviews);
                hotel.MoreAtHotelString = string.Format(Constant.MoreAtHotel, hotel.HotelName);

                hotel.ImageUrl = GetHotelImageUrl(hotel.HotelId);

                hotel.ImageSurveyUrl = GetImageSurveyUrl(hotel.HotelId);
            });
        }

        #endregion

        #region Private Calculate Custom Price

        private ActualPriceObject GetProductDailyPriceByDateDb(Products products, DateTime date)
        {
            if (DayaxeDbContext.BlockedDatesCustomPrice.Any(bdcpchild => bdcpchild.ProductId == products.ProductId && bdcpchild.Date == date.Date))
            {
                var result = GetCustomPriceByDateDb(products.ProductId, date);

                if (result.Price.Equals(0))
                {
                    var defaultPrice = GetRegularPriceByDateDb(products, date);
                    result.Price = defaultPrice.Price;
                }

                return result;
            }

            return GetRegularPriceByDateDb(products, date);
        }

        private ActualPriceObject GetCustomPriceByDateDb(long productId, DateTime date)
        {
            var customPrice =
                DayaxeDbContext.BlockedDatesCustomPrice.First(bdcpchild => bdcpchild.ProductId == productId &&
                                                               bdcpchild.Date.Date == date.Date.Date);
            return new ActualPriceObject
            {
                Price = customPrice.RegularPrice,
                DiscountPrice = customPrice.DiscountPrice,
                Capacity = customPrice.Capacity ?? 0
            };
        }

        private ActualPriceObject GetRegularPriceByDateDb(Products product, DateTime date)
        {
            var defaultPrice = DayaxeDbContext.DefaultPrices.Where(d => d.EffectiveDate.Date < date.Date && d.ProductId == product.ProductId)
                .OrderByDescending(d => d.EffectiveDate).FirstOrDefault();
            if (defaultPrice != null)
            {
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        return new ActualPriceObject
                        {
                            Price = defaultPrice.PriceMon,
                            DiscountPrice = defaultPrice.UpgradeDiscountMon,
                            Capacity = defaultPrice.PassCapacityMon
                        };
                    case DayOfWeek.Tuesday:
                        return new ActualPriceObject
                        {
                            Price = defaultPrice.PriceTue,
                            DiscountPrice = defaultPrice.UpgradeDiscountTue,
                            Capacity = defaultPrice.PassCapacityTue
                        };
                    case DayOfWeek.Wednesday:
                        return new ActualPriceObject
                        {
                            Price = defaultPrice.PriceWed,
                            DiscountPrice = defaultPrice.UpgradeDiscountWed,
                            Capacity = defaultPrice.PassCapacityWed
                        };
                    case DayOfWeek.Thursday:
                        return new ActualPriceObject
                        {
                            Price = defaultPrice.PriceThu,
                            DiscountPrice = defaultPrice.UpgradeDiscountThu,
                            Capacity = defaultPrice.PassCapacityThu
                        };
                    case DayOfWeek.Friday:
                        return new ActualPriceObject
                        {
                            Price = defaultPrice.PriceFri,
                            DiscountPrice = defaultPrice.UpgradeDiscountFri,
                            Capacity = defaultPrice.PassCapacityFri
                        };
                    case DayOfWeek.Saturday:
                        return new ActualPriceObject
                        {
                            Price = defaultPrice.PriceSat,
                            DiscountPrice = defaultPrice.UpgradeDiscountSat,
                            Capacity = defaultPrice.PassCapacitySat
                        };
                    default:
                        return new ActualPriceObject
                        {
                            Price = defaultPrice.PriceSun,
                            DiscountPrice = defaultPrice.UpgradeDiscountSun,
                            Capacity = defaultPrice.PassCapacitySun
                        };
                }
            }
            else
            {
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        return new ActualPriceObject
                        {
                            Price = product.PriceMon,
                            DiscountPrice = product.UpgradeDiscountMon,
                            Capacity = product.PassCapacityMon
                        };
                    case DayOfWeek.Tuesday:
                        return new ActualPriceObject
                        {
                            Price = product.PriceTue,
                            DiscountPrice = product.UpgradeDiscountTue,
                            Capacity = product.PassCapacityTue
                        };
                    case DayOfWeek.Wednesday:
                        return new ActualPriceObject
                        {
                            Price = product.PriceWed,
                            DiscountPrice = product.UpgradeDiscountWed,
                            Capacity = product.PassCapacityWed
                        };
                    case DayOfWeek.Thursday:
                        return new ActualPriceObject
                        {
                            Price = product.PriceThu,
                            DiscountPrice = product.UpgradeDiscountThu,
                            Capacity = product.PassCapacityThu
                        };
                    case DayOfWeek.Friday:
                        return new ActualPriceObject
                        {
                            Price = product.PriceFri,
                            DiscountPrice = product.UpgradeDiscountFri,
                            Capacity = product.PassCapacityFri
                        };
                    case DayOfWeek.Saturday:
                        return new ActualPriceObject
                        {
                            Price = product.PriceSat,
                            DiscountPrice = product.UpgradeDiscountSat,
                            Capacity = product.PassCapacitySat
                        };
                    default:
                        return new ActualPriceObject
                        {
                            Price = product.PriceSun,
                            DiscountPrice = product.UpgradeDiscountSun,
                            Capacity = product.PassCapacitySun
                        };
                }
            }
        }

        #endregion

        #region Set ImageDefault for Subscription

        private void SetImagePathForSubscription(ref List<Subscriptions> entities)
        {
            entities.ForEach(item =>
            {
                string imageUrl = Constant.ImageDefault;
                var image = SubscriptionImagesList.FirstOrDefault(x => x.SubscriptionId == item.Id && x.IsCover && x.IsActive);
                if (image != null)
                {
                    imageUrl = image.Url;
                }
                try
                {
                    item.AdminImageUrl = imageUrl;
                    item.ImageUrl = string.Format("{0}", new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), imageUrl).AbsoluteUri);
                }
                catch (Exception)
                {
                    item.ImageUrl = imageUrl;
                }
            });
        }

        private void SetImagePathForProducts(ref List<Products> entities)
        {
            entities.ForEach(item =>
            {
                string imageUrl = Constant.ImageDefault;
                var image = ProductImageList.FirstOrDefault(x => x.ProductId == item.ProductId && x.IsCover && x.IsActive);
                if (image != null)
                {
                    imageUrl = image.Url;
                }
                try
                {
                    item.AdminImageUrl = imageUrl;
                    item.ImageUrl = string.Format("{0}", new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), imageUrl).AbsoluteUri);
                }
                catch (Exception)
                {
                    item.ImageUrl = imageUrl;
                }
            });
        }

        #endregion

        #region Set Hotel Data

        private string GetHotelImageUrl(int hotelId)
        {
            string imageUrl = Constant.ImageDefault;
            var image = PhotoList.FirstOrDefault(x => x.HotelId == hotelId && x.ImageTypeId > 3 && x.IsActive.HasValue && x.IsActive.Value);
            if (image != null)
            {
                imageUrl = image.Url;
            }
            try
            {
                return string.Format("{0}",
                    new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), imageUrl).AbsoluteUri);
            }
            catch (Exception)
            {
                return imageUrl;
            }
        }

        private string GetImageSurveyUrl(int hotelId)
        {
            string imageUrl = Constant.ImageDefault;
            var image = PhotoList.FirstOrDefault(x => x.HotelId == hotelId && x.ImageTypeId > 3 && x.IsActive.HasValue && x.IsActive.Value);
            if (image != null)
            {
                imageUrl = image.Url;
            }
            return imageUrl;
        }

        protected void SetHotelData(ref Hotels hotels, int hotelId)
        {
            var amentyItem = AmentiesList.First(x => x.HotelId == hotelId);
            hotels.AmentiesItem = amentyItem;
            hotels.PoolAmentyListses = AmentyListsList.Where(x => x.AmentyId == amentyItem.Id
                                                                  && x.IsActive.HasValue
                                                                  && x.IsActive.Value
                                                                  && x.IsAmenty.HasValue
                                                                  && x.IsAmenty.Value
                                                                  && x.AmentyTypeId.HasValue
                                                                  && x.AmentyTypeId.Value == (int)Enums.AmentyType.Pool).OrderBy(x => x.AmentyOrder);

            hotels.PoolAmentyUpgrages = AmentyListsList.Where(x => x.AmentyId == amentyItem.Id
                                                                   && x.IsActive.HasValue
                                                                   && x.IsActive.Value
                                                                   && x.IsAmenty.HasValue
                                                                   && !x.IsAmenty.Value
                                                                   && x.AmentyTypeId.HasValue
                                                                   && x.AmentyTypeId.Value == (int)Enums.AmentyType.Pool).OrderBy(x => x.AmentyOrder);

            hotels.GymAmentyListses = AmentyListsList.Where(x => x.AmentyId == amentyItem.Id
                                                                 && x.IsActive.HasValue
                                                                 && x.IsActive.Value
                                                                 && x.IsAmenty.HasValue
                                                                 && x.IsAmenty.Value
                                                                 && x.AmentyTypeId.HasValue
                                                                 && x.AmentyTypeId.Value == (int)Enums.AmentyType.Gym).OrderBy(x => x.AmentyOrder);

            hotels.GymAmentyUpgrages = AmentyListsList.Where(x => x.AmentyId == amentyItem.Id
                                                                  && x.IsActive.HasValue
                                                                  && x.IsActive.Value
                                                                  && x.IsAmenty.HasValue
                                                                  && !x.IsAmenty.Value
                                                                  && x.AmentyTypeId.HasValue
                                                                  && x.AmentyTypeId.Value == (int)Enums.AmentyType.Gym).OrderBy(x => x.AmentyOrder);

            hotels.SpaAmentyListses = AmentyListsList.Where(x => x.AmentyId == amentyItem.Id
                                                                 && x.IsActive.HasValue
                                                                 && x.IsActive.Value
                                                                 && x.IsAmenty.HasValue
                                                                 && x.IsAmenty.Value
                                                                 && x.AmentyTypeId.HasValue
                                                                 && x.AmentyTypeId.Value == (int)Enums.AmentyType.Spa).OrderBy(x => x.AmentyOrder);

            hotels.SpaAmentyUpgrages = AmentyListsList.Where(x => x.AmentyId == amentyItem.Id
                                                                  && x.IsActive.HasValue
                                                                  && x.IsActive.Value
                                                                  && x.IsAmenty.HasValue
                                                                  && !x.IsAmenty.Value
                                                                  && x.AmentyTypeId.HasValue
                                                                  && x.AmentyTypeId.Value == (int)Enums.AmentyType.Spa).OrderBy(x => x.AmentyOrder);

            hotels.BusinessCenterAmentyListses = AmentyListsList.Where(x => x.AmentyId == amentyItem.Id
                                                                            && x.IsActive.HasValue
                                                                            && x.IsActive.Value
                                                                            && x.IsAmenty.HasValue
                                                                            && x.IsAmenty.Value
                                                                            && x.AmentyTypeId.HasValue
                                                                            && x.AmentyTypeId.Value == (int)Enums.AmentyType.BusinessCenter).OrderBy(x => x.AmentyOrder);

            hotels.BusinessCenterAmentyUpgrages = AmentyListsList.Where(x => x.AmentyId == amentyItem.Id
                                                                             && x.IsActive.HasValue
                                                                             && x.IsActive.Value
                                                                             && x.IsAmenty.HasValue
                                                                             && !x.IsAmenty.Value
                                                                             && x.AmentyTypeId.HasValue
                                                                             && x.AmentyTypeId.Value == (int)Enums.AmentyType.BusinessCenter).OrderBy(x => x.AmentyOrder);

            hotels.DinningAmentyListes = AmentyListsList.Where(x => x.AmentyId == amentyItem.Id
                                                                    && x.IsActive.HasValue
                                                                    && x.IsActive.Value
                                                                    && x.IsAmenty.HasValue
                                                                    && x.IsAmenty.Value
                                                                    && x.AmentyTypeId.HasValue
                                                                    && x.AmentyTypeId.Value == (int)Enums.AmentyType.Dining).OrderBy(x => x.AmentyOrder);

            hotels.EventAmentyListes = AmentyListsList.Where(x => x.AmentyId == amentyItem.Id
                                                                  && x.IsActive.HasValue
                                                                  && x.IsActive.Value
                                                                  && x.IsAmenty.HasValue
                                                                  && x.IsAmenty.Value
                                                                  && x.AmentyTypeId.HasValue
                                                                  && x.AmentyTypeId.Value == (int)Enums.AmentyType.Event).OrderBy(x => x.AmentyOrder);

            hotels.OtherAmentyListses = AmentyListsList.Where(x => x.AmentyId == amentyItem.Id
                                                                   && x.IsActive.HasValue
                                                                   && x.IsActive.Value
                                                                   && x.IsAmenty.HasValue
                                                                   && x.IsAmenty.Value
                                                                   && x.AmentyTypeId.HasValue
                                                                   && x.AmentyTypeId.Value == (int)Enums.AmentyType.Other).OrderBy(x => x.AmentyOrder);

            hotels.OtherAmentyUpgrages = AmentyListsList.Where(x => x.AmentyId == amentyItem.Id
                                                                    && x.IsActive.HasValue
                                                                    && x.IsActive.Value
                                                                    && x.IsAmenty.HasValue
                                                                    && !x.IsAmenty.Value
                                                                    && x.AmentyTypeId.HasValue
                                                                    && x.AmentyTypeId.Value == (int)Enums.AmentyType.Other).OrderBy(x => x.AmentyOrder);

            hotels.PhotoList = PhotoList.Where(x => x.HotelId == hotelId && x.IsActive.HasValue && x.IsActive.Value).OrderBy(x => x.Order);
        }

        #endregion
    }
}
