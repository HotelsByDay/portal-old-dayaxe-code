using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;
using DayaxeDal.Parameters;

namespace DayaxeDal.Repositories
{
    public class ProductRepository : BaseRepository, IRepository<Products>
    {
        public IEnumerable<Products> Get(Func<Products, bool> criteria)
        {
            return GetAll().Where(criteria);
        }

        public int Add(Products entity)
        {
            DayaxeDbContext.Products.InsertOnSubmit(entity);
            Commit();
            return entity.ProductId;
        }

        public void Update(Products products)
        {
            var update = DayaxeDbContext.Products.FirstOrDefault(x => x.ProductId == products.ProductId);
            if (update != null)
            {
                update.ProductType = products.ProductType;
                update.ProductName = products.ProductName;
                update.MaxGuest = products.MaxGuest;
                update.ProductHighlight = products.ProductHighlight;
                update.WhatYouGet = products.WhatYouGet;
                update.Service = products.Service;
                update.IsFeatured = products.IsFeatured;

                update.RedemptionPeriod = products.RedemptionPeriod;
                update.IsCheckedInRequired = products.IsCheckedInRequired;
                update.IsKidAllow = products.IsKidAllow;

                update.MetaDescription = products.MetaDescription;
                update.MetaKeyword = products.MetaKeyword;

                update.DailySales = products.DailySales;

                update.IsActive = products.IsActive;

                update.PriceMon = products.PriceMon;
                update.PriceTue = products.PriceTue;
                update.PriceWed = products.PriceWed;
                update.PriceThu = products.PriceThu;
                update.PriceFri = products.PriceFri;
                update.PriceSat = products.PriceSat;
                update.PriceSun = products.PriceSun;

                update.UpgradeDiscountMon = products.UpgradeDiscountMon;
                update.UpgradeDiscountTue = products.UpgradeDiscountTue;
                update.UpgradeDiscountWed = products.UpgradeDiscountWed;
                update.UpgradeDiscountThu = products.UpgradeDiscountThu;
                update.UpgradeDiscountFri = products.UpgradeDiscountFri;
                update.UpgradeDiscountSat = products.UpgradeDiscountSat;
                update.UpgradeDiscountSun = products.UpgradeDiscountSun;

                update.PassCapacityMon = products.PassCapacityMon;
                update.PassCapacityTue = products.PassCapacityTue;
                update.PassCapacityWed = products.PassCapacityWed;
                update.PassCapacityThu = products.PassCapacityThu;
                update.PassCapacityFri = products.PassCapacityFri;
                update.PassCapacitySat = products.PassCapacitySat;
                update.PassCapacitySun = products.PassCapacitySun;
            }
            Commit();
        }

        public void Delete(Products entity)
        {
            var products = DayaxeDbContext.Products.FirstOrDefault(x => x.ProductId == entity.ProductId);
            if (products != null)
            {
                products.IsDelete = true;
            }
            Commit();
        }

        public void Delete(Func<Products, bool> predicate)
        {
            //IEnumerable<Bookings> bookings = DayaxeDbContext.Bookings.Where(predicate).AsEnumerable();
            //DayaxeDbContext.Bookings.DeleteAllOnSubmit(bookings);
            //Commit();
        }

        public Products GetById(long id)
        {
            var products = GetAll().FirstOrDefault(x => x.ProductId == id && x.IsDelete == false);

            var dateNow = DateTime.UtcNow;
            ReAssignProductData(ref products, dateNow);

            return products;
        }

        public Products GetById(long id, DateTime searchDate)
        {
            var products = GetAll().FirstOrDefault(x => x.ProductId == id && x.IsDelete == false);

            ReAssignProductData(ref products, searchDate);

            return products;
        }

        public Products GetById(long id, SearchParams searchParams)
        {
            var products = GetAll().FirstOrDefault(x => x.ProductId == id && x.IsDelete == false);

            ReAssignProductData(ref products, searchParams.StartDate);

            if (products != null)
            {
                searchParams.ProductType = new List<Enums.ProductType>
                {
                    (Enums.ProductType)products.ProductType
                };

                products.Similarproduct = GetSimilarProduct(id, searchParams);
            }

            return products;
        }

        public IEnumerable<Products> GetAll()
        {
            var entities = CacheLayer.Get<List<Products>>(CacheKeys.ProductsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.Products.ToList();
            CacheLayer.Add(entities, CacheKeys.ProductsCacheKey);
            return entities.AsEnumerable();
        }

        public Products Refresh(Products entity)
        {
            throw new NotImplementedException();
        }

        #region Custom

        public void Delete(int productId)
        {
            var products = DayaxeDbContext.Products.FirstOrDefault(x => x.ProductId == productId);
            if (products != null)
            {
                products.IsDelete = true;
            }
            Commit();
        }

        public List<Products> GetAvailbleUpgradeProducts(int productId, DateTime checkinDate)
        {
            var productIds = (from p in ProductList
                              join p1 in ProductUpgradeList
                              on p.ProductId equals p1.ProductId
                              where p1.ProductId == productId &&
                                  p1.UpgradeId != productId &&
                                  p.IsActive &&
                                  !p.IsDelete &&
                                  p.ProductType != (int)Enums.ProductType.DayPass
                              select p1.UpgradeId).ToList();

            var result = GetAll().Where(p => productIds.Contains(p.ProductId)).ToList();

            result.ForEach(products =>
            {
                products.BookingsToday = CountBookingsToday(products, checkinDate);

                products.DailySales = GetDefaultPassLimit(products.ProductId, checkinDate);

                products.CdnImage = new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), products.ImageUrl).AbsoluteUri;
            });

            return result;
        }

        public Products GetProductsByName(string hotelName, string productName, string sessionId = "")
        {
            var products = (from p in HotelList
                            join p1 in ProductList
                            on p.HotelId equals p1.HotelId
                            where
                            p.IsActive &&
                            !p.IsDelete &&
                            p1.IsActive &&
                            !p1.IsDelete &&
                            p.HotelName.Trim().Replace(" ", "-").Replace("-&-", "-").Replace("$", "").ToLower() == hotelName &&
                            p1.ProductName.Trim().Replace(" ", "-").Replace("-&-", "-").Replace("$", "").ToLower() == productName
                            select p1).FirstOrDefault();

            if (!string.IsNullOrEmpty(sessionId))
            {
                var user = CustomerInfoList.FirstOrDefault(x => x.SessionId == sessionId);
                if (products != null && !products.Hotels.IsPublished && (user == null || !user.IsAdmin))
                {
                    return null;
                }
            }

            ReAssignProductData(ref products, DateTime.UtcNow);

            return products;
        }

        public SearchResult SearchProducts(SearchParams searchParams)
        {
            var result = new SearchResult
            {
                Result = new ListResult<Products>()
            };

            var productResult = (from h in HotelList
                                 join p in ProductList on h.HotelId equals p.HotelId
                                 join mh in MarketHotelList on h.HotelId equals mh.HotelId
                                 join m in MarketList on mh.MarketId equals m.Id
                                 where !p.IsDelete &&
                                       !h.IsDelete &&
                                       h.IsActive &&
                                       p.IsActive &&
                                       p.ProductType != (int)Enums.ProductType.AddOns
                                 orderby h.Order
                                 select p).ToList();

            productResult = FilterByBlackOutDays(productResult, searchParams.StartDate);

            productResult.ForEach(products =>
            {
                var reviews = (from s in SurveyList
                               join b in BookingList on s.BookingId equals b.BookingId
                               join p in ProductList on b.ProductId equals p.ProductId
                               join h in HotelList on p.HotelId equals h.HotelId
                               where h.HotelId == products.HotelId
                                     && s.IsFinish
                               select s).ToList();

                products.Hotels.TotalCustomerReviews = reviews.Count;
                products.Hotels.CustomerRating = reviews.Any() ? reviews.Average(r => r.Rating ?? 5) : 0;

                var startDate = searchParams.StartDate;
                var hotels = HotelList.First(h => h.HotelId == products.HotelId);
                if (hotels != null &&
                    !string.IsNullOrEmpty(hotels.TimeZoneId) &&
                    DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).Date > startDate.Date)
                {
                    startDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).Date;
                }

                products.BookingsToday = CountBookingsToday(products, startDate);

                products.DailySales = GetDefaultPassLimit(products.ProductId, startDate);

                products.CdnImage = new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), products.ImageUrl).AbsoluteUri;

                // Calculate Distance of user with Hotels
                var currentHotelMarket = MarketHotelList.FirstOrDefault(mh => hotels != null && mh.HotelId == hotels.HotelId);
                var isCurrent = currentHotelMarket != null && currentHotelMarket.MarketId == searchParams.SearchMarkets.Id;
                products.Hotels.GetDistanceWithUser(searchParams.SearchMarkets.Latitude, searchParams.SearchMarkets.Longitude);
                products.Hotels.CalculateCustomOrder(isCurrent);

                products.CustomOrder = products.Hotels.CustomOrder;
                products.DistanceWithUser = products.Hotels.DistanceWithUser;
                products.HotelOrder = products.Hotels.Order ?? 1;

                products.ActualPriceWithDate = GetProductDailyPriceByDate(products, products.IsOnBlackOutDay ? products.NextAvailableDate : startDate);

                var autoPromo = GetAutoPromosByProductId(products.ProductId, startDate).FirstOrDefault();
                products.ActualPriceWithDate.DiscountPrice = autoPromo != null ?
                    Helper.CalculateDiscount(autoPromo, products.ActualPriceWithDate.Price, 1) :
                    products.ActualPriceWithDate.Price;

                products.PerGuestPrice = products.MaxGuest != 0 ?
                    products.ActualPriceWithDate.Price / products.MaxGuest :
                    products.ActualPriceWithDate.Price;
                products.PerGuestDiscountPrice = products.MaxGuest != 0 ?
                    products.ActualPriceWithDate.DiscountPrice / products.MaxGuest :
                    products.ActualPriceWithDate.DiscountPrice;
            });

            // Blackout day in Range
            //result = result.Where(p => !p.IsOnBlackOutDay || 
            //    (p.IsOnBlackOutDay && 
            //        searchParams.EndDate.Date >= p.NextAvailableDate.Date))
            //    .ToList();

            var user = CustomerInfoList.FirstOrDefault(x => x.CustomerId == searchParams.CustomerId);
            // if Admin Get hotel unpublish of User
            if (user == null || !user.IsAdmin)
            {
                productResult = productResult.Where(x => x.Hotels.IsPublished).ToList();
            }

            if (AppConfiguration.MaxRadius > 0)
            {
                if (searchParams.HighDistance > AppConfiguration.MaxRadius)
                {
                    searchParams.HighDistance = AppConfiguration.MaxRadius;
                }
                result.MaxDistance = AppConfiguration.MaxRadius;
                productResult = productResult.Where(p => p.DistanceWithUser <= AppConfiguration.MaxRadius)
                    .ToList();
            }

            // Search Result
            result.Result.TotalRecords = productResult.Count;
            result.MinPrice = productResult.Any() ? productResult.Min(p => p.PerGuestDiscountPrice) : 0;
            if (result.MinPrice % 1 > 0 && result.MinPrice > 1)
            {
                result.MinPrice -= 1;
            }
            result.MaxPrice = productResult.Any() ? productResult.Max(p => p.PerGuestDiscountPrice) : 98;
            result.MaxPrice += 1;
            result.MinDistance = productResult.Any() ? productResult.Min(p => p.DistanceWithUser) : 0;
            if (result.MinDistance > 1)
            {
                result.MinDistance -= 1;
            }
            result.MaxDistance = productResult.Any() ? productResult.Max(p => p.DistanceWithUser) : 98;
            result.MaxDistance += searchParams.SearchMarkets.Latitude == null ? 100 : 1;

            // Reset Search with Params
            if (searchParams.LowPrice.Equals(0))
            {
                searchParams.LowPrice = result.MinPrice;
            }
            if (searchParams.HighPrice.Equals(0))
            {
                searchParams.HighPrice = result.MaxPrice;
            }
            if (searchParams.LowDistance.Equals(0))
            {
                searchParams.LowDistance = result.MinDistance;
            }
            if (searchParams.HighDistance.Equals(0))
            {
                searchParams.HighDistance = result.MaxDistance;
            }

            // Allow / Denied location request
            if (searchParams.IsForceResetFilter)
            {
                ResetFilter(ref searchParams, result);

                result.IsResetFilter = true;
            }

            if (user != null)
            {
                try
                {
                    var searchCriteria = CustomerInfoSearchCriteriaList.FirstOrDefault(x => x.CustomerId == searchParams.CustomerId);
                    if (searchCriteria != null)
                    {
                        if (searchCriteria.Destination != searchParams.SearchMarkets.Id.ToString() ||
                            searchCriteria.TotalGuest != searchParams.AvailableTickets ||
                            searchCriteria.FromDate.Date != searchParams.StartDate.Date ||
                            searchCriteria.ToDate.Date != searchParams.EndDate.Date)
                        {
                            ResetFilter(ref searchParams, result);

                            result.IsResetFilter = true;
                        }
                        searchCriteria.Destination = searchParams.SearchMarkets.Id.ToString();
                        searchCriteria.TotalGuest = searchParams.AvailableTickets;
                        searchCriteria.IsDaypass = searchParams.ProductType.Contains(Enums.ProductType.DayPass);
                        searchCriteria.IsCabana = searchParams.ProductType.Contains(Enums.ProductType.Cabana);
                        searchCriteria.IsDaybed = searchParams.ProductType.Contains(Enums.ProductType.Daybed);
                        searchCriteria.IsSpapass = searchParams.ProductType.Contains(Enums.ProductType.SpaPass);
                        searchCriteria.MinPrice = searchParams.LowPrice;
                        searchCriteria.MaxPrice = searchParams.HighPrice;
                        searchCriteria.MinDistance = (int)searchParams.LowDistance;
                        searchCriteria.MaxDistance = (int)searchParams.HighDistance;
                        if (searchParams.StartDate > new DateTime(1753, 1, 1))
                        {
                            searchCriteria.FromDate = searchParams.StartDate;
                        }
                        if (searchParams.EndDate > new DateTime(1753, 1, 1))
                        {
                            searchCriteria.ToDate = searchParams.EndDate;
                        }

                        UpdateSearchCriteria(searchCriteria);
                    }
                    else
                    {
                        searchCriteria = new CustomerInfoSearchCriteria
                        {
                            CustomerId = searchParams.CustomerId,
                            Destination = searchParams.SearchMarkets.Id.ToString(),
                            TotalGuest = searchParams.AvailableTickets,
                            IsDaypass = searchParams.ProductType.Contains(Enums.ProductType.DayPass),
                            IsCabana = searchParams.ProductType.Contains(Enums.ProductType.Cabana),
                            IsDaybed = searchParams.ProductType.Contains(Enums.ProductType.Daybed),
                            IsSpapass = searchParams.ProductType.Contains(Enums.ProductType.SpaPass),
                            MinPrice = searchParams.LowPrice,
                            MaxPrice = searchParams.HighPrice,
                            MinDistance = (int)searchParams.LowDistance,
                            MaxDistance = (int)searchParams.HighDistance
                        };
                        if (searchParams.StartDate > new DateTime(1753, 1, 1))
                        {
                            searchCriteria.FromDate = searchParams.StartDate;
                        }
                        if (searchParams.EndDate > new DateTime(1753, 1, 1))
                        {
                            searchCriteria.ToDate = searchParams.EndDate;
                        }
                        result.IsResetFilter = true;
                        AddSearchCriteria(searchCriteria);
                        CacheLayer.Clear(CacheKeys.CustomerInfosCacheKey);
                    }
                }
                catch (Exception) { }

                CacheLayer.Clear(CacheKeys.CustomerInfosSearchCriteriaCacheKey);
            }

            // Filter by Price
            productResult = productResult.Where(p => p.PerGuestDiscountPrice >= searchParams.LowPrice && p.PerGuestDiscountPrice <= searchParams.HighPrice)
                .ToList();

            // Filter by Distance
            productResult = productResult
                .Where(p => ((int)searchParams.LowDistance == 1 || p.DistanceWithUser >= searchParams.LowDistance) &&
                    p.DistanceWithUser <= searchParams.HighDistance + 1)
                .ToList();

            // Filter with Available Guest
            //result = result.Where(p => p.DailySales * p.MaxGuest - p.BookingsToday * p.MaxGuest >= searchParams.AvailableTickets).ToList();

            // Filter by Product Type
            productResult = productResult.Where(p => searchParams.ProductType.Contains((Enums.ProductType)p.ProductType)).ToList();

            // With Location Order by CustomOrder
            productResult = productResult
                .OrderBy(r => r.CustomOrder)
                .ThenBy(r => r.DistanceWithUser)
                .ToList();

            if (productResult.Count > 3)
            {
                var productsAdd = new Products
                {
                    CdnImage = new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), "/images/gold-pass.jpg").AbsoluteUri
                };
                productResult.Insert(3, productsAdd);
            }

            result.Result.Items = productResult;

            return result;
        }

        public List<Products> SearchProductsByCode()
        {
            var products = (from p in HotelList
                            join p1 in ProductList
                            on p.HotelId equals p1.HotelId
                            where !p1.IsDelete &&
                                !p.IsDelete &&
                                p.IsActive &&
                                p1.IsActive
                            orderby p.Order
                            select p1).ToList();

            return products.OrderBy(h => h.Hotels.Order).ToList();
        }

        public bool CheckAvailableProduct(CheckAvailableProductParams param, out int ticketAvailable)
        {
            ticketAvailable = 0;
            if (!param.IsAdmin && param.CheckInDate.Date < DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(param.TimezoneId).Date)
            {
                return false;
            }
            // On Blackout 
            var isOnBlackOut = (from p in BlockedDatesCustomPriceList
                                join p2 in ProductList on p.ProductId equals p2.ProductId
                                where p2.ProductId == param.ProductId &&
                                      p.Date.Date == param.CheckInDate.Date &&
                                      p.Capacity == 0
                                select p).FirstOrDefault();
            if (isOnBlackOut != null)
            {
                return false;
            }

            // Decrease Quantity of bookings
            if (param.BookingId > 0)
            {
                var bookings = BookingList.FirstOrDefault(b => b.BookingId == param.BookingId);
                if (bookings != null && bookings.Quantity > param.TotalTicket)
                {
                    return true;
                }
            }

            // Check with Total Booking on this checkindate
            var product = GetById(param.ProductId);
            var maxCapacity = GetDefaultPassLimit(product.ProductId, param.CheckInDate);

            var totalBookings = BookingList
                .Where(x => x.ProductId == product.ProductId &&
                            x.CheckinDate.HasValue &&
                            x.CheckinDate.Value.Date == param.CheckInDate.Date &&
                            x.BookingId != param.BookingId &&
                            x.PassStatus != (int)Enums.BookingStatus.Refunded).Sum(x => x.Quantity);
            ticketAvailable = maxCapacity - totalBookings;
            return maxCapacity >= totalBookings + param.TotalTicket;
        }

        public List<Products> SearchProductsByHotelId(int hotelId)
        {
            return GetAll().Where(h => h.HotelId == hotelId && !h.IsDelete).ToList();
        }

        public List<Products> GetProductsUpgradeByProductId(int productId)
        {
            List<int> upgradeIds = (from p in ProductList
                                    join p1 in ProductUpgradeList
                                    on p.ProductId equals p1.ProductId
                                    where p.ProductId == productId && !p.IsDelete && p.IsActive
                                    select p1.UpgradeId).ToList();
            upgradeIds.Add(productId);

            var products = (from p in ProductList where upgradeIds.Contains(p.ProductId) select p).ToList();
            return products;
        }

        public Products GetProductByHotelName(string hotelName)
        {
            var hotels =
                HotelList.FirstOrDefault(p =>
                        p.HotelName.Trim()
                            .Replace(" ", "-")
                            .Replace("-&-", "-")
                            .Replace("$", "")
                            .ToLower()
                            .Contains(hotelName));
            if (hotels != null)
            {
                return
                    GetAll().FirstOrDefault(
                        p => p.HotelId == hotels.HotelId &&
                        p.IsActive &&
                        !p.IsDelete &&
                        p.ProductType == (int)Enums.ProductType.DayPass);
            }
            return null;
        }

        public Products GetProductBySurveyId(string surveyId)
        {
            var survey = SurveyList.FirstOrDefault(s => s.Code == surveyId);
            if (survey != null)
            {
                return survey.Bookings.Products;
            }
            return null;
        }

        public IEnumerable<Products> GetByHotelId(int hotelId, int productType)
        {
            return (from h in HotelList
                    join p in ProductList on h.HotelId equals p.HotelId
                    where h.IsActive && !h.IsDelete &&
                          p.IsActive && !p.IsDelete &&
                          h.HotelId == hotelId &&
                          p.ProductType == productType
                    select p);
        }

        public KeyValuePair<IEnumerable<Tickets>, IEnumerable<Tickets>> GetTicketsFuture(int productId)
        {
            var products =
                ProductList.First(p => p.ProductId == productId);

            var defaultTickets = new List<Tickets>
            {
                new Tickets
                {
                    D = "Sun",
                    T = products.PassCapacitySun
                },
                new Tickets
                {
                    D = "Mon",
                    T = products.PassCapacityMon
                },
                new Tickets
                {
                    D = "Tue",
                    T = products.PassCapacityTue
                },
                new Tickets
                {
                    D = "Wed",
                    T = products.PassCapacityWed
                },
                new Tickets
                {
                    D = "Thu",
                    T = products.PassCapacityThu
                },
                new Tickets
                {
                    D = "Fri",
                    T = products.PassCapacityFri
                },
                new Tickets
                {
                    D = "Sat",
                    T = products.PassCapacitySat
                }
            };

            var today = DateTime.UtcNow;

            var bookDate = (from p in ProductList
                            join b in BookingList on p.ProductId equals b.ProductId
                            join h in HotelList on p.HotelId equals h.HotelId
                            let checkInDate = b.CheckinDate
                            where checkInDate.HasValue &&
                                  checkInDate.Value.ToLosAngerlesTimeWithTimeZone(h.TimeZoneId).Date >= today.ToLosAngerlesTimeWithTimeZone(h.TimeZoneId).Date &&
                                  p.ProductId == productId &&
                                  b.PassStatus != (int)Enums.BookingStatus.Refunded
                            group b by b.CheckinDate
                into g
                            select new
                            {
                                Date = g.Key.Value,
                                Quantity = g.Sum(x => x.Quantity)
                            }).ToList();

            var blackOutDates = (from bd in BlockedDatesCustomPriceList
                                 join p in ProductList on bd.ProductId equals p.ProductId
                                 where p.ProductId == productId &&
                                       bd.Capacity != 0 &&
                                       bd.Date.Date >= today.Date
                                 orderby bd.Date
                                 select bd).ToList();

            var ticketsNew = new List<Tickets>();

            // Get On Day have Booking
            bookDate.ForEach(date =>
            {
                var quantity = date.Quantity;
                bool hasCustomCapacity = false;
                var blockedDateAtDate =
                    blackOutDates.FirstOrDefault(bd => bd.Date.Date == date.Date.Date);

                if (blockedDateAtDate != null)
                {
                    var customCapacity = BlockedDatesCustomPriceList
                        .FirstOrDefault(x => x.Id == blockedDateAtDate.Id && x.ProductId == productId);
                    if (customCapacity != null && customCapacity.Capacity.HasValue && customCapacity.Capacity.Value != 0)
                    {
                        quantity = customCapacity.Capacity.Value - date.Quantity;
                        blackOutDates.Remove(blockedDateAtDate);
                        hasCustomCapacity = true;
                    }
                }

                if (!hasCustomCapacity)
                {
                    switch (date.Date.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            quantity = products.PassCapacityMon - date.Quantity;
                            break;
                        case DayOfWeek.Tuesday:
                            quantity = products.PassCapacityTue - date.Quantity;
                            break;
                        case DayOfWeek.Wednesday:
                            quantity = products.PassCapacityWed - date.Quantity;
                            break;
                        case DayOfWeek.Thursday:
                            quantity = products.PassCapacityThu - date.Quantity;
                            break;
                        case DayOfWeek.Friday:
                            quantity = products.PassCapacityFri - date.Quantity;
                            break;
                        case DayOfWeek.Saturday:
                            quantity = products.PassCapacitySat - date.Quantity;
                            break;
                        case DayOfWeek.Sunday:
                            quantity = products.PassCapacitySun - date.Quantity;
                            break;
                    }
                }
                ticketsNew.Add(new Tickets
                {
                    D = date.Date.ToString(Constant.DiscountDateFormat),
                    T = quantity
                });
            });

            blackOutDates.ForEach(item =>
            {
                ticketsNew.Add(new Tickets
                {
                    D = item.Date.ToString(Constant.DiscountDateFormat),
                    T = item.Capacity ?? GetDefaultPassLimit(productId, item.Date)
                });
            });

            return new KeyValuePair<IEnumerable<Tickets>, IEnumerable<Tickets>>(ticketsNew, defaultTickets);
        }

        public List<Products> GetProductsAdOnsByProductId(int productId)
        {
            List<int> upgradeIds = (from p in ProductList
                                    join p1 in ProductAddOnList
                                    on p.ProductId equals p1.ProductId
                                    where p.ProductId == productId && !p.IsDelete && p.IsActive
                                    select p1.AddOnId).ToList();

            var products = (from p in ProductList where upgradeIds.Contains(p.ProductId) select p).ToList();
            return products;
        }

        public Products GetProductsByBookingId(int bookingId)
        {
            return (from p in ProductList
                    join b in BookingList on p.ProductId equals b.ProductId
                    where b.BookingId == bookingId
                    select p).FirstOrDefault();
        }

        public List<Products> GetFeaturedProducts()
        {
            var result = (from p in ProductList
                          join h in HotelList on p.HotelId equals h.HotelId
                          where p.IsFeatured && !p.IsDelete && p.IsActive
                                && h.IsPublished && !h.IsDelete && h.IsActive
                          select p).ToList();

            var startDate = DateTime.UtcNow.ToLosAngerlesTime();
            result.ForEach(products =>
            {
                products.BookingsToday = CountBookingsToday(products, startDate);

                products.DailySales = GetDefaultPassLimit(products.ProductId, startDate);

                products.CdnImage = new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), products.ImageUrl).AbsoluteUri;

                var customer = (from b in BookingList
                                join c in CustomerInfoList on b.CustomerId equals c.CustomerId
                                where b.ProductId == products.ProductId
                                orderby b.BookingId descending
                                select c).FirstOrDefault();
                if (customer != null)
                {
                    products.CustomerName = string.Format("{0} {1}", customer.FirstName, customer.LastName);
                }

                products.ActualPriceWithDate = GetProductDailyPriceByDate(products, startDate);
            });

            result = FilterByBlackOutDays(result, startDate);

            return result;
        }

        #endregion

        #region DefaultPrices

        public void AddDefaultPrices(DefaultPrices entity)
        {
            DayaxeDbContext.DefaultPrices.InsertOnSubmit(entity);
            Commit();
        }

        #endregion

        #region Add-Ons

        public int AddAddOns(ProductAddOns entity)
        {
            DayaxeDbContext.ProductAddOns.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        public void DeleteAddOns(List<ProductAddOns> addOns)
        {
            if (addOns.Any())
            {
                var hotelIds = addOns.Select(x => x.AddOnId).ToList();

                var removeList = DayaxeDbContext.ProductAddOns
                    .Where(x => x.ProductId == addOns.First().ProductId
                                && hotelIds.Contains(x.AddOnId));
                DayaxeDbContext.ProductAddOns.DeleteAllOnSubmit(removeList);
            }
            Commit();
        }

        #endregion

        #region Private Function 

        private int CountBookingsToday(Products products, DateTime dateNow)
        {
            if (products.IsCheckedInRequired)
            {
                // Do not need ToLosasgenesTime here because checkin on Date, not Time
                return BookingList
                    .Where(x => x.ProductId == products.ProductId &&
                        x.CheckinDate.HasValue &&
                        x.CheckinDate.Value.Date == dateNow.Date).Sum(x => x.Quantity);
            }
            var hotel = HotelList.FirstOrDefault(h => h.HotelId == products.HotelId);
            return BookingList
                .Where(x => x.ProductId == products.ProductId &&
                    x.BookedDate.ToLosAngerlesTimeWithTimeZone(hotel != null ? hotel.TimeZoneId : Constant.UsDefaultTime).Date == dateNow.Date)
                .Sum(x => x.Quantity);
        }

        private void ReAssignProductData(ref Products products, DateTime dateNow)
        {
            if (products != null)
            {
                //var currentProductType = products.ProductType;
                var productId = products.ProductId;
                var hotelId = products.HotelId;

                var startDate = dateNow;
                var hotels = GetHotels(hotelId);

                if (hotels != null &&
                    !string.IsNullOrEmpty(hotels.TimeZoneId) &&
                    DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).Date > startDate.Date)
                {
                    startDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).Date;

                    // Show next available when hotels is on new timezone 
                    products.IsOnBlackOutDay = true;
                    products.NextAvailableDate = startDate;
                    products.NextAvailableDay = string.Format("Available {0:MMM dd}", startDate);
                }

                var dayPassValid = products.RedemptionPeriod > 0 ? products.RedemptionPeriod : Constant.DefaultRedemptionPeriod;
                var expiredDate = DateTime.UtcNow.AddDays(dayPassValid);

                // Filter Hotels by Blackout Day
                products = FilterByBlackOutDays(new List<Products> { products }, dateNow).First();

                var photos = ProductImageList.Where(x => x.ProductId == productId && x.IsActive).OrderBy(x => x.Order).Select(x => x.Url).ToList();
                photos.AddRange(GetPhotosByHotelsId(products.HotelId));
                products.PhotoUrls = photos.Distinct().ToArray();

                products.PoolFeatures = hotels.PoolAmentyListses.Select(x => x.Name).ToArray();
                products.GymFeatures = hotels.GymAmentyListses.Select(x => x.Name).ToArray();
                products.SpaFeatures = hotels.SpaAmentyListses.Select(x => x.Name).ToArray();
                products.OfficeFeatures = hotels.BusinessCenterAmentyListses.Select(x => x.Name).ToArray();

                products.GetNearBlockedDates = new List<BlockedDatesCustomPrice>();
                products.GetNearBlockedDates.AddRange(BlockedDatesCustomPriceList
                    .Where(x => x.ProductId == productId &&
                        x.Date.Date >= DateTime.UtcNow.Date &&
                        x.Capacity == 0 &&
                        x.Date.Date <= expiredDate.Date)
                    .DistinctBy(x => x.Date)
                    .OrderBy(x => x.Date)
                    .Take(dayPassValid)
                    .ToList());

                products.AmentiesHotels = AmentiesList.FirstOrDefault(x => x.HotelId == hotelId);

                products.BookingsToday = CountBookingsToday(products, dateNow);

                products.RelatedProducts = ProductList.Where(products1 => products1.HotelId == hotelId &&
                        products1.ProductId != productId &&
                        !products1.IsDelete &&
                        products1.IsActive && products1.ProductType != (int)Enums.ProductType.AddOns)
                    .ToList();

                products.AddOnsproduct = (from p in ProductList
                                          join pa in ProductAddOnList on p.ProductId equals pa.AddOnId
                                          where pa.ProductId == productId && !p.IsDelete && p.IsActive
                                          select p).ToList();

                products.ActualPriceWithDate = GetProductDailyPriceByDate(products, dateNow);

                products.RelatedProducts.ForEach(productItem =>
                {
                    productItem.ActualPriceWithDate = GetProductDailyPriceByDate(productItem, dateNow);
                });

                products.AddOnsproduct.ForEach(productItem =>
                {
                    productItem.ActualPriceWithDate = GetProductDailyPriceByDate(productItem, dateNow);
                    productItem.CdnImage = new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), productItem.ImageUrl).AbsoluteUri;
                    productItem.BookingsToday = CountBookingsToday(productItem, dateNow);
                    productItem.PassCapacity = productItem.ActualPriceWithDate.Capacity;
                });

                // Only Add-Ons available on this day
                products.AddOnsproduct = products.AddOnsproduct
                    .Where(p => p.PassCapacity > p.BookingsToday)
                    .ToList();

                // Fine Print
                products.FinePrint = GetFinePrintByHotelId(products.HotelId).ToList();

                var reviews = (from s in SurveyList
                               join b in BookingList on s.BookingId equals b.BookingId
                               join p in ProductList on b.ProductId equals p.ProductId
                               join h in HotelList on p.HotelId equals h.HotelId
                               where h.HotelId == hotelId
                                     && s.IsFinish
                               select s).ToList();

                products.Hotels.TotalCustomerReviews = reviews.Count;
                products.Hotels.CustomerRating = reviews.Any() ? reviews.Average(r => r.Rating ?? 5) : 0;
            }
        }

        private List<Products> FilterByBlackOutDays(List<Products> listProducts, DateTime? dateNow)
        {
            var currentDate = DateTime.UtcNow;
            if (dateNow.HasValue)
            {
                currentDate = dateNow.Value;
            }

            //var productsWithBdIsToday = (from prod in listProducts
            //                             join bd in BlockedDatesCustomPriceList on prod.ProductId equals bd.ProductId
            //                             where prod.IsActive
            //                                    && dateNow.HasValue
            //                                    && bd.Date.Date == dateNow.Value.Date
            //                                    && bd.Capacity == 0
            //                             select prod).ToList();

            var tempProductsWithBdIsToday = listProducts
                                        .Join(BlockedDatesCustomPriceList, p => p.ProductId, d => d.ProductId, (prod, blockDate) => new { prod, blockDate })
                                        .Where(x => x.prod.IsActive);

            // Including products which have PassCapacity == 0 in current day
            switch (currentDate.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    tempProductsWithBdIsToday = tempProductsWithBdIsToday
                                        .Where(x => (dateNow.HasValue && x.blockDate.Date.Date == dateNow.Value.Date && x.blockDate.Capacity == 0) || x.prod.PassCapacitySun == 0);
                    break;
                case DayOfWeek.Monday:
                    tempProductsWithBdIsToday = tempProductsWithBdIsToday
                                        .Where(x => (dateNow.HasValue && x.blockDate.Date.Date == dateNow.Value.Date && x.blockDate.Capacity == 0) || x.prod.PassCapacityMon == 0);
                    break;
                case DayOfWeek.Tuesday:
                    tempProductsWithBdIsToday = tempProductsWithBdIsToday
                                        .Where(x => (dateNow.HasValue && x.blockDate.Date.Date == dateNow.Value.Date && x.blockDate.Capacity == 0) || x.prod.PassCapacityTue == 0);
                    break;
                case DayOfWeek.Wednesday:
                    tempProductsWithBdIsToday = tempProductsWithBdIsToday
                                        .Where(x => (dateNow.HasValue && x.blockDate.Date.Date == dateNow.Value.Date && x.blockDate.Capacity == 0) || x.prod.PassCapacityWed == 0);
                    break;
                case DayOfWeek.Thursday:
                    tempProductsWithBdIsToday = tempProductsWithBdIsToday
                                        .Where(x => (dateNow.HasValue && x.blockDate.Date.Date == dateNow.Value.Date && x.blockDate.Capacity == 0) || x.prod.PassCapacityThu == 0);
                    break;
                case DayOfWeek.Friday:
                    tempProductsWithBdIsToday = tempProductsWithBdIsToday
                                        .Where(x => (dateNow.HasValue && x.blockDate.Date.Date == dateNow.Value.Date && x.blockDate.Capacity == 0) || x.prod.PassCapacityFri == 0);
                    break;
                case DayOfWeek.Saturday:
                    tempProductsWithBdIsToday = tempProductsWithBdIsToday
                                        .Where(x => (dateNow.HasValue && x.blockDate.Date.Date == dateNow.Value.Date && x.blockDate.Capacity == 0) || x.prod.PassCapacitySat == 0);
                    break;
                default:
                    break;
            }

            var productsWithBdIsToday = tempProductsWithBdIsToday.Select(x => x.prod).ToList();

            var blackoutDaysOfProducts = (from prod in listProducts
                                          join p1 in BlockedDatesCustomPriceList on prod.ProductId equals p1.ProductId
                                          where dateNow.HasValue &&
                                              p1.Date.Date >= dateNow.Value.Date &&
                                              p1.Capacity == 0
                                          select p1).ToList();

            // Blackout Day is Today
            listProducts = listProducts.Select(product =>
            {
                var hotels = HotelList.FirstOrDefault(h => h.HotelId == product.HotelId);
                product.NextAvailableDate = currentDate.ToLosAngerlesTimeWithTimeZone(hotels != null ? hotels.TimeZoneId : Constant.UsDefaultTime);
                product.IsOnBlackOutDay = false;
                int day = 1;
                if (productsWithBdIsToday.Contains(product) ||
                    IsSoldOut(product, currentDate.AddDays(day - 1)) ||
                    (hotels != null &&
                         !string.IsNullOrEmpty(hotels.TimeZoneId) &&
                         DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId) > currentDate.Date.AddHours(AppConfiguration.RestrictBookingSameDayAtHour.Hour)))
                {
                    product.IsOnBlackOutDay = true;

                    var checkDate = currentDate;
                    int checkedDays = 0;

                    bool isNotAvailable = true;
                    while (isNotAvailable && (blackoutDaysOfProducts != null && blackoutDaysOfProducts.Where(x => x.ProductId == product.ProductId).Any() ? currentDate.AddDays(day).Date <= blackoutDaysOfProducts.Where(x => x.ProductId == product.ProductId).Max(x => x.Date) : true))
                    {
                        // Checking PassCapacity for 7 days from "Inventory&Pricing"
                        var isAvailableDay = true;
                        int countDay = 0;
                        while (isAvailableDay && countDay < 8)
                        {
                            switch (checkDate.DayOfWeek)
                            {
                                case DayOfWeek.Sunday:
                                    if (product.PassCapacitySun == 0)
                                        checkDate = checkDate.AddDays(1);
                                    else
                                        isAvailableDay = false;
                                    break;
                                case DayOfWeek.Monday:
                                    if (product.PassCapacityMon == 0)
                                        checkDate = checkDate.AddDays(1);
                                    else
                                        isAvailableDay = false;
                                    break;
                                case DayOfWeek.Tuesday:
                                    if (product.PassCapacityTue == 0)
                                        checkDate = checkDate.AddDays(1);
                                    else
                                        isAvailableDay = false;
                                    break;
                                case DayOfWeek.Wednesday:
                                    if (product.PassCapacityWed == 0)
                                        checkDate = checkDate.AddDays(1);
                                    else
                                        isAvailableDay = false;
                                    break;
                                case DayOfWeek.Thursday:
                                    if (product.PassCapacityThu == 0)
                                        checkDate = checkDate.AddDays(1);
                                    else
                                        isAvailableDay = false;
                                    break;
                                case DayOfWeek.Friday:
                                    if (product.PassCapacityFri == 0)
                                        checkDate = checkDate.AddDays(1);
                                    else
                                        isAvailableDay = false;
                                    break;
                                case DayOfWeek.Saturday:
                                    if (product.PassCapacitySat == 0)
                                        checkDate = checkDate.AddDays(1);
                                    else
                                        isAvailableDay = false;
                                    break;
                                default:
                                    break;
                            }

                            countDay++;
                        }

                        checkedDays = countDay;

                        var nextDateAvailable = blackoutDaysOfProducts.Count(b =>
                            b.ProductId == product.ProductId &&
                            b.Capacity == 0 &&
                            b.Date.Date == ((checkDate > currentDate) ? checkDate : currentDate.AddDays(day).Date)
                            ) > 0;

                         //

                        // Check next day is not blackout and not sold out
                        if (!nextDateAvailable && !IsSoldOut(product, checkDate.AddDays(day)))//currentDate.AddDays(day)))
                        {
                            isNotAvailable = false;
                        }
                        else
                        {
                            day++;
                        }
                    }
                    product.NextAvailableDate = (checkDate > currentDate && checkedDays < 8) ? checkDate : currentDate.AddDays(day).Date;
                    product.NextAvailableDay = string.Format("Available {0:MMM dd}", product.NextAvailableDate);
                }

                return product;
            }).ToList();

            return listProducts;
        }

        private bool IsSoldOut(Products products, DateTime startDate)
        {
            var bookingsToday = CountBookingsToday(products, startDate);

            var dailySales = GetDefaultPassLimit(products.ProductId, startDate);

            return dailySales > 0 && bookingsToday > 0 && dailySales <= bookingsToday;
        }

        private IEnumerable<string> GetPhotosByHotelsId(int hotelId)
        {
            var productPhotos = GetHotels(hotelId);
            if (productPhotos != null)
            {
                return productPhotos.PhotoList.Select(x => x.Url);
            }
            return new List<string>();
        }

        private Hotels GetHotels(int id)
        {
            Hotels hotels = HotelList.FirstOrDefault(x => x.HotelId == id && x.IsDelete == false);

            if (hotels != null)
            {
                SetHotelData(ref hotels, hotels.HotelId);

                hotels.PhotoUrls = hotels.PhotoList.Select(x => x.Url).ToArray();
                hotels.PoolFeatures = hotels.PoolAmentyListses.Select(x => x.Name).ToArray();
                hotels.GymFeatures = hotels.GymAmentyListses.Select(x => x.Name).ToArray();
                hotels.SpaFeatures = hotels.SpaAmentyListses.Select(x => x.Name).ToArray();
                hotels.OfficeFeatures = hotels.BusinessCenterAmentyListses.Select(x => x.Name).ToArray();
                hotels.DinningFeatures = hotels.DinningAmentyListes.Select(x => x.Name).ToArray();
                hotels.EventFeatures = hotels.EventAmentyListes.Select(x => x.Name).ToArray();

                hotels.AmentiesHotels = AmentiesList.FirstOrDefault(x => x.HotelId == id);
            }
            return hotels;
        }

        public int GetDefaultPassLimit(int productId, DateTime checkInDate)
        {
            var products = ProductList.FirstOrDefault(p => p.IsActive && !p.IsDelete && p.ProductId == productId);

            if (products != null)
            {
                // if Changed Default Capacity for this day
                var customCapacity = BlockedDatesCustomPriceList
                    .FirstOrDefault(x => x.Date.Date == checkInDate.Date && x.ProductId == productId);
                if (customCapacity != null && customCapacity.Capacity.HasValue && customCapacity.Capacity.Value > 0)
                {
                    return customCapacity.Capacity.Value;
                }

                switch (checkInDate.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        return products.PassCapacityMon;
                    case DayOfWeek.Tuesday:
                        return products.PassCapacityTue;
                    case DayOfWeek.Wednesday:
                        return products.PassCapacityWed;
                    case DayOfWeek.Thursday:
                        return products.PassCapacityThu;
                    case DayOfWeek.Friday:
                        return products.PassCapacityFri;
                    case DayOfWeek.Saturday:
                        return products.PassCapacitySat;
                    default:
                        return products.PassCapacitySun;
                }
            }
            return 0;
        }

        private List<Products> GetSimilarProduct(long productId, SearchParams searchParams)
        {
            var result = (from h in HotelList
                          join p in ProductList
                              on h.HotelId equals p.HotelId
                          where !p.IsDelete && !h.IsDelete &&
                                h.IsActive && p.IsActive &&
                                p.ProductId != productId
                          orderby h.Order
                          select p).ToList();

            result.ForEach(products =>
            {
                products.BookingsToday = CountBookingsToday(products, searchParams.StartDate);

                products.DailySales = GetDefaultPassLimit(products.ProductId, searchParams.StartDate);

                products.CdnImage = new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), products.ImageUrl).AbsoluteUri;

                // Calculate Distance of user with Hotels
                var currentHotelMarket = MarketHotelList.FirstOrDefault(mh => mh.HotelId == products.HotelId);
                var isCurrent = currentHotelMarket != null && currentHotelMarket.MarketId == searchParams.SearchMarkets.Id;
                products.Hotels.GetDistanceWithUser(searchParams.SearchMarkets.Latitude, searchParams.SearchMarkets.Longitude);
                products.Hotels.CalculateCustomOrder(isCurrent);

                products.CustomOrder = products.Hotels.CustomOrder;
                products.DistanceWithUser = products.Hotels.DistanceWithUser;
                products.HotelOrder = products.Hotels.Order ?? 1;

                var customer = (from b in BookingList
                                join c in CustomerInfoList on b.CustomerId equals c.CustomerId
                                where b.ProductId == products.ProductId
                                orderby b.BookingId descending
                                select c).FirstOrDefault();
                if (customer != null)
                {
                    products.CustomerName = string.Format("{0} {1}", customer.FirstName, customer.LastName);
                }

                products.ActualPriceWithDate = GetProductDailyPriceByDate(products, searchParams.StartDate);
            });

            result = FilterByBlackOutDays(result, searchParams.StartDate);

            var user = CustomerInfoList.FirstOrDefault(x => x.CustomerId == searchParams.CustomerId && x.IsAdmin);
            // if Admin Get hotel unpublish of User
            if (user == null)
            {
                result = result.Where(x => x.Hotels.IsPublished).ToList();
            }

            // Remove Add-Ons from result
            switch (searchParams.ProductType.First())
            {
                case Enums.ProductType.DayPass:
                case Enums.ProductType.Cabana:
                case Enums.ProductType.Daybed:
                case Enums.ProductType.SpaPass:
                    result = result.Where(p => p.ProductType != (int)Enums.ProductType.AddOns)
                        .ToList();
                    break;
            }

            var productWithOtherProductType = result.Where(p => p.ProductType != (int)searchParams.ProductType.First())
                .ToList();

            productWithOtherProductType.ForEach(product =>
            {
                product.CustomOrder += 8;
                product.HotelOrder += 999;
            });

            // With Location Order by CustomOrder
            return result
                .OrderBy(r => r.CustomOrder)
                .ThenBy(r => r.DistanceWithUser)
                .ToList();
        }

        private ActualPriceObject GetProductDailyPriceByDate(Products products, DateTime date)
        {
            if (BlockedDatesCustomPriceList.Any(bdcpchild => bdcpchild.ProductId == products.ProductId && bdcpchild.Date == date.Date))
            {
                var result = GetCustomPriceByDate(products.ProductId, date);

                if (result.Price.Equals(0))
                {
                    var defaultPrice = GetRegularPriceByDate(products, date);
                    result.Price = defaultPrice.Price;
                }

                if (result.Capacity == -1)
                {
                    var defaultPrice = GetRegularPriceByDate(products, date);
                    result.Capacity = defaultPrice.Capacity;
                }

                return result;
            }

            return GetRegularPriceByDate(products, date);
        }

        private ActualPriceObject GetCustomPriceByDate(long productId, DateTime date)
        {
            var customPrice = BlockedDatesCustomPriceList
                .First(bdcpchild => bdcpchild.ProductId == productId && bdcpchild.Date.Date == date.Date.Date);
            return new ActualPriceObject
            {
                Price = customPrice.RegularPrice,
                DiscountPrice = customPrice.DiscountPrice,
                Capacity = customPrice.Capacity ?? 0
            };
        }

        private ActualPriceObject GetRegularPriceByDate(Products product, DateTime date)
        {
            var defaultPrice = DefaultPriceList.Where(d => d.EffectiveDate.Date < date.Date && d.ProductId == product.ProductId)
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

        private void ResetFilter(ref SearchParams searchParams, SearchResult result)
        {
            if (!searchParams.ProductType.Contains(Enums.ProductType.DayPass))
            {
                searchParams.ProductType.Add(Enums.ProductType.DayPass);
            }
            if (!searchParams.ProductType.Contains(Enums.ProductType.Cabana))
            {
                searchParams.ProductType.Add(Enums.ProductType.Cabana);
            }
            if (!searchParams.ProductType.Contains(Enums.ProductType.Daybed))
            {
                searchParams.ProductType.Add(Enums.ProductType.Daybed);
            }
            if (!searchParams.ProductType.Contains(Enums.ProductType.SpaPass))
            {
                searchParams.ProductType.Add(Enums.ProductType.SpaPass);
            }

            searchParams.LowPrice = result.MinPrice;
            searchParams.HighPrice = result.MaxPrice;
            searchParams.LowDistance = result.MinDistance;
            searchParams.HighDistance = result.MaxDistance;
        }

        private IEnumerable<Policies> GetFinePrintByHotelId(int hotelId)
        {
            return (from p in PoliciesList
                    join hp in HotelPoliciesList on p.Id equals hp.PolicyId
                    where hp.HotelId == hotelId && !p.IsDelete
                    orderby hp.Order
                    select p);
        }

        #endregion

        #region Bookings

        public DateTime GetMostRecentDate(int customerId)
        {
            DateTime date = DateTime.UtcNow.ToLosAngerlesTime().Date;
            var firstBookings = BookingList
                .Where(b => b.CustomerId == customerId && b.PassStatus == (int)Enums.BookingStatus.Active && b.CheckinDate.HasValue)
                .OrderBy(b => b.CheckinDate).FirstOrDefault();
            if (firstBookings != null)
            {
                date = firstBookings.CheckinDate.HasValue ? firstBookings.CheckinDate.Value : date;
            }

            return date;
        }

        #endregion

        #region Waiting List

        public void AddWaitList(ProductWaittingLists item)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                var exists =
                    DayaxeDbContext.ProductWaittingLists
                        .FirstOrDefault(x => x.EmailAddress.ToUpper() == item.EmailAddress.ToUpper() &&
                             x.CheckInDate.Date == item.CheckInDate.Date);

                if (exists == null)
                {
                    DayaxeDbContext.ProductWaittingLists.InsertOnSubmit(item);

                    var url = string.Format(Constant.KlaviyoListApiUrl, AppConfiguration.KlaviyoWaittingListId);
                    var addToListKlaviyoRes = Helper.Post(url, item.EmailAddress);

                    var logs = new Logs
                    {
                        LogKey = "Klaviyo_Add_To_WaitList",
                        UpdatedBy = 1,
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedContent = string.Format("{0} - {1}", item.EmailAddress, addToListKlaviyoRes)
                    };

                    DayaxeDbContext.Logs.InsertOnSubmit(logs);

                    Commit();
                }

                transaction.Complete();
            }
        }

        public DateTime GetFirstSoldOutDate(int productId)
        {
            var isNotSoldOut = true;
            int idx = 1;
            DateTime date = DateTime.UtcNow.AddDays(30);

            var products = GetAll().FirstOrDefault(x => x.ProductId == productId && x.IsDelete == false);

            while (isNotSoldOut && idx < 30)
            {
                date = DateTime.UtcNow.AddDays(idx);
                products = FilterByBlackOutDays(new List<Products> { products }, date).First();
                if (products.IsOnBlackOutDay)
                {
                    isNotSoldOut = false;
                }
                idx++;
            }

            return date;
        }

        #endregion

        #region Search Criteria

        private void UpdateSearchCriteria(CustomerInfoSearchCriteria entity)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                var update = DayaxeDbContext.CustomerInfoSearchCriteria.FirstOrDefault(x => x.CustomerId == entity.CustomerId);
                if (update != null)
                {
                    update.Destination = entity.Destination;
                    update.TotalGuest = entity.TotalGuest;
                    update.IsDaypass = entity.IsDaypass;
                    update.IsCabana = entity.IsCabana;
                    update.IsSpapass = entity.IsSpapass;
                    update.IsDaybed = entity.IsDaybed;
                    update.MinPrice = entity.MinPrice;
                    update.MaxPrice = entity.MaxPrice;
                    update.MinDistance = entity.MinDistance;
                    update.MaxDistance = entity.MaxDistance;
                    update.FromDate = entity.FromDate;
                    update.ToDate = entity.ToDate;
                    Commit();
                }
                transaction.Complete();
            }
        }

        private void AddSearchCriteria(CustomerInfoSearchCriteria entity)
        {
            DayaxeDbContext.CustomerInfoSearchCriteria.InsertOnSubmit(entity);
            Commit();
        }

        #endregion

        #region BookingsTemp

        public Tuple<BookingsTemps, DiscountBookingsTemps> GetBookingsTempById(int bookingsTempId)
        {
            var bookingsTemp = DayaxeDbContext.BookingsTemps.FirstOrDefault(b => b.BookingId == bookingsTempId);
            var discounts = DayaxeDbContext.DiscountBookingsTemps.FirstOrDefault(x => x.BookingId == bookingsTempId);
            return new Tuple<BookingsTemps, DiscountBookingsTemps>(bookingsTemp, discounts);
        }

        #endregion

        #region Markerts

        public Markets GetMarketsByHotelId(int hotelId)
        {
            return (from m in MarketList
                    join mh in MarketHotelList on m.Id equals mh.MarketId
                    join h in HotelList on mh.HotelId equals h.HotelId
                    where h.HotelId == hotelId
                    select m).FirstOrDefault();
        }

        #endregion
    }
}

