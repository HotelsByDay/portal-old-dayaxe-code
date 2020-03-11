using DayaxeDal;
using DayaxeDal.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace dayaxe.com
{
    public partial class Default2 : BasePage
    {
        private readonly BookingRepository _bookingRepository = new BookingRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            var result = new List<DistanceObject>();
            string url = "http://maps.googleapis.com/maps/api/distancematrix/json?origins={0}&destinations={1}&mode=driving&language=en-EN&sensor=false%22";
            var bookings = _bookingRepository.BookingList.Where(b => b.RedeemedDate.HasValue).ToList();
            bookings.ForEach(booking =>
            {
                var customer =
                    _bookingRepository.CustomerInfoList.FirstOrDefault(c => c.CustomerId == booking.CustomerId);
                var hotels = (from h in _bookingRepository.HotelList
                              join p in _bookingRepository.ProductList on h.HotelId equals p.HotelId
                              where p.ProductId == booking.ProductId
                              select h).FirstOrDefault();
                if (customer != null && !string.IsNullOrEmpty(customer.ZipCode) && hotels != null)
                {
                    var requestResult = Helper.Get(string.Format(url, customer.ZipCode, hotels.ZipCode));
                    var reg = new Regex("\"text\"\\s\x3A\\s\"(?<Km>[0-9\\.\\,]+)\\skm\"", RegexOptions.Multiline);
                    double km;
                    if (double.TryParse(reg.Match(requestResult).Groups["Km"].Value, out km))
                    {
                        double miles = km / Helper.Miles;

                        _bookingRepository.AddDistance(new Distances
                        {
                            BookingId = booking.BookingId,
                            EmailAddress = customer.EmailAddress,
                            UserZipcode = customer.ZipCode,
                            HotelZipcode = hotels.ZipCode,
                            JsonResult = requestResult,
                            DistanceKm = km,
                            DistanceMiles = miles
                        });
                    }
                }
            });

            DistanceRpt.DataSource = result;
            DistanceRpt.DataBind();
        }
    }

    public class DistanceObject
    {
        public long BookingId { get; set; }

        public string EmailAddress { get; set; }

        public string UserZipcode { get; set; }

        public string HotelZipcode { get; set; }

        public string Distance { get; set; }
    }
}