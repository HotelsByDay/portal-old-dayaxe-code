using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Transactions;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;

namespace DayaxeDal
{
    public partial class BaseRepository : IDisposable
    {
        public IEnumerable<Photos> PhotoList;
        public IEnumerable<AmentyLists> AmentyListsList;

        public IEnumerable<Hotels> HotelList;
        public IEnumerable<Amenties> AmentiesList;
        public IEnumerable<Products> ProductList;
        public IEnumerable<CustomerInfos> CustomerInfoList;
        public IEnumerable<MarketHotels> MarketHotelList;
        //public IEnumerable<BlockedDates> BlockedDateList;
        public IEnumerable<ProductUpgrades> ProductUpgradeList;
        public IEnumerable<CustomerInfosHotels> UserHotelList;
        public IEnumerable<Discounts> DiscountList;
        public IEnumerable<DiscountProducts> DiscountProductList;
        public IEnumerable<Bookings> BookingList;
        public IEnumerable<Surveys> SurveyList;
        public IEnumerable<ProductImages> ProductImageList;
        public IEnumerable<BlockedDatesCustomPrice> BlockedDatesCustomPriceList;
        //public IEnumerable<Invoices> InvoiceList;
        public IEnumerable<DiscountBookings> DiscountBookingList;
        public IEnumerable<Markets> MarketList;
        public IEnumerable<DefaultPrices> DefaultPriceList;
        public IEnumerable<ProductAddOns> ProductAddOnList;
        public IEnumerable<CustomerCredits> CustomerCreditList;
        public IEnumerable<CustomerCreditLogs> CustomerCreditLogList;

        public IEnumerable<GiftCards> GiftCardList;
        public IEnumerable<GiftCardBookings> GiftCardBookingList;

        public IEnumerable<BookingHistories> BookingHistoryList;
        public IEnumerable<CustomerInfoSearchCriteria> CustomerInfoSearchCriteriaList;

        public IEnumerable<RevShares> RevShareList;

        public IEnumerable<Subscriptions> SubscriptionsList;
        public IEnumerable<SubscriptionImages> SubscriptionImagesList;
        public IEnumerable<SubscriptionBookings> SubscriptionBookingsList;
        public IEnumerable<SubscriptionCycles> SubscriptionCyclesList;
        public IEnumerable<DiscountSubscriptions> DiscountSubscriptionList;
        public IEnumerable<SubscriptionBookingDiscounts> SubscriptionBookingDiscountsList;
        public IEnumerable<SubsciptionDiscountUseds> SubscriptionDiscountUsedList;
        public IEnumerable<Discounts> DiscountOfSubscriptionList;

        public IEnumerable<Policies> PoliciesList;
        public IEnumerable<HotelPolicies> HotelPoliciesList;

        public IEnumerable<Taxes> TaxList;

        protected DayaxeDataContext DbContext = new DayaxeDataContext(Constant.ConnectionString);

        protected DayaxeDataContext DayaxeDbContext
        {
            get
            {
                return DbContext;
            }
        }

        protected BaseRepository()
        {
            TaxList = GetAllTaxes();


            RevShareList = GetAllRevShares();
            PoliciesList = GetAllPolicies();
            HotelPoliciesList = GetAllHotelPolicies();

            PhotoList = GetAllPhotos();
            AmentyListsList = GetAllAmentyList();

            GiftCardList = GetAllGiftCards();
            GiftCardBookingList = GetAllGiftCardBookings();

            AmentiesList = GetAllAmenties();
            CustomerInfoList = GetAllCustomerInfos();
            CustomerInfoSearchCriteriaList = GetAllCustomerInfoSearchCriterias();
            CustomerCreditList = GetAllCustomerCredits();
            CustomerCreditLogList = GetAllCustomerCreditLogs();
            MarketHotelList = GetAllMarketHotels();
            //BlockedDateList = GetAllBlockedDates();
            ProductUpgradeList = GetAllProductUpgrades();
            UserHotelList = GetAllUserHotels();
            BookingList = GetAllBookings();
            DiscountProductList = GetAllDiscountProducts();
            DiscountBookingList = GetAllDiscountBooking();
            DiscountList = GetAllDiscounts();
            SurveyList = GetAllSurveys();
            BlockedDatesCustomPriceList = GetAllBlockedDatesCustomPrice();
            //InvoiceList = GetAllInvoice();
            MarketList = GetAllMarket();
            DefaultPriceList = GetAllDefaultPriceList();
            ProductAddOnList = GetAllProductAddOns();

            ProductImageList = GetAllProductImages();
            ProductList = GetAllProducts();
            HotelList = GetAllHotels();
            BookingHistoryList = GetAllBookingHistories();

            SubscriptionImagesList = GetAllSubscriptionImages();
            SubscriptionsList = GetAllSubscriptions();
            SubscriptionBookingsList = GetAllSubscriptionBookings();
            SubscriptionCyclesList = GetAllSubscriptionCycles();
            DiscountSubscriptionList = GetAllDiscountSubscriptions();
            SubscriptionBookingDiscountsList = GetAllSubscriptionBookingDiscounts();
            SubscriptionDiscountUsedList = GetAllSubscriptionDiscountUsed();
            DiscountOfSubscriptionList = GetAllSubscriptionDiscounts();
        }

        #region Public Function

        protected void Commit()
        {
            DayaxeDbContext.SubmitChanges();
        }

        public void AddSchedule(Schedules entity)
        {
            DayaxeDbContext.Schedules.InsertOnSubmit(entity);
            Commit();
        }

        public void AddBookingConfirm(BookingConfirmationHotels entity)
        {
            DayaxeDbContext.BookingConfirmationHotels.InsertOnSubmit(entity);
            Commit();
        }

        public IEnumerable<Schedules> GetAllSchedule()
        {
            return DayaxeDbContext.Schedules.Where(x => x.Status == (int)Enums.ScheduleType.NotRun && (x.SendAt == null || x.SendAt.Value < DateTime.UtcNow));
        }

        public void RefreshData()
        {
            DayaxeDbContext.Refresh(RefreshMode.OverwriteCurrentValues, DayaxeDbContext.Bookings);
            DayaxeDbContext.Refresh(RefreshMode.OverwriteCurrentValues, DayaxeDbContext.Hotels);
            DayaxeDbContext.Refresh(RefreshMode.OverwriteCurrentValues, DayaxeDbContext.Products);
            DayaxeDbContext.Refresh(RefreshMode.OverwriteCurrentValues, DayaxeDbContext.CustomerInfos);
            DayaxeDbContext.Refresh(RefreshMode.OverwriteCurrentValues, DayaxeDbContext.Schedules);
        }

        public void UpdateSchedule(Schedules entity)
        {
            var update = DayaxeDbContext.Schedules.FirstOrDefault(x => x.Id == entity.Id);
            if (update != null)
            {
                update.Status = entity.Status;
                update.CompleteDate = entity.CompleteDate;
                update.LastRun = entity.LastRun;
            }
            Commit();
        }

        public void AddLog(Logs entity)
        {
            DayaxeDbContext.Logs.InsertOnSubmit(entity);
            Commit();
        }

        public IEnumerable<Discounts> GetAutoPromosByProductId(int productId, DateTime? searchDate = null)
        {
            var hotels = (from h in HotelList
                          join p in ProductList on h.HotelId equals p.HotelId
                          where p.ProductId == productId
                          select h).FirstOrDefault();
            var destinationTimeZoneId = Constant.UsDefaultTime;
            if (hotels != null && !string.IsNullOrEmpty(hotels.TimeZoneId))
            {
                destinationTimeZoneId = hotels.TimeZoneId;
            }
            DateTime dateNow = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(destinationTimeZoneId);
            if (searchDate.HasValue)
            {
                dateNow = searchDate.Value;
            }
            if (dateNow.Date < DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(destinationTimeZoneId).Date)
            {
                dateNow = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(destinationTimeZoneId);
            }
            var discounts = DiscountList.Where(x => x.IsAllProducts && x.Status == Enums.DiscountStatus.Active && !x.CodeRequired && !x.IsDelete);

            var discountsProducts = (from p in DiscountList
                                     join p1 in DiscountProductList on p.Id equals p1.DiscountId
                                     where !p.IsDelete
                                         && !p.CodeRequired
                                         && (p1.ProductId == productId || p.IsAllProducts)
                                         && p.StartDate.HasValue
                                         && p.EndDate.HasValue
                                         && p.StartDate.Value.Date <= dateNow
                                         && p.EndDate.Value.Date >= dateNow
                                     select p);

            return discounts.Concat(discountsProducts).DistinctBy(x => x.Id);
        }

        public CustomerInfos GetCustomerInfoBySessionId(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                var logs = new Logs
                {
                    LogKey = "BaseRepository - GetCustomerInfoBySessionId",
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedContent = string.Format("sessionId is empty"),
                    UpdatedBy = 0
                };
                AddLog(logs);
            }

            //return CustomerInfoList.FirstOrDefault(x => (x.SessionId != null && x.SessionId.Equals(sessionId, StringComparison.OrdinalIgnoreCase))
            //    || (x.CreateAccountSessionId != null && x.CreateAccountSessionId.Equals(sessionId, StringComparison.OrdinalIgnoreCase)));

            return DayaxeDbContext.CustomerInfos.FirstOrDefault(x => (x.SessionId != null && x.SessionId.ToLower() == sessionId.ToLower())
                                                                        || (x.CreateAccountSessionId != null && x.CreateAccountSessionId.ToLower() == sessionId.ToLower()));
        }

        public void DeleteDefaultPrices(int productId, DateTime updatedDate)
        {
            var defaultPrices = DayaxeDbContext.DefaultPrices.Where(
                    x => x.ProductId == productId && updatedDate.Date == x.UpdatedDate.Date);
            if (defaultPrices.Any())
            {
                DayaxeDbContext.DefaultPrices.DeleteAllOnSubmit(defaultPrices);
            }
        }

        public Surveys GetSurvey(long bookingId)
        {
            return DayaxeDbContext.Surveys.FirstOrDefault(s => s.BookingId == bookingId);
        }

        public Products GetProduct(long id)
        {
            var product = DayaxeDbContext.Products.FirstOrDefault(x => x.ProductId == id);

            if (product != null)
            {
                string imageUrl = Constant.ImageDefault;
                var image = DayaxeDbContext.ProductImages.FirstOrDefault(x => x.ProductId == product.ProductId && x.IsCover && x.IsActive);
                if (image != null)
                {
                    imageUrl = image.Url;
                }
                try
                {
                    product.ImageUrl = string.Format("{0}", new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), imageUrl).AbsoluteUri);
                }
                catch (Exception)
                {
                    product.ImageUrl = imageUrl;
                }
            }

            return product;
        }

        public Hotels GetHotel(long id)
        {
            return DayaxeDbContext.Hotels.FirstOrDefault(x => x.HotelId == id);
        }

        public Markets GetMarketByHotelId(long id)
        {
            return (from m in DayaxeDbContext.Markets
                    join mh in DayaxeDbContext.MarketHotels on m.Id equals mh.MarketId
                    where mh.HotelId == id
                    select m).FirstOrDefault();
        }

        public CustomerInfos GetCustomer(long id)
        {
            return DayaxeDbContext.CustomerInfos.FirstOrDefault(x => x.CustomerId == id);
        }

        public CustomerInfos GetCustomerByEmailAddress(string email)
        {
            return DayaxeDbContext.CustomerInfos.FirstOrDefault(x => x.EmailAddress.ToUpper() == email.ToUpper());
        }

        public CustomerCredits GetCustomerCredits(long id)
        {
            return DayaxeDbContext.CustomerCredits.FirstOrDefault(x => x.CustomerId == id);
        }

        public CustomerCreditLogs GetCustomerCreditLogs(long id)
        {
            return DayaxeDbContext.CustomerCreditLogs.FirstOrDefault(x => x.Id == id);
        }

        public BookingConfirmationHotels GetBookingConfirmationHotels(int bookingId)
        {
            return DayaxeDbContext.BookingConfirmationHotels.FirstOrDefault(x => x.BookingId == bookingId);
        }

        public Bookings GetBookings(long id)
        {
            return DayaxeDbContext.Bookings.FirstOrDefault(b => b.BookingId == id);
        }

        public IEnumerable<Surveys> GetSurveyByHotelId(int hotelId)
        {
            return (from s in DayaxeDbContext.Surveys
                    join b in DayaxeDbContext.Bookings on s.BookingId equals b.BookingId
                    join p in DayaxeDbContext.Products on b.ProductId equals p.ProductId
                    join h in DayaxeDbContext.Hotels on p.HotelId equals h.HotelId
                    where h.HotelId == hotelId
                          && s.IsFinish
                    select s).ToList();
        }

        public BookingHistories GetBookingHistories(long id)
        {
            return DayaxeDbContext.BookingHistories.FirstOrDefault(b => b.Id == id);
        }

        public IEnumerable<Products> GetProductsAdOns(int productId, DateTime checkInDate)
        {
            List<int> upgradeIds = (from p in DayaxeDbContext.Products
                                    join p1 in DayaxeDbContext.ProductAddOns
                on p.ProductId equals p1.ProductId
                                    where p.ProductId == productId && !p.IsDelete && p.IsActive
                                    select p1.AddOnId).ToList();

            var products = (from p in DayaxeDbContext.Products where upgradeIds.Contains(p.ProductId) select p).ToList();

            if (products.Any())
            {
                var bookings = (from b in DayaxeDbContext.Bookings
                                join p in DayaxeDbContext.Products on b.ProductId equals p.ProductId
                                where p.HotelId == products.First().HotelId && b.PassStatus != (int)Enums.BookingStatus.Refunded
                                select b).ToList();

                products.ForEach(item =>
                {
                    item.ActualPriceWithDate = GetProductDailyPriceByDateDb(item, checkInDate);
                    item.BookingsToday = bookings
                        .Where(x => x.ProductId == item.ProductId &&
                                    x.CheckinDate.HasValue &&
                                    x.CheckinDate.Value.Date == checkInDate.Date).Sum(x => x.Quantity);

                    item.ImageAddOnUrl = GetProductAddOnUrl(item.ProductId);
                });

                switch (checkInDate.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        return products.Where(p => p.PassCapacityMon > p.BookingsToday);
                    case DayOfWeek.Tuesday:
                        return products.Where(p => p.PassCapacityTue > p.BookingsToday);
                    case DayOfWeek.Wednesday:
                        return products.Where(p => p.PassCapacityWed > p.BookingsToday);
                    case DayOfWeek.Thursday:
                        return products.Where(p => p.PassCapacityThu > p.BookingsToday);
                    case DayOfWeek.Friday:
                        return products.Where(p => p.PassCapacityFri > p.BookingsToday);
                    case DayOfWeek.Saturday:
                        return products.Where(p => p.PassCapacitySat > p.BookingsToday);
                    case DayOfWeek.Sunday:
                        return products.Where(p => p.PassCapacitySun > p.BookingsToday);
                }
            }

            return products;
        }

        public void AddEmailToProductWaittingLists(List<KeyValuePair<int, DateTime>> dates)
        {
            using (var transaction = new TransactionScope())
            {
                var products = DayaxeDbContext.ProductWaittingLists
                    .Where(p => p.CheckInDate > DateTime.UtcNow.ToLosAngerlesTime().Date)
                    .ToList();

                products = products.Where(x => dates.Contains(new KeyValuePair<int, DateTime>(x.ProductId, x.CheckInDate.Date)))
                .ToList();

                if (products.Any())
                {
                    products.ForEach(product =>
                    {
                        var schedules = new Schedules
                        {
                            Name = "Email Waiting List",
                            ScheduleSendType = (int)Enums.ScheduleSendType.IsEmailWaitingList,
                            Status = (int)Enums.ScheduleType.NotRun,
                            ProductWaitingListId = product.Id
                        };
                        DayaxeDbContext.Schedules.InsertOnSubmit(schedules);
                    });

                    Commit();
                }
                transaction.Complete();
            }
        }

        protected void AddEmailToProductWaitingLists(List<UpdateQuantityObject> updatedObjects, string destinationTimeZoneId)
        {
            using (var transaction = new TransactionScope())
            {
                var productIds = updatedObjects.Select(x => x.ProductId).Distinct().ToList();

                var waitingList = DayaxeDbContext.ProductWaittingLists
                    .Where(p => p.CheckInDate >= DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(destinationTimeZoneId).Date)
                    .ToList();

                waitingList = waitingList.Where(x => productIds.Contains(x.ProductId))
                    .ToList();

                if (waitingList.Any())
                {
                    waitingList.ForEach(item =>
                    {
                        // Changes Quantity but this day is blocked
                        var blockedDate = DayaxeDbContext.BlockedDatesCustomPrice.FirstOrDefault(x => x.Date.Date == item.CheckInDate && x.ProductId == item.ProductId);

                        if (blockedDate == null)
                        {
                            var schedules = new Schedules
                            {
                                Name = "Email Waiting List",
                                ScheduleSendType = (int)Enums.ScheduleSendType.IsEmailWaitingList,
                                Status = (int)Enums.ScheduleType.NotRun,
                                ProductWaitingListId = item.Id
                            };
                            DayaxeDbContext.Schedules.InsertOnSubmit(schedules);
                        }
                    });

                    Commit();
                }

                transaction.Complete();
            }
        }

        public ProductWaittingLists GetProductWaitingList(long id)
        {
            return DayaxeDbContext.ProductWaittingLists.FirstOrDefault(x => x.Id == id);
        }

        public List<string> GetBccEmailOfUser(int hotelId, int productType)
        {
            var hotel = DayaxeDbContext.Hotels.FirstOrDefault(h => h.HotelId == hotelId);
            List<string> email = new List<string>();
            if (hotel != null)
            {
                switch (productType)
                {
                    case (int)Enums.ProductType.DayPass:
                        if (!string.IsNullOrEmpty(hotel.BookingConfirmationEmail))
                        {
                            email = hotel.BookingConfirmationEmail.Split(';').ToList();
                        }
                        break;
                    case (int)Enums.ProductType.SpaPass:
                        if (!string.IsNullOrEmpty(hotel.BookingConfirmationSpaEmail))
                        {
                            email = hotel.BookingConfirmationSpaEmail.Split(';').ToList();
                        }
                        break;
                    default:
                        if (!string.IsNullOrEmpty(hotel.BookingConfirmationCabanaEmail))
                        {
                            email = hotel.BookingConfirmationCabanaEmail.Split(';').ToList();
                        }
                        break;
                }

            }
            return email
                .Where(e => e != null && e != string.Empty)
                .Distinct()
                .ToList();
        }

        public KeyValuePair<List<string>, List<string>> GetEmailAdminOfHotel(int hotelId)
        {
            var emailAddress = (from u in DayaxeDbContext.CustomerInfos
                                join uh in DayaxeDbContext.CustomerInfosHotels on u.CustomerId equals uh.CustomerId
                                where uh.HotelId == hotelId && !u.IsDelete
                                select u.EmailAddress).ToList();

            var toEmailList = emailAddress
                .Where(e => !string.IsNullOrEmpty(e) && Helper.IsValidEmail(e))
                .Distinct()
                .ToList();

            var emailAdmin = DayaxeDbContext.CustomerInfos
                .Where(u => !u.IsDelete && u.IsActive && u.IsSuperAdmin)
                .Select(u => u.EmailAddress)
                .ToList();

            var bccEmailList = emailAdmin
                .Where(e => !string.IsNullOrEmpty(e) && Helper.IsValidEmail(e))
                .Distinct()
                .ToList();

            bccEmailList = bccEmailList.Except(toEmailList).ToList();

            return new KeyValuePair<List<string>, List<string>>(toEmailList, bccEmailList);
        }

        public double GetDailyPriceByDate(Products products, DateTime date)
        {
            if (DayaxeDbContext.BlockedDatesCustomPrice.Any(bdcpchild => bdcpchild.ProductId == products.ProductId && bdcpchild.Date == date.Date))
            {
                return GetPriceByDate(products.ProductId, date);
            }

            return GetPriceByDate(products, date);
        }

        #endregion

        #region Subscription Region

        public SubscriptionBookings GetSubscriptionBookings(long id)
        {
            return DayaxeDbContext.SubscriptionBookings.FirstOrDefault(b => b.Id == id);
        }

        public SubscriptionCycles GetCurrentSubscriptionCycle(long subscriptionBookingId)
        {
            return DayaxeDbContext.SubscriptionCycles
                .Where(sc => sc.SubscriptionBookingId == subscriptionBookingId)
                .OrderByDescending(sc => sc.CycleNumber)
                .FirstOrDefault();
        }

        public Subscriptions GetSubscriptions(long id)
        {
            return DayaxeDbContext.Subscriptions.FirstOrDefault(b => b.Id == id);
        }

        public Discounts GetMembershipId(long bookingId)
        {
            return (from d in DayaxeDbContext.Discounts
                    join du in DayaxeDbContext.SubsciptionDiscountUseds on d.Id equals du.DiscountId
                    where du.SubscriptionBookingId == bookingId
                    select d).FirstOrDefault();
        }

        public Discounts GetDiscountsSubscriptionBookings(long bookingId)
        {
            return (from d in DayaxeDbContext.Discounts
                    join du in DayaxeDbContext.SubscriptionBookingDiscounts on d.Id equals du.DiscountId
                    where du.SubscriptionBookingId == bookingId
                    select d).FirstOrDefault();
        }

        public SubscriptionImages GetSubscriptionImageCover(long subscriptionId)
        {
            return DayaxeDbContext.SubscriptionImages.FirstOrDefault(x => x.SubscriptionId == subscriptionId && x.IsCover && x.IsActive);
        }

        #endregion

        #region GiftCardBookings Region

        public GiftCardBookings GetGiftCardBookings(long id)
        {
            return DayaxeDbContext.GiftCardBookings.FirstOrDefault(b => b.Id == id);
        }
        public GiftCards GetGiftCard(long id)
        {
            return DayaxeDbContext.GiftCards.FirstOrDefault(b => b.Id == id);
        }

        #endregion

        #region Private Function

        private double GetPriceByDate(long productId, DateTime date)
        {
            var customPrice =
                DayaxeDbContext.BlockedDatesCustomPrice.First(bdcpchild => bdcpchild.ProductId == productId &&
                                                               bdcpchild.Date.Date == date.Date.Date);
            return customPrice.RegularPrice;
        }

        private double GetPriceByDate(Products product, DateTime date)
        {
            var defaultPrice = DayaxeDbContext.DefaultPrices.Where(d => d.EffectiveDate.Date < date.Date && d.ProductId == product.ProductId)
                .OrderByDescending(d => d.EffectiveDate).FirstOrDefault();
            if (defaultPrice != null)
            {
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        return defaultPrice.PriceMon;
                    case DayOfWeek.Tuesday:
                        return defaultPrice.PriceTue;
                    case DayOfWeek.Wednesday:
                        return defaultPrice.PriceWed;
                    case DayOfWeek.Thursday:
                        return defaultPrice.PriceThu;
                    case DayOfWeek.Friday:
                        return defaultPrice.PriceFri;
                    case DayOfWeek.Saturday:
                        return defaultPrice.PriceSat;
                    default:
                        return defaultPrice.PriceSun;
                }
            }
            else
            {
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        return product.PriceMon;
                    case DayOfWeek.Tuesday:
                        return product.PriceTue;
                    case DayOfWeek.Wednesday:
                        return product.PriceWed;
                    case DayOfWeek.Thursday:
                        return product.PriceThu;
                    case DayOfWeek.Friday:
                        return product.PriceFri;
                    case DayOfWeek.Saturday:
                        return product.PriceSat;
                    default:
                        return product.PriceSun;
                }
            }
        }

        private string GetProductAddOnUrl(int productId)
        {
            string imageUrl = "/images/default-thumb.png";
            var image = DayaxeDbContext.ProductImages.FirstOrDefault(x => x.ProductId == productId && x.IsCover && x.IsActive);
            if (image != null)
            {
                imageUrl = image.Url;
            }
            try
            {
                return string.Format("{0}", new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), imageUrl).AbsoluteUri);
            }
            catch (Exception)
            {
                return imageUrl;
            }
        }

        #endregion

        public void ResetCache()
        {
            Helper.ResetClientCache();
            CacheLayer.ClearAll();
        }

        private bool _isDisposed;
        public void Free()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Object Name");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~BaseRepository()
        {
            //Pass false as param because no need to free managed resources when you call finalize it
            //by GC itself as its work of finalize to manage managed resources.
            Dispose(false);
        }

        //Implement dispose to free resources
        protected virtual void Dispose(bool disposedStatus)
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                DbContext.Dispose(); // Released unmanaged Resources
                if (disposedStatus)
                {
                    // Released managed Resources
                }
            }
        }
    }
}
