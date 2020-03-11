<%@ Application Language="C#" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="AutoMapper" %>
<%@ Import Namespace="DayaxeDal" %>
<%@ Import Namespace="DayaxeDal.Custom" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>

<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);
        BundleTable.EnableOptimizations = false;

        // config Auto Mapper
        Mapper.Initialize(cfg => {
            cfg.CreateMap<BlockedDatesCustomPrice, CalendarObject>()
                .ForMember(dest => dest.SelectedDate, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Capacity.HasValue ? src.Capacity.Value : 0))
                .ForMember(dest => dest.RegularPrice, opt => opt.MapFrom(src => src.RegularPrice));

            cfg.CreateMap<Products, DefaultPriceObject>();

            cfg.CreateMap<Products, DefaultPrices>();

            cfg.CreateMap<DefaultPrices, DefaultPriceObject>();

            cfg.CreateMap<InportDiscountObject, Discounts>()
                .ForMember(dest => dest.DiscountName, opt => opt.MapFrom(src => src.DiscountName))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.CodeRequired, opt => opt.MapFrom(src => src.CodeRequired))
                .ForMember(dest => dest.PromoType, opt => opt.MapFrom(src => src.PromoType == "Fixed Amount" ? 1 : 0))
                .ForMember(dest => dest.MinAmount, opt => opt.ResolveUsing(b =>
                {
                    double amount;
                    double.TryParse(b.MinAmount.Replace("$", ""), NumberStyles.None, null, out amount);
                    return amount;
                }))
                .ForMember(dest => dest.IsAllProducts, opt => opt.MapFrom(src => src.IsAllProduct))
                .ForMember(dest => dest.MaxPurchases, opt => opt.MapFrom(src => src.MaxPurchases))
                .ForMember(dest => dest.IsDelete, opt => opt.UseValue(false))
                .ForMember(dest => dest.PercentOff, opt => opt.ResolveUsing(b =>
                {
                    double amount;
                    double.TryParse(b.MinAmount.Replace("$", ""), NumberStyles.None, null, out amount);
                    return amount;
                }));

            cfg.CreateMap<Bookings, ExportBookingObject>()
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
                .ForMember(dest => dest.BookingCode, opt => opt.MapFrom(src => src.BookingCode))
                .ForMember(dest => dest.PassStatus, opt => opt.ResolveUsing(b =>
                {
                    return ((Enums.BookingStatus) b.PassStatus).ToString();
                }))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.GuestName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.BookedDate, opt => opt.ResolveUsing(b =>
                {
                    return b.BookedDate.ToString(Constant.DiscountDateFormat);
                }))
                .ForMember(dest => dest.CheckInDate, opt => opt.ResolveUsing(b =>
                {
                    if (b.PassStatus == (int) Enums.BookingStatus.Redeemed)
                    {
                        return b.RedeemedDate.HasValue ? b.RedeemedDate.Value.ToLosAngerlesTimeWithTimeZone(b.TimeZoneId).ToString(Constant.DiscountDateTimeFormat) : string.Empty;
                    }
                    return b.CheckinDate.HasValue ? b.CheckinDate.Value.ToString(Constant.DiscountDateFormat) : string.Empty;
                }))
                .ForMember(dest => dest.RedeemedDate, opt => opt.ResolveUsing(b =>
                {
                    return b.RedeemedDate.HasValue ? b.RedeemedDate.Value.ToLosAngerlesTimeWithTimeZone(b.TimeZoneId).ToString(Constant.DiscountDateTimeFormat) : string.Empty;
                }))
                .ForMember(dest => dest.TicketQuantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.PerTicketPrice, opt => opt.MapFrom(src => src.MerchantPrice))
                .ForMember(dest => dest.GrossEarnings, opt => opt.MapFrom(src => src.MerchantPrice * src.Quantity))
                .ForMember(dest => dest.Rating, opt => opt.ResolveUsing(b =>
                {
                    return b.UserRating > 0 ? b.UserRating.ToString() : string.Empty;
                }))
                .ForMember(dest => dest.Feedback, opt => opt.MapFrom(src => src.SurveyFeedback))
                .ForMember(dest => dest.EstSpend, opt => opt.ResolveUsing(b =>
                {
                    return b.UserRating > 0 ? b.EstSpend.ToString() : string.Empty;
                }))
                .ForMember(dest => dest.Paid, opt => opt.ResolveUsing(b =>
                {
                    return b.PassStatus == (int)Enums.BookingStatus.Redeemed ? "1" : "0";
                }));
        });
    }

    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }

    void Application_Error(object sender, EventArgs e)
    {
        //try
        //{
        //    Exception exception = Server.GetLastError();
        //    HttpException httpException = (HttpException)exception;
        //    int httpCode = httpException.GetHttpCode();

        //    switch (httpCode)
        //    {
        //        case 400:
        //            Response.Redirect("/ErrorPages/conf-error.aspx");
        //            break;
        //        case 401:
        //        case 403:
        //            Response.Redirect("/ErrorPages/unauthorized.aspx");
        //            break;
        //        case 404:
        //            Response.Redirect("/ErrorPages/resource-not-found.aspx");
        //            break;
        //        default:
        //            Response.Redirect("/ErrorPages/conf-error.aspx");
        //            break;
        //    }
        //}
        //catch { }
    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started
        Response.Redirect(Constant.DefaultPage);
    }

</script>
