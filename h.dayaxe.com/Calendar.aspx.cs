using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoMapper;
using DayaxeDal;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;
using Newtonsoft.Json;

namespace h.dayaxe.com
{
    public partial class Calendar : BasePageProduct
    {
        protected static int SelectedMonth { get; set; }

        protected static string SelectedYear { get; set; }
        private static BookingRepository _bookingRepository = new BookingRepository();
        private static ProductRepository _productRepository = new ProductRepository();
        private IEnumerable<Products> _products;

        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "Calendar";

            if (!IsPostBack)
            {
                _bookingRepository = new BookingRepository();
                _productRepository = new ProductRepository();
                Session["CurrentMonth_Calendar"] = DateTime.Now;

                SelectedMonth = DateTime.Parse(Session["CurrentMonth_Calendar"].ToString()).Month - 1;
                SelectedYear = DateTime.Parse(Session["CurrentMonth_Calendar"].ToString()).ToString("yyyy");

                _products = (from h in _bookingRepository.HotelList
                    join p in _bookingRepository.ProductList on h.HotelId equals p.HotelId
                    where h.IsActive && !h.IsDelete &&
                          p.IsActive && !p.IsDelete &&
                          h.HotelId == PublicHotel.HotelId
                    select p);
                BindProductPrice();
            }

            BindListProduct();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            MessageLabel.Text = string.Empty;
        }

        protected void saveCalendar_OnClick(object sender, EventArgs e)
        {
            CacheLayer.Clear(CacheKeys.BookingsCacheKey);
            _bookingRepository = new BookingRepository();

            MessageLabel.Visible = false;
            var start = DateTime.ParseExact(startDate.Value, "MM/dd/yyyy", null);
            var end = DateTime.ParseExact(endDate.Value, "MM/dd/yyyy", null);

            var totalDate = end - start;
            var dateAdd = start.GetDates(totalDate.Days);
            bool availableChange = true;

            var allBookings = _bookingRepository.GetAllBookingsActiveOfHotel(PublicHotel.HotelId, true).ToList();

            var productsWithPrices = new List<SaveCalendarObject>();
            foreach (RepeaterItem item in ProductPriceRpt.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var regularPriceControl = (TextBox) item.FindControl("RegularPriceText");
                    var capacityDropdownList = (DropDownList) item.FindControl("CapacityDropdownList");
                    var productId = (HiddenField) item.FindControl("HidProductId");

                    double rPrice;
                    int quantity;
                    double.TryParse(regularPriceControl.Text, out rPrice);
                    int.TryParse(capacityDropdownList.SelectedValue, out quantity);

                    productsWithPrices.Add(new SaveCalendarObject
                    {
                        ProductId = int.Parse(productId.Value),
                        Price = string.IsNullOrEmpty(regularPriceControl.Text) ? 0 : rPrice,
                        Quantity = quantity
                    });
                }
            }

            dateAdd.ForEach(date =>
            {
                productsWithPrices.ForEach(saveObject =>
                {
                    var bookingWithDate = allBookings
                        .Where(b =>
                            b.CheckinDate.HasValue &&
                            b.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(PublicHotel.TimeZoneId).Date == date.Date &&
                            b.ProductId == saveObject.ProductId)
                        .ToList();

                    if (bookingWithDate.Any())
                    {
                        var totalBookingsWithDate = bookingWithDate.Sum(b => b.Quantity);
                        if (saveObject.Quantity < totalBookingsWithDate)
                        {
                            availableChange = false;
                        }
                    }});
            });
            
            if (!availableChange)
            {
                _bookingRepository.ResetCache();
                MessageLabel.Visible = true;
                MessageLabel.Text = ErrorMessage.BookingsExists;
                MessageLabel.ForeColor = Color.Red;
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "AfterUpdate",
                    "setTimeout(function(){$(function(){$('.saving').addClass('hidden');});}, 100);", true);
                return;
            }

            _bookingRepository.SaveCalendar(productsWithPrices, dateAdd);
            CacheLayer.Clear(CacheKeys.BlockedDatesCustomPricesCacheKey);

            var dateWithQuantityZero = productsWithPrices
                .Where(o => o.Quantity == 0)
                .GroupBy(p => p.ProductId,
                p => p,
                (key, g) => new
                {
                    ProductId = key,
                    Count = g.Count()
                }).ToList();
            if (dateWithQuantityZero.Any())
            {
                _bookingRepository = new BookingRepository();
                var bookingsNeedUpdate = new List<Bookings>();

                dateWithQuantityZero.ForEach(product =>
                {
                    var products = _productRepository.GetById(product.ProductId);
                    var bookings = allBookings.Where(b => b.ProductId == product.ProductId).ToList();
                    var blackoutdays = _bookingRepository.BlockedDatesCustomPriceList
                        .Where(bdcp => bdcp.ProductId == product.ProductId && bdcp.Capacity == 0)
                        .ToList();

                    if (products.IsCheckedInRequired)
                    {

                        allBookings = allBookings.Except(bookings).ToList();
                    }
                    else
                    {
                        bookings.ForEach(booking =>
                        {
                            var expiredDate = booking.BookedDate.AddDays(products.RedemptionPeriod);
                            var blackoutOnExpire = blackoutdays.Count(b => b.Date.Date <= expiredDate.Date);

                            // Recursive add when contains enough Open Days
                            while (blackoutOnExpire > 0)
                            {
                                expiredDate = expiredDate.AddDays(blackoutOnExpire);
                                blackoutOnExpire =
                                    blackoutdays.Count(
                                        b =>
                                            b.Date.Date > expiredDate.Date &&
                                            b.Date.Date < expiredDate.AddDays(blackoutOnExpire).Date);
                            }
                            booking.ExpiredDate = expiredDate.AddDays(blackoutOnExpire);
                        });
                        bookingsNeedUpdate.AddRange(bookings);
                    }

                });
                
                _bookingRepository.UpdateStatus(bookingsNeedUpdate);
            }

            _bookingRepository.ResetCache();

            MessageLabel.Visible = true;
            MessageLabel.Text = ErrorMessage.UpdateSuccess;
            MessageLabel.ForeColor = Color.DarkGreen;
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "BindCalendar",
                "setTimeout(function(){$(function(){$('.btn-cancel').click(); $('.saving').addClass('hidden');$('.product-list-calendar li a.active').click();});}, 100);", true);
        }

        #region Private Function 

        private void BindProductPrice()
        {
            ProductPriceRpt.DataSource = _products;
            ProductPriceRpt.DataBind();
        }

        [WebMethod]
        public static string GetCalendarOfProduct(int productId)
        {
            _bookingRepository = new BookingRepository();
            var customPrice = _bookingRepository.BlockedDatesCustomPriceList
                .Where(x => x.ProductId == productId)
                .ToList();

            var dateDisabled = customPrice
                .Where(cp => !cp.Capacity.HasValue || cp.Capacity == 0)
                .ToList();

            customPrice = customPrice.Except(dateDisabled).ToList();

            var defaultPrices = (from dp in _bookingRepository.DefaultPriceList
                join p in _bookingRepository.ProductList on dp.ProductId equals p.ProductId
                join h in _bookingRepository.HotelList on p.HotelId equals h.HotelId
                where p.ProductId == productId &&
                      h.IsActive && !h.IsDelete &&
                      p.IsActive && !p.IsDelete
                orderby dp.EffectiveDate descending 
                select dp).ToList();

            var defaultP = Mapper.Map<List<DefaultPrices>, List<DefaultPriceObject>>(defaultPrices);

            var booksHotel = _bookingRepository.GetAllBookingsOfProduct(productId)
                .Where(b => b.PassStatus != (int)Enums.BookingStatus.Refunded)
                .ToList();

            var hotels = (from h in _bookingRepository.HotelList
                          join p in _bookingRepository.ProductList on h.HotelId equals p.HotelId
                          where p.ProductId == productId
                          select h).FirstOrDefault();

            var bookData = (from b in booksHotel
                            join p in _bookingRepository.ProductList on b.ProductId equals p.ProductId
                            let checkinDate = b.CheckinDate
                            where checkinDate != null
                            where b.CheckinDate.HasValue && p.ProductId == productId
                            group b by
                             new { b.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(hotels != null ? hotels.TimeZoneId : Constant.DefaultLosAngelesTimezoneId).Date, p.ProductId, p.ProductNameAcronym }
                                        into g
                            select new
                            {
                                BookedDate = g.Key.Date.ToString(Constant.DiscountDateFormat),
                                //g.Key.ProductId,
                                //g.Key.ProductNameAcronym,
                                CountBook = g.Sum(b => b.Quantity)
                            }).ToList();
            var redemptionData = (from b in booksHotel
                                  join p in _bookingRepository.ProductList on b.ProductId equals p.ProductId
                                  let redeemedDate = b.RedeemedDate
                                  where redeemedDate != null
                                  where redeemedDate.HasValue && b.PassStatus == (int)Enums.BookingStatus.Redeemed && p.ProductId == productId
                                  group b by
                                   new { redeemedDate.Value.ToLosAngerlesTimeWithTimeZone(hotels != null ? hotels.TimeZoneId : Constant.DefaultLosAngelesTimezoneId).Date, p.ProductId, p.ProductNameAcronym }
                                        into g
                                  select new
                                  {
                                      RedeemedDate = g.Key.Date.ToString(Constant.DiscountDateFormat),
                                      //g.Key.ProductId,
                                      //g.Key.ProductNameAcronym,
                                      CountBook = g.Sum(x => x.Quantity)
                                  }).ToList();
            
            return JsonConvert.SerializeObject(new
            {
                CustomPrice = customPrice.Select(cp => new { cp.Id, cp.Capacity, cp.Date, cp.RegularPrice }),
                DefaultPrice = defaultP,
                BookData = bookData,
                RedemptionData = redemptionData,
                DateDisabled = string.Format("[{0}]", string.Join(",", dateDisabled.Select(d => string.Format("\"{0:MM/dd/yyyy}\"", d.Date)).ToList()))
            }, CustomSettings.SerializerSettings());
        }

        [WebMethod]
        public static string GetPriceOfHotelByDate(DateTime date, int hotelId)
        {
            _productRepository = new ProductRepository();
            var objReturn = new List<SaveCalendarObject>();

            var products = (from p in _productRepository.ProductList
                join h in _productRepository.HotelList on p.HotelId equals h.HotelId
                where h.HotelId == hotelId && !p.IsDelete
                select p).ToList();

            products.ForEach(product =>
            {
                var item = _productRepository.GetById(product.ProductId, date);
                objReturn.Add(new SaveCalendarObject
                {
                    ProductId = item.ProductId,
                    Price = item.ActualPriceWithDate.Price,
                    Quantity = item.ActualPriceWithDate.Capacity
                });
            });

            return JsonConvert.SerializeObject(objReturn, CustomSettings.SerializerSettings());
        }

        private void BindListProduct()
        {
            ProductRpt.DataSource = _products;
            ProductRpt.DataBind();
        }

        #endregion

        protected void ProductPriceRpt_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var regularPrice = (TextBox)e.Item.FindControl("RegularPriceText");
                var quantityControl = (DropDownList)e.Item.FindControl("CapacityDropdownList");
                var productId = (HiddenField)e.Item.FindControl("HidProductId");

                var products = (Products)e.Item.DataItem;
                
                quantityControl.CssClass += string.Format(" product-{0}", products.ProductId);
                regularPrice.CssClass += string.Format(" regular-price-{0}", products.ProductId);

                var list = Enumerable.Range(0, 71).ToList();
                quantityControl.DataSource = list;
                quantityControl.DataBind();
                quantityControl.Items.Insert(0, new ListItem("Default", "-1"));

                productId.Value = products.ProductId.ToString();
                regularPrice.Attributes["pid"] = products.ProductId.ToString();
            }
        }

        protected void ProductRpt_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            
        }
    }
}