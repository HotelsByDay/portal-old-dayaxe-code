using System.Collections.Generic;
using System.Linq;
using DayaxeDal.Extensions;
using Newtonsoft.Json;

namespace DayaxeDal
{
    public class SearchDataResponse
    {
        private static string BookingDetailUrl = "/BookingDetails.aspx?id={0}";
        private static string CustomerDetailUrl = "/CustomerDetails.aspx?id={0}";

        [JsonIgnore]
        public List<SearchDataObject> Data
        {
            get
            {
                return ListBookingsData.Concat(ListCustomerInfosData).OrderBy(x => x.Description).ToList();
            }
        }

        public List<Bookings> ListBookings { get; set; }

        [JsonIgnore]
        public List<SearchDataObject> ListBookingsData
        {
            get
            {
                return ListBookings.Select(bookings => new SearchDataObject
                {
                    Description = string.Format("{0} - {1} - {2} - {3} - {4} - {5} - {6} - {7} - {8} - {9} - {10}",
                        bookings.BookingId,
                        bookings.BookingIdString,
                        bookings.CustomerInfos.EmailAddress,
                        bookings.CustomerInfos.FirstName,
                        bookings.CustomerInfos.LastName,
                        bookings.BookingsTypeString,
                        bookings.Products.Hotels.HotelName,
                        bookings.Products.ProductName,
                        bookings.CheckinDate.HasValue ? bookings.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(bookings.TimeZoneId).ToString(Constant.DiscountDateFormat) : string.Empty,
                        bookings.BookedDate.ToLosAngerlesTimeWithTimeZone(bookings.TimeZoneId).ToString(Constant.DiscountDateFormat),
                        bookings.BookingStatusString),
                    EditUrl = string.Format(BookingDetailUrl, bookings.BookingId),
                    Id = bookings.BookingId
                }).ToList();
            }
        }

        public List<CustomerInfos> ListCustomerInfos { get; set; }

        [JsonIgnore]
        public List<SearchDataObject> ListCustomerInfosData
        {
            get
            {
                return ListCustomerInfos.Select(customers => new SearchDataObject
                {
                    Description = string.Format("{0} - {1} - {2} {3}",
                        customers.CustomerId,
                        customers.EmailAddress + (!string.IsNullOrEmpty(customers.SubscriptionCode) ? " - " + customers.SubscriptionCode : string.Empty),
                        customers.FirstName,
                        customers.LastName),
                    EditUrl = string.Format(CustomerDetailUrl, customers.CustomerId),
                    Id = customers.CustomerId
                }).ToList();
            }
        }

        public int TotalRecords
        {
            get { return ListBookings.Count + ListCustomerInfos.Count; }
        }

        public int TotalBookingsRecord
        {
            get { return ListBookings.Count; }
        }
        public int TotalCustomersRecord
        {
            get { return ListCustomerInfos.Count; }
        }
    }
}
