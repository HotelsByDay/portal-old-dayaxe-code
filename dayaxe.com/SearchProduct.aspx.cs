using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using Newtonsoft.Json;

namespace dayaxe.com
{
    public partial class SearchProductPage : BasePage
    {
        protected bool ShowAuth { get; set; }

        protected string PageCta { get; set; }

        protected string SearchStartedString { get; set; }

        private readonly ProductRepository _productRepository = new ProductRepository();
        private readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();
        private readonly DiscountRepository _discountRepository = new DiscountRepository();

        private List<Enums.ProductType> _productTypeSearch = new List<Enums.ProductType>
        {
             Enums.ProductType.DayPass,
             Enums.ProductType.Cabana,
             Enums.ProductType.Daybed,
             Enums.ProductType.SpaPass
        };
        private string StrPassSearch { get; set; }
        private int CustomerIdSearch { get; set; }
        private object MarketUrl { get; set; }
        private Markets MarketSearch { get; set; }
        protected DateTime StartDate { get; set; }
        protected DateTime EndDate { get; set; }
        private string RequestRegion { get; set; }

        private double LowPrice { get; set; }

        private double HighPrice { get; set; }

        private int LowDistance { get; set; }

        private int HighDistance { get; set; }

        private int TotalGuest { get; set; }
        private SearchParams SearchParamsSaved { get; set; }

        private SearchResult SearchResultObject { get; set; }

        private bool IsForceResetFilter { get; set; }

        protected DateTime RestrictDate { get; set; }

        protected string UserIp { get; set; }

        private Markets DefaultMarkets { get; set; }


        protected void Page_Init(object sender, EventArgs e)
        {
            MarketUrl = Page.RouteData.Values["market"];
            RequestRegion = GetUserRequestLocation();
            DefaultMarkets = _productRepository.MarketList.First(m => m.IsDefault);
            if (string.IsNullOrEmpty(RequestRegion))
            {
                RequestRegion = DefaultMarkets.LocationName;
            }

            // Market is Valid
            if (MarketUrl == null || (string)MarketUrl == "socal")
            {
                if (Session["SearchPage"] != null &&
                    !string.IsNullOrEmpty((string)Session["SearchPage"]) &&
                    !((string)Session["SearchPage"]).Contains("socal"))
                {
                    Response.Redirect((string)Session["SearchPage"], true);
                }

                MarketSearch = _productRepository.MarketList.FirstOrDefault(m => m.LocationName == RequestRegion.ToString() && m.IsActive) ??
                               DefaultMarkets;
                Response.Redirect(string.Format(Constant.DayPassPage, MarketSearch.Permalink), true);
                MarketUrl = MarketSearch.Permalink;
            }
            else
            {
                MarketSearch = _productRepository.MarketList.FirstOrDefault(m => m.Permalink == (string)MarketUrl && m.IsActive) ??
                               DefaultMarkets;
            }

            if (!IsPostBack)
            {
                Session["SearchPage"] = Request.Url.AbsoluteUri;
                Session.Remove("SearchParamsSaved");
                
                // Get from Last Search if reload page
                if (PublicCustomerSearch != null)
                {
                    SearchParamsSaved = new SearchParams
                    {
                        AvailableTickets = PublicCustomerSearch.TotalGuest,
                        LowPrice = PublicCustomerSearch.MinPrice,
                        HighPrice = PublicCustomerSearch.MaxPrice,
                        LowDistance = PublicCustomerSearch.MinDistance,
                        HighDistance = PublicCustomerSearch.MaxDistance,
                        ProductType = new List<Enums.ProductType>()
                    };
                    if (PublicCustomerSearch.IsDaypass)
                    {
                        SearchParamsSaved.ProductType.Add(Enums.ProductType.DayPass);
                        DaypassLink.CssClass += " active";
                    }
                    else
                    {
                        DaypassLink.CssClass = DaypassLink.CssClass.Replace("active", "");
                    }
                    if (PublicCustomerSearch.IsCabana)
                    {
                        SearchParamsSaved.ProductType.Add(Enums.ProductType.Cabana);
                        CabanasLink.CssClass += " active";
                    }
                    else
                    {
                        CabanasLink.CssClass = CabanasLink.CssClass.Replace("active", "");
                    }
                    if (PublicCustomerSearch.IsDaybed)
                    {
                        SearchParamsSaved.ProductType.Add(Enums.ProductType.Daybed);
                        DaybedsLink.CssClass += " active";
                    }
                    else
                    {
                        DaybedsLink.CssClass = DaybedsLink.CssClass.Replace("active", "");
                    }
                    if (PublicCustomerSearch.IsSpapass)
                    {
                        SearchParamsSaved.ProductType.Add(Enums.ProductType.SpaPass);
                        SpapassLink.CssClass += " active";
                    }
                    else
                    {
                        SpapassLink.CssClass = SpapassLink.CssClass.Replace("active", "");
                    }

                    // Set Public Value for Search
                    HidGuest.Text = PublicCustomerSearch.TotalGuest.ToString();
                    TotalGuest = PublicCustomerSearch.TotalGuest;

                    PriceText.Attributes["data-slider-value"] = string.Format("[{0},{1}]", PublicCustomerSearch.MinPrice,
                        PublicCustomerSearch.MaxPrice);
                    PriceText.Text = string.Format("[{0},{1}]", PublicCustomerSearch.MinPrice,
                        PublicCustomerSearch.MaxPrice);

                    DistanceText.Attributes["data-slider-value"] = string.Format("[{0},{1}]", PublicCustomerSearch.MinDistance,
                        PublicCustomerSearch.MaxDistance);
                    DistanceText.Text = string.Format("[{0},{1}]", PublicCustomerSearch.MinDistance,
                        PublicCustomerSearch.MaxDistance);
                }
                else
                {
                    // Set Search All Product
                    DaypassLink.CssClass += " active";
                    CabanasLink.CssClass += " active";
                    DaybedsLink.CssClass += " active";
                    SpapassLink.CssClass += " active";

                    if (Session["Price"] != null)
                    {
                        PriceText.Text = Session["Price"].ToString();
                    }
                    if (Session["Distance"] != null)
                    {
                        DistanceText.Text = Session["Distance"].ToString();
                    }

                    // Guest
                    if (Session["Guest"] != null)
                    {
                        int totalGuest;
                        int.TryParse(Session["Guest"].ToString(), out totalGuest);
                        HidGuest.Text = totalGuest.ToString();
                        TotalGuest = totalGuest;
                    }

                    // Change Location
                    if (Session["Market"] == null || (MarketUrl != null && Session["Market"] != null &&
                                                      MarketUrl.ToString() != Session["Market"].ToString()))
                    {
                        Session["Market"] = MarketUrl.ToString();
                        //currentDate = DateTime.UtcNow.ToLosAngerlesTime();
                        TotalGuest = 2;
                        HidGuest.Text = "2";
                        Session["Guest"] = 2;
                    }
                }

                BindSearchRange(DateTime.UtcNow.ToLosAngerlesTime());

                SetPageHeader();

                CheckShowAuth();

                CurrentGuest.Text = string.Format("{0} guest(s)", TotalGuest);
            }
            else if(Session["SearchParamsSaved"] != null)
            {
                SearchParamsSaved = JsonConvert.DeserializeObject<SearchParams>(Session["SearchParamsSaved"].ToString());
            }

            BindCity();

            BindGuests();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RestrictDate = DateTime.UtcNow.ToLosAngerlesTime();

            if (RestrictDate > DateTime.UtcNow.ToLosAngerlesTime().Date.AddHours(19))
            {
                RestrictDate = RestrictDate.AddDays(1);
            }

            // Search CallBack
            IsForceResetFilter = Request["__EVENTARGUMENT"] == "true" || !IsPostBack;
            List<Products> listProducts;

            if (!IsPostBack)
            {
                //// Check-In Date
                if (Session["SearchRange"] != null)
                {
                    CheckInDateText.Text = Session["SearchRange"].ToString();
                }
            }
            Initialize();

            // Update new market if user search again
            var currentSession = Session["UserSession"];
            SetCustomerInfo(currentSession);
            
            Session["marketId"] = MarketSearch.Id;
            TitleLiteral.Text = string.Format(Constant.SearchDocumentTitle, MarketSearch.LocationName, StrPassSearch);

            if (currentSession != null)
            {
                // Update new market if user search again
                if (PublicCustomerInfos != null && Request.Path != PublicCustomerInfos.BrowsePassUrl)
                {
                    PublicCustomerInfos.BrowsePassUrl = string.Format(Constant.SearchPageWithCityUrl, MarketSearch.Permalink, Request.Url.Segments[Request.Url.Segments.Length - 1]);
                    _customerInfoRepository.Update(PublicCustomerInfos);
                }

                // Invalid Session
                if (PublicCustomerInfos == null && Request.Params["sp"] == null)
                {
                    Session["UserSession"] = null;
                    Response.Redirect(Request.Url.AbsoluteUri);
                }
            }
            CurrentProductLabel.Text = MarketSearch.LocationName;

            var liTag = (HtmlGenericControl)CityDropdown.FindControl(MarketSearch.Permalink);
            if (liTag != null)
            {
                liTag.Attributes["class"] = "active";
            }
            
            BindHotelSearch(out listProducts);

            if (!IsPostBack)
            {
                BindMetaHeader(listProducts);
            }
        }

        private void BindSearchRange(DateTime currentDate)
        {
            if (currentDate > DateTime.UtcNow.ToLosAngerlesTime().Date.AddHours(19))
            {
                currentDate = currentDate.AddDays(1);
            }
            var endDate = currentDate.AddDays(14);

            var rangeDate = string.Format("{0:ddd, MMM dd} - {1:ddd, MMM dd}", currentDate, endDate);

            CheckInDateText.Text = rangeDate;
            Session["SearchRange"] = rangeDate;
        }

        private void BindCity()
        {
            var markets = _productRepository.MarketList.ToList();

            ReCalculateDistanceWithSearchRegion(ref markets);

            markets.ForEach(market =>
            {
                var liTag = new HtmlGenericControl("li")
                {
                    ID = market.Permalink
                };
                var link = new HtmlAnchor
                {
                    HRef = Helper.ResolveRelativeToAbsoluteUrl(
                        Request.Url,
                        string.Format(Constant.SearchPageWithCityUrl, market.Permalink, "day-passes")),
                    InnerText = market.LocationName
                };
                liTag.Controls.Add(link);

                CityDropdown.Controls.Add(liTag);
            });
        }

        private void ReCalculateDistanceWithSearchRegion(ref List<Markets> markets)
        {
            markets.ForEach(market =>
            {
                market.SetDistanceWithSearchRegion(MarketSearch.Latitude, MarketSearch.Longitude);
            });

            markets = markets.OrderBy(m => m.DistanceWithSearchRegion).ToList();
        }

        private void BindGuests()
        {
            for (var i = 1; i <= 20; i++)
            {
                var liTag = new HtmlGenericControl("li");
                var link = new HtmlAnchor
                {
                    InnerText = string.Format("{0} guest(s)", i),
                    HRef = "#"
                };
                liTag.Controls.Add(link);
                GuestCapacity.Controls.Add(liTag);
            }
        }

        private void SetPageHeader()
        {
            String pageVar = Request.Params["page_var"] != null ? string.Format("\"Page Variant\":\"{0}\",", Request.Params["page_var"]) : String.Empty;
            String lander = Request.Params["lander"] != null ? string.Format("\"Landing Page Name\":\"{0}\",", Request.Params["lander"]) : String.Empty;
            PageCta = Request.Params["page_cta"] != null ? string.Format("\"Landing Page CTA\":\"{0}\"", Request.Params["page_cta"]) : String.Empty;

            if (!String.IsNullOrEmpty(pageVar))
            {
                SearchStartedString = pageVar;
            }
            if (!String.IsNullOrEmpty(lander))
            {
                SearchStartedString += lander;
            }
            if (!String.IsNullOrEmpty(PageCta))
            {
                SearchStartedString += PageCta;
            }
            else if (!String.IsNullOrEmpty(SearchStartedString))
            {
                SearchStartedString = SearchStartedString.Substring(0, SearchStartedString.Length - 1);
            }
        }

        private void SetCustomerInfo(object currentSession)
        {
            if (currentSession != null && PublicCustomerInfos == null)
            {
                PublicCustomerInfos = _customerInfoRepository.GetCustomerInfoBySessionId(currentSession.ToString());
            }
            if (PublicCustomerInfos != null)
            {
                CustomerIdSearch = PublicCustomerInfos.CustomerId;
            }
        }

        private void BindHotelSearch(out List<Products> listProducts)
        {
            SearchResultObject = GetSearchResult();

            listProducts = SearchResultObject.Result.Items;

            const string mixpanelscript = "MixpanelScript";
            string strScript = string.Empty;
            listProducts.ForEach(item =>
            {
                if (item.ProductId == 0)
                {
                    strScript += Helper.GetMixpanelScriptRedirect(item.ProductId, Constant.GoldPassLink + "?source=search");
                }
                else
                {
                    strScript += Helper.GetMixpanelScriptRedirect(item.ProductId,
                        string.Format("/{0}/{1}/{2}/{3}/{4}",
                            Page.RouteData.Values["market"] ?? "socal",
                            Helper.ReplaceSpecialCharacter(item.Hotels.City),
                            Helper.ReplaceSpecialCharacter(item.Hotels.HotelName),
                            Helper.ReplaceSpecialCharacter(item.ProductName),
                            item.ProductId));
                }
            });
            if (!string.IsNullOrEmpty(strScript))
            {
                ScriptManager.RegisterClientScriptBlock(HotelList, typeof(string), mixpanelscript, strScript, true);
            }

            LvHotelRepeater.DataSource = listProducts;
            LvHotelRepeater.DataBind();

            FilterResultHidden.Value = WebUtility.HtmlEncode(string.Format("<b>{0}</b> out of {1}", 
                listProducts.Count > 3 ? listProducts.Count - 1 : listProducts.Count, 
                SearchResultObject.Result.TotalRecords));

            // Keep Search Params in Session
            string json = JsonConvert.SerializeObject(SearchParamsSaved, CustomSettings.SerializerSettings());

            Session["SearchParamsSaved"] = json;
        }

        protected void LvHotelRepeater_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                if (e.Item.DataItemIndex + 1 == 4)
                {
                    var product = (Products)e.Item.DataItem;
                    var aTag = (HtmlAnchor)e.Item.FindControl("HotelItem");
                    var image = (Image)e.Item.FindControl("ProductImage");
                    var titleBottom = (HtmlGenericControl)e.Item.FindControl("titleBottom");

                    aTag.ID = product.ProductId.ToString();
                    aTag.Attributes.Add("onclick", string.Format("f{0}(event)", product.ProductId));

                    image.Attributes["data-original"] = product.CdnImage;
                    image.Attributes["src"] = Constant.ImageDefault;

                    titleBottom.Visible = false;
                }
                else
                {
                    var aTag = (HtmlAnchor)e.Item.FindControl("HotelItem");
                    var image = (Image)e.Item.FindControl("ProductImage");
                    var productHeader = (HtmlGenericControl)e.Item.FindControl("ProductHeader");
                    var address = (HtmlGenericControl)e.Item.FindControl("ProductAddress");
                    var price = (Literal)e.Item.FindControl("PriceProduct");
                    var msrpSpan = (HtmlGenericControl)e.Item.FindControl("msrp");
                    var maxGuestControl = (HtmlGenericControl) e.Item.FindControl("maxGuest");
                    var priceMsrp = (Label)e.Item.FindControl("PriceProductOff");
                    var type = (Literal)e.Item.FindControl("ProductType");
                    var maway = (Literal)e.Item.FindControl("MAwayLit");

                    var passleftDiv = (HtmlGenericControl)e.Item.FindControl("passleftDiv");
                    var litPassleft = (Literal)e.Item.FindControl("LitPassleft");
                    var productTypeLink = (HtmlAnchor) e.Item.FindControl("productTypeLink");
                    var hotelName = (HtmlGenericControl)e.Item.FindControl("HotelName");
                    var titleBottom = (HtmlGenericControl)e.Item.FindControl("titleBottom");
                    var roomTitle = (HtmlGenericControl)e.Item.FindControl("roomTitle");

                    var product = (Products)e.Item.DataItem;
                    bool isOnBlackOutDay = product.IsOnBlackOutDay;
                    DateTime nextAvailableDate = product.NextAvailableDate;
                    string nextAvailableDay = product.NextAvailableDay;

                    aTag.ID = product.ProductId.ToString();
                    aTag.Attributes.Add("onclick", string.Format("f{0}(event)", product.ProductId));
                    productTypeLink.Attributes.Add("onclick", string.Format("f{0}(event)", product.ProductId));
                    image.Attributes["data-original"] = product.CdnImage;
                    image.Attributes["src"] = Constant.ImageDefault;
                    productHeader.InnerText = product.ProductName;
                    address.InnerText = string.Format("{0}, {1}", product.Hotels.Neighborhood, product.Hotels.City);

                    var control = Page.ParseControl(product.Hotels.Rating);

                    if (product.Hotels.TotalCustomerReviews > 0)
                    {
                        control = Page.ParseControl(product.Hotels.CustomerRatingString);
                    }

                    if (control != null)
                    {
                        address.Controls.Add(control);
                    }

                    double actualPrice = product.PerGuestPrice;
                    var normalPrice = actualPrice;
                    priceMsrp.Text = Helper.FormatPrice(actualPrice);

                    // Show Price
                    var discounts = _productRepository.GetAutoPromosByProductId(product.ProductId, StartDate).FirstOrDefault();
                    if (discounts != null)
                    {
                        if (PublicCustomerInfos != null)
                        {
                            discounts = _discountRepository.VerifyDiscounts(discounts, PublicCustomerInfos.CustomerId);
                        }

                        if (discounts != null)
                        {
                            actualPrice = Helper.CalculateDiscount(discounts, actualPrice, 1);
                            if (!normalPrice.Equals(actualPrice))
                            {
                                priceMsrp.Visible = true;
                                msrpSpan.Visible = true;
                            }
                        }
                    }

                    price.Text = Helper.FormatPrice(actualPrice);

                    // Hotel type
                    type.Text = ((Enums.Hoteltype)(product.Hotels.HoteltypeId ?? 0)).ToString();
                    var distanceAway = Math.Round(product.DistanceWithUser, 1);
                    maway.Text = string.Format(Math.Abs(distanceAway % 1) > 0.0 ? "{0:0.#} miles away" : "{0:0} miles away", distanceAway);

                    // Comming Soon
                    if (product.Hotels.IsComingSoon.HasValue && product.Hotels.IsComingSoon.Value)
                    {
                        price.Visible = false;
                        passleftDiv.Visible = true;
                        litPassleft.Text = Constant.CommingSoonString;
                        msrpSpan.Visible = false;
                        priceMsrp.Visible = false;
                        maxGuestControl.Visible = false;
                    }
                    else if (isOnBlackOutDay && StartDate.Date < nextAvailableDate.Date)
                    {
                        passleftDiv.Visible = true;
                        litPassleft.Text = nextAvailableDay;
                    }

                    hotelName.InnerText = product.Hotels.HotelName;
                    //if (product.Hotels.HotelName.Length > 25)
                    //{
                    //    hotelName.Attributes["class"] += " multiline";
                    //    if (Request.Browser["IsMobileDevice"] == "true")
                    //    {
                    //        titleBottom.Attributes["class"] += " multiline";
                    //        roomTitle.Attributes["class"] += " multiline";
                    //    }
                    //}

                    roomTitle.Attributes["onclick"] = string.Format("f{0}(event)", product.ProductId);
                }
            }
        }

        private void BindMetaHeader(List<Products> listProducts)
        {
            string strMeta = "<meta name=\"description\" content=\"{0}\" /> " +
                             "<meta name=\"keywords\" content=\"{1}\" /> " +
                             "<link rel=\"canonical\" href=\"{2}\" />";

            strMeta = string.Format(strMeta,
                string.Format(Constant.SearchMetaDescription, MarketSearch != null ? MarketSearch.LocationName : string.Empty),
                string.Format(Constant.SearchMetaKeywords,
                    MarketSearch != null ? MarketSearch.LocationName : string.Empty,
                    String.Join(", ", listProducts.Select(GetProductNameWithType))),
                Request.Url.AbsoluteUri);

            var metaControl = Page.ParseControl(strMeta);
            if (metaControl != null)
            {
                MetaPlaceHolder.Controls.Add(metaControl);
            }
        }

        private string GetProductNameWithType(Products product)
        {
            string str = product.ProductName;

            switch (product.ProductType)
            {
                case (int)Enums.ProductType.DayPass:
                    str = string.Format("{0} -{1}", str, Constant.DayPassString);
                    break;
                case (int)Enums.ProductType.Cabana:
                    str = string.Format("{0} -{1}", str, Constant.CabanasPassString);
                    break;
                case (int)Enums.ProductType.Daybed:
                    str = string.Format("{0} -{1}", str, Constant.DaybedsString);
                    break;
                case (int)Enums.ProductType.SpaPass:
                    str = string.Format("{0} -{1}", str, Constant.SpaPassString);
                    break;
            }

            return str;
        }

        private void CheckShowAuth()
        {
            var multiView = (MultiView)AuthControl.FindControl("AuthMultiView");
            if (Request.Params["sp"] != null && 
                (Session["UserSession"] == null || (Session["UserSession"] != null && string.Equals(Request.Params["sp"], (string)Session["UserSession"], StringComparison.OrdinalIgnoreCase))) && 
                multiView != null)
            {
                ShowAuth = true;
                multiView.ActiveViewIndex = 3;
            }
            if (Request.Params["reg"] != null
                && string.Equals(Request.Params["reg"], "true", StringComparison.OrdinalIgnoreCase)
                && Session["UserSession"] == null && multiView != null)
            {
                multiView.ActiveViewIndex = 0;
                ShowAuth = true;
            }
        }

        private string GetStrPassByProductType()
        {
            _productTypeSearch.ForEach(productType =>
            {
                switch (productType)
                {
                    case Enums.ProductType.DayPass:
                        StrPassSearch += string.Format(" | {0}", Constant.DayPassString);
                        break;
                    case Enums.ProductType.Cabana:
                        StrPassSearch += string.Format(" | {0}", Constant.CabanasPassString);
                        break;
                    case Enums.ProductType.Daybed:
                        StrPassSearch += string.Format(" | {0}", Constant.DaybedsString);
                        break;
                    case Enums.ProductType.SpaPass:
                        StrPassSearch += string.Format(" | {0}", Constant.SpaPassString);
                        break;
                }
            });
                
            return StrPassSearch;
        }

        #region Support Seach Region

        private string GetUserRequestLocation()
        {
            UserIp = DayaxeDal.Ultility.Ip.GetIpAddress();
            if (Session[UserIp] != null)
            {
                return Session[UserIp].ToString();
            }
            using (var reader = new DatabaseReader(Server.MapPath("/App_Data/GeoLite2-City.mmdb")))
            {
                if (UserIp == "::1")
                {
                    UserIp = "134.201.250.155"; // Los Angeles IP Address
                }
                CityResponse response;
                if (reader.TryCity(UserIp, out response))
                {
                    Session[UserIp] = response.City.Name;
                    return response.City.Name;
                }
                return string.Empty;
            }
        }
        
        private void Initialize()
        {
            StrPassSearch = GetStrPassByProductType();

            // Check-In Date

            // Guest
            if (Session["Guest"] != null)
            {
                int totalGuest;
                int.TryParse(Session["Guest"].ToString(), out totalGuest);
                TotalGuest = totalGuest;
            }

            SetSearchDate(CheckInDateText.Text);

            if (!string.IsNullOrEmpty(HidGuest.Text))
            {
                TotalGuest = int.Parse(HidGuest.Text);
            }

            CurrentGuest.Text = string.Format("{0} guest(s)", TotalGuest);

            Session["Guest"] = HidGuest.Text;

            SetSearchPrice(PriceText.Text);

            SetSearchDistance(DistanceText.Text);
        }

        private void SetSearchDate(string strDate)
        {
            if (!string.IsNullOrEmpty(strDate) && strDate.IndexOf('-') != -1)
            {
                var dates = strDate.Split('-');
                DateTime currentDate = Helper.ParseInThisYearOrNextYear(dates[0].Trim());
                if (currentDate < DateTime.UtcNow.ToLosAngerlesTime().Date)
                {
                    currentDate = DateTime.UtcNow.ToLosAngerlesTime().Date;
                }
                StartDate = currentDate;
                Session["CheckInDateSearch"] = currentDate.ToString(Constant.DiscountDateFormat);

                currentDate = Helper.ParseInThisYearOrNextYear(dates[1].Trim());
                if (currentDate < DateTime.UtcNow.ToLosAngerlesTime().Date && currentDate.Date < StartDate.Date)
                {
                    currentDate = DateTime.UtcNow.ToLosAngerlesTime().Date.AddDays(15);
                }
                EndDate = currentDate;
                strDate = string.Format("{0:ddd, MMM dd} - {1:ddd, MMM dd}", StartDate, EndDate);

                Session["SearchRange"] = strDate;
            }
        }

        private void SetSearchPrice(string strPrice)
        {
            if (!string.IsNullOrEmpty(strPrice))
            {
                var prices = strPrice.Replace("[", "").Replace("]", "").Split(',');

                double price;
                double.TryParse(prices[0], out price);
                LowPrice = price;

                double.TryParse(prices[1], out price);
                HighPrice = price;
            }
        }

        private void SetSearchDistance(string strDistance)
        {
            if (!string.IsNullOrEmpty(strDistance))
            {
                var distances = strDistance.Replace("[", "").Replace("]", "").Split(',');

                int distance;
                int.TryParse(distances[0], out distance);
                LowDistance = distance;

                int.TryParse(distances[1], out distance);
                HighDistance = distance;
            }
        }

        private SearchResult GetSearchResult()
        {
            if (SearchParamsSaved == null)
            {
                SearchParamsSaved = new SearchParams
                {
                    ProductType = _productTypeSearch
                };
            }
            SearchParamsSaved.LowPrice = LowPrice;
            SearchParamsSaved.HighPrice = HighPrice;
            SearchParamsSaved.LowDistance = LowDistance;
            SearchParamsSaved.HighDistance = HighDistance;
            SearchParamsSaved.CustomerId = CustomerIdSearch;
            SearchParamsSaved.SearchMarkets = MarketSearch;
            SearchParamsSaved.AvailableTickets = TotalGuest;
            SearchParamsSaved.StartDate = StartDate;
            SearchParamsSaved.EndDate = EndDate;
            SearchParamsSaved.IsForceResetFilter = IsForceResetFilter;

            var result = _productRepository.SearchProducts(SearchParamsSaved);

            PriceText.Attributes["data-slider-min"] = result.MinPrice.ToString("0");
            PriceText.Attributes["data-slider-max"] = result.MaxPrice.ToString("0");
            DistanceText.Attributes["data-slider-min"] = result.MinDistance.ToString("0");
            DistanceText.Attributes["data-slider-max"] = result.MaxDistance.ToString("0");

            if (!IsPostBack)
            {
                PriceText.Text = string.Format("[{0:0},{1:0}]", result.MinPrice, result.MaxPrice);
                DistanceText.Text = string.Format("[{0:0},{1:0}]", result.MinDistance, result.MaxDistance);
                Session["Price"] = string.Format("[{0:0},{1:0}]", result.MinPrice, result.MaxPrice);
                Session["Distance"] = string.Format("[{0:0},{1:0}]", result.MinDistance, result.MaxDistance);
            }

            if (result.IsResetFilter)
            {
                ScriptManager.RegisterClientScriptBlock(HotelList, typeof(string), Guid.NewGuid().ToString(),
                    string.Format("$(function() {{ resetFilter({0:0}, {1:0}); }});",
                        result.MinDistance,
                        result.MaxDistance), true);
            }
            return result;
        }

        #endregion

        #region Search Region

        protected void CheckInDateText_OnTextChanged(object sender, EventArgs e)
        {
            
        }

        protected void HidGuest_OnTextChanged(object sender, EventArgs e)
        {
            
        }

        protected void ClearAllLinkButtonOnClick(object sender, EventArgs e)
        {
            _productTypeSearch = new List<Enums.ProductType>
            {
                Enums.ProductType.DayPass,
                Enums.ProductType.Cabana,
                Enums.ProductType.Daybed,
                Enums.ProductType.SpaPass
            };
            SearchParamsSaved = null;
            if (!DaypassLink.CssClass.Contains("active"))
                DaypassLink.CssClass += " active";
            if (!CabanasLink.CssClass.Contains("active"))
                CabanasLink.CssClass += " active";
            if (!DaybedsLink.CssClass.Contains("active"))
                DaybedsLink.CssClass += " active";
            if (!SpapassLink.CssClass.Contains("active"))
                SpapassLink.CssClass += " active";
            RebindSearch();
        }

        protected void DaypassLinkOnClick(object sender, EventArgs e)
        {
            RebindSearch(Enums.ProductType.DayPass);
        }

        protected void CabanasLinkOnClick(object sender, EventArgs e)
        {
            RebindSearch(Enums.ProductType.Cabana);
        }

        protected void DaybedsLinkOnClick(object sender, EventArgs e)
        {
            RebindSearch(Enums.ProductType.Daybed);
        }

        protected void SpapassLinkOnClick(object sender, EventArgs e)
        {
            RebindSearch(Enums.ProductType.SpaPass);
        }

        private void RebindSearch(Enums.ProductType? productType = null)
        {
            if (SearchParamsSaved != null && productType != null)
            {
                if (SearchParamsSaved.ProductType.Contains(productType.Value))
                {
                    SearchParamsSaved.ProductType.Remove(productType.Value);
                    switch (productType)
                    {
                        case Enums.ProductType.DayPass:
                            DaypassLink.CssClass = DaypassLink.CssClass.Replace("active", "");
                            break;
                        case Enums.ProductType.Cabana:
                            CabanasLink.CssClass = CabanasLink.CssClass.Replace("active", "");
                            break;
                        case Enums.ProductType.Daybed:
                            DaybedsLink.CssClass = DaybedsLink.CssClass.Replace("active", "");
                            break;
                        case Enums.ProductType.SpaPass:
                            SpapassLink.CssClass = SpapassLink.CssClass.Replace("active", "");
                            break;
                    }
                }
                else
                {
                    SearchParamsSaved.ProductType.Add(productType.Value);
                    switch (productType)
                    {
                        case Enums.ProductType.DayPass:
                            DaypassLink.CssClass += " active";
                            break;
                        case Enums.ProductType.Cabana:
                            CabanasLink.CssClass += " active";
                            break;
                        case Enums.ProductType.Daybed:
                            DaybedsLink.CssClass += " active";
                            break;
                        case Enums.ProductType.SpaPass:
                            SpapassLink.CssClass += " active";
                            break;
                    }
                }
            }

            // ReSharper disable once NotAccessedVariable
            List<Products> listProducts;
            BindHotelSearch(out listProducts);
        }

        protected void ClearAmenitiesOnClick(object sender, EventArgs e)
        {
            _productTypeSearch = new List<Enums.ProductType>
            {
                Enums.ProductType.DayPass,
                Enums.ProductType.Cabana,
                Enums.ProductType.Daybed,
                Enums.ProductType.SpaPass
            };
            SearchParamsSaved = null;
            if (!DaypassLink.CssClass.Contains("active"))
                DaypassLink.CssClass += " active";
            if (!CabanasLink.CssClass.Contains("active"))
                CabanasLink.CssClass += " active";
            if (!DaybedsLink.CssClass.Contains("active"))
                DaybedsLink.CssClass += " active";
            if (!SpapassLink.CssClass.Contains("active"))
                SpapassLink.CssClass += " active";
            RebindSearch();
        }

        #endregion
    }
}