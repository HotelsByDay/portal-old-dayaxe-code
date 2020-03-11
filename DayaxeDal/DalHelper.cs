using System;
using System.Collections.Generic;
using System.Linq;

namespace DayaxeDal
{
    public class DalHelper: BaseRepository
    {
        #region RequestAccess

        public int AddAccessRequest(AccessRequests request)
        {
            DayaxeDbContext.AccessRequests.InsertOnSubmit(request);

            Commit();

            return request.RequestId;
        }

        #endregion

        #region BookingHistories

        public void AddBookingHistories(BookingHistories bookingHistories)
        {
            DayaxeDbContext.BookingHistories.InsertOnSubmit(bookingHistories);

            Commit();
        }

        #endregion

        #region Content pages

        public IEnumerable<SiteMaps> GetAllSiteMaps()
        {
            return DayaxeDbContext.SiteMaps;
        }

        public SiteMaps GetSiteMapsById(int id)
        {
            return DayaxeDbContext.SiteMaps.SingleOrDefault(x => x.Id == id);
        }

        public HtmlContents GetHtmlContentsBySiteMapsId(int id)
        {
            return DayaxeDbContext.HtmlContents.SingleOrDefault(x => x.SiteMapId == id);
        }

        public int AddSiteMaps(SiteMaps siteMaps, HtmlContents contents)
        {
            var siteMap = DayaxeDbContext.SiteMaps.FirstOrDefault(x => x.UrlSegment == siteMaps.UrlSegment);
            if (siteMap != null)
            {
                throw new Exception("Url has exists, please try another Url");
            }

            if (siteMaps.IsHomePage.HasValue && siteMaps.IsHomePage.Value)
            {
                var homePage = DayaxeDbContext.SiteMaps.FirstOrDefault(x => x.IsHomePage.HasValue && x.IsHomePage.Value);
                if (homePage != null)
                {
                    homePage.IsHomePage = false;
                }
            }

            DayaxeDbContext.SiteMaps.InsertOnSubmit(siteMaps);

            Commit();

            contents.SiteMapId = siteMaps.Id;
            DayaxeDbContext.HtmlContents.InsertOnSubmit(contents);

            Commit();

            return siteMaps.Id;
        }

        public void UpdateSiteMaps(SiteMaps siteMaps, HtmlContents contents)
        {
            var siteMap = DayaxeDbContext.SiteMaps.FirstOrDefault(x => x.Id == siteMaps.Id);
            if (siteMap != null)
            {
                if (siteMaps.IsHomePage.HasValue && siteMaps.IsHomePage.Value)
                {
                    var homePage = DayaxeDbContext.SiteMaps.FirstOrDefault(x => x.IsHomePage.HasValue && x.IsHomePage.Value);
                    if (homePage != null)
                    {
                        homePage.IsHomePage = false;
                    }
                }

                siteMap.Name = siteMaps.Name;
                siteMap.UrlSegment = siteMaps.UrlSegment;
                siteMap.IsHomePage = siteMaps.IsHomePage;
                siteMap.IsActive = siteMaps.IsActive;

                var htmlContents = DayaxeDbContext.HtmlContents.FirstOrDefault(x => x.ContentId == contents.ContentId);
                if (htmlContents != null)
                {
                    htmlContents.Title = contents.Title;
                    htmlContents.Data = contents.Data;
                    htmlContents.ScriptAnalyticsHeader = contents.ScriptAnalyticsHeader;
                    htmlContents.BodyClass = contents.BodyClass;
                    htmlContents.MetaDescription = contents.MetaDescription;
                    htmlContents.MetaKeyword = contents.MetaKeyword;
                    htmlContents.ImageLandingDesktop = contents.ImageLandingDesktop;
                    htmlContents.ImageLandingMobile = contents.ImageLandingMobile;
                }

                Commit();
            }
        }

        public HtmlContents GetHtmlContentsByUrlSegment(string urlSegment)
        {
            var content = (from p in DayaxeDbContext.SiteMaps
                join p1 in DayaxeDbContext.HtmlContents
                    on p.Id equals p1.SiteMapId
                where p.UrlSegment == urlSegment.Replace(" ", "") && p.IsActive
                select p1);
            return content.FirstOrDefault();
        }

        public HtmlContents GetDefaultHtmlContents()
        {
            var content = (from p in DayaxeDbContext.SiteMaps
                           join p1 in DayaxeDbContext.HtmlContents
                               on p.Id equals p1.SiteMapId
                           where p.IsHomePage.HasValue && p.IsHomePage.Value && p.IsActive
                           select p1);
            return content.FirstOrDefault();
        }

        #endregion

        #region BookingConfirmationHotel

        public BookingConfirmationHotels GetBookingConfirmationHotelsByBookingId(int bookingId)
        {
            return DayaxeDbContext.BookingConfirmationHotels.FirstOrDefault(x => x.BookingId == bookingId);
        }

        public void AddBookingConfirmationHotel(BookingConfirmationHotels bookingConfirmationHotels)
        {
            DayaxeDbContext.BookingConfirmationHotels.InsertOnSubmit(bookingConfirmationHotels);

            Commit();
        }

        #endregion BookingConfirmationHotel

        #region SearchDataResponse

        public SearchDataResponse SearchData()
        {
            var result = new SearchDataResponse
            {
                ListBookings = DayaxeDbContext.Bookings.OrderByDescending(x => x.BookingId).ToList(),
                ListCustomerInfos = CustomerInfoList.Where(c => !c.IsDelete).ToList()
            };

            result.ListBookings.ForEach(booking =>
            {
                var product = ProductList.FirstOrDefault(p => p.ProductId == booking.ProductId);
                var hotel = HotelList.FirstOrDefault(h => h.HotelId == (product != null ? product.HotelId : 0));

                booking.BookingsTypeString = product != null ? GetProductType(product.ProductType) : string.Empty;
                booking.TimeZoneId = hotel != null ? hotel.TimeZoneId : string.Empty;
            });

            result.ListCustomerInfos.ForEach(customer =>
            {
                var discount = GetSubscriptionDiscount(customer.CustomerId);
                if (discount != null)
                {
                    customer.SubscriptionCode = discount.Code;
                }
            });

            return result;
        }

        public SearchDataResponse SearchDataByCustomerId(int customerId)
        {
            var result = new SearchDataResponse
            {
                ListBookings = BookingList.Where(b => b.CustomerId == customerId).OrderByDescending(x => x.BookingId).ToList(),
                ListCustomerInfos = new List<CustomerInfos>()
            };

            result.ListBookings.ForEach(booking =>
            {
                var product = ProductList.FirstOrDefault(p => p.ProductId == booking.ProductId);
                booking.BookingsTypeString = product != null ? GetProductType(product.ProductType) : string.Empty;
            });

            return result;
        }

        public string GetProductType(int productType)
        {
            switch (productType)
            {
                case (int)Enums.ProductType.Cabana:
                    return Constant.CabanasPassString;
                case (int)Enums.ProductType.Daybed:
                    return Constant.DaybedsString;
                case (int)Enums.ProductType.SpaPass:
                    return Constant.SpaPassString;
                default:
                    return Constant.DayPassString;
            }
        }

        private Discounts GetSubscriptionDiscount(int customerId)
        {
            return (from d in DiscountOfSubscriptionList
                join sdu in SubscriptionDiscountUsedList on d.Id equals sdu.DiscountId
                where sdu.CustomerId == customerId
                      && d.PromoType == (byte)Enums.PromoType.SubscriptionPromo
                      && d.Status == Enums.DiscountStatus.Active
                select d).FirstOrDefault();
        }

        #endregion SearchDataResponse

        #region CustomerInfoLogs

        public int AddCustomerInfoLogs(CustomerInfoLogs entity)
        {
            DayaxeDbContext.CustomerInfoLogs.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        #endregion

        #region Newsletter

        public long AddNewsletterEmail(Newsletters entity)
        {
            DayaxeDbContext.Newsletters.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        #endregion
    }
}
