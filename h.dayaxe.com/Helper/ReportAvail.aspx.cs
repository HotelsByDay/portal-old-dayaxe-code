using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using CsvHelper;
using DayaxeDal;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;

namespace h.dayaxe.com.Helper
{
    public partial class HelperReportAvail : Page
    {
        private readonly BookingRepository _bookingRepository = new BookingRepository();
        private List<Hotels> _hotels;
        private List<Products> _products;

        protected void Page_Init(object sender, EventArgs e)
        {
            _hotels = _bookingRepository.HotelList.Where(h => !h.IsDelete && h.IsActive).ToList();
            RptHotelList.DataSource = _bookingRepository.HotelList.Where(h => !h.IsDelete && h.IsActive);
            RptHotelList.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in RptHotelList.Items)
            {
                var hotelSelected = (CheckBox) item.FindControl("HotelChecked");
                var hidHotelId = (HiddenField) item.FindControl("HidHotelId");
                if (!hotelSelected.Checked)
                {
                    var hotel = _hotels.FirstOrDefault(x => x.HotelId == int.Parse(hidHotelId.Value));
                    if (hotel != null)
                    {
                        _hotels.Remove(hotel);
                    }
                }
            }
        }

        protected void Export_OnClick(object sender, EventArgs e)
        {
            DateTime startDate;
            DateTime endDate;
            if (string.IsNullOrEmpty(DateFrom.Text) || !DateTime.TryParseExact(DateFrom.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out startDate))
            {
                MessageLabel.Text = "Please enter From and To date.";
                return;
            }
            if (string.IsNullOrEmpty(DateTo.Text) || !DateTime.TryParseExact(DateTo.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out endDate))
            {
                MessageLabel.Text = "Please enter From and To date.";
                return;
            }
            _products = _bookingRepository.ProductList.ToList();

            var objects = new List<ReportAvailObject>();

            for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                _hotels.ForEach(hotel =>
                {
                    var bookings = (from b in _bookingRepository.BookingList
                        join p in _bookingRepository.ProductList on b.ProductId equals p.ProductId
                        let checkInDate = b.CheckinDate
                        where !p.IsDelete && p.IsActive && checkInDate.HasValue &&
                              checkInDate.Value.ToLosAngerlesTimeWithTimeZone(hotel.TimeZoneId).Date == date.Date &&
                              b.PassStatus != (int)Enums.BookingStatus.Refunded &&
                              p.HotelId == hotel.HotelId
                        select b);

                    FilterBookingsByProductType(ref bookings);

                    objects.Add(new ReportAvailObject
                    {
                        HotelId = hotel.HotelId,
                        HotelName = hotel.HotelName,
                        Available = GetCapacity(hotel.HotelId, date),
                        Sold = bookings.Sum(x => x.Quantity),
                        Date = date
                    });
                });
            }

            var result = objects.GroupBy(p => p.Date,
                p => p,
                (key, g) => new
                {
                    Date = key.ToString("MMM dd"),
                    DateObj = key,
                    ListObj = g.ToList()
                }).ToList();

            DataTable dataTable = new DataTable();
            // Add Column to Datatable
            dataTable.Columns.Add("Sell Through", typeof(string));
            dataTable.Columns.Add("Total", typeof(string));
            result.ForEach(item =>
            {
                dataTable.Columns.Add(item.Date, typeof(string));
            });

            var totalAvailRow = dataTable.NewRow();
            var totalSoldRow = dataTable.NewRow();
            var percentSellThrough = dataTable.NewRow();
            totalAvailRow[0] = "Total Available";
            totalSoldRow[0] = "Total Sold";
            percentSellThrough[0] = "% sell through";

            // First Row
            int idx = 2;
            var dateRow = dataTable.NewRow();
            result.ForEach(item =>
            {
                var culture = new CultureInfo("en-US");
                var day = culture.DateTimeFormat.GetDayName(item.DateObj.DayOfWeek);
                dateRow[idx] = day;
                idx++;
            });
            dataTable.Rows.Add(dateRow);

            // Content
            _hotels.ForEach(hotel =>
            {
                var rowHotelName = dataTable.NewRow();
                rowHotelName[0] = hotel.HotelName;

                var rowAvailable = dataTable.NewRow();
                var rowSold = dataTable.NewRow();
                var totalAvailable = 0;
                var totalSold = 0;
                idx = 2;
                result.ForEach(item =>
                {
                    var listObj = item.ListObj.Where(c => c.HotelId == hotel.HotelId).ToList();
                    totalAvailable += listObj.Sum(x => x.Available);
                    totalSold += listObj.Sum(x => x.Sold);
                    rowAvailable[idx] = listObj.Sum(x => x.Available);
                    rowSold[idx] = listObj.Sum(x => x.Sold);

                    totalAvailRow[idx] = int.Parse("0" + totalAvailRow[idx]) + listObj.Sum(x => x.Available);
                    totalSoldRow[idx] = int.Parse("0" + totalSoldRow[idx]) + listObj.Sum(x => x.Sold);
                    idx++;
                });

                rowAvailable[0] = "Sell Through";
                rowAvailable[1] = totalAvailable;

                rowSold[0] = totalAvailable > 0 ? (totalSold * 100 / totalAvailable).ToString("#.##") + "%" : "";
                rowSold[1] = totalSold;

                dataTable.Rows.Add(rowHotelName);
                dataTable.Rows.Add(rowAvailable);
                dataTable.Rows.Add(rowSold);
            });

            // Footer
            idx = 2;
            result.ForEach(item =>
            {
                percentSellThrough[idx] = int.Parse("0" + totalAvailRow[idx]) > 0 
                    ? (int.Parse("0" + totalSoldRow[idx]) * 100 / int.Parse("0" + totalAvailRow[idx])).ToString("#.##") + "%" 
                    : "%";
                idx++;
            });

            dataTable.Rows.Add(totalAvailRow);
            dataTable.Rows.Add(totalSoldRow);
            dataTable.Rows.Add(percentSellThrough);

            var fileName = string.Format("ReportAvailable_{0:yyyyMMdd}_{1:yyyyMMdd}_{2:yyyyMMddhhmm}.csv",
                startDate, endDate, DateTime.Now);
            using (TextWriter textWriter = File.CreateText(Server.MapPath(string.Format("/Helper/{0}", fileName))))
            {
                var csv = new CsvWriter(textWriter);
                csv.Configuration.Encoding = Encoding.UTF8;
                csv.Configuration.Delimiter = ",";
                csv.Configuration.Quote = '"';
                csv.Configuration.QuoteAllFields = true;
                csv.Configuration.HasHeaderRecord = true;

                // Write columns
                foreach (DataColumn column in dataTable.Columns)
                {
                    csv.WriteField(column.ColumnName);
                }
                csv.NextRecord();

                // Write row values
                foreach (DataRow row in dataTable.Rows)
                {
                    for (var i = 0; i < dataTable.Columns.Count; i++)
                    {
                        csv.WriteField(row[i]);
                    }
                    csv.NextRecord();
                }
            }

            Response.ContentType = "Application/vnd.ms-excel";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.TransmitFile(Server.MapPath("~/Helper/" + fileName));
            Response.End();
        }

        private void FilterBookingsByProductType(ref IEnumerable<Bookings> bookings)
        {
            switch (ProductTypeDdl.SelectedValue)
            {
                case "DayPasses":
                    bookings = bookings.Where(b => _products.Count(p => p.ProductId == b.ProductId && p.ProductType == (int)Enums.ProductType.DayPass) > 0);
                    break;
                case "Cabanas":
                    bookings = bookings.Where(b => _products.Count(p => p.ProductId == b.ProductId && p.ProductType == (int)Enums.ProductType.Cabana) > 0);
                    break;
                case "Daybeds":
                    bookings = bookings.Where(b => _products.Count(p => p.ProductId == b.ProductId && p.ProductType == (int)Enums.ProductType.Daybed) > 0);
                    break;
                case "SpaPasses":
                    bookings = bookings.Where(b => _products.Count(p => p.ProductId == b.ProductId && p.ProductType == (int)Enums.ProductType.SpaPass) > 0);
                    break;
                default: // All
                    break;
            }
        }

        private void FilterProductsByProductType(ref IEnumerable<Products> product)
        {
            switch (ProductTypeDdl.SelectedValue)
            {
                case "DayPasses":
                    product = product.Where(b => b.ProductType == (int) Enums.ProductType.DayPass);
                    break;
                case "Cabanas":
                    product = product.Where(b => b.ProductType == (int)Enums.ProductType.Cabana);
                    break;
                case "Daybeds":
                    product = product.Where(b => b.ProductType == (int)Enums.ProductType.Daybed);
                    break;
                case "SpaPasses":
                    product = product.Where(b => b.ProductType == (int)Enums.ProductType.SpaPass);
                    break;
                default: // All
                    break;
            }
        }

        private int GetCapacity(int hotelId, DateTime date)
        {
            var products = _bookingRepository.ProductList
                .Where(p => p.HotelId == hotelId && !p.IsDelete && p.IsActive);
            int available = 0;

            FilterProductsByProductType(ref products);

            products.ToList().ForEach(item =>
            {
                var blackout = _bookingRepository.BlockedDatesCustomPriceList
                    .FirstOrDefault(x => x.ProductId == item.ProductId &&
                                         x.Date.Date == date.Date &&
                                         x.Capacity == 0);
                if (blackout != null)
                {
                    available = 0;
                }
                else
                {
                    var customCapacity = _bookingRepository.BlockedDatesCustomPriceList
                        .FirstOrDefault(x => x.ProductId == item.ProductId &&
                                             x.Date.Date == date.Date &&
                                             x.Capacity != 0);

                    available += customCapacity != null && customCapacity.Capacity.HasValue
                        ? customCapacity.Capacity.Value
                        : GetDefaultPassLimit(hotelId, item.ProductType);
                }
            });
            return available;
        }

        private int GetDefaultPassLimit(int hotelId, int productType)
        {
            var products = _bookingRepository.ProductList.Where(p => p.HotelId == hotelId && p.IsActive && !p.IsDelete && p.ProductType == productType).ToList();

            switch (productType)
            {
                case (int)Enums.ProductType.SpaPass:
                    return products.Where(h => h.ProductType == (int)Enums.ProductType.SpaPass).DefaultIfEmpty().Max(h => h != null ? h.PassCapacity : 0);
                case (int)Enums.ProductType.Cabana:
                    return products.Where(h => h.ProductType == (int)Enums.ProductType.Cabana).DefaultIfEmpty().Max(h => h != null ? h.PassCapacity : 0);
                case (int)Enums.ProductType.Daybed:
                    return products.Where(h => h.ProductType == (int)Enums.ProductType.Daybed).DefaultIfEmpty().Max(h => h != null ? h.PassCapacity : 0);
                default:
                    return products.Where(h => h.ProductType == (int)Enums.ProductType.DayPass).DefaultIfEmpty().Max(h => h != null ? h.PassCapacity : 0);
            }
        }
    }
}