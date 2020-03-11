using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DayaxeDal.Custom;
using DayaxeDal.Repositories;
using Newtonsoft.Json;

namespace DayaxeDal
{
    public static class Helper
    {
        private static readonly Random RandomNumber = new Random();
        const double RADIUS = 6371;
        public const double Miles = 1.609344;

        static Regex MobileCheck = new Regex(@"android|(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex MobileVersionCheck = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[RandomNumber.Next(s.Length)]).ToArray());
        }

        public static bool IsValidEmail(string email)
        {
            var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

            return !string.IsNullOrEmpty(email) && r.IsMatch(email);
        }

        public static string GetCreditCardType(string CCNum)
        {
            var CreditCardNumber = Regex.Replace(CCNum, @"\s+", "");
            Regex regVisa = new Regex("^4[0-9]{12}(?:[0-9]{3})?$");
            Regex regMaster = new Regex("^5[1-5][0-9]{14}$");
            Regex regExpress = new Regex("^3[47][0-9]{13}$");
            Regex regDiners = new Regex("^3(?:0[0-5]|[68][0-9])[0-9]{11}$");
            Regex regDiscover = new Regex("^6(?:011|5[0-9]{2})[0-9]{12}$");
            Regex regJCB = new Regex("^(?:2131|1800|35\\d{3})\\d{11}$");


            if (regVisa.IsMatch(CreditCardNumber))
                return "VISA";
            else if (regMaster.IsMatch(CreditCardNumber))
                return "MASTER";
            else if (regExpress.IsMatch(CreditCardNumber))
                return "AEXPRESS";
            else if (regDiners.IsMatch(CreditCardNumber))
                return "DINERS";
            else if (regDiscover.IsMatch(CreditCardNumber))
                return "DISCOVERS";
            else if (regJCB.IsMatch(CreditCardNumber))
                return "JCB";
            else
                return "invalid";
        }

        //public static bool PassesLuhnTest(string cardNumber)
        //{
        //    //Clean the card number- remove dashes and spaces
        //    cardNumber = cardNumber.Replace("-", "").Replace(" ", "");

        //    //Convert card number into digits array
        //    int[] digits = new int[cardNumber.Length];
        //    for (int len = 0; len < cardNumber.Length; len++)
        //    {
        //        digits[len] = Int32.Parse(cardNumber.Substring(len, 1));
        //    }

        //    //Luhn Algorithm
        //    //Adapted from code availabe on Wikipedia at
        //    //http://en.wikipedia.org/wiki/Luhn_algorithm
        //    int sum = 0;
        //    bool alt = false;
        //    for (int i = digits.Length - 1; i >= 0; i--)
        //    {
        //        int curDigit = digits[i];
        //        if (alt)
        //        {
        //            curDigit *= 2;
        //            if (curDigit > 9)
        //            {
        //                curDigit -= 9;
        //            }
        //        }
        //        sum += curDigit;
        //        alt = !alt;
        //    }

        //    //If Mod 10 equals 0, the number is good and this will return true
        //    return sum % 10 == 0;
        //}

        /// <summary>
        /// Use to get string of rating, this function should not be here, will move to other place later
        /// </summary>
        /// <param name="rating">Rating of hotels</param>
        /// <returns></returns>
        public static string GetRatingString(double rating, bool isInlineStyle = true)
        {
            string str = string.Empty;
            for (var i = 0; i <= 4; i++)
            {

                string url;
                string style = string.Empty;

                if (isInlineStyle)
                {
                    style = "height:auto; width:32px !important; display: inline-block;margin-right:7px;margin-left:7px;";
                }

                if (rating - i >= 1)
                {
                    url = Constant.FullStarEmail;
                }
                else if (rating - i > 0)
                {
                    url = Constant.HalfStarEmail;
                }
                else
                {
                    url = Constant.EmptyStarEmail;
                }
                str += string.Format("<img src=\"{0}\" style=\"{1}\" class=\"img-responsive star\" alt=\"star\" />", url, style);
            }
            return str;
        }

        /// <summary>
        /// Use on Web, do not have image url from amazone
        /// </summary>
        /// <param name="rating"></param>
        /// <returns></returns>
        public static string GetRatingString(double rating)
        {
            string str = string.Empty;
            for (var i = 0; i <= 4; i++)
            {

                string url;

                if (rating - i >= 1)
                {
                    url = Constant.FullStar;
                }
                else if (rating - i > 0)
                {
                    url = Constant.HalfStar;
                }
                else
                {
                    url = Constant.EmptyStar;
                }
                str += string.Format("<img src=\"{0}\" style=\"width: 14px;display: inline-block;\" class=\"img-responsive star\" alt=\"star\" />", url);
            }
            return str;
        }

        //public static string GetCompactBlackout(List<BlockedDates> blackoutDays)
        //{
        //    string str = string.Empty;

        //    if (blackoutDays.Any())
        //    {
        //        blackoutDays = blackoutDays.OrderBy(x => x.Date).ToList();
        //        DateTime? startDate = null;
        //        DateTime? endDate = null;
        //        blackoutDays.ForEach(item =>
        //        {
        //            if (item.Date.HasValue)
        //            {
        //                // First item 
        //                if (!startDate.HasValue)
        //                {
        //                    startDate = item.Date;
        //                    endDate = item.Date;
        //                }
        //                else if (endDate.HasValue && endDate.Value.AddDays(1).Date == item.Date.Value.Date)
        //                {
        //                    endDate = item.Date;
        //                }
        //                else
        //                {
        //                    // endDate is not continue
        //                    str += GetDateString(startDate.Value, endDate.Value);
        //                    startDate = item.Date;
        //                    endDate = item.Date;
        //                }
        //            }
        //        });
        //        if (startDate.HasValue && endDate.HasValue)
        //        {
        //            str += GetDateString(startDate.Value, endDate.Value);
        //        }
        //        if (!string.IsNullOrEmpty(str))
        //        {
        //            str = str.Trim().Substring(0, str.Length - 2);
        //        }
        //    }

        //    return str;
        //}

        //private static string GetDateString(DateTime startdate, DateTime enddate)
        //{
        //    if (startdate.Date == enddate.Date)
        //    {
        //        return string.Format("{0}, ", startdate.ToString("MMM dd"));
        //    }
        //    else if (startdate.Month == enddate.Month)
        //    {
        //        return string.Format("{0} - {1}, ", startdate.ToString("MMM dd"), enddate.ToString("dd"));
        //    }
        //    else
        //    {
        //        return string.Format("{0} - {1}, ", startdate.ToString("MMM dd"), enddate.ToString("MMM dd"));
        //    }
        //}

        //public static string GetCompactBlackoutProductPage(List<BlockedDates> blackoutDays)
        //{
        //    string str = string.Empty;

        //    if (blackoutDays.Any())
        //    {
        //        blackoutDays = blackoutDays.OrderBy(x => x.Date).ToList();
        //        DateTime? startDate = null;
        //        DateTime? endDate = null;
        //        blackoutDays.ForEach(item =>
        //        {
        //            if (item.Date.HasValue)
        //            {
        //                // First item 
        //                if (!startDate.HasValue)
        //                {
        //                    startDate = item.Date;
        //                    endDate = item.Date;
        //                }
        //                else if (endDate.HasValue && endDate.Value.AddDays(1).Date == item.Date.Value.Date)
        //                {
        //                    endDate = item.Date;
        //                }
        //                else
        //                {
        //                    // endDate is not continue
        //                    str += GetDateStringProduct(startDate.Value, endDate.Value);
        //                    startDate = item.Date;
        //                    endDate = item.Date;
        //                }
        //            }
        //        });
        //        if (startDate.HasValue && endDate.HasValue)
        //        {
        //            str += GetDateStringProduct(startDate.Value, endDate.Value);
        //        }
        //        if (!string.IsNullOrEmpty(str))
        //        {
        //            str = string.Format("<ul class=\"blackout-days\">{0}</ul>", str);
        //        }
        //    }

        //    return str;
        //}

        //private static string GetDateStringProduct(DateTime startdate, DateTime enddate)
        //{
        //    if (startdate.Date == enddate.Date)
        //    {
        //        return string.Format("<li>{0}</li>", startdate.ToString("ddd, MMM dd"));
        //    }
        //    else if (startdate.Month == enddate.Month)
        //    {
        //        return string.Format("<li>{0} - {1}</li>", startdate.ToString("ddd, MMM dd"), enddate.ToString("dd"));
        //    }
        //    else
        //    {
        //        return string.Format("<li>{0} - {1}</li>", startdate.ToString("ddd, MMM dd"), enddate.ToString("MMM dd"));
        //    }
        //}

        //public static string ReplaceFirstOccurrence(string source, string find, string replaceStr)
        //{
        //    int idx = source.IndexOf(find, StringComparison.OrdinalIgnoreCase);
        //    string result = source.Remove(idx, find.Length).Insert(idx, replaceStr);
        //    return result;
        //}

        public static string ReplaceLastOccurrence(string source, string find, string replaceStr)
        {
            int idx = source.LastIndexOf(find, StringComparison.OrdinalIgnoreCase);
            string result = source.Remove(idx, find.Length).Insert(idx, replaceStr);
            return result;
        }

        /// <summary>
        /// Convert degrees to Radians
        /// </summary>
        /// <param name="x">Degrees</param>
        /// <returns>The equivalent in radians</returns>
        private static double Radians(double x)
        {
            return x * Math.PI / 180;
        }

        /// <summary>
        /// Calculate the distance between two places.
        /// </summary>
        /// <param name="lon1"></param>
        /// <param name="lat1"></param>
        /// <param name="lon2"></param>
        /// <param name="lat2"></param>
        /// <returns></returns>
        public static double DistanceBetweenPlaces(
            double lon1,
            double lat1,
            double lon2,
            double lat2)
        {
            double dlon = Radians(lon2 - lon1);
            double dlat = Radians(lat2 - lat1);

            double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + Math.Cos(Radians(lat1)) * Math.Cos(Radians(lat2)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
            double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return (angle * RADIUS) / Miles;
        }

        //public static double GetDistanceFromLatLonInMiles(double lat1, double lon1, double lat2, double lon2)
        //{
        //    var R = 6371; // Radius of the earth in km
        //    var dLat = Deg2Rad(lat2 - lat1);  // deg2rad below
        //    var dLon = Deg2Rad(lon2 - lon1);
        //    var a =
        //            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
        //            Math.Sin(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
        //            Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
        //        ;
        //    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        //    var d = R * c; // Distance in km
        //    return d / Miles;
        //}

        public static double GetDistanceFromLatLonInMiles(double lat1,
            double long1, double lat2, double long2)
        {
            /*
                The Haversine formula according to Dr. Math.
                http://mathforum.org/library/drmath/view/51879.html

                dlon = lon2 - lon1
                dlat = lat2 - lat1
                a = (sin(dlat/2))^2 + cos(lat1) * cos(lat2) * (sin(dlon/2))^2
                c = 2 * atan2(sqrt(a), sqrt(1-a)) 
                d = R * c

                Where
                    * dlon is the change in longitude
                    * dlat is the change in latitude
                    * c is the great circle distance in Radians.
                    * R is the radius of a spherical Earth.
                    * The locations of the two points in 
                        spherical coordinates (longitude and 
                        latitude) are lon1,lat1 and lon2, lat2.
            */
            double dDistance = Double.MinValue;
            double dLat1InRad = lat1 * (Math.PI / 180.0);
            double dLong1InRad = long1 * (Math.PI / 180.0);
            double dLat2InRad = lat2 * (Math.PI / 180.0);
            double dLong2InRad = long2 * (Math.PI / 180.0);

            double dLongitude = dLong2InRad - dLong1InRad;
            double dLatitude = dLat2InRad - dLat1InRad;

            // Intermediate result a.
            double a = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                       Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) *
                       Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

            // Intermediate result c (great circle distance in Radians).
            double c = 2.0 * Math.Asin(Math.Sqrt(a));

            // Distance.
            const Double kEarthRadiusMiles = 3956.0;
            dDistance = kEarthRadiusMiles * c;
            //const Double kEarthRadiusKms = 6376.5;
            //dDistance = kEarthRadiusKms * c;

            return dDistance;
        }

        private static double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180);
        }

        public static string ResolveRelativeToAbsoluteUrl(Uri baseUri, string relativeUrl)
        {
            return new Uri(baseUri, relativeUrl).AbsoluteUri;
        }

        //public static async Task<T> GetDataAsync<T>(string apiUrl) where T : class
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        var response = await client.GetAsync(apiUrl);
        //        try
        //        {
        //            response.EnsureSuccessStatusCode();
        //            var result = await response.Content.ReadAsStringAsync();
        //            return JsonConvert.DeserializeObject<T>(result);
        //        }
        //        catch (Exception)
        //        {
        //            return default(T);
        //        }
        //    }
        //}

        public static void RefreshPageAsync(string url)
        {
            using (var wb = new WebClient())
            {
                wb.DownloadData(url);
            }
        }

        /// <summary>
        /// Create Control from 1 to 70
        /// </summary>
        /// <param name="control"></param>
        public static void CreateControl(Control control)
        {
            for (var i = 0; i <= 70; i++)
            {
                var liTag = new HtmlGenericControl("li");
                var link = new HtmlAnchor
                {
                    HRef = "#",
                    InnerText = i.ToString()
                };
                liTag.Controls.Add(link);
                control.Controls.Add(liTag);
            }
        }

        public static string GetStringPassByProductType(int productType)
        {
            switch (productType)
            {
                case (int)Enums.ProductType.Cabana:
                    return Constant.CabanasPassString;
                case (int)Enums.ProductType.Daybed:
                    return Constant.DaybedsString;
                case (int)Enums.ProductType.SpaPass:
                    return Constant.SpaPassString;
                case (int)Enums.ProductType.AddOns:
                    return Constant.AddOnsPassString;
                case (int)Enums.ProductType.DayPass:
                    return Constant.DayPassString;
                default:
                    return string.Empty;
            }
        }

        public static string GetTabString(string itemClass, string activeClass, string id, string imageUrl, string text, bool isBusiness = false)
        {
            string strStyle = string.Empty;
            string str = "<li role=\"presentation\" class=\"col-xs-{0} {1}\">"
                       + "<a href=\"#{2}\" aria-controls=\"{2}\" role=\"tab\" data-toggle=\"tab\" aria-expanded=\"true\">"
                       + "<img src=\"{3}\" class=\"img-responsive {2}\" />"
                       + "<span {5}>{4}</span>"
                       + "</a>"
                       + "</li>";
            if (isBusiness)
            {
                strStyle = "style=\"line-height:90%;\"";
            }
            return string.Format(str, itemClass, activeClass, id, imageUrl, text, strStyle);
        }

        public static string GetHotelName(Products products)
        {
            return string.Format("{0} at {1}", products.ProductName, products.Hotels.HotelName);
        }

        public static string ReplaceSpecialCharacter(string str)
        {
            return Regex.Replace(str.Trim(), @"[^0-9a-zA-Z]+", "-").ToLower();
        }

        public static string FormatPrice(double price)
        {
            return string.Format("{0}{1}",
                price >= 0 ? string.Empty : "-",
                price.ToString(Math.Abs(price % 1) <= (Double.Epsilon * 100) ? "C0" : "C", new CultureInfo("en-US")).Replace("(", "").Replace(")", ""));
        }
        public static string FormatPriceWithFixed(double price)
        {
            return string.Format("{0}${1}",
                price >= 0 ? string.Empty : "-",
                price.ToString("##0.00", new CultureInfo("en-US")).Replace("-", ""));
        }

        public static Bitmap ChangeOpacity(Image img, float opacityvalue)
        {
            SolidBrush blueBrush = new SolidBrush(Color.FromArgb(50, 00, 00, 00));

            // Create rectangle.
            Rectangle rect = new Rectangle(0, 0, img.Width, img.Height);

            Bitmap bmp = new Bitmap(img.Width, img.Height); // Determining Width and Height of Source Image
            Graphics graphics = Graphics.FromImage(bmp);
            ColorMatrix colormatrix = new ColorMatrix
            {
                Matrix33 = opacityvalue
            };
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            graphics.FillRectangle(blueBrush, rect);
            graphics.Dispose();   // Releasing all resource used by graphics 
            return bmp;
        }

        //public static string GetPassLeft(int availablePass)
        //{
        //    return string.Format("{0} Left", availablePass);
        //}

        public static string GetStringMaxGuest(int maxGuest)
        {
            return maxGuest == 1 ? "per guest" : string.Format("up to {0} guests", maxGuest);
        }

        public static string GetUrlSendSurvey(Bookings bookings)
        {
            using (var surveyRepository = new SurveyRepository())
            {
                if (bookings != null)
                {
                    var survey = surveyRepository.GetSurvey(bookings.BookingId);
                    string surveyCode = string.Empty;
                    if (survey != null)
                    {
                        surveyCode = survey.Code;
                    }

                    string url = bookings.BrowsePassUrl;
                    if (Regex.IsMatch(bookings.BrowsePassUrl, ".+(?<ProductId>/\\d+)$"))
                    {
                        url = bookings.BrowsePassUrl.Replace(
                                Regex.Match(bookings.BrowsePassUrl, ".+(?<ProductId>/\\d+)$").Groups["ProductId"].Value,
                                string.Empty);
                    }
                    return string.Format(AppConfiguration.DefaultImageUrlSendEmail + "{0}/{1}",
                        url,
                        surveyCode);
                }
                return string.Empty;
            }
        }

        public static List<KeyValuePair<string, string>> EnumToList<T>()
        {
            var array = (T[])(Enum.GetValues(typeof(T)).Cast<T>());
            var array2 = Enum.GetNames(typeof(T)).ToArray<string>();
            List<KeyValuePair<string, string>> lst = null;
            for (int i = 0; i < array.Length; i++)
            {
                if (lst == null)
                    lst = new List<KeyValuePair<string, string>>();
                string name = array2[i];
                T value = array[i];
                lst.Add(new KeyValuePair<string, string>(name, value.ToString()));
            }
            return lst;
        }

        public static void ResetClientCache()
        {
            var url = ResolveRelativeToAbsoluteUrl(new Uri(AppConfiguration.DayaxeClientUrl), Constant.RefreshHotelUrl);
            RefreshPageAsync(url);
        }

        public static double CalculateDiscount(Discounts discounts, double actualPrice, int totalTicket = 0)
        {
            switch (discounts.PromoType)
            {
                case (int)Enums.PromoType.Fixed:
                    if (discounts.PercentOff > 0 && discounts.MinAmount <= totalTicket * actualPrice && totalTicket != 0)
                    {
                        actualPrice -= discounts.PercentOff / totalTicket;
                    }
                    break;
                default:
                    if (discounts.PercentOff > 0)
                    {
                        actualPrice -= actualPrice * discounts.PercentOff / 100;
                    }
                    break;
            }
            return actualPrice;
        }

        public static List<DateTime> GetDateRanges(DateTime startDate, DateTime endDate)
        {
            var result = new List<DateTime>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                result.Add(date);
            }
            return result;
        }

        public static CustomerInfos GetCustomerInfosByBookingId(long bookingId)
        {
            using (var repository = new BookingRepository())
            {
                var bookings = repository.BookingList.First(x => x.BookingId == bookingId);
                var customerInfos = repository.CustomerInfoList.First(x => x.CustomerId == bookings.CustomerId);
                return customerInfos;
            }
        }

        public static string Get(string url)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    // by calling .Result you are performing a synchronous call
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    string responseString = responseContent.ReadAsStringAsync().Result;

                    return responseString;
                }

                return string.Empty;
            }
        }

        public static string Post(string url, string email)
        {
            using (var client = new HttpClient())
            {
                var param = new List<KeyValuePair<string, string>>();
                param.Add(new KeyValuePair<string, string>("api_key", AppConfiguration.KlaviyoPrivateApiKey));
                param.Add(new KeyValuePair<string, string>("email", email));
                param.Add(new KeyValuePair<string, string>("confirm_optin", "false"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new FormUrlEncodedContent(param);
                var response = client.PostAsync(url, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    // by calling .Result you are performing a synchronous call
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    string responseString = responseContent.ReadAsStringAsync().Result;

                    return responseString;
                }

                return string.Empty;
            }
        }

        public static bool BrowserIsMobile()
        {
            Debug.Assert(HttpContext.Current != null);

            if (HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"] != null)
            {
                var u = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"].ToString();

                if (u.Length < 4)
                    return false;

                if (MobileCheck.IsMatch(u) || MobileVersionCheck.IsMatch(u.Substring(0, 4)))
                    return true;
            }

            return false;
        }

        public static short GetTimezoneOffset(string destinationTimezoneId, DateTime currentDate)
        {
            if (destinationTimezoneId == Constant.DefaultLosAngelesTimezoneId)
            {
                if ((currentDate.Month < 11 && currentDate.Month > 3) || 
                    (currentDate.Month == 11 && currentDate.Day > 5) || 
                    (currentDate.Month == 3 && currentDate.Day > 11))
                {
                    return Constant.LosAngelesTimeWithUtc;
                }
                else
                {
                    return Constant.LosAngelesSummerTimeWithUtc;
                }
            }
            return 0;
        }

        public static DateTime ParseInThisYearOrNextYear(string s)
        {
            DateTime dt = DateTime.UtcNow;
            if (!Parse(s, "ddd, MMM dd", out dt))
            {
                if (!Parse(s + " " + DateTime.Now.AddYears(1).Year, "ddd, MMM dd yyyy", out dt) &&
                    !Parse(s + " " + DateTime.Now.AddYears(-1).Year, "ddd, MMM dd yyyy", out dt))
                {
                    throw new FormatException();
                }
            }

            return dt;
        }

        private static bool Parse(string s, string format, out DateTime dt)
        {
            return DateTime.TryParseExact(
                s,
                format,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dt
            );
        }

        public static AmentyControl BindAmenties(Hotels hotels)
        {
            var result = new AmentyControl();
            string tab = string.Empty;
            var amenties = hotels.AmentiesItem;
            double item = amenties.ActiveFeatures > 0 ? 12 / amenties.ActiveFeatures : 0;
            string itemClass = item.ToString(CultureInfo.InvariantCulture);
            if (12 % amenties.ActiveFeatures != 0)
            {
                itemClass = "2-5";
            }
            string activeClass = "active";
            const string strItem = "<div class=\"col-md-6 col-sm-6 col-xs-6 padding-left-0 padding-right-5\">{0}</div>" +
                                   "<div class=\"col-md-6 col-sm-6 col-xs-6 padding-left-5 padding-right-0\">{1}</div>";

            var strPool = GetAmentiesString(ref tab, ref itemClass, ref activeClass, hotels.PoolFeatures, amenties.PoolActive, "restaurant", "/images/pool.png", "pool", false);
            result.PoolAmentyControl = string.Format(strItem, strPool.Item1, strPool.Item2);

            var strGym = GetAmentiesString(ref tab, ref itemClass, ref activeClass, hotels.GymFeatures, amenties.GymActive, "sports-club", "/images/gym-inactive.png", "fitness and<br/>activities", false);
            result.GymAmentyControl = string.Format(strItem, strGym.Item1, strGym.Item2);

            var strSpa = GetAmentiesString(ref tab, ref itemClass, ref activeClass, hotels.SpaFeatures, amenties.SpaActive, "spa-club", "/images/spa.png", "spa", false);
            result.SpaAmentyControl = string.Format(strItem, strSpa.Item1, strSpa.Item2);

            var strBusiness = GetAmentiesString(ref tab, ref itemClass, ref activeClass, hotels.OfficeFeatures, amenties.BusinessActive, "pick-up", "/images/handshake.png", "business<br/>services", true);
            result.BusinessCenterControl = string.Format(strItem, strBusiness.Item1, strBusiness.Item2);

            var strDinning = GetAmentiesString(ref tab, ref itemClass, ref activeClass, hotels.DinningFeatures, amenties.DinningActive, "dining", "/images/icon_dinning.png", "dining", true);
            result.DiningControl = string.Format(strItem, strDinning.Item1, strDinning.Item2);

            var strEvent = GetAmentiesString(ref tab, ref itemClass, ref activeClass, hotels.EventFeatures, amenties.EventActive, "event", "/images/icon_event.png", "events", true);
            result.EventControl = string.Format(strItem, strEvent.Item1, strEvent.Item2);
            
            result.MainTabString = tab;
            return result;
        }

        private static Tuple<string, string> GetAmentiesString(ref string tab, ref string itemClass, ref string activeClass, string[] features, bool isActive, string id, string image, string text, bool isBusiness)
        {
            var itemLeft = string.Empty;
            var itemRight = string.Empty;
            if (isActive)
            {
                tab += Helper.GetTabString(itemClass, activeClass, id, image, text, isBusiness);
                activeClass = string.Empty;

                var leftItem = features.Length / 2;
                if (features.Length % 2 != 0)
                {
                    leftItem += 1;
                }
                for (int i = 0; i < leftItem; i++)
                {
                    itemLeft += "<li><span>" + features[i] + "</span></li>";
                }

                for (int j = leftItem; j < features.Length; j++)
                {
                    itemRight += "<li><span>" + features[j] + "</span></li>";
                }
            }

            return new Tuple<string, string>(itemLeft, itemRight);
        }

        public static string GetMixpanelScriptRedirect(int productId, string url)
        {
            return string.Format("function f{0}(e) {{ e.stopPropagation(); window.location.href = \"{1}\"; }}", productId, url);
        }
    }
}
