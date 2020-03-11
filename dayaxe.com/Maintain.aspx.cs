using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace dayaxe.com
{
    public partial class Maintain : Page
    {
        private readonly BookingRepository _bookingRepository = new BookingRepository();
        private readonly ProductRepository _productRepository = new ProductRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            string newUrl;
            var currentUrl = Request.Url.AbsoluteUri;
            //s/{market}/search.aspx
            var isSearch = currentUrl.Contains("search.aspx") && 
                Page.RouteData.Values["market"] != null;
            if (isSearch)
            {
                newUrl = currentUrl.Replace("search.aspx", "day-passes");
                Response.Redirect(newUrl, true);
            }

            //{market}/{bookingId}/confirm.aspx
            var isConfirm = currentUrl.Contains("confirm.aspx") && 
                Page.RouteData.Values["market"] != null &&
                Page.RouteData.Values["bookingId"] != null;
            if (isConfirm)
            {
                // {market}/{city}/{hotelName}/{productName}-confirm-{bookingId}
                var booking = _bookingRepository.GetById(int.Parse(Page.RouteData.Values["bookingId"].ToString()));
                if (booking == null)
                {
                    _bookingRepository.AddLog(new Logs
                    {
                        LogKey = "Error Redirect Booking",
                        UpdatedContent = currentUrl,
                        UpdatedBy = 1,
                        UpdatedDate = DateTime.UtcNow
                    });
                    throw new HttpException(404, string.Format("Url Error - {0}", currentUrl));
                }

                var products = _bookingRepository.ProductList.First(p => p.ProductId == booking.ProductId);
                var hotels = _bookingRepository.HotelList.First(h => h.HotelId == products.HotelId);

                newUrl = string.Format(Constant.ConfirmProductPage,
                    Page.RouteData.Values["market"],
                    hotels.City.Trim().Replace(" ", "-"),
                    hotels.HotelName.Trim().Replace(" ", "-"),
                    products.ProductName.Trim().Replace(" ", "-"),
                    Page.RouteData.Values["bookingId"]).ToLower();
                Response.Redirect(newUrl, true);
            }

            //{market}/{city}/{hotelName}-day-pass/{id}
            var isSurvey = currentUrl.Contains("-day-pass") &&
                Page.RouteData.Values["market"] != null &&
                Page.RouteData.Values["city"] != null &&
                Page.RouteData.Values["hotelName"] != null &&
                Page.RouteData.Values["id"] != null;
            if (isSurvey)
            {
                var product = _productRepository.GetProductBySurveyId(Page.RouteData.Values["id"].ToString());
                if (product == null)
                {
                    _bookingRepository.AddLog(new Logs
                    {
                        LogKey = "Error Redirect Change Password",
                        UpdatedContent = currentUrl,
                        UpdatedBy = 1,
                        UpdatedDate = DateTime.UtcNow
                    });
                    throw new HttpException(404, string.Format("Url Error - {0}", currentUrl));
                }
                newUrl = string.Format("/{0}/{1}/{2}/{3}/{4}",
                    Page.RouteData.Values["market"] ?? "socal",
                    Helper.ReplaceSpecialCharacter(product.Hotels.City),
                    Helper.ReplaceSpecialCharacter(product.Hotels.HotelName),
                    Helper.ReplaceSpecialCharacter(product.ProductName),
                    Page.RouteData.Values["id"]);
                Response.Redirect(newUrl);
            }

            //{market}/{city}/{hotelName}-day-pass
            var isHotel = currentUrl.Contains("-day-pass") &&
                Page.RouteData.Values["market"] != null &&
                Page.RouteData.Values["city"] != null &&
                Page.RouteData.Values["hotelName"] != null;
            if (isHotel)
            {
                var product = _productRepository.GetProductByHotelName(Page.RouteData.Values["hotelName"].ToString());
                if (product == null)
                {
                    _bookingRepository.AddLog(new Logs
                    {
                        LogKey = "Error Redirect Product",
                        UpdatedContent = currentUrl,
                        UpdatedBy = 1,
                        UpdatedDate = DateTime.UtcNow
                    });
                    throw new HttpException(404, string.Format("Url Error - {0}", currentUrl));
                }
                newUrl = string.Format("/{0}/{1}/{2}/{3}",
                    Page.RouteData.Values["market"] ?? "socal",
                    Helper.ReplaceSpecialCharacter(product.Hotels.City),
                    Helper.ReplaceSpecialCharacter(product.Hotels.HotelName),
                    Helper.ReplaceSpecialCharacter(product.ProductName));
                Response.Redirect(newUrl);
            }

            //{market}/{city}/{hotelName}/reviews
            var isReviews = currentUrl.Contains("reviews") &&
                Page.RouteData.Values["market"] != null &&
                Page.RouteData.Values["city"] != null &&
                Page.RouteData.Values["hotelName"] != null;
            if (isReviews)
            {
                var product = _productRepository.GetProductByHotelName(Page.RouteData.Values["hotelName"].ToString());
                if (product == null)
                {
                    _bookingRepository.AddLog(new Logs
                    {
                        LogKey = "Error Redirect Reviews Products",
                        UpdatedContent = currentUrl,
                        UpdatedBy = 1,
                        UpdatedDate = DateTime.UtcNow
                    });
                    throw new HttpException(404, string.Format("Url Error - {0}", currentUrl));
                }
                newUrl = string.Format("/{0}/{1}/{2}/{3}/reviews",
                    Page.RouteData.Values["market"] ?? "socal",
                    Helper.ReplaceSpecialCharacter(product.Hotels.City),
                    Helper.ReplaceSpecialCharacter(product.Hotels.HotelName),
                    Helper.ReplaceSpecialCharacter(product.ProductName));
                Response.Redirect(newUrl);
            }

            //{market}/{hotelId}/book.aspx
            var isBook = currentUrl.Contains("book.aspx") &&
                Page.RouteData.Values["market"] != null &&
                Page.RouteData.Values["hotelId"] != null;
            if (isBook)
            {
                var product = _productRepository.GetById(int.Parse(Page.RouteData.Values["hotelId"].ToString()));
                if (product == null)
                {
                    _bookingRepository.AddLog(new Logs
                    {
                        LogKey = "Error Redirect Book Hotels",
                        UpdatedContent = currentUrl,
                        UpdatedBy = 1,
                        UpdatedDate = DateTime.UtcNow
                    });
                    throw new HttpException(404, string.Format("Url Error - {0}", currentUrl));
                }
                newUrl = string.Format("/{0}/{1}/{2}/{3}-book",
                    Page.RouteData.Values["market"] ?? "socal",
                    Helper.ReplaceSpecialCharacter(product.Hotels.City),
                    Helper.ReplaceSpecialCharacter(product.Hotels.HotelName),
                    Helper.ReplaceSpecialCharacter(product.ProductName));
                Response.Redirect(newUrl);
            }
        }
    }
}